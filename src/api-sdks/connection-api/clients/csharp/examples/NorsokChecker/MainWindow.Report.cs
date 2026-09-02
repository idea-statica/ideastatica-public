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

			return NorsokHtmlReportGenerator.GenerateReport(
				Path.GetFileName(TxtProjectFile.Text), allResults, expandAll, RenderJointFigures());
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
		/// Only connections with a topology get a figure. A joint the chapter rejected has no
		/// envelope to colour by, and an uncoloured picture beside a "not assessed" card would imply
		/// there was something to see.
		/// </summary>
		private Dictionary<string, string> RenderJointFigures()
		{
			var figures = new Dictionary<string, string>();

			foreach (var (conId, topo) in _topologyPerConnection)
			{
				if (!_meshesPerConnection.TryGetValue(conId, out var meshes) || meshes.Count == 0)
					continue;

				string? name = _connections.FirstOrDefault(c => c.Id == conId)?.Name;
				if (string.IsNullOrEmpty(name)) continue;

				try
				{
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
					if (png != null) figures[name] = Convert.ToBase64String(png);
				}
				catch (Exception ex)
				{
					// A figure is an illustration; the report is still valid without one.
					Log($"  WARNING: the joint figure for {name} could not be rendered ({ex.Message})");
				}
			}

			return figures;
		}

		private async void PopulateReportTab()
		{
			var html = BuildReportHtml();

			try
			{
				await Services.WebViewEnvironment.EnsureAsync(ReportWebView);
				ReportWebView.NavigateToString(html);
			}
			catch (Exception ex)
			{
				Log($"WARNING: WebView2 not available ({ex.Message}).");
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

				bool ok = await ReportWebView.CoreWebView2.PrintToPdfAsync(norsokPdf, null);
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
