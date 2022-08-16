namespace Locksmith.Ctl
{
    using Locksmith.Core;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Unlock
    {
        public static readonly Command Command = new Command
        {
            Name = "unlock",
            Summary = "Unlock this machine or a given machine-id for reboot.",
            Usage = "<machine-id>",
            Description = @"Unlock is for manual unlocking of the reboot unlock for this machine or a
    given machine-id.Under normal operation this should not be necessary.",
            Run = Unlock.RunUnlock,
        };

        public static async Task<int> RunUnlock(string[] args)
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
            var error = await l.Unlock(CancellationToken.None);

            if (error != null)
            {
                Console.Error.WriteLine($"Error unlocking: {error}");
                return 1;
            }

            return 0;
        }
    }
}
