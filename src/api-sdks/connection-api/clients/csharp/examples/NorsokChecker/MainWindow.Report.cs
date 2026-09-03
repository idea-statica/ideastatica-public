using System.IO;
using System.Windows;
using NorsokChecker.Models;
using NorsokChecker.Services;

namespace NorsokChecker
{
	/// <summary>
	/// The Report tab and the PDF export — the same HTML, shown in a WebView2 and printed from it.
	/// </summary>
	public partial class MainWindow
	{
		/// <summary>
		/// The report HTML the tab would show, without a WebView2. Lets a test see what the app
		/// produced rather than what the generator is capable of producing — the two came apart when
		/// the Report tab went blank after a run that had no chapter ticked.
		/// </summary>
		internal string BuildReportHtmlForTest() => BuildReportHtml();

		private string BuildReportHtml(bool expandAll = false)
		{
			var allResults = new List<(string connectionName, List<NorsokFormulaResult> formulas)>();
			foreach (var (conId, formulas) in _formulaResults)
			{
				var conName = _connections.FirstOrDefault(c => c.Id == conId)?.Name ?? $"Connection {conId}";
				allResults.Add((conName, formulas));
			}

			// The topologies, keyed by connection NAME — the generator has no ids, and the figures
			// are keyed the same way. Built here rather than passed as the id-keyed dictionary so
			// the generator stays free of the app's identifiers.
			var topologies = new Dictionary<string, Services.Norsok64.JointTopology>();
			foreach (var (conId, topo) in _topologyPerConnection)
			{
				var name = _connections.FirstOrDefault(c => c.Id == conId)?.Name;
				if (!string.IsNullOrEmpty(name)) topologies[name] = topo;
			}

			// The figures were rendered during the run — see _jointFigures. Building the report stays
			// synchronous: it assembles what is already known and fetches nothing.
			// The footer as CSS: its mode, label and starting number are per-export, so the
			// generator cannot bake them into its static stylesheet — and handing it the rule rather
			// than the PageSetup keeps the generator free of the app's settings types.
			// What was searched, in numbers — the report could not say whether a governing state
			// came out of fifteen candidates or three. Counted over connections whose load effects
			// actually read; a connection whose read failed carries -1 and must not be summed in.
			var counted = _connections.Where(c => c.TotalLoadEffects >= 0).ToList();
			(int, int)? loadEffectCounts = counted.Count == 0
				? null
				: (counted.Where(c => c.ActiveLoadEffects >= 0).Sum(c => c.ActiveLoadEffects),
					counted.Sum(c => c.TotalLoadEffects));

			return NorsokHtmlReportGenerator.GenerateReport(
				Path.GetFileName(TxtProjectFile.Text), allResults, expandAll, _jointFigures,
				topologies, NorsokHtmlReportGenerator.FooterCss(_pageSetup), loadEffectCounts);
		}

