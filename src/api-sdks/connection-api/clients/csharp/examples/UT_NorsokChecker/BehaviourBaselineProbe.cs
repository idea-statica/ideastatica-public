using System.IO;
using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Chapters;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Records what every connection in test_cs is assessed as, so a restructure can be checked
	/// against it. The restructure's whole claim is that nothing observable changes, and unit tests
	/// cannot establish that on their own — they move with the code they test.
	///
	/// Writes `behaviour-baseline.txt` beside the test assembly. Run before the work, keep the file,
	/// run again after each step and diff it. A difference is a defect in the move.
	///
	/// It calls the app's OWN services — <see cref="NorsokCheckRunner"/> and
	/// <see cref="CheckWorkflow"/> — rather than re-implementing them. An earlier attempt copied the
	/// roll-up and the IOM enrichment into the rig and was thrown away: a baseline that restates the
	/// app measures the rig, and a mistake in the copy becomes a fake "regression" later.
	///
	/// Probe category: needs a live service and the .ideaCon.
	///     dotnet test --filter "FullyQualifiedName~BehaviourBaselineProbe"
	/// </summary>
	[TestFixture, Category("Probe")]
	public class BehaviourBaselineProbe
	{
		private const string IdeaCon =
			@"C:\Users\OndrejSkorunka\Claude\01_Folders\NORSOK\ideacon\test_cs.ideaCon";

		/// <summary>
		/// 26.0 — the version the app pins (ServiceLocator.MaxVersion), not the newest installed.
		/// Measuring on a different build than the app runs is how a false finding gets made: the
		/// 26.0.4 client deserialises 26.1's IOM export to null, which looks like a broken service
		/// and silently costs every joint its D/T.
		/// </summary>
		private static string SetupDir =>
			Environment.GetEnvironmentVariable("IDEASTATICA_SETUP_DIR")
			?? @"C:\Program Files\IDEA StatiCa\StatiCa 26.0";

		private ConnectionApiServiceRunner? _runner;

		[OneTimeSetUp]
		public void Setup()
		{
			if (!File.Exists(IdeaCon)) Assert.Ignore($"no test project at {IdeaCon}");
			if (!Directory.Exists(SetupDir)) Assert.Ignore($"no IDEA StatiCa install at {SetupDir}");
			_runner = new ConnectionApiServiceRunner(SetupDir);
		}

		[OneTimeTearDown]
		public void Teardown() => _runner?.Dispose();

		[Test]
		public async Task RecordEveryConnectionsVerdict()
		{
			void Log(string _) { }        // the services want a logger; the baseline wants the numbers

			var client = await _runner!.CreateApiClient();
			var project = await client.Project.OpenProjectAsync(IdeaCon);
			await new ProjectSettingsService(client, Log).ApplyNorsokFactorsAsync(project.ProjectId);

			var crossSections = await client.Material.GetCrossSectionsAsync(project.ProjectId);
			var sectionMap = JointSectionMap.FromCrossSections(crossSections.Cast<object>());

			var rows = new List<string>
			{
				$"# behaviour baseline — {DateTime.Now:yyyy-MM-dd HH:mm}",
				$"# service:  {SetupDir}",
				$"# project:  {IdeaCon}",
				"",
				$"{"connection",-8} {"verdict",-8} {"max util",9}  status",
				new string('-', 70),
			};

			foreach (var con in project.Connections ?? new())
			{
				var v = await AssessAsync(client, project.ProjectId, con, sectionMap, Log);
				rows.Add($"{con.Name,-8} {v.Pass,-8} {v.MaxUtilisation * 100,8:F1}%  {v.Status}");
			}

			string outPath = Path.Combine(
				Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!,
				"behaviour-baseline.txt");
			File.WriteAllLines(outPath, rows);

			foreach (var r in rows) TestContext.Out.WriteLine(r);
			TestContext.Out.WriteLine($"\nwritten to {outPath}");

			await client.Project.CloseProjectAsync(project.ProjectId);
			Assert.Pass("baseline recorded");
		}

		/// <summary>
		/// One connection through the chapter and the verdict roll-up, exactly as the app's run does
		/// it: the same ChapterContext, the same IChapter.EvaluateAsync, the same CheckWorkflow.
		///
		/// It restates nothing. An earlier version prepared the topology and the IOM override itself,
		/// which made the baseline a measurement of the rig — and would have gone on passing while
		/// the chapter it was supposed to be watching changed underneath it.
		/// </summary>
		private static async Task<ConnectionVerdict> AssessAsync(
			IConnectionApiClient client, Guid projectId,
			IdeaStatiCa.Api.Connection.Model.ConConnection con,
			Dictionary<int, JointSectionInfo> sectionMap, Action<string> log)
		{
			var results = new List<NorsokFormulaResult>();
			try
			{
				var loadEffects = await client.LoadEffect.GetLoadEffectsAsync(
					projectId, con.Id, isPercentage: false);

				var ctx = new ChapterContext
				{
					Client = client,
					ProjectId = projectId,
					ConnectionId = con.Id,
					ConnectionName = con.Name ?? $"Con {con.Id}",
					LoadEffects = loadEffects,
					SectionMap = sectionMap,
					Log = log,
				};

				foreach (var chapter in ChapterRegistry.All)
				{
					var outcome = await chapter.EvaluateAsync(ctx, CancellationToken.None);
					results.AddRange(outcome.Rows);
				}
			}
			catch (Exception ex)
			{
				// First line only, and no ids: the service puts a fresh project GUID and traceId in
				// every message, so keeping the whole thing would make each run differ from the last
				// and the diff would report a change on every step.
				string msg = ex.Message.Split('\n')[0];
				if (msg.Length > 90) msg = msg[..90] + "…";
				return new ConnectionVerdict("ERROR", 0, msg);
			}

			return CheckWorkflow.Roll(results);
		}
	}
}
