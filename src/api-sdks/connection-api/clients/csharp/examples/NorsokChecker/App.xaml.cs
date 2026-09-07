using NorsokChecker.Services;
using System.Windows;
using System.Windows.Threading;

namespace NorsokChecker
{
	public partial class App : Application
	{
		/// <summary>
		/// Handle of the diagnostics infrastructure, when the build has one. Disposing it flushes the
		/// sinks — the Google Analytics reporter posts fire-and-forget and the Sentry sink batches,
		/// so without this the process can exit before the requests leave the machine.
		/// </summary>
		private IDisposable? diagnostics;

		public App()
		{
			// The Application constructor is where logging has to be initialized for a WPF app.
			// AppLog.Start never throws — if initialization fails, logging and reporting are simply
			// inactive — and which sinks it sets up depends on the build (see AppLog).
			diagnostics = AppLog.Start();

			// Reports the shared app_started user event, the same one the product's WPF startup base
			// sends. It reaches Google Analytics as category "Application", action
			// "NorsokChecker: application started", with "app_started" in custom dimension 100.
			Telemetry.ApplicationStarted();

			// Anything the per-operation handlers in MainWindow do not catch. Logging at Error level
			// is what reports it to Sentry, with this application's tags, environment and breadcrumb
			// trail attached — capturing through SentrySdk directly would lose all of that.
			DispatcherUnhandledException += OnDispatcherUnhandledException;
			AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			AppLog.LogError("Unhandled exception on the UI thread", e.Exception);

			MessageBox.Show(
				$"An unexpected error occurred and has been reported:\n\n{e.Exception.Message}",
				"NorsokChecker",
				MessageBoxButton.OK,
				MessageBoxImage.Error);

			// Keep the tool alive. A check costs minutes of CBFEM, and the per-operation handlers
			// already restore the UI state of whatever failed, so tearing the process down would
			// throw away a loaded project for no gain.
			e.Handled = true;
		}

		private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception exception)
			{
				AppLog.LogError("Unhandled exception (IsTerminating={IsTerminating})", exception, e.IsTerminating);
			}

			// The process is going down and the Sentry and Google Analytics sinks send
			// asynchronously — flush synchronously or the report never leaves.
			diagnostics?.Dispose();
			diagnostics = null;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			diagnostics?.Dispose();
			diagnostics = null;

			base.OnExit(e);
		}
	}
}
