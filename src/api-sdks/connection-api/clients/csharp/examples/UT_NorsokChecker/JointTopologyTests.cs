using System.IO;
using IdeaStatiCa.Api.Connection.Model;
using Newtonsoft.Json.Linq;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Pins the C# topology pipeline (JointTopologyBuilder + KyxClassifier + JointForceResolver +
	/// JointCheckOrchestrator) to the REFERENCE python implementation (extract.py).
	///
	/// Both sides consume the SAME fixtures (TestData/topology_fixtures.json); the expected values
	/// (TestData/topology_oracle.json) were produced by running the python reference over them
	/// (TestData/gen_topology_oracle.py). If these fail, the C# port has diverged — fix the C#,
	/// not the numbers. Note the built-in cross-links: X_TEST reproduces the Lukáš X-joint script
	/// (util 1.3130 FAIL) and TY_TEST the PURE_TENSION script (util 0.5459 PASS) through the full
	/// auto-classification pipeline.
	/// </summary>
	[TestFixture]
	public class JointTopologyTests
	{
		private const double RelTol = 1e-6;     // both sides run identical IEEE754 math
		private const double AbsTol = 1e-9;

		private static JObject _fixtures = null!;
		private static JObject _oracle = null!;
		private static Dictionary<int, JointSectionInfo> _sections = null!;

		[OneTimeSetUp]
		public void LoadData()
		{
			string dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
			_fixtures = JObject.Parse(File.ReadAllText(Path.Combine(dir, "topology_fixtures.json")));
			_oracle = JObject.Parse(File.ReadAllText(Path.Combine(dir, "topology_oracle.json")));

			// section map — same recipe as extract.py xs_map (name-parsed D/T, inline material fy)
			_sections = new Dictionary<int, JointSectionInfo>();
			foreach (var cs in (JArray)_fixtures["crossSections"]!)
			{
				string? name = (string?)cs["name"];
				var (d, t) = JointSectionInfo.ParseChs(name);
				_sections[(int)cs["id"]!] = new JointSectionInfo
				{
					Name = name, D = d, T = t, IsCHS = d != null,
					Fy = (double?)cs["material"]?["element"]?["fy"],
					Fu = (double?)cs["material"]?["element"]?["fu"],
					MaterialName = (string?)cs["material"]?["element"]?["name"],
				};
			}
		}

		private static void AssertNear(double actual, double expected, string what)
		{
			double tol = Math.Max(AbsTol, Math.Abs(expected) * RelTol);
			Assert.That(actual, Is.EqualTo(expected).Within(tol), what);
		}

		private static JointTopology BuildFixture(string name)
		{
			var fx = ((JArray)_fixtures["fixtures"]!).First(f => (string?)f["name"] == name);
			var members = fx["members"]!
				.Select(j => j.ToObject<ConMember>()!)
				.Select(m => JointMemberData.FromConMember(m,
					_sections.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
				.ToList();
			var loadEffects = fx["loadEffects"]!.Select(j => j.ToObject<ConLoadEffect>()!).ToList();
			return new JointTopologyBuilder().Build(members, loadEffects);
		}

		[TestCase("K_TEST")]
		[TestCase("X_TEST")]
		[TestCase("TY_TEST")]
		[TestCase("KT_TEST")]
		public void Topology_MatchesPythonReference(string fixtureName)
		{
			var topo = BuildFixture(fixtureName);
			var o = (JObject)_oracle[fixtureName]!;

			Assert.Multiple(() =>
			{
				// chord + verdict
				Assert.That(topo.Chord?.Name, Is.EqualTo((string?)o["chord"]), "chord");
				Assert.That(topo.Verdict.Status, Is.EqualTo((string?)o["verdict_status"]), "verdict");

				// per-brace geometry
				foreach (var (bName, bo) in (JObject)o["braces"]!)
				{
					var bm = topo.BracesMeta.FirstOrDefault(x => x.Name == bName);
					Assert.That(bm, Is.Not.Null, $"brace {bName} present");
					AssertNear(bm!.ThetaDeg, (double)bo!["theta_deg"]!, $"{bName} theta");
					if (bo["beta"]!.Type != JTokenType.Null)
						AssertNear(bm.Beta ?? double.NaN, (double)bo["beta"]!, $"{bName} beta");
					AssertNear(bm.CoplanarDevDeg, (double)bo["coplanar_dev_deg"]!, $"{bName} coplanar dev");
				}

				// gaps (matched by unordered pair)
				var oGaps = ((JArray)o["gaps"]!).ToList();
				Assert.That(topo.Gaps.Count, Is.EqualTo(oGaps.Count), "gap count");
				foreach (var og in oGaps)
				{
					string a = (string)og["a"]!, b = (string)og["b"]!;
					var g = topo.Gaps.FirstOrDefault(x =>
						(x.A == a && x.B == b) || (x.A == b && x.B == a));
					Assert.That(g, Is.Not.Null, $"gap {a}-{b} present");
					AssertNear(g!.GapM, (double)og["gap_m"]!, $"gap {a}-{b}");
					Assert.That(g.Side > 0 ? "+" : "-", Is.EqualTo((string?)og["side"]), $"gap {a}-{b} side");
					Assert.That(g.Adjacent, Is.EqualTo((bool)og["adjacent"]!), $"gap {a}-{b} adjacent");
				}

				// STEP 2 brace forces
				foreach (var (leName, braces) in (JObject)o["brace_forces"]!)
				{
					var le = topo.BraceForces.First(x => x.Name == leName);
					foreach (var (bName, bf) in (JObject)braces!)
					{
						var row = le.Rows.First(r => r.Name == bName);
						AssertNear(row.NSd, (double)bf!["N_Sd"]!, $"{leName}/{bName} N_Sd");
						AssertNear(row.Mip, (double)bf["M_ip"]!, $"{leName}/{bName} M_ip");
						AssertNear(row.Mop, (double)bf["M_op"]!, $"{leName}/{bName} M_op");
						AssertNear(row.Vip, (double)bf["V_ip"]!, $"{leName}/{bName} V_ip");
						AssertNear(row.Vop, (double)bf["V_op"]!, $"{leName}/{bName} V_op");
						Assert.That(row.Side, Is.EqualTo((int)bf["side"]!), $"{leName}/{bName} side");
					}
				}

				// STEP 4-prep chord stresses
				foreach (var (leName, braces) in (JObject)o["chord_stresses"]!)
				{
					var le = topo.ChordStresses.First(x => x.Name == leName);
					foreach (var (bName, st) in (JObject)braces!)
					{
						var row = le.Rows.First(r => r.Name == bName);
						AssertNear(row.SigmaA, (double)st!["sigma_a"]!, $"{leName}/{bName} sigma_a");
						AssertNear(row.SigmaMy, (double)st["sigma_my"]!, $"{leName}/{bName} sigma_my");
						AssertNear(row.SigmaMz, (double)st["sigma_mz"]!, $"{leName}/{bName} sigma_mz");
					}
				}

				// STEP 3 classification
				foreach (var (leName, classes) in (JObject)o["classification"]!)
				{
					var le = topo.Classification.First(x => x.Name == leName);
					foreach (var (bName, cl) in (JObject)classes!)
					{
						var row = le.Rows.First(r => r.Name == bName);
						AssertNear(row.FrK, (double)cl!["frK"]!, $"{leName}/{bName} frK");
						AssertNear(row.FrX, (double)cl["frX"]!, $"{leName}/{bName} frX");
						AssertNear(row.FrY, (double)cl["frY"]!, $"{leName}/{bName} frY");
						AssertNear(row.QTrans, (double)cl["q_trans"]!, $"{leName}/{bName} q_trans");

						var oComps = (JArray)cl["K_components"]!;
						Assert.That(row.KComponents.Count, Is.EqualTo(oComps.Count),
							$"{leName}/{bName} K component count");
						for (int i = 0; i < oComps.Count; i++)
						{
							var oc = oComps[i];
							var kc = row.KComponents[i];
							Assert.That(kc.Partner, Is.EqualTo((string?)oc["partner"]),
								$"{leName}/{bName} K[{i}] partner");
							if (oc["gap_m"]!.Type != JTokenType.Null)
								AssertNear(kc.GapM ?? double.NaN, (double)oc["gap_m"]!,
									$"{leName}/{bName} K[{i}] gap");
							AssertNear(kc.Frac, (double)oc["frac"]!, $"{leName}/{bName} K[{i}] frac");
						}
					}
				}

				// STEP 4 joint checks
				foreach (var (leName, braces) in (JObject)o["joint_checks"]!)
				{
					var le = topo.JointChecks.First(x => x.Name == leName);
					foreach (var (bName, jc) in (JObject)braces!)
					{
						var row = le.Rows.First(r => r.Name == bName);
						bool oSkipped = (bool)jc!["skipped"]!;
						Assert.That(row.Skipped, Is.EqualTo(oSkipped), $"{leName}/{bName} skipped");
						if (oSkipped) continue;
						AssertNear(row.Util, (double)jc["util"]!, $"{leName}/{bName} util");
						Assert.That(row.Passed, Is.EqualTo((bool)jc["passed"]!), $"{leName}/{bName} passed");
						AssertNear(row.NRdWeighted, (double)jc["N_Rd_weighted"]!, $"{leName}/{bName} N_Rd");
						AssertNear(row.MRdIp, (double)jc["M_Rd_ip"]!, $"{leName}/{bName} M_Rd_ip");
						AssertNear(row.MRdOp, (double)jc["M_Rd_op"]!, $"{leName}/{bName} M_Rd_op");
						Assert.That(row.WithinRange, Is.EqualTo((bool)jc["within_range"]!),
							$"{leName}/{bName} within_range");
						Assert.That(row.ChordOverstressed, Is.EqualTo((bool)jc["chord_overstressed"]!),
							$"{leName}/{bName} chord_overstressed");
						Assert.That(row.DomClass, Is.EqualTo((string?)jc["dom_class"]),
							$"{leName}/{bName} dom_class");
					}
				}
			});
		}

		// Catalog-name conventions seen in real projects (the comma-separator form is what made the
		// K/X/TY benchmark files fail the CHS gate before the tolerant parser — "ISSUE FOR ONDREJ").
		[TestCase("CHS168.3/8.0", 168.3, 8.0)]
		[TestCase("CHS457,16", 457, 16)]
		[TestCase("CHS457,16 - CHORD(CHS457,16)", 457, 16)]
		[TestCase("CHS168,3/8,0", 168.3, 8.0)]
		[TestCase("CHS 508 x 12.7", 508, 12.7)]
		public void ParseChs_AcceptsRealWorldNames(string name, double d, double t)
		{
			var (pd, pt) = JointSectionInfo.ParseChs(name);
			Assert.Multiple(() =>
			{
				Assert.That(pd, Is.EqualTo(d).Within(1e-9), $"{name} D");
				Assert.That(pt, Is.EqualTo(t).Within(1e-9), $"{name} T");
			});
		}

		[TestCase("IPE300")]
		[TestCase("HEB 200")]
		[TestCase(null)]
		public void ParseChs_RejectsNonChs(string? name)
		{
			var (pd, _) = JointSectionInfo.ParseChs(name);
			Assert.That(pd, Is.Null, $"{name}");
		}

		[Test]
		public void NodeEquilibrium_MatchesPythonReference()
		{
			// residuals per fixture must match the reference (validates the force-reading recipe:
			// application points, r×F transfer, continuous-member projection)
			foreach (var fx in (JArray)_fixtures["fixtures"]!)
			{
				string name = (string)fx["name"]!;
				var members = fx["members"]!
					.Select(j => j.ToObject<ConMember>()!)
					.Select(m => JointMemberData.FromConMember(m,
						_sections.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
					.ToList();
				var les = fx["loadEffects"]!.Select(j => j.ToObject<ConLoadEffect>()!).ToList();
				var eq = JointForceResolver.NodeEquilibrium(members, les, Vec3.Zero);

				var oEq = (JArray)_oracle[name]!["equilibrium"]!;
				Assert.That(eq.Count, Is.EqualTo(oEq.Count), $"{name} LE count");
				for (int i = 0; i < eq.Count; i++)
				{
					AssertNear(eq[i].ResF, (double)oEq[i]["resF_N"]!, $"{name} LE{i} resF");
					AssertNear(eq[i].ResM, (double)oEq[i]["resM_Nm"]!, $"{name} LE{i} resM");
				}
			}
		}
	}
}
