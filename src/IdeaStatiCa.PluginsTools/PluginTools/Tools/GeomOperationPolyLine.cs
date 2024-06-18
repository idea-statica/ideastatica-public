using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CI.Geometry2D;
using CI.GiCL2D;
using CI.Mathematics;
using WM = System.Windows.Media.Media3D;

namespace CI.Geometry3D
{
	/// <summary>
	/// Geometry operations related with PolyLine3D
	/// </summary>
	public static partial class GeomOperation
	{
		/// <summary>
		/// Clones IPolyLine3D
		/// </summary>
		/// <param name="src">Source IPolyLine3D</param>
		/// <returns>New instance of IPolyLine3D</returns>
		public static IPolyLine3D Clone(IPolyLine3D src)
		{
			return new PolyLine3D(src);
		}

		/// <summary>
		/// Append second polyline at the end of first polyline
		/// </summary>
		/// <param name="polyline1">First polyline to which second polyline is appended</param>
		/// <param name="polyline2">Second polyline to be appended</param>
		public static void Append(IPolyLine3D polyline1, IPolyLine3D polyline2)
		{
			if (polyline1 == null || polyline2 == null)
			{
				return;
			}

			foreach (ISegment3D segment in polyline2.Segments)
			{
				polyline1.Add(segment);
			}
		}

