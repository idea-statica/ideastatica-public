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
		/// <param name="connectionName">The joint this brace belongs to.</param>
		/// <param name="loadEffectName">
		/// The mode being inspected — a load effect's name in per-LC mode, or "envelope". Only used
		/// when the row does not name a governing state of its own; see below.
		/// </param>
		public Joint64DerivationWindow(Joint64RowView row, Window owner,
			string connectionName = "", string loadEffectName = "")
		{
			InitializeComponent();
			// WPF refuses an Owner that has never been shown, and throws rather than ignoring it. The
			// app always passes a live window, so this only guards the case where the owner is not on
			// screen yet — losing the owner relationship is a far smaller matter than not opening.
			if (owner.IsLoaded) Owner = owner;

			// The STATE this derivation is of. An envelope is not a state: the numbers below belong to
			// the one load effect that governs THIS brace, and in an envelope each brace may be
			// governed by a different one — so naming the mode would leave two windows both titled
			// "envelope" while showing different states. The row carries the governing state precisely
			// for this, and it is resolved BEFORE the title so the title gets it too (it did not, and
			// the taskbar showed "envelope" where it should have shown "LE7").
			string state = !string.IsNullOrEmpty(row.GoverningLe)
				? row.GoverningLe
				: loadEffectName;

			// Connection · state · brace, in the WINDOW TITLE as well as on the page: these windows are
			// meant to be left open side by side while other braces are inspected, and a title of just
			// "M4" cannot be told from another joint's M4 on the taskbar.
			string who = string.Join(" · ", new[] { connectionName, state, row.Brace }
				.Where(s => !string.IsNullOrEmpty(s)));
			Title = $"NORSOK §6.4 — {who}";

			if (row.Detail == null)
			{
				ShowFallback("No derivation is available for this brace.");
				return;
			}

			// On the page the state is qualified as governing, which the title has no room for.
			string pageState = !string.IsNullOrEmpty(row.GoverningLe)
				? $"governing {row.GoverningLe}"
				: loadEffectName;

			string html = NorsokHtmlReportGenerator.GenerateDerivationPage(
				row.Detail,
				brace: row.Brace,
				connection: connectionName,
				state: pageState,
				utilisation: row.Util,
				verdict: row.Verdict);

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
