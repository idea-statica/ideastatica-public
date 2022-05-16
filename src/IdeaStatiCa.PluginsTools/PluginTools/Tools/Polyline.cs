using CI.GiCL2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;

namespace IdeaRS.GeometricItems
{
	/// <summary>
	/// Specifies the type of an polyline segment.
	/// </summary>
	[DataContract]
	public enum SegmentType
	{
		/// <summary>
		/// Start point of polyline - first item in segments.
		/// </summary>
		[EnumMember]
		StartPoint = 1,

		/// <summary>
		/// Line segment specified only by end point {x, y}.
		/// </summary>
		[EnumMember]
		Line = 2,

		/// <summary>
		/// Circular arc segment specified by end point and any intermediate point laying on the circlular arc.
		/// </summary>
		[EnumMember]
		CircArc3Points = 3,
	}

	[DataContract]
	public class Polyline
	{
		public Polyline()
		{
			Segments = new List<Segment>();
		}

		public Polyline(int capacity)
		{
			Segments = new List<Segment>(capacity);
		}

		[DataMember]
		public List<Segment> Segments { get; set; }

		public static Polyline StartPolyline(int capacity = 8)
		{
			var p = new Polyline(capacity);
			p.Segments.Add(Segment.StartPoint(0, 0));
			return p;
		}

		public Polyline Clone()
		{
			var count = this.Segments.Count;
			var clone = new Polyline(count);
			var segments = clone.Segments;
			for (var i = 0; i < count; ++i)
			{
				segments.Add(this.Segments[i].Clone());
			}

			return clone;
		}
	}

	[DataContract]
	public class Segment
	{
		[DataMember]
		public IList<double> Parameters { get; set; }

		[DataMember]
		public SegmentType Type { get; set; }

		/// <summary>
		/// Create Starting Segment
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Segment StartPoint(double x, double y)
		{
			return new Segment() { Type = SegmentType.StartPoint, Parameters = new double[] { x, y } };
		}

		/// <summary>
		/// Create Line Segment
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Segment LineSegment(double x, double y)
		{
			return new Segment() { Type = SegmentType.Line, Parameters = new double[] { x, y } };
		}

		/// <summary>
		/// Create Arc Segment
		/// </summary>
		/// <param name="EndX"></param>
		/// <param name="EndY"></param>
		/// <param name="PointX"></param>
		/// <param name="PointY"></param>
		/// <returns></returns>
		public static Segment ArcSegment(double EndX, double EndY, double PointX, double PointY)
		{
			return new Segment() { Type = SegmentType.CircArc3Points, Parameters = new double[] { EndX, EndY, PointX, PointY } };
		}

		/// <summary>
		/// Create Arc Segment
		/// </summary>
		/// <param name="radius"></param>
		/// <param name="largeArcFlag"></param>
		/// <param name="sweepFlag"></param>
		/// <param name="endX"></param>
		/// <param name="endY"></param>
		/// <returns></returns>
		public static Segment ArcSegment(double radius, double largeArcFlag, double sweepFlag, double endX, double endY)
		{
			return new Segment() { Type = SegmentType.CircArc3Points, Parameters = new double[] { radius, largeArcFlag, sweepFlag, endX, endY } };
		}

		/// <summary>
		/// Clone Segment
		/// </summary>
		/// <returns></returns>
		public Segment Clone()
		{
			var clone = new Segment { Type = this.Type, Parameters = this.Parameters.ToArray(), };
			return clone;
		}

		/// <summary>
		/// Get Segment Boundary
		/// </summary>
		/// <param name="startPoint"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public Rect2D GetBoundary(Point startPoint)
		{
			switch (Type)
			{
				case SegmentType.StartPoint:
					return new Rect2D(Parameters[0], Parameters[1], 0, 0);

				case SegmentType.Line:
					return new Rect2D(new Point(startPoint.X, startPoint.Y), new Point(Parameters[0], Parameters[1]));

				case SegmentType.CircArc3Points:
					{
						var arc = new Arc2D(startPoint, new Point(Parameters[2], Parameters[3]), new Point(Parameters[0], Parameters[1]));
						return arc.GetBoundary();
					}
				default:
					throw new NotImplementedException();
			}
		}
	}
}