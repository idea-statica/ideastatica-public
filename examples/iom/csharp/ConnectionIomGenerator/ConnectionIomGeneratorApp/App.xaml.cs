using ConnectionIomGenerator.UI.Services;
using ConnectionIomGenerator.UI.ViewModels;
using ConnectionIomGeneratorApp.View;
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
		private MainWindowViewModel? _mainViewModel;

		public App()
		{
			IConfiguration configuration = BuildConfiguration();
			var services = new ServiceCollection();

			services.AddTransient<IProjectService, ProjectService>();
			services.AddSingleton<MainWindowViewModel>();

			services.AddTransient<MainWindow>(serviceProvider => new MainWindow
			{
				DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
			});

			//services.AddTransient<MainWindowViewModel>();



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
			var vm = mainWindow.DataContext as MainWindowViewModel;
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
