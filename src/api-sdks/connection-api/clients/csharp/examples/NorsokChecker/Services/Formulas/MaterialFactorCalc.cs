using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.3.7 — Material Factor (Equations 6.22–6.25)
	///
	/// For class 4 cross-sections (f_y/f_cle > 0.170), γ_M is variable:
	///   γ_M = 1.15                    for λ_s &lt; 0.5
	///   γ_M = 0.85 + 0.60·λ_s        for 0.5 ≤ λ_s ≤ 1.0
	///   γ_M = 1.45                    for λ_s > 1.0
	///
	/// For class 1-3 (f_y/f_cle ≤ 0.170), γ_M = 1.15.
	/// </summary>
	public static class MaterialFactorCalc
	{
		/// <summary>
		/// Calculate γ_M based on the combined stress state (Eq. 6.22).
		/// Returns both the factor and a formula result for reporting.
		/// </summary>
		/// <param name="sigma_c_Sd">Combined axial + bending stress [MPa] (Eq. 6.25)</param>
		/// <param name="f_cl">Local buckling strength [MPa] (from §6.3.3)</param>
		/// <param name="f_y">Yield strength [MPa]</param>
		/// <param name="f_cle">Elastic local buckling strength [MPa]</param>
		/// <param name="sigma_p_Sd">Hoop stress from pressure [MPa] (0 if no hydrostatic)</param>
		/// <param name="f_h">Hoop buckling strength [MPa] (0 if no hydrostatic)</param>
		/// <param name="f_he">Elastic hoop buckling strength [MPa] (0 if no hydrostatic)</param>
		public static (double gammaM, NorsokFormulaResult result) Calculate(
			double sigma_c_Sd, double f_cl, double f_y, double f_cle,
			double sigma_p_Sd = 0, double f_h = 0, double f_he = 0)
		{
			// Check if class 4 (f_y/f_cle > 0.170)
			double ratio = f_cle > 0 ? f_y / f_cle : 0;
			bool isClass4 = ratio > 0.170;

			double gammaM;
			double lambda_s = 0;
			string gammaEquation;

			if (!isClass4)
			{
				gammaM = 1.15;
				gammaEquation = "1.15 (class 1-3)";
			}
			else
			{
				// ξ_c = f_y / f_cle (Eq. 6.24)
				double xi_c = f_cle > 0 ? f_y / f_cle : 0;

				// ξ_h = f_y / f_he (Eq. 6.24)
				double xi_h = f_he > 0 ? f_y / f_he : 0;

				// λ_s (Eq. 6.23) — without hydrostatic pressure, the hoop term is zero
				double term_c = f_cl > 0 ? (sigma_c_Sd * sigma_c_Sd) / (f_cl * f_cl) * xi_c : 0;
				double term_h = f_h > 0 ? (sigma_p_Sd * sigma_p_Sd) / (f_h * f_h) * xi_h : 0;
				lambda_s = Math.Sqrt(term_c + term_h);

				// Eq. 6.22
				if (lambda_s < 0.5)
				{
					gammaM = 1.15;
					gammaEquation = "6.22a (λ_s < 0.5)";
				}
				else if (lambda_s <= 1.0)
				{
					gammaM = 0.85 + 0.60 * lambda_s;
					gammaEquation = "6.22b (0.5 ≤ λ_s ≤ 1.0)";
				}
				else
				{
					gammaM = 1.45;
					gammaEquation = "6.22c (λ_s > 1.0)";
				}
			}

			var formulaResult = new NorsokFormulaResult
			{
				Section = "6.3.7",
				Equation = "6.22",
				Title = "Material Factor γ_M",
				CheckExpression = isClass4 ? "Variable γ_M for class 4 section" : "γ_M = 1.15 (class 1-3)",
				Demand = gammaM,
				Capacity = 1.45, // maximum possible
				Utilization = gammaM / 1.45,
				Passed = true, // material factor is informational, not a pass/fail check
				Variables = new List<FormulaVariable>
				{
					new() { Symbol = "f_y", Description = "Yield strength", Value = f_y, Unit = "MPa" },
					new() { Symbol = "f_cle", Description = "Elastic local buckling strength", Value = f_cle, Unit = "MPa" },
					new() { Symbol = "f_y/f_cle", Description = "Buckling ratio", Value = ratio, Unit = "-" },
					new() { Symbol = "Class", Description = isClass4 ? "Class 4 (shell)" : "Class 1-3", Value = isClass4 ? 4 : 1, Unit = "-" },
					new() { Symbol = "σ_c,Sd", Description = "Combined axial+bending stress (Eq. 6.25)", Value = sigma_c_Sd, Unit = "MPa" },
					new() { Symbol = "f_cl", Description = "Local buckling strength", Value = f_cl, Unit = "MPa" },
					new() { Symbol = "λ_s", Description = "Shell slenderness (Eq. 6.23)", Value = lambda_s, Unit = "-" },
					new() { Symbol = "γ_M", Description = $"Material factor (Eq. {gammaEquation})", Value = gammaM, Unit = "-" },
				}
			};

			return (gammaM, formulaResult);
		}
	}
}
