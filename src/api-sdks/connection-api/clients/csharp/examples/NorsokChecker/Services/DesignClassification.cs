using NorsokChecker.Models;

namespace NorsokChecker.Services
{
	/// <summary>
	/// NORSOK N-004 §5 — Design Class (DC) and NDT Inspection Category classification.
	///
	/// Table 5-1: DC1–DC5 based on consequence of failure + joint complexity
	/// Table 5-2: DC → Steel Quality Level (SQL)
	/// Table 5-3: DC + stress level → NDT Inspection Category (low fatigue)
	/// Table 5-4: DC + stress direction → NDT Inspection Category (high fatigue)
	/// </summary>
	public static class DesignClassification
	{
		/// <summary>
		/// Determine Design Class per Table 5-1.
		/// </summary>
		public static DesignClass ClassifyJoint(bool substantialConsequences, bool hasResidualStrength, bool highComplexity)
		{
			if (substantialConsequences && !hasResidualStrength)
				return highComplexity ? DesignClass.DC1 : DesignClass.DC2;

			if (!substantialConsequences || hasResidualStrength)
			{
				if (substantialConsequences && hasResidualStrength)
					return highComplexity ? DesignClass.DC3 : DesignClass.DC4;
				return DesignClass.DC5;
			}

			return DesignClass.DC5;
		}

		/// <summary>
		/// Steel Quality Level per Table 5-2.
		/// </summary>
		public static string GetSteelQualityLevel(DesignClass dc, bool throughThicknessStress)
		{
			return dc switch
			{
				DesignClass.DC1 => throughThicknessStress ? "I" : "I",
				DesignClass.DC2 => throughThicknessStress ? "I" : "II",
				DesignClass.DC3 => throughThicknessStress ? "II" : "III",
				DesignClass.DC4 => throughThicknessStress ? "III" : "IV",
				DesignClass.DC5 => "IV",
				_ => "IV"
			};
		}

		/// <summary>
		/// NDT Inspection Category per Table 5-3 (low fatigue utilisation).
		/// stressLevel: "high" (>0.85 f_d), "moderate" (0.6–0.85 f_d), "low" (&lt;0.6 f_d)
		/// </summary>
		public static string GetInspectionCategory_LowFatigue(DesignClass dc, WeldStressLevel stressLevel)
		{
			return (dc, stressLevel) switch
			{
				(DesignClass.DC1 or DesignClass.DC2, WeldStressLevel.High) => "A",
				(DesignClass.DC1 or DesignClass.DC2, WeldStressLevel.Moderate) => "B",
				(DesignClass.DC1 or DesignClass.DC2, WeldStressLevel.Low) => "C",
				(DesignClass.DC3 or DesignClass.DC4, WeldStressLevel.High) => "B",
				(DesignClass.DC3 or DesignClass.DC4, WeldStressLevel.Moderate) => "C",
				(DesignClass.DC3 or DesignClass.DC4, WeldStressLevel.Low) => "D",
				(DesignClass.DC5, _) => stressLevel == WeldStressLevel.Low ? "E" : "D",
				_ => "D"
			};
		}

		/// <summary>
		/// NDT Inspection Category per Table 5-4 (high fatigue utilisation).
		/// </summary>
		public static string GetInspectionCategory_HighFatigue(DesignClass dc, bool stressTransverseToWeld)
		{
			return (dc, stressTransverseToWeld) switch
			{
				(DesignClass.DC1 or DesignClass.DC2, true) => "A",
				(DesignClass.DC1 or DesignClass.DC2, false) => "B",
				(DesignClass.DC3 or DesignClass.DC4, true) => "B",
				(DesignClass.DC3 or DesignClass.DC4, false) => "C",
				(DesignClass.DC5, true) => "D",
				(DesignClass.DC5, false) => "E",
				_ => "D"
			};
		}

		/// <summary>
		/// Determine weld stress level from utilization (stress / design strength).
		/// Per Table 5-3 notes: high > 0.85, moderate 0.6–0.85, low &lt; 0.6.
		/// </summary>
		public static WeldStressLevel ClassifyStressLevel(double utilization)
		{
			if (utilization > 0.85) return WeldStressLevel.High;
			if (utilization > 0.6) return WeldStressLevel.Moderate;
			return WeldStressLevel.Low;
		}

		/// <summary>
		/// Generate a full classification report as a NorsokFormulaResult.
		/// </summary>
		public static NorsokFormulaResult GenerateClassificationReport(
			DesignClass dc, string steelQualityLevel, string inspectionCategory,
			double maxWeldUtilization, bool highFatigueUtilisation)
		{
			string dcDesc = dc switch
			{
				DesignClass.DC1 => "Substantial consequences, limited residual strength, high complexity",
				DesignClass.DC2 => "Substantial consequences, limited residual strength, low complexity",
				DesignClass.DC3 => "Residual strength available, high complexity",
				DesignClass.DC4 => "Residual strength available, low complexity",
				DesignClass.DC5 => "Without substantial consequences",
				_ => ""
			};

			string icRequirement = inspectionCategory switch
			{
				"A" => "100% NDT of all welds (MPI + UT/RT)",
				"B" => "> 50% NDT at hotspot regions, 20% elsewhere",
				"C" => "20% NDT, mandatory inspection at stress concentrations",
				"D" => "10% NDT, visual inspection of all welds",
				"E" => "Visual inspection only",
				_ => ""
			};

			string sqlRequirement = steelQualityLevel switch
			{
				"I" => "Highest toughness — Charpy test per NORSOK M-120, full traceability",
				"II" => "High toughness — Charpy test required",
				"III" => "Standard toughness",
				"IV" => "Basic requirements",
				_ => ""
			};

			return new NorsokFormulaResult
			{
				Section = "5",
				Equation = "Table 5-1",
				Title = $"Design Classification — {dc}",
				CheckExpression = $"{dc}: {dcDesc}",
				Formula = "",
				FormulaSubstituted = "",
				Demand = 0,
				Capacity = 1,
				Utilization = 0,
				Passed = true, // Classification is informational, not pass/fail
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "DC", Description = dcDesc, Value = (int)dc, Unit = dc.ToString() },
					new() { Symbol = "SQL", Description = $"Steel Quality Level {steelQualityLevel}: {sqlRequirement}", Value = 0, Unit = steelQualityLevel },
					new() { Symbol = "IC", Description = $"Inspection Category {inspectionCategory}: {icRequirement}", Value = 0, Unit = inspectionCategory },
					new() { Symbol = "σ_max/f_d", Description = $"Max weld utilization → stress level: {DesignClassification.ClassifyStressLevel(maxWeldUtilization)}", Value = maxWeldUtilization, Unit = "-" },
					new() { Symbol = "Fatigue", Description = highFatigueUtilisation
						? "High fatigue utilisation (DFF < 3) — use Table 5-4 for inspection"
						: "Low fatigue utilisation (DFF ≥ 3) — use Table 5-3 for inspection",
						Value = 0, Unit = highFatigueUtilisation ? "High" : "Low" },
				}
			};
		}
	}

	public enum DesignClass { DC1 = 1, DC2 = 2, DC3 = 3, DC4 = 4, DC5 = 5 }
	public enum WeldStressLevel { Low, Moderate, High }
}
