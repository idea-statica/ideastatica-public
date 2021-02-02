using System;
using System.Windows;
using System.Windows.Media;

namespace CI.Geometry2D
{
	public class CircArc2DDiscretizator : ISegment2DDiscretizator
	{
		#region Fields

		private double angle = 22.5;
		private int numberOfTiles = 3;

		#endregion Fields

		#region Public properties

		/// <summary>
		/// Sets angle, in degrees, for discretization of a circular segment.
		/// </summary>
		public double Angle
		{
			set
			{
				if (value == 0)
				{
					System.Diagnostics.Debug.Fail("Angle cannot be zero!\nThe value is not set.");
				}
				else
				{
					angle = Math.Abs(value);
				}
			}
		}

		/// <summary>
		/// Sets the minimal number of tiles of arc segment.
		/// </summary>
		public int NumberOfTiles
		{
			set { this.numberOfTiles = value; }
		}

		#endregion Public properties

		/// <summary>
		/// Divide <c>CircularArcSegment2D</c> on straight lines polygon.
		/// </summary>
		/// <param name="start">The start point of segment.</param>
		/// <param name="source">The segment to discretization.</param>
		/// <param name="target">The <c>Region2D</c> as target of discretization.</param>
		public void Discretize(Point start, ISegment2D source, IPolygon2D target)
		{
			CircularArcSegment2D segment = source as CircularArcSegment2D;

			// The angle in degrees
			double totalAngle = Math.Abs(segment.GetAngle(ref start));

			if (angle < totalAngle || this.numberOfTiles > 1)
			{
				int numberOfTiles = Math.Max((int)Math.Ceiling(totalAngle / angle), this.numberOfTiles);

				double deltaAngle = totalAngle / numberOfTiles * segment.IsCounterClockwise(ref start);
				Matrix mat = new Matrix();
				Point centre = segment.GetCentre(ref start);
				for (int i = 1; i < numberOfTiles; ++i)
				{
					mat.RotateAt(deltaAngle, centre.X, centre.Y);
					target.Add(mat.Transform(start));
				}
			}

			target.Add(segment.EndPoint);
		}
	}
}