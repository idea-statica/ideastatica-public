using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RstabToIdeaPluginInstall
{
	internal static class Program
	{
		private static void DisplayHelp(TextWriter writter)
		{
			writter.WriteLine("");
			writter.WriteLine("/i[:directory] | /u[:directory] | /r | /?");
			writter.WriteLine("");
			writter.WriteLine("  /i         Install RFEM plugin");
			writter.WriteLine("  /u         Remove RFEM plugin");
			writter.WriteLine("  /r         Without COM registration");
			writter.WriteLine("  /?         This help");
			writter.WriteLine("  directory  Install RFEM plugin from this IDEA directory");
		}

		public static int Main(string[] args)
		{
			try
			{
				return Run(args) ? 0 : 1;
			}
			catch (Exception e)
			{
				Console.WriteLine("RSTAB link installer failed {0}", e.Message);
			}

			return 1;
		}

		private static bool Run(string[] args)
		{
			bool checkLinkStatus = false;
			bool checkApplicationStatus = false;
			bool install = false;
			bool uninstall = false;
			bool error = false;
			bool dontRegister = false;
			string path = null;

			// parse arguments
			foreach (string arg in args)
			{
				string[] parts = arg.Split(new char[] { ':' }, 2);
				string argName = parts[0].ToLower();
				string argValue = null;

				if (parts.Length == 2)
				{
					argValue = parts[1].Trim('"', '\'');
				}

				switch (arg.ToLower())
				{
					case "ls":
						checkLinkStatus = true;
						break;

					case "aps":
						checkApplicationStatus = true;
						break;

					case "-i":
					case "/i":
						install = true;
						path = argValue;
						break;

					case "-u":
					case "/u":
						uninstall = true;
						path = argValue;
						break;

					case "-r":
					case "/r":
						dontRegister = true;
						break;

					case "/h":
					case "-h":
					case "--help":
						DisplayHelp(Console.Out);
						return true;

					default:
						error = true;
						break;
				}
			}

			// -i -u together is not a valid combination of arguments
			if (install && uninstall)
			{
				Console.Error.WriteLine("You cannot select both install and uninstall.");
				error = true;
			}

			if (error || args.Length == 0)
			{
				if (error)
				{
					Console.Error.WriteLine("Invalid argument.");
				}

				DisplayHelp(Console.Error);
				return false;
			}

			if (path == null)
			{
				path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
			else
			{
				path = Path.GetFullPath(path);
			}

			Console.WriteLine($"Path to plugins: '{path}'");

			RSTAB rstab = RSTAB.GetLatestVersion();
			List<AbstractPlugin> plugins = new List<AbstractPlugin>()
			{
				new DotNetPlugin("IDEA StatiCa Checkbot", "IdeaRstabPlugin", "CheckbotCommand")
			};

			List<KeyValuePair<string, string>> status = new List<KeyValuePair<string, string>>();

			if (checkApplicationStatus)
			{
				int apsResult = rstab == null ? 0 : 1;
				status.Add(new KeyValuePair<string, string>("APS", apsResult.ToString()));
			}

			if (checkLinkStatus)
			{
				int lsResult = 0;
				if (rstab != null)
				{
					lsResult = plugins.All(x => x.IsInstalled(rstab.Config, path)) ? 1 : 0;
				}

				status.Add(new KeyValuePair<string, string>("LS", lsResult.ToString()));
			}

			if (install)
			{
				plugins.ForEach(x => x.Install(rstab.Config, dontRegister, path));
			}

			if (uninstall)
			{
				plugins.ForEach(x => x.Uninstall(rstab.Config, path));

				// remove the old plugin
				new DotNetPlugin("", " RstabToIdeaPlugin", "IdeaConnection").Uninstall(rstab.Config, path);
			}

			if (install || uninstall)
			{
				rstab.Config.Write();
			}

			if (status.Count != 0)
			{
				Console.Write(JsonConvert.SerializeObject(status));
			}

			return true;
		}
	}
}