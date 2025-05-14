using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.RamToIdeaApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
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

		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var startupService = serviceProvider.GetRequiredService<IStartupService>();

			string ramFilePath = null;
			foreach (var arg in e.Args)
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
			}
			else
			{
				await startupService.RunCheckbotAsync();
			}

			Shutdown();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
		}
	}

}
