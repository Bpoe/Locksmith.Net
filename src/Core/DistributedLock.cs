namespace Locksmith.Core
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using dotnet_etcd;
    using dotnet_etcd.interfaces;
    using Etcdserverpb;
    using Newtonsoft.Json;

    public class DistributedLock : IDistributedLock
    {
        private const string keyPrefix = "coreos.com/updateengine/rebootlock";
        private const string groupBranch = "groups";
        private const string semaphoreBranch = "semaphore";

        // SemaphorePrefix is the key in etcd where the semaphore will be stored
        public const string SemaphorePrefix = keyPrefix + "/" + semaphoreBranch;

        private readonly IEtcdClient etcdClient;
        private readonly string machineId;
        private readonly string keyPath;

        public DistributedLock(string endpoint, string machineId, string group)
        {
            this.machineId = machineId;
            this.keyPath = SemaphorePrefix;

            if (!string.IsNullOrEmpty(group))
            {
                this.keyPath = Path.Join(keyPrefix, groupBranch, Uri.EscapeDataString(group), semaphoreBranch);
            }

            this.etcdClient = new EtcdClient(endpoint);
            this.Init().Wait();
        }

        public async Task<(Semaphore, Error)> Get(CancellationToken cancellationToken = default)
        {
            var response = await this.etcdClient.GetAsync(this.keyPath, cancellationToken: cancellationToken);
            if (response.Count == 0)
            {
                return (null, new Error("key not found"));
            }

            var key = response.Kvs[0];
            var s = key.Value.ToStringUtf8().Trim();

            var sem = JsonConvert.DeserializeObject<Semaphore>(s);
            sem.Version = key.Version;

            return (sem, null);
        }

        public async Task<(Semaphore, int oldMax, Error)> SetMax(int max, CancellationToken cancellationToken = default)
        {
            var oldMax = -1;
            Semaphore sem = null;

            var error = await this.Store(s =>
                {
                    oldMax = s.Max;
                    sem = s;
                    s.SetMax(max);
                    return null;
                },
                cancellationToken);

            return (sem, oldMax, error);
        }

        public async Task<Error> Lock(CancellationToken cancellationToken)
        {
            return await this.Store(s => s.Lock(this.machineId), cancellationToken: cancellationToken);
        }

        public async Task<Error> Unlock(CancellationToken cancellationToken = default)
        {
            return await this.Store(s => s.Unlock(this.machineId), cancellationToken: cancellationToken);
        }

        private async Task Init(CancellationToken cancellationToken = default)
        {
            var semaphore = new Semaphore();
            var json = JsonConvert.SerializeObject(semaphore);

            var tx = new TxnRequest();
            tx.Compare.Add(TxnExtensions.CompareVersion(this.keyPath, Compare.Types.CompareResult.Equal, 0));
            tx.Success.Add(TxnExtensions.Put(this.keyPath, json));

            var response = await this.etcdClient.TransactionAsync(tx, cancellationToken: cancellationToken);
        }

        private async Task<Error> Set(Semaphore semaphore, CancellationToken cancellationToken = default)
        {
            if (semaphore == null)
            {
                return new Error("cannot set null semaphore");
            }

            var json = JsonConvert.SerializeObject(semaphore);

            var tx = new TxnRequest();
            tx.Compare.Add(TxnExtensions.CompareVersion(this.keyPath, Compare.Types.CompareResult.Equal, semaphore.Version));
            tx.Success.Add(TxnExtensions.Put(this.keyPath, json));

            var response = await this.etcdClient.TransactionAsync(tx, cancellationToken: cancellationToken);

            return response.Succeeded ? null : new Error("update failed");
        }

        private async Task<Error> Store(Func<Semaphore, Error> func, CancellationToken cancellationToken = default)
        {
            var (semaphore, error) = await this.Get(cancellationToken);
            if (error != null)
            {
                return error;
            }

            error = func(semaphore);
            if (error != null)
            {
                return error;
            }

            return await this.Set(semaphore, cancellationToken: cancellationToken);
        }
    }
}
