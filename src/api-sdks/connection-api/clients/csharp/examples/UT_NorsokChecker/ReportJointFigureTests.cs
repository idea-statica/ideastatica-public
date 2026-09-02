using System.IO;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Which connections get a joint figure in the report.
	///
	/// Reported 2026-09-01: the figure reached CON1 and nothing else. The cause was not the renderer —
	/// RenderJointFigures read _meshesPerConnection, a cache filled ONLY by MeshesForAsync, which only
	/// the Check tab's row selection and the §6.4 tab's combo ever call. A connection nobody had
	/// clicked had no bodies cached, so the loop skipped it; CON1 is the one selected automatically
	/// when a project opens, which is why exactly one figure appeared.
	///
	/// The §6.4 tab had this defect for the same reason and MeshesForAsync — fetch-then-cache — is
	/// what fixed it there. This asserts the report goes through the same door.
	///
	/// Asserted on the SOURCE. The behavioural path needs a running service: MeshesForAsync fetches
	/// the 1.7 MB presentation payload over HTTP and returns empty without an API client, so an
	/// offline test would see no figure either way and could not tell the fixed code from the broken.
	/// What IS decidable offline is where the bodies come from, and that is the whole defect.
	/// </summary>
	[TestFixture]
	public class ReportJointFigureTests
	{
		private static string ReportSource()
		{
			var dir = new DirectoryInfo(Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Ignore("cannot locate the NorsokChecker source from the test output");
			return File.ReadAllText(Path.Combine(dir!.FullName, "NorsokChecker", "MainWindow.Report.cs"));
		}

		/// <summary>
		/// The report FETCHES the bodies rather than reading whatever the user's clicking left behind.
		///
		/// Both halves matter and neither alone is the fix: calling MeshesForAsync is what makes a
		/// figure possible for every assessed connection, and not touching the cache directly is what
		/// stops the old skip from creeping back beside it.
		/// </summary>
		[Test]
		public void TheReportFetchesTheBodiesItDraws()
		{
			string src = ReportSource();

			Assert.Multiple(() =>
			{
				Assert.That(src, Does.Contain("MeshesForAsync"),
					"the report must fetch the bodies — the cache is filled by selection alone, so "
					+ "reading it gives a figure only to the connections the user happened to click");
				Assert.That(src, Does.Not.Contain("_meshesPerConnection"),
					"and it must not read that cache directly, which is what limited the figure to CON1");
			});
		}

		/// <summary>
		/// Every connection with a topology is offered a figure — the loop skips on the RESULT of the
		/// fetch, never before it.
		///
		/// This is the shape of the original defect: a `continue` that ran before anything was fetched
		/// decided the connection had no bodies when nobody had ever asked for them.
		/// </summary>
		[Test]
		public void NoConnectionIsSkippedBeforeItsBodiesAreFetched()
		{
			string src = ReportSource();

			int loop = src.IndexOf("foreach (var (conId, topo) in _topologyPerConnection)",
				StringComparison.Ordinal);
			Assert.That(loop, Is.GreaterThan(0), "the figure loop is over the connections with a topology");

			int fetch = src.IndexOf("MeshesForAsync", loop, StringComparison.Ordinal);
			Assert.That(fetch, Is.GreaterThan(0), "and it fetches inside that loop");

			// Between the loop header and the fetch there may be one skip only: the connection has no
			// NAME, which is the report's key for the figure and costs no payload to check. A skip on
			// the bodies has to come after they were asked for.
			string beforeFetch = src[loop..fetch];
			int skips = System.Text.RegularExpressions.Regex.Matches(beforeFetch, @"\bcontinue\b").Count;

			Assert.That(skips, Is.LessThanOrEqualTo(1),
				"a connection may be skipped before the fetch only for having no name; skipping on its "
				+ "bodies before asking for them is the defect that left the figure on CON1 alone");
		}
	}
}
