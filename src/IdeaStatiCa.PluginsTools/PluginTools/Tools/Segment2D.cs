using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;


namespace CI.Geometry2D
{
	/// <summary>
	/// The base class for 1D segments (primitives) in 2D plane.
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[XmlInclude(typeof(LineSegment2D))]
	[DebuggerDisplay("EndPoint=[{EndPoint.X}; {EndPoint.Y}]")]
	public abstract class Segment2D : ISegment2D, ISegment2DCom
	{
		private Point endPoint;

		/// <summary>
		/// Gets or sets the endpoint of the segment.
		/// </summary>
		public IdaComPoint2D EndPoint
		{
			get { return endPoint; }
			set { endPoint = value; }
		}

		/// <summary>
		/// Get, sets x-coordinate of the end point
		/// </summary>
		[XmlIgnore]
		public double EndX
		{
			get { return endPoint.X; }
			set { endPoint.X = value; }
		}

		/// <summary>
		/// Gets, sets y-coordinate of the end point
		/// </summary>
		[XmlIgnore]
		public double EndY
		{
			get { return endPoint.Y; }
			set { endPoint.Y = value; }
		}

		/// <summary>
		/// Gets the length of a segment.
		/// </summary>
		/// <param name="start">The start point of segment.</param>
		/// <returns>The lenght of segment.</returns>
		public abstract double GetLength(ref Point start);

		/// <summary>
		/// Gets the rectangular boundary of a segment.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <returns>The boundary of segment.</returns>
		public abstract Rect GetBounds(ref Point start);

		/// <summary>
		/// Gets a point on this segment defined by relative position.
		/// This method doesn't checks, if relative position is in range (0, 1).
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="relPosition">The relative position on the segment.</param>
		/// <returns>A point on the segment.</returns>
		public abstract Point GetPointOnSegment(ref Point start, double relPosition);

		/// <summary>
		/// Gets a tangent on point on this segment defined by relative position.
		/// This method doesn't checks, if relative position is in range (0, 1).
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="relPosition">The relative position on the segment.</param>
		/// <returns>A tangent on point on the segment.</returns>
		public abstract Vector GetTangentOnSegment(ref Point start, double relPosition);

		/// <summary>
		/// Gets a functional values in defined x position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="x">The x position.</param>
		/// <param name="global">True, if x and returned y value is are global space, false, if x and y are related to the start point.</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>A values related to the x position.</returns>
		public abstract double[] GetValueByX(ref Point start, double x, bool global, double tolerance = 0);

		/// <summary>
		/// Gets a functional values in defined y position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="y">The y position.</param>
		/// <param name="global">True, if y and returned x value is are global space, false, if y and x are related to the start point.</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>A values related to the y position.</returns>
		public abstract double[] GetValueByY(ref Point start, double y, bool global, double tolerance = 0);

		/// <summary>
		/// Calculates relative position on segment by given x position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="x">The x position.</param>
		/// <param name="global">True, if x and returned y value is are global space, false, if x and y are related to the start point.</param>
		/// <param name="tolerance">Distance tolerance.</param>
		/// <returns>The relative position.</returns>
		public abstract double[] GetRelativePosition(ref Point start, double x, bool global, double tolerance = 0);

		/// <summary>
		/// Gets a curvature in specified x position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="x">The x position.</param>
		/// <returns>The curvature.</returns>
		public abstract double GetCurvature(ref Point start, double x);

		/// <summary>
		/// Reverse segment orientation.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <returns>A new start point.</returns>
		public Point Reverse(ref Point start)
		{
			var temp = this.EndPoint;
			this.EndPoint = start;
			return temp;
		}

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public abstract object Clone();

		#endregion ICloneable Members
	}
}