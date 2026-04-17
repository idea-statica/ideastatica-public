using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.2 — Axial Tension (Equation 6.1)
	/// N_Sd ≤ N_t,Rd = A · f_y / γ_M
	/// </summary>
	public static class AxialTensionCheck
	{
		public static NorsokFormulaResult Evaluate(double N_Sd, double A, double f_y, double gammaM = 1.15)
		{
			// N_t,Rd = A · f_y / γ_M  [N] → convert to kN
			double N_t_Rd_N = A * f_y / gammaM;
			double N_t_Rd_kN = N_t_Rd_N / 1000.0;

			double utilization = N_Sd > 0 ? N_Sd / N_t_Rd_kN : 0;

			return new NorsokFormulaResult
			{
				Section = "6.3.2",
				Equation = "6.1",
				Title = "Axial Tension",
				CheckExpression = "N_Sd ≤ N_t,Rd",
				Formula = "N_t,Rd = A · f_y / γ_M",
				FormulaSubstituted = $"N_t,Rd = {A:F0} × {f_y:F1} / {gammaM:F2} = {N_t_Rd_kN:F1} kN",
				Demand = N_Sd,
				Capacity = N_t_Rd_kN,
				Utilization = utilization,
				Passed = utilization <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "N_Sd", Description = "design axial tensile force", Value = N_Sd, Unit = "kN" },
					new() { Symbol = "A", Description = "cross-sectional area", Value = A, Unit = "mm²" },
					new() { Symbol = "f_y", Description = "characteristic yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "γ_M", Description = "partial safety factor for steel material (NORSOK N-004 Table 6-1)", Value = gammaM, Unit = "-" },
					new() { Symbol = "N_t,Rd", Description = "design tensile resistance", Value = N_t_Rd_kN, Unit = "kN" },
				}
			};
		}
	}
}
