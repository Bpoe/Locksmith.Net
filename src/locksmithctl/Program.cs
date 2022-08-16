namespace Locksmith.Ctl
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
	using Microsoft.Extensions.Configuration;

	public class Program
    {
		public static async Task<int> Main(string[] args)
        {
			var configuration = new ConfigurationBuilder()
				.AddCommandLine(args)
				.Build();

			configuration.Bind(Context.Options);
			if (!string.IsNullOrWhiteSpace(Context.Options.Endpoint))
            {
				Context.Options.Endpoints = Context.Options.Endpoint.Split(",");
            }

			var progName = AppDomain.CurrentDomain.FriendlyName;

			if (args.Length < 1)
            {
				args = new[] { "help" };
			}

			var cmd = Context.Commands
				.FirstOrDefault(c => c.Name.Equals(args[0], StringComparison.InvariantCultureIgnoreCase));

			if (cmd is null)
			{
				Console.WriteLine($"{Context.cliName}: unknown subcommand: {args[0]}\n");
				Console.WriteLine($"Run '{Context.cliName} help' for usage.\n");
				return 2;
			}

			return await cmd.Run(args[1..]);
		}
	}
}
