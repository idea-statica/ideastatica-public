namespace NorsokChecker.Models
{
	/// <summary>
	/// Geometric parameters of a tubular joint per NORSOK N-004 §6.4.
	/// See Figures 6-3 to 6-6 for definitions.
	/// </summary>
	public class TubularJointGeometry
	{
		// ── Chord ──
		/// <summary>Chord outside diameter [mm]</summary>
		public double D { get; set; }
		/// <summary>Chord wall thickness [mm]</summary>
		public double T { get; set; }
		/// <summary>Chord yield strength [MPa]</summary>
		public double FyChord { get; set; } = 355;

		// ── Brace ──
		/// <summary>Brace outside diameter [mm]</summary>
		public double d { get; set; }
		/// <summary>Brace wall thickness [mm]</summary>
		public double t { get; set; }
		/// <summary>Brace-to-chord angle [degrees]</summary>
		public double ThetaDeg { get; set; } = 90;
		/// <summary>Brace yield strength [MPa]</summary>
		public double FyBrace { get; set; } = 355;

		// ── K-joint gap (optional) ──
		/// <summary>Gap between braces [mm] (only for K-joints)</summary>
		public double Gap { get; set; }

		// ── Joint classification ──
		public JointType JointType { get; set; } = JointType.T_Y;

		// ── Derived parameters ──
		/// <summary>β = d/D (brace-to-chord diameter ratio). Validity: 0.2 ≤ β ≤ 1.0</summary>
		public double Beta => D > 0 ? d / D : 0;
		/// <summary>γ = D/(2T) (chord slenderness). Validity: 10 ≤ γ ≤ 50</summary>
		public double Gamma => T > 0 ? D / (2.0 * T) : 0;
		/// <summary>τ = t/T (brace-to-chord thickness ratio)</summary>
		public double Tau => T > 0 ? t / T : 0;
		/// <summary>θ in radians</summary>
		public double ThetaRad => ThetaDeg * Math.PI / 180.0;
		/// <summary>g/D ratio (for K-joints)</summary>
		public double GapRatio => D > 0 ? Gap / D : 0;
	}

	public enum JointType
	{
		K,
		T_Y,
		X
	}
}
