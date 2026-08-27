using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.8.3 — Interaction Shear and Bending Moment (Eq. 6.31–6.32)
	///
	/// M_Sd/M_Rd ≤ 1.4 - V_Sd/V_Rd    for V_Sd ≥ 0.4·V_Rd
	/// M_Sd/M_Rd ≤ 1.0                 for V_Sd &lt; 0.4·V_Rd
	/// </summary>
	public static class ShearBendingCheck
	{
		/// <param name="M_Sd">Design bending moment [kNm]</param>
		/// <param name="M_Rd">Design bending resistance [kNm] (from §6.3.4)</param>
		/// <param name="V_Sd">Design shear force [kN]</param>
		/// <param name="V_Rd">Design shear resistance [kN] (from §6.3.5)</param>
		public static NorsokFormulaResult Evaluate(
			double M_Sd, double M_Rd,
			double V_Sd, double V_Rd)
		{
			// Use absolute values — the formulas check magnitudes
			M_Sd = Math.Abs(M_Sd);
			V_Sd = Math.Abs(V_Sd);

			double shearRatio = V_Rd > 0 ? V_Sd / V_Rd : 0;
			double bendingRatio = M_Rd > 0 ? M_Sd / M_Rd : 0;

			bool highShear = V_Sd >= 0.4 * V_Rd;
			double allowable;
			string equation;

			if (highShear)
			{
				// Eq. 6.31: linear interaction for high shear
				allowable = 1.4 - shearRatio;
				equation = "6.31";

				// If shear alone exceeds 1.4×V_Rd, the allowable goes ≤ 0 → automatic fail
				if (allowable <= 0)
					allowable = 0.001; // avoid division by zero, will show very high utilization
			}
			else
			{
				// Eq. 6.32: pure bending check for low shear
				allowable = 1.0;
				equation = "6.32";
			}

			double utilization = bendingRatio / allowable;

			return new NorsokFormulaResult
			{
				Section = "6.3.8.3",
				Equation = equation,
				Title = "Shear + Bending Interaction",
				CheckExpression = highShear
					? "M_Sd/M_Rd ≤ 1.4 - V_Sd/V_Rd"
					: "M_Sd/M_Rd ≤ 1.0 (low shear)",
				Demand = bendingRatio,
				Capacity = allowable,
				Utilization = utilization,
				Passed = bendingRatio <= allowable,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "M_Sd", Description = "Design bending moment", Value = M_Sd, Unit = "kNm" },
					new() { Symbol = "M_Rd", Description = "Design bending resistance", Value = M_Rd, Unit = "kNm" },
					new() { Symbol = "V_Sd", Description = "Design shear force", Value = V_Sd, Unit = "kN" },
					new() { Symbol = "V_Rd", Description = "Design shear resistance", Value = V_Rd, Unit = "kN" },
					new() { Symbol = "V_Sd/V_Rd", Description = "Shear utilization", Value = shearRatio, Unit = "-" },
					new() { Symbol = "M_Sd/M_Rd", Description = "Bending utilization", Value = bendingRatio, Unit = "-" },
					new() { Symbol = "0.4·V_Rd", Description = "High shear threshold", Value = 0.4 * V_Rd, Unit = "kN" },
					new() { Symbol = "Allowable", Description = $"Maximum M_Sd/M_Rd (Eq. {equation})", Value = allowable, Unit = "-" },
				}
			};
		}
	}
}
