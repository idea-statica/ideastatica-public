using System.IO;
using NorsokChecker.Models;
using NorsokChecker.Services.Chapters;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The guarantee the chapter interface exists for: a new chapter is one new file plus one line in
	/// the registry, and nothing else changes.
	///
	/// Before the registry a chapter was described in four places that had to agree — a checkbox in
	/// the XAML, a branch in the run, a string-prefix test in the results router, and a four-entry
	/// array in the report generator — with nothing to notice when they did not. Missing the last one
	/// filed the new chapter's rows under "Other Checks" and said nothing.
	/// </summary>
	[TestFixture]
	public class ChapterRegistryTests
	{
		/// <summary>
		/// A chapter that exists only in this test file reaches the router and the report grouping.
		///
		/// This is the acceptance test for the whole design: if it needed an edit anywhere in the app
		/// to pass, the interface would not have delivered what it promised.
		/// </summary>
		private sealed class ThrowawayChapter : IChapter
		{
			public string Key => "9.9";
			public string DisplayName => "§9.9 Throwaway";
			public string ReportGroup => "§9.9 — Throwaway";
			public bool HasOwnTab => false;

			public Task<ChapterOutcome> EvaluateAsync(ChapterContext ctx, CancellationToken ct) =>
				Task.FromResult(new ChapterOutcome
				{
					Rows = new[]
					{
						new NorsokFormulaResult { Section = "9.9", Title = "one row", Passed = true },
					},
				});
		}

		[Test]
		public void AChapterRoutesItsOwnRowsWithoutTouchingTheRouter()
		{
			var chapter = new ThrowawayChapter();

			// The router keys off the registry, so a row tagged with a chapter's key finds it. The
			// old router tested `Section.StartsWith("6.4")` and could only ever recognise §6.4.
			Assert.Multiple(() =>
			{
				Assert.That(ChapterRegistry.ForSection("6.4"), Is.Not.Null, "§6.4 is routed");
				Assert.That(ChapterRegistry.ForSection("6.4.3")?.Key, Is.EqualTo("6.4"),
					"and so is a subsection of it — the norm numbers its own subsections");
				Assert.That(ChapterRegistry.ForSection(chapter.Key), Is.Null,
					"an unregistered chapter routes nowhere, which is why registering is the one step");
			});
		}

		/// <summary>
		/// Every registered chapter is completely described: a key, a label, a report heading. A
		/// missing heading is the failure that motivated the registry — the rows still appear, just
		/// under the wrong one.
		/// </summary>
		[Test]
		public void EveryChapterIsFullyDescribed()
		{
			Assert.That(ChapterRegistry.All, Is.Not.Empty);

			Assert.Multiple(() =>
			{
				foreach (var c in ChapterRegistry.All)
				{
					Assert.That(c.Key, Is.Not.Empty, "a chapter needs a key");
					Assert.That(c.DisplayName, Is.Not.Empty, $"{c.Key} needs a toggle label");
					Assert.That(c.ReportGroup, Is.Not.Empty,
						$"{c.Key} needs a report heading, or its rows fall into 'Other Checks'");
				}

				Assert.That(ChapterRegistry.All.Select(c => c.Key).Distinct().Count(),
					Is.EqualTo(ChapterRegistry.All.Count), "keys must be unique — the router uses them");
			});
		}

		/// <summary>
		/// The consumers read the registry rather than naming a chapter.
		///
		/// Asserted on the source, because a hardcoded "6.4" that happens to agree with the registry
		/// today still breaks the next chapter, and no behavioural test can see the difference while
		/// §6.4 is the only one registered.
		///
		/// Each case checks BOTH that the registry is used and that the thing it replaced is absent.
		/// The presence half alone is too weak: an oracle run put the hardcoded group array back and
		/// left an unused `ChapterRegistry.All` beside it, and the test passed.
		/// </summary>
		[TestCase("MainWindow.Run.cs", "SelectedChapters()", "ChkChapter64",
			"the run iterates the selection, and names no chapter")]
		[TestCase("MainWindow.CheckTab.cs", "ChapterRegistry.All", "new Services.Chapters.IChapter[]",
			"the toggles come from the registry, not from a literal list")]
		[TestCase("Services/NorsokHtmlReportGenerator.cs",
			"var groups = Chapters.ChapterRegistry.All", "(key: \"6.4\"",
			"the report groups by the registry, with no hardcoded group array")]
		public void TheConsumersReadTheRegistry(string file, string required, string forbidden, string why)
		{
			var dir = new DirectoryInfo(Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Ignore("cannot locate the NorsokChecker source");

			string text = File.ReadAllText(Path.Combine(dir!.FullName, "NorsokChecker", file));

			Assert.Multiple(() =>
			{
				Assert.That(text, Does.Contain(required), why);
				Assert.That(text, Does.Not.Contain(forbidden),
					$"{file} still carries what the registry replaced");
			});
		}

		/// <summary>
		/// A chapter can report a check it deliberately did NOT make.
		///
		/// The part of the design §6.3 forced in: it can evaluate nine of its eleven checks without a
		/// member length, but the compression buckling check cannot run at all, and
		/// CHAPTER_63_REVISIT.md is explicit that the omission must be visible where the result is —
		/// the gap reaches 28 % of capacity with nothing in the number to show it. An outcome that
		/// could only say passed / failed / rejected would have no way to express that.
		/// </summary>
		[Test]
		public void AnOutcomeCanCarryChecksThatWereNotPerformed()
		{
			var outcome = new ChapterOutcome
			{
				Rows = new[] { new NorsokFormulaResult { Section = "6.3", Passed = true } },
				NotPerformed = new[]
				{
					new NotPerformed("§6.3.3 axial compression, eq (6.2)",
						"no unbraced length is available for this member"),
				},
			};

			Assert.Multiple(() =>
			{
				Assert.That(outcome.NothingAssessed, Is.False, "something WAS assessed");
				Assert.That(outcome.NotPerformed, Has.Count.EqualTo(1),
					"and the gap is reported alongside it, not silently dropped");
				Assert.That(outcome.NotPerformed[0].Why, Does.Contain("length"));
			});
		}
	}
}
