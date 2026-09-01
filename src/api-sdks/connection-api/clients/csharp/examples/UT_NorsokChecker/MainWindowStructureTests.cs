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
			if (dir == null) Assert.Ignore("cannot locate the NorsokChecker source from the test output");
			return Path.Combine(dir!.FullName, "NorsokChecker");
		}

		private static string Read(string file) => File.ReadAllText(Path.Combine(AppDir(), file));

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

		/// <summary>
		/// MainWindow.xaml.cs stays small — it holds the constructor, the shared state and the few
		/// things every partial uses, and nothing else.
		///
		/// The number is a ceiling with room in it, not a target: the point is to fail when the file
		/// starts absorbing features again, which is how it reached 1,676 lines carrying eight
		/// concerns. If a change genuinely belongs there, raise the limit deliberately and say why.
		/// </summary>
		[Test]
		public void TheCatchAllPartialStaysSmall()
		{
			int lines = File.ReadAllLines(Path.Combine(AppDir(), "MainWindow.xaml.cs")).Length;

			Assert.That(lines, Is.LessThan(300),
				$"MainWindow.xaml.cs is {lines} lines — new code belongs in one of the per-concern "
				+ "partials (Api / CheckTab / Run / Results / Report) or in a service");
		}

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
	}
}
