using System.IO;
using System.Text;
using NorsokChecker.Models;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Generates an HTML report with KaTeX-rendered formulas for Norsok check results.
	/// Produces output that looks like the formulas in the NORSOK N-004 PDF and
	/// the IDEA StatiCa CHECK tab formula explanations.
	/// </summary>
	public static class NorsokHtmlReportGenerator
	{
		/// <summary>
		/// The document's name, in ONE place — it is emitted twice (the HTML title, which becomes the
		/// PDF /Title, and the printed header on page 1) and the two used to be separate literals.
		/// </summary>
		internal const string ReportTitle = "NORSOK N-004 — Tubular joint check";

		/// <summary>
		/// The footer's default label. Deliberately without "§6.4": a title freed of the chapter
		/// should not have it reasserted on all 173 pages, and of 469 mentions of "6.4" in the
		/// reviewed sample, 173 were this footer alone — one per page, the single largest source.
		/// The substantive references remain (15 chapter headings, 128 clause references, the norm
		/// box). Overridable per export via PageSetup.
		/// </summary>
		internal const string DefaultFooterLabel = "NORSOK N-004";

		/// <summary>
		/// KaTeX formula mappings: section → (symbolic formula in LaTeX, check expression in LaTeX)
		/// </summary>
		private static readonly Dictionary<string, (string latex, string check)> FormulaLatex = new()
		{
			["5"] = (
				@"\text{DC} \rightarrow \text{Steel Quality Level} \rightarrow \text{Inspection Category}",
				@"\text{Table 5-1} \rightarrow \text{Table 5-2} \rightarrow \text{Table 5-3/5-4}"
			),
			["6.3.2"] = (
				@"N_{t,Rd} = \frac{A \cdot f_y}{\gamma_M}",
				@"N_{Sd} \leq N_{t,Rd}"
			),
			["6.3.3"] = (
				@"N_{c,Rd} = \frac{A \cdot f_c}{\gamma_M}",
				@"N_{Sd} \leq N_{c,Rd}"
			),
			["6.3.4"] = (
				@"M_{Rd} = \frac{f_m \cdot W}{\gamma_M}",
				@"M_{Sd} \leq M_{Rd}"
			),
			["6.3.5"] = (
				@"V_{Rd} = \frac{A \cdot f_y}{2\sqrt{3} \cdot \gamma_M}",
				@"V_{Sd} \leq V_{Rd}"
			),
			["6.3.8.1"] = (
				@"\left(\frac{N_{Sd}}{N_{t,Rd}}\right)^{1.75} + \frac{\sqrt{M_{y,Sd}^2 + M_{z,Sd}^2}}{M_{Rd}} \leq 1.0",
				@"\text{Interaction} \leq 1.0"
			),
			["6.3.8.2"] = (
				@"\frac{N_{Sd}}{N_{c,Rd}} + \frac{1}{M_{Rd}}\sqrt{\left(\frac{C_{my} \cdot M_{y,Sd}}{1-\frac{N_{Sd}}{N_{Ey}}}\right)^2 + \left(\frac{C_{mz} \cdot M_{z,Sd}}{1-\frac{N_{Sd}}{N_{Ez}}}\right)^2} \leq 1.0",
				@"\text{Interaction} \leq 1.0"
			),
			["6.3.8.3"] = (
				@"\frac{M_{Sd}}{M_{Rd}} \leq 1.4 - \frac{V_{Sd}}{V_{Rd}}",
				@"\frac{M_{Sd}}{M_{Rd}} \leq 1.4 - \frac{V_{Sd}}{V_{Rd}}"
			),
			// All THREE resistances the check condition names, aligned on their equals signs — the
			// three share one base and differ only in the extra d and in which Q_u, which is the
			// thing worth seeing at a glance. Printing N_Rd alone left two of the condition's three
			// terms with no formula anywhere near them.
			//
			// f_{y,chord}, not a bare f_y: eq (6.52)/(6.53) key on the CHORD yield
			// (Norsok64Engine.cs:117), the inputs table shows two f_y values, and with both steels
			// S355 nothing on the page said which one was substituted.
			["6.4.3.6"] = (
				@"\begin{aligned}
					N_{Rd} &= \frac{f_{y,chord} \cdot T^2}{\gamma_M \cdot \sin\theta} \cdot Q_u \cdot Q_f \\[2pt]
					M_{y,Rd} &= \frac{f_{y,chord} \cdot T^2 \cdot d}{\gamma_M \cdot \sin\theta} \cdot Q_{u,ipb} \cdot Q_{f,mom} \\[2pt]
					M_{z,Rd} &= \frac{f_{y,chord} \cdot T^2 \cdot d}{\gamma_M \cdot \sin\theta} \cdot Q_{u,opb} \cdot Q_{f,mom}
				  \end{aligned}",
				// The BARS on the out-of-plane term are not decoration: the derivation evaluates
				// |M_z,Sd/M_z,Rd| (see the eq 6.57 Step below), and without them here the header
				// stated a DIFFERENT check from the one performed — a reviewer reads that as a
				// calculation error, not a typo, because a negative M_z would reduce the sum.
				@"\frac{N_{Sd}}{N_{Rd}} + \left(\frac{M_{y,Sd}}{M_{y,Rd}}\right)^2 + \left|\frac{M_{z,Sd}}{M_{z,Rd}}\right| \leq 1.0"
			),
			["Bolt"] = (
				@"\frac{F_{t,Sd}}{F_{t,Rd}} + \frac{F_{v,Sd}}{1.4 \cdot F_{v,Rd}} \leq 1.0",
				@"\text{Interaction}_{tension+shear} \leq 1.0"
			),
			["Weld"] = (
				@"f_{w,Rd} = \frac{f_u}{\beta_w \cdot \gamma_{M2}}",
				@"\sigma_w \leq f_{w,Rd}"
			),
		};

		/// <param name="expandAll">Render every check card expanded — used for the
		/// PDF export so the customer sees all formulas, not only the failed ones.</param>
		/// <param name="jointImages">
		/// Optional joint figure per connection name, as a base64 PNG.
		///
		/// Passed in rather than rendered here: drawing it needs the WPF control and the joint
		/// topology, both of which live in the window, and a report generator that reached for either
		/// could no longer be called from a test without a UI thread. Base64 rather than a file path
		/// because WebView2's NavigateToString has no base URL to resolve one against.
		/// </param>
		/// <param name="topologies">
		/// The resolved joint topology per connection name, for the "Joint plane and force
		/// transformation" section.
		///
		/// Optional, and the section is simply absent without it — a test that only wants the check
		/// cards should not have to build a topology. Passed in for the same reason the figures are:
		/// deriving one needs the API and the members, and a generator that reached for either could
		/// no longer be called without a service.
		/// </param>
		public static string GenerateReport(
			string projectName,
			IReadOnlyList<(string connectionName, List<NorsokFormulaResult> formulas)> allResults,
			bool expandAll = false,
			IReadOnlyDictionary<string, string>? jointImages = null,
			IReadOnlyDictionary<string, Norsok64.JointTopology>? topologies = null,
			string? footerCss = null,
			(int Active, int Total)? loadEffectCounts = null)
		{
			var sb = new StringBuilder();

			sb.AppendLine("<!DOCTYPE html>");
			sb.AppendLine("<html><head>");
			sb.AppendLine("<meta charset='utf-8'/>");
			// NOT "Compliance Report": that claims conformity for a document in which connections
			// routinely go unassessed (9 of 15 in the reviewed sample), and the <title> becomes the
			// PDF's /Title — what a reader sees in Explorer, a browser tab and an archive.
			//
			// Nor "§6.4 — Tubular joint check", which the review proposed: it pins the title to one
			// chapter and will be wrong again when §6.3 and CIDECT land. The chapter scope is already
			// in the document, in the norm box and on every page's footer.
			sb.AppendLine($"<title>{ReportTitle}</title>");

			AppendKatex(sb);

			sb.AppendLine("<style>");
			sb.AppendLine(CssStyles);
			sb.AppendLine("</style>");
			// The footer, from the export's own settings — AFTER CssStyles so it overrides the
			// default @page rule there. It cannot live in CssStyles itself: that is a static string
			// and the label, the mode and the starting number are per-export.
			//
			// Passed as CSS rather than as a PageSetup, so the generator does not need to know what
			// a page setup is (and PageSetup can stay internal). MainWindow builds it.
			if (!string.IsNullOrEmpty(footerCss))
			{
				sb.AppendLine("<style>");
				sb.AppendLine(footerCss);
				sb.AppendLine("</style>");
			}
			sb.AppendLine("<style>");
			sb.AppendLine(".report-footer { margin-top: 28px; padding: 12px 16px; border-top: 2px solid #F36E21; color: #546E7A; font-size: 11px; line-height: 1.6; }");
			// print-color-adjust asks the RENDERER not to drop background colours. Necessary but not
			// sufficient: WebView2's PrintToPdf has its own ShouldPrintBackgrounds, false by default,
			// which overrides this — the report was exported colourless for that reason alone, with
			// this rule already in place. Both have to say yes; see Models.PageSetup.
			sb.AppendLine("* { -webkit-print-color-adjust: exact; print-color-adjust: exact; }");
			// The print rules live in ONE place, in CssStyles. A second @media print block used to sit
			// here and quietly compete with it.
			sb.AppendLine("</style>");
			sb.AppendLine("</head><body>");

			// Report header — IDEA StatiCa primary, Norsok as feature
			sb.AppendLine("<div class='report-header'>");
			sb.AppendLine("  <div class='brand-line'>");
			sb.AppendLine("    <span class='idea-brand'><span class='idea-orange'>IDEA</span> StatiCa</span>");
			sb.AppendLine("    <span class='brand-sep'>|</span>");
			sb.AppendLine($"    <span class='norsok-badge'>{ReportTitle}</span>");
			sb.AppendLine("  </div>");
			sb.AppendLine($"  <p class='subtitle'>Project: {Esc(projectName)} &mdash; Generated: {DateTime.Now:yyyy-MM-dd HH:mm}</p>");
			sb.AppendLine("</div>");

			// Norm reference box
			sb.AppendLine("<div class='norm-box'>");
			sb.AppendLine("  <strong>Design Code:</strong> NORSOK N-004, Rev. 3, February 2013 &mdash; Design of Steel Structures<br/>");
			sb.AppendLine("  <strong>Chapter 6.4:</strong> Tubular Joints &mdash; Simple Joints (&sect;6.4.3)<br/>");
			// What the app actually does. This used to read "Engine: IDEA StatiCa Connection CBFEM
			// Analysis via REST API", which described the mothballed CBFEM chapter: since that went,
			// no calculation is run at all — the model and its load effects are read over the API and
			// §6.4 is evaluated here.
			// PROVENANCE, not mechanism. This line replaced a false "Engine: … CBFEM Analysis via
			// REST API" when the CBFEM chapter went, and its first wording ("read from … via its
			// REST API") named the transport — which tells the reader of a compliance report
			// nothing. What matters is WHICH quantities came from the model and which this app
			// derived. The service version would belong here too, but ServiceLocator does not retain
			// it (it is fetched to pick an install and dropped), so it is not promised.
			// Checks by, THEN model source: who did the work before where the inputs came from.
			// And no "evaluated from that model — no analysis is run": it restated the line below it
			// (the model IS the source, so "from that model" adds nothing) and the "no analysis"
			// half described the app's internals rather than the document's provenance.
			sb.AppendLine("  <strong>Checks by:</strong> NorsokChecker<br/>");
			sb.AppendLine("  <strong>Model source:</strong> IDEA StatiCa Connection "
				+ "&mdash; geometry, cross-sections, materials and load effects");
			// WHAT WAS SEARCHED. The report stated which load effect governs each brace and nothing
			// else, so a reader could not tell a search over fifteen states from a search over three
			// — and had no way to know that nothing was skipped.
			//
			// COUNTS ONLY, never a list of identifiers: a model may hold arbitrarily many load
			// effects, so an enumeration is a paragraph that grows without bound. The names stay on
			// the per-brace rows, where there is one of them however many states exist.
			if (loadEffectCounts is { Total: > 0 } lec)
			{
				sb.AppendLine("<br/>  <strong>Load effects:</strong> "
					+ $"{lec.Total} defined in the model, <b>{lec.Active} active</b> and evaluated "
					+ "&mdash; every active state was checked on every brace of every assessed "
					+ "connection"
					+ (lec.Active < lec.Total
						? $"; the remaining {lec.Total - lec.Active} are switched off in the model "
							+ "and were not evaluated, which is a limitation of this run"
						: ""));
			}
			sb.AppendLine("</div>");

			// Table 6-1 Material factors
			sb.AppendLine("<div class='settings-card'>");
			sb.AppendLine("  <h3 class='settings-title'>Table 6-1 &mdash; Material Factors Applied to Project</h3>");
			// FIXED layout with declared widths. Left to auto-size, the browser gave the description
			// column everything and squeezed the rest, so "not applied" and the header "EC3 Default"
			// each wrapped onto two lines while the row heights ran 1, 1, 2, 1, 3 — the table read as
			// damaged rather than dense.
			sb.AppendLine("  <table class='settings-table'>");
			sb.AppendLine("    <colgroup><col style='width:5.5em'/><col style='width:4.5em'/>"
				+ "<col style='width:5em'/><col/></colgroup>");
			sb.AppendLine("    <thead><tr><th>Factor</th><th>Value</th><th>EC3</th><th>Application</th></tr></thead>");
			sb.AppendLine("    <tbody>");
			// TEXT-MODE symbols, not KaTeX, for SIZE — not for placement.
			//
			// Measured on the shipped PDF (glyph coordinates, page 1): the KaTeX γ sits at x=32 on
			// y=327/365/412/459 and its row's value at x=105 on y=328/366/413/459 — the same line to
			// within a point. The symbols are NOT detached from their rows; a linear extract_text()
			// reading them "at the foot of the page" is an artefact of KaTeX drawing in a separate
			// batch, and reading that as layout was wrong.
			//
			// What the measurement does show: γ renders at 15.7 pt and its subscript at 10 pt while
			// every value and description in the table is 13 pt. One cell 20 % larger than its row is
			// what makes nothing line up optically. Text-mode γ<sub> inherits the table's size.
			// KaTeX stays for display formulas, where its own block layout is what is wanted.
			// THE ROWS ARE THE STANDARD'S OWN, verified against N-004 Rev. 3 page 14.
			//
			// Table 6-1 lists γM0, γM1 and γM2 and NOTHING else. A γM3 = 1.30 "Slip-resistant
			// connections" row was printed here under the standard's table caption; "slip" does not
			// occur anywhere in N-004 and neither does M3, so the document attributed to the norm a
			// factor the norm does not contain. That is worse than a typo in a report which states,
			// three lines below, that it WRITES these factors into the shared project.
			//
			// The standard's own six lines collapse onto three factors; its bolted-connections line
			// under γM2 was missing and is restored.
			foreach (var (sub, val, ec3, use) in new[]
			{
				("M0", "1.15", "1.00", "Resistance of Class 1, 2 or 3 cross-sections"),
				("M1", "1.15", "1.00", "Resistance of Class 4 cross-sections; member buckling"),
				("M2", "1.30", "1.25", "Net section at bolt holes; fillet &amp; partial penetration "
					+ "welds; bolted connections"),
			})
			{
				sb.AppendLine($"      <tr><td class='fac'>&gamma;<sub>{sub}</sub></td>"
					+ $"<td class='val-norsok'>{val}</td><td class='val-ec3'>{ec3}</td>"
					+ $"<td>{use}</td></tr>");
			}
			// γBC: "not applied" and nothing else. The three-line argument for WHY used to live in
			// this cell — italics and all — which tripled the row height and put a paragraph where a
			// datum belongs. It is a sentence under the table now.
			sb.AppendLine("      <tr class='row-note'><td class='fac'>&gamma;<sub>BC</sub></td>"
				+ "<td class='val-ec3'>&mdash;</td><td class='val-ec3'>&mdash;</td>"
				+ "<td>Additional building code factor (&sect;6.1) &mdash; not applied</td></tr>");
			sb.AppendLine("    </tbody>");
			sb.AppendLine("  </table>");
			// Out of the cell. §6.1 asks for γBC only "where OTHER material factors are used than
			// given in Table 6-1", and the rows above ARE Table 6-1, so applying it on top would
			// double-count (γM0 = 1.15 × 1.05 = 1.21 against the 1.15 the norm asks for).
			sb.AppendLine("  <p class='settings-note'>&gamma;<sub>BC</sub> is required only where "
				+ "factors <em>other</em> than Table 6-1 are used; the factors above <em>are</em> "
				+ "Table 6-1, so it does not apply on top of them.</p>");
			// TWO statements, and they used to be one grey italic paragraph. The first is the
			// STANDARD speaking; the second is this tool disclosing that it MODIFIES the input model.
			// Running them together made the disclosure read as part of the quotation — the reviewer
			// asked for them split and for the second to be prominent, and both requests are right:
			// nothing else in the document tells a reader that opening their project afterwards will
			// find different material factors in it.
			// VERBATIM. Inside quotation marks the words are the standard's, not ours: this read
			// "γM0 ... for ULS" where N-004 page 13 has "γM ... for ULSs". Silently tightening a
			// quotation is how a reader ends up unable to find the sentence in their own copy — and
			// γM vs γM0 is not cosmetic here, since Table 6-1 assigns different values to γM1/γM2.
			sb.AppendLine("  <p class='settings-note settings-quote'>&sect;6.1: &ldquo;The material "
				+ "factor &gamma;<sub>M</sub> is 1.15 for ULSs unless noted otherwise. The material "
				+ "factors according to Table 6-1 shall be used if NS-EN 1993-1-1 and "
				+ "NS-EN 1993-1-8 are used for calculation of structural resistance.&rdquo;</p>");
			sb.AppendLine("  <p class='settings-disclosure'><strong>This tool writes these factors "
				+ "into the project.</strong> They are stored in the IDEA StatiCa Connection "
				+ "project's own settings, so any calculation run there afterwards &mdash; by this "
				+ "app or by anyone opening the file &mdash; uses them instead of whatever was set "
				+ "before. The model is modified, not only read.</p>");
			sb.AppendLine("</div>");

			// ── Contents, then the summary, then the table the contents indexes ──
			//
			// The order matters and was wrong: the contents used to sit AFTER the connection overview,
			// so a reader met the table first and the index to it second. Worse, in the shipped PDF it
			// broke across pages 2 and 3 — its break-after started a page but nothing stopped it
			// splitting, and the front matter above it had already filled page 1.
			//
			// It carries no chapter number: it is the navigation apparatus, not a chapter. That also
			// leaves the numbering alone — Summary stays 1, the overview 2, connections 3+.
			//
			// And it is printed only when there is a hierarchy to map. With one method per connection
			// it is a list of connection names duplicating the overview's first column on the next
			// page — see ShouldRenderContents.
			bool contents = ShouldRenderContents(allResults);
			if (contents)
				RenderIndex(sb, allResults);

			// ── Executive Summary Card, then chapter 2 ──
			RenderSummaryCard(sb, allResults);
			RenderConnectionTable(sb, allResults);

			// Chapter 3: the method, once. It used to be repeated inside every assessed connection —
			// six connections carrying six identical paragraphs each.
			RenderMethodChapter(sb);

			int chapter = ConnectionChapterBase;
			foreach (var (connectionName, formulas) in allResults)
			{
				// The id the index links to, and the number it announces. The first connection is
				// excepted from the page break ONLY when the contents was printed: its break-after
				// has already started a page, and a second break would leave a blank one between
				// them. With no contents there is nothing to have started that page, so the first
				// connection needs its own break like every other — without this, it would run on
				// from the overview table and the "one page per connection" rule would hold for
				// every connection except the first.
				string anchor = AnchorFor(chapter);
				string firstClass = contents && chapter == ConnectionChapterBase
					? " first-connection" : "";
				sb.AppendLine($"<h2 class='connection-header{firstClass}' id='{anchor}'>"
					+ $"<span class='chapter-no'>{chapter}</span> {Esc(connectionName)}</h2>");
				chapter++;

				// The joint, seen along its own plane normal — the same figure the §6.4 tab shows, so
				// the reader is looking at one picture of the joint rather than two projections of it.
				// Only where one was rendered: a joint the chapter rejected has no envelope to colour
				// by, and an uncoloured figure beside a "not assessed" card would suggest otherwise.
				if (jointImages != null && jointImages.TryGetValue(connectionName, out var png)
					&& !string.IsNullOrEmpty(png))
				{
					sb.AppendLine("<figure class='joint-figure'>");
					sb.AppendLine($"  <img src='data:image/png;base64,{png}' alt='Joint {Esc(connectionName)}'/>");
					sb.AppendLine("  <figcaption>Joint plane, viewed along its normal &mdash; "
						+ "members coloured by their governing utilisation.</figcaption>");
					sb.AppendLine("</figure>");
					// AFTER the figure element, not inside it: .joint-figure carries
					// break-inside: avoid, and adding the legend to that block made the figure too
					// tall to share a page with the geometry table — six pages at 8 % fill, and the
					// document grew from 173 pages to 187. See the CSS comment on .joint-figure.
					RenderUtilisationLegend(sb);
				}

				// The joint-plane section is NOT rendered here — it belongs INSIDE the §6.4 group,
				// and only where §6.4 actually ran. See the chapter loop below.

				// Group by chapter, from the registry rather than a list kept here.
				//
				// This used to be a hardcoded four-entry array, which meant a new chapter's rows
				// landed in "Other Checks" below with nothing to say they had been mis-filed — the
				// report looked complete and quietly grouped the new work as leftovers.
				var groups = Chapters.ChapterRegistry.All
					.Select(c => (key: c.Key, title: c.ReportGroup))
					.ToArray();

				var assigned = new HashSet<NorsokFormulaResult>();

				foreach (var (key, title) in groups)
				{
					var groupFormulas = formulas
						.Where(f => !assigned.Contains(f) && f.Section.StartsWith(key))
						.ToList();
					if (groupFormulas.Count == 0) continue;

					foreach (var f in groupFormulas) assigned.Add(f);

					// A joint outside the chapter's scope becomes ONE card listing every unmet
					// condition, not one card each. CON6 produced seven, all headed "Outside the
					// scope of §6.4 (n of 7)" and each opening onto the same sentence — seven cards
					// that say the chapter does not apply, where one says it better and names the
					// seven reasons. The Results table was fixed the same way.
					var rejections = groupFormulas.Where(f => f.NotAssessed && !f.IsNote).ToList();
					var rest = rejections.Count > 1
						? groupFormulas.Except(rejections).ToList()
						: groupFormulas;

					sb.AppendLine($"<div class='chapter-group'>");
					sb.AppendLine($"  <h3 class='chapter-header'>{Esc(title)} <span class='chapter-count'>{groupFormulas.Count}</span></h3>");

					// The joint plane, INSIDE this chapter's group — it is strictly §6.4's, not a
					// general property of the connection, and it used to render before the groups
					// where it read as the latter.
					//
					// For a REJECTED joint too, but showing different things: see RenderJointPlane.
					// The first fix here suppressed it entirely when nothing was assessed, which
					// removed one contradiction (transformed forces above a card saying no force
					// could be resolved) and created another — every assessed joint showed its
					// workings while a rejected one gave only a verdict, though its conditions quote
					// measured numbers. So the block stays and drops the forces instead.
					if (key == "6.4"
						&& topologies != null && topologies.TryGetValue(connectionName, out var topo))
						RenderJointPlane(sb, topo, groupFormulas);

					if (rejections.Count > 1)
						RenderRejectionCard(sb, rejections, key, expandAll);

					foreach (var fr in rest)
						RenderFormulaCard(sb, fr, expandAll);
					sb.AppendLine($"</div>");
				}

				// Any uncategorized
				var uncategorized = formulas.Where(f => !assigned.Contains(f)).ToList();
				if (uncategorized.Count > 0)
				{
					sb.AppendLine($"<div class='chapter-group'>");
					sb.AppendLine($"  <h3 class='chapter-header'>Other Checks <span class='chapter-count'>{uncategorized.Count}</span></h3>");
					foreach (var fr in uncategorized)
						RenderFormulaCard(sb, fr, expandAll);
					sb.AppendLine($"</div>");
				}
			}

			// Script: auto-expand FAIL cards, re-render KaTeX when toggled
			sb.AppendLine("<script>");
			sb.AppendLine("document.addEventListener('DOMContentLoaded', function() {");
			sb.AppendLine("  document.querySelectorAll('details.fail').forEach(d => d.open = true);");
			sb.AppendLine("  document.querySelectorAll('details').forEach(d => {");
			sb.AppendLine("    d.addEventListener('toggle', function() {");
			sb.AppendLine("      if (d.open && typeof renderMathInElement === 'function') renderMathInElement(d, {delimiters: [{left:'$$',right:'$$',display:true},{left:'$',right:'$',display:false}]});");
			sb.AppendLine("    });");
			sb.AppendLine("  });");
			sb.AppendLine("});");
			sb.AppendLine("</script>");

			// Attribution footer — rendered in-app and in the exported PDF
			sb.AppendLine("<div class='report-footer'>");
			sb.AppendLine("  Generated by <strong>NorsokChecker</strong>, powered by <strong><span class='idea-orange'>IDEA</span> StatiCa</strong>.<br/>");
			// The connection model — geometry, cross-sections, materials and load effects — comes from
			// IDEA StatiCa Connection over its REST API; every NORSOK check in this report is computed
			// here from that model.
			//
			// It used to say the results were "computed by the IDEA StatiCa Connection CBFEM engine"
			// and that NORSOK was evaluated "on top of these results". That described the CBFEM
			// chapter, which is mothballed: no calculation is run any more, and §6.4 needs none —
			// it works from the geometry and the load effects alone.
			sb.AppendLine("  The connection model &mdash; geometry, cross-sections, materials and load effects &mdash;");
			sb.AppendLine("  is read from <strong>IDEA StatiCa Connection</strong> through its REST API.");
			sb.AppendLine("  The NORSOK N-004 Rev. 3 checks in this report are evaluated by NorsokChecker from that model.");
			sb.AppendLine("</div>");
			sb.AppendLine("</body></html>");
			return sb.ToString();
		}

		/// <summary>
		/// Chapters 1–3 are the summary, the connection overview and the method, so the first
		/// connection is chapter 4. A named constant because three places have to agree on it — the
		/// index, the headings, and the "is this the first connection" test that suppresses one page
		/// break. (It was 3 until the method chapter was pulled out of every connection.)
		/// </summary>
		private const int ConnectionChapterBase = 4;

		private static string AnchorFor(int chapter) => $"ch-{chapter}";

		/// <summary>
		/// How the checks are made — ONCE, before the connections.
		///
		/// These six paragraphs used to sit inside every assessed connection: measured on the export,
		/// six connections × six paragraphs of identical prose, and half of them had just been added
		/// to answer the reviewer's questions. That is the repetition their §4 objected to, made
		/// worse by answering §6.
		///
		/// What stays per connection is what DIFFERS there: the plane, the chord, the geometry, the
		/// forces, the governing state, and the one sentence saying how many of ITS braces lie in the
		/// fitted plane. The method is the same for all of them, so it is stated where a reader meets
		/// it first and can refer back to it.
		/// </summary>
		private static void RenderMethodChapter(StringBuilder sb)
		{
			sb.AppendLine("<h2 class='section-header' id='ch-method'>"
				+ "<span class='chapter-no'>3</span> How the checks are made</h2>");
			sb.AppendLine("<div class='norm-box'>");

			// These were two paragraphs, and the first contradicted the second: it opened "Forces are
			// resolved into the joint plane" and the next one existed to say that they are not —
			// each brace is resolved in ITS OWN chord-brace plane. It also promised what the
			// per-connection section then re-promised in its own words. One paragraph, stating the
			// frame once and correctly.
			sb.AppendLine("  <p><strong>Forces are resolved, and each brace in the plane of its own "
				+ "chord&ndash;brace pair</strong> &mdash; the normal is "
				+ "e<sub>x</sub>&nbsp;&times;&nbsp;(brace axis), not the single fitted joint plane. "
				+ "The &sect;6.4 checks are therefore not evaluated on the member load effects as "
				+ "IDEA StatiCa Connection shows them. The fitted plane does two other things: it "
				+ "decides the K/Y/X classification, and it fixes the SIGN of M<sub>y</sub> "
				+ "consistently across the braces. So a brace's own <i>off-plane</i> deviation cannot "
				+ "appear in its own resolved forces &mdash; that column is a coplanarity check on "
				+ "the joint, not an input to the transformation. Two joints differing only in it "
				+ "therefore have identical force tables, which is a consequence of the frame rather "
				+ "than a transformation that failed to run.</p>");

			sb.AppendLine("  <p><strong>The plane passes through the chord axis.</strong> A brace's "
				+ "out-of-plane eccentricity is measured from THAT plane, not from the model's work "
				+ "point, so a joint whose members are all displaced together &mdash; the braces "
				+ "staying coplanar, their common plane merely offset &mdash; is not penalised for "
				+ "it. Each connection states the offset where there is one. What the check judges "
				+ "is how far a brace sits from the plane through its chord; moving the chord by "
				+ "&minus;e is the same joint as moving every brace by +e, and reads the same.</p>");

			// The (§6.4.3.6) reference used to live only on the per-card legend, which this sentence
			// duplicated word for word 40 times. The sentence is the one that stays, so it takes the
			// clause reference with it.
			sb.AppendLine("  <p><strong>Sign conventions.</strong> N is positive in TENSION. "
				+ "M<sub>y</sub> is in-plane and M<sub>z</sub> out-of-plane bending <em>of the joint "
				+ "plane</em> (eq 6.57, &sect;6.4.3.6), not of a member's local y and z. Section "
				+ "forces are taken AT THE NODE and projected without an r&times;F transfer, "
				+ "matching the reference implementation.</p>");

			// THE EQUATIONS, ONCE, IN THE CHAPTER THAT TALKS ABOUT THEM.
			//
			// This chapter stated that the resistance is recomputed for every load effect and then
			// showed no equation at all, so a reader met eq (6.52) for the first time on page 12,
			// inside a check. Meanwhile every check card announced the same three resistances
			// symbolically before substituting them — 40 identical copies of what belongs here.
			//
			// Taken from FormulaLatex rather than retyped, so the chapter and the cards cannot
			// drift apart: there is one source for these strings and this is a second reader of it.
			if (FormulaLatex.TryGetValue("6.4.3.6", out var eq64))
			{
				sb.AppendLine("  <p><strong>The equations.</strong> Each brace is checked against "
					+ "eq (6.57), whose three resistances come from eq (6.52) and eq (6.53). A check "
					+ "card substitutes these and prints the result; the symbols are defined below "
					+ "so the substitution can be followed without leaving the card.</p>");
				sb.AppendLine($"  <div class='formula-math'>$${eq64.check}$$</div>");
				sb.AppendLine($"  <div class='formula-math'>$${eq64.latex}$$</div>");
				sb.AppendLine("  <table class='deriv-table'>");
				sb.AppendLine("    <tr><th>symbol</th><th>meaning</th></tr>");
				foreach (var (sym, meaning) in new[]
				{
					// "— not the brace's" was here and was WRONG: f_y,brace does enter §6.4, through
					// Q_g's φ = (t·f_y,brace)/(T·f_y,chord), and the derivation's geometry table prints
					// both yields for that reason. The gloss told the reader to disregard a quantity
					// the next page substitutes.
					("f<sub>y,chord</sub>", "yield strength of the chord at the joint, in eq (6.52)/(6.53)"),
					("T, D", "chord wall thickness and outside diameter"),
					("d", "brace outside diameter"),
					("&theta;", "angle between the brace and the chord"),
					("&gamma;<sub>M</sub>", "material factor, 1.15 (&sect;6.4.3.2)"),
					("Q<sub>u</sub>", "strength factor, Table 6-3"),
					("Q<sub>f</sub>", "chord-action factor, eq (6.54), with the Table 6-4 coefficients"),
					("Q<sub>g</sub>", "gap factor for K, Table 6-3"),
					("Q<sub>&beta;</sub>", "geometric factor, Table 6-3"),
				})
					sb.AppendLine($"    <tr><td>{sym}</td><td>{meaning}</td></tr>");
				sb.AppendLine("  </table>");
			}

			sb.AppendLine("  <p><strong>Shear and torsion do not enter this check.</strong> "
				+ "Eq (6.57) has three terms &mdash; axial, in-plane and out-of-plane bending. That "
				+ "is not a statement that the other actions do not matter: they are listed with each "
				+ "brace's forces so their magnitude can be seen, and they must be verified elsewhere "
				+ "(member and section checks to &sect;6.3, and the weld or connection detail "
				+ "itself).</p>");

			// The closing sentence used to announce the per-connection runner-up column. That column
			// introduces itself, in its own note, where the reader is looking at it.
			sb.AppendLine("  <p><strong>The governing load effect is chosen per brace.</strong> "
				+ "Every active load effect is evaluated on every brace, and the state with the "
				+ "highest utilisation governs &mdash; which need not be the same state for two "
				+ "braces of one joint. Note that it is NOT the state with the largest force: "
				+ "N<sub>Rd</sub> depends on Q<sub>f</sub>, Q<sub>f</sub> on the chord stresses, and "
				+ "those on the load effect, so each candidate state has its <b>own resistance</b> "
				+ "and the resistance is recomputed for every state rather than the forces being "
				+ "compared against one.</p>");

			sb.AppendLine("</div>");
		}

		/// <summary>
		/// Is a contents page worth printing? Only when some connection has more than one method
		/// chapter — otherwise there is no hierarchy to map.
		///
		/// A property of the DOCUMENT, not a user setting: nobody should be asked to decide what the
		/// document already determines. With one method per connection the contents degenerates into
		/// a list of connection names reproducing the first column of the overview table on the next
		/// page — fifteen lines costing a full page out of 173. That emptiness is also why the
		/// verdicts were in it: they were filling a structural void rather than serving a reader.
		///
		/// Today this is always false (the registry holds one chapter), which is exactly why the
		/// rule is testable now: its false branch IS the current document.
		/// </summary>
		internal static bool ShouldRenderContents(
			IReadOnlyList<(string connectionName, List<NorsokFormulaResult> formulas)> allResults)
		{
			foreach (var (_, formulas) in allResults)
				if (MethodCountOf(formulas) > 1) return true;
			return false;
		}

		/// <summary>
		/// How many distinct method chapters one connection produced rows for.
		///
		/// Keyed on the row's own Section prefix rather than on ChapterRegistry: the registry holds
		/// exactly one chapter today, so routing through it would make the multi-method case
		/// impossible to construct in a test — a rule whose true branch cannot be reached is a rule
		/// nobody can check. The prefix is what the registry itself matches on ("6.4", "6.3", …), so
		/// this agrees with it for every chapter that exists and keeps working for ones that do not
		/// yet.
		/// </summary>
		internal static int MethodCountOf(IEnumerable<NorsokFormulaResult> formulas) =>
			formulas
				.Where(f => !f.IsNote && !string.IsNullOrEmpty(f.Section))
				.Select(f =>
				{
					// "6.4.3.6" → "6.4": the chapter is the first two dotted components.
					var parts = f.Section.Split('.');
					return parts.Length >= 2 ? $"{parts[0]}.{parts[1]}" : f.Section;
				})
				.Distinct()
				.Count();

		/// <summary>
		/// No page footer. The report does not number its own pages — the reader does, in whatever
		/// document this ends up in.
		///
		/// Numbering was built, shipped, and taken out again, and the reason is worth keeping:
		/// **an offset page number cannot be produced in a page margin box on this engine.** Measured
		/// on WebView2 1.0.2903.40 by printing three pages under six candidate rules
		/// (PrintedPageProbe.WhatCounterResetDoesToPageNumbering):
		///
		///   plain, no reset                   1,  2,  3    the only thing that advances
		///   @page { counter-reset: page 76 }  76, 76, 76   sets the exact value, on EVERY page
		///   @page:first { reset 77 }          77,  1,  2   applies once, then counting restarts
		///   reset + counter-increment         77, 77, 77   the increment re-applies too
		///   document counter, in the content  77, 78, 79   works, but not in the margin
		///   document counter, in @page box     0,  0,  0   page context sees only `page`/`pages`
		///
		/// So "start at page 77" printed 77 on all 187 pages — reported from the running app, which
		/// is how the defect was found. Rather than keep a setting that cannot do what it says, the
		/// footer is gone: a report bound into someone else's calculation package is numbered by
		/// that package, and one read on its own is short enough to page through.
		///
		/// The method stays as the seam. If a PDF post-processing pass is ever added — the same one
		/// /Outlines and the document properties need — page numbers become possible there, stamped
		/// onto the finished file where the offset is a plain integer.
		/// </summary>
		internal static string FooterCss(Models.PageSetup? setup) =>
			"@page { @bottom-center { content: none; } }";

		/// <summary>
		/// The utilisation colour scale, beside the figure it explains.
		///
		/// The caption claimed "members coloured by their governing utilisation" with no scale
		/// anywhere in the document, so an olive member could be at 40 % or at 70 % and the reader
		/// had no way to tell. A claim about a picture that the picture cannot support.
		///
		/// Drawn from <see cref="UtilisationScale.LitHexOfBand"/>, NOT the flat swatches: the figure
		/// is a PNG rendered by the 3D view, so its members carry the lit tones, and a legend in the
		/// flat colours would sit beside a lit cylinder and visibly not match. One definition of the
		/// scale, two sets of tones for two lighting conditions — see UtilisationScale's remarks.
		///
		/// The over-capacity band is separated by a gap: it is not a finer step on the ramp but a
		/// different statement, which is the one distinction a reader must not miss.
		/// </summary>
		private static void RenderUtilisationLegend(StringBuilder sb)
		{
			sb.AppendLine("  <div class='util-legend'>");
			sb.AppendLine("    <span class='util-legend-label'>utilisation</span>");
			for (int band = 0; band < Models.UtilisationScale.RampBandCount; band++)
				sb.AppendLine($"    <span class='util-swatch' style='background:"
					+ $"{Models.UtilisationScale.LitHexOfBand(band)}' "
					+ $"title='{Models.UtilisationScale.BandLabel(band)}'></span>");
			sb.AppendLine("    <span class='util-legend-tick'>0</span>");
			sb.AppendLine("    <span class='util-legend-tick'>100 %</span>");
			int over = Models.UtilisationScale.BandCount - 1;
			sb.AppendLine($"    <span class='util-swatch util-swatch-over' style='background:"
				+ $"{Models.UtilisationScale.LitHexOfBand(over)}' "
				+ $"title='{Models.UtilisationScale.BandLabel(over)}'></span>");
			sb.AppendLine("    <span class='util-legend-tick'>&gt; 100 %</span>");
			sb.AppendLine("  </div>");
		}

		/// <summary>
		/// The contents page: every chapter, numbered, linked to its own heading.
		///
		/// On a page of its own (break-after), because a report of fifteen joints is read by looking
		/// one up rather than front to back. No page NUMBERS: an HTML-to-PDF pass does not know them,
		/// and a contents page with invented numbers is worse than one without.
		///
		/// The verdicts come from <see cref="CheckWorkflow.Roll"/>, the same function the connection
		/// table and the app's own connection list use — recomputing them here is how an index comes
		/// to disagree with the table two inches above it.
		/// </summary>
		private static void RenderIndex(StringBuilder sb,
			IReadOnlyList<(string connectionName, List<NorsokFormulaResult> formulas)> allResults)
		{
			sb.AppendLine("<section class='index-page'>");
			sb.AppendLine("  <h2 class='index-title'>Contents</h2>");
			sb.AppendLine("  <table class='index-table'>");

			sb.AppendLine("    <tr><td class='ix-no'>1</td>"
				+ "<td class='ix-name'><a href='#ch-1'>Summary</a></td>"
				+ "<td class='ix-verdict'></td><td class='ix-util'></td></tr>");
			sb.AppendLine("    <tr><td class='ix-no'>2</td>"
				+ "<td class='ix-name'><a href='#ch-2'>Connection overview</a></td>"
				+ "<td class='ix-verdict'></td><td class='ix-util'></td></tr>");

			int chapter = ConnectionChapterBase;
			foreach (var (name, formulas) in allResults)
			{
				var verdict = CheckWorkflow.Roll(formulas);
				string cls = verdict.Pass switch
				{
					"FAIL" => "fail",
					"PASS" => "pass",
					_ => "warn",
				};
				// An em dash where nothing was assessed — 0.0 % would read as an excellent result for
				// a joint nobody checked, the trap this report has closed three times elsewhere.
				string util = verdict.Pass == "N/A" ? "&mdash;" : Pct(verdict.MaxUtilisation);

				sb.AppendLine($"    <tr><td class='ix-no'>{chapter}</td>"
					+ $"<td class='ix-name'><a href='#{AnchorFor(chapter)}'>{Esc(name)}</a></td>"
					+ $"<td class='ix-verdict {cls}'>{Esc(verdict.Pass)}</td>"
					+ $"<td class='ix-util'>{util}</td></tr>");
				chapter++;
			}

			sb.AppendLine("  </table>");
			sb.AppendLine("</section>");
		}

		private static void RenderSummaryCard(StringBuilder sb,
			IReadOnlyList<(string connectionName, List<NorsokFormulaResult> formulas)> allResults)
		{
			// THREE counters, each with ONE unit. "Total Checks: 55" used to add 30 real check
			// results to 25 unmet scope conditions — two different things in one number, and the
			// bigger half was not checks at all. (One rejected joint contributes one row per unmet
			// condition: CON6 alone put 7 into that total. Measured on the shipped report.)
			//
			// Notes take no part: they qualify a check that ran, so they are neither a check of
			// their own nor a gap in the coverage.
			int checksPerformed = allResults.Sum(r =>
				r.formulas.Count(f => !f.IsNote && !f.NotAssessed));

			// QUALIFIED IS ITS OWN BUCKET, and it comes OUT of "passed".
			//
			// The summary read "40 Checks performed / 40 Passed / 0 Failed" over an overview that
			// called CON11 QUALIFIED — the two levels disagreeing about the same connection, and
			// from the summary alone a reader could not tell that one of the forty rested on
			// formulas extrapolated outside the §6.4.3.1 validity ranges. That is the round-2
			// defect (a caveat reaching the detail but not the roll-up) one level up.
			//
			// Subtracted rather than added: a qualified check DID pass, so counting it in both
			// would make the buckets sum to more than the checks performed — the "Total Checks: 55"
			// mistake in the other direction.
			int qualified = allResults.Sum(r =>
				r.formulas.Count(f => !f.IsNote && !f.NotAssessed && f.Passed && f.IsQualified));
			int passed = allResults.Sum(r =>
				r.formulas.Count(f => !f.IsNote && !f.NotAssessed && f.Passed)) - qualified;
			int failed = checksPerformed - passed - qualified;
			int notes = allResults.Sum(r => r.formulas.Count(f => f.IsNote));

			// The gaps, in their own unit: rows nobody checked, and how they split by REASON — the
			// distinction a reader acts on. Scope means use another method; not evaluated means fix
			// the model and run again.
			var gapRows = allResults
				.SelectMany(r => r.formulas.Where(f => !f.IsNote && f.NotAssessed))
				.ToList();
			// Ask the predicate, never `== NotEvaluated`: the blocked-input case has three
			// refinements (switched off / none defined / unreadable), and a `!=` test counted every
			// one of them as a scope rejection — a joint whose states were merely switched off was
			// reported to the reader as one §6.4 does not cover.
			int outsideScope = gapRows.Count(f => f.Reason.IsOutsideScope());
			int notEvaluated = gapRows.Count(f => f.Reason.IsBlockedInput());
			int notAssessed = gapRows.Count;

			// And the connections, in theirs — the unit a reviewer actually counts in.
			var verdicts = allResults.Select(r => CheckWorkflow.Roll(r.formulas)).ToList();
			// QUALIFIED belongs here: the connection WAS assessed, its result simply carries the
			// §6.4.3.1 caveat. Leaving it out counted a checked joint among the unassessed ones.
			int consAssessed = verdicts.Count(v => v.Pass is "PASS" or "FAIL" or "PARTIAL" or "QUALIFIED");
			int consNotAssessed = verdicts.Count - consAssessed;

			// Governing formula (highest utilization) — only among rows that were actually checked
			NorsokFormulaResult? governing = null;
			string? governingConnection = null;
			foreach (var (name, formulas) in allResults)
			{
				foreach (var f in formulas)
				{
					if (f.IsNote || f.NotAssessed) continue;
					if (governing == null || f.Utilization > governing.Utilization)
					{
						governing = f;
						governingConnection = name;
					}
				}
			}

			// "COMPLIANT" must not be claimed when part of the model was never checked — nor when
			// NOTHING was. With no rows at all every count is zero, so the arithmetic below used to
			// fall through to COMPLIANT with a green tick over "0 Total Checks": reachable by
			// unchecking both chapter boxes and pressing Run, and it is the exportable deliverable
			// that said it. The connection list already reported that run as N/A.
			bool nothingChecked = checksPerformed <= 0;

			// Nor may "COMPLIANT" be claimed when a check ran on geometry OUTSIDE the §6.4.3.1
			// validity ranges. Every check can pass and nothing be unassessed, and the resistance is
			// still an extrapolation of formulas fitted inside those ranges — which the norm itself
			// flags. The headline is the one line a reader takes away, so the caveat has to survive
			// into it, exactly as it now survives into the overview row.
			int consQualified = verdicts.Count(v => v.Pass == "QUALIFIED");

			string statusClass = failed > 0 ? "fail"
				: (notAssessed > 0 || nothingChecked || consQualified > 0) ? "warn"
				: "pass";
			string verdict = failed > 0 ? "NON-COMPLIANT"
				: nothingChecked ? "NOT ASSESSED — no check was performed"
				: notAssessed > 0 ? "INCOMPLETE — part of the model was not assessed"
				: consQualified > 0
					? "QUALIFIED — every check passed, but geometry outside the §6.4.3.1 validity range"
					: "COMPLIANT";
			string icon = failed > 0 ? "&#x2718;"
				: (notAssessed > 0 || nothingChecked || consQualified > 0) ? "&#x26A0;"
				: "&#x2714;";

			// Chapter 1, with the id the contents page links to.
			sb.AppendLine("<h2 class='section-header' id='ch-1'>"
				+ "<span class='chapter-no'>1</span> Summary</h2>");
			sb.AppendLine($"<div class='summary-card {statusClass}'>");
			sb.AppendLine($"  <div class='summary-verdict'>");
			sb.AppendLine($"    <span class='summary-icon'>{icon}</span>");
			sb.AppendLine($"    <span class='summary-text'>NORSOK N-004: <strong>{verdict}</strong></span>");
			sb.AppendLine($"  </div>");
			sb.AppendLine($"  <div class='summary-stats'>");
			// Each counter says what it counts. "Checks performed", not "Total Checks": the old label
			// invited the reader to read it as everything the report covers, which is exactly what
			// made adding unmet conditions to it seem reasonable.
			sb.AppendLine($"    <div class='stat'><span class='stat-value'>{checksPerformed}</span>"
				+ "<span class='stat-label'>Checks performed</span></div>");
			sb.AppendLine($"    <div class='stat stat-pass'><span class='stat-value'>{passed}</span>"
				+ "<span class='stat-label'>Passed</span></div>");
			// Between Passed and Failed, and only when there are any: a qualified check passed, but
			// on extrapolated formulas, and the reader has to meet that here rather than sixty
			// pages later in a detail card.
			if (qualified > 0)
				sb.AppendLine($"    <div class='stat stat-warn'><span class='stat-value'>{qualified}"
					+ "</span><span class='stat-label'>Qualified &mdash; outside &sect;6.4.3.1 "
					+ "range</span></div>");
			sb.AppendLine($"    <div class='stat stat-fail'><span class='stat-value'>{failed}</span>"
				+ "<span class='stat-label'>Failed</span></div>");
			// The two kinds of gap, separately — one is the norm's boundary, the other is our
			// inability to read the model, and a reader does something different about each.
			if (outsideScope > 0)
				sb.AppendLine($"    <div class='stat'><span class='stat-value'>{outsideScope}</span>"
					+ "<span class='stat-label'>Outside &sect;6.4 scope</span></div>");
			if (notEvaluated > 0)
				sb.AppendLine($"    <div class='stat'><span class='stat-value'>{notEvaluated}</span>"
					+ "<span class='stat-label'>Not evaluated</span></div>");
			sb.AppendLine($"    <div class='stat'><span class='stat-value'>"
				+ $"{consAssessed} / {verdicts.Count}</span>"
				+ "<span class='stat-label'>Connections assessed</span></div>");
			if (notes > 0)
				sb.AppendLine($"    <div class='stat'><span class='stat-value'>{notes}</span><span class='stat-label'>Notes</span></div>");

			if (governing != null)
			{
				sb.AppendLine($"    <div class='stat stat-governing'>");
				sb.AppendLine($"      <span class='stat-value'>{Pct(governing.Utilization)}</span>");
				sb.AppendLine($"      <span class='stat-label'>Governing: &sect;{Esc(governing.Section)} {Esc(governing.Title)}</span>");
				sb.AppendLine($"    </div>");
			}

			sb.AppendLine($"  </div>");
			sb.AppendLine($"</div>");

			// The connection table is NOT rendered here any more — GenerateReport orders the three
			// front-matter blocks itself, because the contents page has to come before the table it
			// indexes and could not while this method emitted both.
		}

		/// <summary>
		/// One row per connection: its verdict and its governing utilisation.
		///
		/// The card above answers "did the project pass"; this answers "where is the problem", which
		/// on a project of fifteen joints is the question the first page has to settle. Without it a
		/// reader had to scroll every per-joint section to find the one that failed.
		///
		/// The verdict comes from <see cref="CheckWorkflow.Roll"/> — the same function the connection
		/// list and the run use — so the report cannot disagree with the app about what a connection
		/// is. Restating those rules here is exactly how the two would drift.
		/// </summary>
		private static void RenderConnectionTable(StringBuilder sb,
			IReadOnlyList<(string connectionName, List<NorsokFormulaResult> formulas)> allResults)
		{
			if (allResults.Count == 0) return;

			// Chapter 2. Numbered and anchored like the rest, so the contents page can reach it.
			sb.AppendLine("<h2 class='section-header' id='ch-2'>"
				+ "<span class='chapter-no'>2</span> Connection overview</h2>");
			sb.AppendLine("<table class='connection-table'>");
			sb.AppendLine("  <tr><th>Connection</th><th>Verdict</th><th>Governing utilisation</th>"
				+ "<th>Note</th></tr>");

			foreach (var (name, formulas) in allResults)
			{
				var verdict = CheckWorkflow.Roll(formulas);

				string cls = verdict.Pass switch
				{
					"FAIL" => "fail",
					"PASS" => "pass",
					_ => "warn",
				};

				// The governing utilisation only where something was assessed. On an N/A row the
				// figure would be 0.0 %, which reads as an excellent result for a joint nobody checked
				// — the same trap the check cards and the results table both had.
				string util = verdict.Pass == "N/A" ? "&mdash;" : Pct(verdict.MaxUtilisation);

				sb.AppendLine("  <tr>");
				sb.AppendLine($"    <td class='con-name'>{Esc(name)}</td>");
				sb.AppendLine($"    <td class='con-verdict {cls}'>{Esc(verdict.Pass)}</td>");
				sb.AppendLine($"    <td class='con-util'>{util}</td>");
				// The unmet recommendation goes in the Note column BESIDE the status, not into the
				// verdict: a "should" of the standard is reported, not judged. Without this the
				// overview said "Norsok OK" over a detail page recording §6.4.1 unmet.
				string note = Esc(verdict.Status)
					+ (string.IsNullOrEmpty(verdict.Recommendations)
						? ""
						: $"<br/><span class='con-rec'>{Esc(verdict.Recommendations!)}</span>");
				sb.AppendLine($"    <td class='con-note'>{note}</td>");
				sb.AppendLine("  </tr>");
			}

			sb.AppendLine("</table>");

			// NO project-wide list of governing load effects here.
			//
			// One was built and removed. The review asked for a load-case legend, and this was the
			// answer to it — but pooling the governing states of fifteen joints into one line is the
			// same mistake as "Total Checks: 55": it adds up things that belong to different
			// connections, and the sum means nothing to anyone. Which state governs a joint is a
			// property OF THAT JOINT, and it is already on every check's card and in its derivation,
			// where the reader is looking at the joint it belongs to.
			//
			// (It was also wrong in detail, which is what exposed the idea: the list sorted the names
			// alphabetically, so "LE11" and "LE12" came before "LE2" — and a load effect is named
			// freely in IDEA StatiCa, exactly like a connection, so on names like "Vítr Y+" no
			// ordering carries meaning at all. `LE{id}` is only this app's fallback for a state the
			// model left unnamed; see JointEnvelope.cs:56.)
			//
			// A legend worth having would list each state's FORCES, per connection. That is an
			// appendix, not a line under the overview table.
		}

		/// <summary>
		/// One card for a joint the chapter could not assess, listing every unmet condition.
		///
		/// Replaces one card per condition. The count belongs in the heading — it is the orientation
		/// ("a lot" against "one thing") — and the conditions belong in the body, where they were
		/// missing entirely: each of the old cards opened onto "no §6.4 check was performed for this
		/// joint", while the reason it was not performed sat unread on the row.
		/// </summary>
		private static void RenderRejectionCard(StringBuilder sb,
			IReadOnlyList<NorsokFormulaResult> rejections, string chapterKey, bool expandAll)
		{
			sb.AppendLine($"<details class='check-card warn'{(expandAll ? " open" : "")}>");
			sb.AppendLine("  <summary class='card-header warn'>");
			sb.AppendLine("    <span class='status-icon'>&#x26A0;</span>");
			sb.AppendLine($"    <span class='section-ref'>&sect;{Esc(chapterKey)}</span>");
			sb.AppendLine($"    <span class='card-title'>Outside the scope of &sect;{Esc(chapterKey)}"
				+ $" &mdash; {rejections.Count} conditions not met</span>");
			sb.AppendLine("    <span class='util-badge warn'>&mdash;</span>");
			sb.AppendLine("  </summary>");

			sb.AppendLine("  <div class='card-body'>");
			sb.AppendLine("    <p class='deriv-note'>The checks of this chapter rest on quantities the "
				+ "joint does not provide &mdash; the joint plane, the averaged chord stresses and the "
				+ "force balance are properties of the WHOLE joint, so while any condition below is "
				+ "unmet no brace can be assessed, not even one whose own geometry is fine.</p>");

			sb.AppendLine("    <table class='where-table'>");
			for (int i = 0; i < rejections.Count; i++)
			{
				string reason = string.IsNullOrWhiteSpace(rejections[i].CheckExpression)
					? rejections[i].Title
					: rejections[i].CheckExpression;
				sb.AppendLine("      <tr>");
				sb.AppendLine($"        <td class='var-eq'>{i + 1}.</td>");
				sb.AppendLine($"        <td class='var-desc'>{Esc(reason)}</td>");
				sb.AppendLine("      </tr>");
			}
			sb.AppendLine("    </table>");

			sb.AppendLine("    <div class='result-bar warn'>");
			sb.AppendLine("      <span>Not assessed &mdash; the chapter does not apply to this joint</span>");
			sb.AppendLine("      <span class='result-verdict'>&#x26A0; N/A</span>");
			sb.AppendLine("    </div>");
			sb.AppendLine("  </div>");
			sb.AppendLine("</details>");
		}

		private static void RenderFormulaCard(StringBuilder sb, NorsokFormulaResult fr, bool expandAll = false)
		{
			// four states: a note qualifies a check that ran, an unassessed row means nothing ran,
			// and neither is a tick or a cross
			string statusClass = fr.IsNote || fr.NotAssessed ? "warn" : fr.Passed ? "pass" : "fail";
			string statusIcon = fr.IsNote ? "&#x24D8;"
				: fr.NotAssessed ? "&#x26A0;" : fr.Passed ? "&#x2714;" : "&#x2718;";
			string statusText = fr.Verdict;

			sb.AppendLine($"<details class='check-card {statusClass}'{(expandAll ? " open" : "")}>");

			// Collapsible header — click to expand/collapse
			sb.AppendLine($"  <summary class='card-header {statusClass}'>");
			sb.AppendLine($"    <span class='status-icon'>{statusIcon}</span>");
			sb.AppendLine($"    <span class='section-ref'>&sect;{Esc(fr.Section)}</span>");
			sb.AppendLine($"    <span class='card-title'>{Esc(fr.Title)}</span>");
			// Only where there IS an equation. It used to print unconditionally, so a card that
			// evaluated nothing carried "(Eq. -)" or "(Eq. 6.4.3)" — the first is a placeholder shown
			// to the customer, the second names a CLAUSE as though it were an equation. Suppressing
			// beats placeholdering: a badge that says nothing is worse than no badge.
			if (!string.IsNullOrWhiteSpace(fr.Equation) && fr.Equation != "-")
				sb.AppendLine($"    <span class='eq-ref'>(Eq. {Esc(fr.Equation)})</span>");
			if (fr.LoadCaseId > 0 || !string.IsNullOrEmpty(fr.LoadCaseName))
				sb.AppendLine($"    <span class='lc-badge'>{Esc(fr.LoadCaseName ?? $"LE{fr.LoadCaseId}")}</span>");
			// An em dash, not "0.0 %": on a note or an unassessed row the utilisation is not a small
			// number, it is no number at all (NorsokFormulaResult says so), and "0.0 %" reads as an
			// excellent result. The results grid already does this; the report did not.
			string utilBadge = fr.IsNote || fr.NotAssessed ? "&mdash;" : Pct(fr.Utilization);
			sb.AppendLine($"    <span class='util-badge {statusClass}'>{utilBadge}</span>");
			sb.AppendLine("  </summary>");

			sb.AppendLine("  <div class='card-body'>");

			// A §6.4 CARD BEGINS WITH ITS DERIVATION. Everything that used to precede it was a copy.
			//
			// The card carried, above the derivation: the interaction inequality under a
			// "Check condition" label, the three resistances symbolically, and a "Substitution" box.
			// Each is stated elsewhere, and the elsewhere is better:
			//
			//   inequality       chapter 3 (with the symbol table), the eq (6.57) derivation step
			//                    (with its numbers), and the result bar — which prints the
			//                    comparison a reader actually checks, "0.7373 <= 1.0".
			//   resistances      the M_Rd and N_Rd derivation steps, substituted.
			//   Substitution box "N_Rd(weighted K 100%…) = 241.9 kN; governing LC: LE12" — the
			//                    weighted resistance has its own derivation block, the
			//                    classification is in the card title, LE12 is a header badge. The
			//                    label was wrong too: three recalled values are not a substitution.
			//
			// Only §6.4. FormulaLatex is keyed by section and §6.3 renders through this same method
			// with NO derivation, so for those cards this block is the only formula they get —
			// removing it there would leave a result with nothing behind it, and no §6.4 test would
			// notice. ANonJointCardStillCarriesItsFormulaBlock is the guard.
			//
			// The ["6.4.3.6"] entry stays in the dictionary: chapter 3 renders it and is now its
			// only consumer.
			bool isJointCheck = fr.Section == "6.4.3.6";

			// Main formula in KaTeX (display math)
			if (!isJointCheck && FormulaLatex.TryGetValue(fr.Section, out var latex))
			{
				sb.AppendLine("    <div class='formula-block'>");
				sb.AppendLine($"      <p class='formula-label'>Check condition:</p>");
				sb.AppendLine($"      <div class='formula-math'>$${latex.check}$$</div>");
				sb.AppendLine($"      <p class='formula-label'>Design resistance:</p>");
				sb.AppendLine($"      <div class='formula-math'>$${latex.latex}$$</div>");
				sb.AppendLine("    </div>");
			}

			// Substituted values — for a card that has no DERIVATION.
			//
			// The label promises an expression that gives the result beneath it, and only the
			// mothballed chapters keep that promise: §6.3 prints
			// "N_t,Rd = 2747 × 355.0 / 1.15 = 847.9 kN", a real substitution and the only formula
			// those cards get. Every live §6.4 string is prose — "no §6.4 check was performed for
			// this joint", "the check proceeds; the note above qualifies its result" — or a recall
			// of two values dressed as an equation.
			//
			// The cause is historical: §6.4 gained a derivation, which substitutes properly step by
			// step with each step's own formula, and this one-liner became redundant and filled up
			// with whatever sentence was to hand.
			//
			// So the test is on what the card HAS, not on its section number. The same predicate
			// governs the "Where" table a few lines below, with the same reasoning written out
			// there; keying on the section instead left the box standing on the rejected-joint and
			// assumption cards, which is exactly the miss this replaces.
			//
			// TWO conditions, and each rules out a different kind of card:
			//
			//   JointDetail != null   the derivation IS the substitution, done properly, so a
			//                         one-line box beside it can only be a summary of it.
			//   NotAssessed / IsNote  nothing was computed, so there is nothing to substitute. What
			//                         those cards put in the field is a sentence — "no §6.4 check
			//                         was performed for this joint", "the check proceeds; the note
			//                         above qualifies its result" — and the condition row beneath
			//                         already says it, per brace and more precisely.
			//
			// Both are properties of the card, not of its section string. Keying on "6.4.3.6" left
			// the box standing on the rejected-joint and assumption cards, which is the miss this
			// replaces; keying on the chapter would have caught those two and still not the
			// un-assessable brace, which is a 6.4.3.6 card with no derivation.
			//
			// The producers stop setting the field as well (Chapter64, NorsokCheckRunner) — belt and
			// braces, because a renderer cannot tell prose from an expression and should not have to
			// guess. The FIELD itself stays: the §6.4 tab's grid and CheckWorkflow read it.
			if (fr.JointDetail == null && !fr.NotAssessed && !fr.IsNote
				&& !string.IsNullOrEmpty(fr.FormulaSubstituted))
			{
				sb.AppendLine("    <div class='substituted'>");
				sb.AppendLine($"      <p class='formula-label'>Substitution:</p>");
				sb.AppendLine($"      <p class='formula-sub'>{Esc(fr.FormulaSubstituted)}</p>");
				sb.AppendLine("    </div>");
			}

			// Where block — ONLY for a check that has no derivation of its own.
			//
			// A derivation states the same quantities with their formula AND their substitution, which
			// is strictly more than a value in a table, so printing both made the taller half of the
			// card a duplicate. Audited row by row on §6.4: of its 31 variables, D/T/d/t/θ/β/γ/τ/γ_M
			// and the classification are the derivation's "Geometry & material" table, the three
			// actions are its "Applied forces" table, σ_a/σ_my/σ_mz are three substituted steps,
			// Q_u,ipb/Q_u,opb appear inside the M_Rd steps that use them, Q_g/Q_u,axial/Q_f/N_Rd are
			// one step each per active mode with K split per gap, and the three interaction terms are
			// substituted into eq (6.57) itself. Nothing was left over.
			//
			// The condition is on HAVING a derivation rather than on the section number: §6.3 (see
			// Services/Formulas63_Mothballed) fills Variables and has no derivation renderer, so its
			// cards keep the table — and a chapter that gains a derivation later loses it without
			// anyone having to remember this rule.
			if (fr.Variables.Count > 0 && fr.JointDetail == null)
			{
				sb.AppendLine("    <div class='where-block'>");
				sb.AppendLine("      <p class='where-header'>Where:</p>");
				sb.AppendLine("      <table class='where-table'>");

				foreach (var v in fr.Variables)
				{
					string katexSymbol = SymbolToKatex(v.Symbol);
					sb.AppendLine("        <tr>");
					sb.AppendLine($"          <td class='var-symbol'>$ {katexSymbol} $</td>");
					sb.AppendLine($"          <td class='var-eq'>=</td>");
					sb.AppendLine($"          <td class='var-value'>{v.FormattedValue}</td>");
					sb.AppendLine($"          <td class='var-desc'>&mdash; {Esc(v.Description)}</td>");
					sb.AppendLine("        </tr>");
				}

				sb.AppendLine("      </table>");
				sb.AppendLine("    </div>");
			}

			// §6.4 auto-topology derivation blocks (per-class Qu/Qf, K per gap, chord-stress trail, validity)
			if (fr.JointDetail != null)
				RenderJointDerivation(sb, fr.JointDetail);

			// Result bar. Where nothing was checked there is no utilisation to state, so the bar
			// carries the REASON instead — "Utilization: — (not assessed)" occupied the one line a
			// reader looks at with a non-answer, while the reason sat unread on the row. (The earlier
			// version printed "0.0% (= 0.0000 <= 1.0)" there, which claimed a check had been made and
			// had passed comfortably, next to the word N/A.)
			sb.AppendLine($"    <div class='result-bar {statusClass}'>");
			if (fr.IsNote || fr.NotAssessed)
			{
				string reason = !string.IsNullOrWhiteSpace(fr.CheckExpression)
					? fr.CheckExpression
					: fr.IsNote ? "note" : "not assessed";
				sb.AppendLine($"      <span>{Esc(reason)}</span>");
			}
			else
			{
				// "Utilisation", British — NORSOK and EN use it, and this was the one place in the
				// report still spelling it the American way: 30 occurrences beside 60 of the other.
				// The property is still called Utilization; renaming it would be churn no reader sees.
				sb.AppendLine($"      <span>Utilisation: <strong>{Pct(fr.Utilization)}</strong> "
					// InvariantCulture like the rest — this was the last comma decimal in the
					// document, and the whole-document sweep is what found it.
					+ $"(= {fr.Utilization.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)}"
					+ " &le; 1.0)</span>");
			}
			sb.AppendLine($"      <span class='result-verdict'>{statusIcon} {statusText}</span>");
			sb.AppendLine("    </div>");

			sb.AppendLine("  </div>"); // card-body
			sb.AppendLine("</details>"); // check-card
		}

		/// <summary>
		/// §6.4 auto-topology derivation — mirrors the python reference UI's detailed-check modal:
		/// classification breakdown, per-class Qu/Qf/N_Rd table (Table 6-3/6-4 give K, T/Y and X each
		/// their own axial row), K per-gap resistances, shared bending resistances, chord-stress trail
		/// (Begin/End average → section props → σ), and the §6.4.3.1 validity table.
		/// </summary>
		/// <summary>
		/// The derivation of one brace's check as a standalone page, for the §6.4 tab's derivation
		/// window. Same renderer as the report so the two can never disagree.
		/// </summary>
		/// <param name="brace">The brace this derivation is for — the subject of the page.</param>
		/// <param name="connection">The joint it belongs to.</param>
		/// <param name="state">The load effect, or "governing LEn" in envelope mode.</param>
		/// <param name="utilisation">Formatted utilisation, e.g. "88.8 %".</param>
		/// <param name="verdict">PASS / FAIL / N/A.</param>
		public static string GenerateDerivationPage(Norsok64.JointCheckRow row, string brace,
			string connection = "", string state = "", string utilisation = "", string verdict = "")
		{
			var sb = new StringBuilder();
			sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'/>");
			AppendKatex(sb);
			sb.AppendLine($"<style>{CssStyles}</style></head><body style='padding:14px'>");

			// Identity first, and all three parts of it: several of these pages can be open at once, so
			// the brace name alone does not say which check is being read. The subject (brace) is the
			// heading; the joint and the state qualify it on their own line.
			bool failed = string.Equals(verdict, "FAIL", StringComparison.OrdinalIgnoreCase);
			string verdictColour = failed ? "#C62828" : "#2E7D32";

			sb.AppendLine("<div style='margin:0 0 12px;padding:0 0 10px;border-bottom:2px solid #E3E8ED'>");
			var context = new[] { connection, state }.Where(s => !string.IsNullOrEmpty(s));
			if (context.Any())
				sb.AppendLine("  <div style='font-size:11.5px;color:#607D8B;margin:0 0 3px'>"
					+ Esc(string.Join(" · ", context)) + "</div>");

			sb.AppendLine("  <h2 style='margin:0;color:#1f3a5f;font-size:17px'>"
				+ $"{Esc(brace)}"
				+ (string.IsNullOrEmpty(utilisation) ? "" :
					$" <span style='font-weight:400;color:#455A64'>&mdash; utilisation {Esc(utilisation)}</span>")
				+ (string.IsNullOrEmpty(verdict) ? "" :
					$" <span style='color:{verdictColour};font-size:14px'>{Esc(verdict)}</span>")
				+ "</h2>");
			sb.AppendLine("</div>");

			RenderJointDerivation(sb, row);
			sb.AppendLine("</body></html>");
			return sb.ToString();
		}
		/// <summary>
		/// The full §6.4 derivation for one brace — the structure the python reference's detail modal
		/// uses, block for block, because that is the sheet an engineer checks a number against.
		///
		/// THE ORDER OF A HAND CALCULATION, in four phases, because that is what makes the sheet
		/// checkable — and being checkable is the whole point: an engineer who cannot follow the
		/// numbers will not trust the tool, and a half-transparent derivation is no better than none.
		///
		///   1. INPUTS — geometry &amp; material, then the applied forces.
		///   2. BASIC ASSUMPTIONS — the §6.4.3.1 validity ranges, every condition with its status.
		///      After the inputs (each condition is a relation between dimensions just listed) and
		///      before the derivation (outside these ranges the resistance is extrapolated).
		///   3. THE CHECK, step by step — chord stress trail (averaged sides, then sigma), A² with the
		///      moment resistances, one block per ACTIVE mode (K split per gap, Y, X), and the weighted
		///      axial resistance.
		///   4. FINAL VERDICT ON CAPACITY — eq (6.57).
		///
		/// Each step is label / symbolic formula / substituted numbers / result, so every line can be
		/// recomputed by hand. Two orders were tried and rejected before this one: eq (6.57) first
		/// (reads as a claim with an appendix, and the reader cannot verify a sum whose terms come
		/// after it), and the validity table last (it warns about numbers already read).
		///
		/// Each step is written as label / symbolic formula / substituted numbers / result — see
		/// <see cref="Step"/>. The substitution is the point: a result alone cannot be checked, and a
		/// symbolic formula alone does not say what went into it. Numbers there are in MPa, mm and kN
		/// regardless of any display-unit setting, matching the norm's own convention.
		///
		/// Formulas are typeset by the EMBEDDED KaTeX (see AppendKatex), so this works offline.
		/// </summary>
		private static void RenderJointDerivation(StringBuilder sb, Norsok64.JointCheckRow row)
		{
			var r = row.Engine;
			var inp = row.Inputs;
			var cl = row.Classification;
			if (r == null || inp == null || cl == null)
				return;

			// mm / MPa / kN, as the norm writes them — not the app's display units
			static string N(double v, int d = 3) =>
				double.IsNaN(v) || double.IsInfinity(v) ? "—" : v.ToString("F" + d,
					System.Globalization.CultureInfo.InvariantCulture);
			// parenthesise a negative so "a + (−b)" reads correctly inside a substituted formula
			static string P(double v, int d = 3) => v < 0 ? $"({N(v, d).Replace("-", "−")})" : N(v, d);

			double fy = inp.FyChord / 1e6, sa = inp.SigmaASd / 1e6;
			double smy = inp.SigmaMySd / 1e6, smz = inp.SigmaMzSd / 1e6;
			double dMm = inp.d * 1e3, tMm = inp.t * 1e3, dChordMm = inp.D * 1e3, tChordMm = inp.T * 1e3;

			sb.AppendLine("    <div class='deriv-block'>");

			// ══ 1. INPUTS ══ geometry, material and the actions, as they would be written down at the
			// top of the hand calculation this sheet is meant to be checked against.
			sb.AppendLine("      <p class='deriv-h'>Geometry &amp; material</p>");
			sb.AppendLine("      <table class='deriv-table'>");
			// The two yields are NAMED, because they are two different quantities that the formulas
			// treat differently: f_y,chord is the one in eq (6.52)/(6.53), and f_y,brace reaches a
			// result only through Q_g's phi, and only on an overlapped joint. Printed as a bare f_y
			// on both rows they looked like the same value repeated — which is exactly how they read
			// when both steels are S355 and the numbers coincide.
			Kv(sb, "Chord &oslash; D &times; T", $"{N(dChordMm, 1)}&times;{N(tChordMm, 1)} mm "
				+ $"(f<sub>y,chord</sub> = {N(fy, 0)} MPa)");
			Kv(sb, "Brace &oslash; d &times; t", $"{N(dMm, 1)}&times;{N(tMm, 1)} mm "
				+ $"(f<sub>y,brace</sub> = {N(inp.FyBrace / 1e6, 0)} MPa)");
			// "from the member axes" is worth saying: theta is DERIVED (JointTopologyBuilder's Theta —
			// the angle between the brace's effective direction and the chord axis, folded into
			// 0..90°), not a value anyone typed. It was the one description in the old "Where:" table
			// carrying information the derivation did not already state.
			Kv(sb, "&theta; (brace&ndash;chord)",
				$"{N(r.ThetaDeg, 1)}&deg; <span class='deriv-hint'>(from the member axes)</span>");
			Kv(sb, "chord face", row.ChordStress is { } cs0 ? (cs0.Side >= 0 ? "+ey face" : "&minus;ey face") : "&mdash;");
			Kv(sb, "&beta; = d/D", N(r.Beta));
			Kv(sb, "&gamma; = D/(2T)", N(r.Gamma));
			Kv(sb, "&tau; = t/T", N(r.Tau));
			Kv(sb, "classification", $"K {Pct2(cl.FrK)} &middot; Y {Pct2(cl.FrY)} &middot; X {Pct2(cl.FrX)}");
			Kv(sb, "&gamma;<sub>M</sub>", N(inp.GammaM, 3));
			sb.AppendLine("      </table>");

			sb.AppendLine("      <p class='deriv-h'>Applied forces (in the joint plane)</p>");
			sb.AppendLine("      <table class='deriv-table'>");
			Kv(sb, "N<sub>Sd</sub> (+ tension)", $"{N(inp.NSd / 1e3, 1)} kN");
			// y/z, as eq (6.57) writes them — M_y is the in-plane moment, M_z the out-of-plane one.
			// (The chord's own moments below keep ip/op: the norm gives THOSE no y/z symbol, and
			// they do not appear in eq 6.57 at all.)
			// THREE DECIMALS, not two. At 2 dp a 0.07 kN·m moment carries a 7 % uncertainty, and a
			// reader recomputing the interaction term from the printed inputs got a visibly
			// different answer on half the cards — 7.22 % printed against 7.00 % recomputed on one.
			// The term itself was right; the input shown was rounded and the one used was not. Two
			// decimals is not a scarcity: the same card prints Q_u to three.
			Kv(sb, "M<sub>y,Sd</sub> <span class='deriv-hint'>(in-plane)</span>",
				$"{N(inp.MipSd / 1e3, 3)} kN&middot;m");
			Kv(sb, "M<sub>z,Sd</sub> <span class='deriv-hint'>(out-of-plane)</span>",
				$"{N(inp.MopSd / 1e3, 3)} kN&middot;m");
			sb.AppendLine("      </table>");

			// WHICH PLANE, said where the symbols first appear.
			//
			// "(in-plane)" and "(out-of-plane)" answer which KIND of bending and leave open
			// in-plane of WHAT — and everywhere else in this application y and z are a MEMBER's
			// local axes, so a reader who knows the rest of the app derives the wrong thing. The
			// table heading names the joint plane, but that reads as where the forces were resolved
			// rather than as what the subscripts mean.
			//
			// This renderer is SHARED. The report card sits after chapter 3, which states the
			// convention in full — but the §6.4 tab's derivation window has no chapter 3, and that
			// window is the first place a user meets these symbols. Removing the per-card legend was
			// right for the report and left the app with nothing; the note belongs here instead, at
			// the table, in one line rather than the three-line block that went.
			//
			// Deliberately duplicated with chapter 3: one line beside the symbols is worth more than
			// a correct sentence forty pages earlier.
			sb.AppendLine("      <p class='deriv-note'>y and z are the <b>joint plane's</b>, "
				+ "not the member's local axes (eq 6.57, &sect;6.4.3.6).</p>");

			// ══ 2. BASIC ASSUMPTIONS ══ §6.4.3.1, checked against the inputs above.
			//
			// After the inputs, not before: every condition is a relation between the dimensions just
			// listed (beta = d/D, gamma = D/2T, tau = t/T, theta), so a reader can only verify one with
			// d and D already in front of them. And before the derivation, because §6.4's resistance
			// formulas are fitted to these ranges — outside them the resistance below is an
			// extrapolation, which is something to know before reading it rather than after.
			//
			// Every condition is listed with its status, always. Summarising them as "all met" was a
			// half-measure: whether beta = 0.298 lies in 0.2..1.0 is exactly what is being verified
			// here, and a single tick asks the reader to take the app's word for it — which is the
			// opposite of what this sheet is for.
			if (r.Validity.Count > 0)
			{
				int outside = r.Validity.Count(v => !v.Value);
				sb.AppendLine("      <p class='deriv-h'>Basic assumptions &mdash; validity ranges "
					+ "(&sect;6.4.3.1)"
					+ (outside > 0 ? $" &mdash; {outside} of {r.Validity.Count} OUTSIDE" : "")
					+ "</p>");
				sb.AppendLine("      <table class='deriv-table'>");
				sb.AppendLine("        <tr><th>condition</th><th>status</th></tr>");
				foreach (var (cond, ok) in r.Validity)
					sb.AppendLine($"        <tr><td>{ConditionHtml(cond)}</td>"
						+ $"<td>{(ok ? "&#10003; within" : "&#10007; outside")}</td></tr>");
				sb.AppendLine("      </table>");

				// §6.4.1's GAP PROVISION — informative, and it was missing entirely.
				//
				// "The gap for simple K-joints should be larger than 50 mm and less than D"
				// (N-004 Rev. 3 §6.4.1, read off the clause). It is a detailing requirement in a
				// General clause, separate from the four §6.4.3.1 validity conditions and worded
				// with "should", so it changes no verdict and does not join the table above.
				//
				// But the string "50 mm" appeared nowhere in a 227-page report whose K gaps were
				// 2, 8, 9 and 47 mm — every one of them below the provision — while the same report
				// rejected joints for gap rules at the NEGATIVE end. Stating it is what lets a
				// reader see that the detailing, not the check, is what deserves attention.
				if (row.Inputs is { } gi && gi.FrK > 1e-9 && gi.D > 0.0)
				{
					double gapMm = gi.G * 1e3, dMmChord = gi.D * 1e3;
					bool gapOk = gapMm > 50.0 && gapMm < dMmChord;
					// "informative" was wrong on the first half: §6.4.1 IS a normative clause. It is
					// this PROVISION that is a recommendation, because it says "should", which §3.1
					// defines as "among several possibilities one is recommended" against "shall" =
					// "requirements strictly to be followed in order to conform".
					sb.AppendLine("      <p class='deriv-note'>&sect;6.4.1 (detailing): the gap of a "
						+ "simple K-joint <em>should</em> be larger than 50 mm and less than D. Here "
						+ $"g = <b>{N(gapMm, 1)} mm</b> against 50 mm &lt; g &lt; {N(dMmChord, 0)} mm "
						+ $"&mdash; <b>{(gapOk ? "satisfied" : "not satisfied")}</b>. A "
						+ "&ldquo;should&rdquo; (&sect;3.1): a recommendation, not a condition of "
						+ "conformity, so no verdict depends on it &mdash; it is carried to the "
						+ "overview as a note.</p>");
				}
			}

			// The out-of-range rule, quoted and shown. §6.4.3.1 does not forbid a joint outside the
			// ranges — it requires the check to be run twice and the LESSER capacity used. That is
			// what the engine does, and until this block existed the sheet gave no sign of it: an
			// engineer recomputing N_Rd from the actual beta got a different number with nothing to
			// explain the difference, which is exactly the kind of unexplained gap that costs trust.
			if (r.LimitingPassApplied)
			{
				sb.AppendLine("      <p class='deriv-h'>Out-of-range rule (&sect;6.4.3.1)</p>");
				sb.AppendLine("      <p class='deriv-note'>&sect;6.4.3.1: &ldquo;The equations can be "
					+ "used for joints with geometries which lie outside the validity ranges, by taking "
					+ "the usable strength as the <b>lesser</b> of the capacities calculated on the "
					+ "basis of: a) actual geometric parameters, b) imposed limiting parameters for the "
					+ "validity range, where these limits are infringed.&rdquo; The check below was "
					+ "therefore run twice, and the smaller axial resistance is the one carried "
					+ "forward.</p>");
				sb.AppendLine("      <table class='deriv-table'>");
				sb.AppendLine("        <tr><th>pass</th><th>&beta;</th><th>&gamma;</th>"
					+ "<th>&theta;</th><th>N<sub>Rd</sub></th></tr>");

				bool actualGoverns = !double.IsNaN(r.NRdActual) && !double.IsNaN(r.NRdLimiting)
					&& r.NRdActual <= r.NRdLimiting;
				string mark = " &nbsp;&larr; governs";

				sb.AppendLine($"        <tr><td>a) actual geometry</td><td>{N(r.Beta)}</td>"
					+ $"<td>{N(r.Gamma)}</td><td>{N(r.ThetaDeg, 1)}&deg;</td>"
					+ $"<td>{N(r.NRdActual / 1e3, 1)} kN{(actualGoverns ? mark : "")}</td></tr>");
				sb.AppendLine($"        <tr><td>b) imposed limits</td><td>{N(r.BetaLimiting)}</td>"
					+ $"<td>{N(r.GammaLimiting)}</td><td>{N(r.ThetaLimitingDeg, 1)}&deg;</td>"
					+ $"<td>{N(r.NRdLimiting / 1e3, 1)} kN{(actualGoverns ? "" : mark)}</td></tr>");
				sb.AppendLine("      </table>");
			}

			if (!r.WithinRange)
				sb.AppendLine("      <p class='deriv-warn'>&#9888; Geometry outside the 6.4.3.1 "
					+ "validity range &mdash; the resistance below is the lesser of the two passes "
					+ "above, per &sect;6.4.3.1, and remains an extrapolation of formulas fitted "
					+ "inside the ranges.</p>");

			// ══ 3. THE CHECK, step by step ══ chord stress, A², resistances, then the modes.
			// ── chord stress derivation ──
			if (row.ChordStress is { } st && st.A > 0)
			{
				double aMm2 = st.A * 1e6, iMm4 = st.I * 1e12, rMm = st.R * 1e3;
				sb.AppendLine("      <p class='deriv-h'>Chord stress derivation &mdash; averaged sides "
					+ "&rarr; &sigma; (NORSOK p.31)</p>");
				sb.AppendLine("      <p class='deriv-note'>The chord carries two loadings at a joint "
					+ "(one per side of the brace intersection); NORSOK p.31 requires their AVERAGE in "
					+ "eq (6.54)/(6.55).</p>");
				sb.AppendLine("      <table class='deriv-table'>");
				// y/z here too, with a ,chord index. These are NOT terms of eq (6.57) — they are the
				// chord's own moments, on the way to sigma — but they are resolved into the SAME
				// plane as the brace's M_y/M_z (JointForceResolver projects both onto nb = ex × bx),
				// so calling them ip/op beside a y/z table would suggest two different planes where
				// there is one. The index says which member they belong to.
				// The "side" cell names the chord FACE this brace lands on — the thing that decides
				// which fibre σ_my is taken at (z = side·R below). It used to read "average", which
				// distinguished nothing (there is one row) and only repeated the sentence above. The
				// averaging is stated in the header instead, where it belongs: these three values
				// are already the mean of the chord's two loadings.
				sb.AppendLine("        <tr><th>chord face</th><th>N<sub>chord</sub> (avg)</th>"
					+ "<th>M<sub>y,chord</sub> (avg)</th><th>M<sub>z,chord</sub> (avg)</th></tr>");
				sb.AppendLine($"        <tr><td><b>{(st.Side >= 0 ? "+ey" : "&minus;ey")}</b></td>"
					+ $"<td>{N(st.NChord / 1e3, 1)} kN</td>"
					+ $"<td>{N(st.MipChord / 1e3, 2)} kN&middot;m</td>"
					+ $"<td>{N(st.MopChord / 1e3, 2)} kN&middot;m</td></tr>");
				sb.AppendLine("      </table>");

				Step(sb, "Chord section properties &mdash; CHS, thickness at the joint (p.31)",
					@"A=\dfrac{\pi}{4}(D^2-d_i^2),\quad I=\dfrac{\pi}{64}(D^4-d_i^4),\quad R=D/2",
					null,
					$@"A={N(aMm2, 0)}\,mm^2,\ I={N(iMm4 / 1e6, 1)}\times 10^6\,mm^4,\ R={N(rMm, 1)}\,mm");

				Step(sb, "&sigma;<sub>a</sub> &mdash; axial (+ tension)",
					@"\sigma_{a,Sd} = N_{chord}/A",
					$@"{N(st.NChord / 1e3, 1)}\,kN\ /\ {N(aMm2 / 1e3, 2)}\times10^3\,mm^2",
					$@"{N(sa, 1)}\,MPa");

				Step(sb, $"&sigma;<sub>my</sub> &mdash; in-plane bending, chord face "
					+ (st.Side >= 0 ? "+ey" : "&minus;ey") + " (z = side&middot;R), sign FLIPPED so "
					+ "+ = compression in the footprint (eq 6.54 note)",
					@"\sigma_{my,Sd} = -\dfrac{M_{y,chord}\cdot(\text{side}\cdot R)}{I}",
					$@"-\dfrac{{{N(st.MipChord / 1e3, 2)}\,kNm\cdot({(st.Side >= 0 ? "+" : "-")}1\cdot {N(rMm, 1)}\,mm)}}{{{N(iMm4 / 1e6, 1)}\times10^6\,mm^4}}",
					$@"{N(smy, 1)}\,MPa");

				Step(sb, "&sigma;<sub>mz</sub> &mdash; out-of-plane bending "
					+ "(sign irrelevant &mdash; enters Q<sub>f</sub> only squared, via A&sup2;)",
					@"\sigma_{mz,Sd} = \dfrac{M_{z,chord}\cdot R}{I}",
					$@"\dfrac{{{N(st.MopChord / 1e3, 2)}\,kNm\cdot {N(rMm, 1)}\,mm}}{{{N(iMm4 / 1e6, 1)}\times10^6\,mm^4}}",
					$@"{N(smz, 1)}\,MPa");
			}

			// ── A² and the moment resistances (shared by every class) ──
			sb.AppendLine("      <p class='deriv-h'>Chord utilisation A&sup2; &amp; moment resistance "
				+ "&mdash; &sect;6.4.3.2&ndash;4, eq (6.53)/(6.55)</p>");

			Step(sb, "Chord utilisation A&sup2; &mdash; eq (6.55) (shared by all classes)",
				@"A^2 = \left(\dfrac{\sigma_{a,Sd}}{f_{y,chord}}\right)^2 + \dfrac{\sigma_{my,Sd}^2+\sigma_{mz,Sd}^2}{1.62\,f_{y,chord}^2}",
				$@"\left(\dfrac{{{P(sa, 1)}}}{{{N(fy, 0)}}}\right)^2 + \dfrac{{{P(smy, 1)}^2+{P(smz, 1)}^2}}{{1.62\cdot {N(fy, 0)}^2}}",
				N(r.QfMomentA2, 4));

			// The coefficients come from the RESULT, not from PerClass[K].CAxial — those are the
			// AXIAL row of Table 6-4 and differ (K axial is C2 = 0.2, moment is C2 = 0). Reading them
			// from the class was how this line came to print a formula in symbols and no substitution.
			Step(sb, "Q<sub>f</sub>, moment &mdash; Table 6-4: ONE row for moment, no K/Y/X split"
				+ $" &mdash; C&#8321;={N(r.CMoment.C1, 2)}, C&#8322;={N(r.CMoment.C2, 2)}, "
				+ $"C&#8323;={N(r.CMoment.C3, 2)}",
				@"Q_f = 1 + C_1\dfrac{\sigma_{a,Sd}}{f_{y,chord}} - C_2\dfrac{\sigma_{my,Sd}}{1.62\,f_{y,chord}} - C_3\,A^2",
				$@"1 + {N(r.CMoment.C1, 2)}\cdot\dfrac{{{P(sa, 1)}}}{{{N(fy, 0)}}} - {N(r.CMoment.C2, 2)}\cdot\dfrac{{{P(smy, 1)}}}{{1.62\cdot {N(fy, 0)}}} - {N(r.CMoment.C3, 2)}\cdot {N(r.QfMomentA2, 4)}",
				N(r.QfMoment, 3));

			// THE SUBSTITUTED THICKNESS IS NOT ROUNDED, and the angle belongs to the governing pass.
			//
			// Two defects met on this line. `N(tChordMm, 0)` printed a 6.5 mm chord as `7`, and the
			// result depends on its SQUARE: `355 · 7² · 76 / (1.15 · 0.866) · 3.837 · 0.974` gives
			// 4961 where the printed result was 4.28 kN·m — with 6.5² it is 4278. 80 substitutions
			// in the reviewed report carried the rounded value and none the real one, so evaluating
			// any printed moment line overstated it by 16 %. `d` beside it was unrounded, which is
			// what made the line look checkable.
			//
			// And r.SinTheta is the ACTUAL angle while Q_u,ipb/opb come from whichever §6.4.3.1 pass
			// governs — see the N_Rd note below. Same fix: substitute the governing pass's sinθ and
			// name the angle.
			//
			// One decimal on the thickness, not zero: enough for 6.5, and a catalogue wall thickness
			// is never specified finer.
			bool momLimiting = r.LimitingPassApplied
				&& !double.IsNaN(r.NRdLimiting) && !double.IsNaN(r.NRdActual)
				&& r.NRdLimiting < r.NRdActual;
			double sinMom = momLimiting && !double.IsNaN(r.ThetaLimitingDeg)
				? Math.Sin(r.ThetaLimitingDeg * Math.PI / 180.0)
				: r.SinTheta;
			string momNote = momLimiting
				? $" &mdash; imposed &theta; = {N(r.ThetaLimitingDeg, 1)}&deg; (&sect;6.4.3.1)"
				: "";

			// THE BENDING Q_u FACTORS, WITH THEIR FORMULAS. They were bare numbers — 8.538, 5.374 —
			// and no formula for either appeared anywhere in the document, while on half the cards
			// the out-of-plane term is the LARGEST of eq (6.57)'s three. Table 6-3 gives one
			// expression each, shared by K, Y and X, so they belong here beside A² rather than
			// inside a per-class block.
			Step(sb, $"Q<sub>u,ipb</sub> &mdash; Table 6-3, in-plane bending "
				+ $"(all classes), &beta; = {N(r.Beta, 3)}, &gamma; = {N(r.Gamma, 2)}",
				@"Q_{u,ipb} = (5+0.7\gamma)\,\beta^{1.2}",
				$@"(5+0.7\cdot {N(r.Gamma, 2)})\cdot {N(r.Beta, 3)}^{{1.2}}",
				N(r.QuIpb, 3));

			Step(sb, $"Q<sub>u,opb</sub> &mdash; Table 6-3, out-of-plane bending "
				+ $"(all classes), &beta; = {N(r.Beta, 3)}, &gamma; = {N(r.Gamma, 2)}",
				@"Q_{u,opb} = 2.5+(4.5+0.2\gamma)\,\beta^{2.6}",
				$@"2.5+(4.5+0.2\cdot {N(r.Gamma, 2)})\cdot {N(r.Beta, 3)}^{{2.6}}",
				N(r.QuOpb, 3));

			Step(sb, "In-plane bending resistance M<sub>y,Rd</sub> &mdash; eq (6.53) "
				+ $"(Q<sub>u,ipb</sub> shared by all classes, Table 6-3){momNote}",
				@"M_{y,Rd} = \dfrac{f_{y,chord}\,T^2\,d}{\gamma_M \sin\theta}\,Q_{u,ipb}\,Q_{f,mom}",
				$@"\dfrac{{{N(fy, 0)}\cdot {N(tChordMm, 1)}^2\cdot {N(dMm, 0)}}}{{{N(inp.GammaM, 2)}\cdot {N(sinMom, 3)}}}\cdot {N(r.QuIpb, 3)}\cdot {N(r.QfMoment, 3)}",
				$@"{N(r.MRdIp / 1e3, 2)}\,kN\!\cdot\!m");

			Step(sb, $"Out-of-plane bending resistance M<sub>z,Rd</sub> &mdash; eq (6.53){momNote}",
				@"M_{z,Rd} = \dfrac{f_{y,chord}\,T^2\,d}{\gamma_M \sin\theta}\,Q_{u,opb}\,Q_{f,mom}",
				$@"\dfrac{{{N(fy, 0)}\cdot {N(tChordMm, 1)}^2\cdot {N(dMm, 0)}}}{{{N(inp.GammaM, 2)}\cdot {N(sinMom, 3)}}}\cdot {N(r.QuOpb, 3)}\cdot {N(r.QfMoment, 3)}",
				$@"{N(r.MRdOp / 1e3, 2)}\,kN\!\cdot\!m");

			// ── one block per ACTIVE mode. An inactive class is computed but plays no part in
			// this brace's check, and showing it would suggest it does.
			//
			// THE PREFACTOR BELONGS TO THE PASS THE RESISTANCES CAME FROM.
			//
			// r.SinTheta is always the brace's ACTUAL angle — Norsok64Engine keeps β/γ/θ from the
			// real geometry even when the §6.4.3.1 limiting pass governs the resistance, and rightly
			// so: the validity statement has to describe the real brace. But the Q_u and N_Rd below
			// then come from the clamped pass, so a prefactor built on the actual sinθ made the
			// printed line unreconcilable: a 20° brace printed `38.1 kN · 9.697 · 1.000 = 252.8 kN`,
			// whose factors give 369.5. 38.1 is the 20° prefactor; the result is the 30° pass's.
			//
			// So when the limiting pass governs, substitute ITS sinθ and say which angle it is. The
			// alternative — printing the actual-θ prefactor and the actual-θ resistance — would be a
			// different number from the one the check used, which is worse.
			bool limitingGoverns = r.LimitingPassApplied
				&& !double.IsNaN(r.NRdLimiting) && !double.IsNaN(r.NRdActual)
				&& r.NRdLimiting < r.NRdActual;
			double sinAx = limitingGoverns && !double.IsNaN(r.ThetaLimitingDeg)
				? Math.Sin(r.ThetaLimitingDeg * Math.PI / 180.0)
				: r.SinTheta;
			double baseAx = inp.FyChord * inp.T * inp.T / (inp.GammaM * sinAx);
			string axNote = limitingGoverns
				? $" &mdash; imposed &theta; = {N(r.ThetaLimitingDeg, 1)}&deg; (&sect;6.4.3.1 "
					+ "limiting pass, which governs here)"
				: "";

			// K: one sub-block per gap — a brace's K share is a SUM over its pairings, and the sum
			// alone cannot say whether it is one strong pairing or three weak ones
			if (cl.FrK > 1e-9 && r.KTerms.Count > 0)
			{
				sb.AppendLine($"      <p class='deriv-h'>Mode K &mdash; fraction of N<sub>Sd</sub> = "
					+ Pct2(cl.FrK) + (r.KTerms.Count > 1 ? $" (split over {r.KTerms.Count} gaps)" : "")
					+ "</p>");

				// Q_f FIRST, and once: it is the same for every gap of this brace (the chord stresses
				// and the Table 6-4 K coefficients do not vary per pairing), so it belongs above the
				// per-gap blocks rather than repeated inside each.
				//
				// It used to be absent altogether. The N_Rd line below promised Q_u,i · Q_f,K and
				// substituted only the first, so `15.1 kN · 16.425 = 241.9 kN` printed a product
				// whose printed factors give 248.0 — the 0.978 was applied and shown nowhere, while
				// Y and X printed theirs correctly. A reader multiplying what they saw got a
				// resistance 2.5 % high with nothing on the page to explain the gap.
				var kQf = r.PerClass.TryGetValue(Norsok64.Joint64Class.K, out var kc) ? kc : null;
				if (kQf != null)
					Step(sb, $"Q<sub>f</sub>, axial &mdash; class K, Table 6-4: "
						+ $"C&#8321;={N(kQf.CAxial.C1, 2)}, "
						+ $"C&#8322;={N(kQf.CAxial.C2, 2)}, C&#8323;={N(kQf.CAxial.C3, 2)}"
						+ (string.IsNullOrEmpty(kQf.CAxial.Note) ? "" : $" ({Esc(kQf.CAxial.Note)})"),
						@"Q_f = 1 + C_1\dfrac{\sigma_{a,Sd}}{f_{y,chord}} - C_2\dfrac{\sigma_{my,Sd}}{1.62\,f_{y,chord}} - C_3\,A^2",
						$@"1 + {N(kQf.CAxial.C1, 2)}\cdot\dfrac{{{P(sa, 1)}}}{{{N(fy, 0)}}} - {N(kQf.CAxial.C2, 2)}\cdot\dfrac{{{P(smy, 1)}}}{{1.62\cdot {N(fy, 0)}}} - {N(kQf.CAxial.C3, 2)}\cdot {N(kQf.QfAxialA2, 4)}",
						N(kQf.QfAxial, 3));

				for (int i = 0; i < r.KTerms.Count; i++)
				{
					var kt = r.KTerms[i];
					string lbl = r.KTerms.Count > 1 ? $"K{i + 1}" : "K";
					sb.AppendLine($"      <p class='deriv-note'><b>{lbl}</b> &mdash; {Pct(kt.FrK)} of "
						+ "N<sub>Sd</sub> balanced across this gap.</p>");
					// Q_g SHOWS ITS BRANCH AND ITS INPUTS.
					//
					// It was a heading and a value. Note (b) has three branches — g/D >= 0.05,
					// g/D <= -0.05, and a linear interpolation between their limiting values in
					// between — and the interpolation turns on phi = (t*fy,brace)/(T*fy,chord),
					// which appeared nowhere on the page. So two braces of ONE joint both printed
					// "g = 2 mm, g/D = 0.011" and then 1.188 and 1.810: a 52 % spread from input a
					// reader sees as identical, feeding Q_u directly. The gap is printed to a tenth
					// for the same reason — "2 mm" beside "g/D = 0.011" cannot both be right at
					// D = 141 (2/141 = 0.0142), because the displayed gap was rounded from 1.55.
					double gdI = kt.GapM / inp.D;
					double phiI = inp.T > 0.0 && inp.FyChord > 0.0
						? (inp.t * inp.FyBrace) / (inp.T * inp.FyChord)
						: 0.0;
					string qgBranch = gdI >= 0.05
						? @"Q_g = \max\{1 + 0.2(1-2.8\,g/D)^3,\ 1\}"
						: gdI <= -0.05
							? @"Q_g = 0.13 + 0.65\,\varphi\,\gamma^{0.5}"
							: @"Q_g = Q_g^{-} + (Q_g^{+} - Q_g^{-})\dfrac{g/D + 0.05}{0.10}"
								+ @"\quad\text{(interpolated)}";
					string qgSubst = gdI >= 0.05
						? $@"\max\{{1 + 0.2(1-2.8\cdot {N(gdI, 4)})^3,\ 1\}}"
						: $@"\varphi = \dfrac{{{N(inp.t * 1e3, 1)}\cdot {N(inp.FyBrace / 1e6, 0)}}}"
							+ $@"{{{N(inp.T * 1e3, 1)}\cdot {N(inp.FyChord / 1e6, 0)}}} = {N(phiI, 4)}"
							+ $@",\ \gamma = {N(r.Gamma, 2)},\ g/D = {N(gdI, 4)}";
					Step(sb, $"Q<sub>g</sub> &mdash; {lbl}, gap g = {N(kt.GapM * 1e3, 1)} mm, "
						+ $"g/D = {N(gdI, 4)} "
						+ $"&mdash; {(gdI >= 0.05 ? "gap branch" : gdI <= -0.05 ? "overlap branch" : "interpolated between the two limiting values")}"
						+ " (Table 6-3)",
						qgBranch, qgSubst, N(kt.Qg, 3));
					Step(sb, $"Q<sub>u,axial</sub> &mdash; {lbl}, Table 6-3, class K, "
						+ $"&beta; = {N(r.Beta, 3)}, &gamma; = {N(r.Gamma, 2)}",
						@"Q_u = \min\{(16+1.2\gamma)\beta^{1.2}Q_g,\ 40\beta^{1.2}Q_g\}",
						$@"\min\{{(16+1.2\cdot {N(r.Gamma, 2)})\cdot {N(r.Beta, 3)}^{{1.2}}\cdot {N(kt.Qg, 3)},\ 40\cdot {N(r.Beta, 3)}^{{1.2}}\cdot {N(kt.Qg, 3)}\}}",
						N(kt.QuAxial, 3));
					// All THREE factors the formula above names. The Q_f,K was applied and omitted
					// here, so the printed product did not give the printed result.
					Step(sb, $"N<sub>Rd</sub> &mdash; {lbl}, eq (6.52){axNote}",
						@"N_{Rd,i} = \dfrac{f_{y,chord}\,T^2}{\gamma_M \sin\theta}\,Q_{u,i}\,Q_{f,K}",
						kQf == null
							? $@"{N(baseAx / 1e3, 2)}\,kN\cdot {N(kt.QuAxial, 3)}"
							: $@"{N(baseAx / 1e3, 2)}\,kN\cdot {N(kt.QuAxial, 3)}\cdot {N(kQf.QfAxial, 3)}",
						$@"{N(kt.NRd / 1e3, 1)}\,kN");
				}
			}

			foreach (var (cls, frac) in new[]
			{
				(Norsok64.Joint64Class.Y, cl.FrY),
				(Norsok64.Joint64Class.X, cl.FrX),
			})
			{
				if (frac <= 1e-9 || !r.PerClass.TryGetValue(cls, out var c)) continue;
				string tension = r.LoadAxial == "tension" ? "tension" : "compression";
				sb.AppendLine($"      <p class='deriv-h'>Mode {cls} &mdash; fraction of "
					+ $"N<sub>Sd</sub> = {Pct2(frac)}</p>");
				Step(sb, $"Q<sub>f</sub>, axial &mdash; class {cls}, Table 6-4: "
					+ $"C&#8321;={N(c.CAxial.C1, 2)}, "
					+ $"C&#8322;={N(c.CAxial.C2, 2)}, C&#8323;={N(c.CAxial.C3, 2)}"
					+ (string.IsNullOrEmpty(c.CAxial.Note) ? "" : $" ({Esc(c.CAxial.Note)})"),
					@"Q_f = 1 + C_1\dfrac{\sigma_{a,Sd}}{f_{y,chord}} - C_2\dfrac{\sigma_{my,Sd}}{1.62\,f_{y,chord}} - C_3\,A^2",
					$@"1 + {N(c.CAxial.C1, 2)}\cdot\dfrac{{{P(sa, 1)}}}{{{N(fy, 0)}}} - {N(c.CAxial.C2, 2)}\cdot\dfrac{{{P(smy, 1)}}}{{1.62\cdot {N(fy, 0)}}} - {N(c.CAxial.C3, 2)}\cdot {N(c.QfAxialA2, 4)}",
					N(c.QfAxial, 3));
				// Q_beta, ON THE ONE BRANCH THAT CONSUMES IT, and it names which rule fired.
				//
				// X-compression is the only Table 6-3 entry with a Q_beta in it. The factor is
				// BRANCHED at beta = 0.6 and both branches occur in one document, so a bare value
				// would leave the reader unable to tell which applied — the same defect Q_g's block
				// above was rewritten to fix. It appeared nowhere at all until now: the formula
				// printed `·Q_β` and the symbol was never bound to a number.
				bool needsQBeta = cls == Norsok64.Joint64Class.X && r.LoadAxial != "tension";
				if (needsQBeta)
					Step(sb, $"Q<sub>&beta;</sub> &mdash; Table 6-3, &beta; = {N(r.Beta, 3)} "
						+ $"{(r.Beta > 0.6 ? "&gt;" : "&le;")} 0.6",
						r.Beta > 0.6
							? @"Q_\beta = \dfrac{0.3}{\beta(1-0.833\beta)}"
							: @"Q_\beta = 1.0",
						r.Beta > 0.6
							? $@"\dfrac{{0.3}}{{{N(r.Beta, 3)}\cdot(1-0.833\cdot {N(r.Beta, 3)})}}"
							: null,
						N(r.QBeta, 3));

				// AND THE SUBSTITUTION. Y and X printed the formula in symbols and then the result —
				// 43 of the 64 axial blocks in the reviewed report — while the K block twenty lines
				// above substituted its own. Q_u is the largest factor in every resistance, so this
				// was the biggest unreachable number on the page.
				string quFormula = cls == Norsok64.Joint64Class.Y
					? (r.LoadAxial == "tension" ? @"Q_u = 30\beta"
						: @"Q_u = \min\{2.8+(20+0.8\gamma)\beta^{1.6},\ 2.8+36\beta^{1.6}\}")
					: (r.LoadAxial == "tension" ? @"Q_u = 6.4\,\gamma^{0.6\beta^2}"
						: @"Q_u = (2.8+(12+0.1\gamma)\beta)\,Q_\beta");
				string b3 = N(r.Beta, 3), g2 = N(r.Gamma, 2);
				string quSubst = cls == Norsok64.Joint64Class.Y
					? (r.LoadAxial == "tension" ? $@"30\cdot {b3}"
						: $@"\min\{{2.8+(20+0.8\cdot {g2})\cdot {b3}^{{1.6}},\ 2.8+36\cdot {b3}^{{1.6}}\}}")
					: (r.LoadAxial == "tension" ? $@"6.4\cdot {g2}^{{0.6\cdot {b3}^2}}"
						: $@"(2.8+(12+0.1\cdot {g2})\cdot {b3})\cdot {N(r.QBeta, 3)}");
				Step(sb, $"Q<sub>u,axial</sub> &mdash; Table 6-3, class {cls} (brace in {tension}), "
					+ $"&beta; = {b3}, &gamma; = {g2}",
					quFormula, quSubst, N(c.QuAxial, 3));
				Step(sb, $"N<sub>Rd</sub> &mdash; eq (6.52){axNote}",
					@"N_{Rd} = \dfrac{f_{y,chord}\,T^2}{\gamma_M \sin\theta}\,Q_u\,Q_f",
					$@"{N(baseAx / 1e3, 2)}\,kN\cdot {N(c.QuAxial, 3)}\cdot {N(c.QfAxial, 3)}",
					$@"{N(c.NRd / 1e3, 1)}\,kN");
			}

			// ── the weighted axial resistance across whichever modes are active ──
			var active = new[]
			{
				(Norsok64.Joint64Class.K, cl.FrK),
				(Norsok64.Joint64Class.Y, cl.FrY),
				(Norsok64.Joint64Class.X, cl.FrX),
			}.Where(x => x.Item2 > 1e-9).ToList();

			if (active.Count > 0)
			{
				sb.AppendLine("      <p class='deriv-h'>Weighted axial resistance (all active modes) "
					+ "&mdash; &sect;6.4.3.2</p>");
				sb.AppendLine("      <table class='deriv-table'>");
				sb.AppendLine("        <tr><th>mode</th><th>fraction</th><th>N<sub>Rd,mode</sub></th></tr>");
				foreach (var (cls, frac) in active)
					sb.AppendLine($"        <tr><td>{cls}</td><td>{Pct2(frac)}</td>"
						+ $"<td>{N(r.PerClass[cls].NRd / 1e3, 1)} kN</td></tr>");
				sb.AppendLine("      </table>");
				// A WEIGHTED AVERAGE, which is what the clause says and what the engine computes.
				//
				// This printed the HARMONIC form — `1/N_Rd = Σ fr/N_Rd,mode`, substituted the same
				// way — above an ARITHMETIC result. Measured on a three-mode brace:
				// 0.190/367.6 + 0.380/368.5 + 0.430/236.9 gives 297.3, and the printed figure was
				// 311.7, which is 0.190·367.6 + 0.380·368.5 + 0.430·236.9 exactly. The spread
				// reaches 22 % on other braces, so this was not a rounding quibble.
				//
				// The standard settles it, N-004 §6.4.3.2 page 30: "a weighted average of N_Rd
				// based on the portion of each in the total action is used to calculate the
				// resistance." A weighted average is the sum of fr·N_Rd. The engine was right and
				// the formula was wrong.
				//
				// Cited to §6.4.3.2, which is the normative sentence; it used to point at
				// Comm. 6.4.2, where that wording is not.
				Step(sb, "Weighted axial resistance &mdash; &sect;6.4.3.2 (mixture of K/Y/X)",
					@"N_{Rd} = \sum_{\text{mode}} fr_{\text{mode}} \cdot N_{Rd,\text{mode}}",
					string.Join(" + ", active.Select(x =>
						$@"{N(x.Item2, 3)}\cdot {N(r.PerClass[x.Item1].NRd / 1e3, 1)}")),
					$@"{N(r.NRdWeighted / 1e3, 1)}\,kN");
			}
			else
			{
				sb.AppendLine("      <p class='deriv-h'>Axial resistance</p>");
				sb.AppendLine("      <p class='deriv-note'>No K/Y/X classification for this brace "
					+ "(no axial force) &mdash; the axial term of eq. (6.57) is not applicable; only "
					+ "the bending check below applies.</p>");
			}

			// ══ 4. FINAL VERDICT ON CAPACITY ══ eq (6.57).
			//
			// Last, because it is the last line of the hand calculation this page exists to be checked
			// against: every quantity in it — N_Rd, M_Rd,ip, M_Rd,op — was derived in the blocks above,
			// and a reader verifying the sum has to have read them first. Putting it at the top made
			// the page a claim followed by its justification rather than a calculation to follow
			// through. (The number is also in the header, which is identity rather than verdict: the
			// reader clicked a row that already showed it.)
			sb.AppendLine("      <p class='deriv-h'>Utilisation &mdash; eq (6.57)</p>");
			var dom = r.PerClass.TryGetValue(
				Enum.TryParse<Norsok64.Joint64Class>(row.DomClass, out var dc) ? dc : Norsok64.Joint64Class.K,
				out var dr) ? dr : null;

			// THE AXIAL TERM COMES FROM THE WEIGHTED RESISTANCE — the same N_Rd the total uses.
			//
			// It used to print dom.UtilAxialTerm, which is N_Sd / N_Rd(dominant mode), while the
			// total beside it is the engine's weighted sum. On a MULTI-MODE brace those are
			// different numbers, so the three printed terms did not add up to the printed total:
			// the governing brace of the reviewed report showed 32.44 + 1.43 + 47.30 = 73.73 %,
			// three figures that sum to 81.17. 21 of that report's 40 checks were affected and the
			// error ran both ways (+7.44 pp and -5.36 pp), so a reader could not even assume the
			// breakdown erred safely. The 19 single-mode checks balanced, which is exactly why it
			// survived: on those two resistances coincide.
			//
			// The dominant mode's own term is not lost — the per-mode table above prints each
			// mode's resistance, which is where a reader looks for it.
			double axialTerm = r.NRdWeighted > 0.0 && row.Inputs != null
				? Math.Abs(row.Inputs.NSd) / r.NRdWeighted
				: 0.0;

			// The step's label says what the three terms ARE, not the heading again. Both used to read
			// "Utilisation — eq (6.57)", printing it twice in immediate succession — 60 times in a
			// 30-check report. The HEADING is the half that stays: it anchors the four-phase order the
			// sheet is built on (DerivationContentTests asserts the sequence by position).
			Step(sb, "Sum of the three interaction terms &mdash; axial, in-plane, out-of-plane",
				@"u = \dfrac{N_{Sd}}{N_{Rd}} + \left(\dfrac{M_{y,Sd}}{M_{y,Rd}}\right)^2 + \left|\dfrac{M_{z,Sd}}{M_{z,Rd}}\right|",
				dom == null ? null
					: $@"{N(axialTerm * 100, 2)}\% + {N(dom.UtilIpTerm * 100, 2)}\% + {N(dom.UtilOpTerm * 100, 2)}\%",
				$@"{N(row.Util * 100, 2)}\%\ \ \text{{{(row.Passed ? "PASS" : "FAIL")}}}");

			if (r.ChordOverstressed)
				sb.AppendLine("      <p class='deriv-warn'>&#9940; CHORD OVERSTRESSED: Q<sub>f</sub> "
					+ "(eq 6.54, no floor in the norm) drove an active resistance to &le; 0 &mdash; the "
					+ "check is forced to FAIL regardless of the utilisation sum (app-level safety "
					+ "rule).</p>");

			sb.AppendLine("    </div>");
		}

		/// <summary>
		/// Where the checked forces came from: the joint plane, the chord, and the transformation.
		///
		/// THE gap a reviewer holding the IDEA StatiCa model hit hardest. §6.4 is not evaluated on
		/// the load effects as the application shows them — the tool resolves a joint plane,
		/// identifies the through chord, classifies each brace into K/Y/X fractions and projects the
		/// member forces into that plane. The report printed only the OUTPUT of that pipeline, under
		/// a heading that said "in the joint plane" and nothing more, so not one force in the
		/// document could be reconciled against anything visible in Connection.
		///
		/// Once per connection, not once per brace: the plane, the chord and the frame are properties
		/// of the JOINT. The same reasoning removes the per-brace repetition the review measured —
		/// the chord's section properties and stresses were printed identically on pages 6 and 20 of
		/// the shipped report.
		///
		/// Reports what was RESOLVED, and computes nothing: every number here is already in the
		/// topology, so this section cannot disagree with the checks below it.
		/// </summary>
		/// <param name="chapterRows">
		/// This chapter's rows for this connection — the source of each brace's GOVERNING load
		/// effect. Without them the force table could only show an arbitrary state, which is what it
		/// did (the first one) and why none of its numbers matched the checks below it.
		/// </param>
		private static void RenderJointPlane(StringBuilder sb, Norsok64.JointTopology topo,
			IReadOnlyList<NorsokFormulaResult>? chapterRows = null)
		{
			// InvariantCulture, like every other number in this report. Written without it first,
			// and the test caught it immediately: on this cs-CZ machine the normal came out as
			// "+0,577" while the forces beside it used points. The same slip as the utilisation
			// percentages, made again ten minutes after fixing those.
			static string F(double x, string fmt) =>
				x.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture);
			static string V(Norsok64.Vec3 v) =>
				$"({F(v.X, "+0.000;-0.000;0.000")}, {F(v.Y, "+0.000;-0.000;0.000")}, "
				+ $"{F(v.Z, "+0.000;-0.000;0.000")})";
			static string N(double x, int d = 1) => double.IsNaN(x) || double.IsInfinity(x)
				? "&mdash;"
				: x.ToString("F" + d, System.Globalization.CultureInfo.InvariantCulture);

			// TWO purposes, one block. On an assessed joint this says where the checked forces came
			// from; on a REJECTED one it says how the chapter reached that verdict — the conditions
			// on the card quote measured numbers ("gap -16 mm", "20.0° off plane (>15°)", "2
			// continuous members"), and without the geometry they were measured from, a reader is
			// asked to take the rejection on trust while every assessed joint shows its workings.
			//
			// The forces are omitted when nothing was assessed: none were resolved, and inventing a
			// state to display would be the contradiction this block had before.
			bool assessed = chapterRows?.Any(f => !f.IsNote && !f.NotAssessed) == true;

			sb.AppendLine("<div class='deriv-block'>");
			if (assessed)
			{
				// No note under this heading. It used to carry one announcing "how the plane and
				// chord were determined, and both sets of forces side by side" — which the heading
				// already implies, 140 lines before the table that delivers it, and that table's
				// own note points at chapter 3 where the reader is actually looking.
				sb.AppendLine("  <p class='deriv-h'>Joint plane and force transformation</p>");
			}
			else
			{
				sb.AppendLine("  <p class='deriv-h'>How the joint was read &mdash; the basis of the "
					+ "conditions below</p>");
				sb.AppendLine("  <p class='deriv-note'>No &sect;6.4 check was performed on this joint, "
					+ "so no forces were resolved. What follows is the geometry "
					+ "the chapter measured and the chord it identified &mdash; the numbers the "
					+ "unmet conditions below are read from, given so the verdict can be checked "
					+ "rather than taken on trust.</p>");
			}

			// ── the plane and its frame ──
			sb.AppendLine("  <table class='deriv-table'>");
			// The chord, and whether naming it was a MEASUREMENT or a CHOICE.
			//
			// On an ambiguous joint the builder picks "the largest Ø" among the continuous members
			// (JointTopologyBuilder.cs:63) — a tie-break, not a fact about the model. Printed bare,
			// as it was, the row read as certainty directly above a condition saying "2 continuous
			// members — the chord is ambiguous", which is the document disagreeing with itself.
			bool chordAmbiguous = chapterRows?.Any(f =>
				(f.CheckExpression ?? "").Contains("chord is ambiguous",
					StringComparison.OrdinalIgnoreCase)) == true;
			Kv(sb, "chord (through member)", topo.Chord == null
				? "&mdash; none identified"
				: $"<b>{Esc(topo.Chord.Name ?? "?")}</b>"
					+ (topo.Chord.Section?.Name is { Length: > 0 } cs ? $" &mdash; {Esc(cs)}" : "")
					+ (chordAmbiguous
						? " <span class='deriv-hint'>(ambiguous &mdash; taken as the largest &oslash; "
							+ "of the continuous members; see the conditions below)</span>"
						: ""));
			Kv(sb, "plane normal <span class='deriv-hint'>(model coordinates)</span>", V(topo.NPlane));
			Kv(sb, "chord axis e<sub>x</sub>", V(topo.Ex));
			Kv(sb, "in-plane axis e<sub>y</sub>", V(topo.Ey));
			// The tolerance is typeset HERE, from the number, and labelled as ours. The engine used to
			// bake "within 2deg" into the sentence: ASCII in a document that typesets ≤ and °, and
			// printed beside real clause references with nothing saying §6.4 does not specify it.
			Kv(sb, "how the plane was fixed",
				(topo.PlaneFitBasis is { Length: > 0 } basis ? Esc(basis) : "&mdash;")
				+ (topo.PlaneFitTolDeg > 0
					? $" <span class='deriv-hint'>(within {N(topo.PlaneFitTolDeg, 1)}&deg; "
						+ "&mdash; tool tolerance, not a &sect;6.4 requirement)</span>"
					: ""));
			// WHERE the plane sits, when it is not on the work point. Reported because it is a real
			// feature of the model — the joint is displaced — and because a reader tracing the
			// geometry will otherwise find eccentricities in the model that appear nowhere here.
			//
			// Stated, not judged: a rigid displacement is not a defect (it changes no force — the
			// residual moved by 0.0001 kN over 15 load effects when measured), and §6.4 gives no
			// limit for it. What IS judged is each brace's distance FROM this plane, in the
			// geometry table below.
			if (Math.Abs(topo.PlaneOffsetM) > 1e-6)
				Kv(sb, "plane offset from the work point",
					$"{N(Math.Abs(topo.PlaneOffsetM) * 1e3, 1)} mm "
					+ "<span class='deriv-hint'>along the plane normal &mdash; the whole joint is "
					+ "displaced; brace eccentricities below are measured from THIS plane</span>");
			if (!topo.Coplanar || topo.PlaneSpread > 0)
				Kv(sb, "out-of-plane spread",
					// PlaneSpread is the scatter of the brace DIRECTIONS (unit vectors from
					// DominantDirection), so it is dimensionless — it used to print "mm".
					$"{N(topo.PlaneSpread, 4)} <span class='deriv-hint'>(direction scatter, "
					+ "dimensionless)</span>"
					+ (topo.Coplanar ? "" : " <span class='deriv-hint'>(not coplanar)</span>"));
			sb.AppendLine("  </table>");

			if (!string.IsNullOrEmpty(topo.PlaneWarn))
				sb.AppendLine($"  <p class='deriv-warn'>&#9888; {Esc(topo.PlaneWarn)}</p>");

			// ── one geometry row per brace, instead of the same numbers inside every check ──
			if (topo.BracesMeta.Count > 0)
			{
				sb.AppendLine("  <p class='deriv-h'>Members &mdash; geometry at the joint</p>");
				// The last two columns are CHECKS against the fitted plane, not inputs to the
				// resistance, and the table used to present all seven alike. A reviewer read the 8°
				// in "off-plane" as a quantity the projection uses — reasonably, sitting between θ
				// and β, which it does use — and then found two connections differing only in that
				// column with identical force tables. Naming the two groups is the fix; see the note
				// under the transformation table for why the angle cannot change those forces.
				sb.AppendLine("  <table class='deriv-table'>");
				sb.AppendLine("    <tr><th rowspan='2'>member</th><th rowspan='2'>section</th>"
					+ "<th colspan='3'>used by the check</th>"
					+ "<th colspan='2'>coplanarity checks (tool tolerances)</th></tr>");
				// The last column holds BraceMeta.OopOffsetM — the brace's distance FROM the joint
				// plane through the chord — and was headed "ecc. along chord", which is a different
				// quantity (the offset measured ALONG the chord axis, the one the D/4 warning is
				// about). So the table and its own rejection message disagreed: a reader saw 40 mm
				// under "along chord" beside a condition reading "40 mm out of the joint plane
				// through the chord", with no way to tell they were the same number.
				sb.AppendLine("    <tr><th>&theta;</th><th>&beta;</th><th>chord face</th>"
					+ "<th>off-plane</th><th>offset from joint plane</th></tr>");
				foreach (var b in topo.BracesMeta)
				{
					sb.AppendLine($"    <tr><td><b>{Esc(b.Name)}</b></td>"
						+ $"<td>{Esc(b.Section?.Name ?? "&mdash;")}</td>"
						+ $"<td>{N(b.ThetaDeg, 1)}&deg;</td>"
						+ $"<td>{(b.Beta is { } be ? N(be, 3) : "&mdash;")}</td>"
						+ $"<td>{(b.Side >= 0 ? "+ey" : "&minus;ey")}</td>"
						+ $"<td>{N(b.CoplanarDevDeg, 1)}&deg;</td>"
						+ $"<td>{N(b.OopOffsetM * 1e3, 1)} mm</td></tr>");
				}
				sb.AppendLine("  </table>");
			}

			// ── the transformation, per brace, for the state that GOVERNS that brace ──
			//
			// Not the first load effect. It was, with the excuse that "the arithmetic is the same for
			// every state" — which argues for showing SOME state, not for showing the first one. The
			// result was a table of LE1 forces above checks evaluated on LE9, LE12 and whatever else
			// each brace's envelope picked: none of the numbers a reader wanted to trace appeared
			// anywhere, which is the exact failure this section exists to fix.
			//
			// Each row now names its own state, taken from the check row's GovLeId — the same
			// envelope decision the derivation and the results table use, so the three cannot
			// disagree. A brace whose check row has no governing state (nothing assessed on it) is
			// left out rather than filled from an arbitrary one.
			var governing = new List<(string Brace, string State, Norsok64.BraceForceRow Row)>();
			foreach (var fr in chapterRows ?? Array.Empty<NorsokFormulaResult>())
			{
				var det = fr.JointDetail;
				if (fr.IsNote || fr.NotAssessed || det == null || string.IsNullOrEmpty(det.Name))
					continue;

				var le = topo.BraceForces.FirstOrDefault(p => p.Id == det.GovLeId)
					?? topo.BraceForces.FirstOrDefault();
				var row = le?.Rows.FirstOrDefault(x => x.Name == det.Name);
				if (le == null || row == null) continue;

				governing.Add((det.Name, det.GovLeName ?? le.Name ?? $"LE{le.Id}", row));
			}

			if (governing.Count > 0)
			{
				sb.AppendLine("  <p class='deriv-h'>Force transformation &mdash; each brace at its "
					+ "governing load effect</p>");
				// One line. The conventions, the sub-plane frame and why an off-plane deviation cannot
				// appear in its own projection are all in the method chapter now — they were repeated
				// under every assessed connection, and three of the paragraphs had just been added.
				sb.AppendLine("  <p class='deriv-note'>Left: the member loading in its own local axes, "
					+ "as the model carries it. Right: the same loading resolved into the brace's own "
					+ "chord&ndash;brace sub-plane, which is what &sect;6.4 checks. Each row is the "
					+ "state that GOVERNS that brace. Conventions and frame: chapter 3.</p>");

				int offPlane = topo.BracesMeta.Count(b => Math.Abs(b.CoplanarDevDeg) > 0.05);
				if (topo.BracesMeta.Count > 0)
					sb.AppendLine($"  <p class='deriv-note'>In this joint "
						+ $"<b>{topo.BracesMeta.Count - offPlane} of {topo.BracesMeta.Count}</b> "
						+ "braces lie in the fitted plane to within 0.1&deg;. For those, the two halves "
						+ "of the table differ by a relabelling and the sign convention above rather "
						+ "than by arithmetic &mdash; worth knowing before reading the side-by-side "
						+ "columns as evidence of a computation.</p>");
				// SHEAR AND TORSION ARE PRINTED, in the model half of the table.
				//
				// The method chapter says the other actions "are listed with each brace's forces so
				// their magnitude can be seen" — and they were not: V_y, V_z and M_x appeared nowhere
				// in the document. That made the chapter meant to stand on its own the one place
				// stating something untrue about the report.
				//
				// They belong in the model half, not the resolved half: §6.4 does not project them
				// and eq (6.57) has three terms. A reader who sees them in IDEA StatiCa looks for
				// them here, and their absence reads as an omission rather than as a scope decision
				// — so the honest fix is to satisfy the sentence instead of deleting it.
				// "resolved into the JOINT plane" is what this said, and it contradicted the method
				// chapter's own paragraph: each brace is resolved in the plane of ITS chord-brace
				// pair, and the fitted joint plane only classifies K/Y/X and fixes the sign of M_y.
				// The heading asserted the reading the chapter exists to refute.
				sb.AppendLine("  <table class='deriv-table'>");
				sb.AppendLine("    <tr><th rowspan='2'>member</th><th rowspan='2'>governing</th>"
					+ "<th colspan='6'>from the model (local axes)</th>"
					+ "<th colspan='3'>resolved into the chord&ndash;brace plane</th></tr>");
				sb.AppendLine("    <tr><th>N</th><th>V<sub>y</sub></th><th>V<sub>z</sub></th>"
					+ "<th>M<sub>x</sub></th><th>M<sub>y,loc</sub></th><th>M<sub>z,loc</sub></th>"
					+ "<th>N<sub>Sd</sub></th><th>M<sub>y,Sd</sub></th><th>M<sub>z,Sd</sub></th></tr>");
				foreach (var (brace, state, f) in governing)
				{
					sb.AppendLine($"    <tr><td><b>{Esc(brace)}</b></td><td>{Esc(state)}</td>"
						+ $"<td>{N(f.LocalN / 1e3, 1)} kN</td>"
						+ $"<td class='not-checked'>{N(f.LocalVy / 1e3, 1)} kN</td>"
						+ $"<td class='not-checked'>{N(f.LocalVz / 1e3, 1)} kN</td>"
						+ $"<td class='not-checked'>{N(f.LocalMx / 1e3, 2)} kN&middot;m</td>"
						+ $"<td>{N(f.LocalMy / 1e3, 3)} kN&middot;m</td>"
						+ $"<td>{N(f.LocalMz / 1e3, 3)} kN&middot;m</td>"
						+ $"<td>{N(f.NSd / 1e3, 1)} kN</td>"
						+ $"<td>{N(f.Mip / 1e3, 3)} kN&middot;m</td>"
						+ $"<td>{N(f.Mop / 1e3, 3)} kN&middot;m</td></tr>");
				}
				sb.AppendLine("  </table>");
				// What is LOCAL is which columns are the unchecked ones. Why they are unchecked, and
				// where they must be verified instead, is chapter 3's — this note used to give all
				// three propositions and so restated that paragraph in full under every connection.
				sb.AppendLine("  <p class='deriv-note'>V<sub>y</sub>, V<sub>z</sub> and M<sub>x</sub> "
					+ "are shown for completeness and are <b>not</b> checked here (chapter 3).</p>");

				RenderStateSelection(sb, chapterRows);
			}

			sb.AppendLine("</div>");
		}

		/// <summary>
		/// How the governing state was chosen, and by what margin.
		///
		/// Two things the report could not answer. First, the CRITERION: "governing" is not the
		/// largest force, because N_Rd depends on Q_f, Q_f on the chord stresses, and those on the
		/// load effect — so every candidate state has its own resistance and the winner comes out of
		/// a search in which the resistance is recomputed per state. Until that is stated the
		/// selection is not reproducible even by a reader holding every number in the document.
		///
		/// Second, the MARGIN. A 0.3-point gap to the next state means a small change to the model
		/// hands the joint to a different one; a 30-point gap means it does not. That is what a
		/// reviewer wants from an envelope, and it cannot be recovered from a dump of every state.
		///
		/// One column per brace, so it does not grow with the number of load effects — a model may
		/// hold arbitrarily many.
		/// </summary>
		private static void RenderStateSelection(StringBuilder sb,
			IReadOnlyList<NorsokFormulaResult>? chapterRows)
		{
			var rows = (chapterRows ?? Array.Empty<NorsokFormulaResult>())
				.Where(fr => !fr.IsNote && !fr.NotAssessed && fr.JointDetail is { Name.Length: > 0 })
				.Select(fr => fr.JointDetail!)
				.ToList();
			if (rows.Count == 0) return;

			sb.AppendLine("  <p class='deriv-h'>Governing state, and by what margin</p>");
			// The criterion is in chapter 3. What is per-connection is the MARGIN — how close the
			// decision was on each brace of THIS joint.
			sb.AppendLine("  <p class='deriv-note'>&Delta; is the gap in percentage points to the "
				+ "next state. A small margin means a change to the model could hand this brace to a "
				+ "different state; see chapter 3 for why the state with the largest force is not "
				+ "necessarily the governing one.</p>");
			sb.AppendLine("  <table class='deriv-table'>");
			sb.AppendLine("    <tr><th>member</th><th>governing</th><th>utilisation</th>"
				+ "<th>runner-up</th><th>&Delta;</th></tr>");

			foreach (var det in rows)
			{
				string util = double.IsNaN(det.Util) || double.IsInfinity(det.Util)
					? "&mdash;" : Pct(det.Util);

				string runner, delta;
				if (det.RunnerUpUtil is { } ru && det.RunnerUpLeName is { Length: > 0 } rn)
				{
					runner = $"{Esc(rn)} &nbsp;{Pct(ru)}";
					// Percentage POINTS, and said so: a "Δ 2.3 %" beside two percentages invites the
					// reader to take it as a relative difference.
					delta = double.IsNaN(det.Util) || double.IsInfinity(det.Util)
						? "&mdash;"
						: ((det.Util - ru) * 100).ToString("0.0",
							System.Globalization.CultureInfo.InvariantCulture) + " pp";
				}
				else
				{
					// Three different facts, and a bare dash for all of them would be the CON10
					// mistake again — one symbol standing for several situations a reader acts on
					// differently.
					runner = det.RunnerUpAbsence switch
					{
						Norsok64.JointEnvelope.RunnerUpAbsence.SingleState =>
							"<span class='deriv-hint'>only one load effect was evaluated</span>",
						Norsok64.JointEnvelope.RunnerUpAbsence.OthersSkipped =>
							"<span class='deriv-hint'>no other state produced a check on this brace</span>",
						_ => "&mdash;",
					};
					delta = "&mdash;";
				}

				sb.AppendLine($"    <tr><td><b>{Esc(det.Name)}</b></td>"
					+ $"<td>{Esc(det.GovLeName ?? $"LE{det.GovLeId}")}</td>"
					+ $"<td>{util}</td><td>{runner}</td><td>{delta}</td></tr>");
			}
			sb.AppendLine("  </table>");
		}

		/// <summary>A key/value row in a derivation table.</summary>
		private static void Kv(StringBuilder sb, string key, string value) =>
			sb.AppendLine($"        <tr><td class='deriv-k'>{key}</td><td>{value}</td></tr>");

		/// <summary>
		/// One derivation step: what is being computed, the formula, the numbers put into it, and the
		/// result. The python reference's `step()`, and the reason its sheet can be checked — a result
		/// on its own cannot be verified, and a symbolic formula on its own does not say what went in.
		///
		/// <paramref name="substituted"/> may be null when the substitution would only repeat the
		/// symbolic form (a table lookup, say) rather than show anything.
		/// </summary>
		private static void Step(StringBuilder sb, string label, string symbolic,
			string? substituted, string result)
		{
			sb.AppendLine("      <div class='deriv-step'>");
			sb.AppendLine($"        <div class='deriv-step-label'>{label}</div>");
			sb.AppendLine($"        <div class='deriv-step-math'>$${symbolic}$$</div>");
			if (!string.IsNullOrEmpty(substituted))
				sb.AppendLine($"        <div class='deriv-step-math'>$$=\\;{substituted}$$</div>");
			sb.AppendLine($"        <div class='deriv-step-res'>$$=\\;{result}$$</div>");
			sb.AppendLine("      </div>");
		}

		/// <summary>Convert variable symbol names to KaTeX notation.</summary>
		private static string SymbolToKatex(string symbol)
		{
			return symbol
				.Replace("γ_M", @"\gamma_M")
				.Replace("σ_vM", @"\sigma_{vM}")
				.Replace("σ_Ed", @"\sigma_{Ed}")
				.Replace("σ_⊥", @"\sigma_{\perp}")
				.Replace("σ_w", @"\sigma_w")
				.Replace("σ_max", @"\sigma_{max}")
				.Replace("τ_⊥", @"\tau_{\perp}")
				.Replace("τ_∥", @"\tau_{\parallel}")
				.Replace("τ_T,Sd", @"\tau_{T,Sd}")
				.Replace("τ/f_d", @"\tau / f_d")
				.Replace("ε_max", @"\varepsilon_{max}")
				.Replace("λ_s", @"\lambda_s")
				.Replace("λ", @"\lambda")
				.Replace("β_w", @"\beta_w")
				.Replace("N_Sd", @"N_{Sd}")
				.Replace("N_Ey", @"N_{Ey}")
				.Replace("N_Ez", @"N_{Ez}")
				.Replace("N_t,Rd", @"N_{t,Rd}")
				.Replace("N_c,Rd", @"N_{c,Rd}")
				.Replace("N_cl,Rd", @"N_{cl,Rd}")
				.Replace("M_Sd", @"M_{Sd}")
				.Replace("M_Rd", @"M_{Rd}")
				.Replace("M_Red,Rd", @"M_{Red,Rd}")
				.Replace("M_y,Sd", @"M_{y,Sd}")
				.Replace("M_z,Sd", @"M_{z,Sd}")
				.Replace("M_T,Sd", @"M_{T,Sd}")
				.Replace("M_T,Rd", @"M_{T,Rd}")
				.Replace("V_Sd", @"V_{Sd}")
				.Replace("V_Rd", @"V_{Rd}")
				.Replace("F_t,Sd", @"F_{t,Sd}")
				.Replace("F_v,Sd", @"F_{v,Sd}")
				.Replace("F_t,Rd", @"F_{t,Rd}")
				.Replace("F_v,Rd", @"F_{v,Rd}")
				.Replace("C_my", @"C_{my}")
				.Replace("C_mz", @"C_{mz}")
				.Replace("C_e", @"C_e")
				.Replace("f_y", @"f_y")
				.Replace("f_u", @"f_u")
				.Replace("f_c", @"f_c")
				.Replace("f_m", @"f_m")
				.Replace("f_d", @"f_d")
				.Replace("f_cl", @"f_{cl}")
				.Replace("f_cle", @"f_{cle}")
				.Replace("f_E", @"f_E")
				.Replace("f_m,Red", @"f_{m,Red}")
				.Replace("f_w,Rd", @"f_{w,Rd}")
				.Replace("I_p", @"I_p")
				.Replace("kl/i", @"kl/i")
				.Replace("Z/W", @"Z/W")
				.Replace("(N/N_t)^1.75", @"(N/N_t)^{1.75}")
				.Replace("√(M²y+M²z)/M_Rd", @"\sqrt{M_y^2+M_z^2}/M_{Rd}")
				.Replace("f_y·D/(E·t)", @"f_y \cdot D / (E \cdot t)")
				.Replace("f_y/f_cle", @"f_y / f_{cle}")
				.Replace("0.4·V_Rd", @"0.4 \cdot V_{Rd}")
				.Replace("UC_tension", @"\text{UC}_{tension}")
				.Replace("UC_shear", @"\text{UC}_{shear}")
				.Replace("Interaction", @"\text{Interaction}")
				.Replace("Axial term", @"\text{Axial term}")
				.Replace("Moment term", @"\text{Moment term}")
				.Replace("Allowable", @"\text{Allowable}")
				.Replace("Assembly", @"\text{Assembly}")
				.Replace("Resistance", @"\text{Resistance}")
				.Replace("Class", @"\text{Class}")
				.Replace("LC", @"\text{LC}")
				.Replace("Qu_axial", @"Q_{u,axial}")
				.Replace("Qu_IPB", @"Q_{u,IPB}")
				.Replace("Qu_OPB", @"Q_{u,OPB}")
				.Replace("Qf_axial", @"Q_{f,axial}")
				.Replace("Qf_moment", @"Q_{f,moment}")
				.Replace("N/N_Rd", @"N_{Sd}/N_{Rd}")
				.Replace("(My/MyRd)²", @"(M_y/M_{y,Rd})^2")
				.Replace("Mz/MzRd", @"M_z/M_{z,Rd}")
				.Replace("N_Rd", @"N_{Rd}")
				.Replace("θ", @"\theta")
				.Replace("β", @"\beta")
				.Replace("γ", @"\gamma")
				.Replace("τ", @"\tau");
		}

		private static string Esc(string s) => System.Net.WebUtility.HtmlEncode(s);

		/// <summary>
		/// A fraction as a percentage to TWO decimals — the K/Y/X split and the per-mode fractions.
		///
		/// Same reason as <see cref="Pct"/>, and these were the larger half of the problem: measured
		/// 126 comma decimals in one printed report ("100,00", "0,7370") beside the points used
		/// everywhere else, all from `:P2`/`:P1` interpolations carrying no culture. They predate
		/// this round of work — what is new is the check that found them.
		/// </summary>
		private static string Pct2(double ratio) =>
			ratio.ToString("P2", System.Globalization.CultureInfo.InvariantCulture)
				.Replace(" ", "").Replace(" ", "");

		/// <summary>
		/// A utilisation as a percentage, to one decimal — through ONE formatter, in one culture.
		///
		/// Measured on a printed report from this machine: the summary read "73,7%" with a comma
		/// while every derivation step on the pages below read "73.70" with a point, because the
		/// steps go through an InvariantCulture helper and these did not. Two decimal separators in
		/// one English document, and on a machine with a different locale the report would differ
		/// again — a document that renders differently per machine cannot be a deliverable.
		///
		/// Invariant rather than the norm's own locale: the report is written in English and NORSOK
		/// is an English-language standard. A Czech localisation, if it ever comes, changes this one
		/// method.
		/// </summary>
		private static string Pct(double ratio) =>
			ratio.ToString("P1", System.Globalization.CultureInfo.InvariantCulture)
				// "P1" gives "73.7 %" — the space is not wanted, the rest is.
				.Replace(" ", "").Replace(" ", "");

		/// <summary>
		/// A validity condition, typeset. The engine states them in ASCII — <c>"0.2&lt;=beta&lt;=1.0"</c>,
		/// <c>"30&lt;=theta&lt;=90"</c> — and the report used to print that verbatim, in a monospace
		/// face, beside fully typeset KaTeX everywhere else. Measured in the exported PDF: 180
		/// occurrences of <c>&lt;=</c>.
		///
		/// Translated HERE rather than in the engine, deliberately: the strings are the engine's
		/// description of its own mathematics, and an engine that emitted HTML entities so a report
		/// could look right would be the wrong dependency. The engine says what it checked; this says
		/// how it is written down.
		///
		/// Escaped FIRST, then the operators replaced — the other order would let HtmlEncode mangle
		/// the entities it just inserted.
		/// </summary>
		internal static string ConditionHtml(string condition)
		{
			return Esc(condition)
				.Replace("&lt;=", "&nbsp;&le;&nbsp;")
				.Replace("&gt;=", "&nbsp;&ge;&nbsp;")
				.Replace("&lt;", "&nbsp;&lt;&nbsp;")
				.Replace("&gt;", "&nbsp;&gt;&nbsp;")
				.Replace("beta", "&beta;")
				.Replace("gamma", "&gamma;")
				.Replace("theta", "&theta;")
				.Replace("tau", "&tau;");
		}

		/// <summary>
		/// Write the whole of KaTeX into the page — library, styles and the call that typesets it.
		///
		/// EMBEDDED, not fetched. It used to be three cdn.jsdelivr.net tags, so on a machine with no
		/// network the equations came out as raw LaTeX source (`$$\dfrac{f_y T^2}{...}$$`) — in a
		/// report that is a deliverable and is often read exactly there. The fonts are inline data:
		/// URIs too, because the page is handed to WebView2 as a STRING and may be saved and moved,
		/// so a relative `url(fonts/…)` has no base to resolve against.
		///
		/// One method for both the report and the derivation window, because there WERE two copies
		/// of the CDN block and fixing one left the other fetching from the network — the tests
		/// caught that, a build could not.
		/// </summary>
		private static void AppendKatex(StringBuilder sb)
		{
			sb.AppendLine("<style>");
			sb.AppendLine(ReadResource("katex.min.css"));
			sb.AppendLine("</style>");
			sb.AppendLine("<script>");
			sb.AppendLine(ReadResource("katex.min.js"));
			sb.AppendLine("</script>");
			sb.AppendLine("<script>");
			sb.AppendLine(ReadResource("katex-auto-render.min.js"));
			sb.AppendLine("</script>");
			// inline scripts run in order, so KaTeX is defined by now; the call waits for
			// DOMContentLoaded only because the body it walks does not exist yet
			sb.AppendLine("<script>document.addEventListener('DOMContentLoaded', function () {");
			sb.AppendLine("  renderMathInElement(document.body, {delimiters: ["
				+ "{left:'$$',right:'$$',display:true},{left:'$',right:'$',display:false}]});");
			sb.AppendLine("});</script>");
		}

		/// <summary>
		/// One of the embedded KaTeX files, as text.
		///
		/// Returns "" and does NOT throw when a resource is missing: a report without typeset
		/// formulas is still a usable report (the LaTeX source shows through), whereas an exception
		/// here would take the whole report with it. The build embeds them, so an empty result means
		/// the csproj lost its EmbeddedResource entries — which is what the cached-name assert in
		/// the tests is for.
		/// </summary>
		private static string ReadResource(string fileName)
		{
			if (_resourceCache.TryGetValue(fileName, out var cached)) return cached;

			var assembly = typeof(NorsokHtmlReportGenerator).Assembly;
			// resource names are "<default namespace>.<folder>.<file>"; matched by suffix so a
			// namespace or folder rename cannot silently break it
			string? name = assembly.GetManifestResourceNames()
				.FirstOrDefault(n => n.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase));

			string text = "";
			if (name != null)
			{
				using var stream = assembly.GetManifestResourceStream(name);
				if (stream != null)
				{
					using var reader = new StreamReader(stream);
					text = reader.ReadToEnd();
				}
			}

			_resourceCache[fileName] = text;
			return text;
		}

		/// <summary>Read once — the CSS alone is 359 kB, and a report renders several times.</summary>
		private static readonly Dictionary<string, string> _resourceCache = new();

		/// <summary>Are the KaTeX resources actually embedded? For the tests.</summary>
		internal static bool KatexIsEmbedded =>
			ReadResource("katex.min.js").Length > 1000
			&& ReadResource("katex.min.css").Length > 1000
			&& ReadResource("katex-auto-render.min.js").Length > 100;

		private const string CssStyles = @"
