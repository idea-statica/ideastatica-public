using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Which check cards print a "Where:" table.
	///
	/// A §6.4 card used to carry both a 31-row variable table AND the derivation below it, and the
	/// two said the same thing — the derivation states every one of those quantities WITH its formula
	/// and its substitution, which is strictly more than a value in a table. It was the taller half of
	/// the card and pure repetition. Audited row by row before removing it: geometry, the actions,
	/// the three chord stresses, Q_u,ipb/Q_u,opb (substituted inside the M_Rd steps that use them),
	/// Q_g/Q_u,axial/Q_f/N_Rd per active mode, and the three interaction terms (substituted into
	/// eq 6.57 itself) are all in the derivation. Nothing was left over.
	///
	/// The rule is "no table where there is a derivation", NOT "no table for §6.4" — and that
	/// distinction is what these two tests hold apart. §6.3 (Services/Formulas63_Mothballed) fills
	/// Variables and has no derivation renderer, so its cards must keep the table; it is mothballed
	/// but slated to return, and losing its only variable listing would be silent.
	/// </summary>
	[TestFixture]
	public class WhereBlockTests
	{
		/// <summary>
		/// A §6.4 row: variables AND a derivation, which is the case that loses the table.
		///
		/// Run through the REAL engine. RenderJointDerivation returns early unless Engine, Inputs and
		/// Classification are all present, so a row with a hand-built JointDetail renders no
		/// derivation at all — and then "no Where: table" would pass on a card that lost both, which
		/// is worse than the defect. (Caught by the second assertion on the first run of this test.)
		/// </summary>
		private static NorsokFormulaResult WithDerivation()
		{
			var inputs = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 60.0, g: 0.047,
				frK: 1.0, frY: 0.0, frX: 0.0,
				nSd: -10e3, mipSd: -1e3, mopSd: 0,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0,
				gammaM: 1.15);

			var engine = Norsok64Engine.CheckJoint(inputs);

			return new NorsokFormulaResult
			{
				Section = "6.4.3.6", Equation = "6.57",
				Title = "Tubular Joint — M1",
				Utilization = 0.476, Passed = true,
				// The same variables the §6.4 adapter emits — the point is that they are PRESENT and
				// still not rendered, so a test that omitted them would pass for the wrong reason.
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "D", Description = "chord outside diameter", Value = 141.0, Unit = "mm" },
					new() { Symbol = "θ", Description = "brace-to-chord angle", Value = 60.0, Unit = "°" },
					new() { Symbol = "Qu_axial", Description = "strength factor — axial", Value = 16.42, Unit = "-" },
				},
				JointDetail = new JointCheckRow
				{
					Name = "M1", Skipped = false, Util = 0.476, Passed = true,
					Engine = engine, Inputs = inputs, DomClass = "K",
					Classification = new KyxClass
					{
						Name = "M1", FrK = 1.0, FrY = 0, FrX = 0,
						NSd = -10e3, MipSd = -1e3, MopSd = 0,
					},
				},
			};
		}

		/// <summary>
		/// A check with variables and NO derivation — the shape of every §6.3 check, and the reason
		/// the renderer cannot simply drop the block.
		/// </summary>
		private static NorsokFormulaResult WithoutDerivation() => new()
		{
			Section = "6.3.8.2", Equation = "6.28",
			Title = "Compression & bending — cross section",
			Utilization = 0.62, Passed = true,
			Variables = new List<FormulaVariable>
			{
				new() { Symbol = "N_Sd", Description = "design axial force", Value = 420.0, Unit = "kN" },
				new() { Symbol = "N_cl,Rd", Description = "local buckling resistance", Value = 980.0, Unit = "kN" },
			},
			// JointDetail deliberately null: §6.3 has no derivation renderer.
		};

		private static string Report(params NorsokFormulaResult[] rows) =>
			NorsokHtmlReportGenerator.GenerateReport(
				"test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)> { ("CON1", rows.ToList()) },
				expandAll: false);

		/// <summary>
		/// The §6.4 card has no "Where:" table, and its derivation is what replaced it.
		///
		/// Both halves are asserted together on purpose: a card that lost the table AND the
		/// derivation would satisfy the first assertion while being strictly worse than before.
		/// </summary>
		[Test]
		public void ACheckWithADerivationHasNoWhereTable()
		{
			string html = Report(WithDerivation());

			Assert.Multiple(() =>
			{
				// The emitted markup, not the class name: the stylesheet every report carries mentions
				// .where-header, so matching the bare string could never be false.
				Assert.That(html, Does.Not.Contain("<p class='where-header'>"),
					"a check whose derivation states the same quantities must not also table them");
				Assert.That(html, Does.Contain("<div class='deriv-block'>"),
					"and the derivation is what carries them instead");
			});
		}

		/// <summary>
		/// A check with no derivation KEEPS its table. This is the §6.3 case, and the one that would
		/// break quietly: those checks have no other place where their variables are listed.
		/// </summary>
		[Test]
		public void ACheckWithoutADerivationKeepsItsWhereTable()
		{
			string html = Report(WithoutDerivation());

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("<p class='where-header'>"),
					"§6.3-shaped checks have no derivation, so the table is their only variable listing");
				Assert.That(html, Does.Contain("local buckling resistance"),
					"and the descriptions are in it");
			});
		}

		/// <summary>
		/// θ says where it came from. It is derived from the member axes rather than typed, and that
		/// was the one description in the removed table carrying information the derivation did not
		/// already state.
		/// </summary>
		[Test]
		public void TheDerivationSaysThetaComesFromTheMemberAxes()
		{
			string html = Report(WithDerivation());

			Assert.That(html, Does.Contain("from the member axes"),
				"θ is computed from the brace and chord directions (JointTopologyBuilder.Theta), "
				+ "not entered — a reader checking the sheet by hand needs to know that");
		}
	}
}
