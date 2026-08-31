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
			NorsokHtmlReportGenerator.GenerateDerivationPage(CheckedRow(), brace: "M1",
				connection: "CON1", state: "LE1", utilisation: "88.8 %", verdict: "PASS");

		/// <summary>
		/// Every block the python sheet has, in the page. Named individually so a failure says WHICH
		/// one went missing rather than "the derivation changed".
		///
		/// The validity ranges are checked separately below: their heading only appears when a
		/// condition FAILS, and this fixture's geometry is within range.
		/// </summary>
		[TestCase("Geometry &amp; material")]
		[TestCase("Applied forces")]
		[TestCase("Chord utilisation A&sup2;")]
		[TestCase("Utilisation &mdash; eq (6.57)")]
		public void TheDerivationContainsItsBlocks(string heading)
		{
			Assert.That(Page(), Does.Contain(heading));
		}

		/// <summary>
		/// The page names the connection, the state and the brace.
		///
		/// Several derivation windows are meant to be open side by side — that is why it is a window
		/// and not a modal — and a page headed only "M4" cannot be told from another joint's M4. All
		/// three have to be there; the brace alone was what it used to carry.
		/// </summary>
		[Test]
		public void ThePageNamesTheConnectionTheStateAndTheBrace()
		{
			string html = Page();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("CON1"), "the connection");
				Assert.That(html, Does.Contain("LE1"), "the load effect");
				Assert.That(html, Does.Contain("M1"), "the brace");
				Assert.That(html, Does.Contain("88.8 %"), "the utilisation");
				Assert.That(html, Does.Contain("PASS"), "the verdict");
			});
		}

		/// <summary>
		/// The WINDOW's own title carries all three names too.
		///
		/// Separate from the page test above, and not redundant with it: that one calls the generator
		/// directly, so it says nothing about whether the window passes the connection and the state
		/// on. Proven by reverting exactly that — the window reduced to the brace name alone — which
		/// left the page test green. The title is what distinguishes these windows on the taskbar,
		/// which is the whole reason they carry names.
		///
		/// STA: constructs a WPF Window.
		/// </summary>
		[Test, Apartment(System.Threading.ApartmentState.STA)]
		public void TheWindowTitleNamesTheConnectionTheStateAndTheBrace()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}

			var row = new NorsokChecker.Models.Joint64RowView
			{
				Brace = "M4", Util = "88.8 %", Verdict = "PASS", Detail = CheckedRow(),
			};
			var win = new NorsokChecker.Controls.Joint64DerivationWindow(
				row, new System.Windows.Window(), "CON7", "LE12");

			Assert.Multiple(() =>
			{
				Assert.That(win.Title, Does.Contain("CON7"), "the connection");
				Assert.That(win.Title, Does.Contain("LE12"), "the state");
				Assert.That(win.Title, Does.Contain("M4"), "the brace");
			});

			win.Close();
		}

		/// <summary>
		/// In envelope mode the title names the GOVERNING state, not the word "envelope".
		///
		/// An envelope is not a state. The numbers in the window belong to the one load effect that
		/// governs this brace, and in an envelope two braces can be governed by different ones — so a
		/// title of "envelope" would leave several windows identically titled while showing different
		/// states, which defeats the reason they are titled at all.
		///
		/// The bug this pins was an ordering one: the governing state was resolved AFTER the title was
		/// built, so the page had it and the taskbar did not.
		/// </summary>
		[Test, Apartment(System.Threading.ApartmentState.STA)]
		public void InEnvelopeModeTheTitleNamesTheGoverningState()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}

			var row = new NorsokChecker.Models.Joint64RowView
			{
				Brace = "M4", Util = "88.8 %", Verdict = "PASS", Detail = CheckedRow(),
				GoverningLe = "LE7",          // what the envelope resolved to for THIS brace
			};
			// the tab passes the MODE; the row's governing state must win over it
			var win = new NorsokChecker.Controls.Joint64DerivationWindow(
				row, new System.Windows.Window(), "CON1", "envelope");

			Assert.Multiple(() =>
			{
				Assert.That(win.Title, Does.Contain("LE7"), "the governing state belongs in the title");
				Assert.That(win.Title, Does.Not.Contain("envelope"),
					"the mode is not a state — two windows would be titled alike");
				Assert.That(win.Title, Does.Contain("CON1").And.Contain("M4"));
			});

			win.Close();
		}

		/// <summary>
		/// No section heading appears twice.
		///
		/// A real defect, shipped and spotted by eye rather than by any test: moving the validity block
		/// left "Geometry &amp; material" printed twice in a row. Every position test passed — they use
		/// IndexOf, which happily finds the first of two — and so did every content test, because the
		/// content was all there. Counting the headings is what catches a block emitted twice, whether
		/// by a duplicated line or by a loop that runs once too often.
		/// </summary>
		[Test]
		public void NoSectionHeadingIsEmittedTwice()
		{
			string html = Page();
			var headings = System.Text.RegularExpressions.Regex
				.Matches(html, @"<p class='deriv-h'>(.*?)</p>")
				.Select(m => m.Groups[1].Value)
				.ToList();

			var duplicated = headings.GroupBy(h => h)
				.Where(g => g.Count() > 1)
				.Select(g => $"'{g.Key}' x{g.Count()}")
				.ToList();

			Assert.Multiple(() =>
			{
				Assert.That(headings, Is.Not.Empty, "the sheet has section headings at all");
				Assert.That(duplicated, Is.Empty,
					"repeated heading(s): " + string.Join(", ", duplicated));
			});
		}

		/// <summary>
		/// The four phases of a hand calculation, in order: inputs, basic assumptions, the check, the
		/// verdict on capacity.
		///
		/// This is the property that makes the sheet checkable, and being checkable is what the window
		/// is for — an engineer who cannot follow the numbers will not trust the tool. Asserted by
		/// POSITION, because any order contains all four blocks and only their sequence distinguishes
		/// a calculation from a claim with an appendix.
		///
		/// Two earlier orders failed here on purpose: eq (6.57) first (a sum whose terms come after
		/// it cannot be verified), and the validity table last (it warns about numbers already read).
		/// </summary>
		[Test]
		public void TheSheetFollowsTheOrderOfAHandCalculation()
		{
			string html = Page();
			// section headings throughout, for the reason noted at `verdict` below
			int inputs = html.IndexOf("<p class='deriv-h'>Geometry &amp; material</p>", StringComparison.Ordinal);
			int forces = html.IndexOf("<p class='deriv-h'>Applied forces", StringComparison.Ordinal);
			int assumptions = html.IndexOf("<p class='deriv-h'>Basic assumptions", StringComparison.Ordinal);
			int check = html.IndexOf("<p class='deriv-h'>Chord stress derivation", StringComparison.Ordinal);
			// the SECTION heading, not the step label inside it: Step() emits the same words, so
			// matching the text alone found the step and the assertion held wherever the section was
			// moved to (caught by the oracle — moving the whole block left this test green)
			int verdict = html.IndexOf("<p class='deriv-h'>Utilisation &mdash; eq (6.57)</p>",
				StringComparison.Ordinal);

			Assert.Multiple(() =>
			{
				Assert.That(inputs, Is.GreaterThan(-1), "1. inputs");
				Assert.That(forces, Is.GreaterThan(inputs), "1. the actions follow the geometry");
				Assert.That(assumptions, Is.GreaterThan(forces),
					"2. the validity ranges come AFTER the dimensions they are computed from");
				Assert.That(check, Is.GreaterThan(assumptions),
					"3. the derivation comes after the assumptions that make it valid");
				Assert.That(verdict, Is.GreaterThan(check),
					"4. eq (6.57) closes the sheet — its terms are derived above it");
			});
		}

		/// <summary>
		/// Every §6.4.3.1 condition is listed with its status even when they all pass.
		///
		/// A summarising "✓ all met" line was tried and removed: whether beta lies inside 0.2..1.0 is
		/// exactly what an engineer opens this sheet to verify, and a single tick asks them to take the
		/// app's word for it. Half-transparency earns no more trust than none.
		/// </summary>
		[Test]
		public void EveryValidityConditionIsListedEvenWhenAllPass()
		{
			var row = CheckedRow();
			int conditions = row.Engine!.Validity.Count;

			// the premise: this fixture is within range, so this is the all-pass case
			Assume.That(row.Engine.Validity.All(v => v.Value),
				"this fixture must be inside the validity ranges");

			string html = Page();
			int rows = System.Text.RegularExpressions.Regex.Matches(html, "&#10003; within").Count;

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("<th>condition</th>"),
					"the table is rendered, not summarised away");
				Assert.That(rows, Is.EqualTo(conditions),
					$"all {conditions} conditions must be listed, one row each");
			});
		}

		/// <summary>
		/// The other branch, and the control for the test above: a geometry OUTSIDE §6.4.3.1 marks the
		/// failing conditions and warns that the resistance is extrapolated.
		///
		/// The rows stay in the NORM's order, not sorted by status — the sheet is read against the
		/// standard, so §6.4.3.1's own sequence is the one an engineer is checking off.
		///
		/// beta = 20/141 = 0.14 is below the 0.2 floor, so at least one condition fails.
		/// </summary>
		[Test]
		public void AFailingConditionIsMarkedCountedAndItsConsequenceStated()
		{
			var inp = Joint64Input.FromSI(
				D: 0.141, T: 0.003, fyChord: 355e6,
				d: 0.020, t: 0.003, fyBrace: 355e6,
				thetaDeg: 60.0, g: 0.047,
				frK: 0.0, frY: 0.0, frX: 1.0,
				nSd: -10e3, mipSd: -1e3, mopSd: 0.0,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0.0,
				gammaM: 1.15);
			var result = Norsok64Engine.CheckJoint(inp);

			// the premise of the test — if the engine ever calls this geometry valid, the test below
			// would be measuring nothing
			Assume.That(result.Validity.Any(v => !v.Value),
				"this fixture must have at least one failing validity condition");

			var row = new JointCheckRow
			{
				Name = "M9", Skipped = false, Engine = result, Inputs = inp,
				Classification = new KyxClass { Name = "M9", FrK = 0, FrY = 0, FrX = 1, NSd = -10e3 },
				DomClass = "X",
				Util = result.UtilWeighted, Passed = result.Passed,
				NRdWeighted = result.NRdWeighted, MRdIp = result.MRdIp, MRdOp = result.MRdOp,
				WithinRange = result.WithinRange, ChordOverstressed = result.ChordOverstressed,
			};
			string html = NorsokHtmlReportGenerator.GenerateDerivationPage(row, brace: "M9");

			int outside = result.Validity.Count(v => !v.Value);
			int marked = System.Text.RegularExpressions.Regex.Matches(html, "&#10007; outside").Count;

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("Basic assumptions"), "the heading");
				Assert.That(html, Does.Contain("<th>condition</th>"), "the table is rendered");
				Assert.That(marked, Is.EqualTo(outside),
					"every failing condition is marked as outside");
				Assert.That(html, Does.Contain($"{outside} of {result.Validity.Count} OUTSIDE"),
					"the heading counts them, so the reader knows before scanning the table");
				Assert.That(html, Does.Contain("the resistance below is extrapolated"),
					"and says what that means for the numbers that follow");
			});
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
