namespace NorsokChecker.Services.Norsok64
{
	/// <summary>Per-class (K / Y / X) axial result. Mirrors n64.py <c>ClassResult</c>.</summary>
	public sealed class ClassResult64
	{
		public Joint64Class Cls { get; set; }
		public double QuAxial { get; set; }
		public double QfAxial { get; set; }
		public double QfAxialA2 { get; set; }
		public (double C1, double C2, double C3, string Note) CAxial { get; set; }
		/// <summary>design axial resistance [N]</summary>
		public double NRd { get; set; }
		/// <summary>eq (6.57) for this class (abs-value convention)</summary>
		public double Util { get; set; }
		public double UtilAxialTerm { get; set; }
		public double UtilIpTerm { get; set; }
		public double UtilOpTerm { get; set; }
	}

	/// <summary>One K balancing gap's contribution. Mirrors an entry of n64.py <c>K_terms</c>.</summary>
	public sealed class KTerm64
	{
		public double FrK { get; set; }
		/// <summary>gap [m]</summary>
		public double GapM { get; set; }
		public double Qg { get; set; }
		public double QuAxial { get; set; }
		/// <summary>this gap's K resistance [N]</summary>
		public double NRd { get; set; }
	}

	/// <summary>
	/// Full NORSOK N-004 §6.4 simple-joint result with every intermediate value.
	/// Faithful C# port of n64.py <c>JointResult</c>. Resistances are in SI (N, N·m).
	/// </summary>
	public sealed class JointResult64
	{
		// ── geometry ──
		public double Beta { get; set; }
		public double Gamma { get; set; }
		public double Tau { get; set; }
		public double ThetaDeg { get; set; }
		public double SinTheta { get; set; }
		public double GD { get; set; }

		// ── validity (§6.4.3.1) ──
		public Dictionary<string, bool> Validity { get; set; } = new();
		public bool WithinRange { get; set; }

		/// <summary>
		/// Set when §6.4.3.1's out-of-range rule was applied — the check ran TWICE, once on the actual
		/// geometry and once with the infringed parameters imposed at their limits, and the resistance
		/// below is the LESSER of the two.
		///
		/// Recorded so the derivation can say so. Without it the sheet showed a resistance already
		/// reduced by the rule with nothing to distinguish it from a plain in-range result, and an
		/// engineer recomputing N_Rd from the actual β would get a different number and have no way to
		/// see why.
		/// </summary>
		public bool LimitingPassApplied { get; set; }

		/// <summary>The two candidate axial resistances [N] the rule chose between — actual, limiting.</summary>
		public double NRdActual { get; set; } = double.NaN;
		public double NRdLimiting { get; set; } = double.NaN;

		/// <summary>The clamped values used in the limiting pass, for the ones that were infringed.</summary>
		public double BetaLimiting { get; set; } = double.NaN;
		public double GammaLimiting { get; set; } = double.NaN;
		public double ThetaLimitingDeg { get; set; } = double.NaN;

		// ── helper factors ──
		public double QBeta { get; set; }
		public double Qg { get; set; }

		// ── bending resistances (shared across classes) ──
		public double QuIpb { get; set; }
		public double QuOpb { get; set; }
		public double QfMoment { get; set; }
		public double QfMomentA2 { get; set; }
		/// <summary>
		/// Table 6-4 coefficients behind <see cref="QfMoment"/>. One row covers every class
		/// ("All joints under brace moment loading"), so unlike <see cref="ClassResult64.CAxial"/>
		/// this lives on the result, not per class. Carried so the report can substitute them —
		/// eq (6.54) is a fixed formula and these three numbers are its only variable input.
		/// </summary>
		public (double C1, double C2, double C3, string Note) CMoment { get; set; }
		/// <summary>in-plane bending resistance [N·m]</summary>
		public double MRdIp { get; set; }
		/// <summary>out-of-plane bending resistance [N·m]</summary>
		public double MRdOp { get; set; }

		// ── per-class axial ──
		public Dictionary<Joint64Class, ClassResult64> PerClass { get; set; } = new();

		// ── weighted ──
		/// <summary>weighted axial resistance [N]</summary>
		public double NRdWeighted { get; set; }
		public double UtilWeighted { get; set; }
		public bool Passed { get; set; }
		/// <summary>"tension" | "compression"</summary>
		public string LoadAxial { get; set; } = "";

		/// <summary>K balancing breakdown: one entry per gap. Sum of FrK = FrK.</summary>
		public List<KTerm64> KTerms { get; set; } = new();

		/// <summary>
		/// True when Q_f (eq 6.54, which has no floor in the norm) drove an ACTIVE class's N_Rd,
		/// or either shared M_Rd, to ≤ 0 — the chord wall has no meaningful remaining resistance.
		/// Forces <see cref="Passed"/>=false regardless of <see cref="UtilWeighted"/>. This is an
		/// app-level safety decision, not a norm requirement (see n64.py).
		/// </summary>
		public bool ChordOverstressed { get; set; }

		/// <summary>Shallow copy — used by the §6.4.3.1 "lesser capacity" rule.</summary>
		public JointResult64 Clone() => (JointResult64)MemberwiseClone();
	}
}
