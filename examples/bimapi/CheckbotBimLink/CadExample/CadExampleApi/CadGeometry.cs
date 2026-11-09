using System;
using System.Collections.Generic;

namespace CadExampleApi
{
	/// <summary>
	/// A point in the model. Coordinates are in metres.
	/// </summary>
	public class CadPoint3D
	{
		public CadPoint3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public double X { get; }

		public double Y { get; }

		public double Z { get; }

		/// <summary>
		/// The vector from this point to the end point.
		/// </summary>
		public CadVector3D VectorTo(CadPoint3D end) => new CadVector3D(end.X - X, end.Y - Y, end.Z - Z);
	}

	/// <summary>
	/// A point in the XY plane of a coordinate system.
	/// </summary>
	public class CadPoint2D
	{
		public CadPoint2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double X { get; }

		public double Y { get; }
	}

	/// <summary>
	/// A direction or an offset in the model.
	/// </summary>
	public class CadVector3D
	{
		public CadVector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public double X { get; }

		public double Y { get; }

		public double Z { get; }

		/// <summary>
		/// The length of the vector.
		/// </summary>
		public double Length => Math.Sqrt((X * X) + (Y * Y) + (Z * Z));

		/// <summary>
		/// The vector of length one in the same direction.
		/// </summary>
		public CadVector3D Normalize() => new CadVector3D(X / Length, Y / Length, Z / Length);

		/// <summary>
		/// The cross product: a vector perpendicular to both vectors.
		/// </summary>
		public CadVector3D Cross(CadVector3D other) => new CadVector3D(
			(Y * other.Z) - (Z * other.Y),
			(Z * other.X) - (X * other.Z),
			(X * other.Y) - (Y * other.X));
	}

	/// <summary>
	/// An origin and three unit axes. Plate outlines and bolt positions are given in the XY plane of one.
	/// </summary>
	public class CadCoordinateSystem
	{
		public CadCoordinateSystem(CadPoint3D origin, CadVector3D x, CadVector3D y, CadVector3D z)
		{
			Origin = origin;
			X = x;
			Y = y;
			Z = z;
		}

		public CadPoint3D Origin { get; }

		public CadVector3D X { get; }

		public CadVector3D Y { get; }

		public CadVector3D Z { get; }

		/// <summary>
		/// Converts a point in the XY plane of this coordinate system to model coordinates.
		/// </summary>
		public CadPoint3D ToWorld(CadPoint2D point) => new CadPoint3D(
			Origin.X + (point.X * X.X) + (point.Y * Y.X),
			Origin.Y + (point.X * X.Y) + (point.Y * Y.Y),
			Origin.Z + (point.X * X.Z) + (point.Y * Y.Z));
	}

	/// <summary>
	/// A closed polygon in the XY plane of a coordinate system. The last point is the same as the first.
	/// </summary>
	public class CadOutline2D
	{
		public CadOutline2D(CadCoordinateSystem coordinateSystem, IReadOnlyList<CadPoint2D> points)
		{
			CoordinateSystem = coordinateSystem;
			Points = points;
		}

		public CadCoordinateSystem CoordinateSystem { get; }

		public IReadOnlyList<CadPoint2D> Points { get; }
	}
}
