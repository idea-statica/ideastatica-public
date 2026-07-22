using NorsokChecker.Models;
using NorsokChecker.Services.Norsok64;

namespace NorsokChecker.Services.Formulas
{
	/// <summary>
	/// NORSOK N-004 §6.4 — Tubular Joint Checks (report adapter).
	///
	/// The actual code-check math lives in <see cref="Norsok64Engine"/> (a faithful port of the
	/// verified <c>n64.py</c> reference engine). This adapter converts the app's mm/MPa/kN
	/// <see cref="TubularJointGeometry"/> + brace forces + chord stresses into a pure-SI
	/// <see cref="Joint64Input"/>, runs the check, and packs the result into a
	/// <see cref="NorsokFormulaResult"/> for the report.
	///
	/// §6.4.3.2  Basic resistance:  N_Rd = fy·T²/(γM·sinθ)·Qu·Qf     (Eq. 6.52)
	///                              M_Rd = fy·T²·d/(γM·sinθ)·Qu·Qf   (Eq. 6.53)
	/// §6.4.3.3  Strength factor Qu (Table 6-3);  §6.4.3.4 Chord action Qf (Eq. 6.54–6.55)
	/// §6.4.3.6  Interaction:  |N|/N_Rd + (M_ip/M_ip,Rd)² + |M_op|/M_op,Rd ≤ 1.0  (Eq. 6.57)
	/// </summary>
	public static class TubularJointCheck
	{
		/// <summary>Map the app's single-select <see cref="JointType"/> to the engine's K/Y/X fractions.</summary>
		private static (double frK, double frY, double frX) Fractions(JointType type) => type switch
		{
			JointType.K => (1.0, 0.0, 0.0),
			JointType.X => (0.0, 0.0, 1.0),
			_ => (0.0, 1.0, 0.0),   // T_Y → Y
		};

		private static Joint64Class ActiveClass(JointType type) => type switch
		{
			JointType.K => Joint64Class.K,
			JointType.X => Joint64Class.X,
			_ => Joint64Class.Y,
		};

