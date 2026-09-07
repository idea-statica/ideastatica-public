using System.IO;
using System.Text;

namespace NorsokChecker.Services
{
	/// <summary>
	/// The standalone half of <see cref="AppLog"/>: a log file beside the app's own data, and
	/// nothing that leaves the machine.
	///
	/// Compiled when this example is built from a clone of ideastatica-public, where
	/// <c>IdeaStatiCa.Diagnostics</c> — which is not published to any feed — does not exist. The
	/// user events are still declared and still called in this build; they simply go into the log
	/// as one line each, so a reader of the sample can see WHAT the tool considers worth reporting
	/// without anything being reported anywhere.
	///
	/// Inside the IDEA StatiCa monorepo the csproj drops this file and compiles
	/// <c>AppLog.Idea.cs</c> instead, which sends the same calls to the log file, Sentry and Google
	/// Analytics through the product's diagnostics library.
	/// </summary>
	internal static partial class AppLog
	{
		/// <summary>%LOCALAPPDATA%\IdeaStatiCa\NorsokChecker\NorsokChecker.log</summary>
		private static readonly string LogFile = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"IdeaStatiCa", "NorsokChecker", "NorsokChecker.log");

		private static readonly object Gate = new();
		private static bool _writable;

		/// <summary>
		/// Prepares the log file. Returns null — there is nothing to flush, because every line is
		/// written and closed as it comes; the handle exists only so the caller can treat both
		/// builds the same way.
		/// </summary>
		internal static IDisposable? Start()
		{
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(LogFile)!);
				_writable = true;
			}
			catch (Exception ex)
			{
				// No log is not a reason to fail to start; the app's own on-screen log still works.
				System.Diagnostics.Trace.WriteLine($"NorsokChecker: cannot write {LogFile}: {ex.Message}");
			}

			return null;
		}

		private static void Write(bool isError, string message, Exception? exception,
			object[]? propertyValues = null)
		{
			// The {Named} holes are left as they are and the values appended: this build has no
			// structured sink to send them to, and a hand-rolled substitution would be a second
			// implementation of Serilog's that could disagree with the one in the IDEA build.
			if (propertyValues is { Length: > 0 })
				message += " [" + string.Join(", ", propertyValues) + "]";

			string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {(isError ? "ERR" : "DBG")} {message}"
				+ (exception == null ? "" : $"{Environment.NewLine}{exception}");

			System.Diagnostics.Trace.WriteLine(line);
			if (!_writable) return;

			try
			{
				// Serialized: the WebView2 callbacks and the check run both report failures, and they
				// are not on the same thread.
				lock (Gate) File.AppendAllText(LogFile, line + Environment.NewLine, Encoding.UTF8);
			}
			catch (Exception)
			{
				// A log that cannot be written must not become the failure being logged.
				_writable = false;
			}
		}

		internal static void ReportApplicationStarted()
			=> Write(isError: false, "event: application started", null);

		internal static void ReportEvent(
			string category, string eventName, string action, string? label, int value)
			=> Write(isError: false,
				$"event: {category}/{eventName} \"{action}\""
				+ (label == null ? "" : $" [{label}]") + (value == 0 ? "" : $" = {value}"), null);

		/// <summary>
		/// The event categories. In the IDEA build these are the shared
		/// <c>IdeaStatiCa.Diagnostics.UserEvents.EventCategories</c> constants; the literals here
		/// mirror them, and are the only place the two builds can drift apart.
		/// </summary>
		internal static class Category
		{
			internal static readonly string Application = "Application";
			internal static readonly string Project = "Project";
			internal static readonly string Calculation = "Calculation";
		}
	}
}
