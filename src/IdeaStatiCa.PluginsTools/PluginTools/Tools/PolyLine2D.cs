using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;

namespace CI.Geometry2D
{
	/// <summary>
	/// Represents a subsection of a geometry, a single connected series of two-dimensional geometric segments.
	/// </summary>
	[Obfuscation(Feature = "renaming")]

	[DebuggerDisplay("StartPoint=[{StartPoint.X}; {StartPoint.Y}], Closed={IsClosed}")]
	public class PolyLine2D : IPolyLine2D, IPolyLine2DCom
	{
		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private IList<ISegment2D> segments;

		private Point startPoint;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public PolyLine2D()
		{
			segments = new List<ISegment2D>();
			Id = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <c>PolyLine2D</c> class with empty list of segments that has the specified initial capacity.
		/// </summary>
		/// <param name="capacity">The number of elements that the new list of segments can initially store.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">The capacity is less than 0.</exception>
		public PolyLine2D(int capacity)
		{
			segments = new List<ISegment2D>(capacity);
			Id = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <c>PolyLine2D</c> class as copy of source.
		/// </summary>
		/// <param name="source">The source.</param>
		public PolyLine2D(PolyLine2D source)
		{
			StartPoint = source.StartPoint;
			Segments = (IList<ISegment2D>)source.Segments.Clone();
			Id = source.Id;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets or sets the point where the PolyLine2D begins.
		/// </summary>
		public IdaComPoint2D StartPoint
		{
			get { return startPoint; }
			set { startPoint = value; }
		}

		/// <summary>
		/// Get, sets x-coordinate of the starting point
		/// </summary>
		[XmlIgnore]
		public double StartX
		{
			get { return startPoint.X; }
			set { startPoint.X = value; }
		}

		/// <summary>
		/// Gets, sets y-coordinate of the starting point
		/// </summary>
		[XmlIgnore]
		public double StartY
		{
			get { return startPoint.Y; }
			set { startPoint.Y = value; }
		}

		/// <summary>
		/// Gets a value that specifies whether this polyline first and last segments are connected.
		/// </summary>
		public bool IsClosed
		{
			get
			{
				return Segments.Count > 0 && Segments[Segments.Count - 1].EndPoint.Equals(StartPoint);
			}
		}

		/// <summary>
		/// Gets segments of <c>PolyLine2D</c>.
		/// </summary>
		[XmlIgnore]
		public IList<ISegment2D> Segments
		{
			get { return segments; }
			set { segments = value; }
		}

		/// <summary>
		/// Gets the length of this <c>PolyLine2D</c> object.
		/// </summary>
		public double Length
		{
			get
			{
				double length = 0;
				Point pt = StartPoint;
				foreach (var segment in segments)
				{
					length += segment.GetLength(ref pt);
					pt = segment.EndPoint;
				}

				return length;
			}
		}

		/// <summary>
		/// Gets a <c>Rect</c> that specifies the bounding box of this <c>PolyLine2D</c> object.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Rect Bounds
		{
			get
			{
				Rect boundary = new Rect(StartPoint, StartPoint);
				Point pt = StartPoint;
				double x, y;

				foreach (ISegment2D segment in segments)
				{
					Rect r = segment.GetBounds(ref pt);

					x = Math.Max(boundary.Location.X + boundary.Width, r.Location.X + r.Width);
					y = Math.Max(boundary.Location.Y + boundary.Height, r.Location.Y + r.Height);

					boundary.Location = new Point(
						Math.Min(boundary.Location.X, r.Location.X),
						Math.Min(boundary.Location.Y, r.Location.Y));
					boundary.Size = new Size(x - boundary.Location.X, y - boundary.Location.Y);

					pt = segment.EndPoint;
				}

				return boundary;
			}
		}

		/// <summary>
		/// polyline Id
		/// </summary>
		public int Id { get; set; }

		#endregion Properties

		#region COM support properties

		/// <summary>
		/// Gets segments of <c>PolyLine2D</c>.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[XmlArray(ElementName = "Segments")]
		[XmlArrayItem(Type = typeof(Segment2D), ElementName = "Segment")]
		public IList SegmentsCom
		{
			get { return segments as IList; }
		}

		#endregion COM support properties

		#region Public methods

		/// <summary>
		/// Close <c>PolyLine2D</c> by a LineSegment.
		/// </summary>
		public void Close()
		{
			if (Segments.Count > 0 && StartPoint != Segments[Segments.Count - 1].EndPoint)
			{
				Segments.Add(new LineSegment2D(StartPoint));
			}
		}

		#endregion Public methods

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public virtual object Clone()
		{
			return new PolyLine2D(this);
		}

		#endregion ICloneable Members
	}
}