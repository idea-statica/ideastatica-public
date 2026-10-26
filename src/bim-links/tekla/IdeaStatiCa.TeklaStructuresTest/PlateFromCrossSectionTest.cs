using CI.Geometry3D;
using FluentAssertions;
using IdeaStatiCa.TeklaStructuresPlugin.Utilities;
using NUnit.Framework;
using System.Linq;

namespace IdeaStatiCa.TeklaStructuresTest
{
	/// <summary>
	/// How a part the source models as a beam with a rectangular profile becomes a plate. Millimetres, as everywhere
	/// the link reads Tekla model coordinates.
	/// </summary>
	public class PlateFromCrossSectionTest
	{
		/// <summary>
		/// PL19.1*245: the thickness is the thin direction, not twice it. The box the node search runs on doubles any
		/// cross-section half-extent under 50 mm so small parts are easier to catch, which is every plate's thickness -
		/// so the plate's own dimensions cannot be read off that box.
		/// </summary>
		[Test]
		public void Thickness_IsTheThinCrossSectionDirection()
		{
			var plate = BulkSelectionHelper.PlateFromCrossSection(
				IdentityLcs, Begin, End, halfExtentAcrossY: 122.5, halfExtentAcrossZ: 9.55);

			plate.Thickness.Should().BeApproximately(19.1, 1e-9);
		}

		/// <summary>
		/// The contour spans the wide direction along the axis that direction actually lies on. Both cases are here
		/// because a rule that reads a fixed axis answers one of them correctly by luck: with the wide half-extent on
		/// Y the plate must lie in Y, with it on Z the plate must lie in Z.
		/// </summary>
		[TestCase(122.5, 9.55, TestName = "Contour_SpansTheWideDirection_WhenItIsY")]
		public void Contour_SpansTheWideDirection_OnY(double acrossY, double acrossZ)
		{
			var plate = BulkSelectionHelper.PlateFromCrossSection(IdentityLcs, Begin, End, acrossY, acrossZ);

			SpanAlong(plate.Contour, p => p.Y).Should().BeApproximately(245, 1e-9);
			SpanAlong(plate.Contour, p => p.Z).Should().BeApproximately(0, 1e-9);
		}

		/// <summary>
		/// The same part with its section turned a quarter turn: reading a fixed axis would return a 19 mm wide plate
		/// 245 mm thick - the part inside out.
		/// </summary>
		[Test]
		public void SectionTurnedAQuarterTurn_SpansTheWideDirectionOnZ()
		{
			var plate = BulkSelectionHelper.PlateFromCrossSection(
				IdentityLcs, Begin, End, halfExtentAcrossY: 9.55, halfExtentAcrossZ: 122.5);

			plate.Thickness.Should().BeApproximately(19.1, 1e-9);
			SpanAlong(plate.Contour, p => p.Z).Should().BeApproximately(245, 1e-9);
			SpanAlong(plate.Contour, p => p.Y).Should().BeApproximately(0, 1e-9);
		}

		/// <summary>
		/// The axes are the matrix's own, not the world's. A part lying along the world Z with its width across the
		/// world X has a local frame that maps neither to the other, and reading a world axis - or handing the two
		/// half-extents over in the frame's opposite order - puts the contour in the plane of the plate's normal
		/// while leaving its dimensions and thickness looking entirely correct.
		/// </summary>
		[Test]
		public void Contour_FollowsTheMatrixAxes_NotTheWorldAxes()
		{
			// X along world Z, local Y along world X, local Z along world -Y.
			var rotated = new Matrix44(
				new Point3D(0, 0, 0), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0));

			// The part runs along its own X, which here is the world Z.
			var plate = BulkSelectionHelper.PlateFromCrossSection(
				rotated, new Point3D(0, 0, 0), new Point3D(0, 0, 410), halfExtentAcrossY: 122.5, halfExtentAcrossZ: 9.55);

			// The wide half-extent is on the local Y, which is the world X - so the contour must spread in world X.
			SpanAlong(plate.Contour, p => p.X).Should().BeApproximately(245, 1e-9);
			SpanAlong(plate.Contour, p => p.Y).Should().BeApproximately(0, 1e-9);
			SpanAlong(plate.Contour, p => p.Z).Should().BeApproximately(410, 1e-9);
		}

		/// <summary>
		/// Four corners, and the part's own length between the two ends - the contour is the rectangle the wide
		/// direction sweeps, not a strip at one end.
		/// </summary>
		[Test]
		public void Contour_IsTheRectangleSweptBetweenTheEnds()
		{
			var plate = BulkSelectionHelper.PlateFromCrossSection(
				IdentityLcs, Begin, End, halfExtentAcrossY: 122.5, halfExtentAcrossZ: 9.55);

			plate.Contour.Should().HaveCount(4);
			SpanAlong(plate.Contour, p => p.X).Should().BeApproximately(410, 1e-9);
		}

		/// <summary>
		/// The box measures across Tekla's axes and the part matrix is built from a different pair of them, so the two
		/// half-extents cross on the way over. Handing them across in Tekla's own order instead leaves the width, the
		/// length and the thickness all reading correctly while the plate lies in the plane of its own normal - which
		/// is why the swap has to be pinned here rather than noticed in a model.
		/// </summary>
		[Test]
		public void CrossSectionHalfExtents_CrossTeklaAxesOntoTheMatrixAxes()
		{
			var across = BulkSelectionHelper.CrossSectionHalfExtents(
				extentAcrossTeklaY: 122.5, extentAcrossTeklaZ: 9.55);

			across.AcrossY.Should().BeApproximately(9.55, 1e-9, "Extent2 is measured across Tekla Z, which the matrix takes as its Y");
			across.AcrossZ.Should().BeApproximately(122.5, 1e-9, "Extent1 is measured across Tekla Y, which the matrix takes as its Z");
		}

		private static readonly IPoint3D Begin = new Point3D(0, 0, 0);
		private static readonly IPoint3D End = new Point3D(410, 0, 0);

		private static Matrix44 IdentityLcs => new Matrix44(
			new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1));

		private static double SpanAlong(System.Collections.Generic.IEnumerable<IPoint3D> contour, System.Func<IPoint3D, double> axis)
			=> contour.Max(axis) - contour.Min(axis);
	}
}
