using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.3 — Axial Compression (Equations 6.2–6.8)
	/// N_Sd ≤ N_c,Rd = A · f_c / γ_M
	/// </summary>
	public static class AxialCompressionCheck
	{
		public const double Ce = 0.3;
		public const double E = 2.1e5; // MPa

		public static NorsokFormulaResult Evaluate(
			double N_Sd, double A, double f_y,
			double D, double t,
			double k, double l, double i,
			double gammaM = 1.15)
		{
			double f_cle = 2.0 * Ce * E * t / D;
			double ratio = f_y / f_cle;
			double f_cl;
			string f_cl_eq;

			if (ratio <= 0.170)
			{
				f_cl = f_y;
				f_cl_eq = "Eq. 6.6: f_cl = f_y (compact)";
			}
			else if (ratio <= 1.911)
			{
				f_cl = (1.047 - 0.274 * ratio) * f_y;
				f_cl_eq = $"Eq. 6.7: f_cl = (1.047 - 0.274 × {ratio:F3}) × {f_y:F1} = {f_cl:F1} MPa";
			}
			else
			{
				f_cl = f_cle;
				f_cl_eq = "Eq. 6.8: f_cl = f_cle (very slender)";
			}

			double kl_i = k * l / i;
			double f_E = Math.PI * Math.PI * E / (kl_i * kl_i);
			double lambda = Math.Sqrt(f_cl / f_E);

			double f_c;
			string f_c_eq;
			if (lambda <= 1.34)
			{
				f_c = (1.0 - 0.28 * lambda * lambda) * f_cl;
				f_c_eq = $"Eq. 6.3: f_c = (1.0 - 0.28 × {lambda:F3}²) × {f_cl:F1} = {f_c:F1} MPa";
			}
			else
			{
				f_c = (0.9 / (lambda * lambda)) * f_cl;
				f_c_eq = $"Eq. 6.4: f_c = 0.9/{lambda:F3}² × {f_cl:F1} = {f_c:F1} MPa";
			}

			double N_c_Rd_N = A * f_c / gammaM;
			double N_c_Rd_kN = N_c_Rd_N / 1000.0;
			double utilization = N_Sd > 0 ? N_Sd / N_c_Rd_kN : 0;

			return new NorsokFormulaResult
			{
				Section = "6.3.3",
				Equation = "6.2",
				Title = "Axial Compression",
				CheckExpression = "N_Sd ≤ N_c,Rd",
				Formula = "N_c,Rd = A · f_c / γ_M",
				FormulaSubstituted = $"N_c,Rd = {A:F0} × {f_c:F1} / {gammaM:F2} = {N_c_Rd_kN:F1} kN",
				Demand = N_Sd,
				Capacity = N_c_Rd_kN,
				Utilization = utilization,
				Passed = utilization <= 1.0,
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "N_Sd", Description = "design axial compressive force", Value = N_Sd, Unit = "kN" },
					new() { Symbol = "A", Description = "cross-sectional area", Value = A, Unit = "mm²" },
					new() { Symbol = "f_y", Description = "characteristic yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "D", Description = "outside diameter", Value = D, Unit = "mm" },
					new() { Symbol = "t", Description = "wall thickness", Value = t, Unit = "mm" },
					new() { Symbol = "f_cle", Description = $"elastic local buckling = 2·C_e·E·t/D = 2×{Ce}×{E:F0}×{t:F1}/{D:F1}", Value = f_cle, Unit = "MPa" },
					new() { Symbol = "f_y/f_cle", Description = "local buckling ratio", Value = ratio, Unit = "-" },
					new() { Symbol = "f_cl", Description = f_cl_eq, Value = f_cl, Unit = "MPa" },
					new() { Symbol = "k", Description = "effective length factor (Table 6-2)", Value = k, Unit = "-" },
					new() { Symbol = "l", Description = "unbraced length", Value = l, Unit = "mm" },
					new() { Symbol = "i", Description = "radius of gyration", Value = i, Unit = "mm" },
					new() { Symbol = "kl/i", Description = "slenderness ratio", Value = kl_i, Unit = "-" },
					new() { Symbol = "f_E", Description = $"Euler buckling = π²E/(kl/i)²", Value = f_E, Unit = "MPa" },
					new() { Symbol = "λ", Description = "column slenderness = √(f_cl/f_E)", Value = lambda, Unit = "-" },
					new() { Symbol = "f_c", Description = f_c_eq, Value = f_c, Unit = "MPa" },
					new() { Symbol = "γ_M", Description = "partial safety factor (NORSOK N-004 Table 6-1)", Value = gammaM, Unit = "-" },
					new() { Symbol = "N_c,Rd", Description = "design compressive resistance", Value = N_c_Rd_kN, Unit = "kN" },
				}
			};
		}
	}
}
