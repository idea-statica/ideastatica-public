using System;
using System.IO;
using System.Reflection;

namespace YjkInstaller
{
	internal class YjkPlugin
	{
		private const string PluginDllName = "IDEAStatiCa.yjk.dll";

		// Embedded as resources — extracted to the YJK install dir during install.
		// These replace the stock YJK DLLs to enable plugin-loading capability.
		private static readonly string[] YjkReplacementDlls = { "ClrYJKAPI.dll", "YAPIData.dll", "YJKAPI.dll" };

		public void Install(Yjk yjk, string sourceDirectory)
		{
			Console.WriteLine($"Installing YJK plugin from '{sourceDirectory}' to '{yjk.InstallPath}'");

			ExtractReplacementDlls(yjk.InstallPath);
			CopyPluginFiles(sourceDirectory, yjk.InstallPath);

			yjk.Config.AddToApiPlugList();
			yjk.Config.AddToCui();
			//yjk.Config.MergeBindingRedirects();
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

		private static void CopyPluginFiles(string sourceDirectory, string targetDirectory)
		{
			string pluginDllSource = Path.Combine(sourceDirectory, PluginDllName);
			if (!File.Exists(pluginDllSource))
			{
				Console.WriteLine($"WARNING: Plugin DLL not found at '{pluginDllSource}'. Skipping file copy.");
				return;
			}

			foreach (string srcFile in Directory.GetFiles(sourceDirectory, "*", SearchOption.TopDirectoryOnly))
			{
				string fileName = Path.GetFileName(srcFile);

				// Skip replacement DLLs — handled by ExtractReplacementDlls
				bool isReplacementDll = Array.Exists(YjkReplacementDlls, d => d.Equals(fileName, StringComparison.OrdinalIgnoreCase));
				if (isReplacementDll) continue;

				// Skip the installer itself
				if (fileName.Equals("YjkInstaller.exe", StringComparison.OrdinalIgnoreCase)) continue;
				if (fileName.Equals("YjkInstaller.exe.config", StringComparison.OrdinalIgnoreCase)) continue;
				if (fileName.Equals("YjkInstaller.pdb", StringComparison.OrdinalIgnoreCase)) continue;

				string dest = Path.Combine(targetDirectory, fileName);
				File.Copy(srcFile, dest, overwrite: true);
				Console.WriteLine($"Copied '{fileName}' to '{targetDirectory}'.");
			}
		}
	}
}
