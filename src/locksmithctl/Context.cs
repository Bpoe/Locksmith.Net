namespace Locksmith.Ctl
{
    using Locksmith.Core;
    using Locksmith.Core.Flag;

    public static class Context
    {
        public const string cliName = "locksmithctl";

        public const string cliDescription = "locksmithctl";

        public static readonly Command[] Commands = new[]
{
            Unlock.Command,
            Status.Command,
            SetMax.Command,
            Lock.Command,
            Help.Command,
            Reboot.Command,
        };

        public static readonly GlobalFlags Options = new GlobalFlags();

        public static readonly Flag[] GlobalFlags = new[]
        {
            new Flag { Name = "debug", DefaultValue = false, Usage = "Print out debug information to stderr." },
            new Flag { Name = "endpoint", Usage = "etcd endpoint for locksmith. Specify multiple times to use multiple endpoints." },
            new Flag { Name = "etcd-keyfile", DefaultValue = "", Usage = "etcd key file authentication" },
            new Flag { Name = "etcd-certfile", DefaultValue = "", Usage = "etcd cert file authentication" },
            new Flag { Name = "etcd-cafile", DefaultValue = "", Usage = "etcd CA file authentication" },
            new Flag { Name = "etcd-username", DefaultValue = "", Usage = "username for secure etcd communication" },
            new Flag { Name = "etcd-password", DefaultValue = "", Usage = "password for secure etcd communication" },
            new Flag { Name = "group", DefaultValue = "", Usage = "locksmith group" },
            new Flag { Name = "version", DefaultValue = false, Usage = "Print the version and exit." },
        };

        public static readonly FlagSet GlobalFlagSet = new FlagSet("locksmithctl", ErrorHandling.ExitOnError);
    }
}
