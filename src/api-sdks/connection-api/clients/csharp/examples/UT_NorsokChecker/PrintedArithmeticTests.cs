using System.Globalization;
using System.Text.RegularExpressions;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// A PRINTED SUBSTITUTION MUST EVALUATE TO ITS PRINTED RESULT.
	///
	/// One rule, and its absence is why five separate defects reached an exported report at once:
	/// the eq (6.57) terms did not sum to the utilisation beside them, Q_f was applied to the K-mode
	/// resistance without appearing in the substitution, the chord thickness printed as 7 where the
	/// result used 6.5, Q_g printed as a bare value, and the out-of-range brace mixed one pass's
	/// prefactor with the other's result. Every one of them is invisible to a test that asserts a
	/// formula is PRESENT — which is what the derivation tests did — and obvious to a reader who
	/// adds up the three numbers, which is the first thing a checking engineer does.
	///
	/// THE FIXTURE IS THE POINT: a MULTI-MODE brace (K/Y/X fractions all non-zero). On a single-mode
	/// brace the dominant-mode and weighted N_Rd are the same number, the sum balances, and the
	/// defect cannot appear — 19 of the 40 checks in the reviewed report were single-mode and every
	/// one of them added up. DerivationContentTests uses frX = 1.0, which is why it never saw this.
	/// </summary>
	[TestFixture]
	public class PrintedArithmeticTests
	{
		private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

		/// <summary>
		/// A brace classified across all three modes, run through the real engine.
		///
		/// The K fraction carries a gap so the K branch computes a real Q_g, and the axial force is
		/// large enough that the axial term dominates the sum — a small axial term would make the
		/// discrepancy fall inside a rounding tolerance and the test would pass on a defect.
		/// </summary>
		private static JointCheckRow MultiModeRow()
		{
			var inp = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.102, t: 0.0065, fyBrace: 355e6,
				thetaDeg: 45.0, g: 0.047,
				frK: 0.19, frY: 0.38, frX: 0.43,
				nSd: -88.8e3, mipSd: -1.2e3, mopSd: 2.4e3,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0.0,
				gammaM: 1.15);

			var result = Norsok64Engine.CheckJoint(inp);

			return new JointCheckRow
			{
				Name = "M3",
				Skipped = false,
				Engine = result,
				Inputs = inp,
				Classification = new KyxClass
				{
					Name = "M3", FrK = 0.19, FrY = 0.38, FrX = 0.43,
					NSd = -88.8e3, MipSd = -1.2e3, MopSd = 2.4e3,
				},
				DomClass = "X",          // the largest fraction, as the app picks it
				Util = result.UtilWeighted,
				Passed = result.Passed,
				NRdWeighted = result.NRdWeighted,
				MRdIp = result.MRdIp,
				MRdOp = result.MRdOp,
				WithinRange = result.WithinRange,
				ChordOverstressed = result.ChordOverstressed,
			};
		}

		private static string Page() =>
			NorsokHtmlReportGenerator.GenerateDerivationPage(MultiModeRow(), brace: "M3",
				connection: "CON11", state: "LE11", utilisation: "73.7 %", verdict: "PASS");

		/// <summary>
		/// The fixture really is multi-mode and the two resistances really do differ — asserted
		/// FIRST, because if they coincided every test below would pass while measuring nothing.
		/// </summary>
		[Test]
		public void TheFixtureIsMultiModeAndTheTwoResistancesDiffer()
		{
			var row = MultiModeRow();
			var dom = row.Engine!.PerClass[Joint64Class.X];

			Assert.Multiple(() =>
			{
				Assert.That(row.Classification!.FrK, Is.GreaterThan(0.0), "K fraction present");
				Assert.That(row.Classification!.FrY, Is.GreaterThan(0.0), "Y fraction present");
				Assert.That(row.Classification!.FrX, Is.GreaterThan(0.0), "X fraction present");
				Assert.That(row.Engine!.NRdWeighted, Is.Not.EqualTo(dom.NRd).Within(1.0),
					"the weighted and dominant-mode resistances must differ, or this fixture "
					+ "cannot show the defect it exists for");
			});
		}

		/// <summary>
		/// The three printed interaction terms add up to the printed total.
		///
		/// This is the reader's first check and the document failed it on 21 of 40 checks. The
		/// printed axial term was N_Sd / N_Rd(dominant mode) while the total used
		/// N_Sd / N_Rd(weighted): on the governing brace of the reviewed report that printed
		/// 32.44 + 1.43 + 47.30 = 73.73 %, three numbers that add to 81.17. The error ran BOTH ways
		/// across the document (+7.44 pp on one page, -5.36 pp on another), so a checker could not
		/// even assume the breakdown was conservative.
		/// </summary>
		[Test]
		public void TheThreePrintedTermsSumToThePrintedTotal()
		{
			string html = Page();

			var (terms, total) = InteractionLine(html);

			Assert.That(terms, Has.Length.EqualTo(3),
				"the substitution prints three terms — axial, in-plane, out-of-plane");
			Assert.That(terms.Sum(), Is.EqualTo(total).Within(0.02),
				$"the printed terms {string.Join(" + ", terms.Select(t => t.ToString("F2", Inv)))}"
				+ $" = {terms.Sum().ToString("F2", Inv)} must equal the printed total "
				+ $"{total.ToString("F2", Inv)} — a reader adds these up");
		}

		/// <summary>
		/// And the printed axial term is the one the engine actually used: N_Sd over the WEIGHTED
		/// resistance, not over the dominant mode's.
		///
		/// Separate from the sum test on purpose. The sum could be made to balance by changing the
		/// total to match a wrong term, which would be the same defect with the error moved; this
		/// pins which of the two values is correct.
		/// </summary>
		[Test]
		public void ThePrintedAxialTermUsesTheWeightedResistance()
		{
			var row = MultiModeRow();
			var (terms, _) = InteractionLine(Page());

			double weighted = Math.Abs(row.Inputs!.NSd) / row.Engine!.NRdWeighted * 100.0;
			double dominant = row.Engine!.PerClass[Joint64Class.X].UtilAxialTerm * 100.0;

			Assert.Multiple(() =>
			{
				Assert.That(terms[0], Is.EqualTo(weighted).Within(0.02),
					"the axial term is N_Sd / N_Rd(weighted)");
				Assert.That(terms[0], Is.Not.EqualTo(dominant).Within(0.02),
					"and NOT N_Sd / N_Rd(dominant mode), which is what it used to print — if these "
					+ "two are equal the fixture stopped being multi-mode");
			});
		}

		/// <summary>
		/// No substituted thickness is printed rounded when the result depends on its square.
		///
		/// `355 · 7² · 76 / (1.15 · 0.866) · 3.837 · 0.974` printed the result 4.28 kN·m; with 7²
		/// that expression is 4961 and with the real 6.5² it is 4278. 80 substitutions in the
		/// reviewed report carried the rounded value and none carried 6.5, so evaluating any printed
		/// moment line overstated it by 16 %. `d` in the same expression was unrounded, which is
		/// what made the line look trustworthy.
		/// </summary>
		[Test]
		public void TheSubstitutedChordThicknessIsNotRounded()
		{
			string html = Page();
			double T_mm = MultiModeRow().Inputs!.T * 1000.0;   // 6.5

			Assert.That(T_mm, Is.EqualTo(6.5).Within(0.001), "the fixture's chord is 6.5 mm thick");
			Assert.That(html, Does.Not.Match(@"355\s*\\cdot\s*7\^?\{?2"),
				"the chord thickness must not be substituted as 7 when the result uses 6.5");
		}

		/// <summary>
		/// Pull the eq (6.57) substitution and result out of the page.
		///
		/// Scoped to the interaction step rather than searched document-wide: percentages occur
		/// throughout a derivation, and a loose regex would happily match a chord-stress line and
		/// then compare unrelated numbers. Returns the terms in printed order and the total.
		/// </summary>
		private static (double[] Terms, double Total) InteractionLine(string html)
		{
			// To the end of the STEP, not to the next </div>: the label closes its own div
			// immediately, so a `(?=</div>)` lookahead captured the heading and nothing else and
			// every percentage search came back empty. Measured against the real markup rather
			// than assumed — the step is label, symbolic math, substitution, result.
			var block = Regex.Match(html,
				@"Sum of the three interaction terms.*?deriv-step-res.*?</div>",
				RegexOptions.Singleline);
			Assert.That(block.Success, Is.True,
				"the eq (6.57) step must be on the page — without it this test measures nothing");

			var pcts = Regex.Matches(block.Value, @"(-?\d+(?:\.\d+)?)\\?%")
				.Select(m => double.Parse(m.Groups[1].Value, Inv))
				.ToArray();

			Assert.That(pcts, Has.Length.GreaterThanOrEqualTo(4),
				"three terms and a total; found " + pcts.Length);

			return (pcts.Take(3).ToArray(), pcts[3]);
		}
	}
}
