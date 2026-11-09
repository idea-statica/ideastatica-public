using CadExampleApi;
using CI.Geometry3D;
using IdeaStatiCa.BIM.Common;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BimApiLinkCadExample.BulkSelection
{
	/// <summary>
	/// Finds joints with the IDEA StatiCa items sorter: the places where members meet, with the plates and bolt grids
	/// around them.
	/// </summary>
	internal static class JointFinder
	{
		/// <summary>
		/// Groups the objects into joints: the places where members meet, with the plates and bolt grids found
		/// around them. Negative plates and cuts are not passed to the sorter.
		/// </summary>
		public static IReadOnlyList<FoundJoint> FindJoints(IEnumerable<CadObject> objects, ICadGeometryApi cad)
		{
			var data = new SorterData
			{
				Members = objects.OfType<CadMember>().Select(member => ToSorterMember(member, cad)).ToList(),
				Plates = objects.OfType<CadPlate>().Where(plate => !plate.IsNegative).Select(ToSorterPlate).ToList(),
				Fasteners = objects.OfType<CadBoltGrid>().Select(ToSorterFastener).ToList(),
				Welds = new List<Weld>(),
			};

			return new ItemsSorter().Sort(data, new SorterSettings()).Joints
				.Select(joint => new FoundJoint(
					new CadPoint3D(joint.Location.X, joint.Location.Y, joint.Location.Z),
					joint.Members.Select(member => (CadMember)member.Parent).ToList(),
					joint.Plates.Select(plate => (CadObject)plate.Parent)
						.Concat(joint.Fasteners.Select(fastener => (CadObject)fastener.Parent))
						.ToList()))
				.ToList();
		}

		/// <summary>
		/// Describes a member to the sorter: its axis, its coordinate system and the size of its cross-section.
		/// </summary>
		private static Member ToSorterMember(CadMember member, ICadGeometryApi cad)
		{
			CadCrossSection crossSection = cad.GetCrossSection(member.CrossSectionName);
			double halfWidth = crossSection.Width / 2;
			double halfHeight = crossSection.Height / 2;

			// The cross-section's extent along the member's local Y and Z.
			var bounds = new Rect(new System.Windows.Point(-halfWidth, -halfHeight), new System.Windows.Point(halfWidth, halfHeight));

			return new Member(member, ToMatrix(cad.GetMemberCoordinateSystem(member.Id)), ToPoint(member.Start), ToPoint(member.End), bounds);
		}

		/// <summary>
		/// Describes a plate to the sorter: its outline in model coordinates and its thickness.
		/// </summary>
		private static Plate ToSorterPlate(CadPlate plate)
		{
			CadCoordinateSystem plane = plate.Outline.CoordinateSystem;
			List<IPoint3D> contour = plate.Outline.Points.Select(point => (IPoint3D)ToPoint(plane.ToWorld(point))).ToList();

			return new Plate(plate, ToMatrix(plane), contour, plate.Thickness);
		}

		/// <summary>
		/// Describes a bolt grid to the sorter: its bolt positions in model coordinates.
		/// </summary>
		private static FastenerGrid ToSorterFastener(CadBoltGrid boltGrid)
		{
			CadCoordinateSystem plane = boltGrid.CoordinateSystem;
			List<Point3D> positions = boltGrid.Positions.Select(position => ToPoint(plane.ToWorld(position))).ToList();

			return new FastenerGrid(boltGrid, ToMatrix(plane), positions);
		}

		/// <summary>
		/// Converts a coordinate system to the sorter's geometry type.
		/// </summary>
		private static Matrix44 ToMatrix(CadCoordinateSystem coordinateSystem)
		{
			return new Matrix44(
				ToPoint(coordinateSystem.Origin),
				ToVector(coordinateSystem.X),
				ToVector(coordinateSystem.Y),
				ToVector(coordinateSystem.Z));
		}

		/// <summary>
		/// Converts a point to the sorter's geometry type.
		/// </summary>
		private static Point3D ToPoint(CadPoint3D point) => new Point3D(point.X, point.Y, point.Z);

		/// <summary>
		/// Converts a vector to the sorter's geometry type.
		/// </summary>
		private static Vector3D ToVector(CadVector3D vector) => new Vector3D(vector.X, vector.Y, vector.Z);
	}
}
