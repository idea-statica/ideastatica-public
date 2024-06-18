using System;
using CI.Mathematics;
using WM = System.Windows.Media.Media3D;

namespace CI.Geometry3D
{
	/// <summary>
	/// Geometry operations related with Point3D
	/// </summary>
	public static partial class GeomOperation
	{
		/// <summary>
		/// Adding vector to a point
		/// </summary>
		/// <param name="point">Point</param>
		/// <param name="vector">Vector</param>
		/// <returns>Returns the resultant sum</returns>
		public static IPoint3D Add(IPoint3D point, Vector3D vector)
		{
			IPoint3D newPoint = new Point3D(point.X + vector.DirectionX, point.Y + vector.DirectionY, point.Z + vector.DirectionZ);
			return newPoint;
		}

		/// <summary>
		/// Adding two points
		/// </summary>
		/// <param name="point1">first point</param>
		/// <param name="point2">secont point</param>
		/// <returns>Returns the resultant sum</returns>
		public static IPoint3D Add(IPoint3D point1, IPoint3D point2)
		{
			IPoint3D point = new Point3D(point1.X + point2.X, point1.Y + point2.Y, point1.Z + point2.Z);
			return point;
		}

		/// <summary>
		/// Adding two points
		/// </summary>
		/// <param name="point1">first point</param>
		/// <param name="point2">secont point</param>
		/// <returns>Returns the resultant sum</returns>
		public static WM.Point3D Add(WM.Point3D point1, IPoint3D point2)
		{
			return new WM.Point3D(point1.X + point2.X, point1.Y + point2.Y, point1.Z + point2.Z);
		}

		/// <summary>
		/// Make a clone of given point
		/// </summary>
		/// <param name="point">Source point</param>
		/// <returns>New point</returns>
		public static IPoint3D Clone(IPoint3D point)
		{
			return new Point3D(point.X, point.Y, point.Z);
		}

		/// <summary>
		/// Converting Geometry2D.IdaComPoint2D to Geometry3D.IPoint3D
		/// </summary>
		/// <param name="point2D">Geometry2D.IdaComPoint2D</param>
		/// <returns>Geometry3D.IPoint3D</returns>
		public static IPoint3D ConvertTo3D(Geometry2D.IdaComPoint2D point2D)
		{
#if NET48
			if (point2D == null)
			{
				return null;
			}
#endif

			return new Point3D(point2D.X, point2D.Y, 0);
		}

		/// <summary>
		/// Converting Geometry2D.IdaComPoint2D to Geometry3D.IPoint3D swapping Y->Z values
		/// </summary>
		/// <param name="point2D">Geometry2D.IdaComPoint2D</param>
		/// <param name="Yvalue">Yvalue to add</param>
		/// <returns>Geometry3D.IPoint3D</returns>
		public static IPoint3D ConvertTo3D(Geometry2D.IdaComPoint2D point2D, double Yvalue)
		{
#if NET48
			if (point2D == null)
			{
				return null;
			}
#endif
			return new Point3D(point2D.X, Yvalue, point2D.Y);
		}

		/// <summary>
		/// Assign values of right to left. This will assign values only
		/// </summary>
		/// <param name="left">Point on left side of operation</param>
		/// <param name="right">Point on right side of operation</param>
		public static void CopyValues(IPoint3D left, IPoint3D right)
		{
			left.X = right.X;
			left.Y = right.Y;
			left.Z = right.Z;
		}

		/// <summary>
		/// Calculate and return distance between two Point
		/// </summary>
		/// <param name="point1">first point</param>
		/// <param name="point2">second point</param>
		/// <returns>The distance between points</returns>
		public static double Distance(IPoint3D point1, IPoint3D point2)
		{
			//double xx = System.Math.Abs(point1.X - point2.X);
			//double yy = System.Math.Abs(point1.Y - point2.Y);
			//double zz = System.Math.Abs(point1.Z - point2.Z);
			//return System.Math.Sqrt((xx * xx) + (yy * yy) + (zz * zz));

			return Subtract(point1, point2).Magnitude;
		}

