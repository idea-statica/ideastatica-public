using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace yjk
{
	internal static class AssemblyResolver
	{
		private static string[] _searchPaths;
		private static bool _installed;
		private static readonly object _lock = new object();

		public static string Net48Path => _searchPaths?[0];

		public static void Install()
		{
			lock (_lock)
			{
				if (_installed) return;
				string ideaInstallRoot = DiscoverIdeaInstallRoot();
				_searchPaths = new[]
				{
					Path.Combine(ideaInstallRoot, "net48"),
					ideaInstallRoot,
				};
				AppDomain.CurrentDomain.AssemblyResolve += Handle;

				// Grpc.Core locates grpc_csharp_ext.x64.dll relative to Grpc.Core.dll's own
				// CodeBase — but YJK ships its own Grpc.Core.dll in YJKS_8_1_0\ (without the
				// native DLL), so the relative probe fails. GRPC_CSHARP_EXT_OVERRIDE_LOCATION
				// is the only supported override: Grpc.Core checks it before any path probing.
				string grpcNative = Path.Combine(_searchPaths[0], "grpc_csharp_ext.x64.dll");
				Environment.SetEnvironmentVariable("GRPC_CSHARP_EXT_OVERRIDE_LOCATION", grpcNative);

				_installed = true;
			}
		}

			private static Assembly Handle(object sender, ResolveEventArgs args)
		{
			string simpleName = new AssemblyName(args.Name).Name;
			if (simpleName.EndsWith(".resources")) return null;

			foreach (string dir in _searchPaths)
			{
				string candidate = Path.Combine(dir, simpleName + ".dll");
				if (File.Exists(candidate))
					return Assembly.LoadFrom(candidate);
			}
			return null;
		}

		private static string DiscoverIdeaInstallRoot()
		{
			// Enumerate HKLM\SOFTWARE\IDEAStatiCa\* and pick the highest version subkey.
			try
			{
				using (var ideaKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\IDEAStatiCa"))
				{
					if (ideaKey != null)
					{
						string bestVersionName = null;
						Version bestVersion = null;

						foreach (string subKeyName in ideaKey.GetSubKeyNames())
						{
							if (Version.TryParse(subKeyName, out Version v))
							{
								if (bestVersion == null || v > bestVersion)
								{
									bestVersion = v;
									bestVersionName = subKeyName;
								}
							}
						}

						if (bestVersionName != null)
						{
							using (var versionKey = ideaKey.OpenSubKey(
								$@"{bestVersionName}\IDEAStatiCa\Designer"))
							{
								string installDir = versionKey?.GetValue("InstallDir64") as string;
								if (!string.IsNullOrEmpty(installDir) && Directory.Exists(installDir))
									return installDir;
							}
						}
					}
				}
			}
			catch
			{
				// fall through to config fallback
			}

			// Fallback: derive from CheckbotLocation in IDEAStatiCa.yjk.dll.config
			string configPath = Path.Combine(
				Path.GetDirectoryName(new Uri(typeof(AssemblyResolver).Assembly.CodeBase).LocalPath),
				"IDEAStatiCa.yjk.dll.config");

			if (File.Exists(configPath))
			{
				try
				{
					var map = new System.Configuration.ExeConfigurationFileMap { ExeConfigFilename = configPath };
					var config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(
						map, System.Configuration.ConfigurationUserLevel.None);
					string checkbotLocation = config.AppSettings.Settings["CheckbotLocation"]?.Value;
					if (!string.IsNullOrEmpty(checkbotLocation))
					{
						string dir = Path.GetDirectoryName(checkbotLocation);
						if (Directory.Exists(dir))
							return dir;
					}
				}
				catch
				{
					// fall through to hardcoded default
				}
			}

			return @"C:\Program Files\IDEA StatiCa\StatiCa 26.0";
		}
	}
}
