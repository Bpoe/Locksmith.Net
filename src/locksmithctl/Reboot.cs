namespace Locksmith.Ctl
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Locksmith.Core;

    public static class Reboot
    {
        public static readonly Command Command = new Command
        {
            Name = "reboot",
            Summary = "Reboot honoring reboot locks.",
            Description = "Reboot will attempt to reboot immediately after taking a reboot lock. The user is responsible for unlocking after a successful reboot.",
            Run = RunReboot,
        };

        public static async Task<int> RunReboot(string[] args)
        {
            var mID = MachineId.GetMachineId();
            if (string.IsNullOrEmpty(mID))
            {
                Console.Error.WriteLine("Cannot read machine-id");
                return 1;
            }

            var l = new DistributedLock(Context.Options.Endpoints[0], mID, string.Empty);
            var error = await l.Lock(CancellationToken.None);
            if (error != null)
            {
                Console.Error.WriteLine("Error locking: {0}", error);
                return 1;
            }

            NativeMethods.Reboot();

            return 0;
        }
    }
}
