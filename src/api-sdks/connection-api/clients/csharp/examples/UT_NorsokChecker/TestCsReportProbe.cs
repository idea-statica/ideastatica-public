using System.IO;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using Microsoft.Web.WebView2.Wpf;
using NorsokChecker.Controls;
using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Chapters;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Produce the report PDF for the REAL project — test_cs.ideaCon, all fifteen connections —
	/// through the same path the app takes, and print it.
	///
	/// Why not the synthetic fixture in PrintedPageProbe: that one proves the defects are gone and
	/// the typography holds, but its joints are invented. The report review was written against
	/// test_cs (15 connections, 6 assessed, 30 checks), so a reply carrying a different model gives
	/// the reviewer nothing to compare against — and the interesting cases are exactly the ones a
	/// fixture does not have: CON10's unreadable load effects, CON5/CON6's six and seven unmet
	/// conditions, the K/Y/X mixtures.
	///
	/// Probe: needs the live 26.0 service and the .ideaCon. STA, because the figures need WPF.
	/// Prints the path of the PDF it wrote — read that file, do not trust this test's own asserts.
	///
	/// Explicit, not merely Category: a category alone does not stop a plain `dotnet test`, and this
	/// probe starts a service and renders a report. It used to write straight into the folder the
	/// reviewer's deliverable is sent from, so an ordinary test run silently replaced a file nobody
	/// had decided to change. It now writes to TEMP unless NORSOK_PROBE_PDF says otherwise.
	/// </summary>
	[TestFixture, Category("Probe"), Explicit("Starts a live service and renders the whole report")]
	[Apartment(System.Threading.ApartmentState.STA)]
	public class TestCsReportProbe
	{
		private const string IdeaCon =
			@"C:\Users\OndrejSkorunka\Claude\01_Folders\NORSOK\ideacon\test_cs.ideaCon";

		private static readonly string OutPdf =
			Environment.GetEnvironmentVariable("NORSOK_PROBE_PDF")
			?? Path.Combine(Path.GetTempPath(), "NORSOK-report-probe.pdf");

		private static string SetupDir =>
			Environment.GetEnvironmentVariable("IDEASTATICA_SETUP_DIR")
			?? @"C:\Program Files\IDEA StatiCa\StatiCa 26.0";

		private ConnectionApiServiceRunner? _runner;

		[OneTimeSetUp]
		public void Setup()
		{
			if (!File.Exists(IdeaCon)) Assert.Ignore($"no test project at {IdeaCon}");
			if (!Directory.Exists(SetupDir)) Assert.Ignore($"no IDEA StatiCa install at {SetupDir}");

			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}
			_runner = new ConnectionApiServiceRunner(SetupDir);
		}

		[OneTimeTearDown]
		public void Teardown() => _runner?.Dispose();

		[Test]
		public void ExportTheRealProjectsReport()
		{
			var dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
			var frame = new System.Windows.Threading.DispatcherFrame();
			Exception? failure = null;
			string? html = null;

			// Everything inside the dispatcher frame, posted before the frame is pushed: WebView2
			// refuses to initialise unless the event loop is already running.
			dispatcher.BeginInvoke(new Action(async () =>
			{
				try { html = await BuildAsync(); if (html != null) await PrintAsync(html); }
				catch (Exception ex) { failure = ex; }
				finally { frame.Continue = false; }
			}));

			// A deadline: fifteen joints cost ~22 s of figures plus the checks, so five minutes is
			// generous — and a probe that hangs holds the test host and blocks every later build.
			var timer = new System.Windows.Threading.DispatcherTimer(
				TimeSpan.FromMinutes(5), System.Windows.Threading.DispatcherPriority.Background,
				(_, _) => frame.Continue = false, dispatcher);
			timer.Start();
			System.Windows.Threading.Dispatcher.PushFrame(frame);
			timer.Stop();

			if (failure != null) Assert.Fail($"{failure.GetType().Name}: {failure.Message}");
			Assert.That(File.Exists(OutPdf), Is.True, "the export did not finish within 5 minutes");
			TestContext.Out.WriteLine($"\nPDF: {new FileInfo(OutPdf).Length / 1024} kB -> {OutPdf}");
		}

		/// <summary>
		/// Run every chapter over every connection and build the report HTML — the run's own steps,
		/// in the run's own order, including the figures and the topologies.
		/// </summary>
		private async Task<string?> BuildAsync()
		{
			void Log(string _) { }

			var client = await _runner!.CreateApiClient();
			var project = await client.Project.OpenProjectAsync(IdeaCon);
			await new ProjectSettingsService(client, Log).ApplyNorsokFactorsAsync(project.ProjectId);

			var crossSections = await client.Material.GetCrossSectionsAsync(project.ProjectId);
			var sectionMap = JointSectionMap.FromCrossSections(crossSections.Cast<object>());

			var connections = project.Connections ?? new();
			TestContext.Out.WriteLine($"{connections.Count} connections in {Path.GetFileName(IdeaCon)}\n");

			var allResults = new List<(string, List<NorsokFormulaResult>)>();
			var figures = new Dictionary<string, string>();
			var topologies = new Dictionary<string, JointTopology>();

			foreach (var con in connections)
			{
				var rows = new List<NorsokFormulaResult>();
				var chapter = new Chapter64();
				JointTopology? topo = null;
				chapter.Topology = (_, t) => topo = t;

				// Guarded as the app guards it: CON10's braces are deleted, so its inherited load
				// effects reference members that no longer exist and the service answers 404. Passing
				// null is what makes the chapter report "could not be evaluated" — which is one of the
				// cases the review is about, so it must reach the report rather than be skipped here.
				List<ConLoadEffect>? loadEffects = new();
				try
				{
					var read = await client.LoadEffect.GetLoadEffectsAsync(
						project.ProjectId, con.Id, isPercentage: false);
					if (read != null) loadEffects.AddRange(read);
				}
				catch (Exception)
				{
					loadEffects = null;
				}

				// ACTIVE ONLY — what the app does by default (MainWindow.Run.cs:159, the checkbox
				// ships IsChecked="True"). A load effect the engineer disabled in IDEA StatiCa is one
				// they decided not to design for.
				//
				// Omitting this filter is not a small difference: measured, it flipped CON8's
				// classification from X 100 % to K 100 % and its utilisation from 59.2 % to 88.8 %,
				// on identical geometry — the disabled states change the joint's force balance, and
				// K and X take different Q_u from Table 6-3. The first version of this probe reported
				// those numbers as the app's output. They were the rig's.
				if (loadEffects != null)
				{
					int total = loadEffects.Count;
					loadEffects = loadEffects.Where(le => le.Active).ToList();
					if (loadEffects.Count < total)
						TestContext.Out.WriteLine(
							$"  {con.Name,-8} load effects: {loadEffects.Count} of {total} active");
				}

				var outcome = await chapter.EvaluateAsync(new ChapterContext
				{
					Client = client, ProjectId = project.ProjectId,
					ConnectionId = con.Id, ConnectionName = con.Name,
					LoadEffects = loadEffects, SectionMap = sectionMap, Log = Log,
				}, CancellationToken.None);

				rows.AddRange(outcome.Rows);
				allResults.Add((con.Name, rows));
				if (topo != null) topologies[con.Name] = topo;

				var verdict = CheckWorkflow.Roll(rows);
				TestContext.Out.WriteLine(
					$"  {con.Name,-8} {verdict.Pass,-8} {rows.Count,2} row(s)  {verdict.Status}");

				// The figure, on the same rule the app uses: only where something was assessed.
				if (verdict.Pass != "N/A" && topo != null)
				{
					try
					{
						string json = await client.Presentation.GetDataScene3DTextAsync(
							project.ProjectId, con.Id);
						var meshes = JointPresentationReader.ReadMembers(json, Log, null);
						if (meshes.Count > 0)
						{
							var view = new Joint3DView
							{
								Interactive = false, ShowMemberLabels = true, ChromeVisible = false,
							};
							view.Load(meshes);
							if (topo.NPlane.Norm > 1e-9)
								view.LookAtPlane(
									new System.Windows.Media.Media3D.Vector3D(
										topo.NPlane.X, topo.NPlane.Y, topo.NPlane.Z),
									new System.Windows.Media.Media3D.Vector3D(
										topo.Ex.X, topo.Ex.Y, topo.Ex.Z));

							byte[]? png = view.RenderToPng();
							if (png != null) figures[con.Name] = Convert.ToBase64String(png);
						}
					}
					catch (Exception ex)
					{
						TestContext.Out.WriteLine($"           (no figure: {ex.Message.Split('\n')[0]})");
					}
				}
			}

			await client.Project.CloseProjectAsync(project.ProjectId);

			TestContext.Out.WriteLine(
				$"\n{allResults.Count} connections, {figures.Count} figures, "
				+ $"{topologies.Count} topologies");

			return NorsokHtmlReportGenerator.GenerateReport(
				Path.GetFileName(IdeaCon), allResults, expandAll: true, figures, topologies);
		}

		/// <summary>Print the HTML with the app's own page setup.</summary>
		private static async Task PrintAsync(string html)
		{
			var view = new WebView2();
			var window = new System.Windows.Window
			{
				Content = view, Width = 900, Height = 700,
				Left = -10000, Top = -10000, ShowActivated = false,
			};
			window.Show();
			try
			{
				await WebViewEnvironment.EnsureAsync(view);

				var navigated = new TaskCompletionSource<bool>();
				void OnDone(object? s,
					Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs a)
				{
					view.NavigationCompleted -= OnDone;
					navigated.TrySetResult(a.IsSuccess);
				}
				view.NavigationCompleted += OnDone;
				view.NavigateToString(html);
				if (!await navigated.Task) throw new InvalidOperationException("navigation failed");

				// The app waits 1200 ms for KaTeX and the fonts; this document is larger.
				await Task.Delay(3000);

				var setup = new PageSetup();
				var print = view.CoreWebView2.Environment.CreatePrintSettings();
				print.PageWidth = setup.WidthInches;
				print.PageHeight = setup.HeightInches;
				print.MarginLeft = setup.MarginLeftInches;
				print.MarginRight = setup.MarginRightInches;
				print.MarginTop = setup.MarginTopInches;
				print.MarginBottom = setup.MarginBottomInches;
				print.ShouldPrintBackgrounds = setup.PrintBackgrounds;
				print.ShouldPrintHeaderAndFooter = false;

				Directory.CreateDirectory(Path.GetDirectoryName(OutPdf)!);
				if (!await view.CoreWebView2.PrintToPdfAsync(OutPdf, print))
					throw new InvalidOperationException("PrintToPdf reported failure");
			}
			finally
			{
				window.Close();
			}
		}
	}
}
