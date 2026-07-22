namespace NorsokChecker.Services.Norsok64
{
	/// <summary>Load kind for the Table 6-4 chord-action coefficients.</summary>
	public enum QfLoadKind { Moment, AxialTension, AxialCompression }

	/// <summary>
	/// NORSOK N-004 Rev.3 (Feb 2013) — §6.4 Tubular joints (simple joints) engine.
	///
	/// Faithful, bit-for-bit C# port of <c>python_prototype/norsok/n64.py</c>, itself the twin of
	/// the reference HTML calculator. Pure functions + <see cref="JointResult64"/> carrying every
	/// intermediate value. Scope: simple joints (6.4.3) — NO joint cans (assumes T_c = T_n),
	/// NO overlap/ring/cast. Classification K/Y/X is given as fractions; all three classes are
	/// ALWAYS computed and the weighted result uses the fractions.
	///
	/// Units: PURE SI internally — forces N, moments N·m, lengths m, stresses Pa. (Qu/Qf/β/γ/τ are
	/// dimensionless.) γ_M = 1.15 (LRFD). Do NOT mix in API RP-2A-WSD allowable-stress apparatus.
	/// </summary>
	public static class Norsok64Engine
	{
		private static readonly Joint64Class[] Classes = { Joint64Class.K, Joint64Class.Y, Joint64Class.X };

		// ── Table 6-3 strength factors Qu ──────────────────────────────────

		/// <summary>Table 6-3 axial Q_u by joint class and load sense.</summary>
		public static double QuAxial(Joint64Class cls, double beta, double gamma, double qg, double qb, bool tension)
		{
			switch (cls)
			{
				case Joint64Class.K:
					// tension and compression share the same expression
					return Math.Min((16.0 + 1.2 * gamma) * Math.Pow(beta, 1.2) * qg,
									 40.0 * Math.Pow(beta, 1.2) * qg);
				case Joint64Class.Y:
					if (tension) return 30.0 * beta;
					return Math.Min(2.8 + (20.0 + 0.8 * gamma) * Math.Pow(beta, 1.6),
									 2.8 + 36.0 * Math.Pow(beta, 1.6));
				default: // X
					if (tension) return 6.4 * Math.Pow(gamma, 0.6 * beta * beta);
					return (2.8 + (12.0 + 0.1 * gamma) * beta) * qb;
			}
		}

		/// <summary>Table 6-3 in-plane bending Q_u (same for all classes).</summary>
		public static double QuIpb(double beta, double gamma) => (5.0 + 0.7 * gamma) * Math.Pow(beta, 1.2);

		/// <summary>Table 6-3 out-of-plane bending Q_u (same for all classes).</summary>
		public static double QuOpb(double beta, double gamma) => 2.5 + (4.5 + 0.2 * gamma) * Math.Pow(beta, 2.6);

		/// <summary>Geometric factor Q_beta (note (a) under Table 6-3).</summary>
		public static double QBeta(double beta) => beta > 0.6 ? 0.3 / (beta * (1.0 - 0.833 * beta)) : 1.0;

		/// <summary>Gap factor Q_g (note (b) under Table 6-3). Linear interp in the transition band.</summary>
		public static double Qg(double g, double D, double t, double T, double fyB, double fyC, double gamma)
		{
			double gD = g / D;
			if (gD >= 0.05)
				return Math.Max(1.0 + 0.2 * Math.Pow(1.0 - 2.8 * gD, 3), 1.0);
			double phi = (t * fyB) / (T * fyC);
			if (gD <= -0.05)
				return 0.13 + 0.65 * phi * Math.Pow(gamma, 0.5);
			// -0.05 < gD < 0.05 : linear interpolation between the two limiting values
			double qgPos = Math.Max(1.0 + 0.2 * Math.Pow(1.0 - 2.8 * 0.05, 3), 1.0);
			double qgNeg = 0.13 + 0.65 * phi * Math.Pow(gamma, 0.5);
			double w = (gD - (-0.05)) / 0.10;
			return qgNeg + (qgPos - qgNeg) * w;
		}

		// ── Table 6-4 chord-action factor Qf ───────────────────────────────

		/// <summary>Table 6-4 coefficients C1, C2, C3 (+ note). X interpolates between β 0.9..1.0.</summary>
		public static (double C1, double C2, double C3, string Note) CCoeffs(Joint64Class cls, QfLoadKind kind, double beta)
		{
			if (kind == QfLoadKind.Moment)
				return (0.2, 0.0, 0.4, "All joints, brace moment");
			if (cls == Joint64Class.K)
				return (0.2, 0.2, 0.3, "K, balanced axial");
			if (cls == Joint64Class.Y)
				return (0.3, 0.0, 0.8, "T/Y, brace axial");

			// X — linear interpolation between beta=0.9 and beta=1.0
			double Lerp(double a, double b)
			{
				if (beta <= 0.9) return a;
				if (beta >= 1.0) return b;
				return a + (b - a) * (beta - 0.9) / 0.1;
			}
			if (kind == QfLoadKind.AxialTension)
				return (Lerp(0.0, 0.2), 0.0, Lerp(0.4, 0.2), "X, brace axial tension (β-interp)");
			return (Lerp(0.2, -0.2), 0.0, Lerp(0.5, 0.2), "X, brace axial compression (β-interp)");
		}

		/// <summary>
		/// Chord action factor Q_f (6.54) and chord utilisation A² (6.55).
		/// NB: (6.55) denominator of the 2nd term is 1.62·fy²  (NOT (1.62·fy)²).
		/// No floor is applied here (the norm defines none) — see the chord-overstressed guard.
		/// </summary>
		public static (double Qf, double A2) Qf(double c1, double c2, double c3, double sa, double smy, double smz, double fy)
		{
			double a2 = (sa / fy) * (sa / fy) + (smy * smy + smz * smz) / (1.62 * fy * fy);
			double qf = 1.0 + c1 * (sa / fy) - c2 * (smy / (1.62 * fy)) - c3 * a2;
			return (qf, a2);
		}

		// ── Main entry points ──────────────────────────────────────────────

		/// <summary>
		/// Run the full NORSOK 6.4 simple-joint check ONCE with the given inputs, returning all
		/// intermediate values. If any clamp is given it OVERRIDES the corresponding geometric ratio
		/// (β/γ/θ) right after it is derived — everything downstream (Qu, Qf, N_Rd, M_Rd) then uses the
		/// override. Used by <see cref="CheckJoint"/> to implement 6.4.3.1's "lesser of actual-vs-limiting
		/// capacity" rule. g/D has no clamp here (only K uses it; clamped by overriding g upstream).
		/// </summary>
		public static JointResult64 CheckJointOnce(Joint64Input inp,
			double? clampBeta = null, double? clampGamma = null, double? clampThetaDeg = null)
		{
			double D = inp.D, T = inp.T, d = inp.d, t = inp.t;
			double fy = inp.FyChord;      // f_y in (6.52)/(6.53) keys on chord yield
			double gM = inp.GammaM;

			double beta = clampBeta ?? d / D;
			double gamma = clampGamma ?? D / (2.0 * T);
			double thetaDeg = clampThetaDeg ?? inp.ThetaDeg;
			double theta = thetaDeg * Math.PI / 180.0;
			double sinT = Math.Sin(theta);
			double tau = t / T;
			double gD = inp.G / D;

			// validity range (6.4.3.1). g/D only matters for K (it feeds Q_g); a brace with FrK=0 never
			// touches Q_g, so an odd gap on it is not a real validity breach for THIS brace.
			var validity = new Dictionary<string, bool>
			{
				["0.2<=beta<=1.0"] = 0.2 <= beta && beta <= 1.0,
				["10<=gamma<=50"] = 10 <= gamma && gamma <= 50,
				["30<=theta<=90"] = 30 <= inp.ThetaDeg && inp.ThetaDeg <= 90,
				["g/D>-0.6 (K)"] = (gD > -0.6) || inp.FrK <= 0.0,
			};
			bool within = validity.Values.All(v => v);

			double qb = QBeta(beta);

			// K balancing components: each gap g_i → its own Q_g(g_i). Default (no list): one component (FrK, G).
			var kComps = ResolveKComponents(inp);
			// representative Q_g for reporting = the one from the single/first gap.
			double qg = Qg(inp.G, D, t, T, inp.FyBrace, inp.FyChord, gamma);

			bool tension = inp.NSd >= 0;
			QfLoadKind loadKindAx = tension ? QfLoadKind.AxialTension : QfLoadKind.AxialCompression;

			// shared bending resistances (independent of classification)
			double quI = QuIpb(beta, gamma);
			double quO = QuOpb(beta, gamma);
			var cm = CCoeffs(Joint64Class.K, QfLoadKind.Moment, beta);   // moment coeffs same for all classes
			var (qfM, qfMA2) = Qf(cm.C1, cm.C2, cm.C3, inp.SigmaASd, inp.SigmaMySd, inp.SigmaMzSd, fy);
			double baseM = fy * T * T * d / (gM * sinT);
			double mRdIp = baseM * quI * qfM;
			double mRdOp = baseM * quO * qfM;

			double baseAx = fy * T * T / (gM * sinT);

			var perClass = new Dictionary<Joint64Class, ClassResult64>();
			foreach (var cls in Classes)
			{
				double quA = QuAxial(cls, beta, gamma, qg, qb, tension);
				var cax = CCoeffs(cls, loadKindAx, beta);
				var (qfA, qfaA2) = Qf(cax.C1, cax.C2, cax.C3, inp.SigmaASd, inp.SigmaMySd, inp.SigmaMzSd, fy);
				double nrd = baseAx * quA * qfA;
				double a = Math.Abs(inp.NSd) / nrd;
				double b = Math.Pow(Math.Abs(inp.MipSd) / mRdIp, 2);
				double c = Math.Abs(inp.MopSd) / mRdOp;
				perClass[cls] = new ClassResult64
				{
					Cls = cls, QuAxial = quA, QfAxial = qfA, QfAxialA2 = qfaA2, CAxial = cax,
					NRd = nrd, Util = a + b + c, UtilAxialTerm = a, UtilIpTerm = b, UtilOpTerm = c,
				};
			}

			// weighted axial resistance — K splits per balancing gap (Comm. 6.4.2), Y/X carry no gap.
			var caxK = CCoeffs(Joint64Class.K, loadKindAx, beta);
			var (qfK, _) = Qf(caxK.C1, caxK.C2, caxK.C3, inp.SigmaASd, inp.SigmaMySd, inp.SigmaMzSd, fy);
			var kTerms = new List<KTerm64>();
			double wNK = 0.0;
			foreach (var (frKi, gi) in kComps)
			{
				double qgi = Qg(gi, D, t, T, inp.FyBrace, inp.FyChord, gamma);
				double quAi = QuAxial(Joint64Class.K, beta, gamma, qgi, qb, tension);
				double nrdi = baseAx * quAi * qfK;
				wNK += frKi * nrdi;
				kTerms.Add(new KTerm64 { FrK = frKi, GapM = gi, Qg = qgi, QuAxial = quAi, NRd = nrdi });
			}

			double wN = wNK + inp.FrY * perClass[Joint64Class.Y].NRd + inp.FrX * perClass[Joint64Class.X].NRd;
			// wN == 0 (FrK=FrY=FrX=0, e.g. a pure-moment brace) → axial share of util is ABSENT (0), not
			// undefined; the shared M_Rd check is unaffected, so callers must NOT skip the brace on this.
			double axialTerm = wN > 0.0 ? Math.Abs(inp.NSd) / wN : 0.0;
			double wU = axialTerm + Math.Pow(Math.Abs(inp.MipSd) / mRdIp, 2) + Math.Abs(inp.MopSd) / mRdOp;

			// CHORD-OVERSTRESSED GUARD (app-level safety decision, not a norm requirement): Q_f (6.54) has
			// no floor, so a heavily-stressed chord can drive N_Rd/M_Rd ≤ 0. Treat as explicit FAIL. Only
			// ACTIVE axial classes are checked; the two shared bending resistances are always checked.
			bool chordOver = (mRdIp <= 0.0) || (mRdOp <= 0.0);
			if (inp.FrK > 0.0 && kTerms.Any(k => k.NRd <= 0.0)) chordOver = true;
			if (inp.FrY > 0.0 && perClass[Joint64Class.Y].NRd <= 0.0) chordOver = true;
			if (inp.FrX > 0.0 && perClass[Joint64Class.X].NRd <= 0.0) chordOver = true;

			return new JointResult64
			{
				Beta = beta, Gamma = gamma, Tau = tau, ThetaDeg = inp.ThetaDeg, SinTheta = sinT, GD = gD,
				Validity = validity, WithinRange = within,
				QBeta = qb, Qg = qg,
				QuIpb = quI, QuOpb = quO, QfMoment = qfM, QfMomentA2 = qfMA2,
				MRdIp = mRdIp, MRdOp = mRdOp,
				PerClass = perClass,
				NRdWeighted = wN, UtilWeighted = wU, Passed = wU <= 1.0 && !chordOver,
				ChordOverstressed = chordOver,
				LoadAxial = tension ? "tension" : "compression",
				KTerms = kTerms,
			};
		}

		/// <summary>
		/// Run the NORSOK 6.4 simple-joint check, applying 6.4.3.1's out-of-range rule VERBATIM:
		/// "The equations can be used for joints with geometries which lie outside the validity ranges,
		/// by taking the usable strength as the LESSER of the capacities calculated on the basis of:
		/// (a) actual geometric parameters, (b) imposed limiting parameters for the validity range,
		/// where these limits are infringed." — i.e. an actual second calculation at the clamped limit
		/// and taking whichever capacity is smaller.
		/// </summary>
		public static JointResult64 CheckJoint(Joint64Input inp)
		{
			double beta = inp.d / inp.D;
			double gamma = inp.D / (2.0 * inp.T);
			bool outOfRange = !(0.2 <= beta && beta <= 1.0 && 10 <= gamma && gamma <= 50 && 30 <= inp.ThetaDeg && inp.ThetaDeg <= 90);
			var kComps = ResolveKComponents(inp);
			bool gdOutOfRange = inp.FrK > 0.0 && kComps.Any(kc => (kc.GapM / inp.D) <= -0.6);

			var actual = CheckJointOnce(inp);
			if (!(outOfRange || gdOutOfRange))
				return actual;

			double clampBeta = Math.Min(Math.Max(beta, 0.2), 1.0);
			double clampGamma = Math.Min(Math.Max(gamma, 10.0), 50.0);
			double clampTheta = Math.Min(Math.Max(inp.ThetaDeg, 30.0), 90.0);
			var inpClamped = inp;
			if (gdOutOfRange)
			{
				// clamp EVERY K component's gap to the g/D = -0.6 limit (only K uses g/D; Y/X untouched)
				var clampedComps = kComps.Select(kc => (kc.Frac, Math.Max(kc.GapM, -0.6 * inp.D))).ToList();
				inpClamped = inp.Clone();
				inpClamped.KComponents = clampedComps;
			}
			var limiting = CheckJointOnce(inpClamped, clampBeta, clampGamma, clampTheta);

			// take the LESSER capacity (6.4.3.1) — compare on N_Rd_weighted / M_Rd. Keep the WHOLE result
			// (per_class / K_terms) from whichever pass gave the smaller N_Rd_weighted, so the detail view
			// shows one internally-consistent derivation, not a mix of two passes.
			var lesser = limiting.NRdWeighted < actual.NRdWeighted ? limiting : actual;
			double mRdIp = Math.Min(actual.MRdIp, limiting.MRdIp);
			double mRdOp = Math.Min(actual.MRdOp, limiting.MRdOp);
			double wN = lesser.NRdWeighted;
			double axialTerm = wN > 0.0 ? Math.Abs(inp.NSd) / wN : 0.0;
			double wU = (mRdIp > 0 && mRdOp > 0)
				? axialTerm + Math.Pow(Math.Abs(inp.MipSd) / mRdIp, 2) + Math.Abs(inp.MopSd) / mRdOp
				: double.PositiveInfinity;
			bool chordOver = lesser.ChordOverstressed || mRdIp <= 0.0 || mRdOp <= 0.0;

			// β/γ/θ/gD/validity/within_range must ALWAYS reflect the brace's ACTUAL geometry (never the
			// clamped comparison pass). Only the RESISTANCE side may come from the clamped pass.
			var result = lesser.Clone();
			result.Beta = actual.Beta; result.Gamma = actual.Gamma; result.Tau = actual.Tau;
			result.ThetaDeg = actual.ThetaDeg; result.SinTheta = actual.SinTheta; result.GD = actual.GD;
			result.Validity = actual.Validity; result.WithinRange = actual.WithinRange;
			result.MRdIp = mRdIp; result.MRdOp = mRdOp; result.NRdWeighted = wN; result.UtilWeighted = wU;
			result.Passed = wU <= 1.0 && !chordOver; result.ChordOverstressed = chordOver;
			return result;
		}

		/// <summary>
		/// K balancing components: the explicit list if present, else the single-gap shortcut (FrK, G)
		/// when FrK &gt; 0, else empty (no K contribution).
		/// </summary>
		private static List<(double Frac, double GapM)> ResolveKComponents(Joint64Input inp)
		{
			if (inp.KComponents != null && inp.KComponents.Count > 0)
				return inp.KComponents;
			return inp.FrK > 0.0
				? new List<(double, double)> { (inp.FrK, inp.G) }
				: new List<(double, double)>();
		}
	}
}