		/// <summary>
		/// Build a report card from an auto-topology §6.4 check row (JointCheckOrchestrator output).
		/// The classification (frK/frY/frX) came from the K/Y/X force-decomposition classifier, the
		/// chord stresses from the Begin/End averaging — no manual joint-type input involved.
		/// </summary>
		public static NorsokFormulaResult BuildResultFromRow(JointCheckRow row, string loadCaseName)
		{
			var r = row.Engine!;
			var inp = row.Inputs!;
			var cl = row.Classification!;
			var dom = Enum.Parse<Joint64Class>(row.DomClass);
			var ac = r.PerClass[dom];

			double nRdKn = r.NRdWeighted / 1e3;
			double axialTerm = r.NRdWeighted > 0 ? Math.Abs(inp.NSd) / r.NRdWeighted : 0.0;
			double ipbTerm = r.MRdIp > 0 ? Math.Pow(Math.Abs(inp.MipSd) / r.MRdIp, 2) : 0.0;
			double opbTerm = r.MRdOp > 0 ? Math.Abs(inp.MopSd) / r.MRdOp : 0.0;

			string clsStr = $"K {cl.FrK:P0} / Y {cl.FrY:P0} / X {cl.FrX:P0}";
			string title = $"Tubular Joint — {row.Name} ({clsStr})";
			if (r.ChordOverstressed) title += " — CHORD OVERSTRESSED";
			else if (!r.WithinRange) title += " — outside validity range (6.4.3.1)";

			double utilDisplay = double.IsInfinity(r.UtilWeighted) ? 999.0 : r.UtilWeighted;

			var variables = new List<FormulaVariable>
			{
				new() { Symbol = "D", Description = "chord outside diameter", Value = inp.D * 1000, Unit = "mm" },
				new() { Symbol = "T", Description = "chord wall thickness", Value = inp.T * 1000, Unit = "mm" },
				new() { Symbol = "d", Description = "brace outside diameter", Value = inp.d * 1000, Unit = "mm" },
				new() { Symbol = "t", Description = "brace wall thickness", Value = inp.t * 1000, Unit = "mm" },
				new() { Symbol = "θ", Description = "brace-to-chord angle (auto, from member axes)", Value = inp.ThetaDeg, Unit = "°" },
				new() { Symbol = "β", Description = "d/D (validity: 0.2–1.0)", Value = r.Beta, Unit = "-" },
				new() { Symbol = "γ", Description = "D/(2T) (validity: 10–50)", Value = r.Gamma, Unit = "-" },
				new() { Symbol = "τ", Description = "t/T", Value = r.Tau, Unit = "-" },
				new() { Symbol = "frK", Description = "K fraction (force-balance classification)", Value = cl.FrK, Unit = "-" },
				new() { Symbol = "frY", Description = "Y fraction", Value = cl.FrY, Unit = "-" },
				new() { Symbol = "frX", Description = "X fraction", Value = cl.FrX, Unit = "-" },
				new() { Symbol = "Q_g", Description = "gap factor (first K gap)", Value = r.Qg, Unit = "-" },
				new() { Symbol = "Qu_axial", Description = $"strength factor — axial, {r.LoadAxial}, dominant class {row.DomClass}", Value = ac.QuAxial, Unit = "-" },
				new() { Symbol = "Qu_IPB", Description = "strength factor — in-plane bending", Value = r.QuIpb, Unit = "-" },
				new() { Symbol = "Qu_OPB", Description = "strength factor — out-of-plane bending", Value = r.QuOpb, Unit = "-" },
				new() { Symbol = "Qf_axial", Description = $"chord action factor (dominant class {row.DomClass})", Value = ac.QfAxial, Unit = "-" },
				new() { Symbol = "Qf_moment", Description = "chord action factor for moments", Value = r.QfMoment, Unit = "-" },
				new() { Symbol = "σ_a", Description = "chord axial stress (avg Begin/End; + tension)", Value = inp.SigmaASd / 1e6, Unit = "MPa" },
				new() { Symbol = "σ_my", Description = "chord in-plane bending stress (+ compression at footprint)", Value = inp.SigmaMySd / 1e6, Unit = "MPa" },
				new() { Symbol = "σ_mz", Description = "chord out-of-plane bending stress", Value = inp.SigmaMzSd / 1e6, Unit = "MPa" },
				new() { Symbol = "N_Rd", Description = "joint axial resistance, weighted over K/Y/X (Eq. 6.52)", Value = nRdKn, Unit = "kN" },
				new() { Symbol = "M_ip,Rd", Description = "in-plane bending resistance (Eq. 6.53)", Value = r.MRdIp / 1e3, Unit = "kNm" },
				new() { Symbol = "M_op,Rd", Description = "out-of-plane bending resistance (Eq. 6.53)", Value = r.MRdOp / 1e3, Unit = "kNm" },
				new() { Symbol = "N_Sd", Description = "brace axial force (+ tension)", Value = inp.NSd / 1e3, Unit = "kN" },
				new() { Symbol = "M_ip,Sd", Description = "brace in-plane bending", Value = inp.MipSd / 1e3, Unit = "kNm" },
				new() { Symbol = "M_op,Sd", Description = "brace out-of-plane bending", Value = inp.MopSd / 1e3, Unit = "kNm" },
				new() { Symbol = "|N|/N_Rd", Description = "axial utilization term", Value = axialTerm, Unit = "-" },
				new() { Symbol = "(M_ip/M_ip,Rd)²", Description = "in-plane bending term (squared)", Value = ipbTerm, Unit = "-" },
				new() { Symbol = "|M_op|/M_op,Rd", Description = "out-of-plane bending term", Value = opbTerm, Unit = "-" },
				new() { Symbol = "γ_M", Description = "material factor", Value = inp.GammaM, Unit = "-" },
			};
			// K per-gap breakdown (KT / multi-gap balancing)
			for (int i = 0; i < r.KTerms.Count; i++)
			{
				var k = r.KTerms[i];
				variables.Add(new FormulaVariable
				{
					Symbol = $"K gap {i + 1}",
					Description = $"frK={k.FrK:F3}, g={k.GapM * 1000:F0} mm, Q_g={k.Qg:F3}, N_Rd={k.NRd / 1e3:F1} kN",
					Value = k.FrK, Unit = "-",
				});
			}

			return new NorsokFormulaResult
			{
				Section = "6.4.3.6",
				Equation = "6.57",
				Title = title,
				CheckExpression = "|N_Sd|/N_Rd + (M_ip,Sd/M_ip,Rd)² + |M_op,Sd|/M_op,Rd ≤ 1.0",
				Formula = @"N_{Rd} = \frac{f_y \cdot T^2}{\gamma_M \cdot \sin\theta} \cdot Q_u \cdot Q_f",
				FormulaSubstituted =
					$"N_Rd(weighted {clsStr}) = {nRdKn:F1} kN;  worst LC: {loadCaseName}",
				Demand = utilDisplay,
				Capacity = 1.0,
				Utilization = utilDisplay,
				Passed = row.Passed,
				Variables = variables,
			};
		}

