namespace Locksmith.Ctl
{
    using Locksmith.Core;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class SetMax
    {
        public static readonly Command Command = new Command
        {
            Name = "set-max",
            Summary = "Set the maximum number of lock holders",
            Usage = "UNIT",
            Description = @"Set the maximum number of machines that can be rebooting at a given time. This 
    can be set at any time and will not affect current holders of the lock.",
            Run = SetMax.RunSetMax,
        };

        public static async Task<int> RunSetMax(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("New maximum value must be set.");
                return 1;
            }

            var l = new DistributedLock(Context.Options.Endpoints[0], string.Empty, string.Empty);

            var max = Convert.ToInt32(args[0]);

            var (semaphore, old, error) = await l.SetMax(max, CancellationToken.None);

            if (error != null)
            {
                Console.Error.WriteLine($"Error setting value: {error}");
            }

            Console.WriteLine($"Old-Max: {old}");
            Console.WriteLine($"Max: {semaphore.Max}");

            return 0;
        }
    }
}
