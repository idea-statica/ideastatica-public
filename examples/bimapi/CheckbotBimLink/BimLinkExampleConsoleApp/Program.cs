using BimApiLinkCadExample;
using CadExampleApi;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BimLinkExampleConsoleApp
{
	/// <summary>
	/// Starts IDEA StatiCa Checkbot linked to the CAD example, and ends when Checkbot is closed.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// Where IDEA StatiCa 26.1 installs Checkbot unless the user chooses another folder.
		/// </summary>
		private const string DefaultCheckbotPath = @"C:\Program Files\IDEA StatiCa\StatiCa 26.1\IdeaCheckbot.exe";

		/// <summary>
		/// Runs the CAD example link until Checkbot is closed.
		/// </summary>
		/// <param name="args">
		/// Optional: the full path to IdeaCheckbot.exe. Without it, the app uses <see cref="DefaultCheckbotPath"/>.
		/// </param>
		/// <returns>0 when Checkbot has closed, 1 when Checkbot is not found or the link fails.</returns>
		private static async Task<int> Main(string[] args)
		{
			string checkbotPath = args.Length > 0 ? args[0] : DefaultCheckbotPath;
			if (!File.Exists(checkbotPath))
			{
				Console.Error.WriteLine($"Checkbot is not at '{checkbotPath}'. Give the full path to IdeaCheckbot.exe as the first argument.");
				return 1;
			}

			string projectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CadExampleCheckbotProject");

			try
			{
				// A console has no window to keep responsive, so the app waits for the link right here. A window
				// starts it on a pool thread instead, as BimLinkExampleRunner does.
				await CheckbotLink.Run(checkbotPath, projectPath, new CadApi(), new ConsoleLogger());
				return 0;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"The link failed: {ex.Message}");
				return 1;
			}
		}
	}
}
