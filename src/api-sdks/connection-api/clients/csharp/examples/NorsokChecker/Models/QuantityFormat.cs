using System.Globalization;

namespace NorsokChecker.Models
{
	/// <summary>
	/// One quantity, one format — for the GUI and the report alike.
	///
	/// They disagreed. The same M_Rd printed `F1` in the joint table and `F2` in the report; the
	/// same gap printed `F1`, `F0` and `F0` at three sites; a utilisation printed `73.7%` in one
	/// place and `73.7 %` in another. Nobody chose that: it is what happens when a conversion is
	/// written inline at seventeen call sites in one file and again in another.
	///
	/// The engine holds everything in SI (N, N·m, m, Pa) and converts only here.
	///
	/// CULTURE IS A PARAMETER, never a default chosen inside. The report must be invariant — a test
	/// sweeps its whole body for comma decimals under cs-CZ — while the GUI must follow the machine
	/// locale, which its own tests assert. A formatter that picked one would break the other.
	/// </summary>
	internal static class QuantityFormat
	{
		/// <summary>The report's culture. Never the current one: `1,5` in an HTML table is a defect.</summary>
		internal static readonly CultureInfo Report = CultureInfo.InvariantCulture;

		/// <summary>The GUI's culture — the machine's, as its tests require.</summary>
		internal static CultureInfo Gui => CultureInfo.CurrentCulture;

		// ── the quantities, by category ──────────────────────────────────────
		//
		// Each takes SI in and returns the display string. The decimal counts are the ones that
		// ship today, deliberately: this seam is introduced behaviour-preserving, and the defect
		// fixes that changed some of them were made separately so that a failing test says which
		// of the two broke it.

		// ── conversions, SI → the chosen unit ────────────────────────────────
		//
		// One factor each, and the label beside it. Kip and ksi are exact by definition of the
		// pound-force; the inch is exactly 25.4 mm.

		internal static double ToForce(double n, ForceUnit u) => u switch
		{
			ForceUnit.Newton => n,
			ForceUnit.MegaNewton => n / 1e6,
			ForceUnit.Kip => n / 4448.2216152605,
			_ => n / 1e3,
		};

		internal static string ForceLabel(ForceUnit u) => u switch
		{
			ForceUnit.Newton => "N", ForceUnit.MegaNewton => "MN", ForceUnit.Kip => "kip", _ => "kN",
		};

		internal static double ToStress(double pa, StressUnit u) => u switch
		{
			StressUnit.KPa => pa / 1e3,
			StressUnit.Ksi => pa / 6_894_757.293168,
			_ => pa / 1e6,   // MPa and N/mm² are the same number
		};

		internal static string StressLabel(StressUnit u) => u switch
		{
			StressUnit.KPa => "kPa", StressUnit.Ksi => "ksi",
			StressUnit.NPerMm2 => "N/mm²", _ => "MPa",
		};

		internal static double ToLength(double m, LengthUnit u) => u switch
		{
			LengthUnit.Metre => m,
			LengthUnit.Centimetre => m * 1e2,
			LengthUnit.Inch => m / 0.0254,
			_ => m * 1e3,
		};

		internal static string LengthLabel(LengthUnit u) => u switch
		{
			LengthUnit.Metre => "m", LengthUnit.Centimetre => "cm",
			LengthUnit.Inch => "in", _ => "mm",
		};

		/// <summary>An area, m² → the unit implied by the length choice.</summary>
		internal static double ToArea(double m2, LengthUnit u)
		{
			double f = ToLength(1.0, u);
			return m2 * f * f;
		}

		/// <summary>A second moment of area, m⁴ → the unit implied by the length choice.</summary>
		internal static double ToInertia(double m4, LengthUnit u)
		{
			double f = ToLength(1.0, u);
			return m4 * f * f * f * f;
		}

		/// <summary>A force, SI → the settings' unit, with its own precision.</summary>
		internal static string Force(double n, CultureInfo c, DisplaySettings s) =>
			s.ForceScientific
				? SciString(ToForce(n, s.Force), c)
				: Num(ToForce(n, s.Force), c, s.ForceDecimals);

		/// <summary>A force, N → kN.</summary>
		internal static string Force(double n, CultureInfo c, int dp = 1) =>
			Num(n / 1e3, c, dp);

		/// <summary>A moment, N·m → kN·m. Three decimals: at two, a 0.07 kN·m moment is 7 % out.</summary>
		internal static string Moment(double nm, CultureInfo c, int dp = 3) =>
			Num(nm / 1e3, c, dp);

		/// <summary>A stress, Pa → MPa.</summary>
		internal static string Stress(double pa, CultureInfo c, int dp = 1) =>
			Num(pa / 1e6, c, dp);

		/// <summary>A length, m → mm.</summary>
		internal static string Length(double m, CultureInfo c, int dp = 1) =>
			Num(m * 1e3, c, dp);

		/// <summary>An angle, already in degrees.</summary>
		internal static string Angle(double deg, CultureInfo c, int dp = 1) =>
			Num(deg, c, dp);

		/// <summary>A dimensionless factor — β, γ, τ, Q, A², φ.</summary>
		internal static string Ratio(double v, CultureInfo c, int dp = 3) =>
			Num(v, c, dp);

		/// <summary>
		/// A 0..1 ratio as a percentage. The SPACE is part of it: the two consumers printed
		/// `73.7%` and `73.7 %` for the same number, and a reader comparing two screens noticed.
		/// </summary>
		internal static string Percent(double ratio, CultureInfo c, int dp = 1) =>
			double.IsNaN(ratio) || double.IsInfinity(ratio)
				? "—"
				: (ratio * 100.0).ToString("F" + dp, c) + " %";

