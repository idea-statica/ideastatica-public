using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.RamToIdeaApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
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

			// Call the async method and wait for it
			await startupService.RunCheckbotAsync();

			// Shut down the application after completion
			Shutdown();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
		}
	}

}
