using ConnectionWebClient.ViewModels;
using IdeaStatiCa.ConnectionClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Windows;

namespace ConnectionWebClient
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly IServiceProvider serviceProvider;

		public App()
		{
			IConfiguration configuration = BuildConfiguration();
			var services = new ServiceCollection();

			services.AddTransient<MainWindow>(serviceProvider => new MainWindow
			{
				DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
			});
			services.AddTransient<MainWindowViewModel>();

			services.AddTransient<HttpClient>(serviceProvider => new HttpClient() { BaseAddress = new Uri(configuration["CONNECTION_API_ENDPOINT"]!) });

			services.AddTransient<IConnectionClient, ConnectionClientHTTP>();

			serviceProvider = services.BuildServiceProvider();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
			mainWindow.Show();
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
