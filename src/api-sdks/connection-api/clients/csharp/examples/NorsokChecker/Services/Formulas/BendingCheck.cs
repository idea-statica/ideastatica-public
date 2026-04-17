using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.4 — Bending (Equations 6.9–6.12)
	/// M_Sd ≤ M_Rd = f_m · W / γ_M
	/// </summary>
	public static class BendingCheck
	{
		public const double E = 2.1e5; // MPa

		public static NorsokFormulaResult Evaluate(
			double M_Sd, double W, double Z,
			double f_y, double D, double t,
			double gammaM = 1.15)
		{
			double ZW = Z / W;
			double fyD_Et = f_y * D / (E * t);

			double f_m;
			string f_m_eq;

			if (fyD_Et <= 0.0517)
			{
				f_m = ZW * f_y;
				f_m_eq = $"Eq. 6.10 (compact): f_m = Z/W × f_y = {ZW:F3} × {f_y:F1}";
			}
			else if (fyD_Et <= 0.1034)
			{
				f_m = (1.13 - 2.58 * fyD_Et) * ZW * f_y;
				f_m_eq = $"Eq. 6.11: f_m = (1.13 - 2.58×{fyD_Et:F4}) × {ZW:F3} × {f_y:F1}";
			}
			else
			{
				f_m = (0.94 - 0.76 * fyD_Et) * ZW * f_y;
				f_m_eq = $"Eq. 6.12 (slender): f_m = (0.94 - 0.76×{fyD_Et:F4}) × {ZW:F3} × {f_y:F1}";
			}

			double M_Rd_Nmm = f_m * W / gammaM;
			double M_Rd_kNm = M_Rd_Nmm / 1e6;

			double utilization = M_Sd > 0 ? M_Sd / M_Rd_kNm : 0;

			return new NorsokFormulaResult
			{
				Section = "6.3.4",
				Equation = "6.9",
				Title = "Bending",
				CheckExpression = "M_Sd ≤ M_Rd",
				Formula = "M_Rd = f_m · W / γ_M",
				FormulaSubstituted = $"M_Rd = {f_m:F1} × {W:F0} / {gammaM:F2} / 10⁶ = {M_Rd_kNm:F1} kNm",
				Demand = M_Sd,
				Capacity = M_Rd_kNm,
				Utilization = utilization,
				Passed = utilization <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "M_Sd", Description = "design bending moment", Value = M_Sd, Unit = "kNm" },
					new() { Symbol = "W", Description = "elastic section modulus", Value = W, Unit = "mm³" },
					new() { Symbol = "Z", Description = "plastic section modulus", Value = Z, Unit = "mm³" },
					new() { Symbol = "Z/W", Description = "shape factor", Value = ZW, Unit = "-" },
					new() { Symbol = "f_y", Description = "characteristic yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "D", Description = "outside diameter", Value = D, Unit = "mm" },
					new() { Symbol = "t", Description = "wall thickness", Value = t, Unit = "mm" },
					new() { Symbol = "f_y·D/(E·t)", Description = "compactness parameter", Value = fyD_Et, Unit = "-" },
					new() { Symbol = "f_m", Description = f_m_eq, Value = f_m, Unit = "MPa" },
					new() { Symbol = "γ_M", Description = "partial safety factor (NORSOK N-004 Table 6-1)", Value = gammaM, Unit = "-" },
					new() { Symbol = "M_Rd", Description = "design bending resistance", Value = M_Rd_kNm, Unit = "kNm" },
				}
			};
		}
	}
}
