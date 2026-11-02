using System;
using CI.Mathematics;

namespace CI.Geometry3D
{
	/// <summary>
	/// Geometry operations related with Vector3D
	/// </summary>
	public static partial class GeomOperation
	{
		/// <summary>
		/// GetAngle - Returns the inside angle between vector1 and vector2.
		/// </summary>
		/// <param name="vector1">vector1</param>
		/// <param name="vector2">vector2</param>
		/// <returns>Returns the angle between the vectors in radians (absolute value)</returns>
		public static double GetAngle(Vector3D vector1, Vector3D vector2)
		{
			double temp = (vector1 | vector2) / (vector1.Magnitude * vector2.Magnitude);
			double radian = System.Math.Acos(System.Math.Round(temp, 14));
			return radian;
		}

		/// <summary>
		/// Returns the clockwise angle between two vectors.
		/// </summary>
		/// <param name="vector1">first vector</param>
		/// <param name="vector2">second vector</param>
		/// <param name="normalVector">normal vector of the two. It is considered as vector of rotation to compute the 'direction' of angle.</param>
		/// <returns></returns>
		public static double GetClockwiseAngle(Vector3D vector1, Vector3D vector2, Vector3D normalVector)
		{
			//http://stackoverflow.com/questions/14066933/direct-way-of-computing-clockwise-angle-between-2-vectors

			double x1 = vector1.DirectionX;
			double x2 = vector2.DirectionX;		
			double y1 = vector1.DirectionY;
			double y2 = vector2.DirectionY;	
			double z1 = vector1.DirectionZ;
			double z2 = vector2.DirectionZ;
			Vector3D n = normalVector.Normalize;

			double dot = x1 * x2 + y1 * y2 + z1 * z2;
			double det = x1 * y2 * n.DirectionZ + x2 * n.DirectionY * z1 + n.DirectionX * y1 * z2 - z1 * y2 * n.DirectionX - z2 * n.DirectionY * x1 - n.DirectionZ * y1 * x2;
			double angle = Math.Atan2(det, dot);
			return angle;
		}

		/// <summary>
		/// Calculate the Matrix for a given Normal.
		/// </summary>
		/// <param name="normal">Normal which define plane</param>
		/// <returns>returns the matrix for a given normal</returns>
		public static Matrix44 GetLCSMatrix(Vector3D normal)
		{
			//Matrix44 matrix = new Matrix44();

			//Vector3D vectorZ = normal.Normalize;
			Vector3D vectorZ = normal;
			if (Math.Abs(vectorZ.DirectionZ).IsLesser(MathConstants.ZeroWeak))
			{
				if (Math.Abs(vectorZ.DirectionX).IsLesser(MathConstants.ZeroWeak))
				{
					if (vectorZ.DirectionY.IsLesser(0))
					{
						vectorZ *= -1;
					}
				}
				else if (vectorZ.DirectionX.IsLesser(0))
				{
					vectorZ *= -1;
				}
			}
			else if (vectorZ.DirectionZ.IsLesser(0))
			{
				vectorZ *= -1;
			}

			//Vector3D vectorX = GetLocalX(vectorZ);
			//Vector3D vectorY = vectorZ * vectorX;
			//matrix.AxisX = vectorX;
			//matrix.AxisY = vectorY;
			//matrix.AxisZ = vectorZ;

			Vector3D vectorX = GetLocalX(vectorZ);
			Vector3D vectorY = vectorZ * vectorX;
			Matrix44 matrix = new Matrix44(vectorX.Normalize, vectorY.Normalize, vectorZ.Normalize);
			return matrix;
		}

		/// <summary>
		/// GetOrientAngleAroundAxis - calculate Oriented angle around axis
		/// </summary>
		/// <param name="vector">input vector</param>
		/// <param name="viewVector">view vector</param>
		/// <param name="axisVector">Normal vector</param>
		/// <returns>return angle around axis</returns>
		public static double GetOrientAngleAroundAxis(Vector3D vector, Vector3D viewVector, Vector3D axisVector)
		{
			Vector3D a = axisVector.Normalize;
			Vector3D u = vector.Normalize;
			Vector3D v = viewVector.Normalize;

			u -= (u | a) * a;
			v -= (v | a) * a;

			Vector3D w = u * v;

			double sineAngle = ~w;

			if ((w | axisVector) < 0)
			{
				sineAngle = -sineAngle;
			}

			double cosineAngle = u | v;

			return System.Math.Atan2(sineAngle, cosineAngle);
		}

