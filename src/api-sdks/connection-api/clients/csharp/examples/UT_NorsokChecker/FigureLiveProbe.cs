using System.IO;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Controls;
using NorsokChecker.Services;
using NorsokChecker.Services.Chapters;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The figure path against the LIVE service, step by step, with the real payload.
	///
	/// This is the measurement that should have come first. Every earlier probe ran without a service,
	/// so MeshesForAsync returned empty and the render never happened — 12 ms of nothing, which I then
	/// read as "the generator is fine". It was, but it was not what the app does.
	///
	/// The user has now ruled out the wait theory too: over a minute on v34 with nothing appearing,
	/// where the render alone measures at most ~13 s for fifteen joints. So something on this path
	/// does not merely cost time — it does not finish. This prints where.
	///
	/// Probe: needs a live 26.0 service and the .ideaCon. STA, because RenderToPng needs WPF.
	/// </summary>
	[TestFixture, Category("Probe"), Apartment(System.Threading.ApartmentState.STA)]
	public class FigureLiveProbe
	{
		private const string IdeaCon =
			@"C:\Users\OndrejSkorunka\Claude\01_Folders\NORSOK\ideacon\test_cs.ideaCon";

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

		/// <summary>
		/// Fetch the bodies and render a figure for EVERY connection, timing each step separately.
		///
		/// Reports rather than asserts. What it separates: the fetch (HTTP, ~1.7 MB a time), the
		/// presentation parse, and the render (measure/arrange/RenderTargetBitmap/encode). A step that
		/// never returns shows up as a missing line — the last line printed is where it stopped.
		/// </summary>
		[Test]
		public async Task WhereDoesTheFigurePathActuallySpendItsTime()
		{
			void Log(string _) { }

			var client = await _runner!.CreateApiClient();
			var project = await client.Project.OpenProjectAsync(IdeaCon);
			await new ProjectSettingsService(client, Log).ApplyNorsokFactorsAsync(project.ProjectId);

			var crossSections = await client.Material.GetCrossSectionsAsync(project.ProjectId);
			var sectionMap = JointSectionMap.FromCrossSections(crossSections.Cast<object>());

			var connections = project.Connections ?? new();
			TestContext.Out.WriteLine($"{connections.Count} connections in the project\n");

			var total = System.Diagnostics.Stopwatch.StartNew();

			foreach (var con in connections)
			{
				var sw = System.Diagnostics.Stopwatch.StartNew();

				// 1. the chapter, to get the topology the figure needs
				var chapter = new Chapter64();
				JointTopology? topo = null;
				chapter.Topology = (_, t) => topo = t;

				// Guarded exactly as the run guards it: CON10 answers 404 here ("The given key '1' was
				// not present in the dictionary"), which is a service-side fault on that connection.
				// The app survives it; an unguarded probe does not, and would report the whole path
				// broken on the strength of one bad joint.
				var loadEffects = new List<ConLoadEffect>();
				string? leError = null;
				try
				{
					var read = await client.LoadEffect.GetLoadEffectsAsync(
						project.ProjectId, con.Id, isPercentage: false);
					if (read != null) loadEffects.AddRange(read);
				}
				catch (Exception ex)
				{
					leError = ex.Message.Split('\n')[0];
				}

				if (leError != null)
				{
					TestContext.Out.WriteLine($"{con.Name,-8} LOAD EFFECTS FAILED — {leError[..Math.Min(90, leError.Length)]}");
					continue;
				}

				await chapter.EvaluateAsync(new ChapterContext
				{
					Client = client, ProjectId = project.ProjectId,
					ConnectionId = con.Id, ConnectionName = con.Name,
					LoadEffects = loadEffects, SectionMap = sectionMap, Log = Log,
				}, CancellationToken.None);
				long tChapter = sw.ElapsedMilliseconds;

				if (topo == null)
				{
					TestContext.Out.WriteLine($"{con.Name,-8} chapter {tChapter,5} ms — no topology, no figure");
					continue;
				}

				// 2. the fetch — the step that was empty in every earlier probe
				sw.Restart();
				string json = await client.Presentation.GetDataScene3DTextAsync(project.ProjectId, con.Id);
				long tFetch = sw.ElapsedMilliseconds;

				sw.Restart();
				var meshes = JointPresentationReader.ReadMembers(json, Log, null);
				long tParse = sw.ElapsedMilliseconds;

				// 3. the render
				sw.Restart();
				var view = new Joint3DView
				{
					Interactive = false, ShowMemberLabels = true, ChromeVisible = false,
				};
				view.Load(meshes);
				long tLoad = sw.ElapsedMilliseconds;

				sw.Restart();
				byte[]? png = view.RenderToPng();
				long tRender = sw.ElapsedMilliseconds;

				TestContext.Out.WriteLine(
					$"{con.Name,-8} chapter {tChapter,5} | fetch {tFetch,5} ({json.Length / 1024,5} kB) "
					+ $"| parse {tParse,4} ({meshes.Count,2} bodies) | load {tLoad,4} | render {tRender,5} ms "
					+ $"-> {(png?.Length ?? 0) / 1024,3} kB");
			}

			TestContext.Out.WriteLine($"\nTOTAL {total.Elapsed.TotalSeconds:F1} s for {connections.Count} connections");
		}
	}
}
