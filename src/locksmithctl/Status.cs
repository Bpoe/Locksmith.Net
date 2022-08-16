namespace Locksmith.Ctl
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Locksmith.Core;

    public static class Status
    {
        public static readonly Command Command = new Command
        {
            Name = "status",
            Summary = "Get the status of the cluster wide reboot lock.",
            Description = "Status will return the number of locks that are held and available and a list of the holders.",
            Run = Status.RunStatus,
        };

        public static async Task<int> RunStatus(string[] args)
        {
            var l = new DistributedLock(Context.Options.Endpoints[0], string.Empty, string.Empty);

            var (semaphore, error) = await l.Get(CancellationToken.None);
            if (error != null)
            {
                Console.Error.WriteLine($"Error getting value: {error}");
                return 1;
            }

            Console.WriteLine($"Available: {semaphore.Value}");
            Console.WriteLine($"Max: {semaphore.Max}");

            if (semaphore.Holders.Count == 0)
            {
                return 0;
            }

            Console.WriteLine();
            Console.WriteLine("MACHINE ID");
            foreach (var h in semaphore.Holders)
            {
                Console.WriteLine(h);
            }

            return 0;
        }
    }
}
