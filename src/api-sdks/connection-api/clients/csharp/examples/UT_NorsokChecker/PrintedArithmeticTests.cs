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
		/// The K-mode N_Rd substitution prints ALL THREE factors, Q_f,K included.
		///
		/// The formula line promises `N_Rd,i = f_y·T²/(γ_M·sinθ) · Q_u,i · Q_f,K` and the
		/// substitution printed only two of them: `15.1 kN · 16.425 = 241.9 kN`, where the product
		/// of the printed pair is 248.0. The missing factor was Q_f,K = 0.9780, applied by the
		/// engine and printed nowhere — the reviewed report had zero `Q_f, axial — class K` blocks
		/// against 10 for Y and 33 for X, which print theirs correctly.
		///
		/// A reader multiplying what they see gets a resistance 2.5 % too high and no way to find
		/// out why.
		/// </summary>
		[Test]
		public void TheKModeResistanceSubstitutionPrintsItsQf()
		{
			string html = Page();
			var row = MultiModeRow();
			double qfK = row.Engine!.PerClass[Joint64Class.K].QfAxial;

			Assert.That(qfK, Is.Not.EqualTo(1.0).Within(0.001),
				"this fixture's chord is stressed, so Q_f,K differs from 1 and its absence is "
				+ "arithmetically visible — with Q_f = 1 the defect would hide");

			var block = Regex.Match(html, @"Mode K.*?(?=<p class='deriv-h'>|</div>\s*</div>)",
				RegexOptions.Singleline);
			Assert.That(block.Success, Is.True, "the Mode K section must be on the page");

			Assert.Multiple(() =>
			{
				Assert.That(block.Value, Does.Contain("Q<sub>f</sub>, axial &mdash; class K"),
					"class K gets its own Q_f block, as Y and X do");
				Assert.That(block.Value, Does.Contain(N3(qfK)),
					$"and the N_Rd substitution carries the factor {N3(qfK)} it was multiplied by");
			});
		}

		/// <summary>
		/// Q_g prints the inputs its branch needs — φ above all, on the branch that uses it.
		///
		/// It was a label and a value. Note (b) under Table 6-3 has three branches and the middle
		/// one interpolates between the other two, which needs φ = (t·f_y,brace)/(T·f_y,chord); φ
		/// appeared nowhere on the page, so two braces in one joint printed `g = 2 mm,
		/// g/D = 0.011` and then 1.188 and 1.810 — a 52 % spread from what a reader sees as
		/// identical input, feeding Q_u directly.
		///
		/// SEPARATE FIXTURE, and that is the point: the multi-mode row's gap is 47 mm, i.e.
		/// g/D = 0.333, which is the plain gap branch where φ plays no part. Asserting φ there
		/// would demand a quantity the branch does not use — my first version of this test did
		/// exactly that and failed on correct output. A 2 mm gap at D = 141 gives g/D = 0.014 and
		/// lands in the interpolation band, which is the case the defect was found on.
		/// </summary>
		[Test]
		public void TheInterpolatedQgBlockPrintsPhiAndItsSubstitution()
		{
			var inp = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.102, t: 0.0065, fyBrace: 355e6,
				thetaDeg: 45.0, g: 0.002,                    // g/D = 0.014 -> interpolated
				frK: 1.0, frY: 0.0, frX: 0.0,
				nSd: -88.8e3, mipSd: -1.2e3, mopSd: 2.4e3,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0.0,
				gammaM: 1.15);
			var result = Norsok64Engine.CheckJoint(inp);

			Assert.That(inp.G / inp.D, Is.InRange(-0.05, 0.05),
				"the fixture must be INSIDE the interpolation band, or φ is not what decides it");

			string html = NorsokHtmlReportGenerator.GenerateDerivationPage(
				new JointCheckRow
				{
					Name = "M1", Skipped = false, Engine = result, Inputs = inp,
					Classification = new KyxClass
					{
						Name = "M1", FrK = 1.0, FrY = 0.0, FrX = 0.0,
						NSd = -88.8e3, MipSd = -1.2e3, MopSd = 2.4e3,
					},
					DomClass = "K", Util = result.UtilWeighted, Passed = result.Passed,
					NRdWeighted = result.NRdWeighted, MRdIp = result.MRdIp, MRdOp = result.MRdOp,
					WithinRange = result.WithinRange, ChordOverstressed = result.ChordOverstressed,
				},
				brace: "M1", connection: "CON1", state: "LE1", utilisation: "—", verdict: "PASS");

			var block = Regex.Match(html,
				@"Q<sub>g</sub>.*?deriv-step-res.*?</div>", RegexOptions.Singleline);
			Assert.That(block.Success, Is.True, "the Q_g step must be on the page");

			Assert.Multiple(() =>
			{
				Assert.That(block.Value, Does.Match(@"\\varphi|\\phi|&phi;"),
					"φ is what the interpolation branch turns on, so it must be printed");
				Assert.That(block.Value, Does.Contain("interpolated"),
					"and the block says which of the three branches was taken");
				Assert.That(block.Value, Does.Contain("deriv-step-math'>$$=\\;"),
					"and shows a substitution, not just a result");
			});
		}

		/// <summary>
		/// The plain gap branch names itself and substitutes its own inputs too.
		///
		/// The multi-mode fixture's 47 mm gap takes this branch, so it is the one that would go
		/// unguarded if only the interpolated case were tested. A reader cannot tell which of the
		/// three branches produced a value unless the block says so — and two braces printing
		/// different Q_g from the same-looking heading is precisely how the defect surfaced.
		/// </summary>
		[Test]
		public void TheGapBranchQgNamesItselfAndSubstitutes()
		{
			var row = MultiModeRow();
			double gd = row.Inputs!.G / row.Inputs!.D;

			Assert.That(gd, Is.GreaterThanOrEqualTo(0.05),
				"this fixture's gap is outside the band, so it exercises the gap branch");

			var block = Regex.Match(Page(),
				@"Q<sub>g</sub>.*?deriv-step-res.*?</div>", RegexOptions.Singleline);

			Assert.Multiple(() =>
			{
				Assert.That(block.Value, Does.Contain("gap branch"), "the branch is named");
				Assert.That(block.Value, Does.Not.Contain("interpolated"),
					"and not described as interpolated, which it is not");
				Assert.That(block.Value, Does.Contain("deriv-step-math'>$$=\\;"),
					"a substitution is shown");
			});
		}

		/// <summary>
		/// An OUT-OF-RANGE brace's derivation does not mix the two §6.4.3.1 passes.
		///
		/// The clause is handled correctly — both passes run, the lesser governs, the table above
		/// the derivation shows both and marks the winner. The DERIVATION is what went wrong: the
		/// engine keeps the lesser pass's resistances but overwrites β, γ, θ and sinθ with the
		/// brace's ACTUAL geometry (Norsok64Engine.cs:268-271, deliberately — the validity statement
		/// must describe the real brace). So the substitution printed the actual sinθ against the
		/// imposed pass's Q_u: on a 20° brace `38.1 kN · 9.697 · 1.000 = 252.8 kN`, where the
		/// printed factors give 369.5. The prefactor is pass (a)'s, the result is pass (b)'s.
		///
		/// A reader cannot reconcile that, and the page gives them no clue which θ each factor
		/// belongs to. The fix is to say so in the substitution, not to change either number.
		/// </summary>
		[Test]
		public void TheOutOfRangeDerivationSubstitutionIsSelfConsistent()
		{
			// θ = 20° is below the §6.4.3.1 lower limit of 30°, so the limiting pass clamps it.
			var inp = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 20.0, g: 0.047,
				frK: 0.0, frY: 0.0, frX: 1.0,
				nSd: -38.1e3, mipSd: -1.2e3, mopSd: 0.0,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0.0,
				gammaM: 1.15);
			var result = Norsok64Engine.CheckJoint(inp);

			Assert.Multiple(() =>
			{
				Assert.That(result.WithinRange, Is.False, "θ = 20° is outside 30–90°");
				Assert.That(result.LimitingPassApplied, Is.True, "so the second pass ran");
				Assert.That(result.ThetaLimitingDeg, Is.EqualTo(30.0).Within(0.01),
					"clamped to the lower limit");
			});

			string html = NorsokHtmlReportGenerator.GenerateDerivationPage(
				new JointCheckRow
				{
					Name = "M1", Skipped = false, Engine = result, Inputs = inp,
					Classification = new KyxClass
					{
						Name = "M1", FrK = 0.0, FrY = 0.0, FrX = 1.0,
						NSd = -38.1e3, MipSd = -1.2e3, MopSd = 0.0,
					},
					DomClass = "X", Util = result.UtilWeighted, Passed = result.Passed,
					NRdWeighted = result.NRdWeighted, MRdIp = result.MRdIp, MRdOp = result.MRdOp,
					WithinRange = result.WithinRange, ChordOverstressed = result.ChordOverstressed,
				},
				brace: "M1", connection: "CON11", state: "LE1", utilisation: "—", verdict: "PASS");

			// The N_Rd step, and whether its printed factors give its printed result.
			// Through to the RESULT div, not to the first </div> — the label closes its own, which
			// is how an earlier version of this regex captured a heading and nothing else.
			var step = Regex.Match(html,
				@"N<sub>Rd</sub>[^<]*&mdash;[^<]*eq \(6\.52\).*?deriv-step-res'>\$\$=[^$]*\$\$",
				RegexOptions.Singleline);
			Assert.That(step.Success, Is.True, "the N_Rd step must be on the page");

			// The arithmetic, not the wording: multiply the printed factors and compare with the
			// printed result. `38.1 kN · 9.697 · 1.000` printed `252.8 kN`, and 38.1 · 9.697 is
			// 369.5 — the prefactor was built from the actual sinθ (20°, giving 38.1) while Q_u and
			// the result came from the clamped pass (30°, whose prefactor is 26.1).
			//
			// Read the SUBSTITUTION div only. A first version scanned the whole step and swallowed
			// the digits of the clause reference in the label ("6.4.3.1" became 6.4 · 3.1 · 2.0),
			// which is a reminder that a number in prose is not a factor.
			var subst = Regex.Match(step.Value,
				@"deriv-step-math'>\$\$=\\;(?<s>[^$]*)\$\$.*?deriv-step-res'>\$\$=\\;(?<r>[\d.]+)",
				RegexOptions.Singleline);
			Assert.That(subst.Success, Is.True,
				"the step must carry a substitution and a result; got:\n" + step.Value);

			var vals = Regex.Matches(subst.Groups["s"].Value, @"(\d+(?:\.\d+)?)")
				.Select(m => double.Parse(m.Groups[1].Value, Inv)).ToArray();
			Assert.That(vals, Has.Length.GreaterThanOrEqualTo(2),
				"expected at least a prefactor and Q_u; found " + string.Join(", ", vals));

			double product = vals.Aggregate(1.0, (a, b) => a * b);
			double printed = double.Parse(subst.Groups["r"].Value, Inv);

			Assert.That(product, Is.EqualTo(printed).Within(Math.Max(0.15, printed * 0.005)),
				$"the printed factors {string.Join(" · ", vals.Select(v => v.ToString("F3", Inv)))}"
				+ $" = {product.ToString("F1", Inv)} must give the printed result "
				+ $"{printed.ToString("F1", Inv)} — on an out-of-range brace the prefactor came from "
				+ "the actual θ and the result from the clamped pass");
		}

		/// <summary>
		/// The moment-resistance substitution evaluates to its own printed result.
		///
		/// This is the arithmetic form of the rounded-thickness finding, and it is stronger than
		/// asserting the absence of `7`: it evaluates `f_y·T²·d / (γ_M·sinθ) · Q_u · Q_f` exactly as
		/// printed and compares. The reviewed report printed `355 · 7² · 76 / (1.15 · 0.866) ·
		/// 3.837 · 0.974` against a result of 4.28 kN·m; the expression gives 4961 Nmm·10³ and the
		/// real 6.5² gives 4278. Whatever precision policy the generator adopts, this test says the
		/// policy has to keep the line self-consistent.
		/// </summary>
		[TestCase("In-plane bending resistance")]
		[TestCase("Out-of-plane bending resistance")]
		public void TheMomentResistanceSubstitutionEvaluatesToItsResult(string heading)
		{
			var step = Regex.Match(Page(),
				Regex.Escape(heading) + @".*?deriv-step-res'>\$\$=[^$]*\$\$",
				RegexOptions.Singleline);
			Assert.That(step.Success, Is.True, $"the {heading} step must be on the page");

			// f_y · T² · d / (γ_M · sinθ) · Q_u · Q_f  =  result [kN·m]
			var m = Regex.Match(step.Value,
				@"dfrac\{(?<fy>[\d.]+)\\cdot (?<T>[\d.]+)\^2\\cdot (?<d>[\d.]+)\}"
				+ @"\{(?<gm>[\d.]+)\\cdot (?<sin>[\d.]+)\}\\cdot (?<qu>[\d.]+)\\cdot (?<qf>[\d.]+)");
			Assert.That(m.Success, Is.True,
				"the substitution must be readable as f_y·T²·d/(γ_M·sinθ)·Q_u·Q_f — got:\n"
				+ step.Value);

			double G(string k) => double.Parse(m.Groups[k].Value, Inv);
			// MPa · mm² · mm = N·mm; /1e6 -> kN·m
			double computed = G("fy") * G("T") * G("T") * G("d") / (G("gm") * G("sin"))
				* G("qu") * G("qf") / 1e6;

			double printed = double.Parse(
				Regex.Match(step.Value, @"deriv-step-res'>\$\$=\\;([\d.]+)").Groups[1].Value, Inv);

			Assert.That(computed, Is.EqualTo(printed).Within(Math.Max(0.02, printed * 0.01)),
				$"the printed expression evaluates to {computed.ToString("F2", Inv)} kN·m but the "
				+ $"printed result is {printed.ToString("F2", Inv)} — a reader checking this line by "
				+ "hand gets a different number from the one the check used");
		}

		/// <summary>
		/// §6.4.1's gap provision is stated, with the actual gap, and marked informative.
		///
		/// "The gap for simple K-joints should be larger than 50 mm and less than D" (N-004 Rev. 3
		/// §6.4.1). The string "50 mm" occurred nowhere in a 227-page report whose K gaps were 2, 8,
		/// 9 and 47 mm — all below it — while the same report rejected joints for gap rules at the
		/// negative end. The clause says "should", so it must NOT read as a verdict.
		/// </summary>
		[Test]
		public void TheGapProvisionIsStatedAsInformative()
		{
			string html = Page();
			var row = MultiModeRow();

			Assert.That(row.Classification!.FrK, Is.GreaterThan(0.0),
				"the provision is about K joints, so the fixture must have a K fraction");

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("50 mm"), "the provision's threshold is printed");
				Assert.That(html, Does.Contain("&sect;6.4.1"), "attributed to its own clause");
				Assert.That(html, Does.Match(@"informative|should"),
					"and marked as a 'should', not as a check");
				Assert.That(html, Does.Contain(N(row.Inputs!.G * 1e3, 1)),
					"beside the joint's actual gap, so the reader can compare");
			});
		}

		// Shear and torsion in the per-brace force table are covered in JointPlaneSectionTests,
		// which owns that section and already has a JointTopology fixture — the table is emitted by
		// RenderJointPlane and needs one. My first attempt built a report with no topology, where
		// the table is not rendered at all, so the test failed on an absence it had created itself.

		/// <summary>
		/// A §6.4 check card announces the CRITERION and not the formulas.
		///
		/// Each card used to open with `Check condition:` + the interaction inequality, the M_y/M_z
		/// legend, and `Design resistance:` + three stacked fractions — 40 times each in the
		/// reviewed report. Two of those three are duplicates the reader already has:
		///
		///   * the resistance formulas reappear a few lines below WITH NUMBERS IN THEM, so the
		///     symbolic announcement adds nothing that is not on the same page;
		///   * the legend is chapter 3's sign-conventions sentence, word for word.
		///
		/// The inequality STAYS. It is the only place on the card that says what the result is
		/// compared against: the footer prints `= 73.73 % PASS` and the eq (6.57) step shows three
		/// terms and their sum, but `≤ 1.0` appears nowhere else.
		///
		/// The distinction matters more than the page count: removing the symbolic line that stands
		/// directly over its own substitution is what would force a cross-reference in a 150-page
		/// document, and that is deliberately NOT done here.
		/// </summary>
		[Test]
		public void TheCheckCardKeepsItsCriterionAndDropsTheDuplicatedFormulas()
		{
			string html = QualifiedReport();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("Check condition:"),
					"the criterion is announced");
				Assert.That(html, Does.Contain(@"\leq 1.0"),
					"and it is the inequality, which is stated nowhere else on the card");

				Assert.That(html, Does.Not.Contain("Design resistance:"),
					"the symbolic resistances are not announced — they are printed below with "
					+ "their numbers, on the same page");
				Assert.That(html, Does.Not.Contain("M<sub>y</sub> = in-plane"),
					"and the legend is chapter 3's sentence, not the card's");
			});
		}

		/// <summary>
		/// Chapter 3 shows the equations it talks about.
		///
		/// It said the resistance is recomputed for every load effect and contained no equation at
		/// all — measured, zero `$$` blocks — so a reader met eq (6.52) for the first time on page
		/// 12, inside a check. The chapter that exists to stand on its own could not.
		/// </summary>
		[Test]
		public void ChapterThreeShowsTheEquations()
		{
			string html = QualifiedReport();

			int at = html.IndexOf("How the checks are made", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "chapter 3 is rendered");
			// To the first connection chapter, so this cannot pass on a check card's own maths.
			int end = html.IndexOf("class='connection-header", at, StringComparison.Ordinal);
			string chapter = end > at ? html[at..end] : html[at..];

			Assert.Multiple(() =>
			{
				Assert.That(chapter, Does.Contain("N_{Rd}"), "eq (6.52) is shown");
				Assert.That(chapter, Does.Contain("M_{y,Rd}"), "eq (6.53) is shown");
				Assert.That(chapter, Does.Contain(@"\leq 1.0"), "and the interaction criterion");
				Assert.That(chapter, Does.Match(@"Q_u|Q<sub>u</sub>"),
					"with its symbols named, or the equations cannot be read");
			});
		}

		/// <summary>A report with one §6.4.3.6 card, which is what both tests above read.</summary>
		private static string QualifiedReport()
		{
			var row = MultiModeRow();
			return NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokChecker.Models.NorsokFormulaResult>)>
				{
					("CON11", new List<NorsokChecker.Models.NorsokFormulaResult>
					{
						new()
						{
							Section = "6.4.3.6", Equation = "6.57",
							Title = "Tubular Joint — M3",
							Utilization = row.Util, Passed = row.Passed,
							FormulaSubstituted = "28.49% + 0.55% + 23.65%",
						},
					}),
				});
		}

		private static string N(double v, int dp) => v.ToString("F" + dp, Inv);
		private static string N3(double v) => v.ToString("F3", Inv);

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