* { box-sizing: border-box; margin: 0; padding: 0; }
body {
  font-family: 'Segoe UI', -apple-system, sans-serif;
  font-size: 14px;
  color: #333;
  background: #f5f5f5;
  padding: 24px;
  line-height: 1.5;
}
.report-header {
  text-align: center;
  margin-bottom: 16px;
}
.brand-line {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  margin-bottom: 4px;
}
.idea-brand { font-size: 22px; font-weight: 700; color: #2D2D2D; }
.idea-orange { color: #F57C00; }
.brand-sep { color: #ccc; font-size: 22px; font-weight: 300; }
.norsok-badge {
  font-size: 18px;
  font-weight: 600;
  color: #00838F;
}
.subtitle { color: #757575; font-size: 13px; margin-top: 4px; }
.norm-box {
  background: #FFF3E0;
  border-left: 4px solid #F57C00;
  padding: 12px 16px;
  margin-bottom: 24px;
  border-radius: 4px;
  font-size: 13px;
}
.settings-card {
  background: #fff;
  border-radius: 6px;
  padding: 16px 20px;
  margin-bottom: 24px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}
.settings-title {
  font-size: 15px;
  font-weight: 600;
  color: #00838F;
  margin-bottom: 12px;
}
.settings-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
  /* declared widths in the colgroup, honoured: the auto algorithm starved the narrow columns and
     wrapped two-word values onto two lines */
  table-layout: fixed;
}
/* ONE vertical rhythm for the whole table. The rows used to run 1, 1, 2, 1, 3 lines because each
   cell sized itself, and nothing lined up across a row. */
.settings-table td, .settings-table th { vertical-align: baseline; }
.settings-table .fac { white-space: nowrap; }
/* numbers form columns: same width per digit, aligned on the same edge as their header */
.val-norsok, .val-ec3 { font-variant-numeric: tabular-nums; }
.settings-table thead th {
  background: #1B2A4A;
  color: #fff;
  padding: 8px 12px;
  text-align: left;
  font-weight: 500;
}
.settings-table thead th:first-child { border-radius: 4px 0 0 0; }
.settings-table thead th:last-child { border-radius: 0 4px 0 0; }
.settings-table td {
  padding: 8px 12px;
  border-bottom: 1px solid #eee;
}
.settings-table tr:nth-child(even) { background: #fafafa; }
.val-norsok { font-weight: 700; color: #00838F; text-align: center; }
.val-ec3 { color: #999; text-align: center; }
.row-note { border-top: 2px solid #e0e0e0; }
.settings-note {
  font-size: 12px;
  color: #757575;
  margin-top: 10px;
  font-style: italic;
}
/* A quotation from the standard: italic, quiet, clearly not ours. */
.settings-quote { border-left: 3px solid #CFD8DC; padding-left: 10px; }
/* A disclosure about what the TOOL does to the model — upright, not italic, and framed, because it
   must not read as part of the quotation above it. The one thing in the front matter that tells a
   reader their project file is changed. */
.settings-disclosure {
  font-size: 12px;
  color: #37474F;
  margin-top: 10px;
  padding: 8px 10px;
  background: #FFF8E1;
  border-left: 3px solid #FFA726;
}
/* .connection-header shares .section-header's rule below — a connection heading IS a section
   heading, and two copies of the same declarations drift. */
.chapter-group { margin-bottom: 20px; }
.chapter-header {
  font-size: 14px;
  font-weight: 600;
  color: #00838F;
  padding: 8px 0 4px 0;
  border-bottom: 1px solid #e0e0e0;
  margin-bottom: 8px;
}
.chapter-count {
  display: inline-block;
  background: #e0f2f1;
  color: #00695c;
  font-size: 11px;
  font-weight: 500;
  padding: 1px 8px;
  border-radius: 10px;
  margin-left: 6px;
}
.check-card {
  background: #fff;
  border-radius: 6px;
  margin-bottom: 16px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.12);
  overflow: hidden;
}
.check-card > summary { list-style: none; cursor: pointer; }
.check-card > summary::-webkit-details-marker { display: none; }
.check-card > summary::before {
  content: '▸';
  display: inline-block;
  width: 16px;
  font-size: 14px;
  color: #999;
  transition: transform 0.15s;
}
.check-card[open] > summary::before {
  transform: rotate(90deg);
}
.card-header {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  font-weight: 500;
  user-select: none;
}
.card-header:hover { filter: brightness(0.97); }
.card-header.pass { background: #e8f5e9; border-left: 4px solid #4caf50; }
.card-header.fail { background: #ffebee; border-left: 4px solid #f44336; }
/* warn = a note, or a row nothing was assessed for. It was missing, so those cards rendered
   with an unstyled white header and an icon in the body colour -- the one state the app
   deliberately distinguishes was the one the report did not show. */
.card-header.warn { background: #fff8e1; border-left: 4px solid #ffa726; }
.status-icon { font-size: 18px; }
.pass .status-icon { color: #2e7d32; }
.fail .status-icon { color: #c62828; }
.warn .status-icon { color: #e65100; }
.section-ref { color: #00695c; font-weight: 600; }
.card-title { flex: 1; }
.eq-ref { color: #9e9e9e; font-size: 12px; }
.lc-badge {
  padding: 2px 8px;
  border-radius: 10px;
  font-size: 11px;
  font-weight: 500;
  background: #E3F2FD;
  color: #1565C0;
}
.util-badge {
  padding: 2px 10px;
  border-radius: 12px;
  font-size: 13px;
  font-weight: 600;
}
.util-badge.pass { background: #c8e6c9; color: #2e7d32; }
.util-badge.fail { background: #ffcdd2; color: #c62828; }
/* not assessed: deliberately neither green nor red — nothing was checked */
.util-badge.warn { background: #ffe0b2; color: #e65100; }
.card-body { padding: 12px 20px 16px 20px; }
.formula-block { margin-bottom: 12px; }
.formula-label {
  font-size: 12px;
  color: #757575;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin-bottom: 4px;
  margin-top: 8px;
}
.formula-math {
  padding: 8px 0;
  font-size: 16px;
}
/* what a symbol means, under the formula that introduces it — not upper-cased like a section
   label, because it is a sentence to read rather than a heading to skip */
.formula-legend {
  font-size: 11.5px;
  color: #607D8B;
  margin: 0 0 4px 0;
  text-align: center;
}
.substituted {
  background: #fafafa;
  padding: 8px 12px;
  border-radius: 4px;
  margin-bottom: 12px;
  border: 1px solid #e0e0e0;
}
.formula-sub {
  font-family: 'Consolas', 'Courier New', monospace;
  font-size: 13px;
  color: #424242;
}
.where-block { margin-bottom: 12px; }
.where-header {
  font-weight: 600;
  color: #555;
  margin-bottom: 6px;
}
.where-table {
  width: 100%;
  border-collapse: collapse;
}
.where-table tr:nth-child(even) { background: #fafafa; }
.where-table td { padding: 4px 8px; vertical-align: middle; }
.var-symbol { width: 140px; text-align: right; }
.var-eq { width: 20px; text-align: center; color: #999; }
.var-value { width: 140px; font-family: 'Consolas', monospace; font-size: 13px; }
.var-desc { color: #757575; font-size: 13px; }
.result-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  border-radius: 4px;
  font-size: 13px;
}
.result-bar.pass { background: #e8f5e9; }
.result-bar.fail { background: #ffebee; }
.result-bar.warn { background: #fff8e1; }
.result-verdict { font-weight: 700; font-size: 15px; }
.pass .result-verdict { color: #2e7d32; }
.fail .result-verdict { color: #c62828; }

/* §6.4 auto-topology derivation blocks */
.deriv-block {
  margin: 12px 0;
  padding: 10px 14px;
  background: #f7f9fb;
  border: 1px solid #e0e5ea;
  border-left: 3px solid #F36E21;
  border-radius: 4px;
}
.deriv-title { font-weight: 700; font-size: 13px; color: #37474F; margin: 0 0 8px 0; }
.deriv-h { font-weight: 600; font-size: 12px; color: #546E7A; margin: 10px 0 4px 0; }
/* One derivation step, as a card: label, formula, substituted numbers, result. Boxed and with a
   left rule so the four lines read as one unit — a wall of undivided formulas is what makes a
   derivation unreadable, and the python sheet cards them for the same reason. */
.deriv-step {
  background: #F8FAFB; border: 1px solid #E3E9ED; border-left: 3px solid #90A4AE;
  border-radius: 3px; padding: 7px 10px; margin: 6px 0;
}
.deriv-step-label { font-size: 11px; color: #546E7A; margin-bottom: 3px; }
.deriv-step-math { font-size: 12px; margin: 2px 0; overflow-x: auto; }
/* the result carries the weight: it is what a reader checks their own number against */
.deriv-step-res { font-size: 12px; margin: 2px 0; color: #1B5E20; font-weight: 600; }
.deriv-k { color: #546E7A; }
/* a value's provenance, subordinate to the value itself */
.deriv-hint { font-size: 11px; color: #78909C; }
.deriv-note { font-size: 11px; color: #78909C; margin: 3px 0 5px 0; }
.deriv-note { font-size: 12px; color: #546E7A; margin: 4px 0; }
/* A value shown for completeness that no check consumes. Greyed so a reader scanning the row
   does not take it for an input to the resistance — the mistake the off-plane column invited. */
.not-checked { color: #90A4AE; }
.deriv-warn {
  font-size: 12px; font-weight: 600; color: #c62828;
  background: #ffebee; border-radius: 4px; padding: 6px 10px; margin: 6px 0;
}
.deriv-table {
  border-collapse: collapse;
  font-size: 12px;
  margin: 2px 0 6px 0;
}
.deriv-table th {
  background: #eceff1; color: #455A64; font-weight: 600;
  padding: 3px 10px; text-align: left; border: 1px solid #dde3e8;
}
.deriv-table td {
  padding: 3px 10px; border: 1px solid #dde3e8;
  font-family: 'Consolas', monospace; font-size: 12px;
}
.deriv-table tr.active-class td { background: #fff3e0; font-weight: 600; }
.deriv-table td.v-ok { color: #2e7d32; }
.deriv-table td.v-bad { color: #c62828; font-weight: 700; }

/* Summary card */
.summary-card {
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 24px;
  text-align: center;
}

/* Per-connection verdicts, directly under the summary card: which joints are the problem. */
.connection-table {
  width: 100%;
  border-collapse: collapse;
  margin: 0 0 28px;
  font-size: 13px;
}
.connection-table th {
  text-align: left;
  padding: 7px 10px;
  background: #ECEFF1;
  border-bottom: 2px solid #CFD8DC;
  color: #37474F;
  font-weight: 600;
}
.connection-table td { padding: 6px 10px; border-bottom: 1px solid #ECEFF1; }
.connection-table .con-name { font-weight: 600; }
.connection-table .con-util { text-align: right; font-variant-numeric: tabular-nums; }
.connection-table .con-note { color: #607D8B; }
/* An unmet RECOMMENDATION of the standard. Lighter than the status it sits under, because it
   qualifies nothing about conformity: a joint missing a should-provision still passes. */
.connection-table .con-rec { color: #90A4AE; font-size: 11px; }
.connection-table .con-verdict { font-weight: 600; }
.connection-table .con-verdict.pass { color: #2E7D32; }
.connection-table .con-verdict.fail { color: #C62828; }
.connection-table .con-verdict.warn { color: #EF6C00; }

/* The joint figure. break-inside so a page break never lands between the picture and its caption. */
/* The figure and its caption stay together; the LEGEND is outside this box on purpose.
   Measured on the first export after the legend was added: 173 pages became 187, and the six
   figure pages went from 13 lines of text each to 3 — the legend pushed the protected block past
   what would fit beside the geometry table, so the whole figure moved to a page of its own and
   took 8 % of it. The legend is two lines of swatches; it may break away from the picture. */
.joint-figure {
  margin: 0 0 4px;
  padding: 0;
  break-inside: avoid;
}
.joint-figure img {
  display: block;
  max-width: 100%;
  border: 1px solid #E0E4E7;
  border-radius: 4px;
  background: #FCFCFD;
}
.joint-figure figcaption {
  margin-top: 5px;
  font-size: 11px;
  color: #78909C;
}

/* The utilisation scale, beside the figure whose colours it explains. Ten bands for 0..100 % plus
   a separated eleventh for over-capacity, which is a different statement rather than a finer step.
   Swatches are borderless and butt together so the ramp reads as one scale. */
.util-legend {
  display: flex;
  align-items: center;
  gap: 2px;
  margin-top: 6px;
  font-size: 10px;
  color: #78909C;
}
.util-legend-label { margin-right: 6px; }
.util-legend-tick { margin: 0 4px; font-variant-numeric: tabular-nums; }
.util-swatch {
  display: inline-block;
  width: 16px;
  height: 9px;
  margin: 0;
}
.util-swatch-over { margin-left: 8px; }

.summary-card.pass {
  background: linear-gradient(135deg, #e8f5e9, #c8e6c9);
  border: 2px solid #4caf50;
}
.summary-card.fail {
  background: linear-gradient(135deg, #ffebee, #ffcdd2);
  border: 2px solid #f44336;
}
/* incomplete: something was not assessed. Deliberately not green — nobody checked it. */
.summary-card.warn {
  background: linear-gradient(135deg, #fff8e1, #ffe0b2);
  border: 2px solid #ff9800;
}
.summary-verdict {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  margin-bottom: 16px;
}
.summary-icon { font-size: 32px; }
.summary-card.pass .summary-icon { color: #2e7d32; }
.summary-card.fail .summary-icon { color: #c62828; }
.summary-card.warn .summary-icon { color: #e65100; }
.summary-text { font-size: 20px; color: #333; }
.summary-stats {
  display: flex;
  justify-content: center;
  gap: 32px;
  flex-wrap: wrap;
}
.stat {
  text-align: center;
}
.stat-value {
  display: block;
  font-size: 28px;
  font-weight: 700;
  color: #333;
}
.stat-pass .stat-value { color: #2e7d32; }
.stat-fail .stat-value { color: #c62828; }
/* Amber, not green: a qualified check passed on extrapolated formulas. The label carries the
   words too — the document is read in greyscale as often as in colour. */
.stat-warn .stat-value { color: #e65100; }
.stat-governing .stat-value { color: #F57C00; }
.stat-label {
  display: block;
  font-size: 11px;
  color: #757575;
  margin-top: 2px;
}

/* ── Contents page ───────────────────────────────────────────────────────────
   A chapter number is a span, not a list marker: the numbering has to be the same in the contents
   and in the heading it points at, and CSS counters do not survive the HTML-to-PDF pass reliably. */
.section-header, .connection-header {
  font-size: 17px;
  color: #2D2D2D;
  border-bottom: 2px solid #F57C00;
  padding-bottom: 6px;
  margin: 24px 0 12px 0;
}
.chapter-no {
  display: inline-block;
  min-width: 1.6em;
  color: #F57C00;
  font-weight: 700;
}
.index-page { margin: 24px 0; }
.index-title {
  font-size: 17px;
  color: #2D2D2D;
  border-bottom: 2px solid #F57C00;
  padding-bottom: 6px;
  margin: 0 0 12px 0;
}
.index-table { width: 100%; border-collapse: collapse; font-size: 13px; }
.index-table td { padding: 5px 8px; border-bottom: 1px solid #ECEFF1; }
.ix-no { width: 3em; color: #F57C00; font-weight: 700; text-align: right; }
.ix-name a { color: #1B2A4A; text-decoration: none; }
.ix-name a:hover { text-decoration: underline; }
.ix-verdict { width: 5em; font-weight: 700; font-size: 11.5px; }
.ix-verdict.pass { color: #2e7d32; }
.ix-verdict.fail { color: #c62828; }
.ix-verdict.warn { color: #e65100; }
.ix-util { width: 6em; text-align: right; font-variant-numeric: tabular-nums; }

/* Print styles.
   ONE block: there used to be a second, minimal @media print emitted inline with the <style> tag,
   which is how the print rules came to be half-specified — a rule added to one was invisible in the
   other. */
@page {
  /* The physical page comes from CoreWebView2PrintSettings (see Models.PageSetup) — this is what a
     browser print from the tab obeys, and the two must agree or Ctrl+P gives a different document
     from the Export button. Keep them in step. */
  size: A4 portrait;
  margin: 20mm 15mm;

  /* No footer, and no page numbers. They were built here and removed, because an OFFSET page number
     cannot be produced in a margin box on this engine: the reset re-applies on every page, and no
     counter other than the two page ones is visible from page context at all. So a report inserted
     into a larger document could not be numbered to match it — measured six ways in
     PrintedPageProbe, and reported from the running app as start-at-77 printing 77 on every page.
     The reader numbers whatever document this is bound into. FooterCss holds the measurements, and
     stays as the seam if a PDF post-processing pass is ever added.

     ShouldPrintHeaderAndFooter also stays false (MainWindow.Report.cs): WebView2's own furniture
     would add the print date, the page title and a file:// URI to every page.

     (No quotation marks anywhere in this comment: the stylesheet is a C# verbatim string, and a
     single quote here ends the constant — 171 compile errors, all of them reported further down.) */
}
@media print {
  body { background: #fff; padding: 0; }
  /* The card MAY flow across a page break. It used to carry break-inside: avoid, and on a card
     that does not fit the remainder of a page the whole thing moved to a fresh one — measured on
     the shipped 173-page PDF: 11 pages filled to 22.7 % of their height, 41 under 65 %, against a
     median of 80 %. The atomic blocks below are what must not split; a card is a container, and
     protecting the container instead of its contents is what wasted three quarters of a sheet. */
  .check-card { box-shadow: none; border: 1px solid #ddd; }
  /* The disclosure triangle, gone in print — it is an affordance for a click that paper cannot
     take. This rule USED to read `details > summary::before`, which matches nothing: the marker is
     declared on `.check-card > summary::before`, so the more specific selector kept winning and the
     glyph printed anyway. Measured in the shipped PDF: 41 × '▸'. Both selectors are listed now,
     because the point is that the marker never prints, whichever rule drew it. */
  details > summary::before,
  .check-card > summary::before,
  .check-card[open] > summary::before { content: none; display: none; }

  /* Nothing that reads as one unit may be split.
     The summary card WAS only break-after: avoid, which says where the page may end AFTER it and
     nothing about breaking inside it — so the shipped PDF put the headline 73.7 % on page 2, alone,
     away from the verdict and the counters it belongs to. Measured: 'INCOMPLETE' at y=748 on page 1
     with the figure on the next page. That was the worst piece of typography in the document. */
  .summary-card, .index-page, .settings-card, .norm-box,
  .formula-block, .deriv-step, .joint-figure, table { break-inside: avoid; }
  /* NOT .deriv-block: it is a CONTAINER — the whole joint-plane section, several tables and half a
     page of prose — so protecting it is the same mistake as protecting the check card was.
     Measured after adding it in this round: the joint figure could no longer share a page with the
     section below it, six pages came out at 8 % fill, and the document grew from 173 to 187 pages.
     Its CONTENTS are protected individually, which is the whole point of the re-scoping. */

  /* A row is never split from itself, and a long table repeats its header rather than continuing
     into a page of anonymous numbers. */
  tr, td, th { break-inside: avoid; }
  thead { display: table-header-group; }

  /* Body text does not leave one line behind. */
  p, li { orphans: 3; widows: 3; }

  /* A VERDICT IS NEVER LEFT ALONE ON A PAGE.
     Measured on the 227-page export: page 35 held one line, the not-assessed banner, at 2 % fill.
     The conditions table above it carries break-inside: avoid, did not fit the space left, moved
     whole to the next page, and left its own verdict bar behind on the old one.
     The fix is not to let the table split (a list of unmet conditions broken across a page is
     worse) but to keep the bar WITH what it concludes: a break may not fall immediately before a
     result bar, nor immediately after the table that feeds it.
     NB this comment lives inside a C# string literal, so no double quotes here. */
  .result-bar { break-before: avoid; }
  .where-table { break-after: avoid; }

  /* Contents on a page of its own, and one page per connection.
     The FIRST connection is excepted: the contents page's break-after has already started a new
     page, and a break-before here as well would leave a blank one between them. */
  .index-page { break-after: page; }
  .connection-header { break-before: page; }
  .connection-header.first-connection { break-before: auto; }

  /* A heading must not be the last thing on a page — the reader turns over to find out what it was
     introducing.

     `.deriv-h` is the one that mattered and was missing: those are the derivation headings, and the
     shipped PDF ended 25 pages on one. Measured, by heading: 11 × 'Utilisation — eq (6.57)',
     8 × 'Weighted axial resistance', 6 × 'Members — geometry at the joint'. `p { orphans: 3 }`
     cannot help there — an orphaned heading is a whole paragraph, not the last line of one, so the
     orphans property has nothing to hold back. */
  .connection-header, .section-header, .index-title, .chapter-header,
  .deriv-h { break-after: avoid; }

  /* And a heading is not split from itself either, so it cannot land half on each page. */
  .deriv-h { break-inside: avoid; }

  /* Every card open, whatever state the page is in.
     The export passes expandAll, so the markup already carries <details open> — but a closed card
     in a PDF cannot be opened, so the derivation would simply be gone, and nothing about the file
     would say it was ever there. Making print independent of the attribute means it cannot be lost
     to a stray click, a re-render, or a future change to how the page is built.
     `details > *` reaches the body regardless of the browser's own summary handling. */
  details.check-card > *:not(summary) { display: block !important; }
  details.check-card { display: block !important; }
}
";
	}
}
