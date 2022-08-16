namespace Locksmith.Core
{
    using System;
    using System.Management;

    public static class MachineId
    {
		public static string GetMachineId()
        {
			var mc = new ManagementClass("Win32_ComputerSystemProduct");
			var instances = mc.GetInstances();
			foreach (var i in instances)
            {
				return (string)i["UUID"];
			}

			return Environment.MachineName;
		}
    }
}