		/// <summary>
		/// Append a segment to polyline only if end points are matching
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="newSegment">Segment</param>
		/// <returns>true if segment is appended,
		/// false otherwise</returns>
		public static bool Append(IPolyLine3D polyline, ISegment3D newSegment)
		{
			if (polyline.Count > 0)
			{
				ISegment3D firstSegment = polyline[0];
				if (GeomOperation.IsEqual(newSegment.EndPoint, firstSegment.StartPoint))
				{
					polyline.Insert(0, newSegment);
					return true;
				}
				else if (GeomOperation.IsEqual(newSegment.StartPoint, firstSegment.StartPoint))
				{
					newSegment = GeomOperation.Reverse(newSegment);
					polyline.Insert(0, newSegment);
					return true;
				}

				ISegment3D lastSegment = polyline[polyline.Count - 1];
				if (GeomOperation.IsEqual(lastSegment.EndPoint, newSegment.StartPoint))
				{
					polyline.Add(newSegment);
					return true;
				}
				else if (GeomOperation.IsEqual(lastSegment.EndPoint, newSegment.EndPoint))
				{
					newSegment = GeomOperation.Reverse(newSegment);
					polyline.Add(newSegment);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks end points of polyline and segment
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="newSegment">Segment</param>
		/// <returns>True if any end point of polyline and segment matches</returns>
		public static bool CanAppendSegment(IPolyLine3D polyline, ISegment3D newSegment)
		{
			if (polyline.Count > 0)
			{
				ISegment3D firstSegment = polyline[0];
				if (GeomOperation.IsEqual(newSegment.EndPoint, firstSegment.StartPoint) ||
					GeomOperation.IsEqual(newSegment.StartPoint, firstSegment.StartPoint))
				{
					return true;
				}

				ISegment3D lastSegment = polyline[polyline.Count - 1];
				if (GeomOperation.IsEqual(lastSegment.EndPoint, newSegment.StartPoint) ||
					GeomOperation.IsEqual(lastSegment.EndPoint, newSegment.EndPoint))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Close a polyline if it is not a closed polyline
		/// </summary>
		/// <param name="polyline">The polylien to be modified</param>
		public static void Close(IPolyLine3D polyline)
		{
			if (polyline == null)
			{
				return;
			}

			if (polyline.IsClosed == false)
			{
				if (polyline.Count < 1)
				{
					return;
				}

				if (polyline.Count == 1)
				{
					if (polyline[0].SegmentType == SegmentType.Line)
					{
						return;
					}
				}

				if (!polyline.IsClosed)
				{
					polyline.Add(new LineSegment3D(polyline[polyline.Count - 1].EndPoint, polyline[0].StartPoint));
				}
			}
		}

		/// <summary>
		/// Convert Polyline to point list.
		/// </summary>
		/// <param name="polyline">PolyLine3D</param>
		/// <param name="points">list of point</param>
		/// <param name="basePointsOnly">if true use the base points of segments, otherwise dividing no straight segments</param>
		/// <param name="stepAngle">Angle for dividing curved segments (in degrees)</param>
		public static void ConvertPolylineToPointList(IPolyLine3D polyline, IList<IPoint3D> points, bool basePointsOnly, double stepAngle = 15.0)
		{
			//TODO: IList to ICollection
			if (polyline == null || polyline.Count == 0 || points == null)
			{
				return;
			}

			points.Add(polyline[0].StartPoint);
			foreach (ISegment3D segment in polyline.Segments)
			{
				if (segment.SegmentType == SegmentType.Line)
				{
					points.Add(segment.EndPoint);
				}
				else if (segment.SegmentType == SegmentType.CircularArc)
				{
					IArcSegment3D arc = segment as IArcSegment3D;
					if (arc == null)
					{
						throw new InvalidCastException("Wrong segment data");
					}

					if (basePointsOnly)
					{
						points.Add(arc.IntermedPoint);
					}
					else
					{
						double splitAngle = Conversions.DegreesToRadians(stepAngle); // Math.PI / 10.0;
						double angle = GeomOperation.GetArcAngle(arc);
						if (angle > splitAngle)
						{
							//int sz = (int)(angle / splitAngle) + 1;
							//double delta = angle / (double)sz;
							//for (int i = 1; i < sz; i++)
							//{
							//  double rel = delta * i / angle;
							//  IPoint3D point1 = null;
							//  GetPointOnSegment(arc, rel, ref point1);
							//  points.Add(point1);
							//}

							double delta = 1.0 / Math.Ceiling(angle / splitAngle);
							double rel = delta;
							while (rel.IsLesser(1))
							{
								IPoint3D point1 = null;
								GetPointOnSegment(arc, rel, ref point1);
								points.Add(point1);
								rel += delta;
							}
						}
						else
						{
							points.Add(arc.IntermedPoint);
						}
					}

					points.Add(arc.EndPoint);
				}
				else if (segment.SegmentType == SegmentType.Parabola)
				{
					IArcSegment3D arc = segment as IArcSegment3D;
					if (arc == null)
					{
						throw new InvalidCastException("Wrong segment data");
					}

					if (basePointsOnly)
					{
						points.Add(arc.IntermedPoint);
						points.Add(arc.EndPoint);
					}
					else
					{
						IList<double> relativePosition = new List<double>();
						int cnt = GeomOperation.GetQuadraticSplineRelativePosition(arc, Conversions.DegreesToRadians(stepAngle)/*(30.0 / 180.0) * System.Math.PI*/, ref relativePosition);
						IPoint3D point = null;
						for (int i = 1; i < cnt; i++)
						{
							GeomOperation.GetPointOnSegment(segment, relativePosition[i], ref point);
							points.Add(point);
						}
					}
				}
				else
				{
					throw new NotSupportedException();
				}
			}
		}

		/// <summary>
		/// Creates 3D polyline from point list
		/// </summary>
		/// <param name="polyline2D">list of 3D points</param>
		/// /// <param name="closed">should be last point connected to the first</param>
		/// <returns>polyline 3D</returns>
		public static PolyLine3D GetPolylineFromPointList(List<IPoint3D> points, bool closed = false)
		{
			if (points == null || !points.Any())
			{
				return null;
			}

			IList<ISegment3D> segList = new List<ISegment3D>();
			double forLoops = closed? (points.Count - 1) : (points.Count - 2);

			for (int i = 0; i <= forLoops; i++)
			{
				Point3D start = new Point3D(points.ElementAt(i).X, points.ElementAt(i).Y, points.ElementAt(i).Z);
				Point3D end;
				if (i < (points.Count - 1))
				{
					end = new Point3D(points.ElementAt(i + 1).X, points.ElementAt(i + 1).Y, points.ElementAt(i + 1).Z);
				}
				else
				{
					end = new Point3D(points.ElementAt(0).X, points.ElementAt(0).Y, points.ElementAt(0).Z);
				}
				LineSegment3D seg = new LineSegment3D(start, end);
				segList.Add(seg);
				start = new Point3D(end.X, end.Y, end.Z);
			}
			PolyLine3D polyline3D = new PolyLine3D(segList);
			return polyline3D;
		}

		/// <summary>
		/// Gets list of all segments point of polyline
		/// </summary>
		/// <param name="polyline">Polyline 3D</param>
		/// <returns>list of points</returns>
		public static IList<IPoint3D> GetPolylinePoints(IPolyLine3D polyline)
		{
			IList<IPoint3D> points = new List<IPoint3D>();
			if (polyline == null || polyline.Count == 0 || points == null)
			{
				return points;
			}

			points.Add(polyline[0].StartPoint);
			foreach (ISegment3D segment in polyline.Segments)
			{
				if (segment.SegmentType == SegmentType.Line)
				{
					points.Add(segment.EndPoint);
				}
				else if ((segment.SegmentType == SegmentType.CircularArc) || (segment.SegmentType == SegmentType.Parabola))
				{
					IArcSegment3D arc = segment as IArcSegment3D;
					if (arc == null)
					{
						throw new InvalidCastException("Wrong segment data");
					}

					points.Add(arc.IntermedPoint);
					points.Add(arc.EndPoint);
				}
				else
				{
					throw new NotSupportedException();
				}
			}

			return points;
		}

		/// <summary>
		/// Convert Polyline3D to Polyline2D in XY of LCS
		/// </summary>
		/// <param name="matrix">LCS</param>
		/// <param name="polyline3D">PolyLine3D</param>
		/// <returns>New instance of IPolyLine2D</returns>
		public static IPolyLine2D ConvertTo2D(IMatrix44 matrix, IPolyLine3D polyline3D)
		{
			if (polyline3D == null)
			{
				return null;
			}

			var lineClone = Clone(polyline3D);
			TransformToLCS(matrix, lineClone);

			IPolyLine2D polyline2D = new PolyLine2D();
			//polyline2D.Id = polyline3D.Id;
			foreach (ISegment3D segment3D in lineClone.Segments)
			{
				if (segment3D.SegmentType == SegmentType.Line)
				{
					LineSegment2D lineSegment2D = new LineSegment2D(new Point(segment3D.EndPoint.X, segment3D.EndPoint.Y));
					polyline2D.Segments.Add(lineSegment2D);
				}
				else if (segment3D.SegmentType == SegmentType.CircularArc)
				{
					CircularArcSegment2D arcSegment2D = new CircularArcSegment2D(new Point(segment3D.EndPoint.X, segment3D.EndPoint.Y), new Point((segment3D as IArcSegment3D).IntermedPoint.X, (segment3D as IArcSegment3D).IntermedPoint.Y));
					polyline2D.Segments.Add(arcSegment2D);
				}
			}

			if (!lineClone.IsClosed)
			{
				polyline2D.StartPoint = new IdaComPoint2D(lineClone[0].StartPoint.X, lineClone[0].StartPoint.Y);
			}
			else
			{
				polyline2D.StartPoint = new IdaComPoint2D(polyline2D.Segments[polyline2D.Segments.Count - 1].EndPoint.X, polyline2D.Segments[polyline2D.Segments.Count - 1].EndPoint.Y);
			}

			return polyline2D;
		}

		/// <summary>
		/// Convert Polyline3D to Polyline2D
		/// </summary>
		/// <param name="polyline3D">3D Polyline</param>
		/// <returns>2D Polyline</returns>
		public static IPolyLine2D ConvertTo2D(IPolyLine3D polyline3D)
		{
			if (polyline3D == null)
			{
				return null;
			}

			Vector3D normal = new Vector3D();
			if (!GetAreaVector(polyline3D, ref normal))
			{
				return null;
			}

			Matrix44 matrix = GetLCSMatrix(normal);
			IPoint3D point1 = new Point3D();
			IPoint3D point2 = new Point3D();
			GeomOperation.GetBoundingPoints(polyline3D, ref point1, ref point2);
			matrix.Origin = GeomOperation.GetLinearPoint(point1, point2);
			return ConvertTo2D(matrix, polyline3D);

			/*
			TransformToLCS(matrix, polyline3D);
			IPolyLine2D polyline2D = new PolyLine2D();
			foreach (ISegment3D segment3D in polyline3D.Segments)
			{
				if (segment3D.SegmentType == SegmentType.Line)
				{
					LineSegment2D lineSegment2D = new LineSegment2D(new Point(segment3D.EndPoint.X, segment3D.EndPoint.Y));
					polyline2D.Segments.Add(lineSegment2D);
				}
				else if (segment3D.SegmentType == SegmentType.CircularArc)
				{
					CircularArcSegment2D arcSegment2D = new CircularArcSegment2D(new Point(segment3D.EndPoint.X, segment3D.EndPoint.Y), new Point((segment3D as IArcSegment3D).IntermedPoint.X, (segment3D as IArcSegment3D).IntermedPoint.Y));
					polyline2D.Segments.Add(arcSegment2D);
				}
			}

			polyline2D.StartPoint = new IdaComPoint2D(polyline2D.Segments[polyline2D.Segments.Count - 1].EndPoint.X, polyline2D.Segments[polyline2D.Segments.Count - 1].EndPoint.Y);
			return polyline2D;
			*/
		}

		/// <summary>
		/// Converting Geometry2D.IPolyLine2D to Geometry3D.IPolyLine3D
		/// </summary>
		/// <param name="polyline2D">Geometry2D.IPolyLine2D</param>
		/// <returns>Geometry3D.IPolyLine3D</returns>
		public static IPolyLine3D ConvertTo3D(Geometry2D.IPolyLine2D polyline2D)
		{
			if (polyline2D == null)
			{
				return null;
			}

			IPolyLine3D polyline3D = new PolyLine3D();
			IPoint3D startPoint = ConvertTo3D(polyline2D.StartPoint);
			foreach (Geometry2D.ISegment2D segment2D in polyline2D.Segments)
			{
#if NET48
				if (segment2D == null || segment2D.EndPoint == null)
				{
					continue;
				}
#else
				if (segment2D == null)
				{
					continue;
				}
#endif
				IPoint3D endPoint = ConvertTo3D(segment2D.EndPoint);

				if (segment2D is Geometry2D.LineSegment2D)
				{
					polyline3D.Add(new LineSegment3D(startPoint, endPoint));
				}
				else if (segment2D is Geometry2D.CircularArcSegment2D)
				{
					Point point = (segment2D as Geometry2D.CircularArcSegment2D).Point;
					IPoint3D midPoint = new Point3D(point.X, point.Y, 0);
					polyline3D.Add(new ArcSegment3D(startPoint, midPoint, endPoint));
				}

				startPoint = endPoint;
			}

			if (polyline3D.IsClosed)
			{
				if (polyline3D.Count > 0)
				{
					IPoint3D firstPoint = polyline3D[0].StartPoint;
					polyline3D[polyline3D.Count - 1].EndPoint = firstPoint;
				}
			}

			return polyline3D;
		}

		/// <summary>
		/// Converting Geometry2D.IPolyLine2D to Geometry3D.IPolyLine3D swapping Y->Z values
		/// </summary>
		/// <param name="polyline2D">Geometry2D.IPolyLine2D</param>
		/// <param name="Yvalue">Y-value to add</param>
		/// <returns>Geometry3D.IPolyLine3D</returns>
		public static IPolyLine3D ConvertTo3D(Geometry2D.IPolyLine2D polyline2D, double Yvalue)
		{
			if (polyline2D == null)
			{
				return null;
			}

			IPolyLine3D polyline3D = new PolyLine3D();
			IPoint3D startPoint = ConvertTo3D(polyline2D.StartPoint, Yvalue);
			foreach (Geometry2D.ISegment2D segment2D in polyline2D.Segments)
			{
#if NET48
				if (segment2D == null || segment2D.EndPoint == null)
				{
					continue;
				}
#else
				if (segment2D == null)
				{
					continue;
				}
#endif
				IPoint3D endPoint = ConvertTo3D(segment2D.EndPoint, Yvalue);

				if (segment2D is Geometry2D.LineSegment2D)
				{
					polyline3D.Add(new LineSegment3D(startPoint, endPoint));
				}
				else if (segment2D is Geometry2D.CircularArcSegment2D)
				{
					Point point = (segment2D as Geometry2D.CircularArcSegment2D).Point;
					IPoint3D midPoint = new Point3D(point.X, Yvalue, point.Y);
					polyline3D.Add(new ArcSegment3D(startPoint, midPoint, endPoint));
				}

				startPoint = endPoint;
			}

			if (polyline3D.IsClosed)
			{
				if (polyline3D.Count > 0)
				{
					IPoint3D firstPoint = polyline3D[0].StartPoint;
					polyline3D[polyline3D.Count - 1].EndPoint = firstPoint;
				}
			}

			return polyline3D;
		}

		/// <summary>
		/// Calculate area of closed polyLine
		/// </summary>
		/// <param name="polyLine">Closed polyLine </param>
		/// <returns>Area of closed polyline</returns>
		public static double GetAreaDiscretized(IPolyLine3D polyLine)
		{
			IList<ISegment3D> segs = new List<ISegment3D>();
			foreach (ISegment3D seg in polyLine.Segments)
			{
				if (seg is LineSegment3D)
				{
					segs.Add(seg);
				}
				else
				{
					IPoint3D ep = null;
					IPoint3D bp = seg.StartPoint;

					for (int i = 1; i <= 10; i++)
					{
						double relPos = 0.1 * i;
						if (GetPointOnSegment(seg, relPos, ref ep))
						{
							ISegment3D nseg = new LineSegment3D(bp, ep);
							segs.Add(nseg);
							bp = ep;
						}
					}
				}
			}

			IPolyLine3D polyLineDis = new PolyLine3D(segs);
			return GetArea(polyLineDis);
		}

		/// <summary>
		/// Returns the area of the given polygon.
		/// There are 3 conditions.
		/// 1. Polyline should be on a plane
		/// 2. Polyline should be closed
		/// 3. Polyline should not intersect by itself
		/// </summary>
		/// <param name="polyline">Closed polyline for which are has to be calculated</param>
		/// <returns>Area of given polyline</returns>
		public static double GetArea(IPolyLine3D polyline)
		{
			if (!IsCoplanar(polyline))
			{
				//throw new NotSupportedException("Given polyline is not in plane");
			}

			if (!polyline.IsClosed)
			{
				return 0.0;
			}

			if (IsSelfIntersect(polyline))
			{
				System.Diagnostics.Debug.Fail("NOT IMPLEMENTED - Given polyline is intersecting with itself");
				return 0;
			}

			Vector3D resultVect = new Vector3D();
			foreach (ISegment3D segment in polyline.Segments)
			{
				if (segment.SegmentType == SegmentType.Line)
				{
					Vector3D vector = ToVector(segment.StartPoint) * ToVector(segment.EndPoint);
					resultVect = resultVect + vector;
				}
				else if (segment.SegmentType == SegmentType.CircularArc)
				{
					IArcSegment3D arc = (IArcSegment3D)segment;
					if (arc == null)
					{
						throw new InvalidCastException("Wrong segment data");
					}

					Vector3D vector = ToVector(arc.StartPoint) * ToVector(arc.IntermedPoint);
					resultVect = resultVect + vector;
					vector = ToVector(arc.IntermedPoint) * ToVector(arc.EndPoint);
					resultVect = resultVect + vector;

					// TODO: Area of arc is not calculated correctly
					throw new NotImplementedException();
				}
				else
				{
					throw new NotSupportedException();
				}
			}

			Vector3D normal = new Vector3D();
			GetNormal(polyline, ref normal);
			return (normal | resultVect) / 2.0;
		}

		/// <summary>
		/// Get area vector of the given polyline
		/// </summary>
		/// <param name="polyline">Polyline for which normal has to be find</param>
		/// <param name="normal">Normal to the given polyline</param>
		/// <returns>True if polyline is in plane</returns>
		public static bool GetAreaVector(IPolyLine3D polyline, ref Vector3D normal)
		{
			IList<IPoint3D> points = new List<IPoint3D>();
			if (polyline.Count == 0)
			{
				return false;
			}

			bool isCollinear = IsCollinear(polyline, 0.00001);

			ConvertPolylineToPointList(polyline, points, true);

			if (IsEqual(points[0], points[points.Count - 1]))
			{
				points.RemoveAt(0);
			}

			if (points.Count < 2)
			{
				////throw new NotSupportedException("Wrong polyline");
				return false;
			}

			if (points.Count > 2 && isCollinear == false)
			{
				normal = new Vector3D();
				IPoint3D startPt = points[0];
				Vector3D vector1 = new Vector3D(points[1].X - startPt.X, points[1].Y - startPt.Y, points[1].Z - startPt.Z);
				for (int i = 2; i < points.Count; i++)
				{
					Vector3D vector2 = new Vector3D(points[i].X - startPt.X, points[i].Y - startPt.Y, points[i].Z - startPt.Z);
					if (GetAngle(vector1, vector2) < 0.05) // JAR 28.2.2019 - snizeni z puvodnich 0.1
					{
						continue;
					}

					normal = normal + (vector1 * vector2);
					if (normal.Magnitude.IsZero() == false)
					{
						break;
					}

					//vector1 = vector2;
				}

				normal = normal / 2;
				if (normal.Magnitude.IsZero() == false)
				{
					return true;
				}
			}

			Vector3D localX = Subtract(points[1], points[0]);
			Vector3D localY = new Vector3D(0, 1, 0);

			if (IsCollinear(localX, localY, 0.00001))
			{
				localY = new Vector3D(-1, 0, 0);
			}

			normal = (localX * localY).Normalize;
			return true;
		}

		public static bool GetBoundingPoints(IPolyLine3D polyline, ref IPoint3D first, ref IPoint3D second)
		{
			// TODO - slow method
			if (polyline == null)
			{
				return false;
			}

			IList<IPoint3D> points = new List<IPoint3D>();
			ConvertPolylineToPointList(polyline, points, false);

			if (points.Count == 0)
			{
				return false;
			}

			first = Clone(points[0]);
			second = Clone(points[0]);
			for (int i = 1; i < points.Count; i++)
			{
				first.X = Math.Min(first.X, points[i].X);
				first.Y = Math.Min(first.Y, points[i].Y);
				first.Z = Math.Min(first.Z, points[i].Z);
				second.X = Math.Max(second.X, points[i].X);
				second.Y = Math.Max(second.Y, points[i].Y);
				second.Z = Math.Max(second.Z, points[i].Z);
			}

			return true;
		}

		public static bool GetBoundingPoints(IList<IPolyLine3D> polyline, ref IPoint3D first, ref IPoint3D second)
		{
			if (polyline == null)
			{
				return false;
			}

			bool firstStep = true;
			foreach (var item in polyline)
			{
				IList<IPoint3D> points = new List<IPoint3D>();
				ConvertPolylineToPointList(item, points, false);

				if (points.Count == 0)
				{
					return false;
				}

				if (firstStep)
				{
					first = Clone(points[0]);
					second = Clone(points[0]);
				}

				for (int i = firstStep ? 1 : 0; i < points.Count; i++)
				{
					first.X = Math.Min(first.X, points[i].X);
					first.Y = Math.Min(first.Y, points[i].Y);
					first.Z = Math.Min(first.Z, points[i].Z);
					second.X = Math.Max(second.X, points[i].X);
					second.Y = Math.Max(second.Y, points[i].Y);
					second.Z = Math.Max(second.Z, points[i].Z);
				}

				firstStep = false;
			}

			return true;
		}

		/// <summary>
		/// Get a rectanglular polygon from two points
		/// The given two points should be on XY plane of the matrix
		/// </summary>
		/// <param name="matrix">New rectangle will be created on XY plane of matrix</param>
		/// <param name="center">Center of circle</param>
		/// <param name="pointOnCircle">Point on circle</param>
		/// <returns>Rectangle prepared from two points</returns>
		public static IPolyLine3D GetCircle(Matrix44 matrix, IPoint3D center, IPoint3D pointOnCircle)
		{
			if (GeomOperation.IsEqual(center, pointOnCircle))
			{
				return null;
			}

			center = matrix.TransformToLCS(center);
			pointOnCircle = matrix.TransformToLCS(pointOnCircle);
			Vector3D vector = Subtract(pointOnCircle, center);
			Vector3D normal = new Vector3D(0, 0, 1);

			IPoint3D point1 = Add(center, Rotate(vector, normal, Math.PI * 0.5));
			IPoint3D point2 = Add(center, Rotate(vector, normal, Math.PI));
			IPoint3D point3 = Add(center, Rotate(vector, normal, Math.PI * 1.5));

			IPolyLine3D circle = new PolyLine3D();
			circle.Add(new ArcSegment3D(pointOnCircle, point1, point2));
			circle.Add(new ArcSegment3D(point2, point3, pointOnCircle));

			TransformToGCS(matrix, circle);
			return circle;
		}

		/// <summary>
		/// Divide given polyline based on given distance and prepare a new polyline
		/// </summary>
		/// <param name="polyline">The input polyline</param>
		/// <param name="distance">Lenght of a segment</param>
		/// <returns>New polyline</returns>
		public static IPolyLine3D GetDividedSegments(IPolyLine3D polyline, double distance)
		{
			Vector3D normal = new Vector3D();
			GetNormal(polyline, ref normal);
			IPolyLine3D newPolyline = new PolyLine3D();
			foreach (ISegment3D segment in polyline.Segments)
			{
				GetDividedSegments(segment, normal, distance, newPolyline);
			}

			return newPolyline;
		}

		/// <summary>
		/// Divide given polyline into linear segments based on given angle and prepare a new polyline
		/// </summary>
		/// <param name="polyline">The input polyline</param>
		/// <param name="angle">Angle for dividing of curved segments (in degrees)</param>
		/// <returns>New polyline</returns>
		public static IPolyLine3D GetDividedArcSegments(IPolyLine3D polyline, double angle)
		{
			IList<IPoint3D> ptList = new List<IPoint3D>();
			GeomOperation.ConvertPolylineToPointList(polyline, ptList, false, angle);
			IList<ISegment3D> segLst = new List<ISegment3D>(ptList.Count);
			IPoint3D lastPt = null;
			IPoint3D frstPt = null;
			bool frst = true;
			foreach (IPoint3D pt in ptList)
			{
				if (frst)
				{
					frstPt = pt;
					lastPt = pt;
					frst = false;
					continue;
				}

				LineSegment3D seg = new LineSegment3D(lastPt, pt);
				lastPt = pt;
				segLst.Add(seg);
			}

			if (polyline.IsClosed)
			{
				segLst.Last().EndPoint = frstPt;
			}

			return new PolyLine3D(segLst);
		}

		/////// <summary>
		/////// Divide given polyline based on given distance and prepare a new polyline
		/////// A pattern specifies the length of segments that make up the linetype. 
		/////// A positive decimal number specifies a pen-down (dash) segment of that length. 
		/////// A negative decimal number specifies a pen-up (space) segment of that length
		/////// </summary>
		/////// <param name="polyline">The input polyline</param>
		/////// <param name="pattern">Pattern defines the type of line</param>
		/////// <returns>New polyline</returns>
		////public static IPolyLine3D GetDividedSegments(IPolyLine3D polyline, double[] pattern)
		////{
		////    Vector3D normal = new Vector3D();
		////    GetNormal(polyline, ref normal);
		////    int index = 0;
		////    double balance = 0;
		////    IPolyLine3D newPolyline = new PolyLine3D();
		////    foreach (ISegment3D segment in polyline.Segments)
		////    {
		////        balance = GetDividedSegments(segment, normal, pattern, balance, ref index, newPolyline);
		////    }

		////    return newPolyline;
		////}

		/// <summary>
		/// Prepare LCS at given position of polyline
		/// </summary>
		/// <param name="polyLine">Input polyline</param>
		/// <param name="relativePosition">Relative position on polyline</param>
		/// <returns>LCS matrix at given position</returns>
		public static Matrix44 GetLCSMatrix(IPolyLine3D polyLine, double relativePosition)
		{
			if (polyLine == null)
			{
				return null;
			}

			Vector3D localZ = new Vector3D(0, 0, 1);
			if (GetNormal(polyLine, ref localZ) == false)
			{
				Matrix44 matrix = new Matrix44();
				if (polyLine.Count > 0)
				{
					matrix.Origin = new Point3D(polyLine[0].StartPoint.X, polyLine[0].StartPoint.Y, polyLine[0].StartPoint.Z);
				}

				return matrix;
			}

			Vector3D localX = new Vector3D(1, 0, 0);
			IPoint3D origin = new Point3D();
			if (polyLine.Count > 0)
			{
				ISegment3D segment = GetSegmentAtPosition(polyLine, relativePosition, out double positionOnSeg, out int inx);
				if (segment != null)
				{
					if (GetTangentOnSegment(segment, positionOnSeg, ref localX) == false)
					{
						return null;
					}

					if (GetPointOnSegment(segment, positionOnSeg, ref origin) == false)
					{
						return null;
					}
				}
			}

			if (IsCollinear(localX, localZ))
			{
				if (IsCollinear(localX, new Vector3D(0, 1, 0)))
				{
					localX = new Vector3D(0, 0, 1);
				}
				else
				{
					localX = new Vector3D(0, 1, 0);
				}
			}

			//Vector3D localY = (localZ * localX).Normalize;
			//localX = (localY * localZ).Normalize;
			//Matrix44 lcs = new Matrix44(origin, localX, localY);
			Vector3D localY = (localZ * localX);
			localX = (localY * localZ);
			localZ = (localX * localY).Normalize;
			Matrix44 lcs = new Matrix44(origin, localX.Normalize, localY.Normalize, localZ);
			return lcs;
		}

		/// <summary>
		/// Calculate the Length of polyline
		/// </summary>
		/// <param name="polyline">polyline</param>
		/// <returns>Length of given polyline</returns>
		public static double GetLength(IPolyLine3D polyline)
		{
			if (polyline == null)
			{
				return 0;
			}

			double length = 0.0;
			foreach (ISegment3D segment in polyline.Segments)
			{
				length += GetLength(segment);
			}

			return length;
		}

		/// <summary>
		/// Calculate the Length of smallest segment of given polyline
		/// </summary>
		/// <param name="polyline">polyline</param>
		/// <returns>Length of smallest segment of given polyline</returns>
		public static double GetLengthOfSmallestSegment(IPolyLine3D polyline)
		{
			double length = 0.0;
			bool isFirst = true;
			foreach (ISegment3D segment in polyline.Segments)
			{
				if (isFirst)
				{
					length = GetLength(segment);
					isFirst = false;
				}
				else
				{
					length = Math.Min(length, GetLength(segment));
				}
			}

			return length;
		}

		/// <summary>
		/// Prepare new polyline by divide the arc segments into several linear segments
		/// </summary>
		/// <param name="polyline">PolyLine3D</param>
		/// <param name="distance">Maximum distance of a line segment which is created from curved segments</param>
		/// <returns>New PolyLine3D</returns>
		public static IPolyLine3D GetLinearSegments(IPolyLine3D polyline, double distance)
		{
			if (distance.IsZero())
			{
				return null;
			}

			Vector3D normal = new Vector3D();
			GeomOperation.GetNormal(polyline, ref normal);
			IPolyLine3D newPolyline = new PolyLine3D();
			foreach (ISegment3D segment in polyline.Segments)
			{
				if (segment.SegmentType == SegmentType.Line)
				{
					newPolyline.Add(segment.CloneSegment());
				}
				else if (segment.SegmentType == SegmentType.CircularArc || segment.SegmentType == SegmentType.Parabola)
				{
					GetDividedSegments(segment, normal, distance, newPolyline);
				}
				else
				{
					throw new NotSupportedException();
				}
			}

			return new PolyLine3D(newPolyline);
		}

		/// <summary>
		/// Prepare new polyline by divide the arc segments into several linear segments
		/// </summary>
		/// <param name="polyline">PolyLine3D</param>
		/// <param name="numberOfParts">Number of linear parts</param>
		/// <returns>New PolyLine3D</returns>
		public static IPolyLine3D GetLinearSegments(IPolyLine3D polyline, int numberOfParts)
		{
			if (numberOfParts <= 0)
			{
				return null;
			}

			Vector3D normal = new Vector3D();
			GeomOperation.GetNormal(polyline, ref normal);
			IPolyLine3D newPolyline = new PolyLine3D();
			foreach (ISegment3D segment in polyline.Segments)
			{
				if (segment.SegmentType == SegmentType.Line)
				{
					newPolyline.Add(segment.CloneSegment());
				}
				else if (segment.SegmentType == SegmentType.CircularArc || segment.SegmentType == SegmentType.Parabola)
				{
					double length = GetLength(segment);
					double distance = length / numberOfParts;
					GeomOperation.GetDividedSegments(segment, normal, distance, newPolyline);
				}
				else
				{
					throw new NotSupportedException();
				}
			}

			return new PolyLine3D(newPolyline);
		}

		/// <summary>
		/// Get nearest point on polyline
		/// </summary>
		/// <param name="polyline">Input Polyline</param>
		/// <param name="point">Input point</param>
		/// <returns>The point on polyline</returns>
		public static IPoint3D GetNearestPoint(IPolyLine3D polyline, IPoint3D point)
		{
			if (polyline == null || polyline.Count == 0)
			{
				return null;
			}

			Vector3D normal = new Vector3D();
			if (GetNormal(polyline, ref normal) == false)
			{
				return null;
			}

			IPoint3D pointOnPolyline = Clone(polyline[0].StartPoint);
			double minDistance = GeomOperation.Subtract(pointOnPolyline, point).Magnitude;
			foreach (ISegment3D segment in polyline.Segments)
			{
				IPoint3D newPoint = null;
				if (segment.SegmentType == SegmentType.Line)
				{
					newPoint = GetNearestPoint(segment as ILineSegment3D, point);
				}
				else if (segment.SegmentType == SegmentType.CircularArc)
				{
					newPoint = GetNearestPoint(segment as IArcSegment3D, normal, point);
				}
				else
				{
					continue;
				}

				double distance = GeomOperation.Subtract(newPoint, point).Magnitude;
				if (minDistance.IsGreater(distance))
				{
					minDistance = distance;
					pointOnPolyline = newPoint;
				}
			}

			return pointOnPolyline;
		}

		/// <summary>
		/// Get normal of the given polyline
		/// </summary>
		/// <param name="polyline">Polyline for which normal has to be find</param>
		/// <param name="normal">Normal to the given polyline</param>
		/// <returns>True if polyline is in plane</returns>
		public static bool GetNormal(IPolyLine3D polyline, ref Vector3D normal)
		{
			bool isInPlane = GetAreaVector(polyline, ref normal);
			if (normal.Magnitude.IsZero())
			{
				return false;
			}

			normal = normal.Normalize;
			return isInPlane;
		}

		/// <summary>
		/// Get a rectanglular polygon from two points
		/// The given two points should be on XY plane of the matrix
		/// </summary>
		/// <param name="matrix">New rectangle will be created on XY plane of matrix</param>
		/// <param name="point1">First diagonal point [on local XY plane of matrix]</param>
		/// <param name="point3">Second diagonal point [on local XY plane of matrix]</param>
		/// <returns>Rectangle prepared from two points</returns>
		public static IPolyLine3D GetRectangle(IMatrix44 matrix, IPoint3D point1, IPoint3D point3)
		{
			if (GeomOperation.IsEqual(point1, point3))
			{
				return null;
			}

			point1 = matrix.TransformToLCS(point1);
			point3 = matrix.TransformToLCS(point3);

			IPoint3D point2 = new Point3D();
			IPoint3D point4 = new Point3D();

			point2.X = point3.X;
			point2.Y = point1.Y;
			point4.X = point1.X;
			point4.Y = point3.Y;

			IPolyLine3D rectangle = new PolyLine3D();
			rectangle.Add(new LineSegment3D(point1, point2));
			rectangle.Add(new LineSegment3D(point2, point3));
			rectangle.Add(new LineSegment3D(point3, point4));
			rectangle.Add(new LineSegment3D(point4, point1));

			TransformToGCS(matrix, rectangle);
			return rectangle;
		}

		/// <summary>
		/// Calculate relative position of point on polyline.
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="point">Point</param>
		/// <param name="relativePosition">Relative Position</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>True if point exist in polyline</returns>
		public static bool GetRelativePosition(IPolyLine3D polyline, IPoint3D point, ref double relativePosition, double toleranceLevel = MathConstants.ZeroWeak)
		{
			double partialLength = 0;
			foreach (ISegment3D segment in polyline.Segments)
			{
				double segmentPosition = 0;
				if (IsPointOnSegment(segment, point, ref segmentPosition, toleranceLevel))
				{
					double segmentLength = GetLength(segment);
					partialLength += segmentLength * segmentPosition;
					double totalLength = GetLength(polyline);
					relativePosition = partialLength / totalLength;
					return true;
				}

				partialLength += GetLength(segment);
			}

			return false;
		}

		/// <summary>
		/// Gets a segment at relative position
		/// TODO Use GetSegmentIndexOnPolyline here
		/// </summary>
		/// <param name="polyLine">Polyline</param>
		/// <param name="relativePosition">Relative position</param>
		/// <param name="relativePositionOnSegment">return Relative position on segment</param>
		/// <param name="inxOfSegment">return index of segment in the polyline</param>
		/// <returns>segment or null</returns>
		public static ISegment3D GetSegmentAtPosition(IPolyLine3D polyLine, double relativePosition, out double relativePositionOnSegment, out int inxOfSegment)
		{
			inxOfSegment = -1;
			relativePositionOnSegment = 0.0;
			double totalLength = GetLength(polyLine);
			if (totalLength.IsZero())
			{
				return null;
			}

			double length = 0;
			for (int i = 0; i < polyLine.Count; i++)
			{
				ISegment3D segment = polyLine[i];
				double lengthSegment = GetLength(segment);
				length += lengthSegment;
				double endPosition = length / totalLength;
				if (relativePosition.IsLesserOrEqual(endPosition))
				{
					relativePositionOnSegment = (relativePosition * totalLength - (length - lengthSegment)) / lengthSegment;
					inxOfSegment = i;
					return segment;
				}
			}

			if (relativePosition.IsGreater(1.0))
			{
				//zalezi jak moc
				double d = (relativePosition - 1.0) * totalLength;
				if (d.IsZero(1e-4))
				{
					inxOfSegment = polyLine.Count - 1;
					return polyLine[inxOfSegment];
				}
			}

			return null;
		}

		/// <summary>
		/// Gets a segment at relative position
		/// TODO Use GetSegmentIndexOnPolyline here
		/// </summary>
		/// <param name="polyLine">Polyline</param>
		/// <param name="relativePosition">Relative position</param>
		/// <param name="relativePositionOnSegment">return Relative position on segment</param>
		/// <param name="inxOfSegment">return index of segment in the polyline</param>
		/// <returns>segment or null</returns>
		public static ISegment3D GetSegmentAtPositionNoTest(IPolyLine3D polyLine, double relativePosition, out double relativePositionOnSegment, out int inxOfSegment)
		{
			inxOfSegment = -1;
			relativePositionOnSegment = 0.0;
			double totalLength = GetLength(polyLine);
			if (totalLength.IsZero())
			{
				return null;
			}

			double length = 0;
			for (int i = 0; i < polyLine.Count; i++)
			{
				ISegment3D segment = polyLine[i];
				double lengthSegment = GetLength(segment);
				length += lengthSegment;
				double endPosition = length / totalLength;
				if (relativePosition.IsLesserOrEqual(endPosition))
				{
					relativePositionOnSegment = (relativePosition * totalLength - (length - lengthSegment)) / lengthSegment;
					inxOfSegment = i;
					return segment;
				}
			}

			if (relativePosition.IsGreater(1.0))
			{
				//zalezi jak moc
				double d = (relativePosition - 1.0) * totalLength;
				{
					inxOfSegment = polyLine.Count - 1;
					ISegment3D segment = polyLine[inxOfSegment];
					double lengthSegment = GetLength(segment);
					relativePositionOnSegment = (relativePosition * totalLength - (length - lengthSegment)) / lengthSegment;
					return polyLine[inxOfSegment];
				}
			}

			return null;
		}

		/// <summary>
		/// Get Segment Index from polyline based on a given Point.
		/// </summary>
		/// <param name="polyLine">Geometry Polyline</param>
		/// <param name="pointOnSegment">Point which lies on polyline segment</param>
		/// <param name="relativePositionOnSegment">Relative position of Point on a segment</param>
		/// <returns>Segment Index of polyline in which given point lies </returns>
		public static int GetSegmentIndexOnPolyline(IPolyLine3D polyLine, IPoint3D pointOnSegment, ref double relativePositionOnSegment)
		{
			for (int i = 0; i < polyLine.Count; i++)
			{
				double relativePosition = 0;
				if (IsPointOnSegment(polyLine[i], pointOnSegment, ref relativePosition))
				{
					relativePositionOnSegment = relativePosition;
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Gets index of segment within polyline
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="segment">Segment</param>
		/// <returns>Index of segment. -1 if segment is not found</returns>
		public static int GetIndexOfSegment(IPolyLine3D polyline, ISegment3D segment)
		{
			int count = polyline.Count;
			for (int i = 0; i < count; i++)
			{
				if (polyline[i].Equals(segment))
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Intersect between extended segment and polyline
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="segment">Segment</param>
		/// <param name="relativePositions1">Polyline - Relative position for points on polyline</param>
		/// <param name="relativePositions2">Segment - Relative position for points on polyline </param>
		/// <returns>True if it intersects</returns>
		public static bool Intersect(IPolyLine3D polyline, ISegment3D segment, List<double> relativePositions1, List<double> relativePositions2)
		{
			if (polyline == null || segment == null)
			{
				return false;
			}

			bool isIntersects = false;
			double polylineLength = GetLength(polyline);
			double length = 0;
			foreach (ISegment3D polylineSegment in polyline.Segments)
			{
				List<double> segment1RelativePosition = new List<double>();
				List<double> segment2RelativePosition = new List<double>();
				if (IntersectSegment(polylineSegment, segment, segment1RelativePosition, segment2RelativePosition))
				{
					for (int i = 0; i < segment1RelativePosition.Count; i++)
					{
						double relativePosition = segment1RelativePosition[i];
						if (relativePosition.IsLesser(0) || relativePosition.IsGreater(1))
						{
							continue;
						}

						double segmentLength = GetLength(polylineSegment) * relativePosition + length;
						double segmentRelativePosition = segmentLength / polylineLength;
						isIntersects = true;
						if (relativePositions1 != null)
						{
							relativePositions1.Add(segmentRelativePosition);
						}

						if (relativePositions2 != null)
						{
							relativePositions2.Add(segment2RelativePosition[i]);
						}
					}
				}

				length += GetLength(polylineSegment);
			}

			return isIntersects;
		}

		/// <summary>
		/// Converts PolyLine3D to Windows points
		/// </summary>
		/// <param name="polyline">PolyLine3D</param>
		/// <returns>List of windows points</returns>
		public static List<System.Windows.Point> PolylineToWindowsPoints(IPolyLine3D polyline)
		{
			Vector3D localZ = new Vector3D();
			if (GetNormal(polyline, ref localZ) == false)
			{
				return null;
			}

			List<System.Windows.Point> points = new List<System.Windows.Point>();
			int count = polyline.Count;
			IPoint3D point3D = null;
			if (count > 0)
			{
				point3D = polyline[0].StartPoint;
				points.Add(new System.Windows.Point(point3D.X, point3D.Y));
			}

			foreach (ISegment3D segment in polyline.Segments)
			{
				if (segment.SegmentType == SegmentType.CircularArc || segment.SegmentType == SegmentType.Parabola)
				{
					IArcSegment3D arc = segment as IArcSegment3D;
					if (arc == null)
					{
						throw new InvalidCastException("Wrong segment data");
					}

					point3D = arc.IntermedPoint;
					points.Add(new System.Windows.Point(point3D.X, point3D.Y));
				}

				point3D = segment.EndPoint;
				points.Add(new System.Windows.Point(point3D.X, point3D.Y));
			}

			// Remove last point
			if (polyline.IsClosed && points.Count > 0)
			{
				points.RemoveAt(points.Count - 1);
			}

			return points;
		}

		/// <summary>
		/// Calculate Points for polyline at a given interval
		/// </summary>
		/// <param name="polyline">Geometry Polyline</param>
		/// <param name="interval">Length of Interval</param>
		/// <param name="listOfPoint">List of Points</param>
		/// <param name="endPoints">Add Start and Endpoints of segment if True</param>
		public static void PointsAtInterval(IPolyLine3D polyline, double interval, ICollection<IPoint3D> listOfPoint, bool endPoints = false)
		{
			if (listOfPoint == null || polyline == null)
			{
				return;
			}

			if (polyline.Count == 0)
			{
				return;
			}

			if (interval.IsLesserOrEqual(0) || interval.IsGreaterOrEqual(1))
			{
				return;
			}

			foreach (ISegment3D segment in polyline.Segments)
			{
				double offset = interval;
				while (offset.IsLesser(1))
				{
					IPoint3D relativePoint = new Point3D();
					GetPointOnSegment(segment, offset, ref relativePoint);
					listOfPoint.Add(relativePoint);
					offset += interval;
				}

				if (endPoints)
				{
					listOfPoint.Add(segment.StartPoint);
					listOfPoint.Add(segment.EndPoint);
				}
			}
		}

		/// <summary>
		/// Calculate points at a given position for each segment
		/// </summary>
		/// <param name="polyline">Geometry Polyline</param>
		/// <param name="relativePosition">Relative position of each segment</param>
		/// <param name="listOfPoint">List of points</param>
		public static void PointsAtPosition(IPolyLine3D polyline, double relativePosition, ICollection<IPoint3D> listOfPoint)
		{
			if (listOfPoint == null || polyline == null)
			{
				return;
			}

			if (polyline.Count == 0)
			{
				return;
			}

			foreach (ISegment3D segment in polyline.Segments)
			{
				IPoint3D relativePoint = new Point3D();
				GetPointOnSegment(segment, relativePosition, ref relativePoint);
				listOfPoint.Add(relativePoint);
			}
		}

		/// <summary>
		/// Converts array of IPolygonReader to list of IPolyLine3D
		/// </summary>
		/// <param name="polyReaderArray">Array of IPolygonReader</param>
		/// <param name="polylines">List of PolyLine3Ds</param>
		/// <returns>True if converts properly</returns>
		public static bool PolygonReaderToPolylines(IPolygonReader[] polyReaderArray, ICollection<IPolyLine3D> polylines)
		{
			if (polylines == null)
			{
				return false;
			}

			foreach (GiCL2D.IPolygonReader polyReader in polyReaderArray)
			{
				if (polyReader.Length == 0)
				{
					continue;
				}

				IPolyLine3D polyline = new PolyLine3D();
				IPoint3D lastPoint = null;
				for (int j = 0; j < polyReader.Length; j++)
				{
					polyReader.GetRow(j, out double x, out double y);

					IPoint3D point = new Point3D
					{
						X = x,
						Y = y
					};

					if (lastPoint != null)
					{
						polyline.Add(new LineSegment3D(lastPoint, point));
					}

					lastPoint = point;
				}

				polylines.Add(new PolyLine3D(polyline));
			}

			return true;
		}

		/// <summary>
		/// Creates offset of IPolyLine3D
		/// </summary>
		/// <param name="polyline">Source</param>
		/// <param name="offsetLine">Target</param>
		/// <param name="offset">Offset value</param>
		/// <returns>True if succeffull</returns>
		public static bool GetOffset(IPolyLine3D polyline, ref IPolyLine3D offsetLine, double offset)
		{
			IList<IPolyLine3D> offsetLines = null;
			if (GetOffset(polyline, ref offsetLines, offset, 0, true, true))
			{
				offsetLine = new PolyLine3D();
				foreach (var line in offsetLines)
				{
					Append(offsetLine, line);
				}
				offsetLine.Close();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Get offset of the polyline
		/// </summary>
		/// <param name="polyline">Original Polyline to find offset</param>
		/// <param name="offsetLines">Offset of the given polyline</param>
		/// <param name="eY">Offset along Y axis</param>
		/// <param name="eZ">Offset along Z axis</param>
		/// <param name="isClosed">Specify that given polyline is closed</param>
		/// <param name="isOutline">isOutline ?</param>
		/// <returns>True if new offset is created</returns>
		public static bool GetOffset(IPolyLine3D polyline, ref IList<IPolyLine3D> offsetLines, double eY, double eZ, bool isClosed = false, bool isOutline = true)
		{
			offsetLines = new List<IPolyLine3D>();
			if (polyline.Count == 0)
			{
				return true;
			}

			IPolyLine3D newPolyline = new PolyLine3D(polyline);
			if (eY.IsZero() && eZ.IsZero())
			{
				offsetLines.Add(new PolyLine3D(newPolyline));
				return true;
			}

			if (isClosed == true)
			{
				isClosed = newPolyline.IsClosed;
			}

			Matrix44 matrix = GetLCSMatrix(newPolyline, 0);
			if (matrix == null)
			{
				return false;
			}

			TransformToLCS(matrix, newPolyline);

			IPolyLine3D linearSegments = GetLinearSegments(newPolyline, 18);

			IList<IPoint3D> points = new List<IPoint3D>();
			ConvertPolylineToPointList(linearSegments, points, false);

			List<System.Windows.Point> list = new List<System.Windows.Point>();
			foreach (IPoint3D point in points)
			{
				list.Add(new System.Windows.Point(point.X, point.Y));
			}

			if (isClosed)
			{
				if (points.Count > 0)
				{
					list.RemoveAt(points.Count - 1);
				}
				else
				{
					return false;
				}
			}

			////List<System.Windows.Point> list = PolylineToWindowsPoints(newPolyline);
			IPolygonReader polygonReader = new BoxListPoint(list);
			PolygonOffset polygon2DOffset = new PolygonOffset(polygonReader, isClosed, isOutline);
			IPolygonReader[] polyReaderArray = polygon2DOffset.MakeOffset(eY);

			if (polyReaderArray == null && polyReaderArray.Length <= 0)
			{
				return false;
			}

			////offsetLine = new PolyLine3D();
			////if (PolygonReaderToPolyline(polyReaderArray, offsetLine) == false)
			////{
			////    return false;
			////}

			IList<IPolyLine3D> polylines = new List<IPolyLine3D>();
			if (PolygonReaderToPolylines(polyReaderArray, polylines) == false)
			{
				return false;
			}

			if (polylines.Count == 0)
			{
				return false;
			}

			foreach (IPolyLine3D offsetLine in polylines)
			{
				if (isClosed)
				{
					offsetLine.Close();
				}

				Move(offsetLine, new Vector3D(0, 0, eZ));
				TransformToGCS(matrix, offsetLine);
				offsetLines.Add(offsetLine);
			}

			return true;
		}

		/// <summary>
		/// Get offset of the polyline
		/// </summary>
		/// <param name="polyline">Original Polyline to find offset</param>
		/// <param name="offsetLines">Offset of the given polyline</param>
		/// <param name="eY">Offset along Y axis</param>
		/// <param name="eZ">Offset along Z axis</param>
		/// <param name="isClosed">Specify that given polyline is closed</param>
		/// <param name="isOutline">isOutline ?</param>
		/// <returns>True if new offset is created</returns>
		public static bool GetOffset(IList<IPolyLine3D> polyline, ref IList<IPolyLine3D> offsetLines, double eY, double eZ, bool isClosed = false, bool isOutline = true)
		{
			bool retval = true;
			offsetLines = new List<IPolyLine3D>();
			foreach (var item in polyline)
			{
				IList<IPolyLine3D> offsetLinesItem = null;
				if (GetOffset(item, ref offsetLinesItem, eY, eZ, isClosed, isOutline))
				{
					foreach (var off in offsetLinesItem)
					{
						offsetLines.Add(off);
					}
				}
				else
				{
					retval = false;
				}
			}

			return retval;
		}

		/// <summary>
		/// Prepare a part polyline from a given polyline based on the intervals
		/// </summary>
		/// <param name="polyline">Input polyline from which we prepare part polyline</param>
		/// <param name="x1">Relative starting position of the part. [Range 0 to 1]</param>
		/// <param name="x2">Relative ending position of the part. [Range 0 to 1]</param>
		/// <returns>Part polyline</returns>
		public static IPolyLine3D GetPartPolyline(IPolyLine3D polyline, double x1, double x2)
		{
			//tato p5esnot tady byla dodana kvuli opravě chyby https://svn.idea-rs.com/redmine/issues/15445  
			var precission =  1e-6;

			if (x1.IsLesser(0, precission) | x1.IsGreater(x2, precission))
			{
				throw new NotSupportedException("Relative start position ranges from 0 to x2");
			}
			else if (x2.IsGreater(1, precission))
			{
				throw new NotSupportedException("Relative end position ranges from 0 to x2");
			}
			else if (x1.IsEqual(0, precission) && x2.IsEqual(1, precission))
			{
				return polyline;
			}

			IPolyLine3D partPolyline = new PolyLine3D();
			IList<ISegment3D> segments = GetPartialPolyline(polyline, x1, x2);
			if (segments == null)
			{
				return partPolyline;
			}

			foreach (ISegment3D segment in segments)
			{
				partPolyline.Add(segment);
			}

			return partPolyline;
		}

		/// <summary>
		/// Prepare a part polyline from a given polyline based on the intervals
		/// </summary>
		/// <param name="polyline">Input polyline from which we prepare part polyline</param>
		/// <param name="x1">Relative starting position of the part. [Range 0 to 1]</param>
		/// <param name="x2">Relative ending position of the part. [Range 0 to 1]</param>
		/// <returns>Partial polyline as segments list</returns>
		public static IList<ISegment3D> GetPartialPolyline(IPolyLine3D polyline, double x1, double x2)
		{
			List<ISegment3D> segments = new List<ISegment3D>();
			if (polyline == null)
			{
				return segments;
			}

			double length = GetLength(polyline);
			double start = length * x1;
			double end = length * x2;

			foreach (ISegment3D segment in polyline.Segments)
			{
				double segLength = GetLength(segment);

				if (start.IsLesser(segLength))
				{
					ISegment3D newSegment = GeomOperation.Clone(segment);

					double relativeStart = start / segLength;
					double relativeEnd = end / segLength;
					if (relativeStart.IsGreater(0))
					{
						IPoint3D newStart = null;
						GetPointOnSegment(segment, relativeStart, ref newStart);
						newSegment.StartPoint = newStart;
					}
					else
					{
						newSegment.StartPoint = segment.StartPoint;
					}

					if (relativeEnd.IsLesser(1))
					{
						IPoint3D newEnd = null;
						GetPointOnSegment(segment, relativeEnd, ref newEnd);
						newSegment.EndPoint = newEnd;
					}
					else
					{
						newSegment.EndPoint = segment.EndPoint;
					}

					if (segment.SegmentType == SegmentType.CircularArc)
					{
						double arcRelativePosition = 0;
						IsPointOnSegment(segment, (segment as IArcSegment3D).IntermedPoint, ref arcRelativePosition);
						IArcSegment3D arc = newSegment as IArcSegment3D;
						if (relativeStart.IsGreaterOrEqual(arcRelativePosition) || relativeEnd.IsLesserOrEqual(arcRelativePosition))
						{
							IPoint3D newMid = null;
							GetPointOnSegment(segment, (relativeStart + relativeEnd) * 0.5, ref newMid);
							arc.IntermedPoint = newMid;
						}
					}

					segments.Add(newSegment);
				}

				start -= segLength;
				end -= segLength;
				if (end.IsLesserOrEqual(0))
				{
					break;
				}
			}

			return segments;
		}

		/// <summary>
		/// Get new point on arc 
		/// </summary>
		/// <param name="center">Center of arc</param>
		/// <param name="start">Start point of arc</param>
		/// <param name="arcLength">Arc distance from start to end</param>
		/// <param name="normal">Normal to the plane of arc</param>
		/// <param name="newEnd">New end point on arc</param>
		public static void GetPointOnArc(IPoint3D center, IPoint3D start, double arcLength, Vector3D normal, ref IPoint3D newEnd)
		{
			Vector3D vector = Subtract(start, center);
			double radius = vector.Magnitude;
			double angle = arcLength / radius;
			vector = Rotate(vector, normal, angle);
			newEnd = Add(center, vector);
		}

		/// <summary>
		/// Calculate Point on PolyLine based on Relative position
		/// </summary>
		/// <param name="polyLine">PolyLine</param>
		/// <param name="relativePosition">Relative Position</param>
		/// <returns>Point on Polyline</returns>
		public static IPoint3D GetPointOnPolyLine(IPolyLine3D polyLine, double relativePosition)
		{
			////TODO : Use GetSegmentAtPosition in this function
			////double positionOnSeg = 0;
			////ISegment3D segment = GetSegmentAtPosition(polyline, relativePosition, out positionOnSeg);

			double totalLength = GetLength(polyLine);
			double length = 0;

			foreach (ISegment3D segment in polyLine.Segments)
			{
				length += GetLength(segment);
				double endPosition = length / totalLength;
				if (relativePosition.IsLesserOrEqual(endPosition))
				{
					length -= GetLength(segment);
					double startPosition = length / totalLength;
					double segmentPosition = (relativePosition - startPosition) / (endPosition - startPosition);
					IPoint3D point = new Point3D();
					GetPointOnSegment(segment, segmentPosition, ref point);
					return point;
				}
			}

			return null;
		}

		/// <summary>
		/// Split polyline in to list of segments based on given relative position sorted list
		/// </summary>
		/// <param name="polyline">Polyline3D</param>
		/// <param name="relativePositionSet">List of sorted relative position set</param>
		/// <returns>List of splitted segments</returns>
		public static IList<ISegment3D> SplitPolyline(IPolyLine3D polyline, SortedSet<double> relativePositionSet)
		{
			List<ISegment3D> partialPolylineSegments = new List<ISegment3D>();
			if (polyline == null || relativePositionSet == null)
			{
				return partialPolylineSegments;
			}

			List<double> relativePositionList = new List<double>();
			foreach (double relativePosition in relativePositionSet)
			{
				relativePositionList.Add(relativePosition);
			}

			for (int i = 1; i < relativePositionList.Count; i++)
			{
				double startRelativePosition = relativePositionList[i - 1];
				double endRelativePosition = relativePositionList[i];
				IList<ISegment3D> segments = GetPartialPolyline(polyline, startRelativePosition, endRelativePosition);
				if (segments != null)
				{
					partialPolylineSegments.AddRange(segments);
				}
			}

			return partialPolylineSegments;
		}

		/// <summary>
		/// Prepare a polygon without reflex angles
		/// If any segment exists with reflex angle, divide it into two
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <returns>New Polyline without reflex angle</returns>
		public static IPolyLine3D SplitSegmentWithReflexAngle(IPolyLine3D polyline)
		{
			IPolyLine3D newPolyLine = new PolyLine3D();
			Vector3D normal = new Vector3D();
			GetNormal(polyline, ref normal);
			foreach (ISegment3D segment in polyline.Segments)
			{
				if (segment.SegmentType == SegmentType.Line)
				{
					newPolyLine.Add(new LineSegment3D(segment as LineSegment3D));
				}
				else if (segment.SegmentType == SegmentType.CircularArc)
				{
					IArcSegment3D arcSegment = segment as IArcSegment3D;
					double angle = GetArcAngle(arcSegment);
					if (angle.IsLesserOrEqual(Math.PI))
					{
						newPolyLine.Add(new ArcSegment3D(arcSegment));
					}
					else
					{
						// if reflex angle
						IPoint3D point1 = null;
						IPoint3D point2 = null;
						IPoint3D point3 = null;
						GetPointOnSegment(arcSegment, 0.25, ref point1);
						GetPointOnSegment(arcSegment, 0.50, ref point2);
						GetPointOnSegment(arcSegment, 0.75, ref point3);
						newPolyLine.Add(new ArcSegment3D(arcSegment.StartPoint, point1, point2));
						newPolyLine.Add(new ArcSegment3D(point2, point3, arcSegment.EndPoint));
					}
				}
				else
				{
					throw new NotSupportedException();
				}
			}

			return newPolyLine;
		}

		/// <summary>
		/// Get a polygon surrounding the given polyline with an offset
		/// </summary>
		/// <param name="polyline">Input polyline</param>
		/// <param name="offset">Offset distance between given polyline and surrounding polygon</param>
		/// <returns>Surrounding polygon</returns>
		public static IPolyLine3D GetSurroundingPolygon(IPolyLine3D polyline, double offset)
		{
			if (polyline.Count == 0)
			{
				return null;
			}

			IList<IPolyLine3D> offsetLines = null;
			if (GeomOperation.GetOffset(polyline, ref offsetLines, offset, 0) == false)
			{
				return null;
			}

			IPolyLine3D polyline1 = null;
			if (offsetLines == null || offsetLines.Count == 0)
			{
				return null;
			}

			polyline1 = offsetLines[0];
			if (GeomOperation.GetOffset(polyline, ref offsetLines, -offset, 0) == false)
			{
				return null;
			}

			IPolyLine3D polyline2 = null;
			if (offsetLines == null || offsetLines.Count == 0)
			{
				return null;
			}

			polyline2 = offsetLines[0];
			if (polyline1.Count == 0 || polyline2.Count == 0)
			{
				return null;
			}

			polyline2 = GeomOperation.Reverse(polyline2);
			IPoint3D begin = polyline1[polyline1.Count - 1].EndPoint;
			IPoint3D end = polyline2[0].StartPoint;
			ILineSegment3D jointEnd = new LineSegment3D(begin, end);
			begin = polyline2[polyline2.Count - 1].EndPoint;
			end = polyline1[0].StartPoint;
			ILineSegment3D jointBegin = new LineSegment3D(begin, end);

			polyline1.Add(jointEnd);
			GeomOperation.Append(polyline1, polyline2);
			polyline1.Add(jointBegin);

			return polyline1;
		}

		/// <summary>
		/// Calculates the tangent of polyline for a given offset.
		/// </summary>
		/// <param name="polyline">Input polyline</param>
		/// <param name="relativePosition">relative position on polyline</param>
		/// <param name="tangentVector">tangent vector segment</param>
		/// <returns>True if tangent Vector exists for a given polyline</returns>
		public static bool GetTangent(IPolyLine3D polyline, double relativePosition, ref Vector3D tangentVector)
		{
			ISegment3D segment = GetSegmentAtPosition(polyline, relativePosition, out double positionOnSeg, out int inx);
			if (segment != null)
			{
				return GetTangentOnSegment(segment, positionOnSeg, ref tangentVector);
			}

			return false;
		}

		/// <summary>
		/// Validates the polyline. Checks arc segments if they can be exchanged by linear segment, checks if linear segment length is not zero.
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <returns>Validated Polyline</returns>
		public static IPolyLine3D GetValidPolyLine(IPolyLine3D polyline)
		{
			IPolyLine3D newPolyLine = new PolyLine3D();
			foreach (ISegment3D segment in polyline.Segments)
			{
				if (segment.SegmentType == SegmentType.CircularArc)
				{
					IArcSegment3D arcSegment = segment as IArcSegment3D;
					if (GeomOperation.IsCollinear(arcSegment))
					{
						newPolyLine.Add(new LineSegment3D(arcSegment.StartPoint, arcSegment.EndPoint));
						continue;
					}
				}

				if (segment.SegmentType == SegmentType.Line)
				{
					if (!IsValid(segment))
					{
						continue;
					}
				}

				newPolyLine.Add(Clone(segment));
			}

			return newPolyLine;
		}

		/// <summary>
		/// Read value from the array at given index.
		/// Set next index as in circular list
		/// </summary>
		/// <param name="values">Array of values</param>
		/// <param name="index">Read value at this index</param>
		/// <returns>Value at given index</returns>
		public static double GetValue(double[] values, ref int index)
		{
			double value = 0;
			int count = values.GetLength(0);
			if (index >= count)
			{
				index = 0;
			}

			if (count > 0)
			{
				value = values[index];
				if (value.IsZero(MathConstants.ZeroGeneral))
				{
					throw new NotSupportedException();
				}

				if (index < count)
				{
					index++;
				}
				else
				{
					index = 0;
				}
			}

			return value;
		}

		/// <summary>
		/// Insert Point into Polyline Segments (i.e., Divide Polyline segment)
		/// </summary>
		/// <param name="polyline">Polyline whose segment is to be divided by inserting point</param>
		/// <param name="point">Point to be inserted into polyline segment to divide that particular segment</param>
		/// <returns>List of new point created after dividing the node</returns>
		public static List<IPoint3D> InsertPointOn(IPolyLine3D polyline, IPoint3D point)
		{
			List<IPoint3D> insertedPoints = new List<IPoint3D>();
			if (polyline == null)
			{
				return insertedPoints;
			}

			double relativePositionOnSegment = 0;
			int segmentIndex = GetSegmentIndexOnPolyline(polyline, point, ref relativePositionOnSegment);
			int count = polyline.Count;
			if (segmentIndex < 0 || segmentIndex > count)
			{
				return insertedPoints;
			}

			ISegment3D segment = polyline[segmentIndex];
			if (segment.SegmentType == SegmentType.Line)
			{
				ISegment3D startSegment = new LineSegment3D(segment.StartPoint, point);
				ISegment3D endSegment = new LineSegment3D(point, segment.EndPoint);
				polyline.Insert(segmentIndex, endSegment);
				polyline.Insert(segmentIndex, startSegment);
				polyline.RemoveAt(segmentIndex + 2);
				insertedPoints.Add(point);
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				double intermediatePosition = 0;
				GeomOperation.IsPointOnSegment(segment, (segment as ArcSegment3D).IntermedPoint, ref intermediatePosition);
				if (relativePositionOnSegment.IsLesserOrEqual(0) || relativePositionOnSegment.IsGreaterOrEqual(1) || relativePositionOnSegment.IsEqual(intermediatePosition))
				{
					return insertedPoints;
				}

				IPoint3D intermediatePoint = new Point3D();
				ISegment3D startSegment = null;
				ISegment3D endSegment = null;
				if (relativePositionOnSegment.IsGreater(intermediatePosition))
				{
					startSegment = new ArcSegment3D(segment.StartPoint, (segment as ArcSegment3D).IntermedPoint, point);
					double position = (relativePositionOnSegment + 1) / 2;
					GetPointOnSegment(segment, position, ref intermediatePoint);
					endSegment = new ArcSegment3D(point, intermediatePoint, segment.EndPoint);
				}
				else
				{
					double position = relativePositionOnSegment / 2;
					GetPointOnSegment(segment, position, ref intermediatePoint);
					startSegment = new ArcSegment3D(segment.StartPoint, intermediatePoint, point);
					endSegment = new ArcSegment3D(point, (segment as ArcSegment3D).IntermedPoint, segment.EndPoint);
				}

				polyline.Insert(segmentIndex, endSegment);
				polyline.Insert(segmentIndex, startSegment);
				polyline.RemoveAt(segmentIndex + 2);
				insertedPoints.Add(point);
				insertedPoints.Add(intermediatePoint);
			}

			return insertedPoints;
		}

		/// <summary>
		/// Calculate list of Intersection points for two polyline
		/// </summary>
		/// <param name="polyline1">Polyline to find intersection with Poyline2</param>
		/// <param name="polyline2">Polyline to find intersection with Poyline1</param>
		/// <param name="listOfIntersectionPoint">List of intersection points prepared from the given two polyline</param>
		public static void Intersect(IPolyLine3D polyline1, IPolyLine3D polyline2, List<IPoint3D> listOfIntersectionPoint)
		{
			if (polyline1 == null || polyline2 == null || listOfIntersectionPoint == null)
			{
				return;
			}

			foreach (ISegment3D innerSegment in polyline2.Segments)
			{
				foreach (ISegment3D outerSegment in polyline1.Segments)
				{
					IList<IPoint3D> intersectionPoint = IntersectSegment(innerSegment, outerSegment);
					if (intersectionPoint.Count < 1)
					{
						continue;
					}

					listOfIntersectionPoint.AddRange(intersectionPoint);
				}
			}
		}

		/// <summary>
		/// Checks the rotation of polyline
		/// </summary>
		/// <param name="polyline">Input polyline</param>
		/// <returns>True if polyline is in anticlockwise order</returns>
		public static bool IsAntiClockwise(IPolyLine3D polyline)
		{
			Vector3D normal = new Vector3D();
			if (GetAreaVector(polyline, ref normal))
			{
				double direction = normal | (new Vector3D(0, 0, 1));
				if (direction.IsGreater(0, MathConstants.ZeroGeneral))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Check polyline is collinear.
		/// </summary>
		/// <param name="polyline">Input polyline</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>return true if polyline is in a line</returns>
		public static bool IsCollinear(IPolyLine3D polyline, double toleranceLevel = MathConstants.ZeroGeneral)
		{
			if (polyline == null)
			{
				return false;
			}

			Vector3D localX = new Vector3D();
			IList<IPoint3D> points = new List<IPoint3D>();
			ConvertPolylineToPointList(polyline, points, true); // TODO CIH - zkontrolovat jestli lze true použít
			IPoint3D lastPoint = null;
			if (points.Count > 0)
			{
				lastPoint = points[0];
				points.RemoveAt(0);
			}

			foreach (IPoint3D point in points)
			{
				if (localX.Magnitude.IsZero())
				{
					localX = Subtract(point, lastPoint);
				}
				else
				{
					Vector3D vector = Subtract(point, lastPoint);
					if (IsCollinear(localX, vector, toleranceLevel) == false)
					{
						return false;
					}
				}

				lastPoint = point;
			}

			return true;
		}

		/// <summary>
		/// Checks polyline is in a plane
		/// </summary>
		/// <param name="polyline">Input polyline</param>
		/// <returns>True if polyline is in plane</returns>
		public static bool IsCoplanar(IPolyLine3D polyline)
		{
			int count = polyline.Count;
			if (count < 2)
			{
				return true;
			}
			else
			{
				IList<IPoint3D> points = new List<IPoint3D>();
				ConvertPolylineToPointList(polyline, points, true);  // TODO CIH - zkontrolovat jestli lze true použít
				Vector3D vector1 = Subtract(points[1], points[0]);
				Vector3D vector2 = Subtract(points[2], points[1]);
				Vector3D normal = vector1 * vector2;
				for (int i = 3; i < points.Count; i++)
				{
					Vector3D vector3 = Subtract(points[i], points[0]);
					if (normal.Magnitude.IsZero())
					{
						normal = vector1 * vector3;
						continue;
					}

					double value = normal | vector3;
					if (!value.IsEqual(0))
					{
						return false;
					}
				}

				if (normal.Magnitude.IsEqual(0))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Check whether given Polyline(structure) exist in the list of polyline
		/// </summary>
		/// <param name="polylineList">Collection of Polyline</param>
		/// <param name="polyline">Polyline to be compared</param>
		/// <returns>True if the structure exist in list</returns>
		public static bool IsContains(IEnumerable<IPolyLine3D> polylineList, IPolyLine3D polyline)
		{
			if (polylineList == null || polyline == null)
			{
				return false;
			}

			foreach (IPolyLine3D polylineCompare in polylineList)
			{
				if (IsEqual(polylineCompare, polyline))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Check whether two polyline are having equal structure
		/// </summary>
		/// <param name="polyline1">Polyline1</param>
		/// <param name="polyline2">Polyline2</param>
		/// <returns>True if equal</returns>
		public static bool IsEqual(IPolyLine3D polyline1, IPolyLine3D polyline2)
		{
			// TODO CIH - pomalá metoda a nedava presne vysledky
			if (polyline1 == null || polyline2 == null)
			{
				return false;
			}

			if (polyline1.Count != polyline2.Count)
			{
				return false;
			}

			List<IPoint3D> points1 = new List<IPoint3D>();
			List<IPoint3D> points2 = new List<IPoint3D>();
			ConvertPolylineToPointList(polyline1, points1, false);
			ConvertPolylineToPointList(polyline2, points2, false);
			if (points1.Count != points2.Count)
			{
				return false;
			}

			for (int i = 0; i < points1.Count; i++)
			{
				if (IsEqual(points1[i], points2[i]) || IsEqual(points1[i], points2[points2.Count - (1 + i)]))
				{
					continue;
				}

				return false;
			}

			return true;
		}

		/// <summary>
		/// Checks polyline intersects with itself
		/// </summary>
		/// <param name="polyline">Input polyline</param>
		/// <returns>True if polyline intersects with itself</returns>
		public static bool IsSelfIntersect(IPolyLine3D polyline)
		{
			int count = polyline.Count;
			for (int i = 0; i < count; i++)
			{
				for (int j = i + 1; j < count; j++)
				{
					ISegment3D segment1 = polyline[i];
					ISegment3D segment2 = polyline[j];
					if (IsSegmentIntersects(segment1, segment2))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Check the given polyline is valid
		/// </summary>
		/// <param name="polyline">Input polyline</param>
		/// <returns>True if the polyline is valid</returns>
		public static bool IsValid(IPolyLine3D polyline)
		{
			if (polyline.Count == 0)
			{
				return false;
			}

			IPoint3D lastEndPoint = null;
			if (polyline.IsClosed)
			{
				lastEndPoint = polyline[polyline.Count - 1].EndPoint;
			}

			foreach (ISegment3D segment in polyline.Segments)
			{
				if (IsValid(segment) == false)
				{
					return false;
				}

				if (lastEndPoint != null)
				{
					if (!IsEqual(lastEndPoint, segment.StartPoint))
					{
						return false;
					}
				}

				lastEndPoint = segment.EndPoint;
			}

			return true;
		}

		/// <summary>
		/// Move polyline according to given vector
		/// </summary>
		/// <param name="polyline">Polyline to be moved</param>
		/// <param name="displacement">Displacement vector</param>
		public static void Move(IPolyLine3D polyline, WM.Vector3D displacement)
		{
			Move(polyline, new Vector3D(displacement.X, displacement.Y, displacement.Z));
		}

		/// <summary>
		/// Move polyline according to given vector
		/// </summary>
		/// <param name="polyline">Polyline to be moved</param>
		/// <param name="displacement">Displacement vector</param>
		public static void Move(IPolyLine3D polyline, Vector3D displacement)
		{
			if (polyline == null || displacement.Magnitude.IsZero())
			{
				return;
			}

			bool isIndependent = true;
			if (polyline.IsClosed)
			{
				isIndependent = false;
			}

			foreach (ISegment3D segment in polyline.Segments)
			{
				Move(segment, displacement, isIndependent);
				isIndependent = false;
			}
		}

		/// <summary>
		/// Merge second polyline to first if end points are matching
		/// </summary>
		/// <param name="polyline1">First polyline</param>
		/// <param name="polyline2">Second polyline</param>
		/// <returns>true if merged successfully,
		/// false otherwise</returns>
		public static bool MergePolylines(IPolyLine3D polyline1, IPolyLine3D polyline2)
		{
			IPoint3D firstLineEnd = polyline1[polyline1.Count - 1].EndPoint;
			IPoint3D secondLineEnd = polyline2[polyline2.Count - 1].EndPoint;
			if (GeomOperation.IsEqual(firstLineEnd, secondLineEnd))
			{
				polyline2 = GeomOperation.Reverse(polyline2);

				foreach (ISegment3D segment in polyline2.Segments)
				{
					polyline1.Add(segment);
				}

				return true;
			}

			IPoint3D secondLineStart = polyline2[0].StartPoint;
			if (GeomOperation.IsEqual(firstLineEnd, secondLineStart))
			{
				foreach (ISegment3D segment in polyline2.Segments)
				{
					polyline1.Add(segment);
				}

				return true;
			}

			IPoint3D firstLineStart = polyline1[0].StartPoint;
			if (GeomOperation.IsEqual(firstLineStart, secondLineEnd))
			{
				for (int i = polyline2.Count - 1; i >= 0; i--)
				{
					polyline1.Insert(0, polyline2[i]);
				}

				return true;
			}

			if (GeomOperation.IsEqual(firstLineStart, secondLineStart))
			{
				polyline2 = GeomOperation.Reverse(polyline2);
				for (int i = polyline2.Count - 1; i >= 0; i--)
				{
					polyline1.Insert(0, polyline2[i]);
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Move polyline to a given direction with different magnitudes
		/// </summary>
		/// <param name="polyLine">Input polyline</param>
		/// <param name="direction">Displacement vector specifies direction of movement</param>
		/// <param name="m1">Magnittude at starting point</param>
		/// <param name="m2">Magnittude at ending point</param>
		public static void Move(IPolyLine3D polyLine, Vector3D direction, double m1, double m2)
		{
			double polyLineLength = 0;
			double lastStart = m1;
			bool isIndependent = true;
			if (polyLine.IsClosed)
			{
				isIndependent = false;
			}

			// Since polyline is modified in next loop, we need to calculate length seperately
			List<double> segmentLengths = new List<double>();
			foreach (ISegment3D segment in polyLine.Segments)
			{
				polyLineLength += GeomOperation.GetLength(segment);
				segmentLengths.Add(polyLineLength);
			}

			int index = 0;
			foreach (ISegment3D segment in polyLine.Segments)
			{
				double curLength = segmentLengths[index++];

				double magnitudeAtSegEnd = m1 + (m2 - m1) * (curLength / polyLineLength);
				if (segment.SegmentType == SegmentType.Line)
				{
					if (isIndependent)
					{
						GeomOperation.Move(segment.StartPoint, direction * lastStart);
					}

					GeomOperation.Move(segment.EndPoint, direction * magnitudeAtSegEnd);
				}
				else if (segment.SegmentType == SegmentType.CircularArc)
				{
					IArcSegment3D arc = (IArcSegment3D)segment;
					if (arc == null)
					{
						throw new InvalidCastException("Wrong segment data");
					}

					if (isIndependent)
					{
						GeomOperation.Move(arc.StartPoint, direction * lastStart);
					}

					GeomOperation.Move(arc.EndPoint, direction * magnitudeAtSegEnd);

					IPoint3D center = null;
					GeomOperation.GetCentrePoint(arc, ref center);
					Vector3D vector1 = GeomOperation.Subtract(arc.StartPoint, center);
					Vector3D vector2 = GeomOperation.Subtract(arc.IntermedPoint, center);
					Vector3D vector3 = GeomOperation.Subtract(arc.EndPoint, center);
					double angle1 = GeomOperation.GetAngle(vector1, vector2);
					double angle2 = GeomOperation.GetAngle(vector1, vector3);
					double magAtIntmid = lastStart + (magnitudeAtSegEnd - lastStart) * (angle1 / angle2);
					GeomOperation.Move(arc.IntermedPoint, direction * magAtIntmid);
				}
				else
				{
					throw new NotSupportedException();
				}

				lastStart = magnitudeAtSegEnd;
				isIndependent = false;
			}
		}

		/// <summary>
		/// Reverse orientation of a given polyline
		/// </summary>
		/// <param name="polyline">Input polyline</param>
		/// <returns>Reversed polyline</returns>
		public static IPolyLine3D Reverse(IPolyLine3D polyline)
		{
			IPolyLine3D reversed = new PolyLine3D();
			int count = polyline.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				reversed.Add(Reverse(polyline[i]));
			}

			return reversed;
		}

		/// <summary>
		/// Transform a given polyline to GCS using the matrix
		/// </summary>
		/// <param name="matrix">Matrix which is used to transform</param>
		/// <param name="polyline">Polyline which is to be transformed</param>
		public static void TransformToGCS(IMatrix44 matrix, IPolyLine3D polyline)
		{
			if (matrix == null || polyline == null)
			{
				return;
			}

			bool isIndependent = true;
			if (polyline.IsClosed)
			{
				isIndependent = false;
			}

			foreach (ISegment3D segment in polyline.Segments)
			{
				TransformToGCS(matrix, segment, isIndependent);
				isIndependent = false;
			}
		}

		/// <summary>
		/// Transforms collection of Points from LCS to GCS
		/// </summary>
		/// <param name="matrix">Matrix which is used to transform</param>
		/// <param name="points">Collection of Points</param>
		/// <returns>Transformed collection of Points</returns>
		public static IList<IPoint3D> TransformToGCS(IMatrix44 matrix, IEnumerable<IPoint3D> points)
		{
			List<IPoint3D> pointsGCS = new List<IPoint3D>();
			foreach (IPoint3D point in points)
			{
				pointsGCS.Add(matrix.TransformToGCS(point));
			}

			return pointsGCS;
		}

		/// <summary>
		/// Transform a given polyline to LCS using the matrix
		/// </summary>
		/// <param name="matrix">Matrix which is used to transform</param>
		/// <param name="polyline">Polyline which is to be transformed</param>
		public static void TransformToLCS(IMatrix44 matrix, IPolyLine3D polyline)
		{
			if (matrix == null || polyline == null)
			{
				return;
			}

			bool isIndependent = true;
			if (polyline.IsClosed)
			{
				isIndependent = false;
			}

			foreach (ISegment3D segment in polyline.Segments)
			{
				TransformToLCS(matrix, segment, isIndependent);
				isIndependent = false;
			}
		}

		/// <summary>
		/// Transforms collection of Points from GCS to LCS
		/// </summary>
		/// <param name="matrix">Matrix which is used to transform</param>
		/// <param name="points">Collection of Points</param>
		/// <returns>Transformed collection of Points</returns>
		public static IList<IPoint3D> TransformToLCS(Matrix44 matrix, IEnumerable<IPoint3D> points)
		{
			List<IPoint3D> pointsLCS = new List<IPoint3D>();
			foreach (IPoint3D point in points)
			{
				pointsLCS.Add(matrix.TransformToLCS(point));
			}

			return pointsLCS;
		}

		/// <summary>
		/// Transform the vector from LCS to GCS
		/// </summary>
		/// <param name="normal">Normal to the vector</param>
		/// <param name="origin">Origin point</param>
		/// <param name="point">Point on local X axis</param>
		/// <param name="vector">The vector to be transformed</param>
		public static void TransformVector(ref Vector3D normal, IPoint3D origin, IPoint3D point, ref Vector3D vector)
		{
			Vector3D localX = GeomOperation.Subtract(origin, point).Normalize;
			Vector3D localY = (normal * localX).Normalize;
			Matrix44 lcs = new Matrix44(localX, localY);
			lcs = Matrix44.Inverse33(lcs);
			lcs.Transform(ref vector);
		}

		/// <summary>
		/// IsPointOn
		/// </summary>
		/// <param name="polyLine">PolyLine</param>
		/// <param name="pointOnRegion">Point to be checked</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>Intersection Result</returns>
		public static IntersectionResults IsPointOn(IPolyLine3D polyLine, IPoint3D pointOnRegion, double toleranceLevel = MathConstants.ZeroWeak)
		{
			Vector3D normal = new Vector3D();
			GetNormal(polyLine, ref normal);

			double relativePosition = -1;
			double areaAngle = 0;
			for (int i = 0; i < polyLine.Count; i++)
			{
				ISegment3D segment = polyLine[i];
				IPoint3D head = segment.StartPoint;
				Vector3D difference = Subtract(head, pointOnRegion);
				double magnitude = ~difference;
				if (magnitude.IsLesserOrEqual(0))
				{
					relativePosition = i;
					return IntersectionResults.OnBorderNode;
				}
			}

			foreach (ISegment3D segment in polyLine.Segments)
			{
				double relativePos = 0;
				if (IsPointOnSegment(segment, pointOnRegion, ref relativePos, toleranceLevel))
				{
					relativePosition = 1 - relativePos;
					return IntersectionResults.OnBorderCurve;
				}

				double angle = 0.0;
				if (ComputeAngleAroundAxis(segment, pointOnRegion, normal, ref angle))
				{
					areaAngle += angle;
				}
			}

			if (Math.Abs(areaAngle) < (0.5 * Math.PI))
			{
				return IntersectionResults.Outside;
			}

			if (Math.Abs(areaAngle) < (1.5 * Math.PI))
			{
				return IntersectionResults.OnBorderCurve;
			}

			return IntersectionResults.Inside;
		}

		/// <summary>
		/// Gets the index of the segment by point
		/// </summary>
		/// <param name="polyLine">Polyline</param>
		/// <param name="point">Point</param>
		/// <param name="absPosTolerance">Tolerance - absolute position</param>
		/// <param name="relativePosTolerance">Tolerance - relative position</param>
		/// <returns>The index of the segment or -1 if point is not located on polyline</returns>
		public static int GetSegmentInxByPoint(IPolyLine3D polyLine, IPoint3D point, double absPosTolerance = MathConstants.ZeroWeak, double relativePosTolerance = MathConstants.ZeroWeak)
		{
			double relPos = 0;
			for (int i = 0; i < polyLine.Count; i++)
			{
				var seg = polyLine[i];
				if (IsPointOnSegment(seg, point, ref relPos, absPosTolerance, relativePosTolerance, true))
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Gets the index of the segment by point
		/// </summary>
		/// <param name="polyLine">Polyline</param>
		/// <param name="point">Point</param>
		/// <param name="absPosTolerance">Tolerance - absolute position</param>
		/// <param name="relativePosTolerance">Tolerance - relative position</param>
		/// <returns>The list of indexes of the segment or -1 if point is not located on polyline</returns>
		public static List<int> GetSegmentsListInxByPoint(IPolyLine3D polyLine, IPoint3D point, double absPosTolerance = MathConstants.ZeroWeak, double relativePosTolerance = MathConstants.ZeroWeak)
		{
			List<int> result = new List<int>();
			double relPos = 0;
			for (int i = 0; i < polyLine.Count; i++)
			{
				var seg = polyLine[i];
				if (IsPointOnSegment(seg, point, ref relPos, absPosTolerance, relativePosTolerance, true))
				{
					result.Add(i);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the relative postion on the polyline calculated from relative position on the specified segment.
		/// </summary>
		/// <param name="polyline">The polyline.</param>
		/// <param name="segment">The segment witch is the part of polyline.</param>
		/// <param name="relativePositionOnSegment">The relative position on the segment.</param>
		/// <param name="relativePositionOnPolyline">Calculated relative position on the polyline.</param>
		/// <returns>true, if segments is part of specified polyline; false, otherwise.</returns>
		public static bool GetPositionOnPolyline(IPolyLine3D polyline, ISegment3D segment, double relativePositionOnSegment, ref double relativePositionOnPolyline)
		{
			var plength = GeomOperation.GetLength(polyline);
			double currentLength = 0;
			foreach (var s in polyline.Segments)
			{
				var slength = GeomOperation.GetLength(s);
				if (s.Equals(segment))
				{
					relativePositionOnPolyline = (currentLength + relativePositionOnSegment * slength) / plength;
					return true;
				}

				currentLength += slength;
			}

			return false;
		}

		/// <summary>
		/// Tries to find <paramref name="segment"/> in <paramref name="Polyline"/>
		/// </summary>
		/// <param name="segment">Segment</param>
		/// <param name="polyline">Polyline</param>
		/// <returns>Returs index of <paramref name="segment"/> in <paramref name="polyline"/>.
		/// -1 is returned if segment is not part of the polyline</returns>
		public static int IsSegmentInPolyline(ISegment3D segment, IPolyLine3D polyline)
		{
			int inx = 0;
			foreach(var seg in polyline.Segments)
			{
				if (seg.Equals(segment))
				{
					return inx;
				}
				inx++;
			}

			return -1;
		}

		/// <summary>
		/// Checks whethe <paramref name="point"/> in vertex (begin or end point) of any segment of <paramref name="polyline"/>
		/// </summary>
		/// <param name="polyline">Tested polyline</param>
		/// <param name="point">Tested point</param>
		/// <param name="checkInstance">If true - references of instances are compared otherwise their coordinates are taken into account</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>True if point is vertex</returns>
		public static bool IsVertexPoint(IPolyLine3D polyline, IPoint3D point, bool checkInstance = true, double tolerance = 1e-6)
		{
			bool res = false;

			if (checkInstance)
			{
				int segCount = polyline.Count;
				for (int i = 0; i < segCount; i++)
				{
					var seg = polyline[i];
					if (seg.StartPoint.Equals(point))
					{
						res = true;
						break;
					}
				}
			}
			else
			{
				int segCount = polyline.Count;
				for (int i = 0; i < segCount; i++)
				{
					var seg = polyline[i];
					if (GeomOperation.IsEqual(seg.StartPoint, point, tolerance))
					{
						res = true;
						break;
					}
				}
			}

			return res;
		}

		/// <summary>
		/// Fills polyline with the segments
		/// </summary>
		/// <param name="segments">list of segments</param>
		/// <param name="polyline">filled polyline</param>
		/// <returns></returns>
		public static bool FillSegmentsIntoPolyline(IList<ISegment3D> segments, IPolyLine3D polyline)
		{
			List<ISegment3D> listSeg = new List<ISegment3D>(segments);

			int i = 0;
			while (listSeg.Count > 0)
			{
				if (i >= listSeg.Count)
				{
					return false;
				}

				var seg = listSeg[i];
				if (polyline.Count == 0)
				{
					polyline.Add(seg);
					listSeg.Remove(seg);
					i = 0;
					continue;
				}

				if (seg.StartPoint == polyline[polyline.Count - 1].EndPoint)
				{
					polyline.Add(seg);
					listSeg.Remove(seg);
					i = 0;
					continue;
				}

				if (seg.EndPoint == polyline[0].StartPoint)
				{
					polyline.Insert(0, seg);
					listSeg.Remove(seg);
					i = 0;
					continue;
				}

				if (seg.StartPoint == polyline[0].StartPoint)
				{
					IPoint3D p = seg.StartPoint;
					seg.StartPoint = seg.EndPoint;
					seg.EndPoint = p;
					polyline.Insert(0, seg);
					listSeg.Remove(seg);
					i = 0;
					continue;
				}

				if (seg.EndPoint == polyline[polyline.Count - 1].EndPoint)
				{
					IPoint3D p = seg.StartPoint;
					seg.StartPoint = seg.EndPoint;
					seg.EndPoint = p;
					polyline.Add(seg);
					listSeg.Remove(seg);
					i = 0;
					continue;
				}

				i++;
			}

			return true;
		}

		/// <summary>
		/// Returns segments which are parallel to <paramref name="direction"/>
		/// </summary>
		/// <param name="segments">Tested segments</param>
		/// <param name="direction">Direction</param>
		/// <returns>Collection of segments which are parallel to <paramref name="direction"/></returns>
		public static List<ILineSegment3D> GetParallelSegments(IEnumerable<ISegment3D> segments, WM.Vector3D direction)
		{
			var res = new List<ILineSegment3D>();

			direction.Normalize();

			foreach (var seg in segments)
			{
				ILineSegment3D lineSeg = seg as ILineSegment3D;
				if (lineSeg == null)
				{
					continue;
				}

				var segDir = lineSeg.GetDirection();
				if (Math.Abs(WM.Vector3D.DotProduct(direction, segDir)).IsEqual(1, 1e-6))
				{
					// vectors are parallel
					res.Add(lineSeg);
				}
			}

			return res;
		}

		/// <summary>
		/// Returns segments which are parallel to <paramref name="plane"/>
		/// </summary>
		/// <param name="segments">Tested segments</param>
		/// <param name="plane">Plane</param>
		/// <returns>Segments which are parallel to plane</returns>
		public static List<ILineSegment3D> GetParallelSegments(IEnumerable<ISegment3D> segments, Plane3D plane)
		{
			var res = new List<ILineSegment3D>();

			var normalVector = plane.NormalVector;
			foreach (var seg in segments)
			{
				ILineSegment3D lineSeg = seg as ILineSegment3D;
				if (lineSeg == null)
				{
					continue;
				}

				var segDir = lineSeg.GetDirection();
				if (Math.Abs(WM.Vector3D.DotProduct(normalVector, segDir)).IsEqual(0, 1e-6))
				{
					// vectors are parallel
					res.Add(lineSeg);
				}
			}

			return res;
		}


		/// <summary>
		/// Returns the line segment from <paramref name="lineSegments"/> which is closest to the <paramref name="point"/>
		/// </summary>
		/// <param name="lineSegments">Tested segments</param>
		/// <param name="point">Tested point</param>
		/// <returns>The closest segment</returns>
		public static ILineSegment3D GetClosestSegment(IList<ILineSegment3D> lineSegments, WM.Point3D point)
		{
			ILineSegment3D res = null;
			double dist = double.MaxValue;
			Plane3D tempPlane = new Plane3D
			{
				PointOnPlane = point
			};

			foreach (var ls in lineSegments)
			{
				tempPlane.NormalVector = ls.GetDirection();
				var intersection = tempPlane.GetIntersection(ls);

				var vect = intersection - point;
				var tempDist = vect.Length;

				if (tempDist < dist)
				{
					res = ls;
					dist = tempDist;
				}
			}

			return res;
		}


		/// <summary>
		/// Gets the closest parallel segment from the <paramref name="segments"/> to the <paramref name="straightLine"/>
		/// </summary>
		/// <param name="segments">Segments</param>
		/// <param name="straightLine">Reference line</param>
		/// <param name="minDist">The distance of the closest parallel segment</param>
		/// <param name="tolerance">Tolerance for parallel check</param>
		/// <returns>The closest parallel segment ot null</returns>
		public static ISegment3D GetClosestParallelSegment(IEnumerable<ISegment3D> segments, StraightLine straightLine, out double minDist, double tolerance = 1e-6)
		{
			ISegment3D res = null;
			minDist = double.MaxValue;

			foreach (var ls in segments)
			{
				var segDirVect = ls.GetDirection();
				if (!segDirVect.IsParallel(straightLine.Direction, tolerance))
				{
					continue;
				}

				var pointOnSeg = ls.StartPoint.ToMediaPoint();
				double curDist = GeomOperation.Distance(ref straightLine, ref pointOnSeg);
				if (curDist.IsLesser(minDist))
				{
					minDist = curDist;
					res = ls;
				}
			}

			return res;
		}

		/// <summary>
		///  Returns the segment from <paramref name="segments"/> which is closest to <paramref name="plane"/> (in the absolute value)
		/// </summary>
		/// <param name="segments">Tested segments</param>
		/// <param name="plane">Tested plane</param>
		/// <returns>The closest segment</returns>
		public static ISegment3D GetClosestSegmentToPlane(IEnumerable<ISegment3D> segments, Plane3D plane)
		{
			ISegment3D res = null;
			double dist = double.MaxValue;

			foreach (var s in segments)
			{
				double tempDist = Math.Abs(plane.GetPointDistance(s.StartPoint));
				if (tempDist < dist)
				{
					res = s;
					dist = tempDist;
					continue;
				}

				tempDist = Math.Abs(plane.GetPointDistance(s.EndPoint));
				if (tempDist < dist)
				{
					res = s;
					dist = tempDist;
				}
			}

			return res;
		}

		/// <summary>
		/// Returns the index of the segment from <paramref name="segments"/> which is closest to <paramref name="plane"/> (in the absolute value)
		/// </summary>
		/// <param name="segments">Tested segments</param>
		/// <param name="plane">Tested plane</param>
		/// <returns>The index of the closest segment</returns>
		public static int GetClosestSegmentIndexToPlane(IEnumerable<ISegment3D> segments, Plane3D plane)
		{
			ISegment3D res = null;
			int segInx = -1;
			int curSegInx = -1;
			double dist = double.MaxValue;

			foreach (var s in segments)
			{
				curSegInx++;

				WM.Point3D p = new WM.Point3D(s.StartPoint.X, s.StartPoint.Y, s.StartPoint.Z);
				var p1 = s.StartPoint.ToMediaPoint();
				var p2 = s.EndPoint.ToMediaPoint();
				var midPt = GeomOperation.GetMidPoint(ref p1, ref p2);

				double tempDist = Math.Abs(plane.GetPointDistance(ref midPt));
				if (tempDist.IsLesser(dist))
				{
					res = s;
					dist = tempDist;
					segInx = curSegInx;
				}
			}

			return segInx;
		}

		/// <summary>
		/// Returns the index of the segment from <paramref name="segments"/> which is farthest to <paramref name="plane"/> (in the absolute value)
		/// </summary>
		/// <param name="segments">Tested segments</param>
		/// <param name="plane">Tested plane</param>
		/// <returns>The index of the farthest segment</returns>
		public static int GetFarthestSegmentIndexToPlane(IEnumerable<ISegment3D> segments, Plane3D plane)
		{
			ISegment3D res = null;
			int segInx = -1;
			int curSegInx = -1;
			double dist = double.MinValue;

			foreach (var s in segments)
			{
				curSegInx++;

				WM.Point3D p = new WM.Point3D(s.StartPoint.X, s.StartPoint.Y, s.StartPoint.Z);
				var p1 = s.StartPoint.ToMediaPoint();
				var p2 = s.EndPoint.ToMediaPoint();
				var midPt = GeomOperation.GetMidPoint(ref p1, ref p2);

				double tempDist = Math.Abs(plane.GetPointDistance(ref midPt));
				if (tempDist.IsGreater(dist))
				{
					res = s;
					dist = tempDist;
					segInx = curSegInx;
				}
			}

			return segInx;
		}

		/// <summary>
		/// Returns the polyline from <paramref name="polylines"/> which is closest to <paramref name="plane"/> (in the absolute value)
		/// </summary>
		/// <param name="polylines">Tested polylines</param>
		/// <param name="plane">Tested plane</param>
		/// <param name="dist">Distnace of the closest polyline</param>
		/// <returns>The closest polyline from <paramref name="polylines"/></returns>
		public static IPolyLine3D GetClosestPolylineToPlane(IEnumerable<IPolyLine3D> polylines, Plane3D plane, out double dist)
		{
			IPolyLine3D res = null;
			dist = double.MaxValue;

			foreach (var p in polylines)
			{
				ISegment3D foundSeg = null;
				double polyLineDist = double.MaxValue;

				foreach (var s in p.Segments)
				{
					double tempDist = Math.Abs(plane.GetPointDistance(s.StartPoint));
					if (tempDist < polyLineDist)
					{
						foundSeg = s;
						polyLineDist = tempDist;
						continue;
					}

					tempDist = Math.Abs(plane.GetPointDistance(s.EndPoint));
					if (tempDist < polyLineDist)
					{
						foundSeg = s;
						polyLineDist = tempDist;
					}
				}

				if (polyLineDist < dist)
				{
					res = p;
					dist = polyLineDist;
				}
			}

			return res;
		}

		/// <summary>
		///  Returns the segment from <paramref name="segments"/> which is furthermost to <paramref name="plane"/> (in the absolute value)
		/// </summary>
		/// <param name="segments">Tested segments</param>
		/// <param name="plane">Tested plane</param>
		/// <returns>The closest segment</returns>
		public static ISegment3D GetFurthermostSegmentToPlane(IEnumerable<ISegment3D> segments, Plane3D plane)
		{
			ISegment3D res = null;
			double dist = double.MinValue;

			foreach (var s in segments)
			{
				double tempDist = Math.Abs(plane.GetPointDistance(s.StartPoint));
				if (tempDist > dist)
				{
					res = s;
					dist = tempDist;
					continue;
				}

				tempDist = Math.Abs(plane.GetPointDistance(s.EndPoint));
				if (tempDist > dist)
				{
					res = s;
					dist = tempDist;
				}
			}

			return res;
		}

		/// <summary>
		/// Compute angle for segment for a given axis
		/// </summary>
		/// <param name="segment">Segment</param>
		/// <param name="axisPoint">Axis Point</param>
		/// <param name="axisVector">Axis Vector</param>
		/// <param name="angle">Agle</param>
		/// <returns>True if input given is valid</returns>
		private static bool ComputeAngleAroundAxis(ISegment3D segment, IPoint3D axisPoint, Vector3D axisVector, ref double angle)
		{
			if (segment.SegmentType == SegmentType.Line)
			{
				IPoint3D head = segment.StartPoint;
				IPoint3D end = segment.EndPoint;
				Vector3D v1 = Subtract(head, axisPoint);
				Vector3D v2 = Subtract(end, axisPoint);

				Vector3D axis = axisVector;
				axis = axis.Normalize;

				Vector3D localX = v1 - (v1 | axis) * axis;
				localX = localX.Normalize;

				Vector3D vectorNull = new Vector3D();
				if (localX.Equals(vectorNull))
				{
					return false;
				}

				Vector3D localY = axis * localX;

				angle = Math.Atan2(v2 | localY, v2 | localX);

				while (angle > Math.PI)
				{
					angle -= 2 * Math.PI;
				}

				while (angle < -Math.PI)
				{
					angle += 2 * Math.PI;
				}

				return true;
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IList<IPoint3D> listOfPoint = new List<IPoint3D>();
				for (int i = 1; i <= 36; i++)
				{
					double relative = 0;
					if (i != 0)
					{
						relative = (double)i / 36;
					}

					IPoint3D point = new Point3D();
					GetPointOnSegment(segment, relative, ref point);
					listOfPoint.Add(point);
				}

				IPoint3D head = new Point3D(segment.StartPoint.X, segment.StartPoint.Y, segment.StartPoint.Z);

				for (int j = 0; j < listOfPoint.Count; j++)
				{
					double ang = 0;
					Vector3D axis = axisVector;
					axis = axis.Normalize;

					IPoint3D end = listOfPoint[j];

					Vector3D v1 = Subtract(head, axisPoint);
					Vector3D v2 = Subtract(end, axisPoint);
					head = end;

					Vector3D localX = v1 - (v1 | axis) * axis;
					localX = localX.Normalize;

					Vector3D vectorNull = new Vector3D();
					if (localX.Equals(vectorNull))
					{
						return false;
					}

					Vector3D localY = axis * localX;
					ang = Math.Atan2(v2 | localY, v2 | localX);

					while (ang > Math.PI)
					{
						ang -= 2 * Math.PI;
					}

					while (ang < -Math.PI)
					{
						ang += 2 * Math.PI;
					}

					angle += ang;
				}

				return true;
			}

			return false;
		}

		private static ISegment3D AlterSegmentStartPoint(ISegment3D segment, double relativePosition)
		{
			IPoint3D startPoint = new Point3D();
			GetPointOnSegment(segment, relativePosition, ref startPoint);

			if (segment.SegmentType == SegmentType.Line)
			{
				segment.StartPoint = startPoint;
				return segment;
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IPoint3D intermediatePoint = new Point3D();
				double intermediatePosition = Math.Abs(1 - relativePosition) / 2;
				GetPointOnSegment(segment, intermediatePosition, ref intermediatePoint);
				segment.StartPoint = startPoint;
				return segment;
			}

			throw new NotSupportedException();
		}

		private static ISegment3D AlterSegmentEndPoint(ISegment3D segment, double relativePosition)
		{
			IPoint3D endPoint = new Point3D();
			GetPointOnSegment(segment, relativePosition, ref endPoint);

			if (segment.SegmentType == SegmentType.Line)
			{
				segment.EndPoint = endPoint;
				return segment;
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IPoint3D intermediatePoint = new Point3D();
				GetPointOnSegment(segment, relativePosition / 2, ref intermediatePoint);
				segment.EndPoint = endPoint;
				return segment;
			}

			throw new NotSupportedException();
		}
	}
}
