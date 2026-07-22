namespace NorsokChecker.Services.Norsok64
{
	/// <summary>NORSOK N-004 §6.4 simple-joint class. Mirrors n64.py's ("K","Y","X").</summary>
	public enum Joint64Class { K, Y, X }

	/// <summary>
	/// Canonical inputs for a NORSOK N-004 Rev.3 §6.4 simple-joint check.
	///
	/// Faithful C# port of <c>python_prototype/norsok/n64.py</c> <c>JointInput</c>.
	/// Canonical fields are PURE SI: lengths m, stress Pa, force N, moment N·m
	/// (Qu/Qf/β/γ/τ are dimensionless so the resistance formulas are unit-agnostic given
	/// consistent inputs — the project convention is strict SI). Use <see cref="FromKn"/>
	/// for the engineering units engineers actually type (mm / MPa / kN / kNm).
	/// </summary>
	public sealed class Joint64Input
	{
		// ── chord (CHS) ──
		/// <summary>chord outer diameter [m]</summary>
		public double D { get; set; }
		/// <summary>chord wall thickness [m]</summary>
		public double T { get; set; }
		/// <summary>chord yield strength [Pa]</summary>
		public double FyChord { get; set; }

		// ── brace (CHS) ──
		/// <summary>brace outer diameter [m]</summary>
		public double d { get; set; }
		/// <summary>brace wall thickness [m]</summary>
		public double t { get; set; }
		/// <summary>brace yield strength [Pa]</summary>
		public double FyBrace { get; set; }
		/// <summary>brace-chord angle [deg]</summary>
		public double ThetaDeg { get; set; }
		/// <summary>gap (K joints) [m] — single-gap shortcut, used when <see cref="KComponents"/> is empty</summary>
		public double G { get; set; }

		// ── classification fractions (0..1), should sum to 1.0 ──
		public double FrK { get; set; } = 1.0;
		public double FrY { get; set; } = 0.0;
		public double FrX { get; set; } = 0.0;

		/// <summary>
		/// K balancing components: each is (fraction_of_axial_balanced_via_this_gap, gap_m). A brace that
		/// balances against several braces (KT / N joints) has ONE component PER gap, each with its OWN gap →
		/// its OWN Q_g → its OWN K resistance (Table 6-3 row K). Their fractions sum to <see cref="FrK"/>.
		/// When null/empty, the single-gap shortcut (FrK, G) is used as one component. (Y and X have no gap.)
		/// </summary>
		public List<(double Frac, double GapM)>? KComponents { get; set; }

		// ── brace design forces ──
		/// <summary>axial [N] (+ tension / − compression)</summary>
		public double NSd { get; set; }
		/// <summary>in-plane bending [N·m]</summary>
		public double MipSd { get; set; }
		/// <summary>out-of-plane bending [N·m]</summary>
		public double MopSd { get; set; }

		// ── chord design stresses for Qf (6.54 / 6.55) [Pa] ──
		public double SigmaASd { get; set; }
		public double SigmaMySd { get; set; }
		public double SigmaMzSd { get; set; }

		/// <summary>material factor γ_M (§6.4.3.2)</summary>
		public double GammaM { get; set; } = 1.15;

		/// <summary>Explicit pure-SI builder — lengths m, stress Pa, force N, moment N·m.</summary>
		public static Joint64Input FromSI(
			double D, double T, double fyChord, double d, double t, double fyBrace,
			double thetaDeg, double g,
			double frK = 1.0, double frY = 0.0, double frX = 0.0,
			List<(double Frac, double GapM)>? kComponents = null,
			double nSd = 0.0, double mipSd = 0.0, double mopSd = 0.0,
			double sigmaASd = 0.0, double sigmaMySd = 0.0, double sigmaMzSd = 0.0,
			double gammaM = 1.15)
			=> new()
			{
				D = D, T = T, FyChord = fyChord, d = d, t = t, FyBrace = fyBrace,
				ThetaDeg = thetaDeg, G = g, FrK = frK, FrY = frY, FrX = frX, KComponents = kComponents,
				NSd = nSd, MipSd = mipSd, MopSd = mopSd,
				SigmaASd = sigmaASd, SigmaMySd = sigmaMySd, SigmaMzSd = sigmaMzSd, GammaM = gammaM,
			};

		/// <summary>
		/// Convenience builder taking the ENGINEERING units engineers type: lengths mm, stress MPa,
		/// forces kN, moments kNm (σ in MPa, gaps in mm; KComponents gaps in mm). Converts to canonical SI.
		/// Mirrors <c>JointInput.from_kN</c>.
		/// </summary>
		public static Joint64Input FromKn(
			double D, double T, double fyChord, double d, double t, double fyBrace,
			double thetaDeg, double g,
			double frK = 1.0, double frY = 0.0, double frX = 0.0,
			List<(double Frac, double GapMm)>? kComponents = null,
			double nSdKn = 0.0, double mipSdKnm = 0.0, double mopSdKnm = 0.0,
			double sigmaASdMpa = 0.0, double sigmaMySdMpa = 0.0, double sigmaMzSdMpa = 0.0,
			double gammaM = 1.15)
		{
			const double MM = 1e-3, MPA = 1e6;
			var kc = kComponents?.Select(kc => (kc.Frac, kc.GapMm * MM)).ToList();
			return new Joint64Input
			{
				D = D * MM, T = T * MM, FyChord = fyChord * MPA, d = d * MM, t = t * MM, FyBrace = fyBrace * MPA,
				ThetaDeg = thetaDeg, G = g * MM, FrK = frK, FrY = frY, FrX = frX, KComponents = kc,
				NSd = nSdKn * 1e3, MipSd = mipSdKnm * 1e3, MopSd = mopSdKnm * 1e3,
				SigmaASd = sigmaASdMpa * MPA, SigmaMySd = sigmaMySdMpa * MPA, SigmaMzSd = sigmaMzSdMpa * MPA,
				GammaM = gammaM,
			};
		}

		/// <summary>Shallow copy (used by the §6.4.3.1 clamped-pass rule to override K gaps).</summary>
		public Joint64Input Clone() => (Joint64Input)MemberwiseClone();
	}
}
