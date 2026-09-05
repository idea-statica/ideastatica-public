using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using NorsokChecker.Models;

namespace NorsokChecker.Controls
{
	/// <summary>
	/// Units and precision, for the app's tables and for the report alike.
	///
	/// Edits a COPY and returns it only on OK, like PageSetupWindow: a dialog that mutated the live
	/// settings would leave Cancel meaning nothing.
	///
	/// Laid out after IDEA StatiCa's own Preferences → Presentation units — one row per quantity,
	/// unit / decimals / format — with two of its choices taken deliberately:
	///
	///   - DECIMALS ARE TYPED. A dropdown has to pick a range and every range is arbitrary: A²
	///     already prints four places and a small gap could want five.
	///   - FORMAT IS A COMBO on every row that can use one, rather than a checkbox on some.
	///
	/// Two rows show a unit they do not let you choose, and the tooltip on each says why. Angles are
	/// fixed at degrees because §6.4.3.1 states its validity ranges in degrees; coefficients are
	/// dimensionless and have no unit to offer. Moment, area and inertia are DERIVED from the force
	/// and length choice and show what that choice implies, so an incoherent pair — newtons with
	/// kilonewton-metres, millimetres with in⁴ — cannot be assembled.
	/// </summary>
	public partial class DisplaySettingsWindow : Window
	{
		private static readonly string[] Formats = { "Decimal", "Scientific", "Automatic" };

		internal DisplaySettings Result { get; private set; } = new();

		internal DisplaySettingsWindow(DisplaySettings current, Window owner)
		{
			InitializeComponent();
			Owner = owner;

			CbForce.ItemsSource = new[] { "kN", "N", "MN", "kip" };
			CbStress.ItemsSource = new[] { "MPa", "kPa", "N/mm²", "ksi" };
			CbLength.ItemsSource = new[] { "mm", "cm", "m", "in" };

			foreach (var cb in new[] { CbForceFmt, CbMomentFmt, CbStressFmt,
					CbLengthFmt, CbAreaFmt, CbInertiaFmt })
				cb.ItemsSource = Formats;

			// The derived labels follow the two choices above them, live.
			CbForce.SelectionChanged += (_, _) => SyncDerived();
			CbLength.SelectionChanged += (_, _) => SyncDerived();

			Load(current.Clone());
		}

		private void Load(DisplaySettings s)
		{
			CbForce.SelectedIndex = (int)s.Force;
			CbStress.SelectedIndex = (int)s.Stress;
			CbLength.SelectedIndex = (int)s.Length;

			TxtForceDp.Text = s.ForceDecimals.ToString(CultureInfo.InvariantCulture);
			TxtMomentDp.Text = s.MomentDecimals.ToString(CultureInfo.InvariantCulture);
			TxtStressDp.Text = s.StressDecimals.ToString(CultureInfo.InvariantCulture);
			TxtLengthDp.Text = s.LengthDecimals.ToString(CultureInfo.InvariantCulture);
			TxtGapDp.Text = s.SmallLengthDecimals.ToString(CultureInfo.InvariantCulture);
			TxtAreaDp.Text = s.AreaDecimals.ToString(CultureInfo.InvariantCulture);
			TxtInertiaDp.Text = s.InertiaDecimals.ToString(CultureInfo.InvariantCulture);
			TxtAngleDp.Text = s.AngleDecimals.ToString(CultureInfo.InvariantCulture);
			TxtRatioDp.Text = s.RatioDecimals.ToString(CultureInfo.InvariantCulture);
			TxtPercentDp.Text = s.PercentDecimals.ToString(CultureInfo.InvariantCulture);

			CbForceFmt.SelectedIndex = (int)s.ForceFormat;
			CbMomentFmt.SelectedIndex = (int)s.MomentFormat;
			CbStressFmt.SelectedIndex = (int)s.StressFormat;
			CbLengthFmt.SelectedIndex = (int)s.LengthFormat;
			CbAreaFmt.SelectedIndex = (int)s.AreaFormat;
			CbInertiaFmt.SelectedIndex = (int)s.InertiaFormat;

			SyncDerived();
		}

		/// <summary>Show what the force and length choice implies for the derived quantities.</summary>
		private void SyncDerived()
		{
			var probe = new DisplaySettings
			{
				Force = (ForceUnit)Math.Max(0, CbForce.SelectedIndex),
				Length = (LengthUnit)Math.Max(0, CbLength.SelectedIndex),
			};
			TxtMomentUnit.Text = probe.MomentLabel;
			TxtAreaUnit.Text = probe.AreaLabel;
			TxtInertiaUnit.Text = probe.InertiaLabel;
		}

		/// <summary>Digits only, so the field cannot hold something that will not parse.</summary>
		private void Digits_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			e.Handled = !e.Text.All(char.IsAsciiDigit);
		}

		/// <summary>
		/// A typed decimal count, clamped rather than rejected.
		///
		/// 0 to 9: below 0 is meaningless and above 9 exceeds what a double carries in the range
		/// these quantities occupy — a "12 decimals" that silently prints noise would be worse than
		/// a value the dialog quietly brought back into range. An empty or unparseable field keeps
		/// the current setting rather than resetting to zero.
		/// </summary>
		private static int Dp(TextBox tb, int fallback) =>
			int.TryParse(tb.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v)
				? Math.Clamp(v, 0, 9)
				: fallback;

		private static NumberFormat Fmt(ComboBox cb) =>
			(NumberFormat)Math.Max(0, cb.SelectedIndex);

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			var d = new DisplaySettings();   // for the fallbacks

			Result = new DisplaySettings
			{
				Force = (ForceUnit)Math.Max(0, CbForce.SelectedIndex),
				Stress = (StressUnit)Math.Max(0, CbStress.SelectedIndex),
				Length = (LengthUnit)Math.Max(0, CbLength.SelectedIndex),

				ForceDecimals = Dp(TxtForceDp, d.ForceDecimals),
				MomentDecimals = Dp(TxtMomentDp, d.MomentDecimals),
				StressDecimals = Dp(TxtStressDp, d.StressDecimals),
				LengthDecimals = Dp(TxtLengthDp, d.LengthDecimals),
				SmallLengthDecimals = Dp(TxtGapDp, d.SmallLengthDecimals),
				AreaDecimals = Dp(TxtAreaDp, d.AreaDecimals),
				InertiaDecimals = Dp(TxtInertiaDp, d.InertiaDecimals),
				AngleDecimals = Dp(TxtAngleDp, d.AngleDecimals),
				RatioDecimals = Dp(TxtRatioDp, d.RatioDecimals),
				PercentDecimals = Dp(TxtPercentDp, d.PercentDecimals),

				ForceFormat = Fmt(CbForceFmt),
				MomentFormat = Fmt(CbMomentFmt),
				StressFormat = Fmt(CbStressFmt),
				LengthFormat = Fmt(CbLengthFmt),
				AreaFormat = Fmt(CbAreaFmt),
				InertiaFormat = Fmt(CbInertiaFmt),
			};
			DialogResult = true;
		}

		private void Restore_Click(object sender, RoutedEventArgs e) => Load(new DisplaySettings());
	}
}
