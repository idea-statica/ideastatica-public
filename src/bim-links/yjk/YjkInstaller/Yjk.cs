using Microsoft.Win32;
using System.IO;

namespace YjkInstaller
{
	internal class Yjk
	{
		private const string YjkRegistryKey = @"SOFTWARE\YJKSOFT\YJKS8.1.0";
		private const string YjkInstallValueName = "INSTALLFOLDER";
		private const string YjkDefaultInstallPath = @"C:\YJKS\YJKS_8_1_0\";

		public string InstallPath { get; }

		public YjkConfig Config { get; }

		private Yjk(string installPath)
		{
			InstallPath = installPath;
			Config = new YjkConfig(installPath);
		}

		public static Yjk GetInstallation(string overridePath = null)
		{
			if (overridePath != null)
			{
				string fullPath = Path.GetFullPath(overridePath);
				return Directory.Exists(fullPath) ? new Yjk(fullPath) : null;
			}

			string registryPath = GetInstallPathFromRegistry();
			if (registryPath != null && Directory.Exists(registryPath))
				return new Yjk(registryPath);

			if (Directory.Exists(YjkDefaultInstallPath))
				return new Yjk(YjkDefaultInstallPath);

			return null;
		}

		private static string GetInstallPathFromRegistry()
		{
			using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			using (RegistryKey key = hklm.OpenSubKey(YjkRegistryKey))
			{
				return key?.GetValue(YjkInstallValueName) as string;
			}
		}
	}
}
