using IdeaStatiCa.PluginLogger;

namespace DotnetBuildTools
{
	public class Program
	{

		public static void Main(string[] args)
		{
			// initialize logger
			SerilogFacade.Initialize();
			var logger = LoggerProvider.GetLogger("dotnetbuildtools");

			try
			{
				var commandLines = Environment.GetCommandLineArgs();
				if (commandLines.Length < 2)
				{
					logger.LogWarning("Missing the path to the repository");
					return;
				}

				string versionToUpdate = "23.6.100";

				var repositoryDir = commandLines[1];
				logger.LogInformation($"Main {repositoryDir}");
				var updater = new NugetUpdater(logger, repositoryDir);

				var currentVer = updater.GetCurrentVersion();
				logger.LogInformation($"Main current IS nuget version = '{currentVer}' required IS nuget version = '{versionToUpdate}'");

				if (!currentVer.Equals(versionToUpdate))
				{
					updater.UpdateNugetInCsproj(currentVer, versionToUpdate);
				}
				else
				{
					logger.LogInformation("Main : no need to update");
				}
			}
			catch(Exception e)
			{
				logger.LogError("Error", e);
				Environment.Exit(1);
;			}
			finally
			{
				logger.LogInformation("Done");
				if (logger != null && logger is IDisposable disp)
				{
					disp.Dispose();
				}
			}
			Environment.Exit(0);
		}
	}
}