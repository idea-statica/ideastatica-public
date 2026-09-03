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
	///
	/// ONE DELIBERATE DIVERGENCE, 2026-09-02 — the only place the numbers were edited rather than
	/// the C#, so it is recorded here rather than left to be rediscovered as a puzzle:
	///
	///   `M_op`'s SIGN. The python reference builds the in-plane axis as `ip = n_b × bx`
	///   (extract.py:388), which makes the brace frame (bx, n_b, ip) LEFT-handed — measured, triple
	///   product −1 on every off-axis brace. The C# now uses `Cross(bx, nb)` so the frame is
	///   right-handed, which reverses `M_op` and `V_ip`. Two oracle values were negated for it
	///   (K_TEST/BA and KT_TEST/KV, the only non-zero `M_op` in the file).
	///
	///   Why this is defensible: NORSOK gives the BRACE quantities no sign convention at all — `S_d`
	///   is a "design action effect", i.e. a magnitude (§4, eq 4.1), and eq (6.57) takes `M_z` in
	///   absolute value. Nothing in the norm distinguishes the two senses, so neither side was
	///   "right"; the right-handed frame is. BraceFrameTests asserts both the handedness and that no
	///   result moves.
	///
	///   Cost, stated plainly: for `M_op`'s sign this oracle is no longer independent — it now
	///   records our decision. Every other field, `sigma_my` included (where the sign DOES change
	///   Q_f), is still the python reference's own output.
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

		/// <summary>
		/// A joint displaced as a RIGID BODY is assessed; the same joint with only its braces moved
		/// is not.
		///
		/// The defect this pair exists for: the out-of-plane gate measured each brace's eccentricity
		/// from the plane through the WORK POINT, so a joint whose members all carry the same
		/// eccentricity — braces perfectly coplanar, their common plane merely offset — read as
		/// EVERY brace being out of plane and was rejected without one check running. The user found
		/// it on CON16 of their own model and diagnosed it exactly: the plane keeps its directions,
		/// it should sit on the CHORD.
		///
		/// Both fixtures are the model's own CON8 read from the live service, so the geometry is
		/// GENERALLY ORIENTED (chord axis (0.588, 0.158, 0.793), plane normal
		/// (0.660, −0.660, −0.358)). That is deliberate and it is the harder half: an axis-aligned
		/// case cannot tell "measured against the chord" from "measured against global Z", and my
		/// own first probe of this was broken for exactly that reason — applying the SAME local
		/// offset triple to every member is not a rigid translation, because axisY = globalZ × axisX
		/// differs per member. CON17's six triples all differ and produce one global displacement.
		///
		/// CON18 is the control, and without it CON17 would only prove the gate can accept.
		/// </summary>
		[TestCase("CON17_RIGID_SHIFT", 0.0, false,
			TestName = "a rigidly displaced joint is assessed")]
		[TestCase("CON18_BRACES_ONLY", 40.0, true,
			TestName = "braces displaced away from the chord are still rejected")]
		public void OutOfPlaneIsMeasuredFromThePlaneThroughTheChord(
			string fixtureName, double expectedBraceOffsetMm, bool expectRejected)
		{
			var topo = BuildFixture(fixtureName);

			Assert.Multiple(() =>
			{
				Assert.That(topo.Chord, Is.Not.Null, "the chord was identified");
				Assert.That(topo.BracesMeta, Is.Not.Empty, "and the braces were read");

				// Every brace's distance FROM THE PLANE THROUGH THE CHORD — 0 for a rigid shift,
				// 40 mm when only the braces moved.
				foreach (var bm in topo.BracesMeta)
					Assert.That(bm.OopOffsetM * 1000.0,
						Is.EqualTo(expectedBraceOffsetMm).Within(0.5),
						$"{bm.Name}: distance from the plane through the chord");

				// The gate's own verdict, which is what the connection lives or dies by.
				bool rejected = topo.Verdict.Errors.Any(e => e.Contains("out of the joint plane"));
				Assert.That(rejected, Is.EqualTo(expectRejected),
					"errors: " + string.Join(" | ", topo.Verdict.Errors));
			});
		}

		/// <summary>
		/// The plane's own displacement is reported, and it is the CHORD's eccentricity.
		///
		/// It is a real feature of the model — the joint sits off the work point — so the reader has
		/// to be able to see it. §6.4 gives no limit for it, so it is stated and not judged.
		/// </summary>
		[TestCase("CON17_RIGID_SHIFT", 40.0, TestName = "a rigid shift moves the plane")]
		[TestCase("CON18_BRACES_ONLY", 0.0, TestName = "an unmoved chord leaves it at the work point")]
		public void ThePlaneOffsetIsTakenFromTheChord(string fixtureName, double expectedMm)
		{
			var topo = BuildFixture(fixtureName);

			Assert.That(Math.Abs(topo.PlaneOffsetM) * 1000.0, Is.EqualTo(expectedMm).Within(0.5),
				"the plane's offset from the work point along its own normal");
		}

		/// <summary>
		/// Moving the chord by −e is the same joint as moving every brace by +e, and must get the
		/// same verdict.
		///
		/// This is the invariance the old form did not have, and the stronger half of the finding:
		/// the two are one physical joint modelled from opposite ends, and the node-relative measure
		/// gave them OPPOSITE answers — braces +40 mm was an ERROR, chord −40 mm passed. Built here
		/// rather than stored, because it is CON18 with the sign moved onto the chord.
		/// </summary>
		[Test]
		public void MovingTheChordReadsTheSameAsMovingTheBraces()
		{
			var bracesMoved = BuildFixture("CON18_BRACES_ONLY");

			// The same joint with the displacement moved onto the CHORD: braces at zero, chord at −e.
			//
			// Built by decomposing ONE global vector into each member's own frame, not by arithmetic
			// on the stored triples. Subtracting one member's triple from all of them looked
			// equivalent and is not — offsets live in each member's OWN axes, so that leaves the
			// other braces displaced by the difference between their frames. Measured: it left M5 at
			// (0, +0.062, 0) instead of zero, and the test passed while building a joint it did not
			// describe.
			var fx = ((JArray)_fixtures["fixtures"]!).First(f => (string?)f["name"] == "CON18_BRACES_ONLY");
			var members = fx["members"]!.Select(j => j.ToObject<ConMember>()!).ToList();

			// −e as a global vector: the negated displacement CON18 gives its braces.
			var braceRef = members.First(m => m.IsContinuous != true);
			var refData = JointMemberData.FromConMember(braceRef, new JointSectionInfo());
			Vec3 minusE = -JointForceResolver.EccVec(refData);

			foreach (var m in members)
			{
				var md = JointMemberData.FromConMember(m, new JointSectionInfo());
				bool isChord = m.IsContinuous == true;
				// The chord carries −e; every brace goes to zero.
				Vec3 want = isChord ? minusE : Vec3.Zero;
				m.Position!.OffsetEx = Vec3.Dot(want, md.AxisX);
				m.Position.OffsetEy = Vec3.Dot(want, md.AxisY);
				m.Position.OffsetEz = Vec3.Dot(want, md.AxisZ);
			}
			var chordMoved = new JointTopologyBuilder().Build(
				members.Select(m => JointMemberData.FromConMember(m,
					_sections.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo())).ToList(),
				fx["loadEffects"]!.Select(j => j.ToObject<ConLoadEffect>()!).ToList());

			bool RejectedFor(JointTopology t) =>
				t.Verdict.Errors.Any(e => e.Contains("out of the joint plane"));

			Assert.Multiple(() =>
			{
				// FIRST: prove the geometry is the one this test claims to have built. The earlier
				// version of this setup silently produced something else and passed anyway.
				Assert.That(Math.Abs(chordMoved.PlaneOffsetM) * 1000.0, Is.EqualTo(40.0).Within(0.5),
					"the CHORD carries the 40 mm now");
				foreach (var bm in chordMoved.BracesMeta)
					Assert.That(bm.OopOffsetM * 1000.0, Is.EqualTo(40.0).Within(0.5),
						$"{bm.Name}: 40 mm from the plane through the displaced chord");

				Assert.That(RejectedFor(bracesMoved), Is.True,
					"control: braces displaced from the chord IS an out-of-plane joint");
				Assert.That(RejectedFor(chordMoved), Is.EqualTo(RejectedFor(bracesMoved)),
					"the same joint modelled from the other end must get the same verdict; "
					+ "chord-moved errors: " + string.Join(" | ", chordMoved.Verdict.Errors));
			});
		}

		/// <summary>
		/// A rigid displacement changes NO force. The user asked for this explicitly rather than
		/// letting it be assumed, and it is the reason the change can be a geometry fix alone.
		///
		/// Measured against the live service before any code was written: over all 15 load effects
		/// of CON8 vs CON17 the largest change in the node-equilibrium residual was 0.0001 kN, and
		/// the chord's own section loads — the inputs to σ_a, σ_my, σ_mz and hence Q_f — were
		/// identical to three decimals. Expected, because position.origin already embeds the
		/// eccentricity and every lever moves together, but "expected" is not "checked".
		/// </summary>
		[Test]
		public void ARigidShiftChangesNoForce()
		{
			ConMember[] Load(string name) =>
				((JArray)_fixtures["fixtures"]!).First(f => (string?)f["name"] == name)["members"]!
					.Select(j => j.ToObject<ConMember>()!).ToArray();
			List<ConLoadEffect> Les(string name) =>
				((JArray)_fixtures["fixtures"]!).First(f => (string?)f["name"] == name)["loadEffects"]!
					.Select(j => j.ToObject<ConLoadEffect>()!).ToList();

			// CON17 (rigidly displaced) against the same members with every offset removed.
			var shifted = Load("CON17_RIGID_SHIFT");
			var atOrigin = Load("CON17_RIGID_SHIFT");
			foreach (var m in atOrigin)
			{
				m.Position!.OffsetEx = 0; m.Position.OffsetEy = 0; m.Position.OffsetEz = 0;
			}

			List<NodeEquilibriumRow> Residuals(ConMember[] ms) =>
				JointForceResolver.NodeEquilibrium(
					ms.Select(m => JointMemberData.FromConMember(m,
						_sections.GetValueOrDefault(m.CrossSectionId ?? -1) ?? new JointSectionInfo()))
						.ToList(),
					Les("CON17_RIGID_SHIFT"), Vec3.Zero);

			var a = Residuals(atOrigin);
			var b = Residuals(shifted);

			Assert.Multiple(() =>
			{
				Assert.That(b, Has.Count.EqualTo(a.Count), "same load effects both ways");
				for (int i = 0; i < a.Count; i++)
				{
					// 1 N / 1 N·m: the inputs are stored rounded, so an exact match is not the claim
					// — what matters is that a rigid shift does not move the residual.
					Assert.That(b[i].SumF.Norm, Is.EqualTo(a[i].SumF.Norm).Within(1.0),
						$"LE{i}: force residual must not move");
					Assert.That(b[i].SumM.Norm, Is.EqualTo(a[i].SumM.Norm).Within(1.0),
						$"LE{i}: moment residual must not move");
				}
			});
		}

		[Test]
		public void NodeEquilibrium_MatchesPythonReference()
		{
			// residuals per fixture must match the reference (validates the force-reading recipe:
			// application points, r×F transfer, continuous-member projection)
			foreach (var fx in (JArray)_fixtures["fixtures"]!)
			{
				string name = (string)fx["name"]!;
				// The CON17/CON18 pair is not in the python oracle: it exists to pin the plane's
				// POSITION, which the reference implementation measures from the work point (the
				// behaviour being corrected here). Its forces are asserted separately — see
				// ARigidShiftChangesNoForce.
				if (name.StartsWith("CON1", StringComparison.Ordinal)) continue;
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
					// the oracle stores the residual MAGNITUDE; NodeEquilibrium now returns the
					// vectors (the §6.4 tab shows them by component), so compare their norms
					AssertNear(eq[i].SumF.Norm, (double)oEq[i]["resF_N"]!, $"{name} LE{i} resF");
					AssertNear(eq[i].SumM.Norm, (double)oEq[i]["resM_Nm"]!, $"{name} LE{i} resM");
				}
			}
		}
	}
}
