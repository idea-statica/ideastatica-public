using System.IO;

namespace UT_NorsokChecker
{
	/// <summary>
	/// A joint figure is made only for a connection that was actually assessed.
	///
	/// The report already declines to PRINT a figure for an unassessed joint — it has no utilisation,
	/// so there is nothing to colour, and an uncoloured picture beside a "not assessed" card would
	/// imply there was something to see. What was missing is the other half: not MAKING the one the
	/// report will not use. Measured in FigureLiveProbe, each figure costs 0.3–1.3 s of fetch, parse
	/// and render, so a project with several rejected joints paid seconds for pictures nobody sees.
	///
	/// The distinction that matters here, and the reason a topology check is not enough: a topology is
	/// built for a REJECTED joint too, precisely so its unmet conditions can be listed. Only the
	/// verdict separates them — N/A is CheckWorkflow's "assessed == 0".
	///
	/// Asserted on the source: the saving is about work NOT done, and a test cannot observe the
	/// absence of a render without a live service to make the present one real.
	/// </summary>
	[TestFixture]
	public class FigureOnlyWhenAssessedTests
	{
		private static string ReportSource()
		{
			var dir = new DirectoryInfo(Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Ignore("cannot locate the NorsokChecker source from the test output");

			// Comments stripped before any offset is taken: the prose here names N/A and the verdict
			// while explaining the guard, so a raw match finds the explanation instead of the code.
			return System.Text.RegularExpressions.Regex.Replace(
				File.ReadAllText(Path.Combine(dir!.FullName, "NorsokChecker", "MainWindow.Report.cs")),
				@"//[^\n]*", "");
		}

		/// <summary>
		/// The verdict is checked, and BEFORE the work — not as a filter after the figure exists.
		///
		/// Both halves are the point. Checking it at all is what skips the joint; checking it first is
		/// what saves the second or so the skipped joint would otherwise have cost.
		/// </summary>
		[Test]
		public void AnUnassessedJointIsSkippedBeforeAnyWork()
		{
			string code = ReportSource();

			int start = code.IndexOf("private async Task RenderJointFigureAsync", StringComparison.Ordinal);
			Assert.That(start, Is.GreaterThan(0), "the per-connection figure renderer");

			int guard = code.IndexOf("\"N/A\"", start, StringComparison.Ordinal);
			int fetch = code.IndexOf("MeshesForAsync", start, StringComparison.Ordinal);
			int render = code.IndexOf("RenderToPng", start, StringComparison.Ordinal);

			Assert.Multiple(() =>
			{
				Assert.That(guard, Is.GreaterThan(0),
					"it must test the verdict — a topology exists for a rejected joint too, so it "
					+ "cannot tell an assessed joint from an unassessed one");
				Assert.That(guard, Is.LessThan(fetch),
					"and before the 1.7 MB body fetch");
				Assert.That(guard, Is.LessThan(render),
					"and before the render, which is the second or so being saved");
			});
		}

		/// <summary>
		/// The figure comes from a FETCH, not from the selection-filled body cache.
		///
		/// This is the original defect and it survived a revert: reading _meshesPerConnection gave a
		/// figure only to the connections the user had clicked. Confirmed on screen 2026-09-02 — CON8
		/// assessed cleanly, five checks green, and had no figure because nobody had selected it.
		/// </summary>
		[Test]
		public void TheFigureIsFetchedRatherThanReadFromTheClickCache()
		{
			string code = ReportSource();

			Assert.Multiple(() =>
			{
				Assert.That(code, Does.Contain("MeshesForAsync"),
					"the figure fetches its bodies");
				Assert.That(code, Does.Not.Contain("_meshesPerConnection"),
					"and never reads the cache only selection fills — that is what limited the "
					+ "figure to the connections the user happened to click");
			});
		}
	}
}