		/// <summary>
		/// Returns true if both the vectors are collinear
		/// </summary>
		/// <param name="vector1">vector1 to be compared</param>
		/// <param name="vector2">vector2 to be compared</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>return true if two vectors are collinear</returns>
		public static bool IsCollinear(Vector3D vector1, Vector3D vector2, double toleranceLevel = MathConstants.ZeroGeneral)
		{
			var magn1 = vector1.Magnitude;
			var magn2 = vector2.Magnitude;
			if (magn2.IsZero())
			{
				return true;
			}

			double scalar = magn1 / magn2;

			if (scalar.IsGreaterOrEqual(0.0, toleranceLevel))
			{
				if (IsEqual(vector1, (vector2 * scalar), toleranceLevel))
				{
					return true;
				}
				else if (IsEqual(vector1, (vector2 * (-scalar)), toleranceLevel))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Check Arc segment is collinear.
		/// </summary>
		/// <param name="arcSegment">Arc Segment</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>return true if two vectors are collinear</returns>
		public static bool IsCollinear(IArcSegment3D arcSegment, double toleranceLevel = MathConstants.ZeroGeneral)
		{
			Vector3D vector1 = GeomOperation.Subtract(arcSegment.StartPoint, arcSegment.IntermedPoint);
			Vector3D vector2 = GeomOperation.Subtract(arcSegment.IntermedPoint, arcSegment.EndPoint);

			return IsCollinear(vector1, vector2, toleranceLevel);
		}

		/// <summary>
		/// Returns true if the vectors are coplanar
		/// </summary>
		/// <param name="vector1">vector1 to be compared</param>
		/// <param name="vector2">vector2 to be compared</param>
		/// <param name="vector3">vector3 to be compared</param>
		/// <returns>return true if vectors are coplanar</returns>
		/// <comment> If the determinent value is zero then the vectors are coplanar</comment>
		public static bool IsCoplanar(Vector3D vector1, Vector3D vector2, Vector3D vector3)
		{
			return (vector1 | (vector1 * vector2)).IsZero(MathConstants.ZeroWeak);
		}

		/// <summary>
		/// Determines whether the two vectors are equal or not.
		/// </summary>
		/// <param name="vector1">vector1 to be compared</param>
		/// <param name="vector2">vector2 to be compared</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>returns true if two vectors are equal</returns>
		public static bool IsEqual(Vector3D vector1, Vector3D vector2, double toleranceLevel = MathConstants.ZeroWeak)
		{
			return IsZeroVector(vector1 - vector2, toleranceLevel);
		}

		/// <summary>
		/// Returns true if the given vectors are parallel.
		/// </summary>
		/// <param name="vector1">vector1 to be compared</param>
		/// <param name="vector2">vector2 to be compared</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>Returns true if the given vectors are parallel</returns>
		public static bool IsParallel(Vector3D vector1, Vector3D vector2, double toleranceLevel)
		{
			return (vector1 * vector2).Magnitude.IsZero(toleranceLevel);
		}

		/// <summary>
		/// IsPerpendicularTo - Returns true if the given vectors are perpendicular.
		/// </summary>
		/// <param name="vector1">vector1 to be compared</param>
		/// <param name="vector2">vector2 to be compared</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>Returns true if the given vectors are perpendicular</returns>
		public static bool IsPerpendicular(Vector3D vector1, Vector3D vector2, double toleranceLevel)
		{
			return (vector1 | vector2).IsZero(toleranceLevel);
		}

		/// <summary>
		/// IsZeroVector - check whether a vector is Zero or not.
		/// </summary>
		/// <param name="vector">Input vector</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>returns true if the vetcor is Zero</returns>
		public static bool IsZeroVector(Vector3D vector, double toleranceLevel = MathConstants.ZeroWeak)
		{
			if (vector.Magnitude.IsZero(toleranceLevel))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Multiply - Multiplies the vector with the given value.
		/// </summary>
		/// <param name="vector">vector to be multiplied</param>
		/// <param name="scalar">Value to multiply vector</param>
		/// <returns>return the multiplied vector</returns>
		public static Vector3D Multiply(Vector3D vector, double scalar)
		{
			return vector * scalar;
		}

		/// <summary>
		/// Negate - Returns negation of a given vector.
		/// </summary>
		/// <param name="vector">input vector</param>
		/// <returns>Returns negation of a given vector</returns>
		public static Vector3D Negate(Vector3D vector)
		{
			return -vector;
		}

		/// <summary>
		/// Rotates the vector with reference to given axis in arbitrary axis in anticlockwise direction
		/// </summary>
		/// <param name="vector">input vector</param>
		/// <param name="normalAxis">Rotation axis</param>
		/// <param name="rotationAngle">GetAngle for rotation [in radians]</param>
		/// <returns>return rotated vector along the given axis</returns>
		public static Vector3D Rotate(Vector3D vector, Vector3D normalAxis, double rotationAngle) // rotates vector around NORMALIZED axis
		{
			Vector3D vectorAx = (vector | normalAxis) * normalAxis;
			Vector3D vectorX = vector - vectorAx;
			Vector3D vectorY = normalAxis * vectorX;
			double cosValue = System.Math.Cos(rotationAngle);
			double sineValue = System.Math.Sin(rotationAngle);
			return (vectorX * cosValue) + (vectorY * sineValue) + vectorAx;
		}

		/// <summary>
		/// Rotates the vector with reference to given axis in arbitrary axis in anticlockwise direction
		/// </summary>
		/// <param name="vector">input vector</param>
		/// <param name="normalAxis">Rotation axis</param>
		/// <param name="cosValue">Cosine value of rotation angle</param>
		/// <param name="sineValue">Sine value of rotation angle</param>
		/// <returns>return rotated vector along the given axis</returns>
		public static Vector3D Rotate(Vector3D vector, Vector3D normalAxis, double cosValue, double sineValue)
		{
			Vector3D vectorAx = (vector | normalAxis) * normalAxis;
			Vector3D vectorX = vector - vectorAx;
			Vector3D vectorY = normalAxis * vectorX;
			return (vectorX * cosValue) + (vectorY * sineValue) + vectorAx;
		}

		/// <summary>
		/// Creates a new vector by interpolation of two ones
		/// </summary>
		/// <param name="vector1">vector 1</param>
		/// <param name="vector2">vector  2</param>
		/// <param name="value">interpolated value 0 - 1</param>
		/// <returns>interpolated vector</returns>
		public static Vector3D Interpolate(Vector3D vector1, Vector3D vector2, double value)
		{
			return new Vector3D(vector1.DirectionX + (vector2.DirectionX - vector1.DirectionX) * value,
													vector1.DirectionY + (vector2.DirectionY - vector1.DirectionY) * value,
													vector1.DirectionZ + (vector2.DirectionZ - vector1.DirectionZ) * value);
		}

		/// <summary>
		/// Calculate PlaneX for a given Normal.
		/// </summary>
		/// <param name="normal">Plane Normal</param>
		/// <returns>returns the planeX for a given Normal</returns>
		private static Vector3D GetLocalX(Vector3D normal)
		{
			Vector3D vectorX = new Vector3D();

			double xyd = Math.Pow(normal.DirectionX, 2) + Math.Pow(normal.DirectionY, 2);

			if (xyd.IsLesser(MathConstants.ZeroWeak))
			{
				vectorX.DirectionX = Math.Abs(normal.DirectionZ);
				vectorX.DirectionY = 0;
				double sign = normal.DirectionZ.IsLesser(0) ? -1.0 : 1.0;
				vectorX.DirectionZ = -normal.DirectionX * sign;
				vectorX = vectorX.Normalize;
				return vectorX;
			}

			if (Math.Abs(normal.DirectionY).IsLesser(MathConstants.ZeroWeak))
			{
				double sign = normal.DirectionX.IsLesser(0) ? -1.0 : 1.0;
				vectorX.DirectionX = -normal.DirectionY * sign;
				vectorX.DirectionY = Math.Abs(normal.DirectionX);
				vectorX.DirectionZ = 0.0;
				vectorX = vectorX.Normalize;
				return vectorX;
			}

			double sign0 = normal.DirectionY.IsLesser(0) ? -1.0 : 1.0;
			vectorX.DirectionX = Math.Abs(normal.DirectionY);
			vectorX.DirectionY = -normal.DirectionX * sign0;
			vectorX.DirectionZ = 0.0;
			vectorX = vectorX.Normalize;
			return vectorX;
		}
	}
}