using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RstabToIdeaPluginInstall
{
	internal abstract class AbstractPlugin
	{
		private readonly string _name;
		private readonly string _pluginAssembly;
		private readonly string _pluginClassName;

		protected string ProgId => $"{_pluginAssembly}.{_pluginClassName}";

		public AbstractPlugin(string name, string pluginAssembly, string pluginClassName)
		{
			_name = name;
			_pluginAssembly = pluginAssembly;
			_pluginClassName = pluginClassName;
		}

		public void Install(RSTABConfig config, bool dontRegister, string pluginDirectory)
		{
			Console.WriteLine($"Installing plugin '{_name}', progid '{ProgId}'");

			if (!dontRegister)
			{
				Register(GetPath(pluginDirectory));
			}

			config.Add(_name, _name, ProgId);
		}

		public void Uninstall(RSTABConfig config, string pluginDirectory)
		{
			Console.WriteLine($"Uninstalling plugin '{_name}', progid '{ProgId}'");

			string path = GetPath(pluginDirectory);

			if (IsComServerRegistered(path))
			{
				Unregister(GetPath(pluginDirectory));
			}

			config.Remove(ProgId);
		}

		public bool IsInstalled(RSTABConfig config, string pluginDirectory)
		{
			return Type.GetTypeFromProgID(ProgId) != null
				&& config.Contains(ProgId)
				&& IsComServerRegistered(GetPath(pluginDirectory));
		}

		protected string GetPath(string pluginDirectory)
		{
			return Path.Combine(pluginDirectory, _pluginAssembly + ".dll");
		}

		protected string GetClsid()
		{
			return Registry.GetValue($"HKEY_CLASSES_ROOT\\{ProgId}\\CLSID", null, null) as string;
		}

		protected int RunCommand(string program, IEnumerable<string> arguments)
		{
			StringBuilder argumentsBuilder = new StringBuilder();
			string separator = "";

			foreach (string argument in arguments)
			{
				argumentsBuilder.Append(separator);

				if (argument.Contains(" "))
				{
					argumentsBuilder.Append('"');
					argumentsBuilder.Append(argument);
					argumentsBuilder.Append('"');
				}
				else
				{
					argumentsBuilder.Append(argument);
				}

				separator = " ";
			}

			string argumentsStr = argumentsBuilder.ToString();

			Console.WriteLine($"Starting process '{program}' with arguments '{argumentsStr}'.");

			ProcessStartInfo StartInfo = new ProcessStartInfo()
			{
				CreateNoWindow = true,
				FileName = program,
				Arguments = argumentsStr,
				UseShellExecute = false,
			};

			Process process = Process.Start(StartInfo);

			Console.WriteLine($"Process started with pid '{process.Id}'.");

			process.WaitForExit();

			Console.WriteLine($"Process '{process.Id}' exited with code '{process.ExitCode}'.");

			return process.ExitCode;
		}

		protected bool ArePathsEqual(string path1, string path2)
		{
			return Path.GetFullPath(path1).ToLower() == Path.GetFullPath(path2).ToLower();
		}

		abstract protected void Register(string path);

		abstract protected void Unregister(string path);

		abstract protected bool IsComServerRegistered(string path);
	}
}