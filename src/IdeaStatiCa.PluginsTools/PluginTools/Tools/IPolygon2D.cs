using System;
using System.Collections.Generic;
using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Represents a collection of <c>System.Windows.Point</c> that can be individually accessed by index.
	/// </summary>
	public interface IPolygon2D : IList<Point>, ICloneable
	{
		/// <summary>
		/// Gets a value that specifies whether first and last points are connected.
		/// </summary>
		bool IsClosed { get; }

		/// <summary>
		/// Gets the area of this polygon.
		/// </summary>
		double Area { get; }

		/// <summary>
		/// Gets the length of polygon.
		/// </summary>
		double Length { get; }

		/// <summary>
		/// Gets the rectangular boundary of a <c>CI.Geometry2D.IPolygon2D</c>.
		/// </summary>
		Rect Bounds { get; }

		/// <summary>
		/// Gets an information that specifies whether the orientation of polygon is counterclockwise.
		/// </summary>
		bool IsCounterClockwise { get; }

		/// <summary>
		/// Makes the polygon closed (1'st and last point equals).
		/// </summary>
		void Close();

		/// <summary>
		/// Adds the elements of the specified collection to the end of the <c>Polygon2D</c>.
		/// </summary>
		/// <param name="collection">The collection whose elements should be added to the end of the <c>Polygon2D</c>.
		/// The collection itself cannot be null.</param>
		/// <exception cref="System.ArgumentNullException">The collection is null.</exception>
		void AddRange(IEnumerable<Point> collection);

		/// <summary>
		/// Remove collinear and duplicate points.
		/// </summary>
		/// <param name="tolerance"></param>
		void Simplify(double tolerance = 1e-12);

		/// <summary>
		/// Reverses the order of the elements in the entire <c>IPolygon2D</c>.
		/// </summary>
		void Reverse();

		/// <summary>
		/// Removes duplicate points.
		/// </summary>
		void RemoveDuplicateConsecutivePoints();

		/// <summary>
		/// Compare points of polygon with <paramref name="pattern"/>.
		/// </summary>
		/// <param name="pattern">Polygon to be compared with this instance.</param>
		/// <param name="tolerance">Tolerance used in comparison of coordinates</param>
		/// <returns>True if all points are equal.</returns>
		bool EqualPoints(IPolygon2D pattern, double tolerance);
	}
}
