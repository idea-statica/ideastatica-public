using NorsokChecker.Models;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The report after a run that produced nothing, then a run that produced something.
	///
	/// Reported 2026-09-01: running the check with §6.4 UNTICKED, then ticking it and running again,
	/// left the Report tab empty. The report generator itself is fine — measured on the same data it
	/// emits five derivation blocks and 71 steps — so whatever drops the content is in the sequence
	/// the run drives, which is what these pin.
	///
	/// STA: constructs WPF controls.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class ReportAfterEmptyRunTests
	{
		[OneTimeSetUp]
		public void EnsureApplication()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}
		}

		private static NorsokFormulaResult Row() => new()
		{
			Section = "6.4.3.6", Equation = "6.57", Title = "Tubular Joint — M1",
			LoadCaseName = "LE1", Utilization = 0.735, Passed = true,
		};

		/// <summary>
		/// The HTML the Report tab would show. Built through the window's own BuildReportHtml, so
		/// this measures what the app produces rather than what the generator can produce.
		/// </summary>
		private static string ReportHtmlAfter(params Action<NorsokChecker.MainWindow>[] steps)
		{
			var w = new NorsokChecker.MainWindow();
			foreach (var step in steps) step(w);
			// Synchronous on purpose: with no API client the figure fetch returns empty without ever
			// awaiting anything, so the task is already complete and this cannot deadlock the STA
			// thread the fixture runs on.
			return w.BuildReportHtmlForTest().GetAwaiter().GetResult();
		}

		/// <summary>
		/// The reported sequence: a run with no chapter ticked, then a run with one.
		///
		/// The first run stores an empty result set for every selected connection. The second must
		/// replace it — if the report still reflects the first, the second run's results never
		/// reached it.
		/// </summary>
		[Test]
		public void AnEmptyRunFollowedByARealOneReportsTheRealOne()
		{
			string html = ReportHtmlAfter(
				w => w.SetResultsForTest(1, "CON1", new List<NorsokFormulaResult>()),   // chapter off
				w => w.SetResultsForTest(1, "CON1", new List<NorsokFormulaResult> { Row() }));

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("<details class='check-card"),
					"the second run's check is in the report");
				Assert.That(html, Does.Contain("Tubular Joint"), "and it is the row that was computed");
			});
		}

		/// <summary>
		/// A run with no chapter ticked produces a report with no checks — but still a report. An
		/// empty result set is a legitimate outcome ("you ticked nothing"), not a failure to render.
		/// </summary>
		[Test]
		public void ARunWithNoChapterStillProducesAReport()
		{
			string html = ReportHtmlAfter(
				w => w.SetResultsForTest(1, "CON1", new List<NorsokFormulaResult>()));

			Assert.Multiple(() =>
			{
				Assert.That(html, Is.Not.Empty, "an empty run is still a document");
				Assert.That(html, Does.Contain("CON1"), "and it still names the connection");
				// the TAG, not the class name: the stylesheet every report carries mentions
				// .check-card, so matching the bare string can never be false
				Assert.That(html, Does.Not.Contain("<details class='check-card"),
					"with no checks in it");
			});
		}
	}
}
