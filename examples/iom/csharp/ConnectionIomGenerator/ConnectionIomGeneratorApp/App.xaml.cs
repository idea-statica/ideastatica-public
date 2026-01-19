using ConnectionIomGenerator.Model;
using ConnectionIomGenerator.Service;
using ConnectionIomGenerator.UI.Models;
using ConnectionIomGenerator.UI.Services;
using ConnectionIomGenerator.UI.ViewModels;
using ConnectionIomGeneratorApp.View;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;


namespace ConnectionIomGeneratorApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly IServiceProvider _serviceProvider;
		private IomGeneratorViewModel? _mainViewModel;

		public App()
		{
			IConfiguration configuration = BuildConfiguration();
			var services = new ServiceCollection();

			services.AddSingleton<IPluginLogger>(serviceProvider =>
			{
				SerilogFacade.Initialize();
				return LoggerProvider.GetLogger("ConnectionIomGeneratorApp");
			});

			services.AddSingleton<IomGeneratorViewModel>();

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

			services.AddTransient<MainWindow>(serviceProvider => new MainWindow
			{
				DataContext = serviceProvider.GetRequiredService<IomGeneratorViewModel>()
			});

			var serviceProvider = services.BuildServiceProvider();
			if(serviceProvider == null)
			{
				throw new Exception();
			}

			_serviceProvider = serviceProvider;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
			var vm = mainWindow.DataContext as IomGeneratorViewModel;
			if(vm == null)
			{
				throw new Exception();
			}
			_mainViewModel = vm;

			base.OnStartup(e);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
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
