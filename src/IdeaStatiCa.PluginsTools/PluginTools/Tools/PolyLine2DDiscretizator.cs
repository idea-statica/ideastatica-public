using System;
using System.Collections.Generic;
using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Provides discretization of all segments of polyline to the straigt line polygon.
	/// </summary>
	public class PolyLine2DDiscretizator : IDiscretizator
	{
		#region Fields

		private Dictionary<Type, ISegment2DDiscretizator> discretizators;

		#endregion Fields

		/// <summary>
		/// The constructor for registration of segment discretizators.
		/// </summary>
		public PolyLine2DDiscretizator()
		{
			discretizators = new Dictionary<Type, ISegment2DDiscretizator>(2);

			discretizators.Add(typeof(LineSegment2D), new Line2DDiscretizator());
			discretizators.Add(typeof(CircularArcSegment2D), new CircArc2DDiscretizator());
			discretizators.Add(typeof(ParabolicArcSegment2D), new ParabolicArc2DDiscretizator());
		}

		#region Properties

		/// <summary>
		/// Sets the minimal number of tiles on a line segment.
		/// </summary>
		public int NumberOfTiles
		{
			set
			{
				discretizators.TryGetValue(typeof(LineSegment2D), out ISegment2DDiscretizator discretizator);
				if (discretizator != null)
				{
					((Line2DDiscretizator)discretizator).NumberOfTiles = value;
				}
			}
		}

		/// <summary>
		/// Sets the minimal nuber of tiles of arc segment.
		/// </summary>
		public int NumberOfArcTiles
		{
			set
			{
				discretizators.TryGetValue(typeof(CircularArcSegment2D), out ISegment2DDiscretizator discretizator);
				if (discretizator != null)
				{
					((CircArc2DDiscretizator)discretizator).NumberOfTiles = value;
				}

				discretizators.TryGetValue(typeof(ParabolicArcSegment2D), out discretizator);
				if (discretizator != null)
				{
					((ParabolicArc2DDiscretizator)discretizator).NumberOfTiles = value;
				}
			}
		}

		/// <summary>
		/// Sets the average lenght of tile on a line segment.
		/// </summary>
		public double LengthOfTile
		{
			set
			{
				discretizators.TryGetValue(typeof(LineSegment2D), out ISegment2DDiscretizator discretizator);
				if (discretizator != null)
				{
					((Line2DDiscretizator)discretizator).LengthOfTile = value;
				}
			}
		}

		/// <summary>
		/// Sets angle, in degrees, for circular arc segments.
		/// </summary>
		public double Angle
		{
			set
			{
				discretizators.TryGetValue(typeof(CircularArcSegment2D), out ISegment2DDiscretizator discretizator);
				if (discretizator != null)
				{
					((CircArc2DDiscretizator)discretizator).Angle = value;
				}

				discretizators.TryGetValue(typeof(ParabolicArcSegment2D), out discretizator);
				if (discretizator != null)
				{
					((ParabolicArc2DDiscretizator)discretizator).Angle = value;
				}
			}
		}

		#endregion Properties

		/// <summary>
		/// Discretize of <c>PolyLine2D</c> to <c>Polygon2D</c>.
		/// </summary>
		/// <param name="source">The <c>PolyLine2D</c> to discretization.</param>
		/// <param name="target">The <c>Polygon2D</c> as destination.</param>
		/// <param name="targetInxs">The <c>IList</c> as destination.</param>
		public void Discretize(IPolyLine2D source, ref IPolygon2D target, IList<int> targetInxs)
		{
			if (source == null)
			{
				return;
			}

			Point start = source.StartPoint;
			target.Add(start);
			int i = 0;
			int lastInx = 0;
			foreach (var segment in source.Segments)
			{
				SelectSegment2DDiscretizator(segment.GetType()).Discretize(start, segment, target);
				start = segment.EndPoint;
				if (targetInxs != null)
				{
					for (int j = lastInx; j < target.Count; j++)
					{
						targetInxs.Add(i);
					}

					lastInx = target.Count;
				}

				i++;
			}

			if (targetInxs != null)
			{
				if (source.IsClosed && (targetInxs.Count > 0))
				{
					targetInxs[0] = targetInxs[targetInxs.Count - 1];
				}
			}
		}

		/// <summary>
		/// The selection of a segment discretizator.
		/// </summary>
		/// <param name="type">The type of a segment to the discretization.</param>
		/// <returns>The object, which discretize a segment.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException"> discretizator is null.</exception>
		private ISegment2DDiscretizator SelectSegment2DDiscretizator(Type type)
		{
			discretizators.TryGetValue(type, out ISegment2DDiscretizator discretizator);
			if (discretizator == null)
			{
				throw new ArgumentOutOfRangeException(string.Format("PolyLine2DDiscretizator.SelectSegment2DDiscretizator(Type {0}) - discretizator is null", type.Name));
			}

			return discretizator;
		}
	}
}