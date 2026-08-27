using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.5 — Shear (Equations 6.13–6.14)
	/// </summary>
	public static class ShearCheck
	{
		public static NorsokFormulaResult EvaluateBeamShear(
			double V_Sd, double A, double f_y, double gammaM = 1.15)
		{
			double V_Rd_N = A * f_y / (2.0 * Math.Sqrt(3.0) * gammaM);
			double V_Rd_kN = V_Rd_N / 1000.0;
			double utilization = V_Sd > 0 ? V_Sd / V_Rd_kN : 0;

			return new NorsokFormulaResult
			{
				Section = "6.3.5",
				Equation = "6.13",
				Title = "Beam Shear",
				CheckExpression = "V_Sd ≤ V_Rd",
				Formula = "V_Rd = A · f_y / (2·√3·γ_M)",
				FormulaSubstituted = $"V_Rd = {A:F0} × {f_y:F1} / (2×√3×{gammaM:F2}) / 1000 = {V_Rd_kN:F1} kN",
				Demand = V_Sd,
				Capacity = V_Rd_kN,
				Utilization = utilization,
				Passed = utilization <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "V_Sd", Description = "design shear force", Value = V_Sd, Unit = "kN" },
					new() { Symbol = "A", Description = "cross-sectional area", Value = A, Unit = "mm²" },
					new() { Symbol = "f_y", Description = "characteristic yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "γ_M", Description = "partial safety factor (NORSOK N-004 Table 6-1)", Value = gammaM, Unit = "-" },
					new() { Symbol = "V_Rd", Description = "design shear resistance", Value = V_Rd_kN, Unit = "kN" },
				}
			};
		}

		public static NorsokFormulaResult EvaluateTorsionalShear(
			double M_T_Sd, double Ip, double D, double f_y, double gammaM = 1.15)
		{
			double M_T_Rd_Nmm = 2.0 * Ip * f_y / (D * Math.Sqrt(3.0) * gammaM);
			double M_T_Rd_kNm = M_T_Rd_Nmm / 1e6;
			double utilization = M_T_Sd > 0 ? M_T_Sd / M_T_Rd_kNm : 0;

			return new NorsokFormulaResult
			{
				Section = "6.3.5",
				Equation = "6.14",
				Title = "Torsional Shear",
				CheckExpression = "M_T,Sd ≤ M_T,Rd",
				Formula = "M_T,Rd = 2·I_p·f_y / (D·√3·γ_M)",
				FormulaSubstituted = $"M_T,Rd = 2×{Ip:F0}×{f_y:F1} / ({D:F1}×√3×{gammaM:F2}) / 10⁶ = {M_T_Rd_kNm:F1} kNm",
				Demand = M_T_Sd,
				Capacity = M_T_Rd_kNm,
				Utilization = utilization,
				Passed = utilization <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "M_T,Sd", Description = "design torsional moment", Value = M_T_Sd, Unit = "kNm" },
					new() { Symbol = "I_p", Description = "polar moment of inertia", Value = Ip, Unit = "mm⁴" },
					new() { Symbol = "D", Description = "outside diameter", Value = D, Unit = "mm" },
					new() { Symbol = "f_y", Description = "characteristic yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "γ_M", Description = "partial safety factor", Value = gammaM, Unit = "-" },
					new() { Symbol = "M_T,Rd", Description = "design torsional resistance", Value = M_T_Rd_kNm, Unit = "kNm" },
				}
			};
		}
	}
}
