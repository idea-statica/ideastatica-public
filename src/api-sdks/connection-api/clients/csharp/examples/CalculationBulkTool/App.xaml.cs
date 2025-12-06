using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

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
				.Enrich.WithProperty("ProcessId", System.Diagnostics.Process.GetCurrentProcess().Id)
				.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {ProcessId} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
				.WriteTo.File(
					path: Path.Combine(Path.GetTempPath(), "IdeaStatiCa.CalculationBulkTool.log"),
					rollingInterval: RollingInterval.Infinite,
					outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {ProcessId} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
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
				ShowErrorMessageSafe("An unexpected error occurred and the application may need to close.", ex);
			}
			else
			{
				Log.Fatal("Unhandled non-exception object in AppDomain: {Object}. IsTerminating={IsTerminating}", e.ExceptionObject, e.IsTerminating);
				ShowErrorMessageSafe("An unexpected non-exception error occurred and the application may need to close.", null);
			}
		}

		private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Log.Error(e.Exception, "Unhandled dispatcher exception");

			// Show error to the user
			var message = e.Exception?.Message ?? "An unexpected error occurred.";
			var details = e.Exception?.ToString();
			var text = details is not null ? $"{message}{Environment.NewLine}{Environment.NewLine}{details}" : message;

			try
			{
				MessageBox.Show(text, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch
			{
				// Fallback to console if UI cannot be shown
				Console.Error.WriteLine(text);
			}

			e.Handled = true;
		}

		// Helper to show a MessageBox from any thread safely
		private static void ShowErrorMessageSafe(string prefix, Exception? exception)
		{
			var message = exception?.Message ?? "Unknown error.";
			var details = exception?.ToString();
			var text = details is not null ? $"{prefix}{Environment.NewLine}{Environment.NewLine}{message}{Environment.NewLine}{Environment.NewLine}{details}" : $"{prefix}{Environment.NewLine}{Environment.NewLine}{message}";

			try
			{
				var dispatcher = Application.Current?.Dispatcher;
				if (dispatcher is not null && !dispatcher.HasShutdownStarted)
				{
					dispatcher.Invoke(() =>
					{
						MessageBox.Show(text, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
					});
				}
				else
				{
					// If no dispatcher/UI, attempt direct MessageBox or fallback to console
					MessageBox.Show(text, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch
			{
				// Fallback to console logging
				Console.Error.WriteLine(text);
			}
		}
	}
}
