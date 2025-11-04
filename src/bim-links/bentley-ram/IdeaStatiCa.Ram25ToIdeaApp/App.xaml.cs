using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.RamToIdeaApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace IdeaStatiCa.RamToIdeaApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly IServiceProvider serviceProvider;

		public App()
		{
			var services = new ServiceCollection();
			services.AddSingleton<IPluginLogger>(serviceProvider =>
			{
				SerilogFacade.Initialize();
				return LoggerProvider.GetLogger("ramtoideaapp");
			});

			services.AddTransient<IProjectService, ProjectService>();
			services.AddTransient<IStartupService, StartupService>();

			serviceProvider = services.BuildServiceProvider();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			_ = RunAppLogicAsync(e.Args);
		}

		private async Task RunAppLogicAsync(string[] args)
		{
			try
			{
				var startupService = serviceProvider.GetRequiredService<IStartupService>();

				string ramFilePath = null;
				foreach (var arg in args)
				{
					if (arg.StartsWith("-ramFile=", StringComparison.OrdinalIgnoreCase))
					{
						ramFilePath = arg.Substring("-ramFile=".Length).Trim('"');
					}
				}

				if (ramFilePath is { })
				{
					var outputFilePath = Path.ChangeExtension(ramFilePath, ".xml");
					var outputXml = await startupService.ExportIOMModelAsync(ramFilePath);
					await File.WriteAllTextAsync(outputFilePath, outputXml);

					// Shutdown after success
					Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown(0));
				}
				else
				{
					await startupService.RunCheckbotAsync();
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			finally
			{
				Dispatcher.Invoke(() => Shutdown());
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
		}
	}

}
