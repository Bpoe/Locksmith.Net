namespace Locksmith.Ctl
{
    using Locksmith.Core;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Lock
    {
        public static readonly Command Command = new Command
        {
            Name = "lock",
            Summary = "Lock this machine or a given machine-id for reboot.",
            Usage = "<machine-id>",
            Description = @"Lock is for manual locking of the reboot lock for this machine or a given
    machine-id. Under normal operation this should not be necessary.",
            Run = Lock.RunLock,
        };

        public static async Task<int> RunLock(string[] args)
        {
            string mID;

            if (args.Length == 0)
            {
                mID = MachineId.GetMachineId();
                if (string.IsNullOrEmpty(mID))
                {
                    Console.Error.WriteLine("Cannot read machine-id");
                    return 1;
                }
            }
            else
            {
                mID = args[0];
            }

            var l = new DistributedLock(Context.Options.Endpoints[0], mID, string.Empty);
            var error = await l.Lock(CancellationToken.None);

            if (error != null)
            {
                Console.Error.WriteLine($"Error locking: {error}");
                return 1;
            }

            return 0;
        }
    }
}
