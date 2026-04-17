using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.3 — Axial Compression (Equations 6.2–6.8)
	///
	/// N_Sd ≤ N_c,Rd = A · f_c / γ_M
	///
	/// Where f_c depends on column slenderness λ and local buckling strength f_cl.
	/// </summary>
	public static class AxialCompressionCheck
	{
		public const double Ce = 0.3;  // Critical elastic buckling coefficient
		public const double E = 2.1e5; // Young's modulus [MPa]

		/// <param name="N_Sd">Design axial compressive force [kN] (positive = compression)</param>
		/// <param name="A">Cross-sectional area [mm²]</param>
		/// <param name="f_y">Characteristic yield strength [MPa]</param>
		/// <param name="D">Outside diameter [mm]</param>
		/// <param name="t">Wall thickness [mm]</param>
		/// <param name="k">Effective length factor (Table 6-2)</param>
		/// <param name="l">Unbraced length [mm]</param>
		/// <param name="i">Radius of gyration [mm]</param>
		/// <param name="gammaM">Material factor γ_M</param>
		public static NorsokFormulaResult Evaluate(
			double N_Sd, double A, double f_y,
			double D, double t,
			double k, double l, double i,
			double gammaM = 1.15)
		{
			// Elastic local buckling strength (Eq. 6.8 context)
			double f_cle = 2.0 * Ce * E * t / D;

			// Characteristic local buckling strength f_cl (Eq. 6.6–6.8)
			double ratio = f_y / f_cle;
			double f_cl;
			string f_cl_eq;

			if (ratio <= 0.170)
			{
				f_cl = f_y;
				f_cl_eq = "6.6 (f_cl = f_y)";
			}
			else if (ratio <= 1.911)
			{
				f_cl = (1.047 - 0.274 * ratio) * f_y;
				f_cl_eq = "6.7 (f_cl = (1.047 - 0.274·f_y/f_cle)·f_y)";
			}
			else
			{
				f_cl = f_cle;
				f_cl_eq = "6.8 (f_cl = f_cle)";
			}

			// Euler buckling strength
			double kl_i = k * l / i;
			double f_E = Math.PI * Math.PI * E / (kl_i * kl_i);

			// Column slenderness λ (Eq. 6.5)
			double lambda = Math.Sqrt(f_cl / f_E);

			// Characteristic compressive strength f_c (Eq. 6.3 or 6.4)
			double f_c;
			string f_c_eq;

			if (lambda <= 1.34)
			{
				f_c = (1.0 - 0.28 * lambda * lambda) * f_cl;
				f_c_eq = "6.3";
			}
			else
			{
				f_c = (0.9 / (lambda * lambda)) * f_cl;
				f_c_eq = "6.4";
			}

			// Design compressive resistance (Eq. 6.2)
			double N_c_Rd_N = A * f_c / gammaM;
			double N_c_Rd_kN = N_c_Rd_N / 1000.0;

			double utilization = N_Sd > 0 ? N_Sd / N_c_Rd_kN : 0;

			return new NorsokFormulaResult
			{
				Section = "6.3.3",
				Equation = "6.2",
				Title = "Axial Compression",
				CheckExpression = "N_Sd ≤ N_c,Rd = A · f_c / γ_M",
				Demand = N_Sd,
				Capacity = N_c_Rd_kN,
				Utilization = utilization,
				Passed = utilization <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "N_Sd", Description = "Design axial compressive force", Value = N_Sd, Unit = "kN" },
					new() { Symbol = "A", Description = "Cross-sectional area", Value = A, Unit = "mm²" },
					new() { Symbol = "f_y", Description = "Characteristic yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "D", Description = "Outside diameter", Value = D, Unit = "mm" },
					new() { Symbol = "t", Description = "Wall thickness", Value = t, Unit = "mm" },
					new() { Symbol = "E", Description = "Young's modulus", Value = E, Unit = "MPa" },
					new() { Symbol = "C_e", Description = "Elastic buckling coefficient", Value = Ce, Unit = "-" },
					new() { Symbol = "f_cle", Description = "Elastic local buckling strength", Value = f_cle, Unit = "MPa" },
					new() { Symbol = "f_y/f_cle", Description = "Local buckling ratio", Value = ratio, Unit = "-" },
					new() { Symbol = "f_cl", Description = $"Local buckling strength (Eq. {f_cl_eq})", Value = f_cl, Unit = "MPa" },
					new() { Symbol = "k", Description = "Effective length factor (Table 6-2)", Value = k, Unit = "-" },
					new() { Symbol = "l", Description = "Unbraced length", Value = l, Unit = "mm" },
					new() { Symbol = "i", Description = "Radius of gyration", Value = i, Unit = "mm" },
					new() { Symbol = "kl/i", Description = "Slenderness ratio", Value = kl_i, Unit = "-" },
					new() { Symbol = "f_E", Description = "Euler buckling strength", Value = f_E, Unit = "MPa" },
					new() { Symbol = "λ", Description = "Column slenderness parameter", Value = lambda, Unit = "-" },
					new() { Symbol = "f_c", Description = $"Compressive strength (Eq. {f_c_eq})", Value = f_c, Unit = "MPa" },
					new() { Symbol = "γ_M", Description = "Material factor", Value = gammaM, Unit = "-" },
					new() { Symbol = "N_c,Rd", Description = "Design compressive resistance", Value = N_c_Rd_kN, Unit = "kN" },
				}
			};
		}
	}
}
