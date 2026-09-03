using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The report's per-connection summary table: which connections are a problem.
	///
	/// It answers a question the report could not: the summary card says whether the PROJECT passed,
	/// and on fifteen joints that leaves the reader scrolling every section to find the one that
	/// failed.
	///
	/// A geometry table was added here too and then removed: the per-brace derivation already states
	/// the chord and brace dimensions, θ and the K/Y/X split, and the card titles carry the split as
	/// well — so the table repeated three things and contributed only the gap.
	/// </summary>
	[TestFixture]
	public class ReportTableTests
	{
		private static NorsokFormulaResult Brace(
			string name, double util, bool passed, double dMm, double frK)
		{
			// The gap mirrors what the engine does: G is a single-gap shortcut used only when the
			// brace has K components, so a 0 % K brace carries ZERO. Handing every brace 0.047
			// instead made the gap test unfalsifiable — there was no "0 mm" to find, so it passed
			// against the reverted renderer too (caught by the oracle).
			double gapM = frK > 0 ? 0.047 : 0.0;

			var inputs = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: dMm / 1000.0, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 60.0, g: gapM,
				frK: frK, frY: 0.0, frX: 1.0 - frK,
				nSd: -10e3, mipSd: 0, mopSd: 0,
				sigmaASd: 0, sigmaMySd: 0, sigmaMzSd: 0,
				gammaM: 1.15);

			return new NorsokFormulaResult
			{
				Section = "6.4.3.6", Equation = "6.57",
				Title = $"Tubular Joint — {name}",
				Utilization = util, Passed = passed,
				JointDetail = new JointCheckRow
				{
					Name = name, Util = util, Passed = passed, Inputs = inputs,
					Classification = new KyxClass { Name = name, FrK = frK, FrY = 0, FrX = 1.0 - frK },
				},
			};
		}

		private static NorsokFormulaResult Rejected(string why) => new()
		{
			Section = "6.4", Equation = "6.4.3", Title = "Outside the scope of §6.4",
			CheckExpression = why, NotAssessed = true,
		};

		private static string Report(params (string Con, NorsokFormulaResult[] Rows)[] cons) =>
			NorsokHtmlReportGenerator.GenerateReport(
				"test.ideaCon", cons.Select(c => (c.Con, c.Rows.ToList())).ToList(), expandAll: false);

		/// <summary>
		/// One row per connection, with the verdict from CheckWorkflow — the same function the
		/// connection list and the run use, so the report cannot disagree with the app about what a
		/// connection is. A verdict recomputed here is how the two would drift.
		/// </summary>
		[Test]
		public void TheSummaryTableCarriesEveryConnectionsVerdict()
		{
			string html = Report(
				("CON1", new[] { Brace("M1", 0.735, true, 76, 1.0) }),
				("CON9", new[] { Brace("M1", 1.30, false, 76, 1.0) }),
				("CON5", new[] { Rejected("no brace"), Rejected("θ = 0°") }));

			// Read the table's own rows, not the whole document: PASS and FAIL appear in the CSS and
			// in the check cards too, so matching them anywhere would pass on an empty table.
			int at = html.IndexOf("class='connection-table'", StringComparison.Ordinal);
			string table = html[at..html.IndexOf("</table>", at, StringComparison.Ordinal)];

			var verdicts = System.Text.RegularExpressions.Regex
				.Matches(table, @"class='con-verdict [a-z]+'>([^<]+)<")
				.Select(m => m.Groups[1].Value)
				.ToList();

			Assert.Multiple(() =>
			{
				Assert.That(table, Does.Contain("CON1"), "every connection is in it");
				Assert.That(table, Does.Contain("CON9"));
				Assert.That(table, Does.Contain("CON5"));

				// The three verdicts, in order — and they must DIFFER. A renderer that hardcoded one
				// value, or recomputed the rules instead of calling CheckWorkflow.Roll, would give the
				// same answer three times over three genuinely different connections.
				Assert.That(verdicts, Is.EqualTo(new[] { "PASS", "FAIL", "N/A" }),
					"the verdicts come from CheckWorkflow, which the app itself uses");
			});
		}

		/// <summary>
		/// A connection nothing could be assessed on shows an em dash, not 0.0 %.
		///
		/// The third time this trap has been closed in this app — the check cards and the Results
		/// table both printed a utilisation of zero where no utilisation existed, and zero is the most
		/// favourable number there is.
		/// </summary>
		[Test]
		public void ARejectedConnectionShowsNoUtilisation()
		{
			string html = Report(("CON5", new[] { Rejected("no brace") }));

			int table = html.IndexOf("class='connection-table'", StringComparison.Ordinal);
			string row = html[table..html.IndexOf("</table>", table, StringComparison.Ordinal)];

			Assert.That(row, Does.Not.Contain("0.0%").And.Not.Contain("0,0%"),
				"a joint nobody checked has no utilisation to report");
		}

		/// <summary>
		/// The §6.4.3.1 caveat reaches the OVERVIEW ROW, in words, on a non-green verdict.
		///
		/// The round-2 review's highest-priority finding: the reviewed report printed
		/// "CON11 | PASS | 73.7% | Norsok OK" for a joint whose brace sits at θ = 20°, with the
		/// caveat 64 pages away on the check card. Measured on that file, the word "validity"
		/// occurred 34 times and not once on pages 1–3.
		///
		/// This is the WIRING half — the qualifier can be produced correctly and rolled up correctly
		/// and still never be rendered.
		/// </summary>
		[Test]
		public void TheOverviewRowCarriesTheValidityCaveat()
		{
			var qualified = Brace("M1", 0.737, true, 76, 1.0);
			qualified.RangeQualifier = "M1: θ = 20.0°, outside 30–90°";

			string html = Report(
				("CON1", new[] { Brace("M1", 0.42, true, 76, 1.0) }),
				("CON11", new[] { qualified }));

			int at = html.IndexOf("class='connection-table'", StringComparison.Ordinal);
			string table = html[at..html.IndexOf("</table>", at, StringComparison.Ordinal)];

			var verdicts = System.Text.RegularExpressions.Regex
				.Matches(table, @"class='con-verdict ([a-z]+)'>([^<]+)<")
				.Select(m => (Cls: m.Groups[1].Value, Text: m.Groups[2].Value))
				.ToList();

			Assert.Multiple(() =>
			{
				// The control row proves the difference is caused by the qualifier and not by the
				// renderer treating every row alike.
				Assert.That(verdicts[0].Text, Is.EqualTo("PASS"), "control: an ordinary pass");
				Assert.That(verdicts[0].Cls, Is.EqualTo("pass"));

				Assert.That(verdicts[1].Text, Is.EqualTo("QUALIFIED"),
					"the qualified connection does not read as a plain pass");
				Assert.That(verdicts[1].Cls, Is.Not.EqualTo("pass"),
					"and not in the green the review asked us to stop using for it");

				// The note column answers WHICH parameter — matched inside its own cell, because
				// "20.0" and "θ" both occur elsewhere in a 600 kB document.
				var notes = System.Text.RegularExpressions.Regex
					.Matches(table, @"class='con-note'>([^<]*)<")
					.Select(m => m.Groups[1].Value)
					.ToList();
				Assert.That(notes[0], Is.EqualTo("Norsok OK"), "control");
				Assert.That(notes[1], Does.Contain("20.0"), "the value that breached");
				Assert.That(notes[1], Does.Contain("M1"), "and the brace it belongs to");
			});
		}
	}
}
