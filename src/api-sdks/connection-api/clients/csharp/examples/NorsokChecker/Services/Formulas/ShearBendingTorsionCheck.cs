using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.8.4 — Interaction Shear, Bending Moment and Torsional Moment (Eq. 6.33)
	///
	/// M_Sd/M_Red,Rd ≤ 1.4 - V_Sd/V_Rd    for V_Sd ≥ 0.4·V_Rd
	/// M_Sd/M_Red,Rd ≤ 1.0                 for V_Sd &lt; 0.4·V_Rd
	///
	/// Where M_Red,Rd = W·f_m,Red / γ_M
	///       f_m,Red = f_m · √(1 - 3·(τ_T,Sd/f_d)²)
	///       τ_T,Sd = M_T,Sd / (2·π·R²·t)
	///       f_d = f_y / γ_M
	/// </summary>
	public static class ShearBendingTorsionCheck
	{
		/// <param name="M_Sd">Design bending moment [kNm]</param>
		/// <param name="V_Sd">Design shear force [kN]</param>
		/// <param name="V_Rd">Design shear resistance [kN]</param>
		/// <param name="M_T_Sd">Design torsional moment [kNm]</param>
		/// <param name="W">Elastic section modulus [mm³]</param>
		/// <param name="f_m">Characteristic bending strength [MPa] (from §6.3.4)</param>
		/// <param name="f_y">Characteristic yield strength [MPa]</param>
		/// <param name="D">Outside diameter [mm]</param>
		/// <param name="t">Wall thickness [mm]</param>
		/// <param name="gammaM">Material factor γ_M</param>
		public static NorsokFormulaResult Evaluate(
			double M_Sd, double V_Sd, double V_Rd,
			double M_T_Sd, double W, double f_m,
			double f_y, double D, double t,
			double gammaM = 1.15)
		{
			double R = D / 2.0;  // radius [mm]
			double f_d = f_y / gammaM;

			// Torsional shear stress τ_T,Sd = M_T,Sd / (2·π·R²·t)
			// M_T_Sd is in kNm, convert to N·mm
			double M_T_Sd_Nmm = M_T_Sd * 1e6;
			double tau_T_Sd = M_T_Sd_Nmm / (2.0 * Math.PI * R * R * t);

			// Reduced bending strength
			double torsionRatio = tau_T_Sd / f_d;
			double reductionFactor = Math.Sqrt(Math.Max(0, 1.0 - 3.0 * torsionRatio * torsionRatio));
			double f_m_Red = f_m * reductionFactor;

			// M_Red,Rd = W · f_m,Red / γ_M [N·mm] → kNm
			double M_Red_Rd_Nmm = W * f_m_Red / gammaM;
			double M_Red_Rd_kNm = M_Red_Rd_Nmm / 1e6;

			double shearRatio = V_Sd / V_Rd;
			double bendingRatio = M_Red_Rd_kNm > 0 ? M_Sd / M_Red_Rd_kNm : double.PositiveInfinity;

			bool highShear = V_Sd >= 0.4 * V_Rd;
			double allowable = highShear ? 1.4 - shearRatio : 1.0;

			double utilization = allowable > 0 ? bendingRatio / allowable : double.PositiveInfinity;

			return new NorsokFormulaResult
			{
				Section = "6.3.8.4",
				Equation = "6.33",
				Title = "Shear + Bending + Torsion",
				CheckExpression = highShear
					? "M_Sd/M_Red,Rd ≤ 1.4 - V_Sd/V_Rd"
					: "M_Sd/M_Red,Rd ≤ 1.0 (low shear)",
				Demand = bendingRatio,
				Capacity = allowable,
				Utilization = utilization,
				Passed = bendingRatio <= allowable,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "M_Sd", Description = "Design bending moment", Value = M_Sd, Unit = "kNm" },
					new() { Symbol = "M_T,Sd", Description = "Design torsional moment", Value = M_T_Sd, Unit = "kNm" },
					new() { Symbol = "V_Sd", Description = "Design shear force", Value = V_Sd, Unit = "kN" },
					new() { Symbol = "V_Rd", Description = "Design shear resistance", Value = V_Rd, Unit = "kN" },
					new() { Symbol = "R", Description = "Tubular radius", Value = R, Unit = "mm" },
					new() { Symbol = "t", Description = "Wall thickness", Value = t, Unit = "mm" },
					new() { Symbol = "f_y", Description = "Yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "f_d", Description = "Design strength = f_y/γ_M", Value = f_d, Unit = "MPa" },
					new() { Symbol = "τ_T,Sd", Description = "Torsional shear stress", Value = tau_T_Sd, Unit = "MPa" },
					new() { Symbol = "τ/f_d", Description = "Torsion-to-strength ratio", Value = torsionRatio, Unit = "-" },
					new() { Symbol = "f_m", Description = "Bending strength (§6.3.4)", Value = f_m, Unit = "MPa" },
					new() { Symbol = "f_m,Red", Description = "Reduced bending strength", Value = f_m_Red, Unit = "MPa" },
					new() { Symbol = "W", Description = "Elastic section modulus", Value = W, Unit = "mm³" },
					new() { Symbol = "M_Red,Rd", Description = "Reduced bending resistance", Value = M_Red_Rd_kNm, Unit = "kNm" },
					new() { Symbol = "γ_M", Description = "Material factor", Value = gammaM, Unit = "-" },
				}
			};
		}
	}
}
