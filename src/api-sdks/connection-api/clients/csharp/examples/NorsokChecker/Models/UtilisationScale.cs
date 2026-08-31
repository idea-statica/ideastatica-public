using System.Windows.Media;

namespace NorsokChecker.Models
{
	/// <summary>
	/// THE utilisation colour scale — ten bands from green to red — and the single place that defines
	/// it. The same utilisation has to be the same colour in the 3D view, in the load-effect selector's
	/// bar, on a result row and in the legend; before this, three copies of a four-band ramp lived in
	/// Joint3DView, Joint64RowView and MainWindow.xaml, and any change had to be made three times to
	/// stay true.
	///
	/// Ten bands rather than four because four cannot separate the range that matters: everything from
	/// 50 % to 85 % was one yellow, so a brace at 55 % and one at 80 % looked identical. The bands are
	/// even tenths of capacity, which keeps the legend readable as a scale — a reader maps a colour
	/// back to "about 70 %", not to a named category.
	///
	/// Two consumers, two needs. <see cref="Hex"/> is the flat colour for a 2D surface (a bar, a row
	/// background, a legend swatch). <see cref="LitBrush"/> is the same band adjusted for the 3D
	/// viewport, where a lit face never returns its own colour: those tones are lighter so they land
	/// near the flat ones once WPF has multiplied them by (ambient + Σ lights). Keep them in step —
	/// they are the same scale seen under different light, not two scales.
	/// </summary>
	internal static class UtilisationScale
	{
		/// <summary>
		/// How many bands the scale has: ten tenths of capacity, plus one for over-capacity — eleven
		/// in total. The eleventh is NOT a finer subdivision of the ramp, it is a different statement
		/// ("this element is overloaded"), and it has to be its own colour: with ten bands covering
		/// 0..1 inclusive, a brace at 99 % landed in the same band as one at 130 %, which is the one
		/// distinction a reader must never miss. Caught by TheRowTintFollowsUtilisation.
		///
		/// The legend draws <see cref="RampBandCount"/> swatches for the scale and marks the
		/// over-capacity band separately.
		/// </summary>
		internal const int BandCount = 11;

		/// <summary>The bands that divide 0..100 % — the scale proper, without the over-capacity one.</summary>
		internal const int RampBandCount = 10;

		/// <summary>Grey: no number to show — nothing was checked on this element.</summary>
		internal const string NoValueHex = "#D7DBE0";

		/// <summary>
		/// Flat band colours, index 0 = 0–10 % of capacity … index 9 = at or over 100 %.
		///
		/// Green through yellow to red, with the hue turning steadily rather than sitting in one
		/// family and then jumping: adjacent bands are close (they are neighbours on a scale) but no
		/// two are equal, and the ends are unmistakable. The last band is a deliberately harder red
		/// than the ninth — crossing 100 % is a different KIND of fact from being near it, and the
		/// scale should not hide the step.
		/// </summary>
		private static readonly string[] FlatHex =
		{
			"#43A047",   // 0.0 – 0.1   deep green
			"#5CB040",   // 0.1 – 0.2
			"#7CBB3A",   // 0.2 – 0.3
			"#9CC534",   // 0.3 – 0.4
			"#BFCE33",   // 0.4 – 0.5
			"#E0CC38",   // 0.5 – 0.6   yellow — the old four-band ramp's single tone for 0.5–0.85
			"#EFB733",   // 0.6 – 0.7
			"#F09A2E",   // 0.7 – 0.8   amber
			"#EC7A2C",   // 0.8 – 0.9
			"#E2632C",   // 0.9 – 1.0   near capacity, but still passing
			"#D32F2F",   // >= 1.0      over capacity — a deeper red than the band below it
		};

		/// <summary>
		/// The same ten bands, lightened for the 3D viewport. Measured against WPF's own
		/// colour * (ambient + Σ lights) product on the worst case — a face turned away from both
		/// directional lights — so that what the viewer sees is close to the flat swatch in the
		/// legend beside it. Do not "simplify" these to match FlatHex: identical values here make the
		/// darkest bands come back near-black, which is what the four-band ramp originally suffered
		/// from (its darkest tone reached #205823 and no hue survives that little light).
		/// </summary>
		private static readonly string[] LitHex =
		{
			"#66BB6A",
			"#7CC463",
			"#93CC5C",
			"#A9D356",
			"#C6DB52",
			"#E6D956",
			"#F3C74F",
			"#F4AC4B",
			"#F08E47",
			"#EF7043",   // 0.9 – 1.0
			"#E53935",   // >= 1.0
		};

		/// <summary>
		/// The band index for a utilisation: 0..9 across the ten tenths of capacity, and 10 for at or
		/// over 100 %.
		///
		/// Note the divisor is <see cref="RampBandCount"/>, not <see cref="BandCount"/>. Dividing by
		/// the total was the bug: it mapped 0.99 to index 9 — the same index the over-capacity case
		/// returns — so a brace at 99 % and one at 130 % came out the same colour, and the 0.8–0.9
		/// band was unreachable.
		/// </summary>
		internal static int BandOf(double util)
		{
			if (double.IsNaN(util) || util <= 0) return 0;
			if (util >= 1.0) return BandCount - 1;                     // the over-capacity band
			return Math.Min(RampBandCount - 1, (int)(util * RampBandCount));
		}

		/// <summary>Flat colour for a 2D surface: a bar, a row background, a legend swatch.</summary>
		internal static string Hex(double util) => FlatHex[BandOf(util)];

		/// <summary>Flat colour of a band by index — for drawing the legend.</summary>
		internal static string HexOfBand(int band) => FlatHex[Math.Clamp(band, 0, BandCount - 1)];

		/// <summary>
		/// The band's colour for the 3D viewport, pre-lightened for the lighting rig. Brushes are
		/// created once and frozen: one per band, shared by every body in it.
		/// </summary>
		internal static Brush LitBrush(double util) => LitBrushes[BandOf(util)];

		private static readonly Brush[] LitBrushes = BuildLit();

		private static Brush[] BuildLit()
		{
			var brushes = new Brush[BandCount];
			for (int i = 0; i < BandCount; i++)
			{
				var b = new SolidColorBrush(Parse(LitHex[i]));
				b.Freeze();          // shared across bodies and threads
				brushes[i] = b;
			}
			return brushes;
		}

		/// <summary>
		/// A very pale tint of the band, for painting a whole table row. A row background sits behind
		/// text, so the band colour itself is far too strong — this keeps the hue (so the row still
		/// reads on the same scale as the bar and the 3D body) at a fraction of the saturation.
		/// </summary>
		internal static Color RowTint(double util)
		{
			var c = Parse(FlatHex[BandOf(util)]);
			const double keep = 0.16;      // how much of the band's colour survives against white
			return Color.FromRgb(
				(byte)(255 - (255 - c.R) * keep),
				(byte)(255 - (255 - c.G) * keep),
				(byte)(255 - (255 - c.B) * keep));
		}

		internal static Color Parse(string hex) => (Color)ColorConverter.ConvertFromString(hex);
	}
}
