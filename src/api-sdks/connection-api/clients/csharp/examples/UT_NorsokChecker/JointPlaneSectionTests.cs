using NorsokChecker.Models;
using NorsokChecker.Services;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The "Joint plane and force transformation" section — the report's answer to the reviewer who
	/// cannot reconcile a single force in it against the model.
	///
	/// §6.4 is not evaluated on the load effects as IDEA StatiCa Connection shows them: the tool
	/// resolves a joint plane, identifies the through chord, and projects the member forces into it.
	/// The report used to print only the OUTPUT, headed "Applied forces (in the joint plane)", so
	/// there was no way in from the model side at all.
	/// </summary>
	[TestFixture]
	public class JointPlaneSectionTests
	{
		/// <summary>
		/// A topology with a known frame, a chord, two braces and one load effect.
		///
		/// The frame is deliberately OFF-AXIS — no member aligned to a global plane. A frame aligned
		/// to the axes makes "the local force was copied through" and "the force was projected"
		/// produce identical numbers, so the test could not tell them apart.
		/// </summary>
		private static JointTopology Topology()
		{
			var chord = new JointMemberData
			{
				Id = 1, Name = "M1",
				Section = new JointSectionInfo { Name = "CHS273.0/12.5", D = 0.273, T = 0.0125 },
			};

			return new JointTopology
			{
				Chord = chord,
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
		}

		private static NorsokFormulaResult Assessed() => new()
		{
			Section = "6.4.3.6", Equation = "6.57", Title = "Tubular Joint — M2",
			LoadCaseName = "LE12", Utilization = 0.476, Passed = true,
		};

		private static string Report(bool withTopology = true) =>
			NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				new List<(string, List<NorsokFormulaResult>)>
					{ ("CON1", new List<NorsokFormulaResult> { Assessed() }) },
				expandAll: false,
				jointImages: null,
				topologies: withTopology
					? new Dictionary<string, JointTopology> { ["CON1"] = Topology() }
					: null);

		/// <summary>
		/// THE test: the model's own force and the projected force appear TOGETHER.
		///
		/// Both numbers, in one table. Printing only the projected value is the defect; printing
		/// only the local one would be a different defect. The pair is what makes the check
		/// traceable, so the pair is what is asserted.
		/// </summary>
		[Test]
		public void BothTheModelForceAndTheProjectedForceAreShown()
		{
			string html = Report();

			int at = html.IndexOf("Force transformation", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the transformation section is rendered");
			string table = html[at..(at + 3000)];

			Assert.Multiple(() =>
			{
				// M2's axial force, as the model carries it and as §6.4 checks it.
				Assert.That(table, Does.Contain("-142.1 kN"), "N from the model");
				// Its in-plane moment differs between the two frames — 4.70 local, 4.31 projected —
				// which is the whole point: the same loading, two frames, two numbers.
				Assert.That(table, Does.Contain("4.70 kN&middot;m"), "M_y in the member's local axes");
				Assert.That(table, Does.Contain("4.31 kN&middot;m"), "and after projection");
				Assert.That(table, Does.Contain("from the model (local axes)"),
					"the columns say which frame each half is in");
				Assert.That(table, Does.Contain("resolved into the joint plane"));
				Assert.That(table, Does.Contain("LE12"),
					"and WHICH load effect is being shown, or the numbers are unattributable");
			});
		}

		/// <summary>
		/// The plane, the chord and the frame are stated in model coordinates.
		///
		/// Without the normal a reader cannot reproduce the projection at all, and without the chord
		/// they cannot tell which member the β and γ ratios are taken against.
		/// </summary>
		[Test]
		public void ThePlaneAndTheChordAreStated()
		{
			string html = Report();

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("Joint plane and force transformation"), "the section");
				Assert.That(html, Does.Contain("chord (through member)"), "which member is the chord");
				Assert.That(html, Does.Contain("M1"), "and its name");
				Assert.That(html, Does.Contain("CHS273.0/12.5"), "with its section");
				Assert.That(html, Does.Contain("plane normal"), "the normal is given");
				// Off-axis by construction, so the components are all non-trivial.
				Assert.That(html, Does.Contain("+0.577"), "in model coordinates, as a vector");
				Assert.That(html, Does.Contain("least-squares fit"), "and how the plane was fixed");
			});
		}

		/// <summary>
		/// The per-member geometry is printed ONCE for the joint, not once per check.
		///
		/// The review measured the repetition: θ, β, γ, τ and the chord's section properties were
		/// printed identically inside every one of the 30 checks — pages 6 and 20 of the shipped
		/// report carry the same numbers. These are properties of the JOINT.
		/// </summary>
		[Test]
		public void TheMemberGeometryIsListedOncePerJoint()
		{
			string html = Report();

			int at = html.IndexOf("Members &mdash; geometry at the joint", StringComparison.Ordinal);
			Assert.That(at, Is.GreaterThan(0), "the geometry table");
			string table = html[at..html.IndexOf("</table>", at, StringComparison.Ordinal)];

			Assert.Multiple(() =>
			{
				Assert.That(table, Does.Contain("M2").And.Contain("M3"), "every brace has a row");
				Assert.That(table, Does.Contain("47.3&deg;"), "θ per brace");
				Assert.That(table, Does.Contain("0.279"), "β per brace");
				Assert.That(table, Does.Contain("+ey").And.Contain("&minus;ey"),
					"and which chord face each lands on — the two braces are on opposite faces");
			});
		}

		/// <summary>
		/// No topology, no section — and the rest of the report is unaffected.
		///
		/// The parameter is optional so a test about check cards need not build a topology. Worth
		/// pinning: a generator that threw or emitted an empty section here would make every
		/// existing report test carry a topology it does not care about.
		/// </summary>
		[Test]
		public void WithoutATopologyTheSectionIsSimplyAbsent()
		{
			string html = Report(withTopology: false);

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("Joint plane and force transformation"));
				Assert.That(html, Does.Contain("Tubular Joint"), "the check card is still there");
			});
		}

		/// <summary>
		/// The section comes BEFORE the checks that use its forces.
		///
		/// A reader meeting the utilisation first and its inputs afterwards is reading a claim with
		/// an appendix — the same reason eq (6.57) closes the derivation rather than opening it.
		/// </summary>
		[Test]
		public void TheSectionPrecedesTheChecks()
		{
			string html = Report();

			int plane = html.IndexOf("Joint plane and force transformation", StringComparison.Ordinal);
			int card = html.IndexOf("<details class='check-card", StringComparison.Ordinal);

			Assert.Multiple(() =>
			{
				Assert.That(plane, Is.GreaterThan(0));
				Assert.That(card, Is.GreaterThan(plane),
					"the inputs are stated before the check that consumes them");
			});
		}

		/// <summary>
		/// The app HANDS the generator its topologies.
		///
		/// The section renders correctly and would render nothing at all if MainWindow stopped
		/// passing the dictionary — measured on the previous three steps, a change connected to
		/// nothing leaves every test green. On the source, because the alternative is a live service.
		/// </summary>
		[Test]
		public void TheAppPassesTheTopologiesToTheReport()
		{
			var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !System.IO.Directory.Exists(
				System.IO.Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Ignore("cannot locate the NorsokChecker source");

			string code = System.Text.RegularExpressions.Regex.Replace(
				System.IO.File.ReadAllText(System.IO.Path.Combine(
					dir!.FullName, "NorsokChecker", "MainWindow.Report.cs")),
				@"//[^\n]*", "");

			// The ARGUMENT, not the word. Matching "topologies" anywhere keeps finding the local
			// dictionary that builds it — measured: removing the argument from the call left the
			// declaration in place and this test green, which is the wiring defect it exists for.
			var call = System.Text.RegularExpressions.Regex.Match(code,
				@"GenerateReport\(([^;]*?)\);", System.Text.RegularExpressions.RegexOptions.Singleline);

			Assert.Multiple(() =>
			{
				Assert.That(code, Does.Contain("_topologyPerConnection"),
					"the report is built from the run's own topologies");
				Assert.That(call.Success, Is.True, "the report is generated somewhere in this file");
				Assert.That(call.Groups[1].Value, Does.Contain("topologies"),
					"and the topologies are an ARGUMENT to it — a dictionary nothing passes is a "
					+ "section that never renders");
			});
		}

		/// <summary>
		/// The projection carries the model's own numbers through untouched.
		///
		/// The Local* fields exist so the report can show provenance, and their whole value is that
		/// they are NOT recomputed. Asserted against the resolver, not the fixture: a copy that
		/// scaled or re-signed anything would make the left-hand column a second opinion rather than
		/// the model's own figure.
		/// </summary>
		[Test]
		public void TheLocalForcesAreCopiedFromTheModelWithoutArithmetic()
		{
			var m = new JointMemberData
			{
				Id = 2, Name = "M2",
				AxisX = new Vec3(0.6, 0.8, 0), AxisY = new Vec3(-0.8, 0.6, 0),
				AxisZ = new Vec3(0, 0, 1),
				Section = new JointSectionInfo { D = 0.0761, T = 0.0036 },
			};
			var sl = new IdeaStatiCa.Api.Connection.Model.ConLoadEffectSectionLoad
			{
				N = -142_100, Vy = 3_200, Vz = -1_100, Mx = 210, My = 4_700, Mz = -980,
			};

			var row = JointForceResolver.BraceForceInPlane(
				m, sl, new Vec3(0.7071, 0.7071, 0), new Vec3(0.5774, -0.5774, 0.5774));

			Assert.Multiple(() =>
			{
				Assert.That(row.LocalN, Is.EqualTo(-142_100), "N, verbatim");
				Assert.That(row.LocalVy, Is.EqualTo(3_200));
				Assert.That(row.LocalVz, Is.EqualTo(-1_100));
				Assert.That(row.LocalMx, Is.EqualTo(210));
				Assert.That(row.LocalMy, Is.EqualTo(4_700));
				Assert.That(row.LocalMz, Is.EqualTo(-980));

				// And the projection still differs from them, or the section shows one number twice.
				Assert.That(row.Mip, Is.Not.EqualTo(row.LocalMy).Within(1.0),
					"the projected in-plane moment is not the local My");
			});
		}
	}
}
