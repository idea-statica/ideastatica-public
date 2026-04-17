using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.4 — Bending (Equations 6.9–6.12)
	///
	/// M_Sd ≤ M_Rd = f_m · W / γ_M
	///
	/// Where f_m depends on the f_y·D/(E·t) ratio and the Z/W shape factor.
	/// </summary>
	public static class BendingCheck
	{
		public const double E = 2.1e5; // Young's modulus [MPa]

		/// <param name="M_Sd">Design bending moment [kNm]</param>
		/// <param name="W">Elastic section modulus [mm³]</param>
		/// <param name="Z">Plastic section modulus [mm³]</param>
		/// <param name="f_y">Characteristic yield strength [MPa]</param>
		/// <param name="D">Outside diameter [mm]</param>
		/// <param name="t">Wall thickness [mm]</param>
		/// <param name="gammaM">Material factor γ_M</param>
		public static NorsokFormulaResult Evaluate(
			double M_Sd, double W, double Z,
			double f_y, double D, double t,
			double gammaM = 1.15)
		{
			double ZW = Z / W;
			double fyD_Et = f_y * D / (E * t);

			// Characteristic bending strength f_m (Eq. 6.10–6.12)
			double f_m;
			string f_m_eq;

			if (fyD_Et <= 0.0517)
			{
				// Eq. 6.10: compact section — full plastic capacity
				f_m = ZW * f_y;
				f_m_eq = "6.10 (compact)";
			}
			else if (fyD_Et <= 0.1034)
			{
				// Eq. 6.11: intermediate
				f_m = (1.13 - 2.58 * fyD_Et) * ZW * f_y;
				f_m_eq = "6.11 (intermediate)";
			}
			else if (fyD_Et <= 120.0 * f_y / E)
			{
				// Eq. 6.12: slender
				f_m = (0.94 - 0.76 * fyD_Et) * ZW * f_y;
				f_m_eq = "6.12 (slender)";
			}
			else
			{
				// Beyond range — use Eq. 6.12 conservatively
				f_m = (0.94 - 0.76 * fyD_Et) * ZW * f_y;
				f_m_eq = "6.12 (beyond range, conservative)";
			}

			// M_Rd = f_m · W / γ_M  [N·mm] → convert to kNm
			double M_Rd_Nmm = f_m * W / gammaM;
			double M_Rd_kNm = M_Rd_Nmm / 1e6;

			double utilization = M_Sd > 0 ? M_Sd / M_Rd_kNm : 0;

			return new NorsokFormulaResult
			{
				Section = "6.3.4",
				Equation = "6.9",
				Title = "Bending",
				CheckExpression = "M_Sd ≤ M_Rd = f_m · W / γ_M",
				Demand = M_Sd,
				Capacity = M_Rd_kNm,
				Utilization = utilization,
				Passed = utilization <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "M_Sd", Description = "Design bending moment", Value = M_Sd, Unit = "kNm" },
					new() { Symbol = "W", Description = "Elastic section modulus", Value = W, Unit = "mm³" },
					new() { Symbol = "Z", Description = "Plastic section modulus", Value = Z, Unit = "mm³" },
					new() { Symbol = "Z/W", Description = "Shape factor", Value = ZW, Unit = "-" },
					new() { Symbol = "f_y", Description = "Characteristic yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "D", Description = "Outside diameter", Value = D, Unit = "mm" },
					new() { Symbol = "t", Description = "Wall thickness", Value = t, Unit = "mm" },
					new() { Symbol = "f_y·D/(E·t)", Description = "Compactness parameter", Value = fyD_Et, Unit = "-" },
					new() { Symbol = "f_m", Description = $"Bending strength (Eq. {f_m_eq})", Value = f_m, Unit = "MPa" },
					new() { Symbol = "γ_M", Description = "Material factor", Value = gammaM, Unit = "-" },
					new() { Symbol = "M_Rd", Description = "Design bending resistance", Value = M_Rd_kNm, Unit = "kNm" },
				}
			};
		}
	}
}
