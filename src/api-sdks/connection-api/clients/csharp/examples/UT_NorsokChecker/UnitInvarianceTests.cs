using System.Text.RegularExpressions;
using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// A UNIT CHANGE MUST NOT MOVE A CHECK RESULT.
	///
	/// The one invariant of the display-units feature, and the reason it can be changed safely: the
	/// engine computes in SI and everything printed is a conversion, so the utilisation, the mode
	/// split and every Q-factor are the same numbers in kN as in kip. If this fails, a conversion
	/// was applied to a value that FEEDS the calculation rather than to one that leaves it — which
	/// is the one way this feature could produce a wrong structural verdict rather than an ugly page.
	///
	/// It also asserts the other half, which is what makes the first half meaningful: the units DID
	/// change. A page that ignored the setting entirely would pass an invariance test trivially.
	///
	/// THE EMBEDDED KATEX IS EXCLUDED, and that is not a detail. Counting "kN" across the raw HTML
	/// finds 83 hits on an imperial page — every one of them a coincidental letter pair inside the
	/// base64 font, plus KaTeX's own pt/mm/cm/in table. A first version of this test read those as
	/// a defect in the report.
	/// </summary>
	[TestFixture]
	public class UnitInvarianceTests
	{
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
			var res = Norsok64Engine.CheckJoint(inp);
			return new JointCheckRow
			{
				Name = "M3", Skipped = false, Engine = res, Inputs = inp,
				Classification = new KyxClass
				{
					Name = "M3", FrK = 0.19, FrY = 0.38, FrX = 0.43,
					NSd = -88.8e3, MipSd = -1.2e3, MopSd = 2.4e3,
				},
				ChordStress = new ChordStressRow
				{
					Name = "M3", SigmaA = 9.27e6, SigmaMy = -25.48e6, SigmaMz = 0.0,
					A = 2.7477e-3, I = 6.1e-6, R = 0.0705,
					NChord = -103.6e3, MipChord = 0.748e3, MopChord = -1.027e3, Side = 1,
				},
				DomClass = "X",
				Util = res.UtilWeighted, Passed = res.Passed,
				NRdWeighted = res.NRdWeighted, MRdIp = res.MRdIp, MRdOp = res.MRdOp,
				WithinRange = res.WithinRange, ChordOverstressed = res.ChordOverstressed,
			};
		}

		private static string Page(DisplaySettings s) =>
			NorsokHtmlReportGenerator.GenerateDerivationPage(MultiModeRow(), "M3",
				"CON1", "LE12", "47.5909", "PASS", s);

		private static readonly DisplaySettings Metric = new();

		private static readonly DisplaySettings Imperial = new()
		{
			Force = ForceUnit.Kip, Stress = StressUnit.Ksi, Length = LengthUnit.Inch,
		};

		/// <summary>
		/// The page content with the embedded KaTeX stripped — its stylesheet, its base64 fonts and
		/// its script all carry letter sequences that read as units.
		/// </summary>
		private static string Content(string html)
		{
			string s = Regex.Replace(html, "<style.*?</style>", "", RegexOptions.Singleline);
			s = Regex.Replace(s, "<script.*?</script>", "", RegexOptions.Singleline);
			return Regex.Replace(s, @"data:[^""')]{100,}", "");
		}

		/// <summary>
		/// A percentage as the page carries it: digits, then the non-breaking space QuantityFormat
		/// puts before the sign — raw, or HTML-encoded to <c>&amp;#160;</c> where the text went
		/// through Esc — then <c>%</c> (or KaTeX's <c>\%</c>). Group 1 is the decimals.
		/// </summary>
		private const string PercentPattern = @"\d+\.(\d+)(?:\s|&#160;|&nbsp;)*\\?%";

		private static int Count(string content, string unit) =>
			Regex.Matches(content,
				@"(?<![A-Za-z0-9+/])" + Regex.Escape(unit) + @"(?![A-Za-z0-9+/])").Count;

		[Test]
		public void TheDerivationPrintsTheChosenUnitsAndNoOthers()
		{
			string metric = Content(Page(Metric));
			string imperial = Content(Page(Imperial));

			Assert.Multiple(() =>
			{
				Assert.That(Count(metric, "kN"), Is.GreaterThan(0), "metric page prints no kN");
				Assert.That(Count(metric, "MPa"), Is.GreaterThan(0), "metric page prints no MPa");
				Assert.That(Count(metric, "kip"), Is.Zero, "metric page prints kip");

				Assert.That(Count(imperial, "kip"), Is.GreaterThan(0), "imperial page prints no kip");
				Assert.That(Count(imperial, "ksi"), Is.GreaterThan(0), "imperial page prints no ksi");
				Assert.That(Count(imperial, "kN"), Is.Zero, "imperial page still prints kN");
				Assert.That(Count(imperial, "MPa"), Is.Zero, "imperial page still prints MPa");
				Assert.That(Count(imperial, "mm"), Is.Zero, "imperial page still prints mm");
			});
		}

		/// <summary>
		/// Every percentage on the page — the utilisation, its three interaction terms and the K/Y/X
		/// split — is identical in both unit systems. These are the numbers a verdict rests on.
		/// </summary>
		[Test]
		public void NoPrintedResultMovesWhenTheUnitsChange()
		{
			var metric = Regex.Matches(Content(Page(Metric)), PercentPattern)
				.Select(m => m.Value).ToList();
			var imperial = Regex.Matches(Content(Page(Imperial)), PercentPattern)
				.Select(m => m.Value).ToList();

			Assert.That(metric, Is.Not.Empty, "no percentage found — the regex or the page changed");
			Assert.That(imperial, Is.EqualTo(metric),
				"a unit change moved a printed result; the conversion reached a value that feeds "
				+ "the calculation, not one that leaves it");
		}

		/// <summary>
		/// THE WHOLE REPORT, not just one derivation page — the call site is a separate way to fail.
		///
		/// This test exists because the report ignored the setting completely for a while: every
		/// value inside GenerateReport had been converted, and the one call in MainWindow.Report.cs
		/// simply did not pass `_display`. Three unit audits found it; the derivation-page test
		/// above could not, because it calls the generator directly.
		/// </summary>
		[Test]
		public void TheWholeReportPrintsTheChosenUnits()
		{
			var allResults = new List<(string, List<NorsokFormulaResult>)>
			{
				("CON1", new List<NorsokFormulaResult>
				{
					Joint64ReportAdapter.BuildResultFromRow(MultiModeRow(), "LE12"),
				}),
			};

			string html = Content(NorsokHtmlReportGenerator.GenerateReport(
				"probe.ideaCon", allResults, expandAll: true, display: Imperial));

			Assert.Multiple(() =>
			{
				Assert.That(Count(html, "kip"), Is.GreaterThan(0), "the report reached no kip");
				Assert.That(Count(html, "kN"), Is.Zero, "the report still prints kN");
				Assert.That(Count(html, "MPa"), Is.Zero, "the report still prints MPa");
			});
		}

		/// <summary>
		/// THE PRECISION COLUMNS DO SOMETHING, and one quantity keeps ONE precision per page.
		///
		/// γ printed as `10.846` in the geometry table and `10.85` six steps later, γ_M as `1.150`
		/// and `1.15`, because ~30 sites carried their own literal decimal count and none of them
		/// read RatioDecimals. A reader who spots that reasonably asks which of the two the check
		/// used.
		/// </summary>
		[Test]
		public void TheRatioPrecisionIsReadAndIsConsistentAcrossThePage()
		{
			string coarse = Page(new DisplaySettings { RatioDecimals = 2 });
			string fine = Page(new DisplaySettings { RatioDecimals = 6 });

			Assert.Multiple(() =>
			{
				Assert.That(fine, Is.Not.EqualTo(coarse), "RatioDecimals changed nothing");

				// β = 0.723404 on this fixture: at six decimals it must appear in full, and the
				// three-decimal spelling must not survive anywhere on the same page.
				Assert.That(Content(fine), Does.Contain("0.723404"),
					"β is not printed at the requested precision");
				Assert.That(Regex.Matches(Content(fine), @"\b0\.723\b").Count, Is.Zero,
					"β still appears at a hardcoded 3 decimals somewhere on the page");
			});
		}

		/// <summary>
		/// THE FORMAT COLUMN DOES SOMETHING.
		///
		/// The dialog offers Decimal / Scientific / Automatic per quantity, and for a long time the
		/// derivation ignored all three: every number went through a local fixed-decimal helper, so
		/// the column was a promise with nothing behind it. A page that renders identically under
		/// two opposite format choices is that bug returning.
		/// </summary>
		[Test]
		public void TheNumberFormatChoiceChangesThePage()
		{
			var dec = new DisplaySettings
			{
				AreaFormat = NumberFormat.Decimal,
				InertiaFormat = NumberFormat.Decimal,
				ForceFormat = NumberFormat.Decimal,
			};
			var sci = new DisplaySettings
			{
				AreaFormat = NumberFormat.Scientific,
				InertiaFormat = NumberFormat.Scientific,
				ForceFormat = NumberFormat.Scientific,
			};

			Assert.That(Page(sci), Is.Not.EqualTo(Page(dec)),
				"Decimal and Scientific produced the same page — the format setting is not read");
		}

		/// <summary>
		/// A CARD BUILT UNDER ONE SETTING AND PRINTED UNDER ANOTHER SHOWS THE SETTING IN FORCE AT
		/// PRINT TIME — every percentage on its header row, not only the badge.
		///
		/// The user's sequence: run a check, then change "Utilisation, shares" to 2. The row read
		/// `K 0.0 % / Y 0.0 % / X 100.0 %` beside `32.10%`, because the title was a finished sentence
		/// frozen into the card when the check ran, while the badge was formatted by the generator
		/// from the value. Two precisions on one line, and a reader asks which the check used.
		///
		/// The card is built with no settings in scope at all — that is the fix: there is nothing at
		/// calculation time for a precision to freeze into.
		/// </summary>
		[Test]
		public void ACardPrintsThePrecisionInForceWhenItIsPrinted()
		{
			var atPrint = new DisplaySettings { PercentDecimals = 3 };
			var card = Joint64ReportAdapter.BuildResultFromRow(MultiModeRow(), "LE12");

			var allResults = new List<(string, List<NorsokFormulaResult>)>
			{
				("CON1", new List<NorsokFormulaResult> { card }),
			};
			string html = Content(NorsokHtmlReportGenerator.GenerateReport(
				"probe.ideaCon", allResults, expandAll: true, display: atPrint));

			var header = Regex.Match(html, @"<summary class='card-header[^']*'>(.*?)</summary>",
				RegexOptions.Singleline);
			Assert.That(header.Success, "no card header on the page");

			var decimals = Regex.Matches(header.Groups[1].Value, PercentPattern)
				.Select(m => m.Groups[1].Value.Length).ToList();
			Assert.That(decimals, Has.Count.GreaterThanOrEqualTo(4),
				"the header should carry the K/Y/X split and the badge");
			Assert.That(decimals, Is.All.EqualTo(3),
				"a percentage on the card header still carries the precision of the RUN, not of the print");
		}

		/// <summary>
		/// NO DISPLAY STATE INSIDE CALCULATION CODE — structurally, not by inspection.
		///
		/// Every defect in this family had the same shape: a static <c>DisplaySettings</c> reachable
		/// from the engine or the topology builder, read while a sentence was composed at calculation
		/// time, so the sentence froze in whatever units were set at that moment. The card adapter
		/// had one, the topology builder had one; both are gone. This asserts that none comes back:
		/// no type in the calculation namespaces declares a static field or property of that type.
		/// A <c>DisplaySettings</c> PARAMETER is fine — that is how a renderer is handed the setting.
		///
		/// The one static that remains, <c>ConnectionCheckResult.Display</c>, lives in Models and
		/// serves a WPF binding, which is outside the namespaces checked here.
		/// </summary>
		[Test]
		public void NoCalculationTypeHoldsDisplayState()
		{
			var asm = typeof(Norsok64Engine).Assembly;
			var offenders = asm.GetTypes()
				.Where(t => t.Namespace is { } ns
					&& (ns.StartsWith("NorsokChecker.Services.Norsok64")
						|| ns.StartsWith("NorsokChecker.Services.Chapters")))
				.SelectMany(t => t
					.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
						| System.Reflection.BindingFlags.NonPublic)
					.Where(f => f.FieldType == typeof(DisplaySettings))
					.Select(f => $"{t.Name}.{f.Name}")
					.Concat(t
						.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
							| System.Reflection.BindingFlags.NonPublic)
						.Where(p => p.PropertyType == typeof(DisplaySettings))
						.Select(p => $"{t.Name}.{p.Name}")))
				.ToList();

			Assert.That(offenders, Is.Empty,
				"calculation code holds display state again; a sentence composed there freezes in "
				+ "the units of the moment. Carry values and render where the setting is in scope.");
		}

		/// <summary>
		/// A GATE MESSAGE IS WRITTEN IN THE READER'S UNITS WHEN IT IS SHOWN — the same overlap, in
		/// millimetres or in inches, from one record carrying metres. The verdict it belongs to is
		/// the same under both settings; only the words move.
		/// </summary>
		[Test]
		public void AGateMessageRendersInTheUnitsItIsShownIn()
		{
			var overlap = new GateMessage(GateKind.Overlap, "M1", "M3", Value: -0.032);

			var topo = new JointTopology();
			topo.Gaps.Add(new BraceGap { A = "M1", B = "M3", GapM = -0.032, Adjacent = true, Known = true });
			JointTopologyBuilder.FinalizeVerdict(topo);

			Assert.Multiple(() =>
			{
				Assert.That(overlap.Render(Metric), Does.Contain("-32.0 mm"));
				Assert.That(overlap.Render(Imperial), Does.Contain("-1.26 in").Or.Contain("-1.3 in"));
				Assert.That(overlap.Render(Imperial), Does.Not.Contain("mm"));

				Assert.That(topo.Verdict.Status, Is.EqualTo("ERROR"));
				Assert.That(topo.Verdict.Errors.Texts(Imperial).Single(), Does.Contain("in"));
				Assert.That(topo.Verdict.Errors.Texts(Metric).Single(), Does.Contain("mm"));
			});
		}
	}
}
