using System.IO;
using Microsoft.Web.WebView2.Wpf;
using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// PRINT the report through WebView2 and read back what reached the page.
	///
	/// The measurement no assertion on CSS can replace. Two claims about the exported PDF cannot be
	/// checked from the stylesheet at all:
	///   1. does `counter(page)` in an @page margin box actually render on THIS WebView2 build?
	///      It is documented from Chrome 131 and the SDK pins 1.0.2903.40 (= the 131 build), but a
	///      version mapping read from release notes is not the same as a number on a page.
	///   2. do the break rules take effect? `.index-page { break-after: page }` was asserted by its
	///      presence in the CSS and PASSED while the shipped PDF broke the contents across two pages.
	///
	/// So this prints a real PDF to a temp file and extracts its text. No service is needed —
	/// pagination does not depend on the model — but WPF and the WebView2 runtime are, hence STA
	/// and Probe: it does not run in the normal suite.
	///
	/// Run it with:  dotnet test --filter "FullyQualifiedName~PrintedPageProbe"
	/// </summary>
	[TestFixture, Category("Probe"), Apartment(System.Threading.ApartmentState.STA)]
	public class PrintedPageProbe
	{
		[OneTimeSetUp]
		public void Setup()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}
		}

		/// <summary>Enough connections that the document runs to several pages.</summary>
		private static string Report()
		{
			var inputs = Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 60.0, g: 0.047,
				frK: 1.0, frY: 0.0, frX: 0.0,
				nSd: -10e3, mipSd: -1e3, mopSd: 0,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0,
				gammaM: 1.15);

			NorsokFormulaResult Assessed(string brace) => new()
			{
				Section = "6.4.3.6", Equation = "6.57", Title = $"Tubular Joint — {brace}",
				LoadCaseName = "LE1", Utilization = 0.737, Passed = true,
				JointDetail = new JointCheckRow
				{
					Name = brace, Skipped = false, Util = 0.737, Passed = true,
					Engine = Norsok64Engine.CheckJoint(inputs), Inputs = inputs, DomClass = "K",
					Classification = new KyxClass { Name = brace, FrK = 1.0, FrY = 0, FrX = 0 },
				},
			};

			var cons = new List<(string, List<NorsokFormulaResult>)>();
			for (int i = 1; i <= 6; i++)
				cons.Add(($"CON{i}", new List<NorsokFormulaResult>
					{ Assessed("M1"), Assessed("M2"), Assessed("M3") }));

			// WITH topologies, or the printed PDF is missing the joint-plane section entirely — the
			// largest change in this round, and the one a reviewer most needs to see. A probe that
			// prints a document the app would not produce measures the wrong thing.
			var topo = new Dictionary<string, JointTopology>();
			foreach (var (name, _) in cons) topo[name] = SampleTopology();

			return NorsokHtmlReportGenerator.GenerateReport(
				"probe.ideaCon", cons, expandAll: true, jointImages: null, topologies: topo);
		}

		/// <summary>
		/// A resolved topology with an OFF-AXIS frame, so the projected forces genuinely differ from
		/// the local ones — an axis-aligned joint would print two identical columns and show nothing.
		/// </summary>
		private static JointTopology SampleTopology() => new()
		{
			Chord = new JointMemberData
			{
				Id = 1, Name = "M1",
				Section = new JointSectionInfo { Name = "CHS273.0/12.5", D = 0.273, T = 0.0125 },
			},
			Ex = new Vec3(0.7071, 0.7071, 0),
			Ey = new Vec3(-0.4082, 0.4082, 0.8165),
			NPlane = new Vec3(0.5774, -0.5774, 0.5774),
			PlaneFitBasis = "least-squares fit over 2 braces",
			PlaneSpread = 0.0032,
			Coplanar = true,
			BracesMeta = new List<BraceMeta>
			{
				new() { Name = "M2", ThetaDeg = 47.3, Beta = 0.279, CoplanarDevDeg = 2.1,
					OopOffsetM = 0.004, Side = 1,
					Section = new JointSectionInfo { Name = "CHS76.1/3.6" } },
				new() { Name = "M3", ThetaDeg = 61.8, Beta = 0.418, CoplanarDevDeg = 0.4,
					OopOffsetM = -0.002, Side = -1,
					Section = new JointSectionInfo { Name = "CHS114.3/5.0" } },
			},
			BraceForces = new List<PerLoadEffect<BraceForceRow>>
			{
				new()
				{
					Id = 12, Name = "LE12",
					Rows = new List<BraceForceRow>
					{
						new() { Name = "M2",
							LocalN = -142_100, LocalVy = 3_200, LocalVz = -1_100,
							LocalMx = 210, LocalMy = 4_700, LocalMz = -980,
							NSd = -142_100, Vip = 2_900, Vop = -1_500,
							Mip = 4_310, Mop = -1_620 },
						new() { Name = "M3",
							LocalN = 88_400, LocalVy = -2_100, LocalVz = 900,
							LocalMx = -140, LocalMy = -3_300, LocalMz = 720,
							NSd = 88_400, Vip = -1_800, Vop = 1_200,
							Mip = -3_050, Mop = 1_140 },
					},
				},
			},
		};

		/// <summary>
		/// Print any HTML and return the PDF's path. EVERY await is bounded.
		///
		/// The first version of this probe had no timeouts and hung on the first run: the test host
		/// stayed alive holding NorsokChecker.dll, which then blocked every build until it was
		/// killed. A probe that can hang is worse than no probe — it takes the toolchain with it.
		/// </summary>
		/// <summary>
		/// Print HTML to a PDF and return its path.
		///
		/// The whole job runs INSIDE a dispatcher frame, posted before the frame is pushed. That
		/// ordering is not stylistic: WebView2 throws
		/// "EnsureCoreWebView2Async cannot be used before the application's event loop has started
		/// running" if it is called first and the loop is started afterwards. Measured — two failed
		/// attempts, the first hanging for 60 s and the second throwing at 708 ms, which is how this
		/// arrangement was arrived at rather than guessed.
		/// </summary>
		private static string Print(string html, string tag)
		{
			string pdf = Path.Combine(Path.GetTempPath(), $"norsok-{tag}-{Guid.NewGuid():N}.pdf");
			var dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
			var frame = new System.Windows.Threading.DispatcherFrame();
			Exception? failure = null;

			async Task Work()
			{
				var view = new WebView2();
				var window = new System.Windows.Window
				{
					Content = view, Width = 900, Height = 700,
					Left = -10000, Top = -10000,   // off-screen: nothing to look at
					ShowActivated = false,
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

					await Task.Delay(1500);   // KaTeX and the web fonts settle

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

					if (!await view.CoreWebView2.PrintToPdfAsync(pdf, print))
						throw new InvalidOperationException("PrintToPdf reported failure");
				}
				finally
				{
					window.Close();
				}
			}

			// Posted, so the loop is already running when EnsureCoreWebView2Async is reached.
			dispatcher.BeginInvoke(new Action(async () =>
			{
				try { await Work(); }
				catch (Exception ex) { failure = ex; }
				finally { frame.Continue = false; }
			}));

			// A deadline, so a stall FAILS instead of hanging: the first version of this probe held
			// NorsokChecker.dll in a live test host and blocked every build until it was killed.
			var timer = new System.Windows.Threading.DispatcherTimer(
				TimeSpan.FromSeconds(180), System.Windows.Threading.DispatcherPriority.Background,
				(_, _) => frame.Continue = false, dispatcher);
			timer.Start();
			System.Windows.Threading.Dispatcher.PushFrame(frame);
			timer.Stop();

			if (failure != null) Assert.Fail($"printing failed: {failure.Message}");
			Assert.That(File.Exists(pdf), Is.True, "the print did not finish within 180 s");
			return pdf;
		}

		/// <summary>
		/// THE PROBE'S OWN CONTROL, and it runs first.
		///
		/// A minimal document whose answer is known by construction: three divs each forced onto its
		/// own page, so the PDF must have exactly 3 pages carrying "ONE", "TWO", "THREE" — and, if
		/// `counter(page)` works on this build, a footer reading "p 1 of 3" and so on.
		///
		/// This is the step that was skipped the first time. Without it, a wrong result on the real
		/// report cannot be told apart from a broken rig: no footer might mean Chromium lacks the
		/// feature, or that the probe never printed the stylesheet at all.
		/// </summary>
		[Test, Order(1)]
		public void ControlDoesChromiumRenderCounterPageOnThisBuild()
		{
			const string html = """
				<!DOCTYPE html><html><head><meta charset='utf-8'/><style>
				@page { size: A4 portrait; margin: 20mm 15mm;
				        @bottom-center { content: "p " counter(page) " of " counter(pages); } }
				.p { break-after: page; font-size: 40pt; }
				</style></head><body>
				<div class='p'>ONE</div><div class='p'>TWO</div><div class='p'>THREE</div>
				</body></html>
				""";

			string pdf = Print(html, "control");

			TestContext.Out.WriteLine($"CONTROL PDF: {pdf}");
			TestContext.Out.WriteLine("Read it with:");
			TestContext.Out.WriteLine($"  python -c \"import pypdf;r=pypdf.PdfReader(r'{pdf}');"
				+ "print(len(r.pages));[print(repr(p.extract_text())) for p in r.pages]\"");

			Assert.That(new FileInfo(pdf).Length, Is.GreaterThan(500), "a PDF came out");
		}

		/// <summary>
		/// The real report, printed the way the app prints it. Read the file the log names.
		/// </summary>
		[Test, Order(2)]
		public void WhatTheExportedPdfActuallyCarries()
		{
			string pdf = Print(Report(), "report");

			var info = new FileInfo(pdf);
			TestContext.Out.WriteLine($"REPORT PDF: {info.Length / 1024} kB at {pdf}");

			Assert.That(info.Length, Is.GreaterThan(20_000), "a real document came out");

			// Left in place ON PURPOSE: the page-number question is answered by reading it.
		}
	}
}
