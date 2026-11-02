#if NET48

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace IdeaStatiCa.Plugin.Utilities
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
			List<(Version, string)> installedVersions = new List<(Version, string)>();
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
#endif