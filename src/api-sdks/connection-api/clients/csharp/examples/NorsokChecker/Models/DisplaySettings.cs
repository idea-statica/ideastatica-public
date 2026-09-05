namespace NorsokChecker.Models
{
	/// <summary>The unit a force is displayed in. The engine always holds newtons.</summary>
	public enum ForceUnit { KiloNewton, Newton, MegaNewton, Kip }

	/// <summary>The unit a stress is displayed in.</summary>
	public enum StressUnit { MPa, KPa, NPerMm2, Ksi }

	/// <summary>The unit a length is displayed in. Section properties follow it.</summary>
	public enum LengthUnit { Millimetre, Centimetre, Metre, Inch }

	/// <summary>
	/// How quantities are displayed — one setting, honoured by the app's tables and by the report.
	///
	/// THE UNIT IS THE ENGINEER'S CHOICE. Someone who works in kPa every day should be able to read
	/// this calculation in kPa; it is not for the tool to decide that 355 MPa reads better than
	/// 355 000 kPa. So a unit is offered wherever a physical alternative exists, and refused only
	/// where there is a substantive reason:
	///
	///   - ANGLES stay in degrees, because §6.4.3.1 states its validity ranges in degrees and the
	///     validity table is read against the clause (the standard writes "degrees" 11×, "radians"
	///     never).
	///   - DIMENSIONLESS quantities — β, γ, τ, the Q-factors, A², φ, C₁C₂C₃ — have no alternative
	///     unit to offer. Their PRECISION matters more than anywhere else, because they are the
	///     factors a reader substitutes.
	///   - N/mm² is numerically identical to MPa. It is offered as the spelling it is, not as a
	///     conversion.
	///
	/// AND THE DERIVATION IS EXEMPT. Its substitutions stay in MPa, mm and kN whatever is set here,
	/// matching the norm's own convention — that is what lets a reader hold a printed line against
	/// the clause. See the note on RenderJointDerivation. The dialog says so, or a user who selects
	/// kPa and then sees MPa in a substitution reads it as a bug.
	/// </summary>
	internal sealed class DisplaySettings
	{
		// ── units ────────────────────────────────────────────────────────────

		internal ForceUnit Force { get; set; } = ForceUnit.KiloNewton;
		internal StressUnit Stress { get; set; } = StressUnit.MPa;
		internal LengthUnit Length { get; set; } = LengthUnit.Millimetre;

		// Moments and section properties are DERIVED, never chosen. A free choice would let someone
		// select newtons with kilonewton-metres, or millimetres with in⁴ — combinations that are not
		// wrong so much as unreadable, and that no dialog should have to explain.

		/// <summary>The moment unit implied by the force and length choice, e.g. "kN·m".</summary>
		internal string MomentLabel => Force switch
		{
			ForceUnit.Kip => Length == LengthUnit.Inch ? "kip·in" : "kip·ft",
			ForceUnit.Newton => "N·m",
			ForceUnit.MegaNewton => "MN·m",
			_ => "kN·m",
		};

		/// <summary>The area unit implied by the length choice.</summary>
		internal string AreaLabel => Length switch
		{
			LengthUnit.Inch => "in²",
			LengthUnit.Metre => "m²",
			LengthUnit.Centimetre => "cm²",
			_ => "mm²",
		};

		/// <summary>The second-moment-of-area unit implied by the length choice.</summary>
		internal string InertiaLabel => Length switch
		{
			LengthUnit.Inch => "in⁴",
			LengthUnit.Metre => "m⁴",
			LengthUnit.Centimetre => "cm⁴",
			_ => "mm⁴",
		};

		// ── precision ────────────────────────────────────────────────────────
		//
		// Defaults are what ships today, so a user who never opens the dialog sees the report they
		// already have. The two exceptions are the values a defect fix changed — moments, and the
		// gap, which needs a decimal that a 1.5 mm value cannot do without.

		internal int ForceDecimals { get; set; } = 1;
		internal int MomentDecimals { get; set; } = 3;
		internal int StressDecimals { get; set; } = 1;
		internal int LengthDecimals { get; set; } = 1;

		/// <summary>Gaps and eccentricities: the same UNIT as sections, their own precision.</summary>
		internal int SmallLengthDecimals { get; set; } = 1;

		internal int AreaDecimals { get; set; } = 0;
		internal int InertiaDecimals { get; set; } = 2;
		internal int AngleDecimals { get; set; } = 1;
		internal int RatioDecimals { get; set; } = 3;
		internal int PercentDecimals { get; set; } = 1;

		// ── scientific notation ──────────────────────────────────────────────
		//
		// Offered only where the values span orders of magnitude, measured across the sections and
		// load effects this tool sees: I spans 38 947×, moments 3 750×, stresses 2 367×, forces
		// 1 000×, areas 121×. Lengths span 17× and the dimensionless factors are all of order unity,
		// so an exponent there is noise.
		//
		// I defaults ON: it is the one quantity a fixed scale cannot serve, and the fixed ×10⁶ it
		// used to carry printed a CHS 30×3 as `0.0`.

		internal bool ForceScientific { get; set; }
		internal bool MomentScientific { get; set; }
		internal bool StressScientific { get; set; }
		internal bool AreaScientific { get; set; }
		internal bool InertiaScientific { get; set; } = true;

		/// <summary>A copy, for a dialog to edit without committing until OK.</summary>
		internal DisplaySettings Clone() => (DisplaySettings)MemberwiseClone();
	}
}
