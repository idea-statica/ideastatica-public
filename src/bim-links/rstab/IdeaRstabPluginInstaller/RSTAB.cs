using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RstabToIdeaPluginInstall
{
	internal class RSTAB
	{
		private const string RFEMRegistryKey = @"SOFTWARE\DLUBAL\RSTAB";

		public static RSTAB GetLatestVersion()
		{
			List<(Version, string)> installedVersions = new List<(Version, string)>();

			using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = hklm.OpenSubKey(RFEMRegistryKey))
				{
					if (key == null)
					{
						return null;
					}

					foreach (string subKeyName in key.GetSubKeyNames())
					{
						string installPath = GetInstallPath(hklm, Path.Combine(RFEMRegistryKey, subKeyName, "64-bit"));
						if (installPath != null && Directory.Exists(installPath))
						{
							installedVersions.Add((new Version(subKeyName), installPath));
						}
					}
				}
			}

			if (installedVersions.Count == 0)
			{
				return null;
			}

			(Version, string) latestVersion = installedVersions
				.OrderByDescending(x => x.Item1)
				.First();

			return new RSTAB(latestVersion.Item2);
		}

		public RSTABConfig Config
		{
			get
			{
				if (_config == null)
				{
					_config = new RSTABConfig(Path.Combine(_installPath, "RSTAB.ini"));
				}

				return _config;
			}
		}

		private RSTABConfig _config;
		private readonly string _installPath;

		private RSTAB(string installPath)
		{
			_installPath = installPath;
		}

		private static string GetInstallPath(RegistryKey hklm, string key)
		{
			using (RegistryKey registryKey = hklm.OpenSubKey(key))
			{
				if (registryKey == null)
				{
					return null;
				}

				return registryKey.GetValue("InstallDirectory") as string;
			}
		}
	}
}