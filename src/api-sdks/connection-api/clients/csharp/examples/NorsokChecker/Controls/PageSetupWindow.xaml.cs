using System.Globalization;
using System.Windows;
using NorsokChecker.Models;

namespace NorsokChecker.Controls
{
	/// <summary>
	/// Page setup for the PDF export: paper, orientation, the four margins and whether background
	/// colours are printed.
	///
	/// Edits a COPY and hands it back only on OK, so Cancel really cancels — editing the caller's
	/// instance in place would leave half a setup behind when the user changed their mind.
	///
	/// The arithmetic and the defaults live in <see cref="PageSetup"/>, not here: a millimetre that
	/// reaches the API unconverted prints a wrong page without failing, and that has to be
	/// assertable without constructing a window.
	/// </summary>
	public partial class PageSetupWindow : Window
	{
		private readonly PageSetup _edited;

		/// <summary>The setup as the user left it. Only meaningful when ShowDialog returned true.</summary>
		internal PageSetup Result => _edited;

		internal PageSetupWindow(PageSetup current, Window? owner = null)
		{
			InitializeComponent();

			_edited = current.Clone();
			// Owner cannot be set to a window that has not been shown — the derivation window learnt
			// this the hard way (it threw). CenterOwner then falls back to CenterScreen.
			if (owner is { IsLoaded: true }) Owner = owner;

			Load(_edited);
		}

		private void Load(PageSetup s)
		{
			RbLetter.IsChecked = s.IsLetter;
			RbA4.IsChecked = !s.IsLetter;
			RbLandscape.IsChecked = s.Landscape;
			RbPortrait.IsChecked = !s.Landscape;

			TxtLeft.Text = Mm(s.MarginLeftMm);
			TxtRight.Text = Mm(s.MarginRightMm);
			TxtTop.Text = Mm(s.MarginTopMm);
			TxtBottom.Text = Mm(s.MarginBottomMm);

			ChkBackgrounds.IsChecked = s.PrintBackgrounds;

			CmbFooterMode.SelectedIndex = s.FooterMode switch
			{
				FooterMode.Continuous => 1,
				FooterMode.Off => 2,
				_ => 0,
			};
			TxtFooterLabel.Text = s.FooterLabel;
			TxtFooterStartAt.Text = s.FooterStartAt.ToString(CultureInfo.InvariantCulture);
			UpdateFooterDetail();
		}

		/// <summary>
		/// What the chosen mode means, in the dialog, next to the choice. `Off` is the answer for a
		/// report inserted into a document that paginates everything itself — and the margins above
		/// are where a colliding host footer is dealt with, which is why the two sit together.
		/// </summary>
		private void UpdateFooterDetail()
		{
			// Called by the ComboBox's SelectionChanged, which WPF raises during InitializeComponent
			// before the other controls exist.
			if (LblFooterHint == null || PnlFooterDetail == null) return;

			int i = CmbFooterMode.SelectedIndex;
			LblFooterHint.Text = i switch
			{
				1 => "One running number, no total — for binding the report into a larger document "
					+ "at a known position. A total would describe our document, not the host's.",
				2 => "No footer at all — for a host document that paginates and numbers everything "
					+ "itself. Also avoids a collision with the host's own footer.",
				_ => "Page number and total, prefixed by the label — citable on its own.",
			};
			PnlFooterDetail.Visibility = i == 2 ? Visibility.Collapsed : Visibility.Visible;
			// The starting number only means anything in continuous mode.
			LblStartAt.Visibility = TxtFooterStartAt.Visibility =
				i == 1 ? Visibility.Visible : Visibility.Collapsed;
		}

		private void FooterMode_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
			=> UpdateFooterDetail();

		/// <summary>InvariantCulture both ways, so a decimal comma cannot round-trip into a wrong number.</summary>
		private static string Mm(double v) => v.ToString("0.#", CultureInfo.InvariantCulture);

		private void RestoreDefaults_Click(object sender, RoutedEventArgs e) => Load(new PageSetup());

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			// Parse first: a field the user cleared or typed a word into must be named, not silently
			// treated as zero — a zero margin prints to the paper's edge on most printers.
			if (!TryMargins(out double left, out double right, out double top, out double bottom, out string? bad))
			{
				ShowError($"'{bad}' is not a valid margin. Use millimetres, e.g. 15.");
				return;
			}

			_edited.IsLetter = RbLetter.IsChecked == true;
			_edited.Landscape = RbLandscape.IsChecked == true;
			_edited.MarginLeftMm = left;
			_edited.MarginRightMm = right;
			_edited.MarginTopMm = top;
			_edited.MarginBottomMm = bottom;
			_edited.PrintBackgrounds = ChkBackgrounds.IsChecked == true;

			_edited.FooterMode = CmbFooterMode.SelectedIndex switch
			{
				1 => FooterMode.Continuous,
				2 => FooterMode.Off,
				_ => FooterMode.Local,
			};
			_edited.FooterLabel = TxtFooterLabel.Text?.Trim() ?? "";
			// Named, not silently defaulted: a start-at the user typed a word into would otherwise
			// print from page 1 with no sign that the setting was ignored.
			if (_edited.FooterMode == FooterMode.Continuous)
			{
				if (!int.TryParse(TxtFooterStartAt.Text?.Trim(), NumberStyles.Integer,
						CultureInfo.InvariantCulture, out int startAt))
				{
					ShowError($"'{TxtFooterStartAt.Text}' is not a page number. Use a whole number, e.g. 47.");
					return;
				}
				_edited.FooterStartAt = startAt;
			}

			// The model decides what is printable: the print API throws on a negative margin, and a
			// pair of margins wider than the paper leaves no content area without throwing at all.
			if (!_edited.IsValid(out string? error))
			{
				ShowError(error!);
				return;
			}

			DialogResult = true;
			Close();
		}

		private bool TryMargins(out double left, out double right, out double top, out double bottom,
			out string? bad)
		{
			left = right = top = bottom = 0;
			bad = null;

			if (!TryMm(TxtLeft.Text, out left)) { bad = TxtLeft.Text; return false; }
			if (!TryMm(TxtRight.Text, out right)) { bad = TxtRight.Text; return false; }
			if (!TryMm(TxtTop.Text, out top)) { bad = TxtTop.Text; return false; }
			if (!TryMm(TxtBottom.Text, out bottom)) { bad = TxtBottom.Text; return false; }
			return true;
		}

		/// <summary>
		/// Accepts both decimal separators, because the app runs on Czech machines where the numeric
		/// keypad produces a comma and the invariant parse would reject "15,5".
		/// </summary>
		private static bool TryMm(string text, out double mm) =>
			double.TryParse(text?.Trim().Replace(',', '.'), NumberStyles.Float,
				CultureInfo.InvariantCulture, out mm);

		private void ShowError(string message)
		{
			LblError.Text = message;
			LblError.Visibility = Visibility.Visible;
		}
	}
}
