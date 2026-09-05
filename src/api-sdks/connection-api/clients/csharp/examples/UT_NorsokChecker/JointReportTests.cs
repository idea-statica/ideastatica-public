using System.IO;
using System.Text.RegularExpressions;
using IdeaStatiCa.Api.Connection.Model;
using Newtonsoft.Json.Linq;
using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Smoke tests for the §6.4 auto-topology report path: JointCheckRow → BuildResultFromRow card →
	/// NorsokHtmlReportGenerator derivation blocks (per-class table, K-per-gap, chord-stress trail,
	/// validity). Uses the KT fixture — the richest case (two K gaps + mixed K/Y classification).
	/// </summary>
	[TestFixture]
	public class JointReportTests
	{
		private static JointTopology BuildKtTopology()
		{
			string dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
			var fixtures = JObject.Parse(File.ReadAllText(Path.Combine(dir, "topology_fixtures.json")));

			var sections = new Dictionary<int, JointSectionInfo>();
			foreach (var cs in (JArray)fixtures["crossSections"]!)
			{
				var (d, t) = JointSectionInfo.ParseChs((string?)cs["name"]);
				sections[(int)cs["id"]!] = new JointSectionInfo
				{
					Name = (string?)cs["name"], D = d, T = t, IsCHS = d != null,
					Fy = (double?)cs["material"]?["element"]?["fy"],
				};
			}
			var fx = ((JArray)fixtures["fixtures"]!).First(f => (string?)f["name"] == "KT_TEST");
			var members = fx["members"]!
				.Select(j => j.ToObject<ConMember>()!)
				.Select(m => JointMemberData.FromConMember(m,
					sections.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
				.ToList();
			var les = fx["loadEffects"]!.Select(j => j.ToObject<ConLoadEffect>()!).ToList();
			return new JointTopologyBuilder().Build(members, les);
		}

		[Test]
		public void BuildResultFromRow_CarriesDetailAndClassification()
		{
			var topo = BuildKtTopology();
			var row = topo.JointChecks[0].Rows.First(r => r.Name == "KA");   // diagonal with 2 K gaps + Y remainder
			var card = Joint64ReportAdapter.BuildResultFromRow(row, "LE1");

			Assert.Multiple(() =>
			{
				Assert.That(card.JointDetail, Is.SameAs(row), "detail attached for the report");
				Assert.That(card.Section, Is.EqualTo("6.4.3.6"));
				Assert.That(card.Utilization, Is.EqualTo(row.Util).Within(1e-12));
				Assert.That(card.Passed, Is.EqualTo(row.Passed));
				Assert.That(card.Title, Does.Contain("KA"));
				// classification fractions surfaced as variables
				Assert.That(card.Variables.Any(v => v.Symbol == "frK"), "frK variable");
				// two balancing gaps → two K-gap breakdown variables
				Assert.That(card.Variables.Count(v => v.Symbol.StartsWith("K gap")), Is.EqualTo(2), "K per-gap rows");
			});
		}

		[Test]
		public void HtmlReport_RendersDerivationBlocks()
		{
			var topo = BuildKtTopology();
			var results = new List<NorsokFormulaResult>();
			foreach (var r in topo.JointChecks[0].Rows.Where(r => !r.Skipped))
				results.Add(Joint64ReportAdapter.BuildResultFromRow(r, "LE1"));

			string html = NorsokHtmlReportGenerator.GenerateReport(
				"UT", new[] { ("KT_TEST", results) }, expandAll: true);

			Assert.Multiple(() =>
			{
				// STRUCTURE and COUNTS, not headings. This asserted eleven needles, five of them
				// prose — "Geometry &amp; material", "Chord stress derivation" and the like — which
				// pinned the wording of headings that are meant to be edited. What has to hold is
				// that every brace got a derivation and that each one is built out of steps.
				// Counted on the OPENING TAG at its own indentation: a bare "deriv-block" also
				// matches the joint-plane section, which opens one of its own, and the stylesheet
				// rule. My first count said 3 and got 5 for exactly that reason.
				Assert.That(Regex.Matches(html, @"    <div class='deriv-block'>").Count,
					Is.EqualTo(3),
					"one derivation block per brace of the KT joint, not one for the joint");
				Assert.That(Regex.Matches(html, "deriv-step").Count, Is.GreaterThan(3 * 5),
					"and each is a sequence of substitution steps, not a table of results");

				// The braces themselves, so a report that rendered three blocks for one brace fails.
				Assert.That(html, Does.Contain("KA"));
				Assert.That(html, Does.Contain("KV"));
				Assert.That(html, Does.Contain("KB"));

				// The §6.4.3.1 conditions are always tabulated, pass or fail — the sheet is meant to
				// be checked, and a summarised "all met" would ask the reader to trust the app. The
				// clause number is the standard's, not our wording, so it is safe to pin.
				Assert.That(html, Does.Contain("&sect;6.4.3.1"), "the validity table");
			});
		}

		/// <summary>
		/// END TO END for the round-2 §4.1 fix: engine → adapter → report, on geometry that is really
		/// outside a §6.4.3.1 range. The caveat must arrive in the overview row.
		///
		/// Why this exists on top of the CheckWorkflow and ReportTable tests: those build their rows
		/// by hand, so `RangeQualifier = null` in the adapter left every one of them green — measured
		/// with the revert oracle. THE WIRING is the half nothing else guards.
		///
		/// And why the input is built here rather than taken from topology_fixtures.json: **not one
		/// brace in any existing fixture is out of range** (both oracle files carry zero
		/// `within_range: false`). A test whose fixture cannot contain the case it claims to cover
		/// cannot fail.
		/// </summary>
		[Test]
		public void HtmlReport_OutOfRangeBrace_PutsTheCaveatInTheOverview()
		{
			// θ = 20°, everything else inside its range — CON11/M1 of the reviewed report.
			var outOfRange = Joint64Input.FromKn(
				D: 141, T: 6.5, fyChord: 355, d: 76, t: 3.5, fyBrace: 355, thetaDeg: 20, g: 50,
				frK: 0.0, frY: 0.0, frX: 1.0,
				nSdKn: 33.7, mipSdKnm: 0.32, mopSdKnm: 1.23);
			// the same joint at 60° — the control, and it must come out an ordinary PASS
			var inRange = Joint64Input.FromKn(
				D: 141, T: 6.5, fyChord: 355, d: 76, t: 3.5, fyBrace: 355, thetaDeg: 60, g: 50,
				frK: 0.0, frY: 0.0, frX: 1.0,
				nSdKn: 33.7, mipSdKnm: 0.32, mopSdKnm: 1.23);

			static NorsokFormulaResult Card(string brace, Joint64Input inp)
			{
				var eng = Norsok64Engine.CheckJoint(inp);
				var row = new JointCheckRow
				{
					Name = brace, Util = eng.UtilWeighted, Passed = eng.Passed,
					Inputs = inp, Engine = eng, DomClass = "X",
					Classification = new KyxClass { Name = brace, FrK = 0, FrY = 0, FrX = 1.0 },
				};
				return Joint64ReportAdapter.BuildResultFromRow(row, "LE9");
			}

			var qualified = Card("M1", outOfRange);
			var ordinary = Card("M1", inRange);

			string html = NorsokHtmlReportGenerator.GenerateReport(
				"UT",
				new[] { ("CON_OK", new List<NorsokFormulaResult> { ordinary }),
						("CON_OOR", new List<NorsokFormulaResult> { qualified }) },
				expandAll: false);

			int at = html.IndexOf("class='connection-table'", StringComparison.Ordinal);
			string table = html[at..html.IndexOf("</table>", at, StringComparison.Ordinal)];
			var rows = System.Text.RegularExpressions.Regex
				.Matches(table, @"class='con-verdict ([a-z]+)'>([^<]+)<.*?class='con-note'>([^<]*)<",
					System.Text.RegularExpressions.RegexOptions.Singleline)
				.Select(m => (Cls: m.Groups[1].Value, Verdict: m.Groups[2].Value, Note: m.Groups[3].Value))
				.ToList();

			Assert.Multiple(() =>
			{
				// The engine has to actually disagree about these two, or the rest measures nothing.
				Assert.That(Norsok64Engine.CheckJoint(inRange).WithinRange, Is.True, "control is inside");
				Assert.That(Norsok64Engine.CheckJoint(outOfRange).WithinRange, Is.False, "case is outside");
				Assert.That(qualified.Passed, Is.True, "and it PASSES — that is what makes it silent");

				Assert.That(rows, Has.Count.EqualTo(2));
				Assert.That(rows[0].Verdict, Is.EqualTo("PASS"), "control row");
				Assert.That(rows[0].Note, Is.EqualTo("Norsok OK"));

				Assert.That(rows[1].Verdict, Is.EqualTo("QUALIFIED"));
				Assert.That(rows[1].Cls, Is.Not.EqualTo("pass"), "non-green, as the review asked");
				Assert.That(rows[1].Note, Does.Contain("20.0"), "the value that breached");
				Assert.That(rows[1].Note, Does.Contain("M1"), "the brace it belongs to");
			});
		}

		/// <summary>
		/// The joint figure carries a colour scale, and its swatches are the LIT tones.
		///
		/// The caption claims "members coloured by their governing utilisation" and the reviewed
		/// report had no scale anywhere, so an olive member could be at 40 % or 70 %.
		///
		/// The lit/flat distinction is the part that would go wrong silently: the figure is a PNG
		/// rendered by the 3D view, whose members carry UtilisationScale's LIT tones, so a legend
		/// drawn from the flat swatches would sit beside a lit cylinder and not match. Both arrays
		/// exist for that reason, and using the wrong one produces a legend that looks plausible.
		/// </summary>
		[Test]
		public void HtmlReport_JointFigure_CarriesTheUtilisationScale()
		{
			// A 1x1 transparent PNG — the figure only has to be PRESENT for the legend to render.
			const string png = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAC0lEQVR4nGMAAQAABQAB"
				+ "oIJXOQAAAABJRU5ErkJggg==";

			string html = NorsokHtmlReportGenerator.GenerateReport(
				"UT",
				new[] { ("CON1", new List<NorsokFormulaResult>()) },
				expandAll: false,
				jointImages: new Dictionary<string, string> { ["CON1"] = png });

			int at = html.IndexOf("class='util-legend'", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the legend is rendered beside the figure");
			string legend = html[at..html.IndexOf("</div>", at, StringComparison.Ordinal)];

			var swatches = System.Text.RegularExpressions.Regex
				.Matches(legend, @"class='util-swatch[^']*' style='background:(#[0-9A-Fa-f]{6})'")
				.Select(m => m.Groups[1].Value.ToUpperInvariant())
				.ToList();

			Assert.Multiple(() =>
			{
				// Eleven bands: ten across 0..100 % plus the separated over-capacity one.
				Assert.That(swatches, Has.Count.EqualTo(UtilisationScale.BandCount),
					"one swatch per band, including over-capacity");
				Assert.That(swatches.Distinct().Count(), Is.EqualTo(swatches.Count),
					"no two bands share a colour, or the scale cannot be read back");

				// THE assertion: lit tones, not flat ones. Band 0 is #66BB6A lit against #43A047 flat.
				Assert.That(swatches[0],
					Is.EqualTo(UtilisationScale.LitHexOfBand(0).ToUpperInvariant()),
					"the swatches match the LIT scale the 3D figure was rendered with");
				Assert.That(swatches[0],
					Is.Not.EqualTo(UtilisationScale.HexOfBand(0).ToUpperInvariant()),
					"and are therefore NOT the flat swatches, which would not match the picture");

				// The scale has to be readable as a scale: its ends are labelled.
				Assert.That(legend, Does.Contain("100 %"), "the top of the ramp is named");
				Assert.That(legend, Does.Contain("util-swatch-over"),
					"and over-capacity is set apart, not shown as a finer step");
			});
		}

		/// <summary>
		/// The report must not claim COMPLIANT for a run in which nothing was checked.
		///
		/// Every count is zero there, so the verdict arithmetic fell through to the all-clear: a
		/// green tick and "COMPLIANT" over "0 Total Checks", in the exportable PDF, while the
		/// connection list correctly said N/A for the same run. Reachable by unchecking both chapter
		/// boxes and pressing Run. The guard that catches this in the grid was never added here.
		/// </summary>
		[Test]
		public void HtmlReport_WithNoChecksAtAll_IsNotCompliant()
		{
			string html = NorsokHtmlReportGenerator.GenerateReport(
				"UT", new[] { ("EMPTY", new List<NorsokFormulaResult>()) }, expandAll: true);

			Assert.Multiple(() =>
			{
				// `Does.Not.Contain("COMPLIANT</")` was here and could not fail: "NON-COMPLIANT</"
				// contains "COMPLIANT</", so the assertion was equally satisfied by the verdict it
				// was written to reject and by the one it was written to allow. A negative lookbehind
				// separates the two.
				Assert.That(html, Does.Not.Match(@"(?<!NON-)COMPLIANT<"),
					"a run with no checks must not be reported as compliant");
				Assert.That(html, Does.Contain("NOT ASSESSED"), "it must say so instead");
			});
		}

		/// <summary>
		/// A not-assessed row must not show a utilisation. "0.0 %" and "(= 0.0000 ≤ 1.0)" asserted a
		/// comfortable pass next to the word N/A — the same trap the results grid already avoids with
		/// an em dash, duplicated in the report.
		/// </summary>
		[Test]
		public void HtmlReport_NotAssessedRow_ShowsNoUtilisation()
		{
			var results = new List<NorsokFormulaResult>
			{
				new()
				{
					Section = "6.4", Equation = "6.4.3",
					Title = "Outside the scope of §6.4",
					CheckExpression = "Chord: HEB300 is RolledI — NORSOK 6.4 applies to tubular sections only.",
					NotAssessed = true,
				},
			};

			string html = NorsokHtmlReportGenerator.GenerateReport(
				"UT", new[] { ("REJECTED", results) }, expandAll: true);

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("0.0%"),
					"an unassessed row has no utilisation, and 0.0 % reads as an excellent one");
				Assert.That(html, Does.Not.Contain("&le; 1.0"),
					"nor is there any inequality to assert about it");
				Assert.That(html, Does.Contain("card-header warn"),
					"and the card must carry the warn styling that now exists for it");
			});
		}
	}
}
