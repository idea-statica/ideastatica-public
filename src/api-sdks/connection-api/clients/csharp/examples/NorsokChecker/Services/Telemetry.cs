using IdeaStatiCa.Diagnostics;
using IdeaStatiCa.Diagnostics.UserEvents;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Google Analytics user events for this tool, so its usage can be measured next to the rest of
	/// the IDEA StatiCa applications.
	///
	/// Naming: every event name is prefixed <c>norsok_</c> — the diagnostics library reports the name
	/// in custom dimension 100, which is what identifies the event in the analytics reports. The
	/// action text is prefixed with the application name by the library itself, so it arrives as
	/// "NorsokChecker: run check clicked".
	///
	/// Content: only the fact, the category and the outcome of an action are reported. No project
	/// paths, connection or member names, load values or exception messages ever go out — a failure
	/// is identified by its exception type alone.
	/// </summary>
	internal static class Telemetry
	{
		private static IIdeaLogger Logger => AppLog.Logger;

		/// <summary>
		/// The shared application-start event, the same one the product's WPF applications report.
		/// </summary>
		internal static void ApplicationStarted()
		{
			Logger.LogEventInformation(new ApplicationStartedEvent());
		}

		/// <summary>Load of a project was attempted (the picked file exists).</summary>
		internal static void ProjectLoadClicked()
		{
			Report(EventCategories.Project, "norsok_project_load", "load project clicked");
		}

		/// <summary>
		/// Project opened and members read. <paramref name="connectionCount"/> is reported as the
		/// event value, and the scope the geometry allows as the label: all-CHS models get the §6.4
		/// tubular joint checks on top of §6.3, mixed ones only §6.3.
		/// </summary>
		internal static void ProjectLoaded(int connectionCount, bool allTubular)
		{
			Report(EventCategories.Project, "norsok_project_loaded", "project loaded",
				label: allTubular ? "tubular" : "mixed", value: connectionCount);
		}

		/// <summary>Opening the project or reading its members failed.</summary>
		internal static void ProjectLoadFailed(Exception exception)
		{
			Report(EventCategories.Project, "norsok_project_load_failed", "project load failed",
				label: exception.GetType().Name);
		}

		/// <summary>The NORSOK check was started from the Run button.</summary>
		internal static void CheckClicked()
		{
			Report(EventCategories.Calculation, "norsok_check_run", "run check clicked");
		}

		/// <summary>
		/// The check finished. The label carries the overall verdict and the value the governing
		/// utilization in whole percent, so the distribution of results is visible in the reports.
		/// </summary>
		internal static void CheckCompleted(bool allPassed, double governingUtilization)
		{
			Report(EventCategories.Calculation, "norsok_check_completed", "check completed",
				label: allPassed ? "pass" : "fail",
				value: (int)Math.Round(governingUtilization * 100.0));
		}

		/// <summary>The check was interrupted by an error (API, calculation or evaluation).</summary>
		internal static void CheckFailed(Exception exception)
		{
			Report(EventCategories.Calculation, "norsok_check_failed", "check failed",
				label: exception.GetType().Name);
		}

		/// <summary>PDF export was confirmed in the save dialog.</summary>
		internal static void ReportExportClicked()
		{
			Report(EventCategories.Project, "norsok_report_export", "export pdf clicked");
		}

		/// <summary>Both the NORSOK and the IDEA StatiCa CBFEM PDF were written.</summary>
		internal static void ReportExported()
		{
			Report(EventCategories.Project, "norsok_report_exported", "pdf report exported");
		}

		/// <summary>PDF export failed.</summary>
		internal static void ReportExportFailed(Exception exception)
		{
			Report(EventCategories.Project, "norsok_report_export_failed", "pdf report export failed",
				label: exception.GetType().Name);
		}

		/// <summary>
		/// Automatic joint topology was switched on or off. Turning it off means the user fell back
		/// to classifying the joint by hand, which is the signal that the automatic classification
		/// was not trusted or not accepted for that model.
		/// </summary>
		internal static void AutoTopologyToggled(bool enabled)
		{
			Report(EventCategories.Application, "norsok_auto_topology_toggled", "auto topology toggled",
				label: enabled ? "on" : "off");
		}

		private static void Report(string category, string eventName, string action, string? label = null, int value = 0)
		{
			Logger.LogEventInformation(new IdeaGeneralUserEvent(category, eventName, action, label, value));
		}
	}
}
