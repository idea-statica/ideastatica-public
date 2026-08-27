using System.IO;
using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Why does §6.4 produce nothing for CON1 of test_cs, where the python reference returns
	/// verdict OK and five per-brace results?
	///
	/// Measured before writing this: the four Benchmark_MatchesPythonReference cases agree with the
	/// python oracle to 1e-6, so the §6.4 ENGINE is not the difference. CON1 differs from those
	/// benchmarks in one specific way — its section NAMES do not spell out D/T:
	///
	///   chord M2  "PIPE127STD"          nominal size, real D = 141.0 mm
	///   M1        "76.0x3.5"
	///   M3        "PIPE(Imp)3-1/2XS"
	///   M4        "GB-SSP42X2.5"
	///   M5, M6    "CHS30,3"
	///
	/// So this runs the pipeline BOTH WAYS on the same connection: with the section map alone (what
	/// LiveValidationTests does), and again after the IOM facet-ring refinement the app performs
	/// (MainWindow.EnrichSectionsFromIomAsync). If the first refuses and the second succeeds, the
	/// difference is the refinement, not the check.
	///
	/// Explicit: needs a local Connection RestAPI and test_cs.ideaCon.
	/// </summary>
	[TestFixture, Explicit("Requires a local IDEA StatiCa installation and test_cs.ideaCon")]
	[Category("Live")]
	public class Con1DiagnosticTests
	{
		private const string IdeaCon =
			@"C:\Users\OndrejSkorunka\Claude\01_Folders\NORSOK\ideacon\test_cs.ideaCon";

		private ConnectionApiServiceRunner? _runner;

		[OneTimeSetUp]
		public void Setup()
		{
			string setupDir = Environment.GetEnvironmentVariable("IDEASTATICA_SETUP_DIR")
				?? @"C:\Program Files\IDEA StatiCa\StatiCa 26.1";
			_runner = new ConnectionApiServiceRunner(setupDir);
		}

		[OneTimeTearDown]
		public void Teardown() => _runner?.Dispose();

		/// <summary>
		/// The app's own refinement, lifted out of MainWindow so the test exercises the same
		/// TubeFromIom code the app uses rather than a re-implementation of it.
		/// </summary>
		private static void EnrichFromIom(IdeaRS.OpenModel.Connection.ConnectionData? iom,
			List<JointMemberData> members, Action<string> log)
		{
			var beams = TubeFromIom.TubularBeamsByName(iom);
			log($"IOM tubular beams: [{string.Join(", ", beams.Keys)}]");
			foreach (var m in members)
			{
				if (!beams.TryGetValue(m.Name ?? "", out var beam))
				{
					log($"  {m.Name}: NOT among the IOM tubular beams — left as the name gave it");
					continue;
				}
				var (d, t, why) = TubeFromIom.FromBeam(beam);
				if (d is not > 0 || t is not > 0)
				{
					log($"  {m.Name}: D/T not readable ({why})");
					continue;
				}
				log($"  {m.Name}: D={d:F1} T={t:F1} from the model (name gave D={m.Section.D}, T={m.Section.T})");
				m.Section.D = d;
				m.Section.T = t;
				m.Section.IsCHS = true;
			}
		}

		[Test]
		public async Task Con1_WithAndWithoutIomRefinement()
		{
			Assert.That(File.Exists(IdeaCon), $"{IdeaCon} must exist");

			var client = await _runner!.CreateApiClient();
			var project = await client.Project.OpenProjectAsync(IdeaCon);
			var pid = project.ProjectId;

			var conns = await client.Connection.GetConnectionsAsync(pid);
			var con1 = conns.First(c => c.Name == "CON1");

			var crossSections = await client.Material.GetCrossSectionsAsync(pid);
			var sectionMap = JointSectionMap.FromCrossSections(crossSections.Cast<object>());
			var conMembers = await client.Member.GetMembersAsync(pid, con1.Id);
			var loadEffects = await client.LoadEffect.GetLoadEffectsAsync(pid, con1.Id, isPercentage: false);

			TestContext.Out.WriteLine($"CON1 id={con1.Id}, members={conMembers.Count}, "
				+ $"load effects={loadEffects.Count} (active {loadEffects.Count(l => l.Active)})");

			TestContext.Out.WriteLine("\n=== section map (from cross-section NAMES) ===");
			foreach (var m in conMembers)
			{
				var sec = sectionMap.GetValueOrDefault(m.CrossSectionId ?? -1);
				TestContext.Out.WriteLine($"  {m.Name,-4} csId={m.CrossSectionId} "
					+ $"name={sec?.Name ?? "(none)",-22} isCHS={sec?.IsCHS} D={sec?.D} T={sec?.T}");
			}

			// ---------- pass 1: names only, exactly what LiveValidationTests does ----------
			var membersNames = conMembers
				.Select(m => JointMemberData.FromConMember(m,
					sectionMap.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
				.ToList();
			var topoNames = new JointTopologyBuilder().Build(membersNames, loadEffects);

			TestContext.Out.WriteLine("\n=== PASS 1 — section names only ===");
			Report(topoNames);

			// ---------- pass 2: after the IOM facet-ring refinement the app performs ----------
			var iom = await client.Export.ExportIomConnectionDataAsync(pid, con1.Id);

			// TubularBeamsByName filters on three conditions; python finds all six beams here, so
			// print what the C# side actually sees before deciding which condition rejects them.
			TestContext.Out.WriteLine("\n=== raw IOM beams as C# sees them ===");
			TestContext.Out.WriteLine($"  iom is {(iom == null ? "NULL" : "present")}");
			TestContext.Out.WriteLine($"  iom.Beams is {(iom?.Beams == null ? "NULL" : $"{iom.Beams.Count} beam(s)")}");

			// On service 26.1 iom.Beams came back NULL where 26.0 returns six beams. Before blaming
			// the tubular filter, find out whether ANYTHING is populated: an export that returns an
			// empty shell is a different problem from one that moves the members elsewhere.
			if (iom != null)
			{
				foreach (var prop in iom.GetType().GetProperties())
				{
					object? v;
					try { v = prop.GetValue(iom); } catch { continue; }
					string desc = v switch
					{
						null => "null",
						System.Collections.ICollection c => $"{c.Count} item(s)",
						_ => v.ToString() ?? "?",
					};
					if (desc is "null" or "0 item(s)") continue;   // only what IS populated
					TestContext.Out.WriteLine($"    {prop.Name} = {desc}");
				}
			}
			foreach (var b in iom?.Beams ?? new List<IdeaRS.OpenModel.Connection.BeamData>())
			{
				int facets = b.Plates?.Count(p => !p.IsNegativeObject) ?? 0;
				TestContext.Out.WriteLine($"  name={b.Name ?? "(null)",-6} negative={b.IsNegativeObject,-5} "
					+ $"crossSectionType={b.CrossSectionType ?? "(null)",-12} "
					+ $"isTubularTypeName={JointSectionMap.IsTubularTypeName(b.CrossSectionType),-5} "
					+ $"facets={facets}");
			}
			var membersIom = conMembers
				.Select(m => JointMemberData.FromConMember(m,
					sectionMap.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
				.ToList();
			TestContext.Out.WriteLine("\n=== PASS 2 — IOM refinement ===");
			EnrichFromIom(iom, membersIom, s => TestContext.Out.WriteLine(s));
			var topoIom = new JointTopologyBuilder().Build(membersIom, loadEffects);
			Report(topoIom);

			await client.Project.CloseProjectAsync(pid);

			// The python reference returns verdict OK with five per-brace rows on this connection,
			// so pass 2 -- which is what the app actually runs -- must reach a check at all.
			Assert.That(topoIom.Verdict.Status, Is.Not.EqualTo("ERROR"),
				"after the IOM refinement CON1 must not be rejected: python returns OK on it. "
				+ "Errors: " + string.Join(" | ", topoIom.Verdict.Errors));
			Assert.That(topoIom.JointChecks, Is.Not.Empty, "CON1 must produce §6.4 checks");
		}

		private static void Report(JointTopology topo)
		{
			TestContext.Out.WriteLine($"  chord={topo.Chord?.Name}, gapBraces={topo.GapBraces.Count}, "
				+ $"planeFit={topo.PlaneFitBasis}, verdict={topo.Verdict.Status}, "
				+ $"jointChecks={topo.JointChecks.Count}");
			foreach (var e in topo.Verdict.Errors) TestContext.Out.WriteLine($"    [E] {e}");
			foreach (var w in topo.Verdict.Warnings) TestContext.Out.WriteLine($"    [W] {w}");
			foreach (var g in topo.Gaps)
				TestContext.Out.WriteLine($"    gap {g.A}-{g.B}: {g.GapM * 1000:F2} mm");
			var le1 = topo.JointChecks.FirstOrDefault();
			if (le1 != null)
			{
				TestContext.Out.WriteLine($"    LE{le1.Id} '{le1.Name}':");
				foreach (var r in le1.Rows)
					TestContext.Out.WriteLine($"      {r.Name,-4} skipped={r.Skipped,-5} util={r.Util:F4} "
						+ $"N_Rd={r.NRdWeighted / 1e3:F1} kN M_Rd,ip={r.MRdIp / 1e3:F2} "
						+ $"overstressed={r.ChordOverstressed} {r.Reason}");
			}
		}
	}
}
