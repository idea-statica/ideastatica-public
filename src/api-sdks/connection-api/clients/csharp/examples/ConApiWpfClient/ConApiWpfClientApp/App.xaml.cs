using ConApiWpfClientApp.Services;
using ConApiWpfClientApp.ViewModels;
using ConApiWpfClientApp.Views;
using ConnectionIomGenerator.Model;
using ConnectionIomGenerator.Service;
using ConnectionIomGenerator.UI.Models;
using ConnectionIomGenerator.UI.Services;
using IdeaStatiCa.ConRestApiClientUI;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ConApiWpfClientApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly IServiceProvider serviceProvider;
		private MainWindowViewModel? mainWindowViewModel;

		public App()
		{
			IConfiguration configuration = BuildConfiguration();
			var services = new ServiceCollection();

			services.AddSingleton<IConfiguration>(configuration);

			services.AddSingleton<IPluginLogger>(serviceProvider =>
			{
				SerilogFacade.Initialize();
				return LoggerProvider.GetLogger("con.restapi.client");
			});

			services.AddSingleton<IConnectionApiService, ConnectionApiService>();

			services.AddTransient<MainWindow>(serviceProvider =>
			{
				var vm = serviceProvider.GetRequiredService<IConRestApiClientViewModel>();
				return new MainWindow(vm)
				{
					DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
				};
			});
			services.AddSingleton<MainWindowViewModel>();

			services.AddTransient<JsonEditorWindow>(serviceProvider => new JsonEditorWindow
			{
				DataContext = serviceProvider.GetRequiredService<JsonEditorViewModel>()
			});
			services.AddTransient<JsonEditorViewModel>();

			services.AddSingleton<ISceneController, SceneController>();

			services.AddSingleton<IClientHost, ClientHost>();

			services.AddSingleton<IConRestApiClientViewModel, ConRestApiClientViewModel>();

			#region Services required by IOM generator
			services.AddSingleton<ConnectionIomGenerator.UI.ViewModels.IomEditorWindowViewModel>();

			services.AddSingleton<IomGeneratorModel>(serviceProvider =>
			{
				return new IomGeneratorModel
				{
					ConnectionInput = ConnectionInput.GetDefaultECEN()
				};
			});

			services.AddTransient<IFileDialogService, FileDialogService>();
			services.AddTransient<IIomService, IomService>();
			services.AddTransient<IIomGenerator, IomGenerator>(); 
			#endregion

			serviceProvider = services.BuildServiceProvider();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			if (this.mainWindowViewModel != null)
			{
				Task.Run(() => this.mainWindowViewModel.OnExitApplication()).Wait();
				this.mainWindowViewModel = null;
			}
			
			base.OnExit(e);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
			this.mainWindowViewModel = mainWindow.DataContext as MainWindowViewModel;
			base.OnStartup(e);
		}

		public static IConfigurationRoot BuildConfiguration()
		{
			return new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();
		}
	}
}
