using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Geometry3D;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Pins the IOM facet-ring reading of a tube's D and T — the replacement for parsing them out
	/// of the section name, which is wrong for 96 % of catalogue circular profiles.
	///
	/// The fixtures are generated from the geometry rather than captured from a service, so the
	/// expected values are independent of this implementation.
	///
	/// A facet is a flat plate and its origin is the centre of that plate's face, so the origins do
	/// NOT sit on the mid-surface circle of diameter (D-T) — they sit on its APOTHEM circle, radius
	/// (D-T)/2·cos(π/n), which is exactly what the cos(π/n) in the formula undoes. (Generating the
	/// fixture on the mid-surface circle instead makes the formula overshoot by 8 % at n = 8, which
	/// is how this was pinned down.) Feeding the apothem ring back through the formula must return
	/// D exactly, at any facet count.
	/// </summary>
	[TestFixture]
	public class TubeFromIomTests
	{
		/// <summary>
		/// A tube's facet ring as IDEA models it: n plates whose origins are the centres of the
		/// facet faces, i.e. on the apothem circle of the mid-surface polygon.
		/// </summary>
		private static BeamData TubeBeam(double dMm, double tMm, int facets,
			string crossSectionType = "RolledCHS")
		{
			double midRadiusM = (dMm - tMm) / 2.0 / 1000.0 * Math.Cos(Math.PI / facets);
			var plates = new List<PlateData>();
			for (int i = 0; i < facets; i++)
			{
				double a = 2.0 * Math.PI * i / facets;
				plates.Add(new PlateData
				{
					Name = $"facet{i}",
					Thickness = tMm / 1000.0,
					Origin = new Point3D
					{
						X = midRadiusM * Math.Cos(a),
						Y = midRadiusM * Math.Sin(a),
						Z = 0,
					},
				});
			}
			return new BeamData { Name = "B1", CrossSectionType = crossSectionType, Plates = plates };
		}

		[TestCase(8)]
		[TestCase(16)]
		[TestCase(24)]
		[TestCase(64)]
		[TestCase(96)]
		public void FromBeam_RecoversDiameter_AtAnyFacetCount(int facets)
		{
			var (d, t, why) = TubeFromIom.FromBeam(TubeBeam(500.0, 20.0, facets));

			Assert.That(why, Is.Null);
			Assert.Multiple(() =>
			{
				// exact, not approximate: the inscribed-chord correction is what cos(π/n) undoes,
				// which is why the answer does not drift with the project's facet-division setting
				Assert.That(d!.Value, Is.EqualTo(500.0).Within(1e-6));
				Assert.That(t!.Value, Is.EqualTo(20.0).Within(1e-9));
			});
		}

		[Test]
		public void FromBeam_WorksForAThinWallToo()
		{
			var (d, t, _) = TubeFromIom.FromBeam(TubeBeam(141.3, 6.55, 32));

			Assert.Multiple(() =>
			{
				Assert.That(d!.Value, Is.EqualTo(141.3).Within(1e-6));
				Assert.That(t!.Value, Is.EqualTo(6.55).Within(1e-9));
			});
		}

		/// <summary>
		/// An I-section gives 3 facets. The formula would return a plausible-looking number from
		/// them, so the facet-count floor is what keeps a non-tube out.
		/// </summary>
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(7)]
		public void FromBeam_RejectsTooFewFacets(int facets)
		{
			var (d, t, why) = TubeFromIom.FromBeam(TubeBeam(300.0, 10.0, facets));

			Assert.That(d, Is.Null);
			Assert.That(t, Is.Null);
			Assert.That(why, Does.Contain("facet"));
		}

		/// <summary>Two wall thicknesses is an I-section or a welded shape, not a uniform tube.</summary>
		[Test]
		public void FromBeam_RejectsMixedThicknesses()
		{
			var beam = TubeBeam(400.0, 12.0, 16);
			beam.Plates[3].Thickness = 0.020;   // one facet thicker

			var (d, _, why) = TubeFromIom.FromBeam(beam);

			Assert.That(d, Is.Null);
			Assert.That(why, Does.Contain("thickness"));
		}

		/// <summary>
		/// Negative objects are modelling subtractions, not wall facets, and must not be counted.
		/// Asserted on the count rather than on D: dropping facets from a ring generated for n
		/// changes the facet count the formula divides by, so the diameter legitimately shifts —
		/// what matters here is that the negative plates take no part at all. A negative plate with
		/// a foreign thickness would otherwise trip the uniform-wall gate.
		/// </summary>
		[Test]
		public void FromBeam_IgnoresNegativeObjects()
		{
			var beam = TubeBeam(500.0, 20.0, 16);
			foreach (var p in beam.Plates.Take(3))
			{
				p.IsNegativeObject = true;
				p.Thickness = 0.099;          // would break the single-thickness gate if counted
				p.Origin = new Point3D { X = 9, Y = 9, Z = 9 };   // and would blow up maxdist
			}

			var (d, t, why) = TubeFromIom.FromBeam(beam);

			Assert.That(why, Is.Null, "the negative plates must be filtered out, not rejected");
			Assert.Multiple(() =>
			{
				Assert.That(t!.Value, Is.EqualTo(20.0).Within(1e-9), "thickness from the real facets only");
				Assert.That(d!.Value, Is.LessThan(520.0), "and maxdist must not come from the stray origins");
			});
		}

		[Test]
		public void FromBeam_HandlesNullAndEmpty()
		{
			Assert.That(TubeFromIom.FromBeam(null).D, Is.Null);
			Assert.That(TubeFromIom.FromBeam(new BeamData { Plates = new List<PlateData>() }).D, Is.Null);
		}

		/// <summary>
		/// Only tubular beams may reach the formula — the gate is the cross-section type, and it is
		/// the same set the §6.4 path uses (JointSectionMap.ChsTypes).
		/// </summary>
		[Test]
		public void TubularBeamsByName_KeepsTubesAndDropsTheRest()
		{
			var iom = new ConnectionData
			{
				Beams = new List<BeamData>
				{
					TubeBeam(500, 20, 16),                                        // B1, RolledCHS
					TubeBeam(300, 10, 16, crossSectionType: "RolledI"),           // not tubular
					TubeBeam(200, 8, 16, crossSectionType: "CHSPar"),             // tubular
					TubeBeam(150, 6, 16, crossSectionType: "CFRegPolygon"),       // polygon: not a tube
				},
			};
			iom.Beams[1].Name = "I1";
			iom.Beams[2].Name = "B2";
			iom.Beams[3].Name = "P1";

			var map = TubeFromIom.TubularBeamsByName(iom);

			Assert.That(map.Keys, Is.EquivalentTo(new[] { "B1", "B2" }));
		}

		[Test]
		public void TubularBeamsByName_HandlesNull()
			=> Assert.That(TubeFromIom.TubularBeamsByName(null), Is.Empty);
	}
}
