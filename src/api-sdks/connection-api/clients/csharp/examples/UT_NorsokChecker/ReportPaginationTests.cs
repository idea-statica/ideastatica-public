using System.Text.RegularExpressions;
using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The report as a PAGINATED document: a contents page, numbered chapters, and one page per
	/// connection.
	///
	/// Before this the PDF ran the connections together — there was no <c>@page</c> rule anywhere and
	/// <c>.connection-header</c> had margins but no <c>break-before</c> — and there was no way in
	/// other than reading it front to back, which on fifteen joints is not how anyone uses it.
	/// </summary>
	[TestFixture]
	public class ReportPaginationTests
	{
		private static NorsokFormulaResult Assessed(string brace, double util, bool passed)
		{
			var inputs = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 60.0, g: 0.047,
				frK: 1.0, frY: 0.0, frX: 0.0,
				nSd: -10e3, mipSd: -1e3, mopSd: 0,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0,
				gammaM: 1.15);

			return new NorsokFormulaResult
			{
				Section = "6.4.3.6", Equation = "6.57",
				Title = $"Tubular Joint — {brace}",
				Utilization = util, Passed = passed,
				JointDetail = new JointCheckRow
				{
					Name = brace, Skipped = false, Util = util, Passed = passed,
					Engine = Norsok64Engine.CheckJoint(inputs), Inputs = inputs, DomClass = "K",
					Classification = new KyxClass { Name = brace, FrK = 1.0, FrY = 0, FrX = 0 },
				},
			};
		}

		private static NorsokFormulaResult Rejected(string why) => new()
		{
			Section = "6.4", Equation = "6.4.3", Title = "Outside the scope of §6.4",
			CheckExpression = why, NotAssessed = true,
		};

		/// <summary>Three connections, one of each verdict — so the index has something to differ on.</summary>
		private static string Report() =>
			NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", new List<NorsokFormulaResult> { Assessed("M1", 0.476, true) }),
					("CON2", new List<NorsokFormulaResult> { Assessed("M1", 1.30, false) }),
					("CON5", new List<NorsokFormulaResult> { Rejected("no brace") }),
				},
				expandAll: false);

		/// <summary>
		/// A report with TWO method chapters on a connection, which is what makes a contents page
		/// worth printing at all (see ShouldRenderContents). §6.4 is the only method that exists
		/// today, so the second row is synthetic — and that is the point: the contents tests below
		/// describe how a contents behaves, and without a second method there is no contents to
		/// describe. The single-method report is asserted separately, by
		/// ASingleMethodReportHasNoContentsPage.
		/// </summary>
		private static string MultiMethodReport() =>
			NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", new List<NorsokFormulaResult>
					{
						Assessed("M1", 0.476, true),
						new()
						{
							Section = "6.3.2", Equation = "6.1", Title = "Axial tension",
							Utilization = 0.31, Passed = true,
						},
					}),
					("CON2", new List<NorsokFormulaResult> { Assessed("M1", 1.30, false) }),
					("CON5", new List<NorsokFormulaResult> { Rejected("no brace") }),
				},
				expandAll: false);

		/// <summary>
		/// With ONE method per connection there is no contents page.
		///
		/// Round-3 §1, and it supersedes the round-2 decision to keep it: once the verdicts and the
		/// chapter numbers go (they duplicate the overview, and the number shifts whenever a
		/// connection is added), the contents is fifteen lines reproducing the first column of the
		/// table on the next page — a full page out of 173 carrying nothing the reader cannot see
		/// there. The emptiness is also why the verdicts were in it: they were filling a structural
		/// void, not serving anybody.
		///
		/// A property of the DOCUMENT, not a setting: nobody should be asked to decide what the
		/// document already determines. This is today's report, so this branch is the one that ships.
		/// </summary>
		[Test]
		public void ASingleMethodReportHasNoContentsPage()
		{
			string single = Report();
			string multi = MultiMethodReport();

			Assert.Multiple(() =>
			{
				Assert.That(single, Does.Not.Contain("class='index-table'"),
					"one method per connection: nothing to index");
				// The control, and it is what makes the assertion above mean something: the SAME
				// generator does print a contents when there is a hierarchy to map.
				Assert.That(multi, Does.Contain("class='index-table'"),
					"two methods on a connection: a contents earns its place");

				// And the page it used to cost is not left behind as a blank one — the first
				// connection must now break for itself, since no contents started that page.
				//
				// Matched on the HEADING, not on the class name: ".connection-header.first-connection"
				// is a rule in the stylesheet every report carries, so a bare
				// Does.Not.Contain("first-connection") can never fail. Measured — it did not.
				Assert.That(Regex.Matches(single, @"<h2 class='connection-header first-connection'"),
					Is.Empty,
					"with no contents there is nothing to have positioned the first connection");
				Assert.That(Regex.Matches(multi, @"<h2 class='connection-header first-connection'"),
					Has.Count.EqualTo(1),
					"with one, the exception applies to exactly the first connection");
			});
		}

		/// <summary>
		/// THE test: every link in the contents points at an id that EXISTS in the document.
		///
		/// A dangling anchor is the likely bug and the invisible one — a contents page that looks
		/// complete and whose links go nowhere. A test asserting only "the index is present" passes
		/// straight through it, so this collects both sets and compares them.
		/// </summary>
		[Test]
		public void EveryContentsLinkResolvesToARealAnchor()
		{
			string html = MultiMethodReport();

			var links = Regex.Matches(html, @"href='#([^']+)'").Select(m => m.Groups[1].Value).ToList();
			var ids = Regex.Matches(html, @"id='([^']+)'").Select(m => m.Groups[1].Value).ToHashSet();

			Assert.Multiple(() =>
			{
				Assert.That(links, Is.Not.Empty, "the contents page links to its chapters");

				var dangling = links.Where(l => !ids.Contains(l)).ToList();
				Assert.That(dangling, Is.Empty,
					"these contents links point at ids that are not in the document: "
					+ string.Join(", ", dangling));
			});
		}

		/// <summary>
		/// The chapters are numbered consecutively from 1, and every connection has one.
		///
		/// Read off the emitted numbers rather than counted from the input: the numbering is what the
		/// contents and the headings have to agree on, and an off-by-one there is a contents page
		/// that points one chapter too far.
		/// </summary>
		[Test]
		public void ChaptersAreNumberedConsecutively()
		{
			string html = Report();

			// The headings' numbers, in document order — the index's are checked by the anchor test.
			var headingNos = Regex.Matches(html,
					@"<h2 class='(?:section-header|connection-header)[^']*'[^>]*>\s*<span class='chapter-no'>(\d+)</span>")
				.Select(m => int.Parse(m.Groups[1].Value))
				.ToList();

			Assert.Multiple(() =>
			{
				// 1 Summary, 2 Connection overview, 3 How the checks are made, 4..6 the connections.
				// Chapter 3 is the method, pulled out of every connection where it was repeated.
				Assert.That(headingNos, Is.EqualTo(new[] { 1, 2, 3, 4, 5, 6 }),
					"chapters run 1..n with no gap and no repeat");
			});
		}

		/// <summary>
		/// The contents page lists every connection with the verdict the tables show.
		///
		/// The verdicts come from CheckWorkflow.Roll — the same function the connection table and the
		/// app's connection list use. Asserting they DIFFER across the three rows is what catches an
		/// index that hardcoded one value or recomputed the rules itself.
		/// </summary>
		[Test]
		public void TheContentsListsEveryConnectionWithItsVerdict()
		{
			string html = MultiMethodReport();

			int at = html.IndexOf("class='index-table'", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the contents table");
			string index = html[at..html.IndexOf("</table>", at, StringComparison.Ordinal)];

			var verdicts = Regex.Matches(index, @"class='ix-verdict [a-z]+'>([^<]+)<")
				.Select(m => m.Groups[1].Value)
				.ToList();

			Assert.Multiple(() =>
			{
				Assert.That(index, Does.Contain("CON1").And.Contain("CON2").And.Contain("CON5"));
				Assert.That(verdicts, Is.EqualTo(new[] { "PASS", "FAIL", "N/A" }),
					"the verdicts come from CheckWorkflow, so they match the tables");
				Assert.That(index, Does.Contain("&mdash;"),
					"and N/A shows an em dash, not 0.0 % — a joint nobody checked has no utilisation");
			});
		}

		/// <summary>
		/// One page per connection — but NOT a break before the first, or the contents page is
		/// followed by a blank one.
		///
		/// Both halves matter: without the rule the connections run together (the defect), and
		/// without the exception there is a wasted page in every report.
		/// </summary>
		[Test]
		public void EachConnectionStartsOnItsOwnPageExceptTheFirst()
		{
			string html = MultiMethodReport();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain(".connection-header { break-before: page; }"),
					"connections start on a new page");
				Assert.That(html, Does.Contain(".connection-header.first-connection { break-before: auto; }"),
					"except the first, which the contents page's break-after already positioned");
				Assert.That(html, Does.Contain(".index-page { break-after: page; }"),
					"and the contents page ends with one");

				// The marker class is on exactly ONE heading, or the exception applies to all of them
				// (or to none) and the rule above is decoration.
				int firsts = Regex.Matches(html, @"class='connection-header first-connection'").Count;
				Assert.That(firsts, Is.EqualTo(1),
					$"{firsts} headings marked as first — must be exactly one");
			});
		}

		/// <summary>
		/// Table 6-1's factor symbols are set at the table's own type size.
		///
		/// They were KaTeX (<c>$ \gamma_{M0} $</c>), and KaTeX sizes itself: measured by glyph
		/// coordinate in the shipped PDF, γ came out at 15.7 pt with a 10 pt subscript while every
		/// value and description in the same table is 13 pt. One cell a fifth larger than its row is
		/// what stops anything lining up across it.
		///
		/// NOT a placement defect — an earlier reading of mine said the symbols were detached from
		/// their rows and printed at the foot of page 1, and that was wrong: they sit on the same
		/// baselines as their values, to within a point. Reading extract_text() order as layout is
		/// what produced that claim, and the user's "it looks readable to me" was correct.
		///
		/// A declared layout is asserted too: left to auto-size, the browser starved the narrow
		/// columns so "not applied" and the header "EC3 Default" each wrapped onto two lines.
		/// </summary>
		[Test]
		public void TableSixOneUsesTheTablesOwnTypeSize()
		{
			string html = Report();

			int at = html.IndexOf("class='settings-table'", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "Table 6-1 is rendered");
			string table = html[at..html.IndexOf("</table>", at, StringComparison.Ordinal)];

			Assert.Multiple(() =>
			{
				Assert.That(table, Does.Not.Contain(@"\gamma"),
					"no KaTeX in the cells — it sizes itself and ignores the table's 13 pt");
				Assert.That(table, Does.Not.Contain("$"),
					"nor any other math delimiter that KaTeX would pick up");
				Assert.That(table, Does.Contain("&gamma;<sub>M0</sub>"),
					"a text-mode symbol, which inherits the row's size");
				Assert.That(html, Does.Contain("table-layout: fixed"),
					"declared widths are honoured, so no column starves");
				Assert.That(table, Does.Contain("<colgroup>"), "and the widths are declared");
			});
		}

		/// <summary>
		/// The contents comes BEFORE the table it indexes, and before the summary.
		///
		/// It used to sit after both, so a reader met the connection overview first and the index to
		/// it second. Nothing guarded the order: the blocks were moved and all 355 tests stayed
		/// green, which is why this test exists rather than the position being taken on trust.
		///
		/// Asserted by POSITION in the emitted document, the only thing that decides what a reader
		/// meets first.
		/// </summary>
		[Test]
		public void TheContentsPrecedesWhatItIndexes()
		{
			string html = MultiMethodReport();

			int contents = html.IndexOf("class='index-page'", StringComparison.Ordinal);
			int summary = html.IndexOf("id='ch-1'", StringComparison.Ordinal);
			int overview = html.IndexOf("id='ch-2'", StringComparison.Ordinal);

			Assert.Multiple(() =>
			{
				Assert.That(contents, Is.GreaterThan(-1), "the contents page is rendered");
				Assert.That(summary, Is.GreaterThan(contents),
					"the summary follows the contents that lists it");
				Assert.That(overview, Is.GreaterThan(summary),
					"and the connection overview follows the summary");
			});
		}

		/// <summary>
		/// Nothing that reads as one unit may be split across a page.
		///
		/// Measured in the shipped 161-page PDF by glyph coordinate: the summary card's verdict
		/// ("INCOMPLETE", y=748 on page 1) and its headline figure ("73.7 %", 28 pt, y=1004 on
		/// page 2) landed on different pages. The card had `break-after: avoid`, which says where a
		/// page may end AFTER it and nothing about breaking inside it — the two are different
		/// properties and only the second was needed.
		///
		/// The contents page had the same defect for the same reason: `break-after: page` starts a
		/// new page after it without stopping it splitting, so it broke across pages 2 and 3.
		/// </summary>
		[TestCase(".summary-card", TestName = "the summary card, whose figure was orphaned")]
		[TestCase(".index-page", TestName = "the contents, which broke across two pages")]
		[TestCase(".settings-card", TestName = "Table 6-1")]
		[TestCase("table", TestName = "and every table")]
		public void AUnitThatReadsAsOneBlockIsNotSplitAcrossPages(string selector)
		{
			string html = Report();

			int at = html.IndexOf("@media print", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the print block");
			// OUR print block only: the embedded KaTeX stylesheet carries an @media print of its own,
			// and searching the whole document finds that one too.
			string print = html[at..];

			// Comments stripped FIRST, from the whole block: the explanatory /* … */ sits BEFORE the
			// rule, so removing them after splitting left the prose attached to the first selector.
			// (Two failed attempts here, both on correct CSS — the selector list also spans several
			// lines, so the pattern has to cross newlines.)
			string css = System.Text.RegularExpressions.Regex.Replace(
				print, @"/\*.*?\*/", "", System.Text.RegularExpressions.RegexOptions.Singleline);

			var guarded = System.Text.RegularExpressions.Regex
				.Matches(css, @"([^{}]+)\{[^}]*break-inside:\s*avoid[^}]*\}",
					System.Text.RegularExpressions.RegexOptions.Singleline)
				.SelectMany(m => m.Groups[1].Value.Split(','))
				.Select(s => s.Trim())
				.Where(s => s.Length > 0)
				.ToList();

			Assert.That(guarded, Does.Contain(selector),
				$"'{selector}' must carry break-inside: avoid in the print block; guarded: "
				+ string.Join(" | ", guarded));
		}

		/// <summary>
		/// A derivation heading must not be the last thing on a page.
		///
		/// The round-2 review measured 17; re-measuring the same PDF found **25**, all of them
		/// `.deriv-h`: 11 × 'Utilisation — eq (6.57)', 8 × 'Weighted axial resistance', 6 ×
		/// 'Members — geometry at the joint'. The class was in no break rule at all, and
		/// `p { orphans: 3 }` cannot substitute — an orphaned heading is a whole paragraph, not the
		/// last line of one, so the orphans property has nothing to hold back.
		/// </summary>
		[Test]
		public void ADerivationHeadingIsNotLeftAtTheFootOfAPage()
		{
			string print = PrintBlock(Report());

			var guarded = SelectorsCarrying(print, "break-after:\\s*avoid");

			Assert.That(guarded, Does.Contain(".deriv-h"),
				"the derivation headings must keep their following block; guarded: "
				+ string.Join(" | ", guarded));
		}

		/// <summary>
		/// The check card FLOWS across a page break — it is a container, not a unit.
		///
		/// This is a property of the output, not a note-to-self: with `break-inside: avoid` on the
		/// whole card, a card that does not fit the rest of a page moves entirely to the next one.
		/// Measured on the shipped 173-page PDF: 11 pages filled to 22.7 % of their height and 41
		/// under 65 %, against a median of 80 %. The atomic blocks inside it are what must not split,
		/// and those are asserted by AUnitThatReadsAsOneBlockIsNotSplitAcrossPages above.
		/// </summary>
		[Test]
		public void TheCheckCardIsAllowedToFlowAcrossPages()
		{
			string print = PrintBlock(Report());

			var guarded = SelectorsCarrying(print, "break-inside:\\s*avoid");

			Assert.Multiple(() =>
			{
				Assert.That(guarded, Does.Not.Contain(".check-card"),
					"a whole card kept together wastes up to three quarters of a sheet; guarded: "
					+ string.Join(" | ", guarded));
				// And the control: the atomic blocks ARE protected, so this is a re-scoping rather
				// than the break control having been dropped.
				Assert.That(guarded, Does.Contain(".formula-block"), "the formula block still is");
				Assert.That(guarded, Does.Contain("table"), "and so is every table");
			});
		}

		/// <summary>
		/// No footer is printed at all — see PageSetupTests.TheReportPrintsNoPageNumbers for the
		/// measurements. An OFFSET page number cannot be rendered in a page margin box on this
		/// engine, so "start at 77" printed 77 on all 187 pages; the setting is gone rather than
		/// left printing a constant, and the reader numbers the document this is bound into.
		/// </summary>
		[Test]
		public void TheFooterIsEmpty()
		{
			// The @page rule comes from FooterCss and is passed in per export; the static stylesheet
			// no longer declares a margin box at all, which is why this reads both.
			string css = NorsokHtmlReportGenerator.FooterCss(new PageSetup());
			string html = NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
				{
					("CON1", new List<NorsokFormulaResult> { Assessed("M1", 0.476, true) }),
				},
				expandAll: false, jointImages: null, topologies: null, footerCss: css);

			Assert.Multiple(() =>
			{
				Assert.That(css, Does.Contain("content: none"),
					"the margin box is emptied — no numbering that cannot honour an offset");
				Assert.That(css, Does.Not.Contain("counter(page)"), "and no page counter");
				// And the document really carries that rule rather than a stale one of its own.
				Assert.That(html, Does.Contain("content: none"), "the export applies it");
				Assert.That(
					System.Text.RegularExpressions.Regex.Matches(html, @"counter\(page\)"),
					Is.Empty, "nothing anywhere prints a page number");
			});
		}

		/// <summary>
		/// A CONTAINER is never protected from splitting — only the atomic blocks inside it.
		///
		/// Learned twice in one round, the second time from the real export. `.check-card` was the
		/// first: a card that would not fit moved whole, wasting up to three quarters of a page.
		/// `.deriv-block` was the second, and it was added BY the fix for the first — it holds the
		/// entire joint-plane section, several tables and half a page of prose, so the joint figure
		/// could no longer share a page with it. Measured on the export: six pages at 8 % fill, and
		/// 173 pages became 187 — the fix for wasted space made it worse.
		///
		/// So this test names the rule rather than the two instances: a selector that denotes a
		/// container must not carry break-inside: avoid.
		/// </summary>
		[Test]
		public void NoContainerIsProtectedFromSplitting()
		{
			var guarded = SelectorsCarrying(PrintBlock(Report()), "break-inside:\\s*avoid");

			var containers = new[] { ".check-card", ".deriv-block", "details", "body", ".chapter-group" };
			var offenders = containers.Where(c => guarded.Contains(c)).ToList();

			Assert.Multiple(() =>
			{
				Assert.That(offenders, Is.Empty,
					"these are containers, not units: " + string.Join(", ", offenders));
				// The control: the atomic blocks ARE protected, so this is a scoping rule and not a
				// test that would pass on a stylesheet with no break control at all.
				Assert.That(guarded, Does.Contain(".formula-block"));
				Assert.That(guarded, Does.Contain(".deriv-step"));
				Assert.That(guarded, Does.Contain("table"));
			});
		}

		/// <summary>OUR print block, without the embedded KaTeX stylesheet's own @media print.</summary>
		private static string PrintBlock(string html)
		{
			int at = html.IndexOf("@media print", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the print block");
			return html[at..];
		}

		/// <summary>
		/// Every selector carrying the given declaration, comments stripped first — the explanatory
		/// /* … */ sits BEFORE its rule, so stripping after the split leaves prose attached to the
		/// first selector. The selector list also spans lines, so the pattern crosses newlines.
		/// </summary>
		private static List<string> SelectorsCarrying(string print, string declaration)
		{
			string css = System.Text.RegularExpressions.Regex.Replace(
				print, @"/\*.*?\*/", "", System.Text.RegularExpressions.RegexOptions.Singleline);

			return System.Text.RegularExpressions.Regex
				.Matches(css, @"([^{}]+)\{[^}]*" + declaration + @"[^}]*\}",
					System.Text.RegularExpressions.RegexOptions.Singleline)
				.SelectMany(m => m.Groups[1].Value.Split(','))
				.Select(s => s.Trim())
				.Where(s => s.Length > 0)
				.ToList();
		}

		/// <summary>
		/// The disclosure triangle does not print.
		///
		/// A click affordance on paper is noise, and the rule meant to hide it did not work: it read
		/// `details > summary::before` while the marker is declared on `.check-card > summary::before`,
		/// so the more specific selector won. Measured: 41 × '▸' in the exported PDF.
		/// </summary>
		[Test]
		public void ThePrintStylesheetHidesTheDisclosureMarker()
		{
			string html = Report();

			int at = html.IndexOf("@media print", StringComparison.Ordinal);
			string print = html[at..];

			Assert.Multiple(() =>
			{
				Assert.That(print, Does.Contain(".check-card > summary::before"),
					"the selector that actually draws the marker is the one that must be overridden");
				Assert.That(print, Does.Contain("content: none"),
					"and the generated content is removed, not merely hidden");
			});
		}

		/// <summary>
		/// The page is declared, and in ONE place.
		///
		/// There used to be two separate @media print blocks — one inline with the style tag, one in
		/// CssStyles — which is how the print rules came to be half-specified: a rule added to one
		/// was invisible in the other.
		/// </summary>
		[Test]
		public void ThePageIsDeclaredOnceWithASize()
		{
			string html = Report();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("@page"), "the page has a declared size");
				Assert.That(html, Does.Contain("size: A4 portrait"),
					"A4 — WebView2's own default would be Letter");

				// OUR print block, counted by a rule only ours carries. The embedded KaTeX stylesheet
				// has an @media print of its own, so counting them across the document finds two and
				// says nothing — measured: the first version of this assertion failed for exactly
				// that reason, on correct code.
				Assert.That(Regex.Matches(html, @"\.connection-header \{ break-before: page; \}").Count,
					Is.EqualTo(1),
					"ONE print block of ours: two competing ones is how these rules got lost before");
			});
		}

		/// <summary>
		/// The header states where the model came from and who did the checking, without describing
		/// the transport.
		///
		/// The user asked what this line was for. It replaced a false "Engine: … CBFEM Analysis via
		/// REST API" when CBFEM went, and its replacement still named the REST API — which tells the
		/// reader of a compliance report nothing. What it has to say is which quantities are the
		/// model's and which are this app's.
		/// </summary>
		[Test]
		public void TheHeaderStatesProvenanceNotTransport()
		{
			string html = Report();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("Model source:"), "where the inputs came from");
				Assert.That(html, Does.Contain("Checks by:"), "and who evaluated them");
				Assert.That(html, Does.Not.Contain("via its REST API"),
					"the transport is not a fact about the design");
				Assert.That(html, Does.Not.Contain("CBFEM"),
					"no calculation is run — CBFEM was mothballed");
			});
		}
	}
}