		/// <summary>
		/// Gets distance of the <paramref name="point"/> from <paramref name="line"/>
		/// </summary>
		/// <param name="line">Line</param>
		/// <param name="point">Point</param>
		/// <returns>Distance</returns>
		public static double Distance(ref StraightLine line, ref WM.Point3D point)
		{
			Plane3D tempPln = new Plane3D(point, line.Direction);
			var temPlnIntersection = tempPln.GetIntersection(ref line);
			return (temPlnIntersection - point).Length;
		}

		/// <summary>
		/// Returns the distance of <paramref name="line1"/> and <paramref name="line2"/>
		/// </summary>
		/// <param name="line1">The first line</param>
		/// <param name="line2">The second line</param>
		/// <returns>Distance</returns>
		public static double Distance(ref StraightLine line1, ref StraightLine line2)
		{
			var dot = Math.Abs(WM.Vector3D.DotProduct(line1.Direction, line2.Direction));
			if (dot.IsEqual(1, 1e-8))
			{
				// lines are parallel
				var pln = new Plane3D(line1.Point, line1.Direction);
				var line2Intersection = pln.GetIntersection(ref line2);
				return (line1.Point - line2Intersection).Length;
			}

			var cross = WM.Vector3D.CrossProduct(line1.Direction, line2.Direction);
			var pln2 = new Plane3D(line1.Point, cross);
			var dist = pln2.GetPointDistance(ref line2.Point);
			return Math.Abs(dist);
		}

		/// <summary>
		/// Create and return a matrix based on given two points
		/// The matrix represents a coordinate system. 
		/// </summary>
		/// <param name="origin">Origin of the new coordinate system</param>
		/// <param name="pointOnX">Point on which new X axis lies</param>
		/// <returns>The matrix which represents the co-ordinate system</returns>
		public static Matrix44 GetLCSMatrix(IPoint3D origin, IPoint3D pointOnX)
		{
			//Vector3D localX = GeomOperation.Subtract(pointOnX, origin).Normalize;
			Vector3D localX = GeomOperation.Subtract(pointOnX, origin);
			Vector3D localZ = new Vector3D(0, 0, 1);
			if (GeomOperation.IsCollinear(localZ, localX, MathConstants.ZeroWeak)) //MathConstants.ZeroGeneral))
			{
				// When X is vertical then Z goes in X dir or -X
				//localZ = new Vector3D(0, 1, 0);
				if (localX.DirectionZ > 0)
				{
					localZ = new Vector3D(-1, 0, 0);
				}
				else
				{
					localZ = new Vector3D(1, 0, 0);
				}
			}

			//Vector3D localY = (localZ * localX).Normalize;
			//Matrix44 matrix = new Matrix44(origin, localX, localY);
			Vector3D localY = (localZ * localX);
			localZ = (localX * localY).Normalize;
			Matrix44 matrix = new Matrix44(origin, localX.Normalize, localY.Normalize, localZ);
			return matrix;
		}

		/// <summary>
		/// Create and return a matrix based on given two points
		/// The matrix represents a coordinate system. 
		/// </summary>
		/// <param name="origin">Origin of the new coordinate system</param>
		/// <param name="pointOnX">Point on which new X axis lies</param>
		/// <returns>The matrix which represents the co-ordinate system</returns>
		public static Matrix44 GetLCSMatrix(WM.Point3D origin, WM.Point3D pointOnX)
		{
			Vector3D localX = new Vector3D(pointOnX.X - origin.X, pointOnX.Y - origin.Y, pointOnX.Z - origin.Z);
			Vector3D localZ = new Vector3D(0, 0, 1);
			if (GeomOperation.IsCollinear(localZ, localX, MathConstants.ZeroWeak)) //MathConstants.ZeroGeneral))
			{
				if (localX.DirectionZ > 0)
				{
					localZ = new Vector3D(-1, 0, 0);
				}
				else
				{
					localZ = new Vector3D(1, 0, 0);
				}
			}

			Vector3D localY = (localZ * localX);
			localZ = (localX * localY).Normalize;
			Matrix44 matrix = new Matrix44(origin, localX.Normalize, localY.Normalize, localZ);
			return matrix;
		}

