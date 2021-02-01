using System;
using System.Reflection;
using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Creates a line between two points in a PolyLine2D.
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class LineSegment2D : Segment2D, ILineSegment2D
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the LineSegment2D class.
		/// </summary>
		public LineSegment2D()
		{
		}

		/// <summary>
		/// Initializes a new instance of the LineSegment2D class that has the specified end point.
		/// </summary>
		/// <param name="point">The end point of this LineSegment2D.</param>
		public LineSegment2D(Point point)
		{
			EndPoint = point;
		}

		/// <summary>
		/// Copies the source into a new instance of <c>LineSegment2D</c>.
		/// </summary>
		/// <param name="source">The source.</param>
		public LineSegment2D(LineSegment2D source)
		{
			EndPoint = source.EndPoint;
		}

		#endregion Constructors

		#region Segment2D overrides

		/// <summary>
		/// Gets the length of a line segment.
		/// </summary>
		/// <param name="start">The start point of a line segment.</param>
		/// <returns>The lenght of line segment.</returns>
		public override double GetLength(ref Point start)
		{
			Vector v = Point.Subtract(EndPoint, start);
			return v.Length;
		}

		/// <summary>
		/// Gets the rectangular boundary of a line segment.
		/// </summary>
		/// <param name="start">The start point of the line segment.</param>
		/// <returns>The boundary of line segment.</returns>
		public override Rect GetBounds(ref Point start)
		{
			return new Rect(start, EndPoint);
		}

		/// <summary>
		/// Gets a point on LineSegment2D segment defined by relative position.
		/// This method doesn't checks, if relative position is in range (0, 1).
		/// </summary>
		/// <param name="start">The start point of the LineSegment2D.</param>
		/// <param name="relPosition">The relative position on the LineSegment2D.</param>
		/// <returns>A point on the LineSegment2D.</returns>
		public override Point GetPointOnSegment(ref Point start, double relPosition)
		{
			Vector v = Point.Subtract(EndPoint, start);
			return Point.Add(start, v * relPosition);
		}

		/// <summary>
		/// Gets a tangent on point on this segment defined by relative position.
		/// This method doesn't checks, if relative position is in range (0, 1).
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="relPosition">The relative position on the segment.</param>
		/// <returns>A tangent on point on the segment.</returns>
		public override Vector GetTangentOnSegment(ref Point start, double relPosition)
		{
			Vector v = Point.Subtract(EndPoint, start);
			v.Normalize();
			return v;
		}

		/// <summary>
		/// Gets a functional values in defined x position of the LineSegment.
		/// </summary>
		/// <param name="start">The start point of the line segment.</param>
		/// <param name="x">The x position.</param>
		/// <param name="global">True, if x and returned y value is are global space, false, if x and y are related to the start point.</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>A values related to the x position.</returns>
		public override double[] GetValueByX(ref Point start, double x, bool global, double tolerance = 0)
		{
			if (!global)
			{
				x += start.X;
			}

			if (CI.Mathematics.Comparators.InIntervalBoth(start.X, EndPoint.X, x, tolerance))
			{
				var v = Point.Subtract(EndPoint, start);
				var y = start.Y + v.Y / v.X * (x - start.X);
				if (!global)
				{
					y -= start.Y;
				}

				return new double[] { y };
			}

			return new double[0];
		}

		/// <summary>
		/// Gets a functional values in defined y position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="y">The y position.</param>
		/// <param name="global">True, if y and returned x value is are global space, false, if y and x are related to the start point.</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>A values related to the y position.</returns>
		public override double[] GetValueByY(ref Point start, double y, bool global, double tolerance = 0)
		{
			if (!global)
			{
				y += start.Y;
			}

			if (CI.Mathematics.Comparators.InIntervalBoth(start.Y, EndPoint.Y, y, tolerance))
			{
				var v = Point.Subtract(EndPoint, start);
				if (!v.Y.IsZero(1e-5))
				{
					var x = start.X + v.X / v.Y * (y - start.Y);
					if (!global)
					{
						x -= start.X;
					}

					return new double[] { x };
				}
			}

			return new double[0];
		}

		/// <summary>
		/// Calculates relative position on segment by given x position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="x">The x position.</param>
		/// <param name="global">True, if x and returned y value is are global space, false, if x and y are related to the start point.</param>
		/// <param name="tolerance">Distance tolerance.</param>
		/// <returns>The relative position.</returns>
		public override double[] GetRelativePosition(ref Point start, double x, bool global, double tolerance = 0)
		{
			if (!global)
			{
				x += start.X;
			}

			var v = Point.Subtract(EndPoint, start);
			var y = start.Y + v.Y / v.X * (x - start.X);
			var v1 = Point.Subtract(new Point(x, y), start);

			var r = v1.Length / v.Length;
			return new double[] { x < Math.Min(start.X, EndPoint.X) ? -r : r };
		}

		/// <summary>
		/// Calculates a curvature in specified x position.
		/// </summary>
		/// <param name="start">The start point.</param>
		/// <param name="x">The x coordinate.</param>
		/// <returns>The curvature.</returns>
		public override double GetCurvature(ref Point start, double x)
		{
			return 0;
		}

		#endregion Segment2D overrides

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new LineSegment2D(this);
		}

		#endregion ICloneable Members
	}
}