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
		}

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
