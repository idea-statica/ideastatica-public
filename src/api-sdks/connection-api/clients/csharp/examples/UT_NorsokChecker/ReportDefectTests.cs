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
		// Equation EMPTY, matching what NorsokCheckRunner now emits: nothing was evaluated, so there
		// is no equation to name. It used to be "6.4.3" here and in production — a CLAUSE, printed as
		// "(Eq. 6.4.3)" on a card that computed nothing.
		private static NorsokFormulaResult Rejected(string condition) => new()
		{
			Section = "6.4", Equation = "", Title = "Outside the scope of §6.4",
			CheckExpression = condition,
			FormulaSubstituted = "no §6.4 check was performed for this joint",
			NotAssessed = true,
		};

		/// <summary>A topology note — what PublishTopologyNotes emits, with no equation.</summary>
		private static NorsokFormulaResult Note(string what) => new()
		{
			Section = "6.4.3.1", Equation = "", Title = "Assumption",
			CheckExpression = what, IsNote = true, NotAssessed = true,
		};

		private static NorsokFormulaResult Assessed(double util) => new()
		{
			Section = "6.4.3.6", Equation = "6.57", Title = "Tubular Joint — M1",
			LoadCaseName = "LE1", Utilization = util, Passed = true,
		};

		/// <summary>
		/// An assessed row WITH its derivation — the only way the validity table, the chord-stress
		/// trail and the eq (6.57) step are rendered at all.
		/// </summary>
		private static NorsokFormulaResult WithDerivation()
		{
			var inputs = NorsokChecker.Services.Norsok64.Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 60.0, g: 0.047,
				frK: 1.0, frY: 0.0, frX: 0.0,
				nSd: -10e3, mipSd: -1e3, mopSd: 0,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0,
				gammaM: 1.15);

			var row = Assessed(0.476);
			row.JointDetail = new NorsokChecker.Services.Norsok64.JointCheckRow
			{
				Name = "M1", Skipped = false, Util = 0.476, Passed = true,
				Engine = NorsokChecker.Services.Norsok64.Norsok64Engine.CheckJoint(inputs),
				Inputs = inputs, DomClass = "K",
				Classification = new NorsokChecker.Services.Norsok64.KyxClass
				{
					Name = "M1", FrK = 1.0, FrY = 0, FrX = 0,
				},
			};
			return row;
		}

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

		/// <summary>
		/// A card that evaluated no equation carries NO equation badge.
		///
		/// Measured in the exported PDF (161 pages, 15 connections): "(Eq. -)" twice and
		/// "(Eq. 6.4.3)" four times. Both are wrong in different ways — the dash is a placeholder
		/// shown to the customer, and 6.4.3 is a CLAUSE presented as an equation number, which in a
		/// compliance document is a statement about the norm that the norm does not support.
		///
		/// Asserted on the RENDERED string, not on the Equation property: the defect is what reaches
		/// the page, and the property is legitimately empty.
		///
		/// The rows here are this fixture's own, so this covers the RENDERER only. What PRODUCTION
		/// puts in Equation is a separate question with its own test below — measured: reverting
		/// Chapter64/NorsokCheckRunner left this one green, because a fixture cannot see what the app
		/// would have set.
		/// </summary>
		[Test]
		public void ACardWithNoEquationCarriesNoEquationBadge()
		{
			string html = Report(("CON10", new[]
			{
				Rejected("the load effects of this connection could not be read"),
				Note("M1: θ=20.0° outside 30–90°."),
			}));

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("(Eq. -)"),
					"a placeholder dash instead of an equation number");
				Assert.That(html, Does.Not.Contain("(Eq. 6.4.3)"),
					"6.4.3 is a clause, not an equation");
				Assert.That(html, Does.Not.Contain("(Eq. )"),
					"nor an empty badge where the number was suppressed");
				Assert.That(html, Does.Not.Contain("class='eq-ref'"),
					"neither row has an equation, so the badge is absent entirely");
			});
		}

		/// <summary>
		/// The rows the APP builds for an unevaluated check name no equation and no clause prefix.
		///
		/// The renderer test above cannot reach this: it builds its own rows, so it stayed green with
		/// production reverted to <c>Equation = "6.4.3"</c> and <c>Title = "§6.4 could not be
		/// evaluated"</c>. Both halves of the defect live in the app, not in the generator.
		///
		/// On the SOURCE, because the alternative is a live service: Chapter64's blocked path needs a
		/// failing API call, and NorsokCheckRunner's rejection path needs a real topology. A source
		/// check is ugly and it is the thing that fails when the wiring is undone.
		/// </summary>
		[Test]
		public void TheAppNamesNoEquationWhereNothingWasEvaluated()
		{
			string chapter = ReadAppSource("Services/Chapters/Chapter64.cs");
			string runner = ReadAppSource("Services/NorsokCheckRunner.cs");

			Assert.Multiple(() =>
			{
				Assert.That(chapter, Does.Not.Contain("Equation = \"6.4.3\""),
					"the blocked-chapter row: 6.4.3 is a clause, not an equation");
				Assert.That(runner, Does.Not.Contain("Equation = \"6.4.3\""),
					"and neither is the rejection row's");
				Assert.That(runner, Does.Not.Contain("Equation = \"-\""),
					"a note row evaluates nothing, so not a dash either");
				Assert.That(chapter, Does.Not.Contain("Title = \"§6.4 could not be evaluated\""),
					"the card prints §{Section} already — carrying it in the title doubled it");
			});
		}

		/// <summary>One of the app's own source files, with comments stripped.</summary>
		private static string ReadAppSource(string relative)
		{
			var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !System.IO.Directory.Exists(
				System.IO.Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Ignore("cannot locate the NorsokChecker source from the test output");

			string code = System.IO.File.ReadAllText(
				System.IO.Path.Combine(dir!.FullName, "NorsokChecker", relative));

			// The comments explain the defect by quoting it, so a raw match finds the explanation.
			return System.Text.RegularExpressions.Regex.Replace(code, @"//[^\n]*", "");
		}

		/// <summary>
		/// A check that DOES have an equation still shows it — the other half of the rule above.
		///
		/// Without this, suppressing the badge unconditionally would satisfy the test above while
		/// silently dropping "(Eq. 6.57)" from all thirty real checks.
		/// </summary>
		[Test]
		public void ACheckWithAnEquationStillShowsIt()
		{
			string html = Report(("CON1", new[] { Assessed(0.735) }));

			Assert.That(html, Does.Contain("(Eq. 6.57)"), "eq (6.57) is the §6.4.3.6 check");
		}

		/// <summary>
		/// The clause prefix is printed ONCE.
		///
		/// The card emits §{Section} beside the title, and the blocked-chapter row used to carry
		/// "§6.4" in its title as well, so the header read "§6.4 §6.4 could not be evaluated".
		/// Present once in the shipped PDF, which is once too often in a document a customer reads.
		/// </summary>
		[Test]
		public void TheClausePrefixIsNotDoubled()
		{
			string html = Report(("CON10", new[]
			{
				Rejected("the load effects of this connection could not be read"),
			}));

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("&sect;6.4 &sect;6.4"), "as emitted");
				Assert.That(html, Does.Not.Contain("§6.4 §6.4"), "and in case it is ever emitted raw");
			});
		}

		/// <summary>
		/// British spelling throughout — NORSOK and EN use it, and the report mixed both.
		///
		/// Measured: 30 × "Utilization" against 60 × "Utilisation" in one document, the American half
		/// coming entirely from the result bar. The C# property is still Utilization; renaming it
		/// would be churn no reader sees, which is why this asserts on the rendered page.
		/// </summary>
		[Test]
		public void TheRenderedReportUsesBritishSpellingOnly()
		{
			string html = Report(("CON1", new[] { Assessed(0.735) }));

			// The KaTeX stylesheet and script are embedded verbatim and are not ours to respell, so
			// only the report's own body is examined.
			int bodyAt = html.IndexOf("</head>", StringComparison.Ordinal);
			Assert.That(bodyAt, Is.GreaterThan(0), "the document has a head to skip");
			string body = html[bodyAt..];

			Assert.Multiple(() =>
			{
				Assert.That(body, Does.Not.Contain("Utilization"),
					"the result bar was the last American spelling in the report");
				Assert.That(body, Does.Contain("Utilisation:"), "and it still labels the value");
			});
		}

		/// <summary>
		/// The check condition in the header states the SAME formula the derivation evaluates.
		///
		/// The header printed the out-of-plane term bare while eq (6.57) is evaluated with its
		/// absolute value — so the two disagreed about the check being performed. Of everything found
		/// in this report, this is the one a reviewer reads as a calculation error rather than a
		/// typo: without the bars a negative M_z would REDUCE the utilisation sum.
		/// </summary>
		[Test]
		public void TheHeaderFormulaTakesTheAbsoluteValueLikeTheDerivation()
		{
			string html = Report(("CON1", new[] { Assessed(0.735) }));

			int at = html.IndexOf("Check condition:", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the §6.4 card states its check condition");
			string header = html[at..(at + 600)];

			Assert.Multiple(() =>
			{
				Assert.That(header, Does.Contain(@"\left|\frac{M_{z,Sd}}{M_{z,Rd}}\right|"),
					"the out-of-plane term is taken in absolute value, as eq (6.57) evaluates it");
				Assert.That(header, Does.Contain(@"\left(\frac{M_{y,Sd}}{M_{y,Rd}}\right)^2"),
					"and the in-plane term is still squared — bars must not have replaced the square");
			});
		}

		/// <summary>
		/// The validity conditions are typeset, not printed as ASCII source.
		///
		/// The engine states them as "0.2&lt;=beta&lt;=1.0"; the report printed that verbatim in a
		/// monospace face next to fully typeset KaTeX. Measured: 180 occurrences of "&lt;=" in the
		/// exported PDF's text layer.
		///
		/// Asserted on a REPORT carrying a real derivation, not by calling ConditionHtml directly.
		/// Measured: calling it directly left the guard green when the renderer was put back to
		/// Esc(cond) — the function was correct and connected to nothing, which is the failure mode
		/// a unit test on the helper cannot see.
		/// </summary>
		[Test]
		public void ValidityConditionsAreTypesetNotAscii()
		{
			// A row with a JointDetail, or the validity table is not rendered at all and this would
			// search a document that has no conditions in it.
			string html = Report(("CON1", new[] { WithDerivation() }));

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("&beta;"), "the conditions are rendered at all");
				Assert.That(html, Does.Not.Contain("&lt;="),
					"no ASCII <= reaches the page — 180 of them did");
				Assert.That(html, Does.Contain("&le;"), "they are proper ≤ signs");
				Assert.That(html, Does.Not.Contain(">beta"), "beta is a Greek letter, not a word");
				Assert.That(html, Does.Not.Contain("gamma&nbsp;"), "and so is gamma");
			});
		}

		/// <summary>
		/// The document does not call itself a compliance report.
		///
		/// It becomes the PDF's /Title — what Explorer, a browser tab and an archive show — and it
		/// claimed conformity for a document in which connections routinely go unassessed: 9 of 15
		/// in the reviewed sample. Both emission sites must agree, so the title lives in one
		/// constant; a test that read only the &lt;title&gt; would miss the printed header.
		/// </summary>
		[Test]
		public void TheTitleDoesNotClaimCompliance()
		{
			string html = Report(("CON1", new[] { WithDerivation() }));

			var title = System.Text.RegularExpressions.Regex.Match(html, @"<title>([^<]*)</title>");
			var badge = System.Text.RegularExpressions.Regex.Match(html,
				@"class='norsok-badge'>([^<]*)<");

			Assert.Multiple(() =>
			{
				Assert.That(title.Success, Is.True, "the document has a title");
				Assert.That(title.Groups[1].Value, Does.Not.Contain("Compliance"),
					"'Compliance Report' overclaims; it is a check, and it may find nothing to check");
				Assert.That(badge.Success, Is.True, "and a printed header");
				Assert.That(badge.Groups[1].Value, Is.EqualTo(title.Groups[1].Value),
					"the two say the same thing — they were separate literals and could drift");
				Assert.That(title.Groups[1].Value, Does.Contain("NORSOK N-004"),
					"it still names the standard");
			});
		}

		/// <summary>
		/// The §6.1 quotation and the tool's own disclosure are SEPARATE paragraphs.
		///
		/// They were one grey italic block, so "these factors are written into the project's own
		/// settings" — this app telling the reader it MODIFIES their model — read as part of the
		/// quotation from the standard. Nothing else in the document says the file is changed.
		/// </summary>
		[Test]
		public void TheNormQuoteIsSeparateFromTheToolsDisclosure()
		{
			string html = Report(("CON1", new[] { WithDerivation() }));

			var quote = System.Text.RegularExpressions.Regex.Match(html,
				@"class='settings-note settings-quote'>(.*?)</p>",
				System.Text.RegularExpressions.RegexOptions.Singleline);
			var disclosure = System.Text.RegularExpressions.Regex.Match(html,
				@"class='settings-disclosure'>(.*?)</p>",
				System.Text.RegularExpressions.RegexOptions.Singleline);

			Assert.Multiple(() =>
			{
				Assert.That(quote.Success, Is.True, "the §6.1 quotation is its own paragraph");
				Assert.That(disclosure.Success, Is.True, "and the disclosure is another");

				// The decisive assertion: the disclosure's sentence must NOT be inside the quotation.
				Assert.That(quote.Groups[1].Value, Does.Not.Contain("written"),
					"the quotation ends where the standard's words end");
				Assert.That(disclosure.Groups[1].Value, Does.Contain("writes these factors"),
					"and the disclosure states what the tool does");
				Assert.That(disclosure.Groups[1].Value, Does.Contain("<strong>"),
					"prominently — it was invisible as grey italics");
			});
		}

	}
}
