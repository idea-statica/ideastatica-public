using Microsoft.Win32;
using System;

namespace IdeaStatiCa.RamContracts
{
	public static class RamRegistryHelper
	{
		public static bool CheckRegistry(string checkedVersion)
		{
			var bentleyProductsKey = @"SOFTWARE\Bentley\Installed_Products";

			using var baseKey = Registry.LocalMachine.OpenSubKey(bentleyProductsKey);
			if (baseKey == null)
			{
				return false;
			}

			foreach (var subKeyName in baseKey.GetSubKeyNames())
			{
				using var productKey = baseKey.OpenSubKey(subKeyName);
				if (productKey == null)
				{
					continue;
				}

				var productName = productKey.GetValue("ProductName") as string;

				// RAM Structural System
				if (string.Equals(productName, "RAMSS",
								  StringComparison.OrdinalIgnoreCase))
				{
					var version = productKey.GetValue("Version") as string;
					if (version is { } && version.StartsWith(checkedVersion))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
