using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Provides an interface of segments 2D.
	/// </summary>
	[ComVisible(false)]
	public interface ISegment2D : ICloneable
	{
		/// <summary>
		/// Gets or sets the endpoint of the segment.
		/// </summary>
		IdaComPoint2D EndPoint { get; set; }

		/// <summary>
		/// Gets the length of a segment.
		/// </summary>
		/// <param name="start">The start point of segment.</param>
		/// <returns>The lenght of segment.</returns>
		double GetLength(ref Point start);

		/// <summary>
		/// Gets the rectangular boundary of a segment.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <returns>The boundary of segment.</returns>
		Rect GetBounds(ref Point start);

		/// <summary>
		/// Gets a point on this segment defined by relative position.
		/// This method doesn't checks, if relative position is in range (0, 1).
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="relPosition">The relative position on the segment.</param>
		/// <returns>A point on the segment.</returns>
		Point GetPointOnSegment(ref Point start, double relPosition);

		/// <summary>
		/// Gets a tangent on point on this segment defined by relative position.
		/// This method doesn't checks, if relative position is in range (0, 1).
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="relPosition">The relative position on the segment.</param>
		/// <returns>A tangent on point on the segment.</returns>
		Vector GetTangentOnSegment(ref Point start, double relPosition);

		/// <summary>
		/// Gets a functional values in defined x position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="x">The x position.</param>
		/// <param name="global">True, if x and returned y value is are global space, false, if x and y are related to the start point.</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>A values related to the x position.</returns>
		double[] GetValueByX(ref Point start, double x, bool global, double tolerance);

		/// <summary>
		/// Gets a functional values in defined y position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="y">The y position.</param>
		/// <param name="global">True, if y and returned x value is are global space, false, if y and x are related to the start point.</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>A values related to the y position.</returns>
		double[] GetValueByY(ref Point start, double y, bool global, double tolerance = 0);

		/// <summary>
		/// Calculates relative position on segment by given x position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="x">The x position.</param>
		/// <param name="global">True, if x and returned y value is are global space, false, if x and y are related to the start point.</param>
		/// <param name="tolerance">Distance tolerance.</param>
		/// <returns>The relative position.</returns>
		double[] GetRelativePosition(ref Point start, double x, bool global, double tolerance = 0);

		/// <summary>
		/// Gets a curvature in specified x position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="x">The x position.</param>
		/// <returns>The curvature.</returns>
		double GetCurvature(ref Point start, double x);

		/// <summary>
		/// Reverse segment orientation.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <returns>A new start point.</returns>
		Point Reverse(ref Point start);
	}

	/// <summary>
	/// Provides an interface of segments 2D COM visible.
	/// </summary>
	[ComVisible(true)]
	public interface ISegment2DCom
	{
		/// <summary>
		/// Gets or sets the endpoint of the segment.
		/// </summary>
		IdaComPoint2D EndPoint { get; set; }
	}
}
