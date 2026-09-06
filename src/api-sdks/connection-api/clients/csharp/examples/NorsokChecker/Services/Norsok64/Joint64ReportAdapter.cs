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
		///
		/// THE CARD CARRIES VALUES, NOT SENTENCES. Its title, its range qualifier and its
		/// recommendation are composed by <see cref="TitleOf"/>, <see cref="RangeQualifierOf"/> and
		/// <see cref="GapRecommendationOf"/> at the moment they are printed, from the row kept in
		/// <see cref="NorsokFormulaResult.JointDetail"/>. They used to be built here, when the check
		/// ran, in the units and precision in force at that moment — so a card built at one decimal
		/// and printed at two read `K 0.0 % / Y 0.0 % / X 100.0 %` beside a badge saying `32.10%`, and
		/// the results grid had to parse the title back apart to recover the brace name it had been
		/// given a moment earlier. No display setting is read here any more, and there is nothing in
		/// this method for one to reach.
		///
		/// No Variables table either: the derivation IS the substitution, step by step, and the
		/// report already declines to print a "Where" table beside one. The list built here was read
		/// by nothing.
		/// </summary>
		public static NorsokFormulaResult BuildResultFromRow(JointCheckRow row, string loadCaseName)
		{
			var r = row.Engine!;
			double utilDisplay = double.IsInfinity(r.UtilWeighted) ? 999.0 : r.UtilWeighted;

			return new NorsokFormulaResult
			{
				Section = "6.4.3.6",
				Equation = "6.57",
				CheckExpression = "|N_Sd|/N_Rd + (M_y,Sd/M_y,Rd)² + |M_z,Sd|/M_z,Rd ≤ 1.0",
				Formula = @"N_{Rd} = \frac{f_y \cdot T^2}{\gamma_M \cdot \sin\theta} \cdot Q_u \cdot Q_f",
				Demand = utilDisplay,
				Capacity = 1.0,
				Utilization = utilDisplay,
				Passed = row.Passed,
				// The governing load effect. Without this the card reports LoadCaseId = 0, which the
				// results table renders as "envelope" and the report drops the LE badge entirely —
				// the envelope is only useful if it says which state it came from.
				LoadCaseId = row.GovLeId,
				LoadCaseName = row.GovLeName ?? loadCaseName,
				JointDetail = row,
			};
		}

		/// <summary>"Tubular Joint — M3": the part of the title that identifies the row.</summary>
		public static string SubjectOf(JointCheckRow row) => $"Tubular Joint — {row.Name}";

		/// <summary>
		/// The card title, in the reader's precision: the subject, the K/Y/X split, and the caveat
		/// where there is one.
		///
		/// INVARIANT CULTURE. `:P0` formats in the CURRENT culture, so on a Czech machine this title
		/// once put comma decimals into an English report — the defect QuantityFormat.Report exists to
		/// prevent, arriving through the one string that had not been routed through it.
		/// </summary>
		public static string TitleOf(JointCheckRow row, Models.DisplaySettings disp)
		{
			var r = row.Engine!;
			var cl = row.Classification!;
			string Pc(double v) => Models.QuantityFormat.Percent(v, Inv, disp.PercentDecimals);
			string title = $"{SubjectOf(row)} (K {Pc(cl.FrK)} / Y {Pc(cl.FrY)} / X {Pc(cl.FrX)})";
			if (r.ChordOverstressed) title += " — CHORD OVERSTRESSED";
			else if (!r.WithinRange) title += " — outside validity range (6.4.3.1)";
			return title;
		}

		/// <summary>
		/// "M3: g = 1.5 mm, §6.4.1 recommends 50 mm &lt; g &lt; D (141 mm)" — or null when met, or
		/// when the provision does not apply.
		///
		/// §6.4.1, p. 25: *"The gap for simple K-joints **should** be larger than 50 mm and less than
		/// D."* A "should", which §3.1 defines as a recommendation rather than a requirement for
		/// conformity — so this NEVER changes a verdict. Do not confuse it with §6.4.3.1's
		/// `g/D ≥ −0.6`, which is a validity condition on the formulas and does.
		///
		/// Only for a brace with a K share: the provision is about simple K-joints, and a Y or X
		/// brace has no gap the clause speaks of.
		///
		/// Whether this returns null does not depend on <paramref name="display"/> — only the
		/// wording does — so a caller asking "is the recommendation met?" may pass any setting.
		/// </summary>
		internal static string? GapRecommendationOf(string braceName, JointCheckRow row,
			Models.DisplaySettings? display = null)
		{
			if (row.Skipped || row.Inputs is not { } inp) return null;
			if (inp.FrK <= 1e-9 || inp.D <= 0.0) return null;

			// THE VERDICT IS DECIDED IN MILLIMETRES. The clause's 50 mm is a fixed physical length,
			// so a joint that satisfies §6.4.1 in mm satisfies it in inches — the comparison must
			// not move with the display unit.
			double gapMm = inp.G * 1e3, dMm = inp.D * 1e3;
			if (gapMm > 50.0 && gapMm < dMm) return null;

			// The MEASURED values follow the reader's unit; the clause's own 50 mm does not.
			// Converting the bound as well was tried and withdrawn: 50 mm and a 49.6 mm gap both
			// print as "2.0 in", giving "g = 2.0 in against 2.0 in < g — not satisfied".
			var d = display ?? new Models.DisplaySettings();
			string uL = Models.QuantityFormat.LengthLabel(d.Length);
			string g = Models.QuantityFormat.ToLength(inp.G, d.Length)
				.ToString("F" + d.SmallLengthDecimals, Inv);
			string dia = Models.QuantityFormat.ToLength(inp.D, d.Length)
				.ToString("F" + d.LengthDecimals, Inv);

			return $"{braceName}: g = {g} {uL}, §6.4.1 recommends "
				+ $"50 mm < g < D ({dia} {uL})";
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
		internal static string? RangeQualifierOf(string braceName, JointResult64 r,
			Models.DisplaySettings? display = null)
		{
			if (r.WithinRange || r.Validity.Count == 0) return null;

			// The measured value takes the reader's precision; the RANGE is the clause's own text
			// and stays exactly as the standard writes it.
			var d = display ?? new Models.DisplaySettings();
			string Ratio(double v) => v.ToString("F" + d.RatioDecimals, Inv);
			string Angle(double v) => v.ToString("F" + d.AngleDecimals, Inv);

			var parts = new List<string>();
			foreach (var (cond, ok) in r.Validity)
			{
				if (ok) continue;
				parts.Add(cond switch
				{
					"0.2<=beta<=1.0" => $"β = {Ratio(r.Beta)}, outside 0.2–1.0",
					"10<=gamma<=50" => $"γ = {Ratio(r.Gamma)}, outside 10–50",
					"30<=theta<=90" => $"θ = {Angle(r.ThetaDeg)}°, outside 30–90°",
					"g/D>=-0.6 (K)" => $"g/D = {Ratio(r.GD)}, outside ≥ −0.6 (K)",
					_ => cond,
				});
			}
			return parts.Count == 0 ? null : $"{braceName}: {string.Join("; ", parts)}";
		}

		private static readonly System.Globalization.CultureInfo Inv =
			System.Globalization.CultureInfo.InvariantCulture;
	}
}