		/// <summary>
		/// A joint figure per connection, as base64 PNG, for the report.
		///
		/// Rendered here rather than in the generator: it needs the WPF control and the topology, and
		/// a generator that reached for either could no longer be called without a UI thread.
		///
		/// An OFF-SCREEN view, not the one on the §6.4 tab. Reusing that one would leave whatever the
		/// report wanted painted on the tab the user is looking at — a different load effect, a
		/// different rotation — and the two would fight over one control.
		///
		/// Only a joint that was actually ASSESSED gets a figure, and the check is made before any work
		/// is done for it. Two reasons, and the second is the reason it is a guard rather than a
		/// filter at the end: the figure is coloured by utilisation, so a joint with no utilisation
		/// has nothing to show and an uncoloured picture beside a "not assessed" card would imply
		/// otherwise — and rendering one costs ~0.3–1.3 s (measured per connection in
		/// FigureLiveProbe), which is pure waste for a picture the report then declines to print.
		///
		/// A topology is NOT enough on its own: one is built for a rejected joint too, precisely so
		/// its errors can be listed. The verdict is what separates them — N/A means assessed == 0.
		/// PARTIAL does get a figure: something in it was checked, so there is something to colour.
		///
		/// Called from the RUN, once per connection, while its bodies are being fetched anyway — not
		/// when the report is built. Two things forced that. The bodies come from MeshesForAsync, and
		/// reading its CACHE instead gave a figure only to the connections the user had clicked (in
		/// practice CON1, the one selected when a project opens); and building the report must stay
		/// synchronous, because a report that waits on HTTP can fail there, and a failure that skips
		/// the navigation leaves an uninitialised WebView2 — which paints black with no message.
		///
		/// Never throws. A figure is an illustration: a joint that cannot be drawn costs its own
		/// picture and nothing more, and must not take the run down with it.
		/// </summary>
		private async Task RenderJointFigureAsync(int conId)
		{
			if (!_topologyPerConnection.TryGetValue(conId, out var topo)) return;

			var con = _connections.FirstOrDefault(c => c.Id == conId);
			string? name = con?.Name;
			if (string.IsNullOrEmpty(name)) return;

			// Nothing was assessed here — no utilisation, so no figure, and no work spent making one.
			//
			// The review asked for a figure on these too, so a claim like "gap -16 mm" or "IPE100 is
			// RolledI" could be checked by eye. Tried and reverted, on the user's judgement and for a
			// concrete reason: NEITHER of those conditions is visible in this view. It is a
			// projection along the plane normal, where a toe-to-toe gap is foreshortened to nothing
			// and a section TYPE does not appear at all. The picture would not verify the claim, it
			// would merely sit next to it — and an uncoloured figure beside a rejection invites the
			// reader to look for something the image cannot show.
			//
			// These conditions are taken on trust from the numbers in the rejection card, which is
			// what the card is for.
			if (con!.NorsokPass == "N/A") return;

			try
			{
				var meshes = await MeshesForAsync(conId, name);
				if (meshes.Count == 0) return;

				var view = new Controls.Joint3DView
				{
					Interactive = false,
					ShowMemberLabels = true,
					ChromeVisible = false,
				};
				view.Load(meshes);

				var n = topo.NPlane;
				var axis = topo.Ex;
				if (n.Norm > 1e-9)
					view.LookAtPlane(
						new System.Windows.Media.Media3D.Vector3D(n.X, n.Y, n.Z),
						new System.Windows.Media.Media3D.Vector3D(axis.X, axis.Y, axis.Z));

				// The envelope, always: a report is not looking at one load effect, and the
				// governing state per brace is what its result rows carry.
				view.ColourByUtilisation(
					UtilisationByMember(topo, envelope: true, leId: null),
					topo.Chord?.Id ?? -1);

				byte[]? png = view.RenderToPng();
				if (png != null) _jointFigures[name] = Convert.ToBase64String(png);
			}
			catch (Exception ex)
			{
				// A figure is an illustration; the report is still valid without one.
				Log($"  WARNING: the joint figure for {name} could not be rendered ({ex.Message})");
			}
		}

		/// <summary>
		/// Fill the Report tab, saying so on screen while it happens.
		///
		/// The overlay is not decoration. An un-navigated WebView2 paints BLACK, which is the colour
		/// of a failure, so every second of work here read as "the report is broken" — it produced
		/// three wrong diagnoses before anyone measured the actual cost (~22 s on fifteen joints, see
		/// FigureLiveProbe). Two things follow: the view has a white DefaultBackgroundColor, and this
		/// method never leaves the tab blank without a reason printed in it.
		///
		/// async void, so an exception escaping it is unhandled — everything is inside the try, and
		/// the failure is shown rather than only logged.
		/// </summary>
		private async void PopulateReportTab()
		{
			ReportBusyDetail.Text = $"{_formulaResults.Count} connection(s)";
			ReportBusy.Visibility = Visibility.Visible;

			var sw = System.Diagnostics.Stopwatch.StartNew();
			try
			{
				var html = BuildReportHtml();
				Log($"  report HTML built in {sw.ElapsedMilliseconds} ms ({html.Length / 1024} kB)");

				sw.Restart();
				await Services.WebViewEnvironment.EnsureAsync(ReportWebView);
				ReportWebView.NavigateToString(html);
				Log($"  report shown in {sw.ElapsedMilliseconds} ms");

				// Only on success: on failure the overlay stays up, carrying the reason.
				ReportBusy.Visibility = Visibility.Collapsed;
			}
			catch (Exception ex)
			{
				// On screen, not only in the log: a blank tab with the reason elsewhere is what made
				// this so hard to diagnose.
				ReportBusyDetail.Text = $"The report could not be shown: {ex.Message}";
				Log($"WARNING: the report could not be shown ({ex.Message}).");
				AppLog.ReportFailure("Populating the Report tab failed", ex);
			}
		}

		/// <summary>
		/// The page the report prints on. Held for the session so a second export does not re-ask;
		/// not persisted, because this app has no settings store and inventing one is a larger change
		/// than the page setup itself.
		/// </summary>
		private Models.PageSetup _pageSetup = new();

		/// <summary>The page setup, for a test to read back what the dialog left.</summary>
		internal Models.PageSetup PageSetupForTest => _pageSetup;

		private void PageSetup_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new Controls.PageSetupWindow(_pageSetup, this);
			if (dlg.ShowDialog() == true)
			{
				_pageSetup = dlg.Result;
				Log($"  page setup: {(_pageSetup.IsLetter ? "Letter" : "A4")} "
					+ $"{(_pageSetup.Landscape ? "landscape" : "portrait")}, margins "
					+ $"L{_pageSetup.MarginLeftMm:0.#} R{_pageSetup.MarginRightMm:0.#} "
					+ $"T{_pageSetup.MarginTopMm:0.#} B{_pageSetup.MarginBottomMm:0.#} mm, "
					+ $"backgrounds {(_pageSetup.PrintBackgrounds ? "on" : "OFF")}");
			}
		}

