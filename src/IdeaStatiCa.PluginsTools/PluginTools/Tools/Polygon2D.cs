using System;
using System.Collections.Generic;
using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Represents a subsection of a 2D geometry as a simple 2D polygon (array of Point).
	/// </summary>
	public class Polygon2D : List<Point>, IPolygon2D
	{
		/// <summary>
		/// Initializes a new instance of the <c>CI.Geometry2D.Polygon2D</c> class
		/// that is empty and has the default initial capacity.
		/// </summary>
		public Polygon2D()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <c>CI.Geometry2D.Polygon2D</c> class
		/// that is empty and has the specified initial capacity.
		/// </summary>
		/// <param name="capacity">The number of elements that the new list can initially store.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">capacity is less than 0.</exception>
		public Polygon2D(int capacity)
			: base(capacity)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <c>CI.Geometry2D.Polygon2D</c> class
		/// that contains elements copied from the specified collection and has sufficient
		/// capacity to accommodate the number of elements copied.
		/// </summary>
		/// <param name="pts">The collection whose <c>System.Windows.Point</c> are copied to the new <c>CI.Geometry2D.Polygon2D</c>.</param>
		/// <exception cref="System.ArgumentNullException">collection is null.</exception>
		public Polygon2D(IEnumerable<Point> pts)
			: base(pts)
		{
		}

		/// <summary>
		/// Gets a value that specifies whether first and last points are connected.
		/// </summary>
		public bool IsClosed
		{
			get
			{
				if (Count > 2 && this[0].Equals(this[Count - 1]))
				{
					return true;
				}

				return false;
			}
		}

		/// <summary>
		/// Gets the length of polygon.
		/// </summary>
		public double Length
		{
			get
			{
				if (Count < 2)
				{
					return 0;
				}

				double length = 0;
				int count = Count;
				double x, y;
				for (int i = count - 2; i >= 0; --i)
				{
					x = this[i + 1].X - this[i].X;
					y = this[i + 1].Y - this[i].Y;
					length += Math.Sqrt((x * x) + (y * y));
				}

				return length;
			}
		}

		/// <summary>
		/// Gets the area of this polygon.
		/// </summary>
		public double Area
		{
			get
			{
				if (Count < 3)
				{
					return 0.0;
				}

				double area = 0.0;
				int count = Count;
				double x, y;
				for (int i = count - 2; i >= 0; --i)
				{
					x = this[i].X;
					y = this[i].Y;
					area += (x * this[i + 1].Y) - (x * y) - (y * this[i + 1].X) + (x * y);
				}

				if (!IsClosed)
				{
					x = this[count - 1].X;
					y = this[count - 1].Y;
					area += (x * this[0].Y) - (x * y) - (y * this[0].X) + (x * y);
				}

				return area * 0.5;
			}
		}

		/// <summary>
		/// Gets the rectangular boundary of a <c>CI.Geometry2D.Polygon2D</c>.
		/// </summary>
		public Rect Bounds
		{
			get
			{
				var min = new Point(double.PositiveInfinity, double.PositiveInfinity);
				var max = new Point(double.NegativeInfinity, double.NegativeInfinity);

				for (int i = Count - 1; i >= 0; --i)
				{
					if (this[i].X < min.X)
					{
						min.X = this[i].X;
					}

					if (this[i].X > max.X)
					{
						max.X = this[i].X;
					}

					if (this[i].Y < min.Y)
					{
						min.Y = this[i].Y;
					}

					if (this[i].Y > max.Y)
					{
						max.Y = this[i].Y;
					}
				}

				return new Rect(min, max);
			}
		}

		/// <summary>
		/// Gets an information that specifies whether the orientation of polygon is counterclockwise.
		/// </summary>
		public bool IsCounterClockwise
		{
			get
			{
				return Area >= 0.0;
			}
		}

		/// <summary>
		/// Makes the polygon closed (1'st and last point equals).
		/// </summary>
		public void Close()
		{
			if (Count > 1 && !IsClosed)
			{
				this.Add(this[0]);
			}
		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of the <c>Polygon2D</c>.
		/// </summary>
		/// <param name="collection">The collection whose elements should be added to the end of the <c>Polygon2D</c>.
		/// The collection itself cannot be null.</param>
		/// <exception cref="System.ArgumentNullException">The collection is null.</exception>
		public new void AddRange(IEnumerable<Point> collection)
		{
			base.AddRange(collection);
		}

		/// <summary>
		/// Reverses the order of the elements in the entire <c>Polygon2D</c>.
		/// </summary>
		public new void Reverse()
		{
			base.Reverse();
		}

		/// <summary>
		/// Removes duplicate points.
		/// </summary>
		public void RemoveDuplicateConsecutivePoints()
		{
			for (int i = Count - 1; i > 0; --i)
			{
				if (this[i].IsEqualWithTolerance(this[i - 1], 1e-12))
				{
					this.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Remove collinear and duplicate points.
		/// </summary>
		/// <param name="tolerance"></param>
		public void Simplify(double tolerance = 1e-12)
		{
			var tolerance2 = tolerance * 1e4;
			for (int i = Count - 2; i > 0; --i)
			{
				// smazu duplicitni
				if (this[i + 1].IsEqualWithTolerance(this[i], tolerance))
				{
					this.RemoveAt(i + 1);
					continue;
				}

				// tri po sobe jdouci body
				var v1 = Point.Subtract(this[i], this[i - 1]);
				var v2 = Point.Subtract(this[i + 1], this[i]);
				if (Vector.CrossProduct(v1, v2).IsZero(tolerance2))
				{
					this.RemoveAt(i);
				}
			}
		}

		public Point GetPoint(double relative)
		{
			double l = Length;
			double absolute = relative * l;
			int count = Count;

			if (count < 1)
			{
				return new Point();
			}

			if ((count < 2) || relative.IsZero())
			{
				return this[0];
			}

			if (relative.IsGreaterOrEqual(1.0))
			{
				return this[count - 1];
			}

			double length = 0;
			double x, y;
			for (int i = 1; i < count; i++)
			{
				x = this[i].X - this[i - 1].X;
				y = this[i].Y - this[i - 1].Y;
				double locLen = Math.Sqrt((x * x) + (y * y));
				if ((length + locLen).IsEqual(absolute))
				{
					return this[i];
				}

				if ((length + locLen).IsGreater(absolute))
				{
					double d = (absolute - length) / locLen;
					return new Point(this[i - 1].X + d * x, this[i - 1].Y + d * y);
				}

				length += locLen;
			}

			return new Point();
		}

		/// <summary>
		/// Compare points of polygon with <paramref name="pattern"/>.
		/// </summary>
		/// <param name="pattern">Polygon to be compared with this instance.</param>
		/// <param name="tolerance">Tolerance used in comparison of coordinates</param>
		/// <returns>True if all points are equal.</returns>
		public bool EqualPoints(IPolygon2D pattern, double tolerance)
		{
			for (int i = 0, sz = this.Count; i < sz; i++)
			{
				Point left = this[i];
				Point right = pattern[i];
				if (!left.X.IsEqual(right.X, tolerance) || !left.Y.IsEqual(right.Y, tolerance))
				{
					return false;
				}
			}

			return true;
		}

		#region ICloneable Members

		public object Clone()
		{
			return MemberwiseClone();
		}

		#endregion ICloneable Members
	}
}