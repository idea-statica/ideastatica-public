using IdeaStatiCa.Diagnostics;
using NorsokChecker.Services;
using System.Windows;
using System.Windows.Threading;

namespace NorsokChecker
{
	public partial class App : Application
	{
		/// <summary>
		/// Sentry project "desktop_con_norsokchecker" of the idea-statica organisation. A DSN only
		/// permits submitting events, never reading them, so it is safe in a public repository —
		/// every other IDEA application hardcodes its own the same way.
		/// See the Sentry data in https://idea-statica.sentry.io/projects/desktop_con_norsokchecker/
		/// </summary>
		private const string SentryDsn =
			"https://7f2d3fad2441bf229635bcebc6bab28c@o330948.ingest.us.sentry.io/4511971709157376";

		/// <summary>
		/// Name reported to Google Analytics. It is prefixed to every event action
		/// ("NorsokChecker: application started"), so it is what separates this tool's usage
		/// from the rest of the IDEA StatiCa applications in the analytics reports.
		/// </summary>
		private const string TelemetryApplicationName = "NorsokChecker";

		/// <summary>
		/// Identification of the application for Google Analytics screen-view paths.
		/// </summary>
		private const string TelemetryApplicationId = "norsokchecker";

		/// <summary>
		/// Handle of the diagnostics infrastructure. Disposing it flushes the sinks — the Google
		/// Analytics reporter posts fire-and-forget and the Sentry sink batches, so without this the
		/// process can exit before the requests leave the machine.
		/// </summary>
		private IDisposable? diagnostics;

		public App()
		{
			// IdeaDiagnostics.Init documents the Application constructor as the place to initialize
			// logging for WPF applications. It never throws — if initialization fails, logging and
			// reporting are simply inactive. It also calls SentrySdk.Init internally, which is why
			// this application must never do that itself.
			diagnostics = IdeaDiagnostics.Init(
				logToFileName: "NorsokChecker.log",
				sentryDsn: SentryDsn,
				logToGoogleAnalytics: true,
				applicationName: TelemetryApplicationName,
				applicationId: TelemetryApplicationId);

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
			AppLog.Logger.LogError("Unhandled exception on the UI thread", e.Exception);

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
				AppLog.Logger.LogError("Unhandled exception (IsTerminating={IsTerminating})", exception, e.IsTerminating);
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
