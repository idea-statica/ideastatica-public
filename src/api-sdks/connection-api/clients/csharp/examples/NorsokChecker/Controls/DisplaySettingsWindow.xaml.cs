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
	/// Two rows show a unit they do not let you choose, and that is deliberate rather than an
	/// omission — the tooltip on each says why. Angles are fixed at degrees because §6.4.3.1 states
	/// its validity ranges in degrees and the validity table is read against the clause; the
	/// dimensionless factors have no alternative unit to offer at all. Moments, area and inertia are
	/// DERIVED from the force and length choice and show what that choice implies, so an incoherent
	/// pair — newtons with kilonewton-metres, millimetres with in⁴ — cannot be assembled.
	/// </summary>
	public partial class DisplaySettingsWindow : Window
	{
		private readonly DisplaySettings _working;

		internal DisplaySettings Result { get; private set; } = new();

		internal DisplaySettingsWindow(DisplaySettings current, Window owner)
		{
			InitializeComponent();
			Owner = owner;
			_working = current.Clone();

			CbForce.ItemsSource = new[] { "kN", "N", "MN", "kip" };
			CbStress.ItemsSource = new[] { "MPa", "kPa", "N/mm²", "ksi" };
			CbLength.ItemsSource = new[] { "mm", "cm", "m", "in" };

			foreach (var cb in new[] { CbForceDp, CbMomentDp, CbStressDp, CbLengthDp,
					CbGapDp, CbAreaDp, CbInertiaDp, CbAngleDp, CbRatioDp })
				cb.ItemsSource = new[] { 0, 1, 2, 3, 4 };

			// The derived labels follow the two choices above them, live.
			CbForce.SelectionChanged += (_, _) => SyncDerived();
			CbLength.SelectionChanged += (_, _) => SyncDerived();

			Load(_working);
		}

		private void Load(DisplaySettings s)
		{
			CbForce.SelectedIndex = (int)s.Force;
			CbStress.SelectedIndex = (int)s.Stress;
			CbLength.SelectedIndex = (int)s.Length;

			CbForceDp.SelectedItem = s.ForceDecimals;
			CbMomentDp.SelectedItem = s.MomentDecimals;
			CbStressDp.SelectedItem = s.StressDecimals;
			CbLengthDp.SelectedItem = s.LengthDecimals;
			CbGapDp.SelectedItem = s.SmallLengthDecimals;
			CbAreaDp.SelectedItem = s.AreaDecimals;
			CbInertiaDp.SelectedItem = s.InertiaDecimals;
			CbAngleDp.SelectedItem = s.AngleDecimals;
			CbRatioDp.SelectedItem = s.RatioDecimals;

			ChkForceSci.IsChecked = s.ForceScientific;
			ChkMomentSci.IsChecked = s.MomentScientific;
			ChkStressSci.IsChecked = s.StressScientific;
			ChkAreaSci.IsChecked = s.AreaScientific;
			ChkInertiaSci.IsChecked = s.InertiaScientific;

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

		private static int Dp(ComboBox cb, int fallback) =>
			cb.SelectedItem is int v ? v : fallback;

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			Result = new DisplaySettings
			{
				Force = (ForceUnit)Math.Max(0, CbForce.SelectedIndex),
				Stress = (StressUnit)Math.Max(0, CbStress.SelectedIndex),
				Length = (LengthUnit)Math.Max(0, CbLength.SelectedIndex),

				ForceDecimals = Dp(CbForceDp, 1),
				MomentDecimals = Dp(CbMomentDp, 3),
				StressDecimals = Dp(CbStressDp, 1),
				LengthDecimals = Dp(CbLengthDp, 1),
				SmallLengthDecimals = Dp(CbGapDp, 1),
				AreaDecimals = Dp(CbAreaDp, 0),
				InertiaDecimals = Dp(CbInertiaDp, 2),
				AngleDecimals = Dp(CbAngleDp, 1),
				RatioDecimals = Dp(CbRatioDp, 3),

				ForceScientific = ChkForceSci.IsChecked == true,
				MomentScientific = ChkMomentSci.IsChecked == true,
				StressScientific = ChkStressSci.IsChecked == true,
				AreaScientific = ChkAreaSci.IsChecked == true,
				InertiaScientific = ChkInertiaSci.IsChecked == true,
			};
			DialogResult = true;
		}

		private void Restore_Click(object sender, RoutedEventArgs e) => Load(new DisplaySettings());
	}
}
