using IdeaStatiCa.ConnectionApi.Client;
using System.IO;
using System.Net.Http;

namespace NorsokChecker.Services
{
	/// <summary>
	/// The application's logging and reporting seam, shared by the crash handlers, the failure paths
	/// and <see cref="Telemetry"/>.
	///
	/// Where a message ends up depends on the build, and this is the only place that knows:
	/// <c>AppLog.Idea.cs</c> is compiled when the example is built inside the IDEA StatiCa monorepo
	/// and sends everything through <c>IdeaStatiCa.Diagnostics</c> (log file, Sentry, Google
	/// Analytics); <c>AppLog.Standalone.cs</c> is compiled from a clone of ideastatica-public alone,
	/// where that library does not exist, and writes a log file and nothing else. The csproj picks
	/// one — see the <c>HasIdeaDiagnostics</c> condition there.
	///
	/// Severity decides the destination in the IDEA build: Trace/Debug/Information stay in the log
	/// file and become Sentry breadcrumbs, while Warning and above are reported to Sentry as issues.
	/// Keep routine and expected conditions at Debug — every Warning and Error costs Sentry quota.
	/// </summary>
	internal static partial class AppLog
	{
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
				LogDebug(message, exception);
				return;
			}

			LogError(message, exception);
		}

		/// <summary>
		/// Routine and expected conditions — the log file, never an issue.
		/// <paramref name="propertyValues"/> fill the <c>{Named}</c> holes of the message and are
		/// kept as structured properties wherever the build reports them.
		/// </summary>
		internal static void LogDebug(string message, Exception? exception = null,
			params object[] propertyValues)
			=> Write(isError: false, message, exception, propertyValues);

		/// <summary>A defect in this application — reported as an issue where reporting exists.</summary>
		internal static void LogError(string message, Exception? exception = null,
			params object[] propertyValues)
			=> Write(isError: true, message, exception, propertyValues);

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
