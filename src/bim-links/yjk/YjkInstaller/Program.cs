using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace YjkInstaller
{
	internal static class Program
	{
		private static void DisplayHelp(TextWriter writer)
		{
			writer.WriteLine("");
			writer.WriteLine("/i[:directory] | /u | /?");
			writer.WriteLine("");
			writer.WriteLine("  /i             Install YJK plugin");
			writer.WriteLine("  /i:directory   Install YJK plugin, using this path as the YJK install directory");
			writer.WriteLine("  /u             Uninstall YJK plugin");
			writer.WriteLine("  ls             Check if plugin is installed (JSON output)");
			writer.WriteLine("  aps            Check if YJK installation is found (JSON output)");
			writer.WriteLine("  /?             This help");
		}

		public static int Main(string[] args)
		{
			try
			{
				return Run(args) ? 0 : 1;
			}
			catch (Exception e)
			{
				Console.WriteLine($"YJK link installer failed: {e.Message}");
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
			string yjkPath = null;

			foreach (string arg in args)
			{
				string[] parts = arg.Split(new char[] { ':' }, 2);
				string argName = parts[0].ToLower();
				string argValue = parts.Length == 2 ? parts[1].Trim('"', '\'') : null;

				switch (argName)
				{
					case "ls":
						checkLinkStatus = true;
						break;

					case "aps":
						checkApplicationStatus = true;
						break;

					case "/i":
					case "-i":
						install = true;
						yjkPath = argValue;
						break;

					case "/u":
					case "-u":
						uninstall = true;
						yjkPath = argValue;
						break;

					case "/?":
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

			if (install && uninstall)
			{
				Console.Error.WriteLine("You cannot select both install and uninstall.");
				error = true;
			}

			if (error || args.Length == 0)
			{
				if (error)
					Console.Error.WriteLine("Invalid argument.");

				DisplayHelp(Console.Error);
				return false;
			}

			Yjk yjk = Yjk.GetInstallation(yjkPath);
			if (yjk == null)
			{
				Console.Error.WriteLine("YJK installation not found. Use /i:path to specify the YJK install directory.");
				if (checkApplicationStatus)
					Console.Write(JsonConvert.SerializeObject(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("APS", "0") }));
				return checkApplicationStatus;
			}

			Console.WriteLine($"YJK installation found at: '{yjk.InstallPath}'");

			YjkPlugin plugin = new YjkPlugin();
			List<KeyValuePair<string, string>> status = new List<KeyValuePair<string, string>>();

			if (checkApplicationStatus)
				status.Add(new KeyValuePair<string, string>("APS", "1"));

			if (checkLinkStatus)
				status.Add(new KeyValuePair<string, string>("LS", plugin.IsInstalled(yjk) ? "1" : "0"));

			if (install)
				plugin.Install(yjk);

			if (uninstall)
				plugin.Uninstall(yjk);

			if (status.Count != 0)
				Console.Write(JsonConvert.SerializeObject(status));

			return true;
		}
	}
}
