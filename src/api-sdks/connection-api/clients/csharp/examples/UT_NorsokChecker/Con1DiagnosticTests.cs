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

		/// <summary>
		/// Everything the §6.4 tab's four panels draw, for one load effect — so the tab can be
		/// checked against the python sheet without launching the app. Each block is one panel.
		/// </summary>
		[Test]
		public async Task Con1_SheetContentForLe1()
		{
			Assert.That(File.Exists(IdeaCon), $"{IdeaCon} must exist");

			var client = await _runner!.CreateApiClient();
			var project = await client.Project.OpenProjectAsync(IdeaCon);
			var pid = project.ProjectId;
			try
			{
				var conns = await client.Connection.GetConnectionsAsync(pid);
				var con1 = conns.First(c => c.Name == "CON1");
				var crossSections = await client.Material.GetCrossSectionsAsync(pid);
				var sectionMap = JointSectionMap.FromCrossSections(crossSections.Cast<object>());
				var conMembers = await client.Member.GetMembersAsync(pid, con1.Id);
				var loadEffects = await client.LoadEffect.GetLoadEffectsAsync(pid, con1.Id, isPercentage: false);

				var members = conMembers
					.Select(m => JointMemberData.FromConMember(m,
						sectionMap.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
					.ToList();
				EnrichFromIom(await client.Export.ExportIomConnectionDataAsync(pid, con1.Id), members,
					_ => { });
				var topo = new JointTopologyBuilder().Build(members, loadEffects);

				Assert.That(topo.Verdict.Status, Is.Not.EqualTo("ERROR"),
					"CON1 must be checkable: " + string.Join(" | ", topo.Verdict.Errors));

				var le1 = topo.JointChecks.First();
				TestContext.Out.WriteLine($"=== load effect {le1.Name} (id {le1.Id}) ===");

				// PANEL: node equilibrium
				var eq = topo.Equilibrium.FirstOrDefault(r => r.Id == le1.Id);
				TestContext.Out.WriteLine("\n-- Node equilibrium (self-check) --");
				Assert.That(eq, Is.Not.Null, "the tab's equilibrium panel needs a row for this LE");
				TestContext.Out.WriteLine($"  ΣF  [kN] : {eq!.SumF.X / 1e3,8:F1} {eq.SumF.Y / 1e3,8:F1} {eq.SumF.Z / 1e3,8:F1}");
				TestContext.Out.WriteLine($"  ΣM [kNm] : {eq.SumM.X / 1e3,8:F2} {eq.SumM.Y / 1e3,8:F2} {eq.SumM.Z / 1e3,8:F2}");
				TestContext.Out.WriteLine($"  state    : {(eq.Ok ? "OK" : "OUT OF BALANCE")}");

				// PANEL: brace forces in the joint plane, with the chord's own forces
				var bf = topo.BraceForces.First(r => r.Id == le1.Id);
				var cs = topo.ChordStresses.First(r => r.Id == le1.Id);
				var csByName = cs.Rows.GroupBy(r => r.Name).ToDictionary(g => g.Key, g => g.First());
				TestContext.Out.WriteLine("\n-- Brace forces in joint plane --");
				TestContext.Out.WriteLine("  brace   N_Sd    M_ip    M_op    V_ip    V_op   M_tor  face      N_chord  M_ip,ch");
				foreach (var r in bf.Rows)
				{
					var c = csByName.GetValueOrDefault(r.Name);
					TestContext.Out.WriteLine(
						$"  {r.Name,-6}{r.NSd / 1e3,7:F1}{r.Mip / 1e3,8:F2}{r.Mop / 1e3,8:F2}"
						+ $"{r.Vip / 1e3,8:F1}{r.Vop / 1e3,8:F1}{r.Mtor / 1e3,8:F2}"
						+ $"  {(r.Side >= 0 ? "+ey" : "−ey"),-8}"
						+ $"{(c == null ? "—" : (c.NChord / 1e3).ToString("F1")),9}"
						+ $"{(c == null ? "—" : (c.MipChord / 1e3).ToString("F2")),9}");
				}

				// PANEL: classification + member checks, with the K sub-rows
				TestContext.Out.WriteLine("\n-- Classification & member checks --");
				foreach (var row in le1.Rows)
				{
					var cls = row.Classification;
					if (row.Skipped)
					{
						TestContext.Out.WriteLine($"  {row.Name,-6} SKIPPED — {row.Reason}");
						continue;
					}
					TestContext.Out.WriteLine(
						$"  {row.Name,-6} K={cls?.FrK ?? 0,6:P0} X={cls?.FrX ?? 0,6:P0} Y={cls?.FrY ?? 0,6:P0}"
						+ $"  N_Rd={row.NRdWeighted / 1e3,7:F1} kN  util={row.Util,7:P1}"
						+ $"  {(row.Passed ? "PASS" : "FAIL")}"
						+ (string.IsNullOrEmpty(cls?.Note) ? "" : $"   [{cls!.Note}]"));
					foreach (var kc in cls?.KComponents ?? new List<KComponent>())
						TestContext.Out.WriteLine(
							$"         ↳ K via {kc.Partner,-4} {kc.Frac,6:P1} — {kc.Frac * Math.Abs(cls!.NSd) / 1e3:F1} kN"
							+ $" across a {(kc.GapM is { } g ? $"{g * 1000:F0} mm" : "unknown")} gap");
				}

				// PANEL: the joint view's colours
				TestContext.Out.WriteLine("\n-- Joint view colouring --");
				TestContext.Out.WriteLine($"  chord (slate) : {topo.Chord?.Name} (id {topo.Chord?.Id})");
				foreach (var brace in topo.GapBraces)
				{
					var row = le1.Rows.FirstOrDefault(r => r.Name == brace.Name);
					string colour = (row == null || row.Skipped || double.IsNaN(row.Util)) ? "grey (no check)"
						: row.Util >= 1.0 ? "red"
						: row.Util >= 0.85 ? "amber"
						: row.Util >= 0.5 ? "yellow-green" : "green";
					TestContext.Out.WriteLine($"  {brace.Name,-6}(id {brace.Id,2}) : {colour}");
				}

				// The sheet must actually be populated — an empty panel is the defect being fixed.
				Assert.Multiple(() =>
				{
					Assert.That(topo.Equilibrium, Is.Not.Empty, "equilibrium panel");
					Assert.That(bf.Rows, Is.Not.Empty, "brace-force panel");
					Assert.That(cs.Rows, Is.Not.Empty, "chord forces");
					Assert.That(le1.Rows.Any(r => !r.Skipped), "at least one brace checked");
					Assert.That(le1.Rows.Any(r => r.Classification?.KComponents.Count > 0),
						"at least one K sub-row — CON1 has K pairings across its gaps");
				});
			}
			finally
			{
				await client.Project.CloseProjectAsync(pid);
			}
		}

		/// <summary>
		/// Every connection in test_cs: does the presentation payload yield member bodies, and is
		/// the joint checkable?
		///
		/// Reported from the app (2026-08-27): switching from CON1 to CON8 on the §6.4 tab drew
		/// "0 members" while the tables beside it still held CON1's numbers. The cause was the tab
		/// reading a mesh cache that only the Check tab fills — but "the payload is empty for CON8"
		/// was an equally possible explanation, and the two call for opposite fixes. This measures
		/// which it is, per connection, so the answer is not assumed. (Measured: CON8 yields six
		/// bodies and verdict OK, so it was the cache.)
		///
		/// CON10 is exempt and named here rather than passing by luck: it is the deliberate
		/// "no brace (chord only)" gate, so ONE body — the chord — is the correct answer, and its
		/// inherited load effects reference the deleted braces, which is why the service answers 404
		/// for them. Both are documented in the GATE COVERAGE README; the gate fires before any load
		/// is read, so the missing load effect has no consequence.
		/// </summary>
		[Test]
		public async Task EveryConnectionYieldsMemberBodies()
		{
			Assert.That(File.Exists(IdeaCon), $"{IdeaCon} must exist");

			var client = await _runner!.CreateApiClient();
			var project = await client.Project.OpenProjectAsync(IdeaCon);
			var pid = project.ProjectId;
			try
			{
				var conns = await client.Connection.GetConnectionsAsync(pid);
				var crossSections = await client.Material.GetCrossSectionsAsync(pid);
				var sectionMap = JointSectionMap.FromCrossSections(crossSections.Cast<object>());

				var empty = new List<string>();
				TestContext.Out.WriteLine($"{"connection",-12}{"bodies",8}  verdict");
				foreach (var con in conns)
				{
					int bodies;
					try
					{
						string json = await client.Presentation.GetDataScene3DTextAsync(pid, con.Id);
						bodies = NorsokChecker.Services.JointPresentationReader
							.ReadMembers(json, _ => { }).Count;
					}
					catch (Exception ex)
					{
						bodies = -1;
						TestContext.Out.WriteLine($"  {con.Name}: presentation failed — {ex.Message}");
					}

					string verdict;
					try
					{
						var conMembers = await client.Member.GetMembersAsync(pid, con.Id);
						var les = await client.LoadEffect.GetLoadEffectsAsync(pid, con.Id, isPercentage: false);
						var members = conMembers
							.Select(m => JointMemberData.FromConMember(m,
								sectionMap.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
							.ToList();
						EnrichFromIom(await client.Export.ExportIomConnectionDataAsync(pid, con.Id),
							members, _ => { });
						var topo = new JointTopologyBuilder().Build(members, les);
						verdict = topo.Verdict.Status
							+ (topo.Verdict.Errors.Count > 0
								? $" ({topo.Verdict.Errors.Count}: {topo.Verdict.Errors[0]})" : "");
					}
					catch (Exception ex)
					{
						verdict = $"threw — {ex.GetType().Name}";
					}

					TestContext.Out.WriteLine($"{con.Name,-12}{bodies,8}  {verdict}");

					// CON10 is the chord-only gate: one body IS its correct answer, and its load
					// effects legitimately 404. Asserting ">0 bodies" on it would pass by luck today
					// and fail the day the gate is made stricter.
					if (con.Name == "CON10")
					{
						Assert.That(bodies, Is.EqualTo(1),
							"CON10 is the 'no brace (chord only)' gate — the chord alone");
						continue;
					}

					if (bodies <= 0) empty.Add($"{con.Name} ({bodies} bodies)");
				}

				// If the payload IS populated for every connection, then "0 members" in the app was
				// never about the data — it was the cache, and fetching on demand is the right fix.
				Assert.That(empty, Is.Empty,
					"these connections yield no drawable bodies, so an empty §6.4 view for them is "
					+ "the model's doing rather than the cache's: " + string.Join(", ", empty));
			}
			finally
			{
				await client.Project.CloseProjectAsync(pid);
			}
		}

		/// <summary>
		/// Where do the member labels actually land, on the REAL geometry of a real connection?
		///
		/// Reported on CON8: one of six labels visible. A synthetic six-member joint does not
		/// reproduce it — its labels all land inside the view — so the cause is something real
		/// meshes have and the fixture does not (hundreds of vertices, genuine 3D extent, a chord
		/// that dwarfs the braces). This prints the projected position of every label against the
		/// view bounds, which is the property visibility depends on.
		/// </summary>
		[Test, Apartment(System.Threading.ApartmentState.STA)]
		public async Task Con8_WhereDoTheMemberLabelsLand()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}

			var client = await _runner!.CreateApiClient();
			var project = await client.Project.OpenProjectAsync(IdeaCon);
			var pid = project.ProjectId;
			try
			{
				var conns = await client.Connection.GetConnectionsAsync(pid);
				foreach (string conName in new[] { "CON1", "CON8" })
				{
					var con = conns.First(c => c.Name == conName);
					string json = await client.Presentation.GetDataScene3DTextAsync(pid, con.Id);
					var meshes = NorsokChecker.Services.JointPresentationReader.ReadMembers(json, _ => { });

					// the §6.4 view's real size, from the XAML column width
					var view = new NorsokChecker.Controls.Joint3DView { Width = 380, Height = 300 };
					view.Measure(new System.Windows.Size(380, 300));
					view.Arrange(new System.Windows.Rect(0, 0, 380, 300));
					view.UpdateLayout();
					view.ShowMemberLabels = true;
					view.Load(meshes);

					// the joint plane, as the tab sets it
					var crossSections = await client.Material.GetCrossSectionsAsync(pid);
					var sectionMap = JointSectionMap.FromCrossSections(crossSections.Cast<object>());
					var conMembers = await client.Member.GetMembersAsync(pid, con.Id);
					var les = await client.LoadEffect.GetLoadEffectsAsync(pid, con.Id, isPercentage: false);
					var members = conMembers
						.Select(m => JointMemberData.FromConMember(m,
							sectionMap.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
						.ToList();
					EnrichFromIom(await client.Export.ExportIomConnectionDataAsync(pid, con.Id),
						members, _ => { });
					var topo = new JointTopologyBuilder().Build(members, les);
					if (topo.NPlane.Norm > 1e-9)
						view.LookAtPlane(
							new System.Windows.Media.Media3D.Vector3D(topo.NPlane.X, topo.NPlane.Y, topo.NPlane.Z),
							new System.Windows.Media.Media3D.Vector3D(topo.Ex.X, topo.Ex.Y, topo.Ex.Z));

					var layer = (System.Windows.Controls.Canvas)view.FindName("LabelLayer")!;
					TestContext.Out.WriteLine($"\n=== {conName}: {meshes.Count} member(s), "
						+ $"{layer.Children.Count} label(s) in a 380x300 view ===");
					int inside = 0;
					foreach (var t in layer.Children.OfType<System.Windows.Controls.TextBlock>())
					{
						double x = System.Windows.Controls.Canvas.GetLeft(t);
						double y = System.Windows.Controls.Canvas.GetTop(t);
						t.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
						bool ok = !double.IsNaN(x) && !double.IsNaN(y) && x >= 0 && y >= 0
							&& x + t.DesiredSize.Width <= 380 && y + t.DesiredSize.Height <= 300;
						if (ok) inside++;
						TestContext.Out.WriteLine($"   {t.Text,-5} at ({x,7:F1},{y,7:F1})  "
							+ (ok ? "inside" : "OUTSIDE the view"));
					}
					TestContext.Out.WriteLine($"   {inside} of {layer.Children.Count} inside");
				}
			}
			finally
			{
				await client.Project.CloseProjectAsync(pid);
			}
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
