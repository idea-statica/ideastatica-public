using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;

namespace DotnetBuildTools
{
	public class Program
	{
		static async Task<int> Main(string[] args) =>
			await BuildCommandLine()
				.UseHost(_ => Host.CreateDefaultBuilder(),
					host =>
					{
						host.ConfigureServices((hostContext, services) =>
						{
							services.AddLogging(builder => builder.AddConsole());
						});
					})
				.UseDefaults()
				.Build()
				.InvokeAsync(args);

		private static CommandLineBuilder BuildCommandLine()
		{
			var repository = new Option<string>("--repository")
			{
				IsRequired = true,
				Description = "Path to the repository for updating"
			};

			var verToUpdate = new Option<string>("--verToUpdate")
			{
				IsRequired = true,
				Description = "The required version of IdeaStatica nuget packages"
			};

			var root = new RootCommand("Tools for a maintanance of the repository")
			{
				repository, verToUpdate,
			};

			var nugetUpdateCommand = new Command("nuget_update", "Update Idea Statica nuget packages in the repository")
			{
				repository, verToUpdate,
			};

			nugetUpdateCommand.Handler = CommandHandler.Create<ProgramOptions, IHost>(RunUpdateAsync);


			root.AddCommand(nugetUpdateCommand);

			return new CommandLineBuilder(root);
		}

		private static async Task<bool> RunUpdateAsync(ProgramOptions programOptions, IHost host)
		{
			var logger = host.Services.GetRequiredService<ILogger<NugetUpdater>>();
			bool isChange = false;
			string versionToUpdate = programOptions.verToUpdate;
			var repositoryDir = programOptions.repository;
			logger.LogInformation($"RunUpdateAsync repositoryDir = '{repositoryDir}' versionToUpdate = '{versionToUpdate}'");
			string currentVer = string.Empty;

			try
			{
				
				var updater = new NugetUpdater(logger, repositoryDir);

				currentVer = updater.GetCurrentVersion();
				logger.LogInformation($"RunUpdateAsync current IS nuget version = '{currentVer}' required IS nuget version = '{versionToUpdate}'");

				if (!currentVer.Equals(versionToUpdate))
				{
					isChange = updater.UpdateNugetInCsproj(currentVer, versionToUpdate);
				}
				else
				{
					logger.LogInformation("RunUpdateAsync : no need to update");
				}
			}
			catch (Exception e)
			{
				logger.LogError("Error", e);
				Environment.Exit(1);
				;
			}
			finally
			{
				if (isChange)
				{
					logger.LogInformation($"RunUpdateAsync : IS nuget was updated to version {versionToUpdate}");
				}
				else
				{
					logger.LogInformation($"RunUpdateAsync :No nee to update. Actual version is {currentVer}");
				}


				if (logger != null && logger is IDisposable disp)
				{
					disp.Dispose(); 
				}
			}

			return await Task.FromResult(isChange);
		}


		//		public static void Main(string[] args)
		//		{ options,
		//			// initialize logger
		//			SerilogFacade.Initialize();
		//			var logger = LoggerProvider.GetLogger("dotnetbuildtools");

		//			try
		//			{
		//				var commandLines = Environment.GetCommandLineArgs();
		//				if (commandLines.Length < 2)
		//				{
		//					logger.LogWarning("Missing the path to the repository");
		//					return;
		//				}

		//				string versionToUpdate = "23.6.100";

		//				var repositoryDir = commandLines[1];
		//				logger.LogInformation($"Main {repositoryDir}");
		//				var updater = new NugetUpdater(logger, repositoryDir);

		//				var currentVer = updater.GetCurrentVersion();
		//				logger.LogInformation($"Main current IS nuget version = '{currentVer}' required IS nuget version = '{versionToUpdate}'");

		//				if (!currentVer.Equals(versionToUpdate))
		//				{
		//					updater.UpdateNugetInCsproj(currentVer, versionToUpdate);
		//				}
		//				else
		//				{
		//					logger.LogInformation("Main : no need to update");
		//				}
		//			}
		//			catch(Exception e)
		//			{
		//				logger.LogError("Error", e);
		//				Environment.Exit(1);
		//;			}
		//			finally
		//			{
		//				logger.LogInformation("Done");
		//				if (logger != null && logger is IDisposable disp)
		//				{
		//					disp.Dispose();
		//				}
		//			}
		//			Environment.Exit(0);
		//		}
	}
}