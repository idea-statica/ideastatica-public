using System;
using System.IO;
using System.Reflection;

namespace YjkInstaller
{
	internal class YjkPlugin
	{
		// Embedded as resources — extracted to the YJK install dir during install.
		// These replace the stock YJK DLLs to enable plugin-loading capability.
		private static readonly string[] YjkReplacementDlls = { "ClrYJKAPI.dll", "YAPIData.dll", "YJKAPI.dll" };

		public void Install(Yjk yjk)
		{
			Console.WriteLine($"Installing YJK plugin to '{yjk.InstallPath}'");

			ExtractReplacementDlls(yjk.InstallPath);

			yjk.Config.AddToApiPlugList();
			yjk.Config.AddToCui();
			yjk.Config.MergeBindingRedirects();
		}

		public void Uninstall(Yjk yjk)
		{
			Console.WriteLine($"Uninstalling YJK plugin from '{yjk.InstallPath}'");

			yjk.Config.RemoveFromApiPlugList();
			yjk.Config.RemoveFromCui();
			yjk.Config.RemoveBindingRedirects();
		}

		public bool IsInstalled(Yjk yjk)
		{
			return yjk.Config.IsPluginInApiPlugList() && yjk.Config.IsPluginInCui();
		}

		private static void ExtractReplacementDlls(string targetDirectory)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			foreach (string dll in YjkReplacementDlls)
			{
				string resourceName = $"YjkInstaller.Resources.{dll}";
				using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				{
					if (stream == null)
					{
						Console.WriteLine($"WARNING: Embedded resource '{resourceName}' not found. The plugin may not load correctly.");
						continue;
					}

					string dest = Path.Combine(targetDirectory, dll);
					using (FileStream fs = new FileStream(dest, FileMode.Create, FileAccess.Write))
						stream.CopyTo(fs);

					Console.WriteLine($"Extracted replacement DLL '{dll}' to '{targetDirectory}'.");
				}
			}
		}

	}
}
