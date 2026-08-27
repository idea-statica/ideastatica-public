using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.8.1 — Axial Tension and Bending (Equation 6.26)
	/// (N_Sd / N_t,Rd)^1.75 + √(M²_y + M²_z) / M_Rd ≤ 1.0
	/// </summary>
	public static class TensionBendingCheck
	{
		public static NorsokFormulaResult Evaluate(
			double N_Sd, double N_t_Rd,
			double M_y_Sd, double M_z_Sd, double M_Rd)
		{
			double axialRatio = N_t_Rd > 0 ? N_Sd / N_t_Rd : 0;
			double axialTerm = Math.Pow(Math.Abs(axialRatio), 1.75);
			double M_res = Math.Sqrt(M_y_Sd * M_y_Sd + M_z_Sd * M_z_Sd);
			double momentTerm = M_Rd > 0 ? M_res / M_Rd : 0;
			double interaction = axialTerm + momentTerm;

			return new NorsokFormulaResult
			{
				Section = "6.3.8.1",
				Equation = "6.26",
				Title = "Axial Tension + Bending",
				CheckExpression = "(N_Sd / N_t,Rd)^1.75 + √(M²_y,Sd + M²_z,Sd) / M_Rd ≤ 1.0",
				Formula = "Interaction = (N_Sd/N_t,Rd)^1.75 + √(M²_y + M²_z)/M_Rd",
				FormulaSubstituted = $"= ({N_Sd:F1}/{N_t_Rd:F1})^1.75 + √({M_y_Sd:F1}² + {M_z_Sd:F1}²)/{M_Rd:F1} = {axialTerm:F4} + {momentTerm:F4} = {interaction:F4}",
				Demand = interaction,
				Capacity = 1.0,
				Utilization = interaction,
				Passed = interaction <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "N_Sd", Description = "design axial tensile force", Value = N_Sd, Unit = "kN" },
					new() { Symbol = "N_t,Rd", Description = "design tensile resistance (§6.3.2)", Value = N_t_Rd, Unit = "kN" },
					new() { Symbol = "M_y,Sd", Description = "in-plane bending moment", Value = M_y_Sd, Unit = "kNm" },
					new() { Symbol = "M_z,Sd", Description = "out-of-plane bending moment", Value = M_z_Sd, Unit = "kNm" },
					new() { Symbol = "M_Rd", Description = "design bending resistance (§6.3.4)", Value = M_Rd, Unit = "kNm" },
					new() { Symbol = "(N/N_t)^1.75", Description = "axial tension term (note: exponent 1.75 is Norsok-specific)", Value = axialTerm, Unit = "-" },
					new() { Symbol = "√(M²y+M²z)/M_Rd", Description = "bending term", Value = momentTerm, Unit = "-" },
				}
			};
		}
	}
}
