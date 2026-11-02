using System.Reflection;
using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Creates a elliptical arc between two points in a PolyLine2D.
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class EllipticArcSegment2D : Segment2D
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the EllipticArcSegment2D class.
		/// </summary>
		public EllipticArcSegment2D()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Copies the source into a new instance of <c>EllipticArcSegment2D</c>.
		/// </summary>
		/// <param name="source">The source.</param>
		public EllipticArcSegment2D(EllipticArcSegment2D source)
		{
			EndPoint = source.EndPoint;
		}

		#endregion Constructors

		#region Segment2D overrides

		/// <summary>
		/// Gets the length of a elliptic arc segment.
		/// </summary>
		/// <param name="start">The start point of elliptic arc segment.</param>
		/// <returns>The lenght of segment.</returns>
		public override double GetLength(ref Point start)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Gets the rectangular boundary of a elliptical segment.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <returns>The boundary of segment.</returns>
		public override Rect GetBounds(ref Point start)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Gets a point on this segment defined by relative position.
		/// This method doesn't checks, if relative position is in range (0, 1).
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="relPosition">The relative position on the segment.</param>
		/// <returns>A point on the segment.</returns>
		public override Point GetPointOnSegment(ref Point start, double relPosition)
		{
			throw new System.NotImplementedException();
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
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Gets a functional values in defined x position of the ElipticArcSegment.
		/// </summary>
		/// <param name="start">The start point of the parabolic eliptic arc segment.</param>
		/// <param name="x">The x position.</param>
		/// <param name="global">True, if x and returned y value is are global space, false, if x and y are related to the start point.</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>A values related to the x position.</returns>
		public override double[] GetValueByX(ref Point start, double x, bool global, double tolerance = 0)
		{
			throw new System.NotImplementedException();
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
			throw new System.NotImplementedException();
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
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Calculates a curvature in specified x position.
		/// </summary>
		/// <param name="start">The start point.</param>
		/// <param name="x">The x coordinate.</param>
		/// <returns>The curvature.</returns>
		public override double GetCurvature(ref Point start, double x)
		{
			throw new System.NotImplementedException();
		}

		#endregion Segment2D overrides

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new EllipticArcSegment2D(this);
		}

		#endregion ICloneable Members
	}
}