using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.8.1 — Axial Tension and Bending (Equation 6.26)
	///
	/// (N_Sd / N_t,Rd)^1.75 + √(M_y,Sd² + M_z,Sd²) / M_Rd ≤ 1.0
	///
	/// Note: The exponent 1.75 on axial force is Norsok-specific.
	/// </summary>
	public static class TensionBendingCheck
	{
		/// <param name="N_Sd">Design axial tensile force [kN]</param>
		/// <param name="N_t_Rd">Design tensile resistance [kN] (from §6.3.2)</param>
		/// <param name="M_y_Sd">Design in-plane bending moment [kNm]</param>
		/// <param name="M_z_Sd">Design out-of-plane bending moment [kNm]</param>
		/// <param name="M_Rd">Design bending resistance [kNm] (from §6.3.4)</param>
		public static NorsokFormulaResult Evaluate(
			double N_Sd, double N_t_Rd,
			double M_y_Sd, double M_z_Sd, double M_Rd)
		{
			double axialTerm = Math.Pow(N_Sd / N_t_Rd, 1.75);
			double momentTerm = Math.Sqrt(M_y_Sd * M_y_Sd + M_z_Sd * M_z_Sd) / M_Rd;
			double interaction = axialTerm + momentTerm;

			return new NorsokFormulaResult
			{
				Section = "6.3.8.1",
				Equation = "6.26",
				Title = "Axial Tension + Bending",
				CheckExpression = "(N_Sd/N_t,Rd)^1.75 + √(M²_y + M²_z)/M_Rd ≤ 1.0",
				Demand = interaction,
				Capacity = 1.0,
				Utilization = interaction,
				Passed = interaction <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "N_Sd", Description = "Design axial tensile force", Value = N_Sd, Unit = "kN" },
					new() { Symbol = "N_t,Rd", Description = "Design tensile resistance (§6.3.2)", Value = N_t_Rd, Unit = "kN" },
					new() { Symbol = "M_y,Sd", Description = "In-plane bending moment", Value = M_y_Sd, Unit = "kNm" },
					new() { Symbol = "M_z,Sd", Description = "Out-of-plane bending moment", Value = M_z_Sd, Unit = "kNm" },
					new() { Symbol = "M_Rd", Description = "Design bending resistance (§6.3.4)", Value = M_Rd, Unit = "kNm" },
					new() { Symbol = "(N/N_t)^1.75", Description = "Axial tension term", Value = axialTerm, Unit = "-" },
					new() { Symbol = "√(M²y+M²z)/M_Rd", Description = "Bending term", Value = momentTerm, Unit = "-" },
					new() { Symbol = "Interaction", Description = "Combined utilization", Value = interaction, Unit = "-" },
				}
			};
		}
	}
}
