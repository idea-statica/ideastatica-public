using System.IO;
using IdeaStatiCa.ConnectionApi;
using Newtonsoft.Json.Linq;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// PHASE-4 LIVE VALIDATION — runs the C# auto-topology §6.4 pipeline over the four benchmark
	/// .ideaCon projects through a LOCAL Connection RestAPI and compares every per-LE / per-brace
	/// result against TestData/live_oracle.json, captured from the python REFERENCE pipeline
	/// (reference/python_prototype via TestData/gen_live_oracle.py) on the same files and service.
	///
	/// [Explicit]: requires IDEA StatiCa installed locally. Setup dir override:
	///   IDEASTATICA_SETUP_DIR (default C:\Program Files\IDEA StatiCa\StatiCa 26.1)
	/// Run:  dotnet test --filter FullyQualifiedName~LiveValidationTests
	/// </summary>
	[TestFixture, Explicit("Requires a local IDEA StatiCa installation (Connection RestAPI)")]
	[Category("Live")]
	public class LiveValidationTests
	{
		private const double RelTol = 1e-6;
		private const double AbsTol = 1e-9;

		private static string _scriptsDir = null!;
		private static JObject _oracle = null!;
		private ConnectionApiServiceRunner? _runner;

		private static readonly Dictionary<string, string> Benchmarks = new()
		{
			["K_CONNECTION"] = @"NORSOK CHAPTER 6.4 K AND KT-JOINTS\K_CONNECTION.ideaCon",
			["X_CONNECTION"] = @"NORSOK CHAPTER 6.4 X CONNECTION\X_CONNECTION.ideaCon",
			["TY_CONNECTION"] = @"NORSOK CHAPTER 6.4 T_Y CONNECTION\TY_CONNECTION.ideaCon",
			["TY_CONNECTION_UNIT_TEST"] = @"NORSOK CHAPTER 6.4 T_Y CONNECTION\NORSOK_TY_CONNECTIONS_UNIT_TESTS\TY_CONNECTION_UNIT_TEST.ideaCon",
		};

		[OneTimeSetUp]
		public void Setup()
		{
			// locate the repo folders by walking up from the test output
			var dir = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
			while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "UT_NorsokChecker")))
				dir = dir.Parent;
			Assert.That(dir, Is.Not.Null, "examples root not found above the test directory");
			_scriptsDir = Path.Combine(dir!.FullName, "NorsokChecker", "reference", "verification_scripts");
			_oracle = JObject.Parse(File.ReadAllText(
				Path.Combine(dir.FullName, "UT_NorsokChecker", "TestData", "live_oracle.json")));

			string setupDir = Environment.GetEnvironmentVariable("IDEASTATICA_SETUP_DIR")
				?? @"C:\Program Files\IDEA StatiCa\StatiCa 26.1";
			_runner = new ConnectionApiServiceRunner(setupDir);
		}

		[OneTimeTearDown]
		public void Teardown() => _runner?.Dispose();

		private static void AssertNear(double actual, double expected, string what)
		{
			double tol = Math.Max(AbsTol, Math.Abs(expected) * RelTol);
			Assert.That(actual, Is.EqualTo(expected).Within(tol), what);
		}

		[TestCase("K_CONNECTION")]
		[TestCase("X_CONNECTION")]
		[TestCase("TY_CONNECTION")]
		[TestCase("TY_CONNECTION_UNIT_TEST")]
		public async Task Benchmark_MatchesPythonReference(string benchmark)
		{
			string path = Path.Combine(_scriptsDir, Benchmarks[benchmark]);
			Assert.That(File.Exists(path), $"benchmark file {path}");
			var oBench = (JObject)_oracle["benchmarks"]![benchmark]!;

			var client = await _runner!.CreateApiClient();
			try
			{
				var project = await client.Project.OpenProjectAsync(path);

				var crossSections = await client.Material.GetCrossSectionsAsync(project.ProjectId);
				var sectionMap = JointSectionMap.FromCrossSections(crossSections.Cast<object>());

				foreach (var (conName, oConTok) in (JObject)oBench["connections"]!)
				{
					var oCon = (JObject)oConTok!;
					int conId = (int)oCon["id"]!;
					var conMembers = await client.Member.GetMembersAsync(project.ProjectId, conId);
					// isPercentage: false is mandatory (percentage-stored LEs would silently collapse)
					var loadEffects = await client.LoadEffect.GetLoadEffectsAsync(project.ProjectId, conId, isPercentage: false);

					var members = conMembers
						.Select(m => JointMemberData.FromConMember(m,
							sectionMap.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
						.ToList();
					var topo = new JointTopologyBuilder().Build(members, loadEffects);

					Assert.Multiple(() =>
					{
						string ctx = $"{benchmark}/{conName}";
						Assert.That(topo.Chord?.Name, Is.EqualTo((string?)oCon["chord"]), $"{ctx} chord");
						Assert.That(topo.Verdict.Status, Is.EqualTo((string?)oCon["verdict"]), $"{ctx} verdict");

						var oGaps = (JArray)oCon["gaps"]!;
						Assert.That(topo.Gaps.Count, Is.EqualTo(oGaps.Count), $"{ctx} gap count");
						foreach (var og in oGaps)
						{
							var g = topo.Gaps.FirstOrDefault(x =>
								(x.A == (string?)og["a"] && x.B == (string?)og["b"]) ||
								(x.A == (string?)og["b"] && x.B == (string?)og["a"]));
							Assert.That(g, Is.Not.Null, $"{ctx} gap {og["a"]}-{og["b"]}");
							AssertNear(g!.GapM, (double)og["gap_m"]!, $"{ctx} gap {og["a"]}-{og["b"]}");
						}

						foreach (var (leId, oBraces) in (JObject)oCon["joint_checks"]!)
						{
							var le = topo.JointChecks.FirstOrDefault(x => x.Id.ToString() == leId);
							Assert.That(le, Is.Not.Null, $"{ctx} LE{leId} present");
							foreach (var (bName, oRowTok) in (JObject)oBraces!)
							{
								var oRow = (JObject)oRowTok!;
								var row = le!.Rows.FirstOrDefault(r => r.Name == bName);
								Assert.That(row, Is.Not.Null, $"{ctx} LE{leId}/{bName} present");
								string rctx = $"{ctx} LE{leId}/{bName}";

								bool oSkipped = (bool)oRow["skipped"]!;
								Assert.That(row!.Skipped, Is.EqualTo(oSkipped), $"{rctx} skipped");
								if (oSkipped) return;

								AssertNear(row.Util, (double)oRow["util"]!, $"{rctx} util");
								Assert.That(row.Passed, Is.EqualTo((bool)oRow["passed"]!), $"{rctx} passed");
								AssertNear(row.NRdWeighted, (double)oRow["N_Rd_weighted"]!, $"{rctx} N_Rd");
								AssertNear(row.MRdIp, (double)oRow["M_Rd_ip"]!, $"{rctx} M_Rd_ip");
								AssertNear(row.MRdOp, (double)oRow["M_Rd_op"]!, $"{rctx} M_Rd_op");
								Assert.That(row.WithinRange, Is.EqualTo((bool)oRow["within_range"]!), $"{rctx} within_range");
								Assert.That(row.ChordOverstressed, Is.EqualTo((bool)oRow["chord_overstressed"]!), $"{rctx} overstressed");

								var inp = row.Inputs!;
								var cl = row.Classification!;
								AssertNear(cl.FrK, (double)oRow["frK"]!, $"{rctx} frK");
								AssertNear(cl.FrY, (double)oRow["frY"]!, $"{rctx} frY");
								AssertNear(cl.FrX, (double)oRow["frX"]!, $"{rctx} frX");
								AssertNear(inp.NSd, (double)oRow["N_Sd"]!, $"{rctx} N_Sd");
								AssertNear(inp.MipSd, (double)oRow["M_ip_Sd"]!, $"{rctx} M_ip_Sd");
								AssertNear(inp.MopSd, (double)oRow["M_op_Sd"]!, $"{rctx} M_op_Sd");
								AssertNear(inp.SigmaASd, (double)oRow["sigma_a"]!, $"{rctx} sigma_a");
								AssertNear(inp.SigmaMySd, (double)oRow["sigma_my"]!, $"{rctx} sigma_my");
								AssertNear(inp.SigmaMzSd, (double)oRow["sigma_mz"]!, $"{rctx} sigma_mz");
								AssertNear(inp.ThetaDeg, (double)oRow["theta_deg"]!, $"{rctx} theta");
							}
						}
					});
				}

				await client.Project.CloseProjectAsync(project.ProjectId);
			}
			finally
			{
				(client as IDisposable)?.Dispose();
			}
		}
	}
}
