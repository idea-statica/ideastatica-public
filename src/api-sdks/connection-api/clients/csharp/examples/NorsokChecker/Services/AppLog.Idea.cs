using IdeaStatiCa.Diagnostics;
using IdeaStatiCa.Diagnostics.UserEvents;

namespace NorsokChecker.Services
{
	/// <summary>
	/// The IDEA StatiCa half of <see cref="AppLog"/>: log file, Sentry and Google Analytics through
	/// <c>IdeaStatiCa.Diagnostics</c>, exactly as the product's own applications report them.
	///
	/// Compiled ONLY when this example is built inside the IDEA StatiCa monorepo, because that
	/// library is not published to any feed. From a clone of ideastatica-public the csproj drops
	/// this file and compiles <c>AppLog.Standalone.cs</c> in its place — which is what keeps the
	/// example buildable for a reader who has nothing but the public repository.
	/// </summary>
	internal static partial class AppLog
	{
		/// <summary>
		/// Sentry project "desktop_con_norsokchecker" of the idea-statica organisation. A DSN only
		/// permits submitting events, never reading them, which is why Sentry's own documentation
		/// calls a DSN safe to publish; the exposure it carries is junk events against this one
		/// project's quota, and the project is this tool's alone.
		///
		/// The environment variable takes precedence so a rotated DSN does not need a new build —
		/// Sentry recommends configuring it dynamically in a shipped client for that reason.
		/// See the Sentry data in https://idea-statica.sentry.io/projects/desktop_con_norsokchecker/
		/// </summary>
		private const string SentryDsn =
			"https://7f2d3fad2441bf229635bcebc6bab28c@o330948.ingest.us.sentry.io/4511971709157376";

		private const string SentryDsnVariable = "NORSOKCHECKER_SENTRY_DSN";

		/// <summary>
		/// Name reported to Google Analytics. It is prefixed to every event action
		/// ("NorsokChecker: application started"), so it is what separates this tool's usage
		/// from the rest of the IDEA StatiCa applications in the analytics reports.
		/// </summary>
		private const string TelemetryApplicationName = "NorsokChecker";

		/// <summary>Identification of the application for Google Analytics screen-view paths.</summary>
		private const string TelemetryApplicationId = "norsokchecker";

		private static IIdeaLogger? _logger;

		/// <summary>
		/// Initializes the diagnostics infrastructure and returns its handle; disposing the handle
		/// flushes the sinks — the Google Analytics reporter posts fire-and-forget and the Sentry
		/// sink batches, so without that the process can exit before the requests leave the machine.
		///
		/// <c>IdeaDiagnostics.Init</c> documents the Application constructor as the place to call it
		/// for a WPF application. It never throws — if initialization fails, logging and reporting
		/// are simply inactive. It also calls <c>SentrySdk.Init</c> internally, which is why this
		/// application must never do that itself.
		/// </summary>
		internal static IDisposable? Start()
		{
			var handle = IdeaDiagnostics.Init(
				logToFileName: "NorsokChecker.log",
				sentryDsn: Environment.GetEnvironmentVariable(SentryDsnVariable) ?? SentryDsn,
				logToGoogleAnalytics: true,
				applicationName: TelemetryApplicationName,
				applicationId: TelemetryApplicationId);

			// Obtained after Init, never before: a logger created earlier is not attached to the
			// configured sinks.
			_logger = IdeaDiagnostics.GetLogger(
				"norsok.checker.app", LoggerCreationOptions.CrossPlatform_Active_Logger);

			return handle;
		}

		private static void Write(bool isError, string message, Exception? exception,
			object[] propertyValues)
		{
			if (_logger == null) return;      // logging asked for before Start, or Init failed

			// The (message, exception, params) overloads are the ones that keep the {Named} holes as
			// structured properties; the exception has to be passed as null rather than omitted, or
			// the call binds to (message, params object[]) and the exception becomes a property.
			if (isError) _logger.LogError(message, exception, propertyValues);
			else _logger.LogDebug(message, exception, propertyValues);
		}

		/// <summary>
		/// The shared application-start event, the same one the product's WPF startup base sends.
		/// It reaches Google Analytics as category "Application", action "NorsokChecker: application
		/// started", with "app_started" in custom dimension 100.
		/// </summary>
		internal static void ReportApplicationStarted()
			=> _logger?.LogEventInformation(new ApplicationStartedEvent());

		/// <summary>
		/// A user event of this tool. <paramref name="category"/> is one of the
		/// <see cref="Category"/> values below, which is what puts this tool's events in the same
		/// groups as the rest of the applications.
		/// </summary>
		internal static void ReportEvent(
			string category, string eventName, string action, string? label, int value)
			=> _logger?.LogEventInformation(
				new IdeaGeneralUserEvent(category, eventName, action, label, value));

		/// <summary>
		/// The event categories, taken from the shared <see cref="EventCategories"/> rather than
		/// spelled out: <see cref="Telemetry"/> is compiled in both builds and cannot name that
		/// type, and a category that does not match one of the shared ones would land this tool's
		/// events outside the groups the analytics reports are built on. Referencing the constants
		/// here means a rename in the diagnostics library breaks the build instead.
		/// </summary>
		internal static class Category
		{
			internal static readonly string Application = EventCategories.Application;
			internal static readonly string Project = EventCategories.Project;
			internal static readonly string Calculation = EventCategories.Calculation;
		}
	}
}
