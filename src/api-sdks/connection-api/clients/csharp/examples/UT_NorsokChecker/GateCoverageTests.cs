using IdeaStatiCa.Api.Connection.Model;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Runs the real topology builder over synthetic joints, so the gate-coverage connections in
	/// test_cs.ideaCon can be checked against what the builder actually decides rather than against
	/// arithmetic that resembles it.
	///
	/// This distinction mattered: a hand-rolled "are any two braces coplanar" check and the builder's
	/// own seed-and-count fit disagreed on the same geometry, because the builder tries EVERY brace's
	/// perpendicular as a candidate plane and counts inliers against each.
	/// </summary>
	[TestFixture]
	public class GateCoverageTests
	{
		private const double Deg = Math.PI / 180.0;

		private static JointMemberData Member(string name, double bearingDeg, double tiltDeg,
			bool continuous, double d = 76.0, double t = 3.5, bool tubular = true)
		{
			double b = bearingDeg * Deg, ti = tiltDeg * Deg;
			var ax = new Vec3(Math.Cos(b) * Math.Cos(ti), Math.Sin(b) * Math.Cos(ti), Math.Sin(ti)).Unit();
			// axisY/axisZ as the service derives them: axisY = globalZ x axisX
			var ay = Vec3.Cross(new Vec3(0, 0, 1), ax);
			ay = ay.Norm < 1e-9 ? new Vec3(0, 1, 0) : ay.Unit();
			var az = Vec3.Cross(ax, ay).Unit();

			return new JointMemberData
			{
				Id = Math.Abs(name.GetHashCode() % 1000) + 1,
				Name = name,
				IsContinuous = continuous,
				ForcesIn = ConMemberForcesInEnum.Node,
				AxisX = ax, AxisY = ay, AxisZ = az,
				Origin = Vec3.Zero,
				Section = new JointSectionInfo
				{
					Name = tubular ? $"CHS {d}/{t}" : "IPE100",
					TypeName = tubular ? "RolledCHS" : "RolledI",
					IsCHS = tubular, D = d, T = t, Fy = 355e6,
				},
			};
		}

		private static TopologyVerdict Verdict(params JointMemberData[] members)
			=> new JointTopologyBuilder(log: _ => { }).Build(members, null).Verdict;

		/// <summary>
		/// A coplanar joint with room between the brace feet. Deliberately NOT CON1's exact angles:
		/// CON1 relies on five CUT operations to resolve its overlapping feet, and a synthetic
		/// fixture has no operations, so copying its bearings produced
		/// "M3-M4: feet overlap (gap -70 mm)" and the joint was rejected before the gate under test
		/// could speak. Three small braces on one side of a large chord leave clear gaps.
		/// </summary>
		private static JointMemberData[] Con1Like() => new[]
		{
			Member("M2", 0.0, 0.0, continuous: true, d: 273.0, t: 12.5),
			Member("M1", 55.0, 0.0, false, d: 48.3, t: 3.2),
			Member("M3", 125.0, 0.0, false, d: 48.3, t: 3.2),
			Member("M6", -90.0, 0.0, false, d: 48.3, t: 3.2),
		};

		[Test]
		public void ACoplanarJointIsNotWarnedAboutItsPlane()
		{
			var v = Verdict(Con1Like());

			Assert.Multiple(() =>
			{
				// The absence is only evidence if the joint got as far as being judged. Without
				// this, an ERROR verdict — which warns about nothing because nothing ran — passes
				// the assertion below and reads as "no warning was needed".
				Assert.That(v.Status, Is.Not.EqualTo("ERROR"),
					"the joint must have been assessed for the absence of a warning to mean anything");
				Assert.That(v.Warnings.Where(w => w.Contains("plane")), Is.Empty,
					"and coplanar braces need no plane-fit warning");
			});
		}

		/// <summary>
		/// An overlap joint is rejected as OUR limit, not as the standard's.
		///
		/// The message read "overlap joint, out of 6.4 gap rules", which attributes a tool
		/// limitation to N-004. §6.4.4 (Rev. 3 p. 33) says overlap joints "may be designed using the
		/// simple joint provision of 6.4.3 with the following exemptions and additions" — shear
		/// along the chord face becomes a failure mode, §6.4.3.5 stops applying, and the through
		/// brace takes a share of the overlapping brace's force. None of that is implemented here,
		/// and that is the real reason the joint goes unchecked.
		///
		/// It was self-contradictory as well: the report's validity table prints g/D ≥ −0.6 as
		/// SATISFIED at −0.113 on the same joint, and its own Q_g has an overlap branch it uses.
		/// </summary>
		[Test]
		public void AnOverlapJointIsRejectedAsAToolLimitNotAsOutsideTheStandard()
		{
			// Two braces close enough on one side that their feet overlap.
			var v = Verdict(
				Member("M2", 0.0, 0.0, continuous: true, d: 273.0, t: 12.5),
				Member("M1", 40.0, 0.0, false, d: 168.3, t: 8.0),
				Member("M3", 55.0, 0.0, false, d: 168.3, t: 8.0));

			var overlap = v.Errors.Concat(v.Warnings)
				.FirstOrDefault(e => e.Contains("overlap"));
			Assert.That(overlap, Is.Not.Null,
				"this geometry overlaps; errors: " + string.Join(" | ", v.Errors));

			Assert.Multiple(() =>
			{
				Assert.That(overlap, Does.Contain("6.4.4"),
					"the clause that DOES cover overlap joints is named");
				Assert.That(overlap, Does.Contain("this tool does not implement"),
					"and the limit is stated as ours");
				Assert.That(overlap, Does.Not.Contain("out of 6.4 gap rules"),
					"not as the standard's, which would be false");
			});
		}

		/// <summary>E10 — a chord with nothing attached.</summary>
		[Test]
		public void AChordAloneIsRejected()
		{
			var v = Verdict(Member("M2", 0.0, 0.0, continuous: true, d: 141.3, t: 6.5));

			Assert.That(v.Status, Is.EqualTo("ERROR"));
			Assert.That(v.Errors.Any(e => e.Contains("No brace")), Is.True, string.Join(" | ", v.Errors));
		}

		/// <summary>E6 — the chord must be tubular, and the message must name its real type.</summary>
		[Test]
		public void ANonTubularChordIsRejectedByType()
		{
			var ms = Con1Like();
			ms[0] = Member("M2", 0.0, 0.0, continuous: true, d: 300, t: 7.1, tubular: false);

			var v = Verdict(ms);

			Assert.That(v.Status, Is.EqualTo("ERROR"));
			Assert.That(v.Errors.Any(e => e.Contains("RolledI")), Is.True, string.Join(" | ", v.Errors));
		}

		/// <summary>W7 — 20 deg is past the warning floor but above the 5 deg error floor.</summary>
		[Test]
		public void ABraceAt20DegreesIsWarnedNotRejected()
		{
			var ms = Con1Like();
			ms[1] = Member("M1", 20.0, 0.0, false, d: 48.3, t: 3.2);

			var v = Verdict(ms);

			Assert.Multiple(() =>
			{
				Assert.That(v.Errors.Any(e => e.Contains("M1")), Is.False,
					"20 deg is a warning, not an error: " + string.Join(" | ", v.Errors));
				Assert.That(v.Warnings.Any(w => w.Contains("M1") && w.Contains("30")), Is.True,
					string.Join(" | ", v.Warnings));
			});
		}

		/// <summary>W4 — one brace 8 deg out of a plane the other four define.</summary>
		[Test]
		public void ABrace8DegreesOutOfPlaneIsBorderline()
		{
			var ms = Con1Like();
			ms[3] = Member("M6", -90.0, 8.0, false, d: 48.3, t: 3.2);

			var v = Verdict(ms);

			Assert.Multiple(() =>
			{
				Assert.That(v.Status, Is.Not.EqualTo("ERROR"),
					"8 deg is inside the 15 deg limit: " + string.Join(" | ", v.Errors));
				Assert.That(v.Warnings.Any(w => w.Contains("M6") && w.Contains("borderline")), Is.True,
					string.Join(" | ", v.Warnings));
			});
		}

		/// <summary>
		/// W3 — the plane cannot be found, only fitted. These tilts come from PlaneFitSearchTests,
		/// which asks the builder itself; two hand-derived attempts were wrong, because "are any two
		/// braces within 2 deg of a common plane" is not the test the builder applies — it tries each
		/// brace's own perpendicular as a candidate and counts inliers against that.
		/// </summary>
		[Test]
		public void TiltsWithNoCoplanarPairForceThePlaneToBeFitted()
		{
			var ms = new[]
			{
				Member("M2", 0.0, 0.0, continuous: true, d: 273.0, t: 12.5),
				Member("M1", 55.0, -12.0, false, d: 48.3, t: 3.2),
				Member("M3", 125.0, -6.0, false, d: 48.3, t: 3.2),
				Member("M6", -90.0, 12.0, false, d: 48.3, t: 3.2),
			};

			var topo = new JointTopologyBuilder(log: _ => { }).Build(ms, null);

			Assert.Multiple(() =>
			{
				Assert.That(topo.Verdict.Status, Is.Not.EqualTo("ERROR"),
					"the spread must stay inside the 15 deg limit: "
					+ string.Join(" | ", topo.Verdict.Errors));
				Assert.That(topo.PlaneFitBasis, Does.Contain("closest pair"),
					$"the plane had to be fitted from a pair — basis was '{topo.PlaneFitBasis}'");
				Assert.That(topo.Verdict.Warnings.Any(w => w.Contains("plane")), Is.True,
					"and the user is told: " + string.Join(" | ", topo.Verdict.Warnings));
			});
		}
	}
}
