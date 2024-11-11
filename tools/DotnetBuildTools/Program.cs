using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Text;

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
				IsRequired = false,
				Description = "Path to the repository for updating"
			};

			var verToUpdate = new Option<string>("--verToUpdate")
			{
				IsRequired = false,
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

			var createOpenApiMappingCommand = new Command("openApiMapping", "Generate OpenApiMapping")
			{
			};


			createOpenApiMappingCommand.Handler = CommandHandler.Create<MappingOptions, IHost>(GenerateMappingAsync);

			root.AddCommand(nugetUpdateCommand);
			root.AddCommand(createOpenApiMappingCommand);

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


		private static async Task<bool> GenerateMappingAsync(MappingOptions programOptions, IHost host)
		{
			var iom_assembly = Assembly.LoadFrom("IdeaRS.OpenModel.dll");
			var iom_types = iom_assembly.GetTypes();

			var api_assembly = Assembly.LoadFrom("IdeaStatiCa.Api.dll");
			var api_types = api_assembly.GetTypes();

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("{");

			bool first = true;

			first = WriteTypesFromAssembly(iom_types, stringBuilder, "IdeaRS.OpenModel", first);
			first = WriteTypesFromAssembly(api_types, stringBuilder, "IdeaStatiCa.Api.Connection.Model", first);

			stringBuilder.AppendLine("");
			stringBuilder.AppendLine("}");

			File.WriteAllText("import-mappings.json", stringBuilder.ToString());

			return await Task.FromResult(true);
		}

		private static bool WriteTypesFromAssembly(Type[] types, StringBuilder stringBuilder, string rootNamespace, bool first)
		{
			foreach (var type in types)
			{
				if (type.IsInterface)
				{
					continue;
				}

				if (type.Name.StartsWith("<"))
				{
					continue;
				}

				if (type?.Namespace?.StartsWith(rootNamespace, StringComparison.InvariantCulture) != true)
				{
					continue;
				}

				if (!first)
				{
					stringBuilder.AppendLine(",");
				}

				stringBuilder.Append($"  \"{type.Name}\" : \"{type.Namespace}.{type.Name}\"");

				Console.WriteLine($"{type.Namespace} {type.Name}");
				first = false;
			}

			return first;
		}
	}
}