		// ── the two shapes the substitutions need ────────────────────────────

		/// <summary>
		/// Significant figures, floored at a minimum decimal count.
		///
		/// For a value inside a printed expression, where the reader multiplies what they see. A
		/// fixed decimal count cannot serve a quantity spanning orders of magnitude, and
		/// significant figures alone fail near 1 — Q_f = 1.0033 at four of them is `1.003`, and the
		/// digit that matters is the deviation from unity. Five and three, measured: the product of
		/// three such factors misses its result by 2.1 % at two significant figures, 0.17 % at
		/// three, 0.031 % at four and 0.0013 % here.
		/// </summary>
		internal static string Significant(double v, CultureInfo c, int sig = 5, int minDec = 3)
		{
			if (double.IsNaN(v) || double.IsInfinity(v)) return "—";
			if (v == 0.0) return "0";
			int exp = (int)Math.Floor(Math.Log10(Math.Abs(v)));
			return Num(v, c, Math.Max(minDec, Math.Min(15, sig - 1 - exp)));
		}

		/// <summary>
		/// Mantissa and exponent, the exponent taken from the VALUE.
		///
		/// A second moment of area spans nearly five orders across the sections this tool sees —
		/// 23 475 mm⁴ on a CHS 30×3 to 914 277 855 mm⁴ on a CHS 508×20. The fixed `×10⁶` scaling it
		/// used to carry printed the first as `0.0`, and the expression beside it then divided by
		/// the zero it had just printed.
		/// </summary>
		internal static (string Mantissa, int Exponent) Scientific(double v, CultureInfo c, int dp = 2)
		{
			if (double.IsNaN(v) || double.IsInfinity(v) || v == 0.0) return ("0", 0);
			int exp = (int)Math.Floor(Math.Log10(Math.Abs(v)));
			return (Num(v / Math.Pow(10, exp), c, dp), exp);
		}

		/// <summary>NaN and infinity print as an em dash rather than as "NaN" or "∞".</summary>
		internal static string Num(double v, CultureInfo c, int dp) =>
			double.IsNaN(v) || double.IsInfinity(v) ? "—" : v.ToString("F" + dp, c);

		/// <summary>`2.35×10⁴` — the plain-text form, for a WPF cell rather than a KaTeX block.</summary>
		internal static string SciString(double v, CultureInfo c, int dp = 2)
		{
			var (m, e) = Scientific(v, c, dp);
			return e == 0 ? m : $"{m}×10{Superscript(e)}";
		}

		private static string Superscript(int n)
		{
			const string digits = "⁰¹²³⁴⁵⁶⁷⁸⁹";
			string s = Math.Abs(n).ToString(CultureInfo.InvariantCulture);
			var sb = new System.Text.StringBuilder(n < 0 ? "⁻" : "");
			foreach (char ch in s) sb.Append(digits[ch - '0']);
			return sb.ToString();
		}

		// ── the settings-aware overloads ─────────────────────────────────────

		/// <summary>
		/// A moment, N·m → the unit implied by the force and length choice.
		///
		/// It is DERIVED, not chosen: a moment in `kN·m` is a force in kN acting over a metre, so
		/// converting N·m means converting the force and then the length. The metric units all pair
		/// with the metre (kN·m, N·m, MN·m — nobody writes kN·mm); the imperial pair takes the foot
		/// unless lengths are set to inches, which is what kip·ft and kip·in mean.
		/// </summary>
		internal static string Moment(double nm, CultureInfo c, DisplaySettings s)
		{
			double perMetre = s.Force == ForceUnit.Kip && s.Length == LengthUnit.Inch
				? 1.0 / 0.0254                       // kip·in
				: s.Force == ForceUnit.Kip
					? 1.0 / 0.3048                   // kip·ft
					: 1.0;                           // kN·m, N·m, MN·m
			double v = ToForce(nm, s.Force) * perMetre;
			return s.MomentScientific ? SciString(v, c) : Num(v, c, s.MomentDecimals);
		}

		/// <summary>A stress, SI → the settings' unit.</summary>
		internal static string Stress(double pa, CultureInfo c, DisplaySettings s) =>
			s.StressScientific
				? SciString(ToStress(pa, s.Stress), c)
				: Num(ToStress(pa, s.Stress), c, s.StressDecimals);

		/// <summary>A length, SI → the settings' unit.</summary>
		internal static string Length(double m, CultureInfo c, DisplaySettings s) =>
			Num(ToLength(m, s.Length), c, s.LengthDecimals);

		/// <summary>A gap or an eccentricity: the section unit, its own precision.</summary>
		internal static string SmallLength(double m, CultureInfo c, DisplaySettings s) =>
			Num(ToLength(m, s.Length), c, s.SmallLengthDecimals);

		/// <summary>A cross-sectional area, SI → the unit implied by the length choice.</summary>
		internal static string Area(double m2, CultureInfo c, DisplaySettings s) =>
			s.AreaScientific
				? SciString(ToArea(m2, s.Length), c)
				: Num(ToArea(m2, s.Length), c, s.AreaDecimals);

		/// <summary>
		/// A second moment of area. Scientific by default — this is the quantity a fixed scale
		/// cannot serve, spanning 23 475 mm⁴ to 914 277 855 mm⁴ across the sections seen here.
		/// </summary>
		internal static string Inertia(double m4, CultureInfo c, DisplaySettings s) =>
			s.InertiaScientific
				? SciString(ToInertia(m4, s.Length), c)
				: Num(ToInertia(m4, s.Length), c, s.InertiaDecimals);
	}
}
