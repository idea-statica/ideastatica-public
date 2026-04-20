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
			["Bolt"] = (
				@"\frac{F_{t,Sd}}{F_{t,Rd}} + \frac{F_{v,Sd}}{1.4 \cdot F_{v,Rd}} \leq 1.0",
				@"\text{Interaction}_{tension+shear} \leq 1.0"
			),
			["Weld"] = (
				@"f_{w,Rd} = \frac{f_u}{\beta_w \cdot \gamma_{M2}}",
				@"\sigma_w \leq f_{w,Rd}"
			),
		};

		public static string GenerateReport(
			string projectName,
			IReadOnlyList<(string connectionName, List<NorsokFormulaResult> formulas)> allResults)
		{
			var sb = new StringBuilder();

			sb.AppendLine("<!DOCTYPE html>");
			sb.AppendLine("<html><head>");
			sb.AppendLine("<meta charset='utf-8'/>");
			sb.AppendLine("<title>NORSOK N-004 Compliance Report</title>");

			// KaTeX from CDN
			sb.AppendLine("<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/katex@0.16.11/dist/katex.min.css'/>");
			sb.AppendLine("<script defer src='https://cdn.jsdelivr.net/npm/katex@0.16.11/dist/katex.min.js'></script>");
			sb.AppendLine("<script defer src='https://cdn.jsdelivr.net/npm/katex@0.16.11/dist/contrib/auto-render.min.js'");
			sb.AppendLine("  onload=\"renderMathInElement(document.body, {delimiters: [{left:'$$',right:'$$',display:true},{left:'$',right:'$',display:false}]});\"></script>");

			sb.AppendLine("<style>");
			sb.AppendLine(CssStyles);
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
			sb.AppendLine("  <strong>Chapter 6.3:</strong> Tubular Members &mdash; Strength and Stability Requirements<br/>");
			sb.AppendLine("  <strong>Engine:</strong> IDEA StatiCa Connection CBFEM Analysis via REST API");
			sb.AppendLine("</div>");

			// Table 6-1 Material factors
			sb.AppendLine("<div class='settings-card'>");
			sb.AppendLine("  <h3 class='settings-title'>Table 6-1 &mdash; Material Factors Applied to Project</h3>");
			sb.AppendLine("  <table class='settings-table'>");
			sb.AppendLine("    <thead><tr><th>Factor</th><th>Value</th><th>EC3 Default</th><th>Application</th></tr></thead>");
			sb.AppendLine("    <tbody>");
			sb.AppendLine("      <tr><td>$ \\gamma_{M0} $</td><td class='val-norsok'>1.15</td><td class='val-ec3'>1.00</td><td>Resistance of Class 1, 2 or 3 cross-sections</td></tr>");
			sb.AppendLine("      <tr><td>$ \\gamma_{M1} $</td><td class='val-norsok'>1.15</td><td class='val-ec3'>1.00</td><td>Resistance of Class 4 cross-sections; buckling</td></tr>");
			sb.AppendLine("      <tr><td>$ \\gamma_{M2} $</td><td class='val-norsok'>1.30</td><td class='val-ec3'>1.25</td><td>Net section at bolt holes; fillet &amp; partial penetration welds; bolted connections</td></tr>");
			sb.AppendLine("      <tr><td>$ \\gamma_{M3} $</td><td class='val-norsok'>1.30</td><td class='val-ec3'>1.25</td><td>Slip-resistant connections</td></tr>");
			sb.AppendLine("      <tr class='row-note'><td>$ \\gamma_{BC} $</td><td class='val-norsok'>1.05</td><td class='val-ec3'>&mdash;</td><td>Additional building code factor (&sect;6.1) &mdash; multiplied with EN 1993 factors where applicable</td></tr>");
			sb.AppendLine("    </tbody>");
			sb.AppendLine("  </table>");
			sb.AppendLine("  <p class='settings-note'>&sect;6.1: &ldquo;The material factor &gamma;<sub>M0</sub> is 1.15 for ULS unless noted otherwise.&rdquo;</p>");
			sb.AppendLine("</div>");

			foreach (var (connectionName, formulas) in allResults)
			{
				sb.AppendLine($"<h2 class='connection-header'>{Esc(connectionName)}</h2>");

				foreach (var fr in formulas)
				{
					RenderFormulaCard(sb, fr);
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

			sb.AppendLine("</body></html>");
			return sb.ToString();
		}

		private static void RenderFormulaCard(StringBuilder sb, NorsokFormulaResult fr)
		{
			string statusClass = fr.Passed ? "pass" : "fail";
			string statusIcon = fr.Passed ? "&#x2714;" : "&#x2718;";
			string statusText = fr.Passed ? "PASS" : "FAIL";

			sb.AppendLine($"<details class='check-card {statusClass}'>");

			// Collapsible header — click to expand/collapse
			sb.AppendLine($"  <summary class='card-header {statusClass}'>");
			sb.AppendLine($"    <span class='status-icon'>{statusIcon}</span>");
			sb.AppendLine($"    <span class='section-ref'>&sect;{Esc(fr.Section)}</span>");
			sb.AppendLine($"    <span class='card-title'>{Esc(fr.Title)}</span>");
			sb.AppendLine($"    <span class='eq-ref'>(Eq. {Esc(fr.Equation)})</span>");
			sb.AppendLine($"    <span class='util-badge {statusClass}'>{fr.Utilization * 100:F1}%</span>");
			sb.AppendLine("  </summary>");

			sb.AppendLine("  <div class='card-body'>");

			// Main formula in KaTeX (display math)
			if (FormulaLatex.TryGetValue(fr.Section, out var latex))
			{
				sb.AppendLine("    <div class='formula-block'>");
				sb.AppendLine($"      <p class='formula-label'>Check condition:</p>");
				sb.AppendLine($"      <div class='formula-math'>$${latex.check}$$</div>");
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

			// Where block
			if (fr.Variables.Count > 0)
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

			// Result bar
			sb.AppendLine($"    <div class='result-bar {statusClass}'>");
			sb.AppendLine($"      <span>Utilization: <strong>{fr.Utilization * 100:F1}%</strong> (= {fr.Utilization:F4} &le; 1.0)</span>");
			sb.AppendLine($"      <span class='result-verdict'>{statusIcon} {statusText}</span>");
			sb.AppendLine("    </div>");

			sb.AppendLine("  </div>"); // card-body
			sb.AppendLine("</details>"); // check-card
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
				.Replace("LC", @"\text{LC}");
		}

		private static string Esc(string s) => System.Net.WebUtility.HtmlEncode(s);

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
}
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
.connection-header {
  font-size: 17px;
  color: #2D2D2D;
  border-bottom: 2px solid #F57C00;
  padding-bottom: 6px;
  margin: 24px 0 12px 0;
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
.status-icon { font-size: 18px; }
.pass .status-icon { color: #2e7d32; }
.fail .status-icon { color: #c62828; }
.section-ref { color: #00695c; font-weight: 600; }
.card-title { flex: 1; }
.eq-ref { color: #9e9e9e; font-size: 12px; }
.util-badge {
  padding: 2px 10px;
  border-radius: 12px;
  font-size: 13px;
  font-weight: 600;
}
.util-badge.pass { background: #c8e6c9; color: #2e7d32; }
.util-badge.fail { background: #ffcdd2; color: #c62828; }
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
.result-verdict { font-weight: 700; font-size: 15px; }
.pass .result-verdict { color: #2e7d32; }
.fail .result-verdict { color: #c62828; }
";
	}
}
