using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace RstabToIdeaPluginInstall
{
	internal class DotNetPlugin : AbstractPlugin
	{
		public DotNetPlugin(string name, string pluginAssembly, string pluginClassName)
			: base(name, pluginAssembly, pluginClassName)
		{
		}

		protected override bool IsComServerRegistered(string path)
		{
			if (!(GetClsid() is string clsid))
			{
				return false;
			}

			if (!(Registry.GetValue($"HKEY_CLASSES_ROOT\\CLSID\\{clsid}\\InprocServer32", "CodeBase", null) is string comPath))
			{
				return false;
			}

			try
			{
				return ArePathsEqual(new Uri(comPath).LocalPath.ToLowerInvariant(), path);
			}
			catch (UriFormatException)
			{
				return false;
			}
		}

		protected override void Register(string path)
		{
			RunRegAsm(false, path);
		}

		protected override void Unregister(string path)
		{
			RunRegAsm(true, path);
		}

		private void RunRegAsm(bool unregister, string dllPath)
		{
			List<string> arguments = new List<string>();

			if (unregister)
			{
				arguments.Add("/unregister");
			}
			else
			{
				arguments.Add("/codebase");
			}

			arguments.Add(dllPath);

			string windowPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
			string regAsmPath = Path.Combine(windowPath, @"Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe");
			int exitCode = RunCommand(regAsmPath, arguments);

			if (exitCode != 0)
			{
				Console.WriteLine($"Failed to register COM server '{dllPath}'.");
			}
		}
	}
}