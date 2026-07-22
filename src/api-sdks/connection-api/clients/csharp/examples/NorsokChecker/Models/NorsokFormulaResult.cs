namespace NorsokChecker.Models
{
	/// <summary>
	/// Result of evaluating a single Norsok formula (e.g., §6.3.2 Axial Tension).
	/// Contains the formula reference, all populated variable values, and the verdict.
	///
	/// Report output mimics IDEA StatiCa CHECK tab format:
	///   formula → substituted values → "Where:" block explaining each variable
	/// </summary>
	public class NorsokFormulaResult
	{
		/// <summary>Section reference, e.g. "6.3.2"</summary>
		public string Section { get; set; } = string.Empty;

		/// <summary>Equation number, e.g. "6.1"</summary>
		public string Equation { get; set; } = string.Empty;

		/// <summary>Title, e.g. "Axial Tension"</summary>
		public string Title { get; set; } = string.Empty;

		/// <summary>The check expression, e.g. "N_Sd ≤ N_t,Rd"</summary>
		public string CheckExpression { get; set; } = string.Empty;

		/// <summary>The formula with symbols, e.g. "N_t,Rd = A · f_y / γ_M"</summary>
		public string Formula { get; set; } = string.Empty;

		/// <summary>The formula with substituted numbers, e.g. "N_t,Rd = 30159 × 355 / 1.15 = 9310 kN"</summary>
		public string FormulaSubstituted { get; set; } = string.Empty;

		/// <summary>All variable values used in the formula evaluation.</summary>
		public List<FormulaVariable> Variables { get; set; } = new();

		/// <summary>The demand value (left-hand side), e.g. N_Sd</summary>
		public double Demand { get; set; }

		/// <summary>The capacity value (right-hand side), e.g. N_t,Rd</summary>
		public double Capacity { get; set; }

		/// <summary>Utilization ratio = Demand / Capacity</summary>
		public double Utilization { get; set; }

		/// <summary>True if check passes (utilization ≤ 1.0)</summary>
		public bool Passed { get; set; }

		/// <summary>Load case ID (0 = envelope/all). For per-LC breakdown.</summary>
		public int LoadCaseId { get; set; }

		/// <summary>
		/// Full §6.4 auto-topology check detail (engine result, classification, chord-stress trail).
		/// Set only by the auto-topology path; the HTML report renders the derivation blocks from it.
		/// </summary>
		public Services.Norsok64.JointCheckRow? JointDetail { get; set; }

		/// <summary>Generates a report string mimicking IDEA StatiCa CHECK tab format.</summary>
		public string ToReportString()
		{
			var sb = new System.Text.StringBuilder();
			string passSymbol = Passed ? "✓" : "✗";
			string passText = Passed ? "PASS" : "FAIL";

			// ── Header ──
			sb.AppendLine($"┌─────────────────────────────────────────────────────────");
			sb.AppendLine($"│ NORSOK N-004 §{Section} — {Title}   (Eq. {Equation})");
			sb.AppendLine($"├─────────────────────────────────────────────────────────");

			// ── Check condition ──
			sb.AppendLine($"│");
			sb.AppendLine($"│  Check:  {CheckExpression}");

			// ── Formula (symbolic) ──
			if (!string.IsNullOrEmpty(Formula))
			{
				sb.AppendLine($"│");
				sb.AppendLine($"│  {Formula}");
			}

			// ── Formula (substituted values) ──
			if (!string.IsNullOrEmpty(FormulaSubstituted))
			{
				sb.AppendLine($"│  {FormulaSubstituted}");
			}

			// ── Where block ──
			if (Variables.Count > 0)
			{
				sb.AppendLine($"│");
				sb.AppendLine($"│  Where:");
				foreach (var v in Variables)
				{
					sb.AppendLine($"│    {v.Symbol,-16} = {v.FormattedValue,-16}  — {v.Description}");
				}
			}

			// ── Result ──
			sb.AppendLine($"│");
			sb.AppendLine($"│  Utilization:  {Demand:G5} / {Capacity:G5} = {Utilization:F4}");
			sb.AppendLine($"│  Result:       {Utilization:F4} ≤ 1.0  →  {passSymbol} {passText}");
			sb.AppendLine($"└─────────────────────────────────────────────────────────");

			return sb.ToString();
		}
	}

	public class FormulaVariable
	{
		public string Symbol { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public double Value { get; set; }
		public string Unit { get; set; } = string.Empty;

		public string FormattedValue => Unit switch
		{
			"MPa" => $"{Value:F1} {Unit}",
			"kN" => $"{Value:F1} {Unit}",
			"kNm" => $"{Value:F2} {Unit}",
			"mm" => $"{Value:F1} {Unit}",
			"mm²" => $"{Value:F0} {Unit}",
			"mm³" => $"{Value:F0} {Unit}",
			"mm⁴" => $"{Value:F0} {Unit}",
			"-" => $"{Value:F4}",
			_ => $"{Value:G6} {Unit}".Trim()
		};
	}
}