		/// <summary>
		/// Create and return a matrix based on given points
		/// The matrix represents a coordinate system. 
		/// </summary>
		/// <param name="origin">Origin of the new coordinate system</param>
		/// <param name="pointOnX">Point on which new X axis lies</param>
		/// <param name="pointInPlane">Third point on the plane</param>
		/// <param name="plane">Specifies pointInPlane is in XY plane or in XZ plane</param>
		/// <returns>Return the matrix which represents the co-ordinate system</returns>
		public static IMatrix44 GetLCSMatrix(IPoint3D origin, IPoint3D pointOnX, IPoint3D pointInPlane, Plane plane)
		{
			// Vector3D localX = GeomOperation.Subtract(pointOnX, origin).Normalize;
			Vector3D localX = GeomOperation.Subtract(pointOnX, origin);
			Vector3D localY = GeomOperation.Subtract(pointInPlane, origin);
			if (GeomOperation.IsCollinear(localY, localX, MathConstants.ZeroGeneral))
			{
				//throw new NotSupportedException("Given points are in a line. Unable to create coordinate system");
				return null;
			}

			if (plane == Plane.XY)
			{
				//Vector3D localZ = localX * localY;
				//localY = (localZ * localX).Normalize;
				////TODO localY = localY.Normalize; only
				//Matrix44 matrix = new Matrix44(origin, localX, localY);
				Vector3D localZ = localX * localY;
				localY = (localZ * localX);
				localZ = (localX * localY).Normalize;
				IMatrix44 matrix = new Matrix44(origin, localX.Normalize, localY.Normalize, localZ);
				return matrix;
			}
			else if (plane == Plane.ZX)
			{
				//Vector3D localZ = (localY * localX).Normalize;
				//Matrix44 matrix = new Matrix44(origin, localX, localZ);
				Vector3D localZ = (localY * localX);
				localY = (localX * localZ).Normalize;
				IMatrix44 matrix = new Matrix44(origin, localX.Normalize, localZ.Normalize, localY);
				return matrix;
			}

			throw new NotSupportedException("Third point should be either in XY plane or in ZX plane");
		}

		///// <summary>
		///// Gets local transformation matrix at point
		///// </summary>
		///// <param name="point">Point</param>
		///// <returns>Transformation matrix</returns>
		//public static IMatrix44 GetTransformation(Point3D point)
		//{
		//	return point.GetCoordinateSystemMatrix();
		//}

		/// <summary>
		/// Determines whether the two points are equal or not.
		/// </summary>
		/// <param name="point1">Point on LHS</param>
		/// <param name="point2">Point on RHS</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>True if two points are on same position</returns>
		public static bool IsEqual(IPoint3D point1, IPoint3D point2, double toleranceLevel = MathConstants.ZeroGeneral)
		{
			if (point1 == null || point2 == null)
			{
				return false;
			}

			return Distance(point1, point2).IsZero(toleranceLevel);
		}

