using System;
using System.Diagnostics;
using CI.Mathematics;

namespace CI.Geometry3D
{
	[DebuggerDisplay("[{DirectionX}; {DirectionY}; {DirectionZ}]")]
	public struct Vector3D
	{
		#region Members.

		private double directionX;
		private double directionY;
		private double directionZ;

		#endregion

		#region Constructors.

		public Vector3D(double localX, double localY, double localZ)
		{
			directionX = localX;
			directionY = localY;
			directionZ = localZ;
		}

		public Vector3D(Vector3D anotherVector)
		{
			directionX = anotherVector.DirectionX;
			directionY = anotherVector.DirectionY;
			directionZ = anotherVector.DirectionZ;
		}

		#endregion

		#region Properties.

		public double DirectionX
		{
			get
			{
				return directionX;
			}

			set
			{
				directionX = value;
			}
		}

		public double DirectionY
		{
			get
			{
				return directionY;
			}

			set
			{
				directionY = value;
			}
		}

		public double DirectionZ
		{
			get
			{
				return directionZ;
			}

			set
			{
				directionZ = value;
			}
		}

		public Vector3D Normalize
		{
			get
			{
				var magnitude = Magnitude;
				if (magnitude.IsZero(1e-10))
				{
					return this;
				}

				return this / magnitude;
			}
		}

		public double Magnitude
		{
			get
			{
				return Math.Sqrt((DirectionX * DirectionX) + (DirectionY * DirectionY) + (DirectionZ * DirectionZ));
			}
		}

		public double MagnitudeSquared => (DirectionX * DirectionX) + (DirectionY * DirectionY) + (DirectionZ * DirectionZ);

		#endregion

		#region Operator overloading.

		/// <summary>
		/// Binary operator + overloaded - Additon of 2 vectors
		/// </summary>
		/// <param name="vector1">LHS of +</param>
		/// <param name="vector2">RHS of +</param>
		/// <returns>Vector containing the Sum of LHS and RHS Vector</returns>
		public static Vector3D operator +(Vector3D vector1, Vector3D vector2)
		{
			Vector3D newVector = new Vector3D(vector1.DirectionX + vector2.DirectionX, vector1.DirectionY + vector2.DirectionY, vector1.DirectionZ + vector2.DirectionZ);

			return newVector;
		}

		/// <summary>
		/// Binary operator - overloaded - Subtraction of 2 vectors
		/// </summary>
		/// <param name="vector1">LHS of -</param>
		/// <param name="vector2">RHS of -</param>
		/// <returns>Vector containing the Difference between LHS and RHS Vector</returns>
		public static Vector3D operator -(Vector3D vector1, Vector3D vector2)
		{
			Vector3D newVector = new Vector3D(vector1.DirectionX - vector2.DirectionX, vector1.DirectionY - vector2.DirectionY, vector1.DirectionZ - vector2.DirectionZ);

			return newVector;
		}

		/// <summary>
		/// Binary operator * overloaded - Magnification of a Vector3D by a scalar
		/// </summary>
		/// <param name="vector">Vector3D to be magnified</param>
		/// <param name="scalar">Value to be multiplied</param>
		/// <returns>Magnified Vector3D</returns>
		public static Vector3D operator *(Vector3D vector, double scalar)
		{
			Vector3D v1 = new Vector3D(vector.DirectionX * scalar, vector.DirectionY * scalar, vector.DirectionZ * scalar);
			return v1;
		}

		/// <summary>
		/// Binary operator * overloaded - Magnification of a Vector3D by a scalar
		/// </summary>        
		/// <param name="scalar">Value to be multiplied</param>
		/// <param name="vector">Vector3D to be magnified</param>
		/// <returns>Magnified Vector3D</returns>
		public static Vector3D operator *(double scalar, Vector3D vector)
		{
			return vector * scalar;
		}

		/// <summary>
		/// Unary operator - overloaded - Negation of a vector
		/// </summary>
		/// <param name="vector">Vector to be negated</param>
		/// <returns>Negated Vector</returns>
		public static Vector3D operator -(Vector3D vector)
		{
			Vector3D negationVector = new Vector3D(-vector.DirectionX, -vector.DirectionY, -vector.DirectionZ);

			return negationVector;
		}

		/// <summary>
		/// Division of vector by a scalar
		/// </summary>
		/// <param name="vector">Vector to be divided</param>
		/// <param name="scalar">Value to be divided</param>
		/// <returns>Resuting vector on division</returns>
		public static Vector3D operator /(Vector3D vector, double scalar)
		{
			if (scalar.IsZero(1e-14))
			{
				throw new DivideByZeroException();
			}

			//Vector3D divideVector = new Vector3D(vector);
			//divideVector.DirectionX = vector.DirectionX / scalar;
			//divideVector.DirectionY = vector.DirectionY / scalar;
			//divideVector.DirectionZ = vector.DirectionZ / scalar;

			//return divideVector;

			return new Vector3D(vector.DirectionX / scalar, vector.DirectionY / scalar, vector.DirectionZ / scalar);
		}

		/// <summary>
		/// Cross Product of two Vectors
		/// </summary>
		/// <param name="multiplyVector1">vector1 to be multiplied</param>
		/// <param name="multiplyVector2">vector2 to be multiplied</param>
		/// <returns>returns the cross product of two vector</returns>
		public static Vector3D operator *(Vector3D multiplyVector1, Vector3D multiplyVector2)
		{
			double x = (multiplyVector1.DirectionY * multiplyVector2.DirectionZ) - (multiplyVector1.DirectionZ * multiplyVector2.DirectionY);
			double y = (multiplyVector1.DirectionZ * multiplyVector2.DirectionX) - (multiplyVector1.DirectionX * multiplyVector2.DirectionZ);
			double z = (multiplyVector1.DirectionX * multiplyVector2.DirectionY) - (multiplyVector1.DirectionY * multiplyVector2.DirectionX);
			Vector3D perpVector = new Vector3D(x, y, z);
			return perpVector;
		}

		/// <summary>
		/// Calculate the magnitude for a given vector
		/// </summary>
		/// <param name="magnitudeVector">vector to calculate Magnitude</param>
		/// <returns>returns the magnitude value</returns>
		public static double operator ~(Vector3D magnitudeVector)
		{
			return magnitudeVector.Magnitude;
		}

		/// <summary>
		/// DotProduct of two vectors.
		/// </summary>
		/// <param name="dotVector1">vector1 used to calculate the dotproduct</param>
		/// <param name="dotVector2">vector2 used to calculate the dotproduct</param>
		/// <returns>returns the dot product of two vectors</returns>
		public static double operator |(Vector3D dotVector1, Vector3D dotVector2)
		{
			return (dotVector1.DirectionX * dotVector2.DirectionX) + (dotVector1.DirectionY * dotVector2.DirectionY) + (dotVector1.DirectionZ * dotVector2.DirectionZ);
		}
		#endregion
	}
}