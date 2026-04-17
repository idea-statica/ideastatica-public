namespace NorsokChecker.Models
{
	/// <summary>
	/// Result of evaluating a single Norsok formula (e.g., §6.3.2 Axial Tension).
	/// Contains the formula reference, all populated variable values, and the verdict.
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

		/// <summary>All variable values used in the formula evaluation.</summary>
		public List<FormulaVariable> Variables { get; set; } = new();

		/// <summary>The demand value (left-hand side), e.g. N_Sd</summary>
		public double Demand { get; set; }

		/// <summary>The capacity value (right-hand side), e.g. N_t,Rd</summary>
		public double Capacity { get; set; }

		/// <summary>Utilization ratio = Demand / Capacity (or interaction value for combined checks)</summary>
		public double Utilization { get; set; }

		/// <summary>True if check passes (utilization ≤ 1.0)</summary>
		public bool Passed { get; set; }

		/// <summary>Generates a formatted report string for this formula.</summary>
		public string ToReportString()
		{
			var sb = new System.Text.StringBuilder();
			sb.AppendLine($"§{Section} {Title} — Equation ({Equation})");
			sb.AppendLine($"  Check: {CheckExpression}");
			sb.AppendLine();

			foreach (var v in Variables)
			{
				sb.AppendLine($"  {v.Symbol,-12} = {v.FormattedValue,-16} ({v.Description})");
			}

			sb.AppendLine();
			sb.AppendLine($"  Demand:      {Demand:F2}");
			sb.AppendLine($"  Capacity:    {Capacity:F2}");
			sb.AppendLine($"  Utilization: {Utilization:F4}");
			sb.AppendLine($"  Result:      {(Passed ? "PASS" : "FAIL")}");

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
