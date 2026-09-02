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
		/// THE test: every link in the contents points at an id that EXISTS in the document.
		///
		/// A dangling anchor is the likely bug and the invisible one — a contents page that looks
		/// complete and whose links go nowhere. A test asserting only "the index is present" passes
		/// straight through it, so this collects both sets and compares them.
		/// </summary>
		[Test]
		public void EveryContentsLinkResolvesToARealAnchor()
		{
			string html = Report();

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
				// 1 Summary, 2 Connection overview, 3..5 the three connections
				Assert.That(headingNos, Is.EqualTo(new[] { 1, 2, 3, 4, 5 }),
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
			string html = Report();

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
			string html = Report();

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
