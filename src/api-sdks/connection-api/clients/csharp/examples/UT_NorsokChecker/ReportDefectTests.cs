using NorsokChecker.Models;
using NorsokChecker.Services;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The report as a document someone hands over — five defects it carried, each of which made it
	/// state something untrue or withhold something it already had.
	///
	/// Built from result rows rather than through a live service, because every one of these is about
	/// what the generator does with a row, not about what the engine computes.
	/// </summary>
	[TestFixture]
	public class ReportDefectTests
	{
		private static NorsokFormulaResult Rejected(string condition) => new()
		{
			Section = "6.4", Equation = "6.4.3", Title = "Outside the scope of §6.4",
			CheckExpression = condition,
			FormulaSubstituted = "no §6.4 check was performed for this joint",
			NotAssessed = true,
		};

		private static NorsokFormulaResult Assessed(double util) => new()
		{
			Section = "6.4.3.6", Equation = "6.57", Title = "Tubular Joint — M1",
			LoadCaseName = "LE1", Utilization = util, Passed = true,
		};

		private static string Report(params (string Con, NorsokFormulaResult[] Rows)[] connections) =>
			NorsokHtmlReportGenerator.GenerateReport(
				"test.ideaCon",
				connections.Select(c => (c.Con, c.Rows.ToList())).ToList(),
				expandAll: false);

		/// <summary>
		/// A joint outside the chapter's scope is ONE card, listing every condition.
		///
		/// CON6 produced seven, all headed "Outside the scope of §6.4 (n of 7)", each opening onto the
		/// same sentence. Seven cards saying the chapter does not apply, where one says it and names
		/// the seven reasons — the reasons being the part that was missing.
		/// </summary>
		[Test]
		public void RejectedJointIsOneCardListingEveryCondition()
		{
			string html = Report(("CON6", new[]
			{
				Rejected("M4-M6: feet overlap (gap -16 mm < 0)"),
				Rejected("M1: 20.0° off plane (>15°)"),
				Rejected("M6: out-of-plane ecc. 10 mm (>5 mm)"),
			}));

			int cards = System.Text.RegularExpressions.Regex
				.Matches(html, "<details class='check-card").Count;

			Assert.Multiple(() =>
			{
				Assert.That(cards, Is.EqualTo(1), "three conditions, one card");
				Assert.That(html, Does.Contain("3 conditions not met"),
					"the count is the orientation and belongs in the heading");
				Assert.That(html, Does.Contain("feet overlap"), "condition 1");
				Assert.That(html, Does.Contain("off plane"), "condition 2");
				Assert.That(html, Does.Contain("out-of-plane ecc"), "condition 3");
				Assert.That(html, Does.Not.Contain("(1 of 3)"),
					"and the old per-condition numbering is gone");
			});
		}

		/// <summary>
		/// A single rejection still names its reason. It arrives on the row as CheckExpression and the
		/// report used to drop it, so the card said "Outside the scope of §6.4" and, opened,
		/// "no §6.4 check was performed for this joint" — the question restated as its own answer.
		/// </summary>
		[Test]
		public void ASingleRejectionNamesItsReason()
		{
			string html = Report(("CON2", new[] { Rejected("M7: θ=0.0° — parallel to chord") }));

			Assert.That(html, Does.Contain("parallel to chord"));
		}

		/// <summary>
		/// Where nothing was assessed the result bar carries the reason, not a utilisation.
		///
		/// "Utilization: — (not assessed)" spent the one line a reader looks at on a non-answer. (An
		/// earlier version printed "0.0% (= 0.0000 ≤ 1.0)", which claimed a check had been made and
		/// had passed comfortably, beside the word N/A.)
		/// </summary>
		[Test]
		public void AnUnassessedCardStatesTheReasonInsteadOfAUtilisation()
		{
			string html = Report(("CON2", new[] { Rejected("no brace (chord only)") }));

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("(not assessed)"),
					"the filler line is gone");
				Assert.That(html, Does.Contain("no brace"), "the reason took its place");
			});
		}

		/// <summary>
		/// Print CSS forces every card open, independently of the markup.
		///
		/// The export does pass expandAll and the markup does carry &lt;details open&gt;, but a closed
		/// card in a PDF cannot be opened — the derivation would simply be gone, with nothing in the
		/// file to say it had ever been there. Making print independent of the attribute means it
		/// cannot be lost to a stray click or to a future change in how the page is built.
		/// </summary>
		[Test]
		public void PrintForcesEveryCardOpen()
		{
			string html = Report(("CON1", new[] { Assessed(0.735) }));

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("@media print"));
				Assert.That(html, Does.Contain("details.check-card > *:not(summary) { display: block !important; }"),
					"the card body is forced visible in print");
			});
		}

		/// <summary>
		/// The report does not claim the CBFEM engine computed anything.
		///
		/// Two places said it — the footer ("Structural analysis results … are computed by the IDEA
		/// StatiCa Connection CBFEM engine") and the norm box ("Engine: IDEA StatiCa Connection CBFEM
		/// Analysis via REST API"). Both described the mothballed chapter: the app runs no calculation
		/// at all now, and §6.4 needs none. A report that misstates where its numbers come from is
		/// worse than one that says less.
		/// </summary>
		[Test]
		public void TheReportDoesNotClaimTheCbfemEngineComputedTheChecks()
		{
			string html = Report(("CON1", new[] { Assessed(0.735) }));

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("CBFEM engine"),
					"the footer's claim about where the results come from");
				Assert.That(html, Does.Not.Contain("CBFEM Analysis via REST API"),
					"and the norm box's 'Engine:' line");
				Assert.That(html, Does.Contain("evaluated by NorsokChecker"),
					"what it says instead: the checks are computed here");
				Assert.That(html, Does.Contain("read from <strong>IDEA StatiCa Connection</strong>"),
					"and the model is what comes from the API");
			});
		}
	}
}
