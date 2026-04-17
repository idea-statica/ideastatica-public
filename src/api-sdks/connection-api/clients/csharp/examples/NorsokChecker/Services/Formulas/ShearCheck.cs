using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.5 — Shear (Equations 6.13–6.14)
	///
	/// Beam shear:     V_Sd ≤ V_Rd = A·f_y / (2·√3·γ_M)
	/// Torsional shear: M_T,Sd ≤ M_T,Rd = 2·I_p·f_y / (D·√3·γ_M)
	/// </summary>
	public static class ShearCheck
	{
		/// <summary>
		/// Beam shear check — Equation (6.13)
		/// </summary>
		/// <param name="V_Sd">Design shear force [kN]</param>
		/// <param name="A">Cross-sectional area [mm²]</param>
		/// <param name="f_y">Characteristic yield strength [MPa]</param>
		/// <param name="gammaM">Material factor γ_M = 1.15</param>
		public static NorsokFormulaResult EvaluateBeamShear(
			double V_Sd, double A, double f_y, double gammaM = 1.15)
		{
			// V_Rd = A·f_y / (2·√3·γ_M) [N] → kN
			double V_Rd_N = A * f_y / (2.0 * Math.Sqrt(3.0) * gammaM);
			double V_Rd_kN = V_Rd_N / 1000.0;

			double utilization = V_Sd > 0 ? V_Sd / V_Rd_kN : 0;

			return new NorsokFormulaResult
			{
				Section = "6.3.5",
				Equation = "6.13",
				Title = "Beam Shear",
				CheckExpression = "V_Sd ≤ V_Rd = A·f_y / (2·√3·γ_M)",
				Demand = V_Sd,
				Capacity = V_Rd_kN,
				Utilization = utilization,
				Passed = utilization <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "V_Sd", Description = "Design shear force", Value = V_Sd, Unit = "kN" },
					new() { Symbol = "A", Description = "Cross-sectional area", Value = A, Unit = "mm²" },
					new() { Symbol = "f_y", Description = "Characteristic yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "γ_M", Description = "Material factor", Value = gammaM, Unit = "-" },
					new() { Symbol = "V_Rd", Description = "Design shear resistance", Value = V_Rd_kN, Unit = "kN" },
				}
			};
		}

		/// <summary>
		/// Torsional shear check — Equation (6.14)
		/// </summary>
		/// <param name="M_T_Sd">Design torsional moment [kNm]</param>
		/// <param name="Ip">Polar moment of inertia [mm⁴]</param>
		/// <param name="D">Outside diameter [mm]</param>
		/// <param name="f_y">Characteristic yield strength [MPa]</param>
		/// <param name="gammaM">Material factor γ_M = 1.15</param>
		public static NorsokFormulaResult EvaluateTorsionalShear(
			double M_T_Sd, double Ip, double D, double f_y, double gammaM = 1.15)
		{
			// M_T,Rd = 2·I_p·f_y / (D·√3·γ_M) [N·mm] → kNm
			double M_T_Rd_Nmm = 2.0 * Ip * f_y / (D * Math.Sqrt(3.0) * gammaM);
			double M_T_Rd_kNm = M_T_Rd_Nmm / 1e6;

			double utilization = M_T_Sd > 0 ? M_T_Sd / M_T_Rd_kNm : 0;

			return new NorsokFormulaResult
			{
				Section = "6.3.5",
				Equation = "6.14",
				Title = "Torsional Shear",
				CheckExpression = "M_T,Sd ≤ M_T,Rd = 2·I_p·f_y / (D·√3·γ_M)",
				Demand = M_T_Sd,
				Capacity = M_T_Rd_kNm,
				Utilization = utilization,
				Passed = utilization <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "M_T,Sd", Description = "Design torsional moment", Value = M_T_Sd, Unit = "kNm" },
					new() { Symbol = "I_p", Description = "Polar moment of inertia", Value = Ip, Unit = "mm⁴" },
					new() { Symbol = "D", Description = "Outside diameter", Value = D, Unit = "mm" },
					new() { Symbol = "f_y", Description = "Characteristic yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "γ_M", Description = "Material factor", Value = gammaM, Unit = "-" },
					new() { Symbol = "M_T,Rd", Description = "Design torsional resistance", Value = M_T_Rd_kNm, Unit = "kNm" },
				}
			};
		}
	}
}