		/// <summary>
		/// Evaluate the §6.4 joint interaction (Eq. 6.57) for one brace load case.
		/// <paramref name="My_Sd_kNm"/> is the in-plane brace moment, <paramref name="Mz_Sd_kNm"/> the
		/// out-of-plane one; chord stresses σ are in MPa. Returns a <see cref="NorsokFormulaResult"/>
		/// with full variable traceability.
		/// </summary>
		public static NorsokFormulaResult EvaluateJointInteraction(
			TubularJointGeometry joint,
			double N_Sd_kN, double My_Sd_kNm, double Mz_Sd_kNm,
			double sigmaA_chord, double sigmaMy_chord, double sigmaMz_chord,
			double gammaM = 1.15)
		{
			var (frK, frY, frX) = Fractions(joint.JointType);

			var inp = Joint64Input.FromKn(
				D: joint.D, T: joint.T, fyChord: joint.FyChord,
				d: joint.d, t: joint.t, fyBrace: joint.FyBrace,
				thetaDeg: joint.ThetaDeg, g: joint.Gap,
				frK: frK, frY: frY, frX: frX,
				nSdKn: N_Sd_kN, mipSdKnm: My_Sd_kNm, mopSdKnm: Mz_Sd_kNm,
				sigmaASdMpa: sigmaA_chord, sigmaMySdMpa: sigmaMy_chord, sigmaMzSdMpa: sigmaMz_chord,
				gammaM: gammaM);

			var res = Norsok64Engine.CheckJoint(inp);
			var ac = res.PerClass[ActiveClass(joint.JointType)];

			// SI → engineering units for display
			double nRdKn = res.NRdWeighted / 1e3;      // N   → kN
			double myRdKnm = res.MRdIp / 1e3;          // N·m → kNm
			double mzRdKnm = res.MRdOp / 1e3;          // N·m → kNm

			double axialTerm = res.NRdWeighted > 0 ? Math.Abs(inp.NSd) / res.NRdWeighted : 0.0;
			double ipbTerm = res.MRdIp > 0 ? Math.Pow(Math.Abs(inp.MipSd) / res.MRdIp, 2) : 0.0;
			double opbTerm = res.MRdOp > 0 ? Math.Abs(inp.MopSd) / res.MRdOp : 0.0;

			string jointTypeStr = joint.JointType switch
			{
				JointType.K => "K", JointType.T_Y => "T/Y", JointType.X => "X", _ => "?"
			};

			string title = $"Tubular Joint ({jointTypeStr}) — Interaction";
			if (res.ChordOverstressed) title += " — CHORD OVERSTRESSED";
			else if (!res.WithinRange) title += " — outside validity range (6.4.3.1)";

			double utilDisplay = double.IsInfinity(res.UtilWeighted) ? 999.0 : res.UtilWeighted;

			return new NorsokFormulaResult
			{
				Section = "6.4.3.6",
				Equation = "6.57",
				Title = title,
				CheckExpression = "|N_Sd|/N_Rd + (M_ip,Sd/M_ip,Rd)² + |M_op,Sd|/M_op,Rd ≤ 1.0",
				Formula = @"N_{Rd} = \frac{f_y \cdot T^2}{\gamma_M \cdot \sin\theta} \cdot Q_u \cdot Q_f",
				FormulaSubstituted = $"N_Rd = {joint.FyChord:F0}×{joint.T:F1}²/({gammaM:F2}×sin{joint.ThetaDeg:F0}°) × {ac.QuAxial:F2}×{ac.QfAxial:F3} = {nRdKn:F1} kN",
				Demand = utilDisplay,
				Capacity = 1.0,
				Utilization = utilDisplay,
				Passed = res.Passed,
				Variables = new List<FormulaVariable>
				{
					// Joint geometry
					new() { Symbol = "D", Description = "chord outside diameter", Value = joint.D, Unit = "mm" },
					new() { Symbol = "T", Description = "chord wall thickness", Value = joint.T, Unit = "mm" },
					new() { Symbol = "d", Description = "brace outside diameter", Value = joint.d, Unit = "mm" },
					new() { Symbol = "t", Description = "brace wall thickness", Value = joint.t, Unit = "mm" },
					new() { Symbol = "θ", Description = "brace-to-chord angle", Value = joint.ThetaDeg, Unit = "°" },

					// Derived parameters
					new() { Symbol = "β", Description = $"d/D = {joint.d:F0}/{joint.D:F0} (validity: 0.2–1.0)", Value = res.Beta, Unit = "-" },
					new() { Symbol = "γ", Description = $"D/(2T) = {joint.D:F0}/(2×{joint.T:F0}) (validity: 10–50)", Value = res.Gamma, Unit = "-" },
					new() { Symbol = "τ", Description = "t/T (thickness ratio)", Value = res.Tau, Unit = "-" },

					// Q factors
					new() { Symbol = "Q_β", Description = "geometric factor (Table 6-3 note a)", Value = res.QBeta, Unit = "-" },
					new() { Symbol = "Q_g", Description = "gap factor (Table 6-3 note b; K only)", Value = res.Qg, Unit = "-" },
					new() { Symbol = "Qu_axial", Description = $"strength factor — axial, {res.LoadAxial}, {jointTypeStr}", Value = ac.QuAxial, Unit = "-" },
					new() { Symbol = "Qu_IPB", Description = "strength factor — in-plane bending", Value = res.QuIpb, Unit = "-" },
					new() { Symbol = "Qu_OPB", Description = "strength factor — out-of-plane bending", Value = res.QuOpb, Unit = "-" },
					new() { Symbol = "Qf_axial", Description = $"chord action factor (C1={ac.CAxial.C1:F2}, C2={ac.CAxial.C2:F2}, C3={ac.CAxial.C3:F2})", Value = ac.QfAxial, Unit = "-" },
					new() { Symbol = "Qf_moment", Description = "chord action factor for moments", Value = res.QfMoment, Unit = "-" },

					// Resistances
					new() { Symbol = "N_Rd", Description = "joint axial resistance, weighted (Eq. 6.52)", Value = nRdKn, Unit = "kN" },
					new() { Symbol = "M_ip,Rd", Description = "joint in-plane bending resistance (Eq. 6.53)", Value = myRdKnm, Unit = "kNm" },
					new() { Symbol = "M_op,Rd", Description = "joint out-of-plane bending resistance (Eq. 6.53)", Value = mzRdKnm, Unit = "kNm" },

					// Actions
					new() { Symbol = "N_Sd", Description = "design axial force in brace", Value = N_Sd_kN, Unit = "kN" },
					new() { Symbol = "M_ip,Sd", Description = "design in-plane bending in brace", Value = My_Sd_kNm, Unit = "kNm" },
					new() { Symbol = "M_op,Sd", Description = "design out-of-plane bending in brace", Value = Mz_Sd_kNm, Unit = "kNm" },

					// Interaction terms (Eq. 6.57)
					new() { Symbol = "|N|/N_Rd", Description = "axial utilization term", Value = axialTerm, Unit = "-" },
					new() { Symbol = "(M_ip/M_ip,Rd)²", Description = "in-plane bending term (squared)", Value = ipbTerm, Unit = "-" },
					new() { Symbol = "|M_op|/M_op,Rd", Description = "out-of-plane bending term", Value = opbTerm, Unit = "-" },

					new() { Symbol = "γ_M", Description = "material factor (Table 6-1)", Value = gammaM, Unit = "-" },
				}
			};
		}
	}
}
