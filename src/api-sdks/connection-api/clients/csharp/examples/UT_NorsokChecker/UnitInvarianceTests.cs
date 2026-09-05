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
			var metric = Regex.Matches(Content(Page(Metric)), @"(\d+\.\d+)\s*\\?%")
				.Select(m => m.Groups[1].Value).ToList();
			var imperial = Regex.Matches(Content(Page(Imperial)), @"(\d+\.\d+)\s*\\?%")
				.Select(m => m.Groups[1].Value).ToList();

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
					Joint64ReportAdapter.BuildResultFromRow(MultiModeRow(), "LE12", Imperial),
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
		/// The engine itself, asserted directly rather than through the page: the same input gives
		/// the same dimensionless results no matter what the display settings say. The page test
		/// above would also catch this, but not tell you which side broke.
		/// </summary>
		[Test]
		public void TheEngineIsIndifferentToTheDisplaySettings()
		{
			var before = MultiModeRow().Engine!;
			Joint64ReportAdapter.Display = Imperial;
			var after = MultiModeRow().Engine!;
			Joint64ReportAdapter.Display = Metric;   // leave the static as we found it

			Assert.Multiple(() =>
			{
				Assert.That(after.UtilWeighted, Is.EqualTo(before.UtilWeighted).Within(1e-12));
				Assert.That(after.Beta, Is.EqualTo(before.Beta).Within(1e-12));
				Assert.That(after.NRdWeighted, Is.EqualTo(before.NRdWeighted).Within(1e-9));
			});
		}
	}
}
