using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

#if !NET48
using System.Text.Json;
#endif

namespace IdeaStatiCa.PluginsTools.Windows.Utilities
{
	/// <summary>
	/// Tools for getting information about installation of IdeaStatiCa on the local machine
	/// </summary>
	public static class IdeaStatiCaSetupTools
	{
		public const string IdeaStatiCaRegistryKey = @"SOFTWARE\IDEAStatiCa";

		/// <summary>
		/// Get the installation folder for IdeaStatiCa <paramref name="version"/>
		/// </summary>
		/// <param name="version">Required version</param>
		/// <exception cref="System.Exception">Exception is thrown if <paramref name="version"/> is not installed.</exception>
		/// <returns>Path to the installation directory</returns>
		public static string GetIdeaStatiCaInstallDir(string version)
		{
			string installPath = string.Empty;

			using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = hklm.OpenSubKey(IdeaStatiCaRegistryKey))
				{
					if (key == null)
					{
						return null;
					}

					foreach (string subKeyName in key.GetSubKeyNames())
					{
						if (version.Equals(subKeyName, StringComparison.InvariantCultureIgnoreCase))
						{
							installPath = GetInstallPath(hklm, Path.Combine(IdeaStatiCaRegistryKey, subKeyName, "IDEAStatiCa", "Designer"));
							break;
						}
					}
				}
			}

			if (string.IsNullOrEmpty(installPath))
			{
				throw new Exception($"IdeaStatica v ${version} is not installed correctly. ");
			}

			return installPath;
		}


#if !NET48
		/// <summary>
		/// get list of installed IS versions in JSON format
		/// </summary>
		/// <returns></returns>
		public static string GetInstalledVersions()
		{
			List<IdeaStatiCaVer> versions = null;

			using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey key = hklm.OpenSubKey(IdeaStatiCaRegistryKey))
				{
					if (key == null)
					{
						return null;
					}

					foreach (string subKeyName in key.GetSubKeyNames())
					{
						var ver = new IdeaStatiCaVer();
						ver.Version = subKeyName;
						ver.InstallPath = GetInstallPath(hklm, Path.Combine(IdeaStatiCaRegistryKey, subKeyName, "IDEAStatiCa", "Designer"));
					}
				}
			}

			return JsonSerializer.Serialize(versions);
		}
#endif

		private static string GetInstallPath(RegistryKey hklm, string key)
		{
			using (RegistryKey registryKey = hklm.OpenSubKey(key))
			{
				if (registryKey == null)
				{
					return null;
				}

				return registryKey.GetValue("InstallDir64") as string;
			}
		}
	}
}
