using CommunityToolkit.Mvvm.ComponentModel;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.RcsClient.Client;
using IdeaStatiCa.RcsClient.Factory;
using IdeaStatiCa.RcsClient.HttpWrapper;
using Microsoft.Extensions.DependencyInjection;
using RcsApiClient.ViewModels;
using System;
using System.Windows;

namespace RcsApiClient
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private string ideaStatiCaDir = RcsApiClient.Properties.Settings.Default.IdeaStatiCaDir;

		private readonly IServiceProvider serviceProvider;

		public App()
		{
			SerilogFacade.Initialize();
			IServiceCollection services = new ServiceCollection();
			services.AddSingleton<MainWindow>(serviceProvider => new MainWindow
			{
				DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
			});

			services.AddSingleton<IPluginLogger >(serviceProvider =>
			{
				return LoggerProvider.GetLogger("rcsapiclient");
			});

			services.AddSingleton<MainWindowViewModel>();

			services.AddSingleton<Func<Type, ObservableObject>>(serviceProvider => viewModelType => (ObservableObject)serviceProvider.GetRequiredService(viewModelType));

			services.AddSingleton<IRcsClientFactory>(serviceProvider =>
			{
				var rcsClient =  new RcsClientFactory(serviceProvider.GetService<IPluginLogger>(), httpClientWrapper: null, ideaStatiCaDir);
				return rcsClient;
			});


			serviceProvider = services.BuildServiceProvider();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
			base.OnStartup(e);
		}
	}
}
