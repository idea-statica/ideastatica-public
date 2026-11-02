using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Provides functionality to discretize a segment to polygon.
	/// </summary>
	internal interface ISegment2DDiscretizator
	{
		/// <summary>
		/// Divide a segment on straight lines polygon.
		/// </summary>
		/// <param name="start">The start point of segment.</param>
		/// <param name="source">The segment to discretization.</param>
		/// <param name="target">The <c>Polygon2D</c> as target of discretization.</param>
		void Discretize(Point start, ISegment2D source, IPolygon2D target);
	}
}
