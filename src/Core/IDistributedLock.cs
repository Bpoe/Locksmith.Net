namespace Locksmith.Core
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDistributedLock
    {
        Task<Error> Lock(CancellationToken cancellationToken = default);

        Task<Error> Unlock(CancellationToken cancellationToken = default);

        Task<(Semaphore, Error)> Get(CancellationToken cancellationToken = default);

        Task<(Semaphore, int oldMax, Error)> SetMax(int max, CancellationToken cancellationToken = default);
    }
}