using IdeaStatiCa.Api.Connection.Model;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Searches for a brace arrangement whose joint plane the builder has to FIT rather than find —
	/// the W3 warning — using the builder itself as the judge.
	///
	/// Two hand-rolled attempts at this got it wrong: computing "are any two braces within 2 deg of
	/// a common plane" is not what the builder does. It tries every brace's own perpendicular as a
	/// candidate plane and counts how many others fall inside the fit tolerance against THAT
	/// candidate, so the answer depends on which brace is the seed. The only reliable oracle is the
	/// builder, so this test asks it directly.
	///
	/// Marked Explicit: it is a search, not an assertion about the product. Run it when the
	/// gate-coverage file needs a W3 case:
	///   dotnet test --filter FullyQualifiedName~PlaneFitSearch
	/// </summary>
	[TestFixture]
	public class PlaneFitSearchTests
	{
		private const double Deg = Math.PI / 180.0;

		private static JointMemberData Member(string name, double bearingDeg, double tiltDeg,
			bool continuous, double d, double t)
		{
			double b = bearingDeg * Deg, ti = tiltDeg * Deg;
			var ax = new Vec3(Math.Cos(b) * Math.Cos(ti), Math.Sin(b) * Math.Cos(ti), Math.Sin(ti)).Unit();
			var ay = Vec3.Cross(new Vec3(0, 0, 1), ax);
			ay = ay.Norm < 1e-9 ? new Vec3(0, 1, 0) : ay.Unit();
			return new JointMemberData
			{
				Id = Math.Abs(name.GetHashCode() % 1000) + 1,
				Name = name, IsContinuous = continuous,
				ForcesIn = ConMemberForcesInEnum.Node,
				AxisX = ax, AxisY = ay, AxisZ = Vec3.Cross(ax, ay).Unit(),
				Origin = Vec3.Zero,
				Section = new JointSectionInfo
				{
					Name = $"CHS {d}/{t}", TypeName = "RolledCHS",
					IsCHS = true, D = d, T = t, Fy = 355e6,
				},
			};
		}

		[Test, Explicit("a search, not a product assertion — run on demand")]
		public void FindTiltsThatForceThePlaneToBeFitted()
		{
			// three braces on a 273 mm chord, spaced so their feet cannot overlap
			var bearings = new[] { 55.0, 125.0, -90.0 };
			double[] grid = { -12, -9, -6, -4, -2, 0, 2, 4, 6, 9, 12 };
			var found = new List<string>();

			foreach (double t1 in grid)
				foreach (double t2 in grid)
					foreach (double t3 in grid)
					{
						var ms = new[]
						{
							Member("M2", 0.0, 0.0, true, 273.0, 12.5),
							Member("M1", bearings[0], t1, false, 48.3, 3.2),
							Member("M3", bearings[1], t2, false, 48.3, 3.2),
							Member("M6", bearings[2], t3, false, 48.3, 3.2),
						};
						var topo = new JointTopologyBuilder(log: _ => { }).Build(ms, null);
						if (topo.Verdict.Status == "ERROR") continue;
						if ((topo.PlaneFitBasis ?? "").Contains("coplanar braces")) continue;
						found.Add($"tilts {t1,5:+0.0;-0.0} {t2,5:+0.0;-0.0} {t3,5:+0.0;-0.0}  "
							+ $"basis '{topo.PlaneFitBasis}'  "
							+ $"warnings {topo.Verdict.Warnings.Count}");
					}

			TestContext.Out.WriteLine($"{found.Count} arrangement(s) make the builder fit the plane:");
			foreach (string s in found.Take(15))
				TestContext.Out.WriteLine("  " + s);

			Assert.That(found, Is.Not.Empty,
				"no tilt combination on this grid produces a fitted plane — W3 may need a different "
				+ "brace count or spacing");
		}
	}
}
