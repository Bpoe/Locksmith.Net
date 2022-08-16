namespace Locksmith.Ctl
{
	using Microsoft.Extensions.Configuration;

    public class GlobalFlags
    {
		public bool Debug { get; set; } = false;

		[ConfigurationKeyName("endpoint")]
		public string Endpoint { get; set; } = string.Empty;

		public string[] Endpoints { get; set; } = new[] { "http://127.0.0.1:2379", "http://127.0.0.1:4001", };

		[ConfigurationKeyName("etcd-keyfile")]
		public string EtcdKeyFile { get; set; } = string.Empty;

		[ConfigurationKeyName("etcd-certfile")]
		public string EtcdCertFile { get; set; } = string.Empty;

		[ConfigurationKeyName("etcd-cafile")]
		public string EtcdCAFile { get; set; } = string.Empty;

		[ConfigurationKeyName("etcd-username")]
		public string EtcdUsername { get; set; } = string.Empty;

		[ConfigurationKeyName("etcd-password")]
		public string EtcdPassword { get; set; } = string.Empty;

		public string Group { get; set; } = string.Empty;

		public bool Version { get; set; } = false;
	}
}
