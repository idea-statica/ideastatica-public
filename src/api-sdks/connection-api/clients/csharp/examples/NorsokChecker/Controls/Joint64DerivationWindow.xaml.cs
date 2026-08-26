using System.Windows;
using NorsokChecker.Models;
using NorsokChecker.Services;

namespace NorsokChecker.Controls
{
	/// <summary>
	/// The full derivation behind one brace's §6.4 check — Qu, Qf, Qg, A², N_Rd, M_Rd, the chord
	/// stress trail and eq (6.57), each step symbolic and then with the numbers substituted.
	///
	/// A window rather than the python reference's in-page modal: WPF has no modal-in-page idiom,
	/// and a separate window can be left open beside the table while other braces are inspected,
	/// which the modal could not.
	/// </summary>
	public partial class Joint64DerivationWindow : Window
	{
		public Joint64DerivationWindow(Joint64RowView row, Window owner)
		{
			InitializeComponent();
			Owner = owner;
			Title = $"NORSOK §6.4 — {row.Brace}"
				+ (string.IsNullOrEmpty(row.GoverningLe) ? "" : $" ({row.GoverningLe})");

			if (row.Detail == null)
			{
				ShowFallback("No derivation is available for this brace.");
				return;
			}

			string subtitle = $"{row.Brace} — utilisation {row.Util}, {row.Verdict}"
				+ (string.IsNullOrEmpty(row.GoverningLe) ? "" : $" · governing {row.GoverningLe}");
			string html = NorsokHtmlReportGenerator.GenerateDerivationPage(row.Detail, subtitle);

			Loaded += async (_, _) =>
			{
				try
				{
					await Web.EnsureCoreWebView2Async();
					Web.NavigateToString(html);
				}
				catch (Exception ex)
				{
					// WebView2 missing is the one plausible failure; the numbers are still in the
					// table and the report, so say where to look rather than showing an empty window
					ShowFallback("The derivation needs the Microsoft Edge WebView2 runtime, which "
						+ $"could not be started ({ex.Message}). The same derivation is in the "
						+ "Report tab.");
				}
			};
		}

		private void ShowFallback(string message)
		{
			Web.Visibility = Visibility.Collapsed;
			Fallback.Text = message;
			Fallback.Visibility = Visibility.Visible;
		}
	}
}
