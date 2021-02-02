using System;
using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Discretize lines 2D.
	/// </summary>
	internal class Line2DDiscretizator : ISegment2DDiscretizator
	{
		#region Fields

		private int numberOfTiles = 1;
		private double lengthOfTile = 100;

		#endregion Fields

		#region Public properties

		/// <summary>
		/// Sets number of tiles for discretization of a line segment.
		/// </summary>
		public int NumberOfTiles
		{
			set { numberOfTiles = value; }
		}

		/// <summary>
		/// Sets the length of tile.
		/// </summary>
		public double LengthOfTile
		{
			set
			{
				if (value == 0)
				{
					System.Diagnostics.Debug.Fail("LengthOfTile cannot be zero!\nThe value is not set.");
				}
				else
				{
					lengthOfTile = Math.Abs(value);
				}
			}
		}

		#endregion Public properties

		/// <summary>
		/// Divide <c>LineSegment2D</c> on straight lines polygon.
		/// </summary>
		/// <param name="start">The start point of segment.</param>
		/// <param name="source">The segment to discretization.</param>
		/// <param name="target">The <c>Polygon2D</c> as target of discretization.</param>
		public void Discretize(Point start, ISegment2D source, IPolygon2D target)
		{
			double length = source.GetLength(ref start);
			if (numberOfTiles > 1 || length > lengthOfTile)
			{
				int tiles = Math.Max(numberOfTiles, (int)Math.Ceiling(length / lengthOfTile));
				LineSegment2D segment = source as LineSegment2D;
				Vector u = Point.Subtract(segment.EndPoint, start) / tiles;
				Point pt = start;
				for (int i = 1; i < tiles; ++i)
				{
					pt = Point.Add(pt, u);
					target.Add(pt);
				}
			}

			target.Add(source.EndPoint);
		}
	}
}