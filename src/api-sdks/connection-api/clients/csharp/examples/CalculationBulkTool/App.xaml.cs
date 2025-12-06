using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole;
using Application = System.Windows.Application;

namespace CalculationBulkTool
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.Enrich.FromLogContext()
				.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
				.WriteTo.File(
					path: Path.Combine(Path.GetTempPath(), "IdeaStatiCa.CalculationBulkTool.log"),
					rollingInterval: RollingInterval.Infinite,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
				.CreateLogger();

			Log.Information("Application starting");

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			DispatcherUnhandledException += App_DispatcherUnhandledException;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			Log.Information("Application exiting with code {Code}", e.ApplicationExitCode);
			Log.CloseAndFlush();

			base.OnExit(e);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception ex)
			{
				Log.Fatal(ex, "Unhandled exception in AppDomain. IsTerminating={IsTerminating}", e.IsTerminating);
			}
			else
			{
				Log.Fatal("Unhandled non-exception object in AppDomain: {Object}. IsTerminating={IsTerminating}", e.ExceptionObject, e.IsTerminating);
			}
		}

		private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Log.Error(e.Exception, "Unhandled dispatcher exception");
			e.Handled = true;
		}
	}
}