		/// <summary>
		/// Export the NORSOK report: the same HTML the tab shows, printed to PDF through WebView2 with
		/// every card expanded. One file — the second, IDEA StatiCa's own report via the API, was
		/// dropped with the CBFEM chapter.
		/// </summary>
		private async void ExportPdf_Click(object sender, RoutedEventArgs e)
		{
			if (_apiClient == null || _projectId == Guid.Empty || _connections.Count == 0 || _formulaResults.Count == 0)
			{
				MessageBox.Show("Run the Norsok check first.", "PDF Export", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var dlg = new Microsoft.Win32.SaveFileDialog
			{
				Filter = "PDF files (*.pdf)|*.pdf",
				FileName = $"{Path.GetFileNameWithoutExtension(TxtProjectFile.Text)}-NORSOK-report.pdf",
				Title = "Save the NORSOK report"
			};
			if (dlg.ShowDialog() != true) return;

			string norsokPdf = dlg.FileName;

			BtnExportPdf.IsEnabled = false;
			try
			{
				Telemetry.ReportExportClicked();

				// One file: the NORSOK report. The export used to write IDEA StatiCa's own report
				// beside it via the API, which meant re-running a calculation here to obtain a
				// document Connection produces better — tuned to the model as it is worked on there.
				ShowStatus("Exporting NORSOK compliance report to PDF...");
				await Services.WebViewEnvironment.EnsureAsync(ReportWebView);

				var navigated = new TaskCompletionSource<bool>();
				void OnNavCompleted(object? s, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs a)
				{
					ReportWebView.NavigationCompleted -= OnNavCompleted;
					navigated.TrySetResult(a.IsSuccess);
				}
				// All cards expanded — the customer must see every formula in the PDF
				ReportWebView.NavigationCompleted += OnNavCompleted;
				ReportWebView.NavigateToString(BuildReportHtml(expandAll: true));
				await navigated.Task;
				await Task.Delay(1200); // allow KaTeX formulas and web fonts to settle (all cards render)

				// The page, explicitly. Passing null took WebView2's defaults: 8.5 × 11 in — US
				// LETTER, not A4 — and ShouldPrintBackgrounds = false, which drops every background
				// colour. This report encodes PASS/FAIL and the eleven-band utilisation scale in
				// background colour, so the exported PDF had a failing joint looking like a passing
				// one. See Models.PageSetup.
				var print = ReportWebView.CoreWebView2.Environment.CreatePrintSettings();
				print.PageWidth = _pageSetup.WidthInches;
				print.PageHeight = _pageSetup.HeightInches;
				print.MarginLeft = _pageSetup.MarginLeftInches;
				print.MarginRight = _pageSetup.MarginRightInches;
				print.MarginTop = _pageSetup.MarginTopInches;
				print.MarginBottom = _pageSetup.MarginBottomInches;
				print.ShouldPrintBackgrounds = _pageSetup.PrintBackgrounds;
				// The report carries its own header; WebView2's would add the print date, the page
				// title and a file:// URI on every page.
				print.ShouldPrintHeaderAndFooter = false;

				// PDF document properties are NOT settable here, and the review's §3 asked for them.
				// Verified by compiling against the pinned SDK (1.0.2903.40): CoreWebView2PrintSettings
				// has no Title, Subject or Author — the assembly does contain those strings, but on
				// other types, so reading them out of the DLL was not evidence.
				//
				// What the exported file therefore carries: /Title from the HTML <title> (now the real
				// document name, not "Compliance Report"), /Creator = the WebView2 user agent, and no
				// /Subject or /Author. Fixing the rest needs a post-processing pass over the finished
				// PDF — PdfSharp can write those fields — which is the same pass /Outlines would need
				// and is deferred with it.

				bool ok = await ReportWebView.CoreWebView2.PrintToPdfAsync(norsokPdf, print);
				if (!ok)
					throw new InvalidOperationException("WebView2 PrintToPdf reported failure.");
				Log($"  NORSOK report: {norsokPdf}");

				// Restore the interactive (collapsible) report view in the app
				PopulateReportTab();

				Log("PDF export completed.");
				Telemetry.ReportExported();

				MessageBox.Show("Exported:" + Environment.NewLine + norsokPdf, "PDF Export",
					MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				Telemetry.ReportExportFailed(ex);
				AppLog.ReportFailure("Exporting the PDF report failed", ex);
				Log($"ERROR exporting PDF: {ex.Message}");
				MessageBox.Show(ex.Message, "PDF Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				BtnExportPdf.IsEnabled = true;
				HideStatus();
			}
		}

	}
}