		/// <summary>
		/// Check the value is inside interval
		/// This function doesn't consider begin and end values
		/// </summary>
		/// <param name="value">Value to be checked with interval</param>
		/// <param name="begin">Begin of interval</param>
		/// <param name="end">End of interval</param>
		/// <returns>True if value lies in interval</returns>
		public static bool IsInInterval(double value, double begin, double end)
		{
			if (begin < value && end > value)
			{
				return true;
			}

			if (end < value && begin > value)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Check the value is inside or on interval
		/// This function consider begin and end values
		/// </summary>
		/// <param name="value">Value to be checked with interval</param>
		/// <param name="begin">Begin of interval</param>
		/// <param name="end">End of interval</param>
		/// <returns>True if value lies in interval</returns>
		public static bool IsOnInterval(double value, double begin, double end)
		{
			if (begin.IsLesser(value) && end.IsGreater(value))
			{
				return true;
			}

			if (end.IsLesser(value) && begin.IsGreater(value))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Move point according to given vector
		/// </summary>
		/// <param name="point">point to be moved</param>
		/// <param name="displacement">Displacement vector</param>
		public static void Move(IPoint3D point, Vector3D displacement)
		{
			point.X += displacement.DirectionX;
			point.Y += displacement.DirectionY;
			point.Z += displacement.DirectionZ;
		}

		/// <summary>
		/// Move point according to given vector
		/// </summary>
		/// <param name="point">point to be moved</param>
		/// <param name="displacement">Displacement vector</param>
		public static void Move(IPoint3D point, ref WM.Vector3D displacement)
		{
			point.X += displacement.X;
			point.Y += displacement.Y;
			point.Z += displacement.Z;
		}

		/// <summary>
		/// Multiply point and double.
		/// </summary>
		/// <param name="point">Point to be multiplied</param>
		/// <param name="scalar">value to be multiplied</param>
		/// <returns>The multiplied point</returns>
		public static IPoint3D Multiply(IPoint3D point, double scalar)
		{
			if (point == null)
			{
				return null;
			}

			IPoint3D newPoint = new Point3D(point.X * scalar, point.Y * scalar, point.Z * scalar);
			return newPoint;
		}

		/// <summary>
		/// Subtracting two points
		/// </summary>
		/// <param name="point1">first point</param>
		/// <param name="point2">secont point</param>
		/// <returns>Returns the resultant difference vector</returns>
		public static Vector3D Subtract(IPoint3D point1, IPoint3D point2)
		{
			if (point1 == null || point2 == null)
			{
				return new Vector3D();
			}

			Vector3D vector = new Vector3D(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);
			return vector;
		}

		/// <summary>
		/// Subtracting two points
		/// </summary>
		/// <param name="point1">first point</param>
		/// <param name="point2">secont point</param>
		/// <returns>Returns the resultant difference vector</returns>
		public static Vector3D Subtract(WM.Point3D point1, IPoint3D point2)
		{
			Vector3D vector = new Vector3D(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);
			return vector;
		}

		/// <summary>
		/// Converts the given vector to point
		/// </summary>
		/// <param name="vector">Input vector</param>
		/// <returns>return the vector created from point</returns>
		public static Point3D ToPoint(Vector3D vector)
		{
			return new Point3D(vector.DirectionX, vector.DirectionY, vector.DirectionZ);
		}

		/// <summary>
		/// Converts the given point to vector
		/// </summary>
		/// <param name="point">Input point</param>
		/// <returns>return the vector created from point</returns>
		public static Vector3D ToVector(IPoint3D point)
		{
			return new Vector3D(point.X, point.Y, point.Z);
		}

		/// <summary>
		/// Calculate the Relative Point on the line created with the two given point
		/// </summary>
		/// <param name="startPoint">Start point of Line</param>
		/// <param name="endPoint">End point of Line</param>
		/// <param name="relativePosition">Relative Position</param>
		/// <returns>Point on Line based on Relative Position</returns>
		public static IPoint3D GetLinearPoint(IPoint3D startPoint, IPoint3D endPoint, double relativePosition = 0.5)
		{
			//IPoint3D linearPoint = Multiply(startPoint, 1.0 - relativePosition);
			//linearPoint = Add(linearPoint, Multiply(endPoint, relativePosition));
			//return linearPoint;

			Vector3D vect = Subtract(endPoint, startPoint);
			vect = vect * relativePosition;
			return Add(startPoint, vect);
		}

		/// <summary>
		/// Calculate normal of line passing through start and end. Default plane is XY
		/// </summary>
		/// <param name="start">Start point</param>
		/// <param name="end">End point</param>
		/// <returns>Returns the normal</returns>
		private static Vector3D GetNormal(IPoint3D start, IPoint3D end)
		{
			Vector3D localZ = new Vector3D(0, 0, 1);
			Vector3D localX = Subtract(end, start).Normalize;
			if (GeomOperation.IsCollinear(localX, localZ, MathConstants.ZeroGeneral))
			{
				localZ = new Vector3D(0, 1, 0);
			}

			Vector3D localY = localZ * localX;
			return localX * localY;
		}
	}
}