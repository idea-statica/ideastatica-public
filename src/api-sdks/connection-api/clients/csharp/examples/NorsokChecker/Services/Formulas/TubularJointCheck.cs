using NorsokChecker.Models;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.4 — Tubular Joint Checks
	///
	/// §6.4.3.2 Basic resistance:  NRd = (fy·T²)/(γM·sinθ) · Qu·Qf     (Eq. 6.52)
	///                              MRd = (fy·T²·d)/(γM·sinθ) · Qu·Qf   (Eq. 6.53)
	/// §6.4.3.3 Strength factor Qu: depends on joint type and β, γ        (Table 6-3)
	/// §6.4.3.4 Chord action factor Qf: accounts for chord stresses       (Eq. 6.54)
	/// §6.4.3.6 Interaction check:  NSd/NRd + (My/MyRd)² + Mz/MzRd ≤ 1.0 (Eq. 6.57)
	/// </summary>
	public static class TubularJointCheck
	{
		// ═══════════════════════════════════════════════════════════════
		//  §6.4.3.3  Strength factor Qu  (Table 6-3)
		// ═══════════════════════════════════════════════════════════════

		/// <summary>Geometric factor Qβ per note (a) of Table 6-3.</summary>
		public static double CalcQBeta(double beta)
		{
			if (beta > 0.6)
				return 0.3 / (beta * (1.0 - 0.833 * beta));
			return 1.0;
		}

		/// <summary>Gap factor Qg per note (b) of Table 6-3.</summary>
		public static double CalcQg(double gapRatio, double gamma, double fyBrace, double fyChord, double T)
		{
			double eta = (fyBrace * T) / (fyChord * T); // simplified: tau * fy_b/fy_c → for same material = tau

			if (gapRatio >= 0.05)
			{
				double qg = 1.0 + 0.2 * Math.Pow(1.0 - 2.8 * gapRatio, 3);
				return Math.Max(qg, 1.0);
			}
			if (gapRatio <= -0.05)
			{
				return 0.13 + 0.65 * Math.Pow(eta * gamma, 0.5);
			}
			// Linear interpolation for -0.05 < g/D < 0.05
			double qgPos = 1.0 + 0.2 * Math.Pow(1.0 - 2.8 * 0.05, 3);
			qgPos = Math.Max(qgPos, 1.0);
			double qgNeg = 0.13 + 0.65 * Math.Pow(eta * gamma, 0.5);
			double t_interp = (gapRatio + 0.05) / 0.1;
			return qgNeg + t_interp * (qgPos - qgNeg);
		}

		/// <summary>
		/// Compute Qu for a given joint type and action type.
		/// Table 6-3 values. β = d/D, γ = D/(2T).
		/// </summary>
		public static (double Qu, string description) CalcQu(
			JointType jointType, BraceActionType actionType,
			double beta, double gamma, double Qg, double QBeta)
		{
			switch (jointType)
			{
				case JointType.K:
					switch (actionType)
					{
						case BraceActionType.AxialTension:
							return (Math.Min(16.0 + 1.2 * gamma, 40.0 * beta) + beta * Qg,
								"K tension: min(16+1.2γ, 40β) + β·Qg");
						case BraceActionType.AxialCompression:
							return (Math.Min(16.0 + 1.2 * gamma, 40.0 * beta) + beta * Qg,
								"K compression: min(16+1.2γ, 40β) + β·Qg");
						case BraceActionType.InPlaneBending:
							return (5.0 + 0.7 * gamma * beta * 1.2,
								"K IPB: 5+0.7γβ^1.2");
						case BraceActionType.OutOfPlaneBending:
							return (2.5 + 4.5 * beta * beta * 0.2 * gamma * 2.6,
								"K OPB: 2.5+4.5β^0.2·γ^2.6");
					}
					break;

				case JointType.T_Y:
					switch (actionType)
					{
						case BraceActionType.AxialTension:
							return ((2.8 + 20.0 + 0.8 * gamma) * beta * 1.6,
								"T/Y tension: (2.8+20+0.8γ)·β^1.6");
						case BraceActionType.AxialCompression:
							return (Math.Min(2.8 + 36.0 * beta * 1.6, (2.8 + 12.0 + 0.1 * gamma) * beta * QBeta),
								"T/Y compression: min(2.8+36β^1.6, (2.8+12+0.1γ)β·Qβ)");
						case BraceActionType.InPlaneBending:
							return (5.0 + 0.7 * gamma * Math.Pow(beta, 1.2),
								"T/Y IPB: 5+0.7γ·β^1.2");
						case BraceActionType.OutOfPlaneBending:
							return (2.5 + 4.5 * Math.Pow(beta, 0.2) * gamma,
								"T/Y OPB: 2.5+4.5·β^0.2·γ");
					}
					break;

				case JointType.X:
					switch (actionType)
					{
						case BraceActionType.AxialTension:
							return (Math.Min((2.8 + 20.0 + 0.8 * gamma) * beta * 1.6, 30.0 * beta + 6.4 * gamma * 0.6 * beta * beta),
								"X tension: min((2.8+20+0.8γ)β^1.6, 30β+6.4γ^0.6·β²)");
						case BraceActionType.AxialCompression:
							return (Math.Min(2.8 + 36.0 * beta * 1.6, (2.8 + 12.0 + 0.1 * gamma) * beta * QBeta),
								"X compression: min(2.8+36β^1.6, (2.8+12+0.1γ)β·Qβ)");
						case BraceActionType.InPlaneBending:
							return (5.0 + 0.7 * gamma * Math.Pow(beta, 1.2),
								"X IPB: 5+0.7γ·β^1.2");
						case BraceActionType.OutOfPlaneBending:
							return (2.5 + 4.5 * Math.Pow(beta, 0.2) * gamma,
								"X OPB: 2.5+4.5·β^0.2·γ");
					}
					break;
			}
			return (1.0, "default");
		}

		// ═══════════════════════════════════════════════════════════════
		//  §6.4.3.4  Chord action factor Qf  (Eq. 6.54–6.55)
		// ═══════════════════════════════════════════════════════════════

		/// <summary>
		/// Coefficients C1, C2, C3 from Table 6-4.
		/// </summary>
		public static (double C1, double C2, double C3) GetQfCoefficients(
			JointType jointType, BraceActionType actionType, double beta)
		{
			if (actionType == BraceActionType.InPlaneBending || actionType == BraceActionType.OutOfPlaneBending)
				return (0.2, 0, 0.4); // All joints under brace moment loading

			switch (jointType)
			{
				case JointType.K:
					return (0.2, 0.2, 0.3);
				case JointType.T_Y:
					return (0.3, 0, 0.8);
				case JointType.X when actionType == BraceActionType.AxialTension:
					if (beta <= 0.9) return (0, 0, 0.4);
					if (beta >= 1.0) return (0.2, 0, 0.2);
					// Linear interpolation for 0.9 < β < 1.0
					double t = (beta - 0.9) / 0.1;
					return (t * 0.2, 0, 0.4 - t * 0.2);
				case JointType.X when actionType == BraceActionType.AxialCompression:
					if (beta <= 0.9) return (0.2, 0, 0.5);
					if (beta >= 1.0) return (-0.2, 0, 0.2);
					double tc = (beta - 0.9) / 0.1;
					return (0.2 - tc * 0.4, 0, 0.5 - tc * 0.3);
				default:
					return (0.2, 0, 0.4);
			}
		}

		/// <summary>
		/// Chord action factor Qf (Eq. 6.54).
		/// σa,Sd = chord axial stress, σmy,Sd = chord in-plane bending stress,
		/// σmz,Sd = chord out-of-plane bending stress.
		/// </summary>
		public static double CalcQf(
			double C1, double C2, double C3,
			double sigmaA, double sigmaMyC, double sigmaMzC, double fy)
		{
			if (fy <= 0) return 1.0;

			// A² from Eq. 6.55
			double A2 = Math.Pow(sigmaA / fy, 2)
				+ (Math.Pow(sigmaMyC, 2) + Math.Pow(sigmaMzC, 2)) / (1.62 * fy * fy);

			double Qf = 1.0 - C1 * (sigmaA / fy) - C2 * (sigmaMyC / (1.62 * fy)) - C3 * A2;
			return Math.Max(Qf, 0.0); // Qf should not be negative
		}

		// ═══════════════════════════════════════════════════════════════
		//  §6.4.3.2  Basic resistance  (Eq. 6.52, 6.53)
		// ═══════════════════════════════════════════════════════════════

		/// <summary>
		/// Joint design axial resistance NRd [kN] per Eq. 6.52.
		/// </summary>
		public static double CalcNRd(double fy, double T, double theta, double Qu, double Qf, double gammaM)
		{
			double sinT = Math.Sin(theta);
			if (sinT <= 0 || gammaM <= 0) return 0;
			double NRd_N = (fy * T * T) / (gammaM * sinT) * Qu * Qf;
			return NRd_N / 1000.0; // N → kN
		}

		/// <summary>
		/// Joint design bending resistance MRd [kNm] per Eq. 6.53.
		/// </summary>
		public static double CalcMRd(double fy, double T, double d, double theta, double Qu, double Qf, double gammaM)
		{
			double sinT = Math.Sin(theta);
			if (sinT <= 0 || gammaM <= 0) return 0;
			double MRd_Nmm = (fy * T * T * d) / (gammaM * sinT) * Qu * Qf;
			return MRd_Nmm / 1e6; // N·mm → kNm
		}

		// ═══════════════════════════════════════════════════════════════
		//  §6.4.3.6  Interaction check  (Eq. 6.57)
		// ═══════════════════════════════════════════════════════════════

		/// <summary>
		/// Evaluate the joint interaction check per Eq. 6.57.
		/// Returns a NorsokFormulaResult with full variable traceability.
		/// </summary>
		public static NorsokFormulaResult EvaluateJointInteraction(
			TubularJointGeometry joint,
			double N_Sd_kN, double My_Sd_kNm, double Mz_Sd_kNm,
			double sigmaA_chord, double sigmaMy_chord, double sigmaMz_chord,
			double gammaM = 1.15)
		{
			double beta = joint.Beta;
			double gamma = joint.Gamma;
			double tau = joint.Tau;
			double theta = joint.ThetaRad;
			double fy = joint.FyChord;

			// Qu for axial
			double QBeta = CalcQBeta(beta);
			double Qg = joint.JointType == JointType.K
				? CalcQg(joint.GapRatio, gamma, joint.FyBrace, fy, joint.T)
				: 1.0;

			bool isTension = N_Sd_kN > 0;
			var actionAxial = isTension ? BraceActionType.AxialTension : BraceActionType.AxialCompression;
			var (QuAxial, quAxialDesc) = CalcQu(joint.JointType, actionAxial, beta, gamma, Qg, QBeta);
			var (QuIPB, _) = CalcQu(joint.JointType, BraceActionType.InPlaneBending, beta, gamma, Qg, QBeta);
			var (QuOPB, _) = CalcQu(joint.JointType, BraceActionType.OutOfPlaneBending, beta, gamma, Qg, QBeta);

			// Qf
			var (C1a, C2a, C3a) = GetQfCoefficients(joint.JointType, actionAxial, beta);
			double QfAxial = CalcQf(C1a, C2a, C3a, sigmaA_chord, sigmaMy_chord, sigmaMz_chord, fy);
			var (C1m, C2m, C3m) = GetQfCoefficients(joint.JointType, BraceActionType.InPlaneBending, beta);
			double QfMoment = CalcQf(C1m, C2m, C3m, sigmaA_chord, sigmaMy_chord, sigmaMz_chord, fy);

			// Resistances
			double NRd = CalcNRd(fy, joint.T, theta, QuAxial, QfAxial, gammaM);
			double MyRd = CalcMRd(fy, joint.T, joint.d, theta, QuIPB, QfMoment, gammaM);
			double MzRd = CalcMRd(fy, joint.T, joint.d, theta, QuOPB, QfMoment, gammaM);

			// Interaction Eq. 6.57: NSd/NRd + (My,Sd/My,Rd)² + Mz,Sd/Mz,Rd ≤ 1.0
			double N_abs = Math.Abs(N_Sd_kN);
			double axialTerm = NRd > 0 ? N_abs / NRd : 0;
			double ipbTerm = MyRd > 0 ? Math.Pow(Math.Abs(My_Sd_kNm) / MyRd, 2) : 0;
			double opbTerm = MzRd > 0 ? Math.Abs(Mz_Sd_kNm) / MzRd : 0;
			double interaction = axialTerm + ipbTerm + opbTerm;

			string jointTypeStr = joint.JointType switch
			{
				JointType.K => "K", JointType.T_Y => "T/Y", JointType.X => "X", _ => "?"
			};

			return new NorsokFormulaResult
			{
				Section = "6.4.3.6",
				Equation = "6.57",
				Title = $"Tubular Joint ({jointTypeStr}) — Interaction",
				CheckExpression = "N_Sd/N_Rd + (M_y,Sd/M_y,Rd)² + M_z,Sd/M_z,Rd ≤ 1.0",
				Formula = @"N_{Rd} = \frac{f_y \cdot T^2}{\gamma_M \cdot \sin\theta} \cdot Q_u \cdot Q_f",
				FormulaSubstituted = $"N_Rd = {fy:F1}×{joint.T:F1}²/({gammaM:F2}×sin{joint.ThetaDeg:F0}°) × {QuAxial:F2}×{QfAxial:F3} = {NRd:F1} kN",
				Demand = interaction,
				Capacity = 1.0,
				Utilization = interaction,
				Passed = interaction <= 1.0,
				Variables = new List<FormulaVariable>
				{
					// Joint geometry
					new() { Symbol = "D", Description = "chord outside diameter", Value = joint.D, Unit = "mm" },
					new() { Symbol = "T", Description = "chord wall thickness", Value = joint.T, Unit = "mm" },
					new() { Symbol = "d", Description = "brace outside diameter", Value = joint.d, Unit = "mm" },
					new() { Symbol = "t", Description = "brace wall thickness", Value = joint.t, Unit = "mm" },
					new() { Symbol = "θ", Description = "brace-to-chord angle", Value = joint.ThetaDeg, Unit = "°" },

					// Derived parameters
					new() { Symbol = "β", Description = $"d/D = {joint.d:F0}/{joint.D:F0} (validity: 0.2–1.0)", Value = beta, Unit = "-" },
					new() { Symbol = "γ", Description = $"D/(2T) = {joint.D:F0}/(2×{joint.T:F0}) (validity: 10–50)", Value = gamma, Unit = "-" },
					new() { Symbol = "τ", Description = "t/T (thickness ratio)", Value = tau, Unit = "-" },

					// Qu
					new() { Symbol = "Qu_axial", Description = $"strength factor — {quAxialDesc}", Value = QuAxial, Unit = "-" },
					new() { Symbol = "Qu_IPB", Description = "strength factor — in-plane bending", Value = QuIPB, Unit = "-" },
					new() { Symbol = "Qu_OPB", Description = "strength factor — out-of-plane bending", Value = QuOPB, Unit = "-" },

					// Qf
					new() { Symbol = "Qf_axial", Description = $"chord action factor (C1={C1a:F1}, C2={C2a:F1}, C3={C3a:F1})", Value = QfAxial, Unit = "-" },
					new() { Symbol = "Qf_moment", Description = $"chord action factor for moments", Value = QfMoment, Unit = "-" },

					// Resistances
					new() { Symbol = "N_Rd", Description = "joint axial resistance (Eq. 6.52)", Value = NRd, Unit = "kN" },
					new() { Symbol = "M_y,Rd", Description = "joint in-plane bending resistance (Eq. 6.53)", Value = MyRd, Unit = "kNm" },
					new() { Symbol = "M_z,Rd", Description = "joint out-of-plane bending resistance (Eq. 6.53)", Value = MzRd, Unit = "kNm" },

					// Actions
					new() { Symbol = "N_Sd", Description = "design axial force in brace", Value = N_Sd_kN, Unit = "kN" },
					new() { Symbol = "M_y,Sd", Description = "design in-plane bending in brace", Value = My_Sd_kNm, Unit = "kNm" },
					new() { Symbol = "M_z,Sd", Description = "design out-of-plane bending in brace", Value = Mz_Sd_kNm, Unit = "kNm" },

					// Interaction terms
					new() { Symbol = "N/N_Rd", Description = "axial utilization term", Value = axialTerm, Unit = "-" },
					new() { Symbol = "(My/MyRd)²", Description = "IPB utilization term (squared)", Value = ipbTerm, Unit = "-" },
					new() { Symbol = "Mz/MzRd", Description = "OPB utilization term", Value = opbTerm, Unit = "-" },

					new() { Symbol = "γ_M", Description = "material factor (Table 6-1)", Value = gammaM, Unit = "-" },
				}
			};
		}
	}

	public enum BraceActionType
	{
		AxialTension,
		AxialCompression,
		InPlaneBending,
		OutOfPlaneBending
	}
}
