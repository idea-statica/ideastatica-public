using CommunityToolkit.Mvvm.ComponentModel;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.RCS;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.RcsClient.Factory;
using IdeaStatiCa.RcsClient.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RcsApiClient.Services;
using RcsApiClient.ViewModels;
using System;
using System.Windows;
using System.IO;

namespace RcsApiClient
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly IServiceProvider serviceProvider;

		public App()
		{
			SerilogFacade.Initialize();
			IServiceCollection services = new ServiceCollection();

			IConfiguration configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

			services.AddSingleton<RcsClientSettings>((serviceProvider) =>
			{
				RcsClientSettings rcsClientSettings = new RcsClientSettings();
				configuration.GetSection("RcsClientSettings").Bind(rcsClientSettings);
				return rcsClientSettings;
			});

			services.AddTransient<MainWindow>(serviceProvider => new MainWindow
			{
				DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
			});

			services.AddSingleton<IPluginLogger >(serviceProvider =>
			{
				return LoggerProvider.GetLogger("rcsapiclient");
			});

			services.AddSingleton<MainWindowViewModel>();
			services.AddTransient<IReinforcedCrosssSectionSelector, DialogReinforcedCrossSectionSelector>();
			services.AddTransient<IReinforcedCrossSectionTemplateProvider, DialogReinforcedCrossSectionTemplateProvider>();

			services.AddTransient<Func<Type, ObservableObject>>(serviceProvider => viewModelType => (ObservableObject)serviceProvider.GetRequiredService(viewModelType));

			services.AddSingleton<IRcsClientFactory>(serviceProvider =>
			{
				var rcsSettings = serviceProvider.GetRequiredService<RcsClientSettings>();
				return new RcsClientFactory(rcsSettings.IdeaStatiCaDir, serviceProvider.GetService<IPluginLogger>(), httpClientWrapper: null);
			});

			serviceProvider = services.BuildServiceProvider();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
			base.OnStartup(e);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			serviceProvider.GetRequiredService<IRcsClientFactory>().Dispose();
		}
	}
}
