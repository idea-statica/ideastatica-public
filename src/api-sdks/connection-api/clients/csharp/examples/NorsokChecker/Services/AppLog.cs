using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.Diagnostics;
using System.IO;
using System.Net.Http;

namespace NorsokChecker.Services
{
	/// <summary>
	/// The single IDEA logger of the application, shared by the crash handlers, the failure paths and
	/// <see cref="Telemetry"/>.
	///
	/// Severity decides where a message ends up: Trace/Debug/Information stay in the log file and
	/// become Sentry breadcrumbs, while Warning and above are reported to Sentry as issues. Keep
	/// routine and expected conditions at Debug — every Warning and Error costs Sentry quota.
	/// </summary>
	internal static class AppLog
	{
		/// <summary>
		/// Obtained on first use, which is always after <c>IdeaDiagnostics.Init</c> in the
		/// <see cref="App"/> constructor — loggers must not be created before the initialization.
		/// </summary>
		internal static IIdeaLogger Logger { get; } = IdeaDiagnostics.GetLogger(
			"norsok.checker.app",
			LoggerCreationOptions.CrossPlatform_Active_Logger);

		/// <summary>
		/// Logs an operation that failed, choosing the severity by what kind of failure it is:
		/// a bad or unreadable project, a service that is not answering or a file that cannot be
		/// written are the user's data and environment, so they stay out of Sentry and are only
		/// written to the log file (and kept as breadcrumbs). Everything else is a defect in this
		/// application and is reported as an issue.
		/// </summary>
		internal static void ReportFailure(string message, Exception exception)
		{
			if (IsUserDataOrEnvironment(exception))
			{
				Logger.LogDebug(message, exception);
				return;
			}

			Logger.LogError(message, exception);
		}

		/// <summary>
		/// True for failures caused by the input or the surroundings rather than by a bug here:
		/// the project file (missing, locked, corrupt), the Connection API service (not found, not
		/// answering, rejecting the model) or the output path.
		/// </summary>
		private static bool IsUserDataOrEnvironment(Exception exception) => exception switch
		{
			ApiException => true,                    // the service rejected the request or the model
			IOException => true,                     // includes FileNotFound / DirectoryNotFound
			UnauthorizedAccessException => true,
			HttpRequestException => true,            // service not reachable
			OperationCanceledException => true,      // includes TaskCanceledException (timeouts)
			_ => false,
		};
	}
}
