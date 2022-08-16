namespace LocksmithD
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Locksmith.Core;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            var etcdConn = config.GetSection("EtcdConnection").Get<EtcdConnectionOptions>();
            var lockOptions = config.GetSection("Lock").Get<LockOptions>();

            var (lck, error) = SetupLock(etcdConn, lockOptions);
            if (error != null)
            {
                Console.Error.WriteLine($"Failed to set up lock: {error}");
                return 1;
            }

            error = await UnlockIfHeld(lck);
            if (error != null)
            {
                Console.Error.WriteLine($"Failed to unlock held lock: {error}");
                return 1;
            }


            Console.WriteLine("Press enter to aquire lock.");
            Console.ReadLine();

            await WaitForLock(lck);
            return 0;
        }

        // lockAndReboot attempts to acquire the lock and reboot the machine in an
        // infinite loop. Returns if the reboot failed.
        private static async Task WaitForLock(IDistributedLock lck, CancellationToken cancellationToken = default)
        {
            var interval = TimeSpan.FromSeconds(5);
       
            while (true)
            {
                var err = await lck.Lock(cancellationToken);
                if (err != null && err != Errors.ErrExist)
                {
                    //interval = expBackoff(interval);
                    Console.WriteLine($"Failed to acquire lock: {err}. Retrying in {interval}.");

                    await Task.Delay(interval);
                    continue;
                }

                return;
            }
        }

        private static async Task<Error> UnlockIfHeld(IDistributedLock lck, CancellationToken cancellationToken = default)
        {
            var err = await lck.Unlock(cancellationToken);
            if (err == Errors.ErrNotExist)
            {
                return null;
            }
            else if (err == null)
            {
                Console.WriteLine("Unlocked existing lock for this machine");
                return null;
            }

            return err;
        }

        private static (IDistributedLock, Error) SetupLock(EtcdConnectionOptions etcdConn, LockOptions lockOptions)
        {
            if (etcdConn is null)
            {
                throw new ArgumentNullException(nameof(etcdConn));
            }

            var mID = MachineId.GetMachineId();
            if (string.IsNullOrEmpty(mID))
            {
                return (null, new Error("Cannot read machine-id"));
            }

            var l = new DistributedLock(etcdConn.Endpoint, mID, lockOptions.Group);

            return (l, null);
        }
    }
}
