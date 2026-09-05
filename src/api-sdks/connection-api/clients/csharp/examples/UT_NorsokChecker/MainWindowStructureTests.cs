using System.IO;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Structural facts about MainWindow that no behavioural test can express, because they are about
	/// where code lives rather than what it does.
	///
	/// Asserted on the SOURCE. That is unusual and deserves a reason: the properties here are exactly
	/// the ones that decay silently — a partial grows back into a catch-all, or a tab starts driving
	/// another again — and nothing about the running app reveals it until the next person has to work
	/// in the file.
	/// </summary>
	[TestFixture]
	public class MainWindowStructureTests
	{
		private static string AppDir()
		{
			var dir = new DirectoryInfo(Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			// Fail, not Ignore. These tests are the only guard on their wiring, and Ignore made them
			// pass silently wherever the source tree is absent — a CI artefact, another machine —
			// which is precisely the situation they exist to survive.
			if (dir == null) Assert.Fail("cannot locate the NorsokChecker source from the test output");
			return Path.Combine(dir!.FullName, "NorsokChecker");
		}

		private static string Read(string file) => File.ReadAllText(Path.Combine(AppDir(), file));

		/// <summary>
		/// The run assesses the TICKED connections, not every connection in the project.
		///
		/// Here on the source because `RunCheck_Click` is a click handler with no seam. It replaces
		/// four tests that restated `Where(c => c.Selected)` inside themselves and asserted their own
		/// copy — under the mutation that matters (iterate `_connections`) all four stayed green.
		/// This at least fails on it.
		/// </summary>
		[Test]
		public void TheRunAssessesTheSelectedConnectionsOnly()
		{
			string run = Read("MainWindow.Run.cs");

			Assert.That(run, Does.Match(@"_connections\s*\.?\s*Where\(\s*c\s*=>\s*c\.Selected\s*\)"),
				"the run must filter the project's connections by the tick box");
		}

		/// <summary>
		/// Results reads the results and drives nothing else.
		///
		/// It used to call PopulateJoint64Tab and PopulateReportTab from inside itself, so the §6.4
		/// tab could not be refreshed without refreshing Results — backwards for a tab whose whole
		/// job is to summarise what the others already computed. The run calls each in turn instead.
		/// </summary>
		[Test]
		public void TheResultsTabDoesNotDriveTheOtherTabs()
		{
			string results = Read("MainWindow.Results.cs");

			Assert.Multiple(() =>
			{
				Assert.That(results, Does.Not.Contain("PopulateJoint64Tab()"),
					"Results must not populate the §6.4 tab");
				Assert.That(results, Does.Not.Contain("PopulateReportTab()"),
					"nor the Report tab");
				Assert.That(Read("MainWindow.Run.cs"), Does.Contain("PopulateJoint64Tab()"),
					"the run is what fills them, in turn");
			});
		}

		// `TheCatchAllPartialStaysSmall` was here and is gone. It asserted MainWindow.xaml.cs stays
		// under 300 lines — an invented ceiling, currently 239, so it fires on 61 lines of any
		// growth including the legitimate kind, and says nothing about whether the file has taken
		// on a second concern. That concern is real and the tests below measure it directly: each
		// per-concern partial is asserted to exist and to hold what belongs to it.

		/// <summary>
		/// Every partial exists and covers its own concern. Named individually so a failure says
		/// which one is missing rather than "the structure changed".
		/// </summary>
		[TestCase("MainWindow.Api.cs", "CreateApiClientAsync")]
		[TestCase("MainWindow.CheckTab.cs", "LoadProject_Click")]
		[TestCase("MainWindow.Run.cs", "RunCheck_Click")]
		[TestCase("MainWindow.Results.cs", "PopulateResultsTab")]
		[TestCase("MainWindow.Report.cs", "ExportPdf_Click")]
		[TestCase("MainWindow.Joint64.cs", "PopulateJoint64Tab")]
		public void EachPartialHoldsItsOwnConcern(string file, string marker)
		{
			Assert.That(Read(file), Does.Contain(marker),
				$"{file} should be where {marker} lives");
		}

		/// <summary>
		/// Every text button on an action bar carries one of the two IDEA styles.
		///
		/// Page setup carried NO style, so it rendered in default WPF chrome beside an orange Export
		/// PDF and the two did not read as a pair — spotted in the running app, not by either
		/// review. On the source because the alternative is standing up a window; the property is
		/// structural, and the failure it guards is a button someone adds without a style.
		///
		/// The three 24x22 icon buttons in the §6.4 view are deliberately unstyled and are excluded
		/// by name: they are a different category (view manipulation, no text).
		/// </summary>
		[Test]
		public void EveryActionBarButtonCarriesAnIdeaStyle()
		{
			string xaml = Read("MainWindow.xaml");

			var unstyled = new List<string>();
			foreach (var m in System.Text.RegularExpressions.Regex.Matches(
					xaml, @"<Button\b(?<attrs>[^>]*)>", System.Text.RegularExpressions.RegexOptions.Singleline))
			{
				string attrs = ((System.Text.RegularExpressions.Match)m).Groups["attrs"].Value;
				var name = System.Text.RegularExpressions.Regex.Match(attrs, @"x:Name=""(?<n>\w+)""");
				string id = name.Success ? name.Groups["n"].Value : "(unnamed)";

				// The §6.4 view's rotate/flip icons — square, textless, intentionally plain.
				if (id is "Btn64RotL" or "Btn64RotR" or "Btn64Flip") continue;

				if (!attrs.Contains("Style=")) unstyled.Add(id);
			}

			Assert.That(unstyled, Is.Empty,
				"these buttons have no style and will not match their neighbours: "
				+ string.Join(", ", unstyled));
		}

		/// <summary>
		/// Page setup does not fix its own padding, so the shared style governs it.
		///
		/// IdeaOutlineButton sets 12,4 while the button carried an inline 12,6 — with the style
		/// applied and the inline value left in place, the pair would still differ in height and the
		/// mismatch would only have moved.
		/// </summary>
		[Test]
		public void PageSetupButtonDoesNotOverrideTheStylesPadding()
		{
			string xaml = Read("MainWindow.xaml");

			var button = System.Text.RegularExpressions.Regex.Match(xaml,
				@"<Button[^>]*x:Name=""BtnPageSetup""[^>]*>",
				System.Text.RegularExpressions.RegexOptions.Singleline);

			Assert.Multiple(() =>
			{
				Assert.That(button.Success, Is.True, "the Page setup button exists");
				Assert.That(button.Value, Does.Contain("IdeaOutlineButton"),
					"secondary to Export PDF, as Cancel is to Run Norsok Check");
				Assert.That(button.Value, Does.Not.Contain("Padding="),
					"no inline padding — the style's 12,4 must win");
			});
		}
	}
}
