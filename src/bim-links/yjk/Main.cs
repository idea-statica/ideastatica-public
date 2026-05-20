using IdeaStatiCa.Plugin;
//using IdeaStatiCa.PluginLogger;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace yjk
{
	public class Main
	{
		private static readonly IPluginLogger _logger;

		static Main()
		{
			//SerilogFacade.Initialize("IdeaYJKPlugin.log");
			//_logger = LoggerProvider.GetLogger("yjk.launcher");
		}

		[CommandMethod("idea_statica")]
		public void Run()
		{
			//_logger.LogInformation("IDEA StatiCa plugin clicked");
			Task.Run(() =>
			{
				try
				{
					LaunchDriver();
				}
				catch (Exception ex)
				{
					_logger.LogError("Failed to launch YjkDriver", ex);
				}
			});
		}

		private static void LaunchDriver()
		{
			string launcherDir = Path.GetDirectoryName(
				new Uri(typeof(Main).Assembly.CodeBase).LocalPath);
			//_logger.LogInformation($"Launcher dir: {launcherDir}");

			string driverPath = Path.Combine(launcherDir, "YjkDriver.exe");
			if (!File.Exists(driverPath))
			{
				_logger.LogError($"YjkDriver.exe not found at '{driverPath}'");
				return;
			}

			string workingDirectory = Directory.GetCurrentDirectory();
			//_logger.LogInformation($"Working directory: {workingDirectory}");

			var yjkFiles = Directory.GetFiles(workingDirectory, "*.yjk");
			string yjkFileName = yjkFiles.Length == 1
				? Path.GetFileNameWithoutExtension(yjkFiles[0]) : "";
			string fullWorkingDirectory = Path.Combine(workingDirectory, "IdeaStatiCa-" + yjkFileName);

			if (!Directory.Exists(fullWorkingDirectory))
				Directory.CreateDirectory(fullWorkingDirectory);

			int pid = Process.GetCurrentProcess().Id;

			var startInfo = new ProcessStartInfo
			{
				FileName = driverPath,
				Arguments = pid + " \"" + fullWorkingDirectory + "\"",
				UseShellExecute = false,
				CreateNoWindow = false,
			};

			//_logger.LogInformation($"Starting YjkDriver.exe pid={pid} workDir={fullWorkingDirectory}");
			var driver = Process.Start(startInfo);
			driver.WaitForExit();
			//_logger.LogInformation($"YjkDriver.exe exited with code {driver.ExitCode}");
		}
	}
}
