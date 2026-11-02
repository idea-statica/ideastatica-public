using CI.GiCL2D;
using CI.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CI.Geometry2D
{
	/// <summary>
	/// The position of point related to the line.
	/// </summary>
	public enum PointRelatedToLine
	{
		/// <summary>
		/// The point is on the right side from the line (line direction is given by vector).
		/// </summary>
		RightSide,

		/// <summary>
		/// The point is on the left side from the line.
		/// </summary>
		LeftSide,

		/// <summary>
		/// The point lies on the line.
		/// </summary>
		OnLine,
	}

	public enum IntersectionType
	{
		Closest,
		Outermost,
	}

	/// <summary>
	/// Provides geometric operations in geometry 2D.
	/// </summary>
	public static class GeomTools2D
	{
		#region Line operations

		public static double LineToPointDistance(ref Point a, ref Vector ua, ref Point b)
		{
			var na = ua.Perpendicular();
			var dist = Math.Abs(na.X * (b.X - a.X) + na.Y * (b.Y - a.Y)) / na.Length;
			return dist;
		}

		/// <summary>
		/// Find intersection of 2 lines in absolute coordinates. There is no any validation of result!!!
		/// </summary>
		/// <param name="a">The point of 1'st line.</param>
		/// <param name="ua">The direction of 1'st line.</param>
		/// <param name="b">The point of 2'nd line.</param>
		/// <param name="ub">The direction of 2'nd line.</param>
		/// <returns>The intersect point.</returns>
		public static Point LineIntersection(Point a, Vector ua, Point b, Vector ub)
		{
			// ub.X * ua.Y - ua.X * ub.Y;
			double t = Vector.CrossProduct(ub, ua);
			var x = a.X + (ua.X * ((ub.Y * (a.X - b.X)) - (ub.X * (a.Y - b.Y))) / t);
			var y = a.Y + (ua.Y * ((ub.Y * (a.X - b.X)) - (ub.X * (a.Y - b.Y))) / t);
			return new Point(x, y);
		}

		/// <summary>
		/// Find intersection of 2 lines in absolute coordinates.
		/// </summary>
		/// <param name="a">The point of 1'st line.</param>
		/// <param name="ua">The direction of 1'st line.</param>
		/// <param name="b">The point of 2'nd line.</param>
		/// <param name="ub">The direction of 2'nd line.</param>
		/// <param name="result">The intersect point.</param>
		/// <param name="tolerance">The value of tolerance</param>
		/// <returns>False if lines are collinear.</returns>
		public static IntersectionInfo LineIntersection(Point a, Vector ua, Point b, Vector ub, out Point result, double tolerance = 1e-12)
		{
			result = new Point();

			// ub.X * ua.Y - ua.X * ub.Y;
			double t = Vector.CrossProduct(ub, ua);
			if (t.IsZero(tolerance))
			{
				return IntersectionInfo.NoIntersection;
			}

			result.X = a.X + (ua.X * ((ub.Y * (a.X - b.X)) - (ub.X * (a.Y - b.Y))) / t);
			result.Y = a.Y + (ua.Y * ((ub.Y * (a.X - b.X)) - (ub.X * (a.Y - b.Y))) / t);

			if (Comparators.InIntervalBoth(a.X, a.X + ua.X, result.X, tolerance) &&
				Comparators.InIntervalBoth(b.X, b.X + ub.X, result.X, tolerance) &&
				Comparators.InIntervalBoth(a.Y, a.Y + ua.Y, result.Y, tolerance) &&
				Comparators.InIntervalBoth(b.Y, b.Y + ub.Y, result.Y, tolerance))
			{
				return IntersectionInfo.IntersectionInside;
			}
			else if (Comparators.InIntervalBoth(a.X, a.X + ua.X, result.X, tolerance) &&
				Comparators.InIntervalBoth(a.Y, a.Y + ua.Y, result.Y, tolerance))
			{
				return IntersectionInfo.IntersectionFirst;
			}
			else if (Comparators.InIntervalBoth(b.X, b.X + ub.X, result.X, tolerance) &&
				Comparators.InIntervalBoth(b.Y, b.Y + ub.Y, result.Y, tolerance))
			{
				return IntersectionInfo.IntersectionSecond;
			}

			return IntersectionInfo.IntersectionOutside;
		}

		/// <summary>
		/// Determines which side of a line the point p lies.
		/// </summary>
		/// <param name="p">The point p.</param>
		/// <param name="linePt">The point of a line.</param>
		/// <param name="lineDir">The direction of a line.</param>
		/// <param name="tolerance">The tolerance of calculation.</param>
		/// <returns>
		/// RightSide for a point on the right side
		/// OnLine for a point on the line
		/// LeftSide for a point on the left side
		/// </returns>
		public static PointRelatedToLine PointToLine(ref Point p, ref Point linePt, ref Vector lineDir, double tolerance = 1e-32)
		{
			double equation = (lineDir.Y * (p.X - linePt.X)) - (lineDir.X * (p.Y - linePt.Y));
			if (equation.IsZero(tolerance))
			{
				return PointRelatedToLine.OnLine;
			}
			else if (equation > 0)
			{
				return PointRelatedToLine.RightSide;
			}
			else
			{
				return PointRelatedToLine.LeftSide;
			}
		}

		/// <summary>
		/// Determines where <paramref name="testedPt"/> is located on polyline <paramref name="polyline"/>
		/// Method expects only line segments.
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="testedPt">Tested point</param>
		/// <param name="precision">Precision</param>
		/// <returns>True if point is on polyline</returns>
		public static bool IsPointOnPolyline(IPolyLine2D polyline, Point testedPt, double precision = -1e-5)
		{
			Point prevPoint = polyline.StartPoint;
			Point segBegin;
			Point segEnd;
			foreach (var seg in polyline.Segments)
			{
				segBegin = prevPoint;
				segEnd = seg.EndPoint;
				prevPoint = segEnd;

				if (IsPointOnLine(ref segBegin, ref segEnd, ref testedPt, precision))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether <paramref name="testedPt"/> is located on the line given by points <paramref name="lineBeginPt"/> and <paramref name="lineEndPt"/>
		/// </summary>
		/// <param name="lineBeginPt">Begin point of the line</param>
		/// <param name="lineEndPt">End point of the line</param>
		/// <param name="testedPt">Tested point</param>
		/// <param name="precision">Precision</param>
		/// <returns>True if point is located on the line</returns>
		public static bool IsPointOnLine(ref Point lineBeginPt, ref Point lineEndPt, ref Point testedPt, double precision = -1e-5)
		{
			if (lineBeginPt.IsEqualWithTolerance(testedPt, precision) ||
				lineEndPt.IsEqualWithTolerance(testedPt, precision))
			{
				return true;
			}

			var v = lineEndPt - lineBeginPt;
			var v1 = testedPt - lineBeginPt;

			if (v.Length.IsZero(precision))
			{
				return false;
			}

			var d = (v1 * v) / v.Length;
			if (d.IsLesser(0.0) || d.IsGreater(v.Length))
			{
				return false;
			}

			var p = lineBeginPt + (v * (d / v.Length));

			return (p.IsEqualWithTolerance(testedPt, precision));

			//return (
			//	(((testedPt.X.IsGreater(lineBeginPt.X)) && (testedPt.X.IsLesserOrEqual(lineEndPt.X))) ||
			//	((testedPt.X.IsGreaterOrEqual(lineEndPt.X)) && (testedPt.X.IsLesserOrEqual(lineBeginPt.X)))) &&
			//	(((testedPt.Y.IsGreater(lineBeginPt.Y)) && (testedPt.Y.IsLesserOrEqual(lineEndPt.Y))) ||
			//	((testedPt.Y.IsGreaterOrEqual(lineEndPt.Y)) && (testedPt.Y.IsLesserOrEqual(lineBeginPt.Y)))) &&
			//	(Math.Abs((lineEndPt.X - lineBeginPt.X) * (testedPt.Y - lineBeginPt.Y)) - ((testedPt.X - lineBeginPt.X) * (lineEndPt.Y - lineBeginPt.Y))) < precision);
		}

		/// <summary>
		/// Determines if the point p lies on abscissa.
		/// </summary>
		/// <param name="p">The point p.</param>
		/// <param name="linePt1">The first point of a line.</param>
		/// <param name="linePt2">The second point of a line.</param>
		/// <param name="tolerance">The tolerance of calculation.</param>
		/// <returns>bool</returns>
		public static bool PointToAbscissa(ref Point p, ref Point linePt1, ref Point linePt2, double tolerance = 1e-32)
		{
			var vect = Point.Subtract(linePt2, linePt1);
			if (PointToLine(ref p, ref linePt1, ref vect, tolerance) == PointRelatedToLine.OnLine)
			{
				if (Comparators.InIntervalBoth(Math.Min(linePt1.X, linePt2.X), Math.Max(linePt1.X, linePt2.X), p.X, tolerance) &&
					Comparators.InIntervalBoth(Math.Min(linePt1.Y, linePt2.Y), Math.Max(linePt1.Y, linePt2.Y), p.Y, tolerance))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Check positions of two lines
		/// </summary>
		/// <param name="ls1">The start point of the first line</param>
		/// <param name="le1">The end point of the first line</param>
		/// <param name="ls2">The start point of the second line</param>
		/// <param name="le2">The end point of the second line</param>
		/// <param name="tolerance">The tolerance of the check</param>
		/// <returns>True if lines are at the same position</returns>
		public static bool AreLinesAtSamePos(System.Windows.Point ls1, System.Windows.Point le1, System.Windows.Point ls2, System.Windows.Point le2, double tolerance = 1e-6)
		{
			if (ls1.IsEqualWithTolerance(ls2, tolerance))
			{
				if (le1.IsEqualWithTolerance(le2, tolerance))
				{
					return true;
				}
			}

			if (le1.IsEqualWithTolerance(ls2, tolerance))
			{
				if (ls1.IsEqualWithTolerance(le2, tolerance))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Finds intersections of line defined by point and vector and circle defined by centre and radius.
		/// </summary>
		/// <param name="a">The point of line.</param>
		/// <param name="ua">The direction of line.</param>
		/// <param name="centre">The centre point of circle.</param>
		/// <param name="radius">The radius of circle.</param>
		/// <param name="result">The intersections, if no intersection, then null.</param>
		/// <returns>The intersection info.</returns>
		public static IntersectionInfo LineCircleIntersection(Point a, Vector ua, Point centre, double radius, out Point[] result, double tolerance = 1e-12)
		{
			a = (Point)Point.Subtract(a, centre);
			var ls = ua.LengthSquared;
			var d = a.X * (a.Y + ua.Y) - (a.X + ua.X) * a.Y;
			var incidence = radius * radius * ls - d * d;
			if (incidence < 0)
			{
				result = null;
				return IntersectionInfo.NoIntersection;
			}

			incidence = Math.Sqrt(incidence);
			var p1 = new Vector
			{
				X = (+d * ua.Y + (ua.Y < 0 ? -1 : 1) * ua.X * incidence) / ls,
				Y = (-d * ua.X + Math.Abs(ua.Y) * incidence) / ls
			};
			var p2 = new Vector
			{
				X = (+d * ua.Y - (ua.Y < 0 ? -1 : 1) * ua.X * incidence) / ls,
				Y = (-d * ua.X - Math.Abs(ua.Y) * incidence) / ls
			};

			result = new Point[]
			{
				Point.Add(centre, p1),
				Point.Add(centre, p2),
			};

			if (Comparators.InIntervalBoth(a.X, a.X + ua.X, p1.X, tolerance) &&
				Comparators.InIntervalBoth(a.X, a.X + ua.X, p2.X, tolerance) &&
				Comparators.InIntervalBoth(a.Y, a.Y + ua.Y, p1.Y, tolerance) &&
				Comparators.InIntervalBoth(a.Y, a.Y + ua.Y, p2.Y, tolerance))
			{
				return IntersectionInfo.IntersectionInside;
			}

			return IntersectionInfo.IntersectionOutside;
		}

		public static IntersectionInfo LineCircularArcIntersection(Point a, Vector ua, Point start, CircularArcSegment2D arc, out Point[] result)
		{
			var circResult = LineCircleIntersection(a, ua, arc.GetCentre(ref start), arc.GetRadius(ref start), out Point[] circInters);
			if (circResult != IntersectionInfo.NoIntersection)
			{
				var inters = new List<Point>(2);
				foreach (var p in circInters)
				{
					var v0 = p - start;
					var v1 = (arc.EndPoint - start).Perpendicular();
					var d = Vector.Multiply(v0, v1);
					if (d.IsGreaterOrEqual(0))
					{
						inters.Add(p);
					}
				}

				if (inters.Any())
				{
					result = inters.ToArray();
					return IntersectionInfo.Intersection;
				}
			}

			result = null;
			return IntersectionInfo.NoIntersection;
		}

		public static IntersectionInfo CircleCircleIntersection(Point centre0, double radius0, Point centre1, double radius1, out Point[] result)
		{
			// Find the distance between the centers.
			var d = centre0 - centre1;
			var dist = d.Length;

			// See how many solutions there are.
			if (dist > radius0 + radius1)
			{
				// No solutions, the circles are too far apart.
				result = null;
				return IntersectionInfo.NoIntersection;
			}
			else if (dist < Math.Abs(radius0 - radius1))
			{
				// No solutions, one circle contains the other.
				result = null;
				return IntersectionInfo.NoIntersection;
			}
			else if ((dist == 0) && (radius0 == radius1))
			{
				// No solutions, the circles coincide.
				result = null;
				return IntersectionInfo.NoIntersection;
			}
			else
			{
				// Find a and h.
				double a = (radius0 * radius0 -
					radius1 * radius1 + dist * dist) / (2 * dist);
				double h = Math.Sqrt(radius0 * radius0 - a * a);

				// Find P2.
				double cx2 = centre0.X + a * (centre1.X - centre0.X) / dist;
				double cy2 = centre0.Y + a * (centre1.Y - centre0.Y) / dist;

				// Get the points P3.
				var intersection1 = new Point(
					(float)(cx2 + h * (centre1.Y - centre0.Y) / dist),
					(float)(cy2 - h * (centre1.X - centre0.X) / dist));

				// See if we have 1 or 2 solutions.
				if (dist == radius0 + radius1)
				{
					result = new Point[] { intersection1 };
					return IntersectionInfo.Intersection;
				}

				var intersection2 = new Point(
					(float)(cx2 - h * (centre1.Y - centre0.Y) / dist),
					(float)(cy2 + h * (centre1.X - centre0.X) / dist));

				result = new Point[] { intersection1, intersection2 };
				return IntersectionInfo.IntersectionInside;
			}
		}

		public static IntersectionInfo CircleCircularArcIntersection(Point centre, double radius, Point start, CircularArcSegment2D arc, out Point[] result)
		{
			var circResult = CircleCircleIntersection(centre, radius, arc.GetCentre(ref start), arc.GetRadius(ref start), out Point[] circInters);
			if (circResult != IntersectionInfo.NoIntersection)
			{
				var cc = arc.IsCounterClockwise(ref start);
				var angle = arc.GetAngle(ref start);
				Func<Point, Point, double> getAngle = (s, p) =>
				{
					var u1 = Point.Subtract(s, centre);
					var u2 = Point.Subtract(p, centre);
					var a = Vector.AngleBetween(u1, u2) * cc;
					return a < 0 ? 360 + a : a;
				};

				var inters = new List<Point>(2);
				foreach (var p in circInters)
				{
					var a = getAngle(start, p);
					if (a.IsLesserOrEqual(angle))
						inters.Add(p);
				}

				if (inters.Any())
				{
					result = inters.ToArray();
					return IntersectionInfo.IntersectionInside;
				}
			}

			result = null;
			return IntersectionInfo.NoIntersection;
		}

		#endregion Line operations

		#region Convertors real geometry to discretize polygon

		/// <summary>
		/// Creates region 2D
		/// </summary>
		/// <param name="outline">Region outline</param>
		/// <returns>New instance of region 2D</returns>
		public static IRegion2D CreateRegion2D(IPolyLine2D outline)
		{
			return new Region2D(outline);
		}

		/// <summary>
		/// Creates circle as region 2D
		/// </summary>
		/// <param name="x">Centre X</param>
		/// <param name="y">Centre Y</param>
		/// <param name="r">Radius</param>
		/// <returns>New instance of region 2D</returns>
		public static IRegion2D CreateCircleRegion(double x, double y, double r)
		{
			Point b0 = new Point(-r, 0);
			Point b1 = new Point(0, r);
			Point b2 = new Point(r, 0);
			Point b3 = new Point(0, -r);
			b0.X += x;
			b0.Y += y;
			b1.X += x;
			b1.Y += y;
			b2.X += x;
			b2.Y += y;
			b3.X += x;
			b3.Y += y;

			CircularArcSegment2D cr0 = new CircularArcSegment2D(b2, b1);
			CircularArcSegment2D cr1 = new CircularArcSegment2D(b0, b3);

			IRegion2D reg = new Region2D
			{
				Outline = new PolyLine2D
				{
					StartPoint = b0
				}
			};
			reg.Outline.Segments.Add(cr0);
			reg.Outline.Segments.Add(cr1);
			reg.Outline.Close();

			return reg;
		}

		public static IRegion2D CreateSquareRegion(double x, double y, double r)
		{
			Point b0 = new Point(-r, r);
			Point b1 = new Point(r, r);
			Point b2 = new Point(r, -r);
			Point b3 = new Point(-r, -r);
			b0.X += x;
			b0.Y += y;
			b1.X += x;
			b1.Y += y;
			b2.X += x;
			b2.Y += y;
			b3.X += x;
			b3.Y += y;

			LineSegment2D cr1 = new LineSegment2D(b1);
			LineSegment2D cr2 = new LineSegment2D(b2);
			LineSegment2D cr3 = new LineSegment2D(b3);

			IRegion2D reg = new Region2D
			{
				Outline = new PolyLine2D
				{
					StartPoint = b0
				}
			};
			reg.Outline.Segments.Add(cr1);
			reg.Outline.Segments.Add(cr2);
			reg.Outline.Segments.Add(cr3);
			reg.Outline.Close();

			return reg;
		}

		public static IPolyLine2D CreatePolyLine2D(IPolyLine2D source = null)
		{
			var poly = new PolyLine2D(source as PolyLine2D) { StartPoint = source.StartPoint };
			return poly;
		}

		public static ISegment2D CreateLineSegment2D(Point? endPoint = null)
		{
			if (endPoint.HasValue)
			{
				return new LineSegment2D(endPoint.Value);
			}

			return new LineSegment2D();
		}

		public static void ReversePolyline(ref IPolyLine2D polyline)
		{
			var segments = polyline.Segments.ToArray();
			var newStart = polyline.IsClosed ? polyline.StartPoint : segments[segments.Length - 1].EndPoint;
			Point start = polyline.StartPoint;
			int i, j;
			for (i = 0, j = segments.Length - 1; j >= 0; ++i, --j)
			{
				var s = segments[i];
				start = s.Reverse(ref start);
				polyline.Segments[j] = s;
			}

			polyline.StartPoint = newStart;
		}

		/// <summary>
		/// Reverse polyline
		/// </summary>
		/// <param name="polyline">Polyline to be reversed</param>
		/// <returns>Reverted polyline</returns>
		public static IPolyLine2D ReversePolylineCli(IPolyLine2D polyline)
		{
			return ReversePolyline(polyline);
		}

		/// <summary>
		/// Reverse polyline
		/// </summary>
		/// <param name="polyline">Polyline to be reversed</param>
		/// <returns>Reverted polyline</returns>
		public static IPolyLine2D ReversePolyline(IPolyLine2D polyline)
		{
			Point sp = polyline.StartPoint;
			IPolyLine2D retval = new PolyLine2D(polyline.Segments.Count);

			List<ISegment2D> lst = new List<ISegment2D>();
			for (int i = 0, sz = polyline.Segments.Count; i < sz; i++)
			{
				ISegment2D seg = polyline.Segments[i].Clone() as ISegment2D;
				sp = seg.Reverse(ref sp);
				lst.Add(seg);
			}

			retval.StartPoint = sp;
			for (int i = lst.Count - 1; i >= 0; i--)
			{
				ISegment2D seg = lst[i];
				retval.Segments.Add(seg);
			}

			return retval;
		}

		#endregion Convertors real geometry to discretize polygon

		public static double PointToCircArcDistance(ref Point start, CircularArcSegment2D arc, Point pt, out double relPos, double tolerance = 0)
		{
			var centre = arc.GetCentre(ref start);
			var v = pt - centre;
			var res = GeomTools2D.LineCircularArcIntersection(centre, v, start, arc, out Point[] inters);
			if (res == IntersectionInfo.NoIntersection)
			{
				var d1 = (pt - start).Length;
				var d2 = (pt - arc.EndPoint).Length;
				relPos = -1;
				return Math.Min(d1, d2);
			}

			var pos = arc.GetRelativePosition(ref start, inters[0].X, true, tolerance);
			if (pos.Length == 0)
			{
				var d1 = (pt - start).Length;
				var d2 = (pt - arc.EndPoint).Length;
				relPos = -1;
				return Math.Min(d1, d2);
			}

			Point point;
			if (pos[0].IsGreaterOrEqual(0) && pos[0].IsLesserOrEqual(1))
			{
				point = arc.GetPointOnSegment(ref start, pos[0]);
				if (point.IsEqualWithTolerance(inters[0]))
					relPos = pos[0];
				else
					relPos = pos[1];
			}
			else
			{
				relPos = pos[1];
			}

			var d = (pt - inters[0]).Length;
			return d;
		}

		public static IList<IPolyLine2D> GetCommonEdges(IPolyLine2D line1, IPolyLine2D line2, double tolerance)
		{
			//var sih1 = new Geom2DDebug.SceneHelperItem(line1, System.Windows.Media.Colors.Red);
			//var sih2 = new Geom2DDebug.SceneHelperItem(line2, System.Windows.Media.Colors.Green);
			//Geom2DDebug.Draw(sih1, sih2);

			var edges = new List<IPolyLine2D>();
			var count1 = line1.Segments.Count;
			var count2 = line2.Segments.Count;
			Point start1 = line1.StartPoint;
			for (var i = 0; i < count1; ++i)
			{
				var s1 = line1.Segments[i];
				var v1 = s1.EndPoint - start1;
				if (v1.LengthSquared.IsZero())
				{
					start1 = s1.EndPoint;
					continue;
				}

				Point start2 = line2.StartPoint;
				for (var j = 0; j < count2; ++j)
				{
					var s2 = line2.Segments[j];
					var v2 = s2.EndPoint - start2;
					if (v2.LengthSquared.IsZero())
					{
						start2 = s2.EndPoint;
						continue;
					}

					var v1n = v1;
					v1n.Normalize();
					var v2n = v2;
					v2n.Normalize();

					var d = v1n * v2n;

					// jestli bod druhe cary lezi na prvni care nebo bod prvni cary na druhe care a vektory jsou rovnobezne
					if (Math.Abs(d).IsEqual(1, 1e-6) && PointToLine(ref start2, ref start1, ref v1, 1e-6) == PointRelatedToLine.OnLine)
					{
						//var sih3 = new Geom2DDebug.SceneHelperItem(new Polygon2D() { start1, s1.EndPoint }, System.Windows.Media.Colors.Blue) { PenThickness = -1e-3 };
						//var sih4 = new Geom2DDebug.SceneHelperItem(new Polygon2D() { start2, s2.EndPoint }, System.Windows.Media.Colors.Black) { PenThickness = -1e-3 };
						//Geom2DDebug.Draw(sih2, sih1, sih3, sih4);

						var begTest = start1.IsEqualWithTolerance(s2.EndPoint, tolerance);
						var endTest = s1.EndPoint.IsEqualWithTolerance(start2, tolerance);
						if (begTest && endTest)
						{
							var p = CreatePolyline(start1, s1.EndPoint);
							edges.Add(p);
							start2 = s2.EndPoint;
							continue;
						}

						var begTest2 = start1.IsEqualWithTolerance(start2, tolerance);
						var endTest2 = s1.EndPoint.IsEqualWithTolerance(s2.EndPoint, tolerance);
						if (begTest2 && endTest2)
						{
							var p = CreatePolyline(start1, s1.EndPoint);
							edges.Add(p);
							start2 = s2.EndPoint;
							continue;
						}

						if ((d < 0 && (begTest2 || endTest2)) || (d > 0 && (begTest || endTest)))
						{
							start2 = s2.EndPoint;
							continue;
						}

						if (begTest || endTest || begTest2 || endTest2)
						{
							IPolyLine2D p;
							if (v1.LengthSquared < v2.LengthSquared)
							{
								p = CreatePolyline(start1, s1.EndPoint);
							}
							else if (begTest || endTest)
							{
								p = CreatePolyline(s2.EndPoint, start2);
							}
							else
							{
								p = CreatePolyline(start2, s2.EndPoint);
							}

							edges.Add(p);
							start2 = s2.EndPoint;
							continue;
						}

						// musim najit jejich spolecnou cast
						var vb = start2 - start1;
						var ve = s2.EndPoint - start1;

						var d1 = v1 * v1;
						var d2 = vb * v1;
						var d3 = ve * v1;
						if ((d2 < 0 && d3 < 0) || (d2 > d1 && d3 > d1))
						{
							start2 = s2.EndPoint;
							continue;
						}

						var dd = new List<double>() { 0, d2, d1, d3 };
						dd.Sort();
						Point p1, p2;
						if (d1 != 0)
						{
							p1 = start1 + dd[1] * v1 / d1;
							p2 = start1 + dd[2] * v1 / d1;
						}
						else
						{
							p1 = start1 + dd[1] * v1;
							p2 = start1 + dd[2] * v1;
						}

						if (!p1.IsEqualWithTolerance(p2, tolerance))
							edges.Add(CreatePolyline(p1, p2));

						start2 = s2.EndPoint;
						continue;
					}

					start2 = s2.EndPoint;
				}

				start1 = s1.EndPoint;
			}

			return edges;
		}

		private static IPolyLine2D CreatePolyline(Point start, Point end)
		{
			var s = new LineSegment2D(end);
			var p = new PolyLine2D(1) { StartPoint = start };
			p.Segments.Add(s);
			return p;
		}

		/// <summary>
		/// Posune segmenty uzavrene polyline o dany pocet.
		/// </summary>
		/// <param name="polyline"></param>
		/// <param name="count">Pocet, o kolik se ma pootocit.</param>
		public static void RotateSegments(IPolyLine2D polyline, int count)
		{
			if (!polyline.IsClosed)
				throw new ArgumentException("polyline must be closed");

			var c = polyline.Segments.Count;
			count %= c;
			if (count == 0)
				return;

			if (count < 0)
				count += c;

			var arr = new ISegment2D[count];
			for (var i = 0; i < count; ++i)
			{
				arr[i] = polyline.Segments[0];
				polyline.Segments.RemoveAt(0);
			}

			polyline.StartPoint = arr[count - 1].EndPoint;
			polyline.Segments.AddRange(arr);
		}

		public static void MergePolylineSegments(IPolyLine2D polySource)
		{
			for (int i = polySource.Segments.Count - 1; i > 0; i--)
			{
				ISegment2D seg = polySource.Segments[i];
				ISegment2D segPrev = polySource.Segments[i - 1];
				if (!((seg is LineSegment2D) && (segPrev is LineSegment2D)))
				{
					continue;
				}

				IdaComPoint2D last = seg.EndPoint;
				IdaComPoint2D middle = segPrev.EndPoint;
				IdaComPoint2D start;
				if (i > 1)
				{
					start = polySource.Segments[i - 2].EndPoint;
				}
				else
				{
					start = polySource.StartPoint;
				}

				Vector vect = new Vector(last.X - middle.X, last.Y - middle.Y);
				Vector vectPrev = new Vector(middle.X - start.X, middle.Y - start.Y);
				vect.Normalize();
				vectPrev.Normalize();
				if (vect == vectPrev)
				{
					polySource.Segments.RemoveAt(i - 1);
				}
			}
		}

		/// <summary>
		/// Compares two regions. If they has same geometry then true is returned.
		/// </summary>
		/// <param name="left">First region for compare</param>
		/// <param name="right">Second region for compare</param>
		/// <param name="tolerance">optional parameter. Means tolerance for coordinations difference of each point of geometry.</param>
		/// <returns>Result status: True if both regions has the same  geometry</returns>
		public static bool CompareRegion2DGeometry(IRegion2D left, IRegion2D right, double tolerance = 0.0000001)
		{
			if (left.Openings.Count != right.Openings.Count)
			{
				return false;
			}

			Func<IPolyLine2D, IPolyLine2D, bool> comparePolyline = (leftPoly, rightPoly) =>
			{
				if (!leftPoly.StartPoint.IsEqualWithTolerance(rightPoly.StartPoint, tolerance) ||
					leftPoly.Segments.Count != rightPoly.Segments.Count)
				{
					return false;
				}

				for (int i = 0, sz = leftPoly.Segments.Count; i < sz; i++)
				{
					if (!leftPoly.Segments[i].EndPoint.IsEqualWithTolerance(rightPoly.Segments[i].EndPoint, tolerance))
					{
						return false;
					}

					if (leftPoly.Segments[i].GetType() != rightPoly.Segments[i].GetType())
					{
						return false;
					}

					if (leftPoly.Segments[i] is CircularArcSegment2D)
					{
						var leftCircle = leftPoly.Segments[i] as CircularArcSegment2D;
						var rightCircle = rightPoly.Segments[i] as CircularArcSegment2D;
						if (!leftCircle.Point.IsEqualWithTolerance(rightCircle.Point, tolerance))
						{
							return false;
						}
					}
				}

				return true;
			};

			if (!comparePolyline(left.Outline, right.Outline))
			{
				return false;
			}

			for (int i = 0, sz = left.Openings.Count; i < sz; i++)
			{
				IPolyLine2D openingLeft = left.Openings[i];
				IPolyLine2D openingRight = right.Openings[i];
				if (!comparePolyline(openingLeft, openingRight))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Create and return a matrix based on given two points
		/// The matrix represents a coordinate system. 
		/// </summary>
		/// <param name="origin">Origin of the new coordinate system</param>
		/// <param name="pointOnX">Point on X axis</param>
		/// <returns>The matrix which represents the co-ordinate system</returns>
		public static Matrix GetLCSMatrix(Point origin, Point pointOnX)
		{
			var dif = new Vector(pointOnX.X - origin.X, pointOnX.Y - origin.Y);
			var cosAlpha = dif.X / dif.Length;
			var sinAlpha = dif.Y / dif.Length;
			return new Matrix(cosAlpha, sinAlpha, -sinAlpha, cosAlpha, origin.X, origin.Y);
			//return new Matrix(cosAlpha, -sinAlpha, sinAlpha, cosAlpha, origin.X, origin.Y); // this is inverted matrix
		}

		/// <summary>
		/// Get the smallest lenght of segment from region
		/// </summary>
		/// <param name="region">Region</param>
		/// <returns>The smallest length</returns>
		public static double GetLengthOfSmallestSegment(IRegion2D region)
		{
			double d = GetLengthOfSmallestSegment(region.Outline);
			foreach (var op in region.Openings)
			{
				d = Math.Min(d, GetLengthOfSmallestSegment(op));
			}

			return d;
		}

		/// <summary>
		/// Get the smallest lenght of segment from polyline
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <returns>The smallest length</returns>
		public static double GetLengthOfSmallestSegment(IPolyLine2D polyline)
		{
			double d = 0.0;
			Point p = polyline.StartPoint;
			bool first = true;
			foreach (var s in polyline.Segments)
			{
				if (first)
				{
					d = s.GetLength(ref p);
					first = false;
				}
				else
				{
					d = Math.Min(d, s.GetLength(ref p));
				}

				p = s.EndPoint;
			}

			return d;
		}

		/// <summary>
		/// polyline interpolation
		/// </summary>
		/// <param name="polyBeg">first polyline</param>
		/// <param name="polyEnd">secong polyline</param>
		/// <param name="relPosition">relative position</param>
		/// <returns>polyline</returns>
		public static IPolyLine2D PolyLineInterpolation(IPolyLine2D polyBeg, IPolyLine2D polyEnd, double relPosition)
		{
			if (polyBeg == null || polyEnd == null || polyBeg.Segments.Count != polyEnd.Segments.Count)
			{
				return null;
			}

			var outline = new PolyLine2D
			{
				StartPoint = PointInterpolation(polyBeg.StartPoint, polyEnd.StartPoint, relPosition)
			};
			for (int indexSeg = 0; indexSeg < polyBeg.Segments.Count; indexSeg++)
			{
				var segBeg = polyBeg.Segments[indexSeg];
				var segEnd = polyEnd.Segments[indexSeg];
				if (segBeg is LineSegment2D && segEnd is LineSegment2D)
				{
					outline.Segments.Add(new LineSegment2D(PointInterpolation(segBeg.EndPoint, segEnd.EndPoint, relPosition)));
				}
				else if (segBeg is CircularArcSegment2D && segEnd is CircularArcSegment2D)
				{
					outline.Segments.Add(new CircularArcSegment2D(PointInterpolation(segBeg.EndPoint, segEnd.EndPoint, relPosition), PointInterpolation((segBeg as CircularArcSegment2D).Point, (segEnd as CircularArcSegment2D).Point, relPosition)));
				}
				else
				{
					return null;
				}
			}

			return outline;
		}

		/// <summary>
		/// region interpolation
		/// </summary>
		/// <param name="regionBeg">first region</param>
		/// <param name="regionEnd">second region</param>
		/// <param name="relPosition">relative position</param>
		/// <returns>interpolated region as Region2D</returns>
		public static IRegion2D RegionInterpolation(IRegion2D regionBeg, IRegion2D regionEnd, double relPosition)
		{
			if ((regionBeg == null) || (regionEnd == null))
			{
				return null;
			}

			IRegion2D region = new Region2D
			{
				Outline = GeomTools2D.PolyLineInterpolation(regionBeg.Outline, regionEnd.Outline, relPosition)
			};
			if (region.Outline == null)
			{
				// TODO
				return null;
			}

			if (regionBeg.Openings.Count != regionEnd.Openings.Count)
			{
				// TODO
				return null;
			}

			for (int io = 0; io < regionBeg.Openings.Count; io++)
			{
				var opMaster = regionBeg.Openings.ElementAt(io);
				var opSlave = regionEnd.Openings.ElementAt(io);
				var op = GeomTools2D.PolyLineInterpolation(opMaster, opSlave, relPosition);
				if (op == null)
				{
					// TODO
					return null;
				}

				region.AddOpening(op);
			}

			return region;
		}

		/// <summary>
		/// point interpolation
		/// </summary>
		/// <param name="beg">begin point</param>
		/// <param name="end">end point</param>
		/// <param name="relPosition">relative position</param>
		/// <returns>point</returns>
		public static Point PointInterpolation(Point beg, Point end, double relPosition)
		{
			return new Point(beg.X + ((end.X - beg.X) * relPosition), beg.Y + ((end.Y - beg.Y) * relPosition));
		}

		public static IPolyLine2D CreatePolyline(IPolygon2D polygon)
		{
			var count = polygon.Count;
			var polyline = new PolyLine2D(count - 1);
			if (count > 0)
			{
				polyline.StartPoint = polygon[0];
				for (int i = 1; i < count; ++i)
				{
					var s = new LineSegment2D(polygon[i]);
					polyline.Segments.Add(s);
				}
			}

			return polyline;
		}

		public static IPolyLine2D CreatePolyline(IPolygon2D polygon, double maxCircArcAngle)
		{
			var count = polygon.Count;
			var polyline = new PolyLine2D(count);
			if (count > 0)
			{
				const double Tolerance = 1e-6;

				// first point
				var lastPoint = polygon[0];
				polyline.StartPoint = lastPoint;
				for (int i = 1; i < count; ++i)
				{
					// add segments as lines, few point for circle
					if ((i + 3) >= count)
					{
						lastPoint = polygon[i];
						polyline.Segments.Add(new LineSegment2D(lastPoint));
						continue;
					}

					// points of polygon
					var point1 = polygon[i];
					var point2 = polygon[i + 1];
					var point3 = polygon[i + 2];
					var point4 = polygon[i + 3];

					// circle from 1'st 3'rd and 5'th point
					var circle = CreateCircle(lastPoint, point2, point4);

					// checks, whether the 2'nd and 4'th points lies on the circle
					if (circle.IsPointOn(point1, Tolerance) && circle.IsPointOn(point3, Tolerance))
					{
						// all five point lies on the circle, so we check the maximal change of angle between them
						var angle1 = Math.Abs(AngleBetween(circle.Center, lastPoint, point1));
						var angle2 = Math.Abs(AngleBetween(circle.Center, point1, point2));
						var angle3 = Math.Abs(AngleBetween(circle.Center, point2, point3));
						var angle4 = Math.Abs(AngleBetween(circle.Center, point3, point4));
						if (angle1 <= maxCircArcAngle && angle2 <= maxCircArcAngle && angle3 <= maxCircArcAngle && angle4 <= maxCircArcAngle)
						{
							var iTemp = i;
							i += 3;

							// find end of circle
							for (int j = i + 1; j < count; ++j)
							{
								var point5 = polygon[j];
								var angle5 = Math.Abs(AngleBetween(circle.Center, point4, point5));
								if (circle.IsPointOn(point5, Tolerance) && angle5 <= maxCircArcAngle)
								{
									point4 = point5;
									i = j;
									continue;
								}

								i = j - 1;
								break;
							}

							iTemp = (iTemp + i) / 2;
							point2 = polygon[iTemp];

							// add final circle
							lastPoint = point4;
							polyline.Segments.Add(new CircularArcSegment2D(lastPoint, point2));
							continue;
						}
					}

					// points are not circle, add line segment
					lastPoint = point1;
					polyline.Segments.Add(new LineSegment2D(lastPoint));
				}
			}

			return polyline;
		}

		private static Circle2D CreateCircle(Point p1, Point p2, Point p3)
		{
			// create a circle with custom tolerance!
			var v21 = p2 - p1;
			var v32 = p3 - p2;
			var p12 = p1 + v21 / 2;
			var p23 = p2 + v32 / 2;
			var res = LineIntersection(p12, v21.Perpendicular(), p23, v32.Perpendicular(), out Point centre, 1e-50);
			double radius = 0;
			if (res != IntersectionInfo.NoIntersection)
			{
				radius = (p1 - centre).Length;
			}

			return new Circle2D(centre, radius);
		}

		private static double AngleBetween(Point centre, Point p1, Point p2)
		{
			var v1 = p1 - centre;
			var v2 = p2 - centre;
			v1.Normalize();
			var x = v2 * v1;
			var angle = Math.Acos(x / v2.Length) * 180.0 / Math.PI;
			return angle;
		}

		/// <summary>
		/// Calculate the distance between Point and a Line
		/// </summary>
		/// <param name="begPtOfLine">First point on the line</param>
		/// <param name="endPtOfLine">Second point on the line</param>
		/// <param name="point">Given point</param>
		/// <returns>The distance between Point and a Line</returns
		public static double Distance(Point begPtOfLine, Point endPtOfLine, Point point)
		{
			Vector vector = Subtract(ref begPtOfLine, ref point);
			Vector vector2 = Subtract(ref begPtOfLine, ref endPtOfLine);
			double angle = AngleBetweenVectors(vector, vector2);
			return Math.Abs(vector.Length * Math.Sin(angle));
		}

		/// <summary>
		/// Opravi geometrii polyline, pokud se segment vraci po predchozim segmentu.
		/// Kontroluje pouze LINIOVE segmenty!!!
		/// predpokladam, ze nejsou za sebou dva segmenty stejne orientovane (kolinearni)
		/// </summary>
		/// <param name="polyline">The polyline to correct.</param>
		/// <param name="tolerance">The tolerance - precision to check.</param>
		public static void CorrectPolyline(IPolyLine2D polyline, double tolerance = 1e-6)
		{
			var segments = polyline.Segments;
			var count = segments.Count;
			if (count < 2)
				return;

			var start = (Point)polyline.StartPoint;
			for (var i = 1; i < count; ++i)
			{
				var s0 = segments[i - 1] as LineSegment2D;
				var s1 = segments[i] as LineSegment2D;

				if (s0 == null || s1 == null)
				{
					start = segments[i - 1].EndPoint;
					continue;
				}

				if (s0.EndPoint.IsEqualWithTolerance(s1.EndPoint, tolerance))
				{
					// segment nulove delky
					segments.RemoveAt(i);
					--i;
					count -= 1;
					continue;
				}

				if (IsOpposite(ref start, s0, s1, tolerance))
				{
					// segmenty maji spolecny bod a jsou opacne orientovane
					// kratsi necham smazat, delsimu nastavim novy end point, pokud je to ten prvni segment
					var start1 = (Point)s0.EndPoint;
					var d0 = s0.GetLength(ref start);
					var d1 = s1.GetLength(ref start1);
					if (d0 == d1)
					{
						// segmenty jsou stejne dlouhe, smazu oba
						segments.RemoveAt(i);
						segments.RemoveAt(i - 1);
						i -= 2;
						count -= 2;
						continue;
					}

					if (d0 > d1)
					{
						// prvni segment je delsi
						segments.RemoveAt(i);
						start = s0.EndPoint = s1.EndPoint;
						--i;
						count -= 1;
						continue;
					}
					else
					{
						// druhy segment je delsi
						segments.RemoveAt(i - 1);
						--i;
						count -= 1;

						// start point zustava stejny
						continue;
					}
				}

				start = s0.EndPoint;
			}

			if (polyline.IsClosed)
			{
				// pokud je uzavrena, tak jeste prvni a posledni segment
				count = segments.Count;
				if (count < 2)
					return;

				var s0 = segments[count - 1] as LineSegment2D;
				var s1 = segments[0] as LineSegment2D;
				if (IsOpposite(ref start, s0, s1, tolerance))
				{
					var start1 = (Point)s0.EndPoint;
					var d0 = s0.GetLength(ref start);
					var d1 = s1.GetLength(ref start1);
					if (d0 == d1)
					{
						// smazat oba a nastavit start polyline
						polyline.StartPoint = start;
						segments.RemoveAt(count - 1);
						segments.RemoveAt(0);
					}
					else if (d0 > d1)
					{
						polyline.StartPoint = s0.EndPoint = s1.EndPoint;
						segments.RemoveAt(0);
					}
					else
					{
						polyline.StartPoint = start;
						segments.RemoveAt(count - 1);
					}
				}
			}
		}

		private static bool IsOpposite(ref Point start, ISegment2D s0, ISegment2D s1, double tolerance)
		{
			if (s0 != null && s1 != null)
			{
				var dir0 = s0.EndPoint - start;
				var dir1 = (Point)s1.EndPoint - s0.EndPoint;
				dir0.Normalize();
				dir1.Normalize();
				var d = dir0 * dir1;
				if (d.IsEqual(-1, tolerance))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Provides input parameters for discretization of region 2D.
		/// </summary>
		public class DiscretizationParam
		{
			private static DiscretizationParam simple;

			/// <summary>
			/// Gets or sets the number of tiles.
			/// </summary>
			public int NumberOfTiles { get; set; }

			/// <summary>
			/// Gets or sets the average length of tile.
			/// </summary>
			public double LengthOfTile { get; set; }

			/// <summary>
			/// Gets or sets the angle for arc discretization.
			/// </summary>
			public double Angle { get; set; }

			public static DiscretizationParam SimplePolygon
			{
				get
				{
					if (simple == null)
					{
						simple = new DiscretizationParam
						{
							Angle = 5,
							LengthOfTile = 1000,
							NumberOfTiles = 1,
						};
					}

					return simple;
				}
			}
		}

		/// <summary>
		/// Calculates an angle between two vectors.
		/// </summary>
		/// <param name="a">first vector</param>
		/// <param name="b">second vector</param>
		/// <returns>angle in radians</returns>
		public static double AngleBetweenVectors(Vector a, Vector b)
		{
			//V rovině cos(φ) = (a1*b1 + a2*b2) / (|a|*|b|)
			double cosFi = (a.X * b.X + a.Y * b.Y) / (a.Length * b.Length);
			if (cosFi > 1.0)
			{
				cosFi = 1.0;
			}

			if (cosFi < -1.0)
			{
				cosFi = -1.0;
			}

			return Math.Acos(cosFi);
		}

		public static Vector RotateVectorCounterClockwise(Vector vector, double angleDegrees)
		{
			var angleRad = angleDegrees * Math.PI / 180;
			var result = new Vector
			{
				X = vector.X * Math.Cos(angleRad) - vector.Y * Math.Sin(angleRad),
				Y = vector.X * Math.Sin(angleRad) + vector.Y * Math.Cos(angleRad)
			};
			return result;
		}

		public static IPolyLine2D Round(IPolyLine2D polyline, double radius)
		{
			return Round(polyline, Enumerable.Repeat(radius, polyline.Segments.Count).ToArray());
		}

		public static IPolyLine2D Round(IPolyLine2D polyline, IList<double> radius)
		{
			var roundedPolyline = new PolyLine2D(polyline.Segments.Count);
			var start = (Point)polyline.StartPoint;
			roundedPolyline.StartPoint = start;
			var count = polyline.Segments.Count;
			for (var i = 1; i < count; ++i)
			{
				if (radius.Count <= i)
				{
					for (; i < count; ++i)
					{
						var segment = polyline.Segments[i - 1].Clone() as ISegment2D;
						roundedPolyline.Segments.Add(segment);
						start = segment.EndPoint;
					}

					break;
				}

				var s1 = polyline.Segments[i - 1];
				var r = radius[i];
				if (r.IsGreater(0, 1e-6))
				{
					var s2 = polyline.Segments[i];
					if (s1 is LineSegment2D && s2 is LineSegment2D)
					{
						var arcExists = Round(ref start, s1, s2, r, out Point arcStart, out CircularArcSegment2D arc);
						if (arcExists)
						{
							if (!arcStart.IsEqualWithTolerance(start))
							{
								var s11 = new LineSegment2D(arcStart);
								roundedPolyline.Segments.Add(s11);
							}

							roundedPolyline.Segments.Add(arc);
							start = arc.EndPoint;
						}
						else
						{
							var segment = s1.Clone() as ISegment2D;
							roundedPolyline.Segments.Add(segment);
							start = s1.EndPoint;
						}
					}
					else
					{
						var segment = s1.Clone() as ISegment2D;
						roundedPolyline.Segments.Add(segment);
						start = s1.EndPoint;
					}
				}
				else
				{
					var segment = s1.Clone() as ISegment2D;
					roundedPolyline.Segments.Add(segment);
					start = s1.EndPoint;
				}
			}

			if (polyline.IsClosed)
			{
				var r = radius[0];
				if (r.IsGreater(0, 1e-6))
				{
					var s1 = polyline.Segments[count - 1];
					var s2 = polyline.Segments[0];
					if (s1 is LineSegment2D && s2 is LineSegment2D)
					{
						var arcExists = Round(ref start, s1, s2, r, out Point arcStart, out CircularArcSegment2D arc);
						if (arcExists)
						{
							if (!arcStart.IsEqualWithTolerance(start))
							{
								var s11 = new LineSegment2D(arcStart);
								roundedPolyline.Segments.Add(s11);
							}

							roundedPolyline.Segments.Add(arc);
							start = arc.EndPoint;
							roundedPolyline.StartPoint = start;
							if (start.IsEqualWithTolerance(roundedPolyline.Segments[0].EndPoint))
							{
								roundedPolyline.Segments.RemoveAt(0);
							}
						}
						else
						{
							var segment = polyline.Segments[count - 1].Clone() as ISegment2D;
							roundedPolyline.Segments.Add(segment);
						}
					}
					else
					{
						var segment = polyline.Segments[count - 1].Clone() as ISegment2D;
						roundedPolyline.Segments.Add(segment);
					}
				}
			}
			else
			{
				var segment = polyline.Segments[count - 1].Clone() as ISegment2D;
				roundedPolyline.Segments.Add(segment);
			}

			return roundedPolyline;
		}

		public static bool Round(ref Point start, ISegment2D line1, ISegment2D line2, double radius, out Point arcStart, out CircularArcSegment2D arcSegment)
		{
			/// http://stackoverflow.com/questions/24771828/algorithm-for-creating-rounded-corners-in-a-polygon
			var P1 = start;
			var P = (Point)line1.EndPoint;
			var P2 = (Point)line2.EndPoint;
			var angle = (Math.Atan2(P.Y - P1.Y, P.X - P1.X) - Math.Atan2(P.Y - P2.Y, P.X - P2.X)) / 2;
			var PC = radius / Math.Abs(Math.Tan(angle));
			var P_P1 = P - P1;
			var PP1 = P_P1.Length;
			if (PC.IsZero(1e-6) || PC.IsGreater(PP1, 1e-6))
			{
				arcStart = new Point();
				arcSegment = null;
				return false;
			}

			var C1X = P.X - (P.X - P1.X) * PC / PP1;
			var C1Y = P.Y - (P.Y - P1.Y) * PC / PP1;
			var C1 = new Point(C1X, C1Y);
			var P_P2 = P - P2;
			var PP2 = P_P2.Length;
			if (PC.IsGreater(PP2, 1e-6))
			{
				arcStart = new Point();
				arcSegment = null;
				return false;
			}

			var C2X = P.X - (P.X - P2.X) * PC / PP2;
			var C2Y = P.Y - (P.Y - P2.Y) * PC / PP2;
			var C2 = new Point(C2X, C2Y);
			//P_P1 = P_P1.Perpendicular();
			//P_P2 = P_P2.Perpendicular();
			//double t = Vector.CrossProduct(P_P2, P_P1);
			//var CX = C1.X + (P_P1.X * ((P_P2.Y * (C1.X - C2.X)) - (P_P2.X * (C1.Y - C2.Y))) / t);
			//var CY = C1.Y + (P_P1.Y * ((P_P2.Y * (C1.X - C2.X)) - (P_P2.X * (C1.Y - C2.Y))) / t);
			//var C = new Point(CX, CY);
			var C = LineIntersection(C1, P_P1.Perpendicular(), C2, P_P2.Perpendicular());

			var vec = P - P1;
			var ptl = GeomTools2D.PointToLine(ref C, ref P1, ref vec);
			var ccw = ptl == PointRelatedToLine.LeftSide ? true : false;
			arcSegment = new CircularArcSegment2D();
			arcSegment.SetPoints(C1, C, C2, ccw);
			arcStart = C1;
			return true;
		}

		/// <summary>
		/// Zaobli roh na konci zadaneho segmentu
		/// dokaze zaoblit pouze dva za sebou jdouci liniove segmenty
		/// </summary>
		/// <param name="radius">polomer zaobleni, pokud je radius zaporny tak se provede doplnek zaobleni (vykousnuti)</param>
		/// <param name="line1">prvni liniovy segment</param>
		/// <param name="line2">druhy liniovy segment</param>
		public static bool Rounding(ref Point start, ISegment2D line1, ISegment2D line2, double radius, out Point arcStart, out CircularArcSegment2D arcSegment)
		{
			Point pt1 = start;
			Point pt2 = line1.EndPoint;
			Point pt3 = line2.EndPoint;
			bool clockwise = true;

			Vector vv1 = pt2.Minus(pt1);
			Vector vv2 = pt3.Minus(pt2);

			if (vv1.Length < Math.Abs(radius) || vv2.Length < Math.Abs(radius))
			{
				arcStart = new Point();
				arcSegment = null;
				return false;
			}

			var C1X = pt2.X - (pt2.X - pt1.X) * Math.Abs(radius) / vv1.Length;
			var C1Y = pt2.Y - (pt2.Y - pt1.Y) * Math.Abs(radius) / vv1.Length;
			var C1 = new Point(C1X, C1Y);

			var C2X = pt2.X - (pt2.X - pt3.X) * Math.Abs(radius) / vv2.Length;
			var C2Y = pt2.Y - (pt2.Y - pt3.Y) * Math.Abs(radius) / vv2.Length;
			var C2 = new Point(C2X, C2Y);

			var C = LineIntersection(C1, vv1.Perpendicular(), C2, vv2.Perpendicular());
			var ptl = GeomTools2D.PointToLine(ref C, ref pt1, ref vv1);
			clockwise = ptl == PointRelatedToLine.LeftSide ? true : false;

			if (radius < 0.0)
			{
				clockwise = !clockwise;
				C = pt2;
			}

			arcSegment = new CircularArcSegment2D();
			arcSegment.SetPoints(C1, C, C2, clockwise);
			arcStart = C1;
			return true;
		}

		/// <summary>
		/// Zkosi roh na konci zadaneho segmentu
		/// dokaze zkosit pouze dva za sebou jdouci liniove segmenty
		/// </summary>
		/// <param name="v1">velikost zkoseni na prvni linii (na konci zadaneho segmentu)</param>
		/// <param name="v2">velikost zkoseni na druhe linii (na zacatku navazujiciho segmentu)</param>
		/// <param name="first">prvni liniovy segment</param>
		/// <param name="tag">privesek</param>
		/// <returns>vraci novy liniovy segment
		/// pokud vrati null , nedoslo k vytvoreni zkoseni
		/// - zkoseni je vetsi nez delka segmentu</returns>
		public static bool Tappering(ref Point start, ISegment2D line1, ISegment2D line2, double v1, double v2, out Point tapperStart, out LineSegment2D tapperSegment)
		{
			double length1 = line1.GetLength(ref start);
			if (v1.IsGreater(length1, 1e-6))
			{
				tapperStart = start;
				tapperSegment = line1 as LineSegment2D;
				return false;
			}

			if (v1.IsEqual(length1, 1e-6))
			{
				v1 = v1 - 0.00001;
			}

			Point node = line1.EndPoint;
			tapperStart = GetLinearPoint(ref node, ref start, v1 / length1);

			Point end = line2.EndPoint;
			double length2 = line2.GetLength(ref node);
			if (v2.IsGreater(length2, 1e-6))
			{
				tapperStart = start;
				tapperSegment = line1 as LineSegment2D;
				return false;
			}

			if (v2.IsEqual(length2, 1e-6))
			{
				v2 = v2 - 0.00001;
			}

			Point tapperEnd = GetLinearPoint(ref node, ref end, v2 / length2);
			tapperSegment = new LineSegment2D(tapperEnd);
			return true;
		}

		/// <summary>
		/// Create a notch at the end of given segment
		/// </summary>
		/// <param name="v1">velikost zářezu na prvni linii (na konci zadaneho segmentu)</param>
		/// <param name="v2">velikost zářezu na druhe linii (na zacatku navazujiciho segmentu)</param>
		/// <param name="r">radius of inner corner</param>
		/// <param name="start">počátek prvniho linioveho segmentu</param>
		/// <param name="line1">prvni linovy segment rohu</param>
		/// <param name="line2">druhý segment rohu</param>
		/// <returns>vraci novy liniovy segment
		/// pokud vrati null , nedoslo k vytvoreni zářezu
		/// - zářez je vetsi nez delka segmentu</returns>
		public static bool Notch(double v1, double v2, double r, ref Point start, ISegment2D line1, ISegment2D line2, out Point notchStartPoint, out List<ISegment2D> notch)
		{
			if (!(line1 is LineSegment2D && line2 is LineSegment2D))
			{
				notchStartPoint = new Point();
				notch = null;
				return false;
			}

			Vector l1Vec = line1.EndPoint - start;
			if (v1.IsGreater(l1Vec.Length, 1e-6))
			{
				notchStartPoint = new Point();
				notch = null;
				return false;
			}

			if (v1.IsEqual(l1Vec.Length, 1e-6))
			{
				v1 = v1 - 0.00001;
			}

			Point node = line1.EndPoint;
			Point nB = GetLinearPoint(ref node, ref start, v1 / l1Vec.Length);

			Vector l2Vec = line2.EndPoint - node;
			Point end = line2.EndPoint;

			if (v2.IsGreater(l2Vec.Length, 1e-6))
			{
				notchStartPoint = new Point();
				notch = null;
				return false;
			}

			if (v2.IsEqual(l2Vec.Length, 1e-6))
			{
				v2 = v2 - 0.00001;
			}

			Point nE = GetLinearPoint(ref node, ref end, v2 / l2Vec.Length);

			LineIntersection(nB, (new Vector(nB.X - node.X, nB.Y - node.Y)).Perpendicular(), nE, (new Vector(nE.X - node.X, nE.Y - node.Y)).Perpendicular(), out Point nC);

			notchStartPoint = nB;
			notch = new List<ISegment2D>();
			var seg1 = new LineSegment2D(nC);
			var seg2 = new LineSegment2D(nE);
			notch.Add(seg1);

			if (r.IsGreater(0.00001))
			{
				if (Rounding(ref nB, seg1, seg2, r, out Point arcStart, out CircularArcSegment2D arcSegment))
				{
					seg1.EndPoint = arcStart;
					notch.Add(arcSegment);
				}
			}

			notch.Add(seg2);
			return true;
		}

		/// <summary>
		/// Calculate the Relative Point on the line created with the two given point
		/// </summary>
		/// <param name="startPoint">Start point of Line</param>
		/// <param name="endPoint">End point of Line</param>
		/// <param name="relativePosition">Relative Position</param>
		/// <returns>Point on Line based on Relative Position</returns>
		public static Point GetLinearPoint(ref Point startPoint, ref Point endPoint, double relativePosition = 0.5)
		{
			Vector vect = Subtract(ref endPoint, ref startPoint);
			vect = vect * relativePosition;
			return Add(ref startPoint, ref vect);
		}

		/// <summary>
		/// Subtracting two points
		/// </summary>
		/// <param name="point1">first point</param>
		/// <param name="point2">secont point</param>
		/// <returns>Returns the resultant difference vector</returns>
		public static Vector Subtract(ref Point point1, ref Point point2)
		{
#if NET48
			if (point1 == null || point2 == null)
			{
				return new Vector();
			}
#endif

			Vector vector = new Vector(point1.X - point2.X, point1.Y - point2.Y);
			return vector;
		}

		/// <summary>
		/// Adding vector to a point
		/// </summary>
		/// <param name="point">Point</param>
		/// <param name="vector">Vector</param>
		/// <returns>Returns the resultant sum</returns>
		public static Point Add(ref Point point, ref Vector vector)
		{
			Point newPoint = new Point(point.X + vector.X, point.Y + vector.Y);
			return newPoint;
		}

		/// <summary>
		/// Liniovy segment zmeni na oblouk
		/// </summary>
		/// <param name="elevation">Prevyseni ve stredu linioveho segmentu</param>
		/// <param name="line">liniovy segment</param>
		/// <returns>vraci novy obloukovy segment , pokud vrati null , nedoslo k vytvoreni oblouku</returns>
		public static CircularArcSegment2D LineToArc(double elevation, ref Point startPoint, ISegment2D line)
		{
			if (!(line is LineSegment2D))
			{
				return null;
			}

			Point endPt = (Point)line.EndPoint;
			Vector lineVec = endPt.Minus(startPoint);

			Point centrPt = GetLinearPoint(ref startPoint, ref endPt, 0.5);
			Point arcTop = new Point(lineVec.Y + centrPt.X, -lineVec.X + centrPt.Y);
			Point pt2 = GetLinearPoint(ref centrPt, ref arcTop, elevation / lineVec.Length);
			CircularArcSegment2D arc = new CircularArcSegment2D(endPt, pt2);
			return arc;
		}

		/// <summary>
		/// Gets value Y from the polygon by the X value
		/// </summary>
		/// <param name="polygon">Polygon</param>
		/// <param name="x">X value</param>
		/// <param name="tolerance">Comparassion tollerance</param>
		/// <param name="extrapolateBegin">Extrapolation before polygon</param>
		/// <param name="extrapolateEnd">Extrapolation after polygon</param>
		/// <returns>Y value or NaN</returns>
		public static double GetValueByX(IPolygon2D polygon, double x, double tolerance, bool extrapolateBegin = false, bool extrapolateEnd = false)
		{
			if (polygon == null)
			{
				return double.NaN;
			}

			int sz = polygon.Count;
			if (sz == 0)
			{
				return double.NaN;
			}

			if (x.IsLesser(polygon[0].X, tolerance))
			{
				if (extrapolateBegin)
				{
					if (sz == 1)
					{
						return polygon[0].Y;
					}
					else
					{
						//extrapolace pred
						var x0 = polygon[0].X;
						var x1 = polygon[1].X;
						return polygon[0].Y - (x0 - x) * ((polygon[1].Y - polygon[0].Y) / (x1 - x0));
					}
				}
				else
				{
					return polygon[0].Y;
				}
			}

			if (x.IsGreater(polygon[sz - 1].X, tolerance))
			{
				if (extrapolateBegin)
				{
					if (sz == 1)
					{
						return polygon[sz - 1].Y;
					}
					else
					{
						//extrapolace za
						var x0 = polygon[sz - 1].X;
						var x1 = polygon[sz - 2].X;
						return polygon[sz - 1].Y + (x - x0) * ((polygon[sz - 1].Y - polygon[sz - 2].Y) / (x1 - x0));
					}
				}
				else
				{
					return polygon[sz - 1].Y;
				}
			}

			for (int i = 0; i < sz; i++)
			{
				if (polygon[i].X.IsEqual(x, tolerance))
				{
					return polygon[i].Y;
				}

				if (i == 0)
				{
					continue;
				}

				if (x.IsGreater(polygon[i - 1].X, tolerance) && x.IsLesser(polygon[i].X, tolerance))
				{
					//interplace
					var x0 = polygon[i - 1].X;
					var x1 = polygon[i].X;
					return polygon[i - 1].Y + (x - x0) * ((polygon[i].Y - polygon[i - 1].Y) / (x1 - x0));
				}
			}

			return double.NaN;
		}

		/// <summary>
		/// Return the segment and the position on the segment from the polyline and position on the polyline.
		/// </summary>
		/// <param name="polyline">The polyline</param>
		/// <param name="polylinePosition">The position on the polyline</param>
		/// <param name="relative">The position is a relative value</param>
		/// <param name="tolerance">The tolerance oc comparasion</param>
		/// <param name="segment">Returned segment or null</param>
		/// <param name="segmentStartPoint">Returned the start point of the segment</param>
		/// <param name="segmentPosition">Returned the position on the segment or NaN</param>
		/// <returns>Returns the zero based index or -1 if the position is outside the polyline</returns>
		public static int GetSegmentByPosition(IPolyLine2D polyline, double polylinePosition, bool relative, double tolerance, out ISegment2D segment, out Point segmentStartPoint, out double segmentPosition)
		{
			segment = null;
			segmentPosition = double.NaN;
			segmentStartPoint = new Point();
			if (polyline == null)
			{
				return -1;
			}

			var absPos = polylinePosition;
			var length = polyline.Length;
			if (relative)
			{
				absPos *= length;
			}

			var tot = 0.0;
			segmentStartPoint = polyline.StartPoint;
			for (int i = 0; i < polyline.Segments.Count; i++)
			{
				var seg = polyline.Segments[i];
				var lseg = seg.GetLength(ref segmentStartPoint);

				if (absPos.IsGreaterOrEqual(tot, tolerance) && absPos.IsLesserOrEqual(tot + lseg, tolerance))
				{
					segmentPosition = absPos - tot;
					if (relative)
					{
						if (lseg.IsZero(tolerance))
						{
							segmentPosition = 0.0;
						}
						else
						{
							segmentPosition /= lseg;
						}
					}

					segment = seg;
					return i;
				}

				tot += lseg;
				segmentStartPoint = seg.EndPoint;
			}

			return -1;
		}

		/// <summary>
		/// Returns segments between two point
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="p1">First point</param>
		/// <param name="p2">Second point</param>
		/// <param name="precision">Precision</param>
		/// <param name="position1">return relative position at first segment</param>
		/// <param name="position2">return relative position at last segment</param>
		/// <returns>list of segments</returns>
		public static IEnumerable<ISegment2D> GetSegmentsBetweenPoints(IPolyLine2D polyline, ref Point p1, ref Point p2, double precision, out double position1, out double position2)
		{
			var listSeg = new List<ISegment2D>();
			position1 = double.NaN;
			position2 = double.NaN;

			var p = (Point)polyline.StartPoint;
			foreach (var seg in polyline.Segments)
			{
				double pos;

				if (!listSeg.Any())
				{
					if (IsPointOnSegment(seg, ref p, ref p1, precision, out pos))
					{
						listSeg.Add(seg);
						position1 = pos;
					}
				}
				else
				{
					listSeg.Add(seg);
				}

				if (listSeg.Any())
				{
					if (IsPointOnSegment(seg, ref p, ref p2, precision, out pos))
					{
						position2 = pos;
						break;
					}
				}

				p = seg.EndPoint;
			}

			return listSeg;
		}

		/// <summary>
		/// returns point on polyline at specified position
		/// </summary>
		/// <param name="polyline"></param>
		/// <param name="position">abs position</param>
		/// <param name="point"></param>
		/// <returns></returns>
		public static bool GetPointOnPolyLine(IPolyLine2D polyline, double position, out Point point)
		{
			if (polyline != null && polyline.Segments.Any())
			{
				double length = 0.0;
				Point start = polyline.StartPoint;

				foreach (var segment in polyline.Segments)
				{
					var segmentLength = segment.GetLength(ref start);
					if (!segmentLength.IsZero())
					{
						var posOnSegment = position - length;
						if (posOnSegment.IsLesserOrEqual(segmentLength, 1e-6))
						{
							point = segment.GetPointOnSegment(ref start, posOnSegment / segmentLength);
							return true;
						}
					}

					start = segment.EndPoint;
					length += segmentLength;
				}
			}

			point = new Point();
			return false;
		}

		/// <summary>
		/// returns tangent on polyline at specified position
		/// </summary>
		/// <param name="polyline"></param>
		/// <param name="position">abs position</param>
		/// <param name="tangent"></param>
		/// <returns></returns>
		public static bool GetTangentOnPolyLine(IPolyLine2D polyline, double position, out Vector tangent)
		{
			if (polyline != null && polyline.Segments.Any())
			{
				double length = 0.0;
				Point start = polyline.StartPoint;

				foreach (var segment in polyline.Segments)
				{
					var segmentLength = segment.GetLength(ref start);
					if (!segmentLength.IsZero())
					{
						var posOnSegment = position - length;
						if (posOnSegment.IsLesserOrEqual(segmentLength, 1e-6))
						{
							tangent = segment.GetTangentOnSegment(ref start, posOnSegment / segmentLength);
							return true;
						}
					}

					start = segment.EndPoint;
					length += segmentLength;
				}
			}

			tangent = new Vector();
			return false;
		}

		/// <summary>
		/// Returns true if point is on the segment
		/// </summary>
		/// <param name="segment">Segment</param>
		/// <param name="startPoint">First point on the segment</param>
		/// <param name="p">Point</param>
		/// <param name="precision">Precision</param>
		/// <param name="position">Return position on segment</param>
		/// <returns>True if the point on the segment</returns>
		public static bool IsPointOnSegment(ISegment2D segment, ref Point startPoint, ref Point p, double precision, out double position)
		{
			position = double.NaN;
			//zatim primka
			//Debug.Assert(segment is LineSegment2D, "Only LineSegment2D is supported yet");

			if (startPoint.IsEqualWithTolerance(p, precision))
			{
				position = 0.0;
				return true;
			}

			if (segment.EndPoint.IsEqualWithTolerance(p, precision))
			{
				position = 1.0;
				return true;
			}

			var v = segment.EndPoint - startPoint;
			var v1 = p - startPoint;
			if (v.Length.IsZero(precision))
			{
				return false;
			}

			var d = (v1 * v) / v.Length;
			if (d.IsLesser(0.0) || d.IsGreater(v.Length))
			{
				return false;
			}

			position = d / v.Length;
			var pp = startPoint + (v * position);
			return (p.IsEqualWithTolerance(pp, precision));
		}

		/// <summary>
		/// Determines position of <paramref name="testedPt"/> on polyline <paramref name="polyline"/>
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="testedPt">Tested point</param>
		/// <param name="precision">Precision</param>
		/// <returns>abs position</returns>
		public static double GetPositionOnPolyline(IPolyLine2D polyline, Point testedPt, double precision = 1e-5)
		{
			if (polyline == null || polyline.Length.IsZero())
			{
				return 0.0;
			}

			double position = 0.0;

			Point prevPoint = polyline.StartPoint;
			Point segBegin;
			Point segEnd;
			foreach (var seg in polyline.Segments)
			{
				segBegin = prevPoint;
				segEnd = seg.EndPoint;
				prevPoint = segEnd;

				if (IsPointOnSegment(seg, ref segBegin, ref testedPt, precision, out double segPosition))
				{
					position += segPosition * seg.GetLength(ref segBegin);
					return position;
				}

				position += seg.GetLength(ref segBegin);
			}

			return 0.0;
		}

		/// <summary>
		/// Creates a Polygon2D from a IEnumerable&lt;Point&gt;.
		/// </summary>
		/// <param name="source">An IEnumerable&lt;Point&gt; to create an Polygon2D from.</param>
		/// <returns>A Polygon2D that contains the elements from the input sequence.</returns>
		public static Polygon2D ToPolygon2D(this IEnumerable<Point> source)
		{
			return new Polygon2D(source);
		}
	}
}