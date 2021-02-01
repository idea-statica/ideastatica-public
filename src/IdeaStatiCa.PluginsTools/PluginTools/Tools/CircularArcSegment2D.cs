using CI.Mathematics;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace CI.Geometry2D
{
	/// <summary>
	/// Creates a circular arc between two points in a PolyLine2D.
	/// Comment: Arc is defined by Start point, End point and any others point.
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class CircularArcSegment2D : Segment2D, ICircularArcSegment2D
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the CircularArcSegment2D class.
		/// </summary>
		public CircularArcSegment2D()
		{
		}

		/// <summary>
		/// Initializes a new instance of the CircularArcSegment2D class that has the specified end point and any other point.
		/// </summary>
		/// <param name="endPoint">The end point of the arc.</param>
		/// <param name="point">A point of the circle (between the start point and the end point).</param>
		public CircularArcSegment2D(Point endPoint, Point point)
		{
			EndPoint = endPoint;
			Point = point;
		}

		/// <summary>
		/// Copies the source into a new instance of <c>CircularArcSegment2D</c>.
		/// </summary>
		/// <param name="source">The source.</param>
		public CircularArcSegment2D(CircularArcSegment2D source)
		{
			EndPoint = source.EndPoint;
			Point = source.Point;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets or sets the point of circular arc somewhere between start and end point.
		/// </summary>
		public Point Point { get; set; }

		#endregion Properties

		#region Public static methods

		/// <summary>
		/// Gets angle between <paramref name="bk"/> and <paramref name="b"/>
		/// </summary>
		/// <param name="b">Begin point</param>
		/// <param name="bk">End point</param>
		/// <returns>The value of the angle 0-2*PI</returns>
		public static double Angle_arc(Point b, Point bk)
		{
			Point del = new Point();
			double uh;
			del.X = bk.X - b.X;
			del.Y = bk.Y - b.Y;
			if (del.X == 0.0)
			{
				if (del.X.IsZero(1e-10))
				{
					if (del.Y > 0)
					{
						return MathConstants.PI_2;
					}
					else
					{
						return MathConstants.PI * 1.5;
					}
				}
			}

			uh = Math.Atan(Math.Abs(del.Y / del.X));
			if (del.X <= 0 && del.Y <= 0)
			{
				return MathConstants.PI + uh;
			}

			if (del.X < 0 && del.Y > 0)
			{
				return MathConstants.PI - uh;
			}

			if (del.X > 0 && del.Y < 0)
			{
				return (2 * MathConstants.PI) - uh;
			}

			return uh;
		}

		/// <summary>
		/// Creates a circular arc segment defined by tangents and radius.
		/// </summary>
		/// <param name="p1">The first point of tangent</param>
		/// <param name="p2">The intersection of tangents (the vertex).</param>
		/// <param name="p3">The other point of second tangent.</param>
		/// <param name="radius">The radius of curcular arc.</param>
		/// <param name="validateRounding">Indicates, whether validate rounding, that is inside given points p1, p2 and p3.
		/// If true and rounding is outside given point, then ArgumentException is thrown.</param>
		/// <param name="startPoint">The start point of circular arc segment.</param>
		/// <returns>A new circular arc segment.</returns>
		public static CircularArcSegment2D CreateByTangents(Point p1, Point p2, Point p3, double radius, bool validateRounding, out Point startPoint)
		{
			var v1 = p2 - p1;
			var ptl = GeomTools2D.PointToLine(ref p3, ref p1, ref v1);
			if (ptl == PointRelatedToLine.OnLine)
				throw new NotSupportedException();

			var v2 = p3 - p2;

			var n1 = v1.Perpendicular();
			var n2 = v2.Perpendicular();
			n1.Normalize();
			n2.Normalize();
			n1 *= radius;
			n2 *= radius;

			if (ptl == PointRelatedToLine.LeftSide)
			{
				n1 *= -1;
				n2 *= -1;
			}

			var p11 = p1 + n1;
			var p22 = p2 + n2;

			var centre = GeomTools2D.LineIntersection(p11, v1, p22, v2);
			var c1 = centre - n1;
			var c3 = centre - n2;
			var n12 = n1 + n2;
			n12.Normalize();
			n12 *= radius;
			var c2 = centre - n12;

			if (validateRounding)
			{
				var vv1 = c1 - p1;
				var test = v1 * vv1;
				if (test < 0)
					throw new ArgumentException();

				var vv2 = c3 - p3;
				test = v2 * vv2;
				if (test > 0)
					throw new ArgumentException();
			}

			startPoint = c1;
			return new CircularArcSegment2D(c3, c2);
		}

		/// <summary>
		/// Gets point on the circle
		/// </summary>
		/// <param name="b">Centre point</param>
		/// <param name="r">Radius</param>
		/// <param name="angle">Angle - anticlockwise orientation, zero angle is defined by X axis, value is in rad</param>
		/// <param name="result"></param>
		public static void PointCircle(Point b, double r, double angle, ref Point result)
		{
			result.X = b.X + (r * Math.Cos(angle));
			result.Y = b.Y + (r * Math.Sin(angle));
		}

		#endregion Public static methods

		#region Public methods

		/// <summary>
		/// Gets the centre point of circle.
		/// </summary>
		/// <param name="start">The start point of segment.</param>
		/// <returns>The centre point of this circle.</returns>
		public Point GetCentre(ref Point start)
		{
			Point centre = new Point();
			double c1 = 0.5 * (((start.X * start.X) + (start.Y * start.Y)) - ((Point.X * Point.X) + (Point.Y * Point.Y)));
			double c2 = 0.5 * (((start.X * start.X) + (start.Y * start.Y)) - ((EndPoint.X * EndPoint.X) + (EndPoint.Y * EndPoint.Y)));
			centre.Y = ((c1 * (start.X - EndPoint.X)) - (c2 * (start.X - Point.X)))
				/ (((start.Y - Point.Y) * (start.X - EndPoint.X)) - ((start.Y - EndPoint.Y) * (start.X - Point.X)));
			centre.X = ((c1 * (start.Y - EndPoint.Y)) - (c2 * (start.Y - Point.Y)))
				/ (((start.X - Point.X) * (start.Y - EndPoint.Y)) - ((start.X - EndPoint.X) * (start.Y - Point.Y)));
			return centre;
		}

		/// <summary>
		/// Gets the radius.
		/// </summary>
		/// <param name="start">The start point of segment.</param>
		/// <returns>The radius of this circle.</returns>
		public double GetRadius(ref Point start)
		{
			Vector u = Point.Subtract(start, GetCentre(ref start));
			return u.Length;
		}

		/// <summary>
		/// Gets the angle in degrees.
		/// </summary>
		/// <param name="start">The start point of segment.</param>
		/// <returns>The angle in degrees, always positive value.</returns>
		public double GetAngle(ref Point start)
		{
			Point centre = GetCentre(ref start);
			Vector u1 = Point.Subtract(start, centre);
			Vector u2 = Point.Subtract(EndPoint, centre);
			var s = IsCounterClockwise(ref start);

			// The angle in degrees
			double angle = Vector.AngleBetween(u1, u2) * s;
			return angle < 0 ? 360 + angle : angle;
		}

		/// <summary>
		/// Evaluates, if arc is counter clockwise.
		/// Returns 1, if arc is counter clockwise, -1 if is clockwise.
		/// </summary>
		/// <param name="start">The start point of <c>CircularArcSegment2D</c>.</param>
		/// <returns>1, if arc is counter clockwise, -1 if is clockwise.</returns>
		public int IsCounterClockwise(ref Point start)
		{
			var a = (start.X - EndPoint.X) * (Point.Y - EndPoint.Y) - (start.Y - EndPoint.Y) * (Point.X - EndPoint.X);
			return a > 0 ? 1 : -1;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="beg"></param>
		/// <param name="centr"></param>
		/// <param name="end"></param>
		/// <param name="orient">Orientace oblouku, true po směru hodinových ručiček, false - proti směru hodinových ručiček</param>
		public void SetPoints(Point beg, Point centr, Point end, bool orient)
		{
			Point b2 = new Point();
			Point b3 = new Point();
			ArcBCETo3Points(beg, centr, end, orient, ref b2, ref b3);
			EndPoint = end;
		}

		#endregion Public methods

		#region Segment2D overrides

		/// <summary>
		/// Gets the length of a circular arc segment.
		/// </summary>
		/// <param name="start">The start point of circular arc segment.</param>
		/// <returns>The length of the arc.</returns>
		public override double GetLength(ref Point start)
		{
			// L = fi * r
			return GetAngle(ref start) * MathConstants.PI / 180 * GetRadius(ref start);
		}

		public override Rect GetBounds(ref Point start)
		{
			var centre = GetCentre(ref start);
			var angle = GetAngle(ref start);
			if (Comparators.InIntervalDown(start.X, EndPoint.X, centre.X) &&
				Comparators.InIntervalDown(start.Y, EndPoint.Y, centre.Y))
			{
				if (angle < 180)
				{
					return new Rect(start, EndPoint);
				}
				////else
				////{
				////    var r = GetRadius(ref start);
				////    return new Rect(centre.X - r, centre.Y - r, 2 * r, 2 * r);
				////}
			}

			var min = new Point(double.PositiveInfinity, double.PositiveInfinity);
			var max = new Point(double.NegativeInfinity, double.NegativeInfinity);
			CheckMinMax(ref start, ref min, ref max);
			Point ep = EndPoint;
			CheckMinMax(ref ep, ref min, ref max);

			double a = 5;
			angle = Math.Abs(angle);

			if (a < angle)
			{
				int numberOfTiles = (int)Math.Ceiling(angle / a);
				double rel = a / angle;

				for (int i = 1; i < numberOfTiles; ++i)
				{
					var pt = GetPointOnSegment(ref start, i * rel);
					CheckMinMax(ref pt, ref min, ref max);
				}
			}

			return new Rect(min, max);
		}

		public override Point GetPointOnSegment(ref Point start, double relPosition)
		{
			double radius = GetRadius(ref start);
			double angle = GetLength(ref start) * relPosition / radius * IsCounterClockwise(ref start) * 180 / Math.PI;
			Point centre = GetCentre(ref start);

			var mat = new Matrix();
			mat.RotateAt(angle, centre.X, centre.Y);
			return mat.Transform(start);
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
			double radius = GetRadius(ref start);
			double angle = GetLength(ref start) * relPosition / radius * IsCounterClockwise(ref start) * 180 / Math.PI;
			Point centre = GetCentre(ref start);

			Vector v = Point.Subtract(start, centre);
			v = -IsCounterClockwise(ref start) * v.Perpendicular();

			var mat = new Matrix();
			mat.Rotate(angle);
			v = mat.Transform(v);
			v.Normalize();
			return v;
		}

		/// <summary>
		/// Gets a functional values in defined x position of the CircularArcSegment.
		/// </summary>
		/// <param name="start">The start point of the circular arc segment.</param>
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

			var bounds = GetBounds(ref start);
			if (Comparators.InIntervalBoth(bounds.Left, bounds.Right, x, tolerance))
			{
				var radius = GetRadius(ref start);
				var centre = GetCentre(ref start);
				x -= centre.X;
				var y = Math.Sqrt(radius * radius - x * x);
				var c1 = Comparators.InIntervalBoth(bounds.Top, bounds.Bottom, centre.Y + y, tolerance);
				var c2 = Comparators.InIntervalBoth(bounds.Top, bounds.Bottom, centre.Y - y, tolerance);
				if (c1 && c2)
				{
					return new double[]
					{
						global ? centre.Y + y : centre.Y + y - start.Y,
						global ? centre.Y - y : centre.Y - y - start.Y,
					};
				}
				else if (c1)
				{
					return new double[]
					{
						global ? centre.Y + y : centre.Y + y - start.Y,
					};
				}
				else if (c2)
				{
					return new double[]
					{
						global ? centre.Y - y : centre.Y - y - start.Y,
					};
				}
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

			var bounds = GetBounds(ref start);
			if (Comparators.InIntervalBoth(bounds.Top, bounds.Bottom, y, tolerance))
			{
				var radius = GetRadius(ref start);
				var centre = GetCentre(ref start);
				y -= centre.Y;
				var x = Math.Sqrt(radius * radius - y * y);
				var c1 = Comparators.InIntervalBoth(bounds.Left, bounds.Right, centre.X + x, tolerance);
				var c2 = Comparators.InIntervalBoth(bounds.Left, bounds.Right, centre.X - x, tolerance);
				if (c1 && c2)
				{
					return new double[]
					{
						global ? centre.X + x : centre.X + x - start.X,
						global ? centre.X - x : centre.X - x - start.X,
					};
				}
				else if (c1)
				{
					return new double[]
					{
						global ? centre.X + x : centre.X + x - start.X
					};
				}
				else if (c2)
				{
					return new double[]
					{
						global ? centre.X - x : centre.X - x - start.X
					};
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
		/// <returns>The relative position.</returns>
		public override double[] GetRelativePosition(ref Point start, double x, bool global, double tolerance = 0)
		{
			if (!global)
			{
				x += start.X;
			}

			var radius = GetRadius(ref start);
			var centre = GetCentre(ref start);
			if (Comparators.InIntervalBoth(centre.X - radius, centre.X + radius, x, tolerance))
			{
				var xx = x - centre.X;
				var y = (radius * radius - xx * xx).IsZero() ? 0 : Math.Sqrt(radius * radius - xx * xx);
				var cc = IsCounterClockwise(ref start);
				var angle = GetAngle(ref start);
				Func<Point, Point, double> getAngle = (s, p) =>
				{
					var u1 = Point.Subtract(s, centre);
					var u2 = Point.Subtract(p, centre);
					var a = Vector.AngleBetween(u1, u2) * cc;
					if (a.IsZero())
						return 0;

					return a < 0 ? 360 + a : a;
				};
				var px = new Point(x, centre.Y + y);
				var angle1 = getAngle(start, px);
				px.Y = centre.Y - y;
				var angle2 = getAngle(start, px);
				return new double[] { angle1 / angle, angle2 / angle };
			}

			return new double[0];
		}

		/// <summary>
		/// Calculates a curvature in specified x position.
		/// </summary>
		/// <param name="start">The start point.</param>
		/// <param name="x">The x coordinate.</param>
		/// <returns>The curvature.</returns>
		public override double GetCurvature(ref Point start, double x)
		{
			return 1 / GetRadius(ref start);
		}

		#endregion Segment2D overrides

		#region ICloneable Members

		public override object Clone()
		{
			return new CircularArcSegment2D(this);
		}

		#endregion ICloneable Members

		#region Private methods

		private bool ArcBCETo3Points(Point beg, Point centr, Point end, bool orient, ref Point b2, ref Point b3)
		{
			double r, zac = 0.0, uh = 0.0;
			Point centreN = new Point();
			r = ArcFromBCE(beg, centr, end, orient, ref centreN, ref zac, ref uh);
			PointCircle(centreN, r, zac + (uh / 2), ref b2);
			if (orient)
			{
				PointCircle(centreN, r, zac + uh, ref b3);
			}
			else
			{
				PointCircle(centreN, r, zac, ref b3);
			}

			Point = b2;
			return true;
		}

		private double ArcFromBCE(Point b1, Point b2, Point b3, bool orientation, ref Point str, ref double zac, ref double angle)
		{
			str = b2;
			zac = Angle_arc(str, b1);
			angle = Angle_arc(str, b3);
			angle -= zac;
			if (angle < 0)
			{
				angle += 2 * MathConstants.PI;
			}

			if (!orientation)
			{
				zac += angle;
				angle = (2 * MathConstants.PI) - angle;
			}

			Vector u = Point.Subtract(b1, str);
			return u.Length;
		}

		private static void CheckMinMax(ref Point pt, ref Point min, ref Point max)
		{
			if (pt.X < min.X)
			{
				min.X = pt.X;
			}

			if (pt.X > max.X)
			{
				max.X = pt.X;
			}

			if (pt.Y < min.Y)
			{
				min.Y = pt.Y;
			}

			if (pt.Y > max.Y)
			{
				max.Y = pt.Y;
			}
		}

		#endregion Private methods
	}
}