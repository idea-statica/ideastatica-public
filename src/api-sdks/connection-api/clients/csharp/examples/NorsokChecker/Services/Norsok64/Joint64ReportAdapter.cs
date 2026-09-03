using NorsokChecker.Models;

namespace NorsokChecker.Services.Norsok64
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
	/// §6.4.3.6  Interaction:  |N|/N_Rd + (M_y,Sd/M_y,Rd)² + |M_z,Sd|/M_z,Rd ≤ 1.0  (Eq. 6.57)
	///
	/// The symbols this adapter emits are the NORM's: eq (6.57) writes M_y / M_z and defines M_y as
	/// the in-plane and M_z as the out-of-plane moment. The engine's own properties stay MipSd /
	/// MRdIp — a developer reads those, a customer reads these.
	/// </summary>
	public static class Joint64ReportAdapter
	{
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

			// The governing load effect. Without this the card reports LoadCaseId = 0, which the
			// results table renders as "envelope" and the report drops the LE badge entirely —
			// the envelope is only useful if it says which state it came from.
			int govLeId = row.GovLeId;
			string govLeName = row.GovLeName ?? loadCaseName;

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
				new() { Symbol = "M_y,Rd", Description = "in-plane bending resistance (Eq. 6.53)", Value = r.MRdIp / 1e3, Unit = "kNm" },
				new() { Symbol = "M_z,Rd", Description = "out-of-plane bending resistance (Eq. 6.53)", Value = r.MRdOp / 1e3, Unit = "kNm" },
				new() { Symbol = "N_Sd", Description = "brace axial force (+ tension)", Value = inp.NSd / 1e3, Unit = "kN" },
				new() { Symbol = "M_y,Sd", Description = "brace in-plane bending", Value = inp.MipSd / 1e3, Unit = "kNm" },
				new() { Symbol = "M_z,Sd", Description = "brace out-of-plane bending", Value = inp.MopSd / 1e3, Unit = "kNm" },
				new() { Symbol = "|N|/N_Rd", Description = "axial utilization term", Value = axialTerm, Unit = "-" },
				new() { Symbol = "(M_y,Sd/M_y,Rd)²", Description = "in-plane bending term (squared)", Value = ipbTerm, Unit = "-" },
				new() { Symbol = "|M_z,Sd|/M_z,Rd", Description = "out-of-plane bending term", Value = opbTerm, Unit = "-" },
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
				CheckExpression = "|N_Sd|/N_Rd + (M_y,Sd/M_y,Rd)² + |M_z,Sd|/M_z,Rd ≤ 1.0",
				Formula = @"N_{Rd} = \frac{f_y \cdot T^2}{\gamma_M \cdot \sin\theta} \cdot Q_u \cdot Q_f",
				FormulaSubstituted =
					$"N_Rd(weighted {clsStr}) = {nRdKn:F1} kN;  governing LC: {govLeName}",
				Demand = utilDisplay,
				Capacity = 1.0,
				Utilization = utilDisplay,
				Passed = row.Passed,
				LoadCaseId = govLeId,
				LoadCaseName = govLeName,
				Variables = variables,
				JointDetail = row,

				// The caveat travels as DATA, so the roll-up can put it in the overview row. It used
				// to exist only inside `title` above, where nothing but a reader could see it.
				RangeQualifier = RangeQualifierOf(row.Name, r),
			};
		}

		/// <summary>
		/// "M1: θ = 20.0°, outside 30–90°" — which parameter breached its §6.4.3.1 range, and by what
		/// value. Null when the geometry is inside every range.
		///
		/// Built from <see cref="JointResult64.Validity"/> — the same dictionary the derivation table
		/// renders — so the overview and the detail sheet cannot disagree about which condition failed.
		/// The VALUES come from the result's actual geometry, never the clamped comparison pass; the
		/// engine keeps them that way deliberately (see CheckJoint's closing block).
		///
		/// Several breaches are named together rather than only the first: a brace can be outside two
		/// ranges at once, and reporting one of them would understate the caveat exactly where the
		/// reader is scanning for it.
		/// </summary>
		internal static string? RangeQualifierOf(string braceName, JointResult64 r)
		{
			if (r.WithinRange || r.Validity.Count == 0) return null;

			var parts = new List<string>();
			foreach (var (cond, ok) in r.Validity)
			{
				if (ok) continue;
				parts.Add(cond switch
				{
					"0.2<=beta<=1.0" => $"β = {r.Beta.ToString("F3", Inv)}, outside 0.2–1.0",
					"10<=gamma<=50" => $"γ = {r.Gamma.ToString("F1", Inv)}, outside 10–50",
					"30<=theta<=90" => $"θ = {r.ThetaDeg.ToString("F1", Inv)}°, outside 30–90°",
					"g/D>-0.6 (K)" => $"g/D = {r.GD.ToString("F2", Inv)}, outside > −0.6 (K)",
					_ => cond,
				});
			}
			return parts.Count == 0 ? null : $"{braceName}: {string.Join("; ", parts)}";
		}

		private static readonly System.Globalization.CultureInfo Inv =
			System.Globalization.CultureInfo.InvariantCulture;
	}
}
