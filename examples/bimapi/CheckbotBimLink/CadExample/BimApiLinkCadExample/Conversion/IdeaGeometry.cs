using CadExampleApi;
using IdeaRS.OpenModel.Geometry2D;
using IdeaRS.OpenModel.Geometry3D;

namespace BimApiLinkCadExample.Conversion
{
	/// <summary>
	/// Converts CAD geometry to the geometry types of the IDEA StatiCa Open Model.
	/// </summary>
	internal static class IdeaGeometry
	{
		/// <summary>
		/// The axes of a coordinate system. The origin is passed separately, as the node the object refers to.
		/// </summary>
		public static CoordSystemByVector ToIdea(this CadCoordinateSystem coordinateSystem)
		{
			return new CoordSystemByVector
			{
				VecX = coordinateSystem.X.ToIdea(),
				VecY = coordinateSystem.Y.ToIdea(),
				VecZ = coordinateSystem.Z.ToIdea(),
			};
		}

		/// <summary>
		/// The outline as a region in the XY plane of its coordinate system. Straight segments only; an outline with
		/// arcs would need <see cref="ArcSegment2D"/>.
		/// </summary>
		public static Region2D ToIdea(this CadOutline2D outline)
		{
			var polyLine = new PolyLine2D { StartPoint = outline.Points[0].ToIdea() };
			for (int i = 1; i < outline.Points.Count; i++)
			{
				polyLine.Segments.Add(new LineSegment2D { EndPoint = outline.Points[i].ToIdea() });
			}

			return new Region2D { Outline = polyLine };
		}

		/// <summary>
		/// Converts a vector to the Open Model type.
		/// </summary>
		private static Vector3D ToIdea(this CadVector3D vector) => new Vector3D { X = vector.X, Y = vector.Y, Z = vector.Z };

		/// <summary>
		/// Converts a point in a plane to the Open Model type.
		/// </summary>
		private static Point2D ToIdea(this CadPoint2D point) => new Point2D { X = point.X, Y = point.Y };
	}
}
