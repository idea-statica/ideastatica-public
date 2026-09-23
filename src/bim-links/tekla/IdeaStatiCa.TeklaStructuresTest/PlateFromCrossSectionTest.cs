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
		/// The contour spans the wide direction, so the plate is 245 across whichever local axis that turns out to be.
		/// </summary>
		[Test]
		public void Contour_SpansTheWideCrossSectionDirection()
		{
			var plate = BulkSelectionHelper.PlateFromCrossSection(
				IdentityLcs, Begin, End, halfExtentAcrossY: 122.5, halfExtentAcrossZ: 9.55);

			WidthAcross(plate.Contour, p => p.Y).Should().BeApproximately(245, 1e-9);
		}

		/// <summary>
		/// The same part with its section turned a quarter turn: the thin direction is now the local Z, and reading the
		/// plate off a fixed axis would return a 19 mm wide plate 245 mm thick - the part inside out.
		/// </summary>
		[Test]
		public void SectionTurnedAQuarterTurn_StillGivesTheThinDirectionAsThickness()
		{
			var plate = BulkSelectionHelper.PlateFromCrossSection(
				IdentityLcs, Begin, End, halfExtentAcrossY: 9.55, halfExtentAcrossZ: 122.5);

			plate.Thickness.Should().BeApproximately(19.1, 1e-9);
			WidthAcross(plate.Contour, p => p.Z).Should().BeApproximately(245, 1e-9);
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
			WidthAcross(plate.Contour, p => p.X).Should().BeApproximately(410, 1e-9);
		}

		private static readonly IPoint3D Begin = new Point3D(0, 0, 0);
		private static readonly IPoint3D End = new Point3D(410, 0, 0);

		private static Matrix44 IdentityLcs => new Matrix44(
			new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1));

		private static double WidthAcross(System.Collections.Generic.IEnumerable<IPoint3D> contour, System.Func<IPoint3D, double> axis)
			=> contour.Max(axis) - contour.Min(axis);
	}
}
