using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.2 — Axial Tension (Equation 6.1)
	///
	/// N_Sd ≤ N_t,Rd = A · f_y / γ_M
	///
	/// Covered by Von Mises stress in IDEA StatiCa, but this provides
	/// the explicit Norsok formula evaluation with variable traceability.
	/// </summary>
	public static class AxialTensionCheck
	{
		/// <param name="N_Sd">Design axial tensile force [kN] (positive = tension)</param>
		/// <param name="A">Cross-sectional area [mm²]</param>
		/// <param name="f_y">Characteristic yield strength [MPa]</param>
		/// <param name="gammaM">Material factor γ_M (default 1.15 per Table 6-1)</param>
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
				CheckExpression = "N_Sd ≤ N_t,Rd = A · f_y / γ_M",
				Demand = N_Sd,
				Capacity = N_t_Rd_kN,
				Utilization = utilization,
				Passed = utilization <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "N_Sd", Description = "Design axial tensile force", Value = N_Sd, Unit = "kN" },
					new() { Symbol = "A", Description = "Cross-sectional area", Value = A, Unit = "mm²" },
					new() { Symbol = "f_y", Description = "Characteristic yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "γ_M", Description = "Material factor (Table 6-1)", Value = gammaM, Unit = "-" },
					new() { Symbol = "N_t,Rd", Description = "Design tensile resistance", Value = N_t_Rd_kN, Unit = "kN" },
				}
			};
		}
	}
}
