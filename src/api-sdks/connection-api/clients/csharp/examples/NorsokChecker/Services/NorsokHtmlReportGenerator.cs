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
			IReadOnlyDictionary<string, Norsok64.JointTopology>? topologies = null)
		{
			var sb = new StringBuilder();

			sb.AppendLine("<!DOCTYPE html>");
			sb.AppendLine("<html><head>");
			sb.AppendLine("<meta charset='utf-8'/>");
			sb.AppendLine("<title>NORSOK N-004 Compliance Report</title>");

			AppendKatex(sb);

			sb.AppendLine("<style>");
			sb.AppendLine(CssStyles);
			sb.AppendLine("</style>");
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
			sb.AppendLine("    <span class='norsok-badge'>NORSOK N-004 Compliance Report</span>");
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
			sb.AppendLine("  <strong>Model source:</strong> IDEA StatiCa Connection "
				+ "&mdash; geometry, cross-sections, materials and load effects<br/>");
			sb.AppendLine("  <strong>Checks by:</strong> NorsokChecker, evaluated from that model "
				+ "&mdash; no analysis is run");
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
			foreach (var (sub, val, ec3, use) in new[]
			{
				("M0", "1.15", "1.00", "Resistance of Class 1, 2 or 3 cross-sections"),
				("M1", "1.15", "1.00", "Resistance of Class 4 cross-sections; buckling"),
				("M2", "1.30", "1.25", "Net section at bolt holes; fillet &amp; partial penetration welds"),
				("M3", "1.30", "1.25", "Slip-resistant connections"),
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
			sb.AppendLine("  <p class='settings-note'>&sect;6.1: &ldquo;The material factor &gamma;<sub>M0</sub> is 1.15 for ULS unless noted otherwise. The material factors according to Table 6-1 shall be used if NS-EN 1993-1-1 and NS-EN 1993-1-8 are used for calculation of structural resistance.&rdquo; These factors are written into the project's own settings, so anything calculated in IDEA StatiCa Connection afterwards uses them too.</p>");
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
			RenderIndex(sb, allResults);

			// ── Executive Summary Card, then chapter 2 ──
			RenderSummaryCard(sb, allResults);
			RenderConnectionTable(sb, allResults);

			int chapter = ConnectionChapterBase;
			foreach (var (connectionName, formulas) in allResults)
			{
				// The id the index links to, and the number it announces. First connection excepted
				// from the page break: the index's own break-after has already started a page, and a
				// second break here would leave a blank one between them.
				string anchor = AnchorFor(chapter);
				string firstClass = chapter == ConnectionChapterBase ? " first-connection" : "";
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
				}

				// Where the checked forces came from, BEFORE the checks that use them.
				if (topologies != null && topologies.TryGetValue(connectionName, out var topo))
					RenderJointPlane(sb, topo);

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
		/// Chapters 1 and 2 are the summary and the connection overview, so the first connection is
		/// chapter 3. A named constant because three places have to agree on it — the index, the
		/// headings, and the "is this the first connection" test that suppresses one page break.
		/// </summary>
		private const int ConnectionChapterBase = 3;

		private static string AnchorFor(int chapter) => $"ch-{chapter}";

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
			int passed = allResults.Sum(r =>
				r.formulas.Count(f => !f.IsNote && !f.NotAssessed && f.Passed));
			int failed = checksPerformed - passed;
			int notes = allResults.Sum(r => r.formulas.Count(f => f.IsNote));

			// The gaps, in their own unit: rows nobody checked, and how they split by REASON — the
			// distinction a reader acts on. Scope means use another method; not evaluated means fix
			// the model and run again.
			var gapRows = allResults
				.SelectMany(r => r.formulas.Where(f => !f.IsNote && f.NotAssessed))
				.ToList();
			int outsideScope = gapRows.Count(f => f.Reason != NotAssessedReason.NotEvaluated);
			int notEvaluated = gapRows.Count(f => f.Reason == NotAssessedReason.NotEvaluated);
			int notAssessed = gapRows.Count;

			// And the connections, in theirs — the unit a reviewer actually counts in.
			var verdicts = allResults.Select(r => CheckWorkflow.Roll(r.formulas)).ToList();
			int consAssessed = verdicts.Count(v => v.Pass is "PASS" or "FAIL" or "PARTIAL");
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
			string statusClass = failed > 0 ? "fail" : (notAssessed > 0 || nothingChecked) ? "warn" : "pass";
			string verdict = failed > 0 ? "NON-COMPLIANT"
				: nothingChecked ? "NOT ASSESSED — no check was performed"
				: notAssessed > 0 ? "INCOMPLETE — part of the model was not assessed"
				: "COMPLIANT";
			string icon = failed > 0 ? "&#x2718;"
				: (notAssessed > 0 || nothingChecked) ? "&#x26A0;"
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
				sb.AppendLine($"    <td class='con-note'>{Esc(verdict.Status)}</td>");
				sb.AppendLine("  </tr>");
			}

			sb.AppendLine("</table>");

			// Which load effects actually governed anything, in one line under the table.
			//
			// The review asked for a load-case LEGEND, on the grounds that "LE12" appears as a
			// governing state with nothing saying what it is. Not built, deliberately: LE1…LE12 are
			// the model's OWN names for its load effects — the names the engineer typed in IDEA
			// StatiCa — so a legend could only say "LE12 is called LE12". A legend worth having would
			// list each state's FORCES, which is a different and much larger section.
			//
			// What was missing is cheaper and more useful: the reader cannot otherwise tell whether
			// the whole envelope was exercised or one state governed everything.
			var governingStates = allResults
				.SelectMany(r => r.formulas)
				.Where(f => !f.IsNote && !f.NotAssessed && !string.IsNullOrEmpty(f.LoadCaseName))
				.Select(f => f.LoadCaseName!)
				.Distinct()
				.OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
				.ToList();

			if (governingStates.Count > 0)
				sb.AppendLine("<p class='settings-note'>Governing load effects across the project: "
					+ $"<b>{Esc(string.Join(", ", governingStates))}</b> "
					+ $"({governingStates.Count} of the model's states governed at least one check; "
					+ "each check is evaluated against every state and reports its worst).</p>");
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

			// Main formula in KaTeX (display math)
			if (FormulaLatex.TryGetValue(fr.Section, out var latex))
			{
				sb.AppendLine("    <div class='formula-block'>");
				sb.AppendLine($"      <p class='formula-label'>Check condition:</p>");
				sb.AppendLine($"      <div class='formula-math'>$${latex.check}$$</div>");

				// What y and z MEAN, at the point the symbols first appear.
				//
				// They are the norm's own (eq 6.57's where-list defines M_y as in-plane and M_z as
				// out-of-plane), but everywhere else in this application y and z are a MEMBER's local
				// axes — so a reader who knows the rest of the app would derive the wrong thing from
				// them. The plane they refer to is the joint's, and saying so is the whole point of
				// this line.
				if (fr.Section == "6.4.3.6")
					sb.AppendLine("      <p class='formula-legend'>M<sub>y</sub> = in-plane, "
						+ "M<sub>z</sub> = out-of-plane bending &mdash; of the <b>joint plane</b>, "
						+ "not a member's local axes (&sect;6.4.3.6)</p>");

				sb.AppendLine($"      <p class='formula-label'>Design resistance:</p>");
				sb.AppendLine($"      <div class='formula-math'>$${latex.latex}$$</div>");
				sb.AppendLine("    </div>");
			}

			// Substituted values
			if (!string.IsNullOrEmpty(fr.FormulaSubstituted))
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
					+ $"(= {fr.Utilization:F4} &le; 1.0)</span>");
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
			Kv(sb, "classification", $"K {cl.FrK:P2} &middot; Y {cl.FrY:P2} &middot; X {cl.FrX:P2}");
			Kv(sb, "&gamma;<sub>M</sub>", N(inp.GammaM, 3));
			sb.AppendLine("      </table>");

			sb.AppendLine("      <p class='deriv-h'>Applied forces (in the joint plane)</p>");
			sb.AppendLine("      <table class='deriv-table'>");
			Kv(sb, "N<sub>Sd</sub> (+ tension)", $"{N(inp.NSd / 1e3, 1)} kN");
			// y/z, as eq (6.57) writes them — M_y is the in-plane moment, M_z the out-of-plane one.
			// (The chord's own moments below keep ip/op: the norm gives THOSE no y/z symbol, and
			// they do not appear in eq 6.57 at all.)
			Kv(sb, "M<sub>y,Sd</sub> <span class='deriv-hint'>(in-plane)</span>",
				$"{N(inp.MipSd / 1e3, 2)} kN&middot;m");
			Kv(sb, "M<sub>z,Sd</sub> <span class='deriv-hint'>(out-of-plane)</span>",
				$"{N(inp.MopSd / 1e3, 2)} kN&middot;m");
			sb.AppendLine("      </table>");

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
				+ "&mdash; 6.4.3.2&ndash;4, eq (6.53)/(6.55), Table 6-3/6-4</p>");

			Step(sb, "Chord utilisation A&sup2; &mdash; eq (6.55) (shared by all classes)",
				@"A^2 = \left(\dfrac{\sigma_{a,Sd}}{f_{y,chord}}\right)^2 + \dfrac{\sigma_{my,Sd}^2+\sigma_{mz,Sd}^2}{1.62\,f_{y,chord}^2}",
				$@"\left(\dfrac{{{P(sa, 1)}}}{{{N(fy, 0)}}}\right)^2 + \dfrac{{{P(smy, 1)}^2+{P(smz, 1)}^2}}{{1.62\cdot {N(fy, 0)}^2}}",
				N(r.QfMomentA2, 4));

			var cm = r.PerClass.TryGetValue(Norsok64.Joint64Class.K, out var kc0) ? kc0.CAxial : default;
			Step(sb, "Q<sub>f</sub>, moment &mdash; Table 6-4 has ONE row for moment, no K/Y/X split",
				@"Q_f = 1 + C_1\dfrac{\sigma_{a,Sd}}{f_{y,chord}} - C_2\dfrac{\sigma_{my,Sd}}{1.62\,f_{y,chord}} - C_3\,A^2",
				null, N(r.QfMoment, 3));

			Step(sb, "In-plane bending resistance M<sub>y,Rd</sub> &mdash; eq (6.53) "
				+ "(Q<sub>u,ipb</sub> shared by all classes, Table 6-3)",
				@"M_{y,Rd} = \dfrac{f_{y,chord}\,T^2\,d}{\gamma_M \sin\theta}\,Q_{u,ipb}\,Q_{f,mom}",
				$@"\dfrac{{{N(fy, 0)}\cdot {N(tChordMm, 0)}^2\cdot {N(dMm, 0)}}}{{{N(inp.GammaM, 2)}\cdot {N(r.SinTheta, 3)}}}\cdot {N(r.QuIpb, 3)}\cdot {N(r.QfMoment, 3)}",
				$@"{N(r.MRdIp / 1e3, 2)}\,kN\!\cdot\!m");

			Step(sb, "Out-of-plane bending resistance M<sub>z,Rd</sub> &mdash; eq (6.53)",
				@"M_{z,Rd} = \dfrac{f_{y,chord}\,T^2\,d}{\gamma_M \sin\theta}\,Q_{u,opb}\,Q_{f,mom}",
				$@"\dfrac{{{N(fy, 0)}\cdot {N(tChordMm, 0)}^2\cdot {N(dMm, 0)}}}{{{N(inp.GammaM, 2)}\cdot {N(r.SinTheta, 3)}}}\cdot {N(r.QuOpb, 3)}\cdot {N(r.QfMoment, 3)}",
				$@"{N(r.MRdOp / 1e3, 2)}\,kN\!\cdot\!m");

			// ── one block per ACTIVE mode. An inactive class is computed but plays no part in
			// this brace's check, and showing it would suggest it does.
			double baseAx = inp.FyChord * inp.T * inp.T / (inp.GammaM * r.SinTheta);

			// K: one sub-block per gap — a brace's K share is a SUM over its pairings, and the sum
			// alone cannot say whether it is one strong pairing or three weak ones
			if (cl.FrK > 1e-9 && r.KTerms.Count > 0)
			{
				sb.AppendLine($"      <p class='deriv-h'>Mode K &mdash; fraction of N<sub>Sd</sub> = "
					+ $"{cl.FrK:P2}" + (r.KTerms.Count > 1 ? $" (split over {r.KTerms.Count} gaps)" : "")
					+ "</p>");
				for (int i = 0; i < r.KTerms.Count; i++)
				{
					var kt = r.KTerms[i];
					string lbl = r.KTerms.Count > 1 ? $"K{i + 1}" : "K";
					sb.AppendLine($"      <p class='deriv-note'><b>{lbl}</b> &mdash; {kt.FrK:P1} of "
						+ "N<sub>Sd</sub> balanced across this gap.</p>");
					Step(sb, $"Q<sub>g</sub> &mdash; {lbl}, gap g = {N(kt.GapM * 1e3, 0)} mm, "
						+ $"g/D = {N(kt.GapM / inp.D, 3)}",
						@"Q_g\ (\text{note (b) under Table 6-3})", null, N(kt.Qg, 3));
					Step(sb, $"Q<sub>u,axial</sub> &mdash; {lbl}, Table 6-3 class K, "
						+ $"&beta; = {N(r.Beta, 3)}, &gamma; = {N(r.Gamma, 2)}",
						@"Q_u = \min\{(16+1.2\gamma)\beta^{1.2}Q_g,\ 40\beta^{1.2}Q_g\}",
						$@"\min\{{(16+1.2\cdot {N(r.Gamma, 2)})\cdot {N(r.Beta, 3)}^{{1.2}}\cdot {N(kt.Qg, 3)},\ 40\cdot {N(r.Beta, 3)}^{{1.2}}\cdot {N(kt.Qg, 3)}\}}",
						N(kt.QuAxial, 3));
					Step(sb, $"N<sub>Rd</sub> &mdash; {lbl}, eq (6.52)",
						@"N_{Rd,i} = \dfrac{f_{y,chord}\,T^2}{\gamma_M \sin\theta}\,Q_{u,i}\,Q_{f,K}",
						$@"{N(baseAx / 1e3, 1)}\,kN\cdot {N(kt.QuAxial, 3)}",
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
					+ $"N<sub>Sd</sub> = {frac:P2}</p>");
				Step(sb, $"Q<sub>f</sub>, axial &mdash; class {cls}, C&#8321;={N(c.CAxial.C1, 2)}, "
					+ $"C&#8322;={N(c.CAxial.C2, 2)}, C&#8323;={N(c.CAxial.C3, 2)}"
					+ (string.IsNullOrEmpty(c.CAxial.Note) ? "" : $" ({Esc(c.CAxial.Note)})"),
					@"Q_f = 1 + C_1\dfrac{\sigma_{a,Sd}}{f_{y,chord}} - C_2\dfrac{\sigma_{my,Sd}}{1.62\,f_{y,chord}} - C_3\,A^2",
					$@"1 + {N(c.CAxial.C1, 2)}\cdot\dfrac{{{P(sa, 1)}}}{{{N(fy, 0)}}} - {N(c.CAxial.C2, 2)}\cdot\dfrac{{{P(smy, 1)}}}{{1.62\cdot {N(fy, 0)}}} - {N(c.CAxial.C3, 2)}\cdot {N(c.QfAxialA2, 4)}",
					N(c.QfAxial, 3));
				Step(sb, $"Q<sub>u,axial</sub> &mdash; Table 6-3 class {cls} (brace in {tension})",
					cls == Norsok64.Joint64Class.Y
						? (r.LoadAxial == "tension" ? @"Q_u = 30\beta"
							: @"Q_u = \min\{2.8+(20+0.8\gamma)\beta^{1.6},\ 2.8+36\beta^{1.6}\}")
						: (r.LoadAxial == "tension" ? @"Q_u = 6.4\,\gamma^{0.6\beta^2}"
							: @"Q_u = (2.8+(12+0.1\gamma)\beta)\,Q_\beta"),
					null, N(c.QuAxial, 3));
				Step(sb, "N<sub>Rd</sub> &mdash; eq (6.52)",
					@"N_{Rd} = \dfrac{f_{y,chord}\,T^2}{\gamma_M \sin\theta}\,Q_u\,Q_f",
					$@"{N(baseAx / 1e3, 1)}\,kN\cdot {N(c.QuAxial, 3)}\cdot {N(c.QfAxial, 3)}",
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
					+ "&mdash; Comm. 6.4.2</p>");
				sb.AppendLine("      <table class='deriv-table'>");
				sb.AppendLine("        <tr><th>mode</th><th>fraction</th><th>N<sub>Rd,mode</sub></th></tr>");
				foreach (var (cls, frac) in active)
					sb.AppendLine($"        <tr><td>{cls}</td><td>{frac:P2}</td>"
						+ $"<td>{N(r.PerClass[cls].NRd / 1e3, 1)} kN</td></tr>");
				sb.AppendLine("      </table>");
				Step(sb, "Weighted axial resistance &mdash; Comm. 6.4.2 (mixture of K/Y/X)",
					@"\dfrac{1}{N_{Rd}} = \sum_{\text{mode}} \dfrac{fr_{\text{mode}}}{N_{Rd,\text{mode}}}",
					string.Join("+", active.Select(x =>
						$@"\dfrac{{{N(x.Item2, 3)}}}{{{N(r.PerClass[x.Item1].NRd / 1e3, 1)}}}")),
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
			// The step's label says what the three terms ARE, not the heading again. Both used to read
			// "Utilisation — eq (6.57)", printing it twice in immediate succession — 60 times in a
			// 30-check report. The HEADING is the half that stays: it anchors the four-phase order the
			// sheet is built on (DerivationContentTests asserts the sequence by position).
			Step(sb, "Sum of the three interaction terms &mdash; axial, in-plane, out-of-plane",
				@"u = \dfrac{N_{Sd}}{N_{Rd}} + \left(\dfrac{M_{y,Sd}}{M_{y,Rd}}\right)^2 + \left|\dfrac{M_{z,Sd}}{M_{z,Rd}}\right|",
				dom == null ? null
					: $@"{N(dom.UtilAxialTerm * 100, 2)}\% + {N(dom.UtilIpTerm * 100, 2)}\% + {N(dom.UtilOpTerm * 100, 2)}\%",
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
		private static void RenderJointPlane(StringBuilder sb, Norsok64.JointTopology topo)
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

			sb.AppendLine("<div class='deriv-block'>");
			sb.AppendLine("  <p class='deriv-h'>Joint plane and force transformation</p>");
			sb.AppendLine("  <p class='deriv-note'>The &sect;6.4 checks are evaluated on forces "
				+ "resolved into the JOINT plane, not on the member load effects as IDEA StatiCa "
				+ "Connection shows them. This section states how that plane and the chord were "
				+ "determined, and lists both sets of forces side by side so every checked value can "
				+ "be traced back to the model.</p>");

			// ── the plane and its frame ──
			sb.AppendLine("  <table class='deriv-table'>");
			Kv(sb, "chord (through member)", topo.Chord == null
				? "&mdash; none identified"
				: $"<b>{Esc(topo.Chord.Name ?? "?")}</b>"
					+ (topo.Chord.Section?.Name is { Length: > 0 } cs ? $" &mdash; {Esc(cs)}" : ""));
			Kv(sb, "plane normal <span class='deriv-hint'>(model coordinates)</span>", V(topo.NPlane));
			Kv(sb, "chord axis e<sub>x</sub>", V(topo.Ex));
			Kv(sb, "in-plane axis e<sub>y</sub>", V(topo.Ey));
			Kv(sb, "how the plane was fixed", Esc(topo.PlaneFitBasis ?? "&mdash;"));
			if (!topo.Coplanar || topo.PlaneSpread > 0)
				Kv(sb, "out-of-plane spread", $"{N(topo.PlaneSpread * 1e3, 1)} mm"
					+ (topo.Coplanar ? "" : " <span class='deriv-hint'>(not coplanar)</span>"));
			sb.AppendLine("  </table>");

			if (!string.IsNullOrEmpty(topo.PlaneWarn))
				sb.AppendLine($"  <p class='deriv-warn'>&#9888; {Esc(topo.PlaneWarn)}</p>");

			// ── one geometry row per brace, instead of the same numbers inside every check ──
			if (topo.BracesMeta.Count > 0)
			{
				sb.AppendLine("  <p class='deriv-h'>Members &mdash; geometry at the joint</p>");
				sb.AppendLine("  <table class='deriv-table'>");
				sb.AppendLine("    <tr><th>member</th><th>section</th><th>&theta;</th>"
					+ "<th>&beta;</th><th>off-plane</th><th>ecc. along chord</th>"
					+ "<th>chord face</th></tr>");
				foreach (var b in topo.BracesMeta)
				{
					sb.AppendLine($"    <tr><td><b>{Esc(b.Name)}</b></td>"
						+ $"<td>{Esc(b.Section?.Name ?? "&mdash;")}</td>"
						+ $"<td>{N(b.ThetaDeg, 1)}&deg;</td>"
						+ $"<td>{(b.Beta is { } be ? N(be, 3) : "&mdash;")}</td>"
						+ $"<td>{N(b.CoplanarDevDeg, 1)}&deg;</td>"
						+ $"<td>{N(b.OopOffsetM * 1e3, 1)} mm</td>"
						+ $"<td>{(b.Side >= 0 ? "+ey" : "&minus;ey")}</td></tr>");
				}
				sb.AppendLine("  </table>");
			}

			// ── the transformation itself, for the FIRST load effect ──
			//
			// One load effect, not all of them: the point is to show what the projection DOES, and
			// the arithmetic is the same for every state. Naming which one is what keeps it honest.
			var first = topo.BraceForces.FirstOrDefault();
			if (first != null && first.Rows.Count > 0)
			{
				sb.AppendLine("  <p class='deriv-h'>Force transformation &mdash; "
					+ $"{Esc(first.Name ?? $"LE{first.Id}")}</p>");
				sb.AppendLine("  <p class='deriv-note'>Left: the member loading in its own local axes, "
					+ "as the model carries it. Right: the same loading resolved into the brace's "
					+ "sub-plane, which is what &sect;6.4 checks. N is positive in TENSION; "
					+ "M<sub>y</sub> is in-plane and M<sub>z</sub> out-of-plane bending OF THE JOINT "
					+ "PLANE (eq 6.57), not a member's local y and z.</p>");
				sb.AppendLine("  <table class='deriv-table'>");
				sb.AppendLine("    <tr><th rowspan='2'>member</th>"
					+ "<th colspan='3'>from the model (local axes)</th>"
					+ "<th colspan='3'>resolved into the joint plane</th></tr>");
				sb.AppendLine("    <tr><th>N</th><th>M<sub>y,loc</sub></th><th>M<sub>z,loc</sub></th>"
					+ "<th>N<sub>Sd</sub></th><th>M<sub>y,Sd</sub></th><th>M<sub>z,Sd</sub></th></tr>");
				foreach (var f in first.Rows)
				{
					sb.AppendLine($"    <tr><td><b>{Esc(f.Name)}</b></td>"
						+ $"<td>{N(f.LocalN / 1e3, 1)} kN</td>"
						+ $"<td>{N(f.LocalMy / 1e3, 2)} kN&middot;m</td>"
						+ $"<td>{N(f.LocalMz / 1e3, 2)} kN&middot;m</td>"
						+ $"<td>{N(f.NSd / 1e3, 1)} kN</td>"
						+ $"<td>{N(f.Mip / 1e3, 2)} kN&middot;m</td>"
						+ $"<td>{N(f.Mop / 1e3, 2)} kN&middot;m</td></tr>");
				}
				sb.AppendLine("  </table>");
				sb.AppendLine("  <p class='deriv-note'>Section forces are taken AT THE NODE and "
					+ "projected without an r&times;F transfer, matching the reference "
					+ "implementation. Shear does not enter eq (6.57) and torsion is excluded by "
					+ "&sect;6.4.</p>");
			}

			sb.AppendLine("</div>");
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
.connection-table .con-verdict { font-weight: 600; }
.connection-table .con-verdict.pass { color: #2E7D32; }
.connection-table .con-verdict.fail { color: #C62828; }
.connection-table .con-verdict.warn { color: #EF6C00; }

/* The joint figure. break-inside so a page break never lands between the picture and its caption. */
.joint-figure {
  margin: 0 0 18px;
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

  /* Page numbers, and the report says whose they are.
     `counter(page)` in an @page margin box is supported from Chrome 131, and the WebView2 SDK this
     app pins (1.0.2903.40) IS the 131 build — the third version field mirrors the Chromium build.
     So this needs no library and no post-processing pass: the exported PDF had no page numbers at
     all, and 161 pages without them cannot be navigated or cited.

     Scoped to the report deliberately. This document is an INSERT — it gets bound into someone
     else's calculation package — so a bare page number would collide with the host document's own
     numbering. Naming the standard in front of it cannot be mistaken for the host's own footer,
     while still letting a reader cite page 7 of the check.
     (No quotation marks anywhere in this comment: the stylesheet is a C# verbatim string, and a
     single quote here ends the constant — 171 compile errors, all of them reported further down.)

     ShouldPrintHeaderAndFooter stays false (MainWindow.Report.cs): WebView2's own furniture would
     add the print date, the page title and a file:// URI on every page. This replaces it.

     Only `page` and `pages` work in page context — user-defined counters do not, and `string()`
     for a running header carrying the current connection's name is unimplemented in Blink
     (crbug 376420244), which is why the header below is fixed text. */
  @bottom-center {
    /* Doubled quotes: this whole stylesheet is a C# verbatim string, so a CSS string literal has
       to escape them or the constant ends here. */
    content: ""NORSOK N-004 §6.4 — "" counter(page) "" / "" counter(pages);
    font-family: 'Segoe UI', -apple-system, sans-serif;
    font-size: 8pt;
    color: #78909C;
  }
}
@media print {
  body { background: #fff; padding: 0; }
  .check-card { break-inside: avoid; box-shadow: none; border: 1px solid #ddd; }
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

  /* A row is never split from itself, and a long table repeats its header rather than continuing
     into a page of anonymous numbers. */
  tr, td, th { break-inside: avoid; }
  thead { display: table-header-group; }

  /* Body text does not leave one line behind. */
  p, li { orphans: 3; widows: 3; }

  /* Contents on a page of its own, and one page per connection.
     The FIRST connection is excepted: the contents page's break-after has already started a new
     page, and a break-before here as well would leave a blank one between them. */
  .index-page { break-after: page; }
  .connection-header { break-before: page; }
  .connection-header.first-connection { break-before: auto; }

  /* A heading must not be the last thing on a page — the reader turns over to find out what it was
     introducing. */
  .connection-header, .section-header, .index-title, .chapter-header { break-after: avoid; }

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
