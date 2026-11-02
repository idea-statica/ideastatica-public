using System;
using System.Windows;
using System.Windows.Media;

namespace CI.Geometry2D
{
	/// <summary>
	/// Discretization support for parabolic arc segment.
	/// </summary>
	public class ParabolicArc2DDiscretizator : ISegment2DDiscretizator
	{
		#region Fields

		private double angle = 11.25;
		private int numberOfTiles = 5;

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
		/// Divide <c>ParabolicArcSegment2D</c> on straight lines polygon.
		/// </summary>
		/// <param name="start">The start point of segment.</param>
		/// <param name="source">The segment to discretization.</param>
		/// <param name="target">The <c>Region2D</c> as target of discretization.</param>
		public void Discretize(Point start, ISegment2D source, IPolygon2D target)
		{
			var segment = source as ParabolicArcSegment2D;

			// The angle in degrees
			double totalAngle = segment.GetAngle(ref start);

			if (angle < Math.Abs(totalAngle) || this.numberOfTiles > 1)
			{
				int numberOfTiles = Math.Max((int)Math.Ceiling(Math.Abs(totalAngle) / angle), this.numberOfTiles);

				double deltaAngle = totalAngle / numberOfTiles;
				var mat = new Matrix();
				mat.Rotate(deltaAngle);
				var t = segment.GetTangentOnSegment(ref start, 0);
				for (int i = 1; i < numberOfTiles; ++i)
				{
					t = mat.Transform(t);
					if (segment.CoordinatesFromTangent(ref start, t, out Point p))
					{
						target.Add(p);
					}
				}
			}

			target.Add(segment.EndPoint);
		}
	}
}