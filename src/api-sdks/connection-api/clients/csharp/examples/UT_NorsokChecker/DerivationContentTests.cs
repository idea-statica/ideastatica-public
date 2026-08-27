using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The §6.4 derivation must show HOW each number was reached, not just the number.
	///
	/// It used to be six tables of results with no formulas at all, against the python reference's
	/// nine blocks of label / formula / substituted numbers / result. A result on its own cannot be
	/// checked by hand, which is the only reason a derivation view exists.
	///
	/// The engine input is built here rather than fetched, so the test says what a derivation must
	/// contain without needing a service. CHS 141.0/6.5 chord and a 76.0/3.5 brace at 60 deg —
	/// CON1's M1, so the numbers are the ones measured against the python app earlier.
	/// </summary>
	[TestFixture]
	public class DerivationContentTests
	{
		/// <summary>A checked brace, run through the real engine so the values are real.</summary>
		private static JointCheckRow CheckedRow()
		{
			var inp = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 60.0, g: 0.047,
				frK: 0.0, frY: 0.0, frX: 1.0,
				nSd: -10e3, mipSd: -1e3, mopSd: 0.0,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0.0,
				gammaM: 1.15);

			var result = Norsok64Engine.CheckJoint(inp);
			return new JointCheckRow
			{
				Name = "M1",
				Skipped = false,
				Engine = result,
				Inputs = inp,
				Classification = new KyxClass
				{
					Name = "M1", FrK = 0.0, FrY = 0.0, FrX = 1.0,
					NSd = -10e3, MipSd = -1e3, MopSd = 0.0,
				},
				DomClass = "X",
				// the chord stress block is drawn only when this is present with a real section —
				// without it the sigma steps are absent, which is correct but makes the fixture
				// test less than it claims to
				ChordStress = new ChordStressRow
				{
					Name = "M1",
					A = 2.747e-3, I = 6.2252e-6, R = 0.0705, Side = 1,
					NChord = 25.5e3, MipChord = 2.25e3, MopChord = 0.0,
					SigmaA = 9.27e6, SigmaMy = -25.48e6, SigmaMz = 0.0,
				},
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
			NorsokHtmlReportGenerator.GenerateDerivationPage(CheckedRow(), "M1 — NORSOK 6.4 check");

		/// <summary>
		/// Every block the python sheet has, in the page. Named individually so a failure says WHICH
		/// one went missing rather than "the derivation changed".
		/// </summary>
		[TestCase("Geometry &amp; material")]
		[TestCase("Applied forces")]
		[TestCase("Chord utilisation A&sup2;")]
		[TestCase("Utilisation &mdash; eq (6.57)")]
		[TestCase("Validity ranges")]
		public void TheDerivationContainsItsBlocks(string heading)
		{
			Assert.That(Page(), Does.Contain(heading));
		}

		/// <summary>
		/// The formulas are actually there, as LaTeX for KaTeX to typeset. This is the whole
		/// difference from the six-tables-of-numbers version.
		/// </summary>
		[TestCase(@"A^2 = \left(\dfrac{\sigma_{a,Sd}}{f_y}\right)^2", "eq (6.55), chord utilisation")]
		[TestCase(@"Q_f = 1 + C_1\dfrac{\sigma_{a,Sd}}{f_y}", "eq (6.54), the chord action factor")]
		[TestCase(@"M_{Rd,ip} = \dfrac{f_y\,T^2\,d}{\gamma_M \sin\theta}", "eq (6.53), bending resistance")]
		[TestCase(@"N_{Rd} = \dfrac{f_y\,T^2}{\gamma_M \sin\theta}", "eq (6.52), axial resistance")]
		[TestCase(@"u = \dfrac{N_{Sd}}{N_{Rd}}", "eq (6.57), the interaction")]
		public void TheFormulasAreTypeset(string latex, string what)
		{
			Assert.That(Page(), Does.Contain(latex), what);
		}

		/// <summary>
		/// THE property that makes a derivation checkable: the numbers put INTO each formula are
		/// shown, not only the answer. A page carrying the symbolic forms and the results but no
		/// substitution would pass every test above and still be unverifiable by hand.
		/// </summary>
		[Test]
		public void TheNumbersPutIntoTheFormulasAreShown()
		{
			string html = Page();

			Assert.Multiple(() =>
			{
				// each step emits "$$=\;<substituted>$$" before its result line
				int substitutions = System.Text.RegularExpressions.Regex.Matches(
					html, @"\$\$=\\;").Count;
				Assert.That(substitutions, Is.GreaterThanOrEqualTo(8),
					$"only {substitutions} substituted lines — the steps are showing results without "
					+ "the numbers that produced them");
				// and the chord's own stresses have to appear as numbers, since they are what the
				// reader cannot re-derive from the table above
				Assert.That(html, Does.Contain("355"), "f_y in the substitution");
				Assert.That(html, Does.Contain(@"\,MPa"), "the stresses are given in MPa");
			});
		}

		/// <summary>
		/// Only the ACTIVE class gets a block. This brace is pure X, so a "Mode K" or "Mode Y"
		/// heading would describe a mode playing no part in its check — the engine computes all
		/// three regardless.
		/// </summary>
		[Test]
		public void OnlyTheActiveModeIsShown()
		{
			string html = Page();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("Mode X"), "X carries 100 % of this brace's axial force");
				Assert.That(html, Does.Not.Contain("Mode K"), "K plays no part here");
				Assert.That(html, Does.Not.Contain("Mode Y"), "nor does Y");
			});
		}

		/// <summary>
		/// A skipped brace produces no derivation rather than a page of dashes — there is nothing to
		/// derive, and empty formulas would suggest otherwise.
		/// </summary>
		[Test]
		public void ASkippedBraceHasNoDerivation()
		{
			var page = NorsokHtmlReportGenerator.GenerateDerivationPage(
				new JointCheckRow { Name = "M6", Skipped = true, Reason = "no transverse force" }, "M6");

			// Match the DIV, not the class name: ".deriv-step" also appears in the stylesheet every
			// page carries, so a bare Contains("deriv-step") can never be false. The first version
			// of this assertion failed for that reason, not because the code was wrong.
			Assert.That(page, Does.Not.Contain("<div class='deriv-step'>"),
				"nothing was computed, so there are no steps to show");
		}
	}
}
