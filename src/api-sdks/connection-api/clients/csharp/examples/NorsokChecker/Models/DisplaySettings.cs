namespace NorsokChecker.Models
{
	/// <summary>The unit a force is displayed in. The engine always holds newtons.</summary>
	public enum ForceUnit { KiloNewton, Newton, MegaNewton, Kip }

	/// <summary>The unit a stress is displayed in.</summary>
	public enum StressUnit { MPa, KPa, NPerMm2, Ksi }

	/// <summary>The unit a length is displayed in. Section properties follow it.</summary>
	public enum LengthUnit { Millimetre, Centimetre, Metre, Inch }

	/// <summary>
	/// How a number is written — the same three IDEA StatiCa's own preferences offer, minus the
	/// fourth.
	///
	/// Theirs has an Imperial member producing feet-inch-fraction strings (`1' 2 2/16"`). That is
	/// for structure dimensions; this tool measures tubes, where a decimal inch is what a reader
	/// wants. Left out rather than half-implemented.
	/// </summary>
	public enum NumberFormat
	{
		/// <summary>`141.30` — a fixed number of decimals.</summary>
		Decimal,

		/// <summary>`1.41×10²` — mantissa and an exponent taken from the value.</summary>
		Scientific,

		/// <summary>
		/// Decimal until the value stops being readable that way, then scientific. Worth more here
		/// than in most places: the second moment of area runs from 23 475 mm⁴ on a CHS 30×3 to
		/// 914 277 855 mm⁴ on a CHS 508×20, and no fixed choice serves both.
		/// </summary>
		Automatic,
	}

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
	/// THE DERIVATION IS NOT EXEMPT. It was, on the argument that its substitutions should match the
	/// norm's own convention — but the standard writes its formulas in SYMBOLS, which hold in any
	/// consistent set of units, and the engine computes in SI throughout. Everything printed is a
	/// conversion, so a user who selects inches and ksi gets them on the derivation page too.
	///
	/// THE ONE INVARIANT: a check RESULT must not move when the units change. Utilisations, β, γ and
	/// the Q-factors are dimensionless, and the §6.4.1 gap comparison is decided in millimetres for
	/// that reason — the printed sentence follows the setting, the verdict does not.
	/// </summary>
	public sealed class DisplaySettings
	{
		// ── units ────────────────────────────────────────────────────────────

		public ForceUnit Force { get; set; } = ForceUnit.KiloNewton;
		public StressUnit Stress { get; set; } = StressUnit.MPa;
		public LengthUnit Length { get; set; } = LengthUnit.Millimetre;

		// Moments and section properties are DERIVED, never chosen. A free choice would let someone
		// select newtons with kilonewton-metres, or millimetres with in⁴ — combinations that are not
		// wrong so much as unreadable, and that no dialog should have to explain.

		/// <summary>The moment unit implied by the force and length choice, e.g. "kN·m".</summary>
		public string MomentLabel => Force switch
		{
			ForceUnit.Kip => Length == LengthUnit.Inch ? "kip·in" : "kip·ft",
			ForceUnit.Newton => "N·m",
			ForceUnit.MegaNewton => "MN·m",
			_ => "kN·m",
		};

		/// <summary>The area unit implied by the length choice.</summary>
		public string AreaLabel => Length switch
		{
			LengthUnit.Inch => "in²",
			LengthUnit.Metre => "m²",
			LengthUnit.Centimetre => "cm²",
			_ => "mm²",
		};

		/// <summary>The second-moment-of-area unit implied by the length choice.</summary>
		public string InertiaLabel => Length switch
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

		public int ForceDecimals { get; set; } = 1;
		public int MomentDecimals { get; set; } = 3;
		public int StressDecimals { get; set; } = 1;
		public int LengthDecimals { get; set; } = 1;

		/// <summary>Gaps and eccentricities: the same UNIT as sections, their own precision.</summary>
		public int SmallLengthDecimals { get; set; } = 1;

		public int AreaDecimals { get; set; } = 0;
		public int InertiaDecimals { get; set; } = 2;
		public int AngleDecimals { get; set; } = 1;
		public int RatioDecimals { get; set; } = 3;
		public int PercentDecimals { get; set; } = 1;

		// ── number format ────────────────────────────────────────────────────
		//
		// A format per quantity, as IDEA StatiCa's own preferences have it. It replaces a
		// scientific-yes/no checkbox: three values instead of two, and present on every row that
		// can use one rather than only on some, so the table does not look moth-eaten.
		//
		// Angles and coefficients have no format row — degrees and order-unity ratios are always
		// decimal, and offering `β = 7.23×10⁻¹` would be a choice nobody wants.
		//
		// I defaults to AUTOMATIC: it is the one quantity a fixed choice cannot serve, spanning
		// 23 475 mm⁴ to 914 277 855 mm⁴, and the fixed ×10⁶ scaling it used to carry printed a
		// CHS 30×3 as `0.0`.

		public NumberFormat ForceFormat { get; set; } = NumberFormat.Decimal;
		public NumberFormat MomentFormat { get; set; } = NumberFormat.Decimal;
		public NumberFormat StressFormat { get; set; } = NumberFormat.Decimal;
		public NumberFormat LengthFormat { get; set; } = NumberFormat.Decimal;
		public NumberFormat AreaFormat { get; set; } = NumberFormat.Decimal;
		public NumberFormat InertiaFormat { get; set; } = NumberFormat.Automatic;

		/// <summary>A copy, for a dialog to edit without committing until OK.</summary>
		public DisplaySettings Clone() => (DisplaySettings)MemberwiseClone();
	}
}
