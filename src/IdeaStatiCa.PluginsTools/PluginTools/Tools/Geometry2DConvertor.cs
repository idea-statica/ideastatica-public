using System;
using System.Collections.Generic;
using System.Windows;
using CI.Geometry2D;
using CI.GiCL2D;

namespace IdeaRS.GeometricItems
{
	public static partial class Geometry2DConvertor
	{
		#region Region

		public static Region Convert(this IRegion2D source, Point? origin = null)
		{
			var target = new Region { Outline = source.Outline.Convert(origin), };
			var openings = source.Openings;
			if (openings.Count > 0)
			{
				var targetOpenings = new List<Polyline>(openings.Count);
				target.Openings = targetOpenings;
				foreach (var opening in openings)
				{
					targetOpenings.Add(opening.Convert(origin));
				}
			}

			return target;
		}

		public static IRegion2D Convert(this Region source)
		{
			var target = new Region2D(source.Outline.Convert());
			var openings = source.Openings;
			if (openings != null && openings.Count > 0)
			{
				var targetOpenings = target.Openings;
				foreach (var opening in openings)
				{
					targetOpenings.Add(opening.Convert());
				}
			}

			return target;
		}

		#endregion Region

		#region Polyline

		public static Polyline Convert(this IPolyLine2D source, Point? origin = null)
		{
			var segments = new List<Segment>(source.Segments.Count + 1);
			Point start = source.StartPoint;
			if (origin.HasValue) start = new Point(start.X - origin.Value.X, start.Y - origin.Value.Y);
			segments.Add(Segment.StartPoint(start.X, start.Y));
			foreach (var s in source.Segments)
			{
				segments.Add(s.Convert(ref start, origin));
				start = s.EndPoint;
				if (origin.HasValue) start = new Point(start.X - origin.Value.X, start.Y - origin.Value.Y);
			}

			var target = new Polyline { Segments = segments, };
			return target;
		}

		public static IPolyLine2D Convert(this Polyline source)
		{
			var target = new PolyLine2D(source.Segments.Count);
			foreach (var s in source.Segments)
			{
				ISegment2D segment = null;
				switch (s.Type)
				{
					case SegmentType.StartPoint:
						target.StartPoint = new Point(s.Parameters[0], s.Parameters[1]);
						continue;

					case SegmentType.Line:
						segment = new LineSegment2D(new Point(s.Parameters[0], s.Parameters[1]));
						break;

					case SegmentType.CircArc3Points:
						segment = new CircularArcSegment2D(new Point(s.Parameters[0], s.Parameters[1]), new Point(s.Parameters[2], s.Parameters[3]));
						break;
				}

				target.Segments.Add(segment);
			}

			return target;
		}

		private static Segment Convert(this ISegment2D segment, ref Point start, Point? origin = null)
		{
			var line = segment as LineSegment2D;
			if (line != null)
			{
				if (origin.HasValue)
					return Segment.LineSegment(line.EndX - origin.Value.X, line.EndY - origin.Value.Y);
				else
					return Segment.LineSegment(line.EndX, line.EndY);
			}

			var circArc = segment as CircularArcSegment2D;
			if (circArc != null)
			{
				double[] p;
				if (origin.HasValue)
					p = new double[]
				{
					circArc.EndX - origin.Value.X,
					circArc.EndY - origin.Value.Y,
					circArc.Point.X - origin.Value.X,
					circArc.Point.Y - origin.Value.Y,
				};
				else
					p = new double[]
				{
					circArc.EndX,
					circArc.EndY,
					circArc.Point.X,
					circArc.Point.Y,
				};
				return new Segment { Type = SegmentType.CircArc3Points, Parameters = p, };
			}

			throw new NotImplementedException();
		}

		#endregion Polyline

		#region Boundary
		public static Rect2D  Boundary(this IRegion2D source)
		{
			return source.Outline.Boundary();
		}

		public static Rect2D Boundary(this IPolyLine2D source)
		{
			Point start = source.StartPoint;
			var ret = new Rect2D(start,new Size());
			foreach (var s in source.Segments)
			{
				var sg = s.Convert(ref start);
				ret.Union(sg.GetBoundary(start));
				start = s.EndPoint;
			}
			return ret;
		}

		#endregion
	}
}