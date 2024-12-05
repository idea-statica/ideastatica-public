using System;
using System.Collections.Generic;
using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Provides intersection on <c>IRegion2D</c> and <c>IPolyLine2D</c> with cutting line.
	/// </summary>
	public class IntersectionLocator2D
	{
		/// <summary>
		/// Search intersections on collection of <c>IRegion2D</c>.
		/// </summary>
		/// <param name="regions">The enumerable of <c>IRegion2D</c> for determination of intersections.</param>
		/// <param name="linePt">The cutting line point.</param>
		/// <param name="lineDir">The cutting line direction.</param>
		/// <returns>A list of intersections.</returns>
		public List<IntersectionData> Find(IEnumerable<IRegion2D> regions, ref Point linePt, ref Vector lineDir)
		{
			if (regions == null)
			{
				throw new ArgumentNullException("regions", "IntersectionLocator2D.Find - regions");
			}

			List<IntersectionData> intersections = new List<IntersectionData>();

			foreach (var region in regions)
			{
				intersections.AddRange(Find(region, ref linePt, ref lineDir));
			}

			intersections.Sort();

			return intersections;
		}

		/// <summary>
		/// Search intersections on <c>IRegion2D</c>.
		/// </summary>
		/// <param name="region">The object for determination of intersections.</param>
		/// <param name="linePt">The cutting line point.</param>
		/// <param name="lineDir">The cutting line direction.</param>
		/// <returns>A list of intersections.</returns>
		public List<IntersectionData> Find(IRegion2D region, ref Point linePt, ref Vector lineDir)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region", "IntersectionLocator2D.Find - region");
			}

			List<IntersectionData> intersections;

			intersections = Find(region.Outline, ref linePt, ref lineDir);

			intersections.AddRange(Find(region.Openings, ref linePt, ref lineDir));

			intersections.Sort();

			return intersections;
		}

		/// <summary>
		/// Search intersections on collection of <c>IPolyLine2D</c>.
		/// </summary>
		/// <param name="polylines">The enumerable of <c>IPolyLine2D</c> for determination of intersections.</param>
		/// <param name="linePt">The cutting line point.</param>
		/// <param name="lineDir">The cutting line direction.</param>
		/// <returns>A list of intersections.</returns>
		public List<IntersectionData> Find(IEnumerable<IPolyLine2D> polylines, ref Point linePt, ref Vector lineDir)
		{
			if (polylines == null)
			{
				throw new ArgumentNullException("polylines", "IntersectionLocator2D.Find - polylines");
			}

			List<IntersectionData> intersections = new List<IntersectionData>();

			foreach (var p in polylines)
			{
				intersections.AddRange(Find(p, ref linePt, ref lineDir));
			}

			intersections.Sort();

			return intersections;
		}

		/// <summary>
		/// Search intersections on <c>IPolyLine2D</c>.
		/// </summary>
		/// <param name="polyline">The object for determination of intersections.</param>
		/// <param name="linePt">The cutting line point.</param>
		/// <param name="lineDir">The cutting line direction.</param>
		/// <returns>A list of intersections.</returns>
		public List<IntersectionData> Find(IPolyLine2D polyline, ref Point linePt, ref Vector lineDir)
		{
			if (polyline == null)
			{
				throw new ArgumentNullException("polyline", "IntersectionLocator2D.Find - polyline");
			}

			var intersections = new List<IntersectionData>();

			var param = new InputParam(polyline.StartPoint, linePt, lineDir);
			foreach (var segment in polyline.Segments)
			{
				var data = FindIntersections(segment, param);
				if (data != null)
				{
					foreach (var d in data)
					{
						if (d != null)
						{
							intersections.Add(d);
						}
					}
				}

				param.SegmentStartPoint = segment.EndPoint;
			}

			CheckIntersetionData(intersections, linePt, lineDir);

			return intersections;
		}

		/// <summary>
		/// Search intersections on <c>IPolygon2D</c>.
		/// </summary>
		/// <param name="polygon">The object for determination of intersections.</param>
		/// <param name="linePt">The cutting line point.</param>
		/// <param name="lineDir">The cutting line direction.</param>
		/// <returns>A list of intersections.</returns>
		public static List<IntersectionData> Find(IPolygon2D polygon, ref Point linePt, ref Vector lineDir)
		{
			if (polygon == null)
			{
				throw new ArgumentNullException("polygon", "IntersectionLocator2D.Find - polygon");
			}

			var intersections = new List<IntersectionData>();

			var count = polygon.Count;
			for (var i = 1; i < count; ++i)
			{
				var start = polygon[i - 1];
				Vector segmentDir = polygon[i] - start;
				Point intersection;
				IntersectionInfo intersectionResult = GeomTools2D.LineIntersection(start, segmentDir,linePt, lineDir, out intersection);
				if (IntersectionInfoHelper.IsSubset(intersectionResult, IntersectionInfo.IntersectionInside | IntersectionInfo.IntersectionFirst))
				{
					IntersectionData data = new IntersectionData();
					data.StartPoint = start;
					data.Intersection = intersection;

					Vector v = Point.Subtract(intersection, linePt);
					var len = v.Length;
					data.RelPosOnCutLine = len / lineDir.Length;
					double angle = Math.Abs(Vector.AngleBetween(v, lineDir));
					if (170 < angle && angle < 190)
					{
						data.RelPosOnCutLine *= -1;
					}

					v = Point.Subtract(intersection, start);
					data.RelPosOnSegment = len / segmentDir.Length;
					data.Angle = Vector.AngleBetween(segmentDir, lineDir);
					intersections.Add(data);
				}
			}

			CheckIntersetionData(intersections, linePt, lineDir);

			return intersections;
		}

		private static void CheckIntersetionData(List<IntersectionData> intersections, Point linePt, Vector lineDir)
		{
			intersections.Sort();

			int intCount = intersections.Count;
			for (int i = intCount - 2; i >= 0; --i)
			{
				// 2 intersections in one point
				if (intersections[i].RelPosOnCutLine == intersections[i + 1].RelPosOnCutLine)
				{
					// intersect at the joint of two segments
					// or it is acute angle
					if (SegmentsOnSameSide(intersections[i], intersections[i + 1], ref linePt, ref lineDir))
					{
						// remove both intersections
						intersections.RemoveAt(i + 1);
						intersections.RemoveAt(i);
						--i;
					}
					else
					{
						intersections.RemoveAt(i + 1);
					}
				}
			}
		}

		private static bool SegmentsOnSameSide(IntersectionData intersection1, IntersectionData intersection2, ref Point linePt, ref Vector lineDir)
		{
			PointRelatedToLine seg1;
			Point pt;
			if (intersection1.StartPoint == intersection1.Intersection)
			{
				var start = intersection1.StartPoint;
				pt = intersection1.Segment.GetPointOnSegment(ref start, 0.01);
			}
			else
			{
				var start = intersection1.StartPoint;
				pt = intersection1.Segment.GetPointOnSegment(ref start, 0.99);
			}

			seg1 = GeomTools2D.PointToLine(ref pt, ref linePt, ref lineDir);

			PointRelatedToLine seg2;
			if (intersection2.StartPoint == intersection1.Intersection)
			{
				var start = intersection2.StartPoint;
				pt = intersection2.Segment.GetPointOnSegment(ref start, 0.01);
			}
			else
			{
				var start = intersection2.StartPoint;
				pt = intersection2.Segment.GetPointOnSegment(ref start, 0.99);
			}

			seg2 = GeomTools2D.PointToLine(ref pt, ref linePt, ref lineDir);
			return seg1 == seg2;
		}

		#region MultiMethods switch

		private IEnumerable<IntersectionData> FindIntersections(ISegment2D segment, InputParam param)
		{
			switch (segment)
			{
				case LineSegment2D lineSegment2D:
					return FindIntersections(lineSegment2D, param);
				case CircularArcSegment2D circularArcSegment2D:
					return FindIntersections(circularArcSegment2D, param);
				case Segment2D segment2D:
					return FindIntersections(segment2D, param);
			}
			throw new ArgumentException("FindIntersections does not support type", segment.GetType().AssemblyQualifiedName);
		}

		private IEnumerable<IntersectionData> FindIntersections(Segment2D segment, InputParam param)
		{
			throw new ArgumentException("FindIntersections does not support type", segment.GetType().AssemblyQualifiedName);
		}

		private IEnumerable<IntersectionData> FindIntersections(LineSegment2D segment, InputParam param)
		{
			var start = param.SegmentStartPoint;
			Vector segmentDir = Point.Subtract(segment.EndPoint, start);
			Point intersection;
			IntersectionInfo intersectionResult = GeomTools2D.LineIntersection(start, segmentDir, param.LinePt, param.LineDir, out intersection);
			if (IntersectionInfoHelper.IsSubset(intersectionResult, IntersectionInfo.IntersectionInside | IntersectionInfo.IntersectionFirst))
			{
				IntersectionData data = new IntersectionData();
				data.StartPoint = start;
				data.Segment = segment;
				data.Intersection = intersection;

				Vector v = Point.Subtract(intersection, param.LinePt);
				data.RelPosOnCutLine = v.Length / param.LineDir.Length;
				double angle = Math.Abs(Vector.AngleBetween(v, param.LineDir));
				if (170 < angle && angle < 190)
				{
					data.RelPosOnCutLine *= -1;
				}

				v = Point.Subtract(intersection, start);
				data.RelPosOnSegment = v.Length / segment.GetLength(ref start);
				data.Angle = Vector.AngleBetween(segmentDir, param.LineDir);
				return new IntersectionData[] { data };
			}

			return null;
		}

		private IEnumerable<IntersectionData> FindIntersections(CircularArcSegment2D segment, InputParam param)
		{
			var start = param.SegmentStartPoint;
			Point[] intersect;
			var res = GeomTools2D.LineCircleIntersection(param.LinePt, param.LineDir, segment.GetCentre(ref start), segment.GetRadius(ref start), out intersect);
			if (res != IntersectionInfo.NoIntersection)
			{
				var intersections = new List<IntersectionData>(2);
				AddCircularArcIntersection(segment, param, ref intersect[0], ref intersections);
				AddCircularArcIntersection(segment, param, ref intersect[1], ref intersections);
				return intersections;
			}

			return null;
		}

		private void AddCircularArcIntersection(CircularArcSegment2D segment, InputParam param, ref Point intersection, ref List<IntersectionData> intersections)
		{
			var start = param.SegmentStartPoint;
			var centre = segment.GetCentre(ref start);
			var vs = Point.Subtract(centre, start);
			var ve = Point.Subtract(centre, segment.EndPoint);
			var angle = Vector.AngleBetween(vs, ve);
			var vi = Point.Subtract(centre, intersection);

			var pt = segment.Point;
			var vse = Point.Subtract(segment.EndPoint, start);
			var segPtSide = GeomTools2D.PointToLine(ref pt, ref start, ref vse);
			var intPtSide = GeomTools2D.PointToLine(ref intersection, ref start, ref vse);

			if (segPtSide == intPtSide || intPtSide == PointRelatedToLine.OnLine)
			{
				var data = new IntersectionData();
				data.StartPoint = start;
				data.Segment = segment;
				data.Intersection = intersection;
				var v = Point.Subtract(intersection, param.LinePt);
				data.RelPosOnCutLine = v.Length / param.LineDir.Length;
				double ang = Math.Abs(Vector.AngleBetween(v, param.LineDir));
				if (170 < ang && ang < 190)
				{
					data.RelPosOnCutLine *= -1;
				}

				var anglei = Vector.AngleBetween(vs, vi);
				data.RelPosOnSegment = anglei / angle;

				// calculate angle between cut line and segment
				var vi2 = new Vector(vi.Y, vi.X);
				if (segment.IsCounterClockwise(ref start) == 1)
				{
					vi2.Y *= -1;
				}
				else
				{
					vi2.X *= -1;
				}

				data.Angle = Vector.AngleBetween(vi2, param.LineDir);
				intersections.Add(data);
			}
		}

		#endregion MultiMethods switch

		/// <summary>
		/// Internal data for MultiMethod functions.
		/// </summary>
		private class InputParam
		{
			public InputParam(Point segmentStart, Point linePt, Vector lineDir)
			{
				SegmentStartPoint = segmentStart;
				LinePt = linePt;
				LineDir = lineDir;
			}

			public Point SegmentStartPoint { get; set; }

			public Point LinePt { get; set; }

			public Vector LineDir { get; set; }
		}
	}
}