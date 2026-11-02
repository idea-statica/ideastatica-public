using System;
using WM = System.Windows.Media.Media3D;

namespace CI.Geometry3D
{
	/// <summary>
	/// Matrix with fixed number of rows and columns
	/// </summary>
	public class Matrix44 : IMatrix44
	{
		#region MemberVariables.

		/// <summary>
		/// Count of coulumns is 4 always
		/// </summary>
		public const int ColumnCount = 4;

		/// <summary>
		/// Count of rows is 4 always
		/// </summary>
		public const int RowCount = 4;

		/// <summary>
		/// 2D array which stores all values
		/// </summary>
		private double[,] matrixElement = new double[RowCount, ColumnCount];

		#endregion

		#region Constructors.

		/// <summary>
		/// Default constructor
		/// </summary>
		public Matrix44()
		{
			SetToIdentity();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">Source matrix</param>
		public Matrix44(Matrix44 source)
		{
			SetMatrix(source.Origin, source.AxisX, source.AxisY, source.AxisZ);
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">Source matrix</param>
		public Matrix44(IMatrix44 source)
		{
			SetMatrix(source.Origin, source.AxisX, source.AxisY, source.AxisZ);
		}

		/// <summary>
		/// Constructor with origin and 2 perpendicular axis
		/// </summary>
		/// <param name="origin">Origin of coordinate system</param>
		/// <param name="axisX">Local X axis of the coordinate system</param>
		/// <param name="axisY">Local Y axis of the coordinate system</param>
		public Matrix44(IPoint3D origin, Vector3D axisX, Vector3D axisY)
		{
			Vector3D axisZ = (axisX * axisY).Normalize;
			SetMatrix(origin, axisX, axisY, axisZ);
		}

		/// <summary>
		/// Constructor with origin and all axes
		/// </summary>
		/// <param name="origin">Origin of coordinate system</param>
		/// <param name="axisX">Local X axis of the coordinate system</param>
		/// <param name="axisY">Local Y axis of the coordinate system</param>
		/// <param name="axisZ">Local Z axis of the coordinate system</param>
		public Matrix44(IPoint3D origin, Vector3D axisX, Vector3D axisY, Vector3D axisZ)
		{
			SetMatrix(origin, axisX, axisY, axisZ);
		}

		/// <summary>
		/// Constructor with origin and all axes
		/// </summary>
		/// <param name="origin">Origin of coordinate system</param>
		/// <param name="axisX">Local X axis of the coordinate system</param>
		/// <param name="axisY">Local Y axis of the coordinate system</param>
		/// <param name="axisZ">Local Z axis of the coordinate system</param>
		public Matrix44(WM.Point3D origin, Vector3D axisX, Vector3D axisY, Vector3D axisZ)
		{
			SetMatrix(origin, axisX, axisY, axisZ);
		}

		/// <summary>
		/// Constructor - creates matrix which is defined by 3 axes and origin is set at point [0,0,0]
		/// </summary>
		/// <param name="axisX">Local X axis of the coordinate system</param>
		/// <param name="axisY">Local Y axis of the coordinate system</param>
		/// <param name="axisZ">Local Z axis of the coordinate system</param>
		public Matrix44(Vector3D axisX, Vector3D axisY, Vector3D axisZ)
		{
			SetMatrix(axisX, axisY, axisZ);
		}

		/// <summary>
		/// Constructor with 2 perpendicular axis
		/// </summary>
		/// <param name="axisX">Local X axis of the coordinate system</param>
		/// <param name="axisY">Local Y axis of the coordinate system</param>
		public Matrix44(Vector3D axisX, Vector3D axisY)
		{
			Vector3D axisZ = (axisX * axisY).Normalize;
			SetMatrix(axisX, axisY, axisZ);
		}

		/// <summary>
		/// Creates identity matrix which is moved to given point
		/// </summary>
		/// <param name="origin">Origin of coordinate system</param>
		public Matrix44(IPoint3D origin)
		{
			SetToIdentity();
			SetToTranslation(new Vector3D(origin.X, origin.Y, origin.Z));
		}

		#endregion

		#region Properties.

		/// <summary>
		/// Gets metrix elemnts
		/// </summary>
		public double[,] MatrixElements
		{
			get { return matrixElement; }
		}

		/// <summary>
		/// Local X axis of the coordinate system
		/// </summary>
		public Vector3D AxisX
		{
			get
			{
				return new Vector3D(matrixElement[0, 0], matrixElement[0, 1], matrixElement[0, 2]);
			}

			set
			{
				matrixElement[0, 0] = value.DirectionX;
				matrixElement[0, 1] = value.DirectionY;
				matrixElement[0, 2] = value.DirectionZ;
			}
		}

		/// <summary>
		/// Local Y axis of the coordinate system
		/// </summary>
		public Vector3D AxisY
		{
			get
			{
				return new Vector3D(matrixElement[1, 0], matrixElement[1, 1], matrixElement[1, 2]);
			}

			set
			{
				matrixElement[1, 0] = value.DirectionX;
				matrixElement[1, 1] = value.DirectionY;
				matrixElement[1, 2] = value.DirectionZ;
			}
		}

		/// <summary>
		/// Local Z axis of the coordinate system
		/// </summary>
		public Vector3D AxisZ
		{
			get
			{
				return new Vector3D(matrixElement[2, 0], matrixElement[2, 1], matrixElement[2, 2]);
			}

			set
			{
				matrixElement[2, 0] = value.DirectionX;
				matrixElement[2, 1] = value.DirectionY;
				matrixElement[2, 2] = value.DirectionZ;
			}
		}

		/// <summary>
		/// Origin of the coordinate system
		/// </summary>
		public IPoint3D Origin
		{
			get
			{
				return new Point3D(matrixElement[0, 3], matrixElement[1, 3], matrixElement[2, 3]);
			}

			set
			{
				matrixElement[0, 3] = value.X;
				matrixElement[1, 3] = value.Y;
				matrixElement[2, 3] = value.Z;
			}
		}

		#endregion

		#region Methods.

		#region Public static methods.

		/// <summary>
		/// The function returns the adjoint of a given matrix
		/// </summary>
		/// <param name="matrix">Original matrix</param>
		/// <returns>Adjoint matrix</returns>
		public static Matrix44 AdJoint(Matrix44 matrix)
		{
			Matrix44 adjoint = new Matrix44();
			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					adjoint.matrixElement[i, j] = Math.Pow(-1, i + j) * Determinant(Minor(matrix, i, j));
				}
			}

			adjoint = Transpose(adjoint);
			return adjoint;
		}

		/// <summary>
		/// Returns the determinent of the current matrix.
		/// </summary>
		/// <param name="matrix">Original matrix</param>
		/// <param name="order">Order upto which the row/column of matrix is considered</param>
		/// <returns>Determinent of the current matrix</returns>
		public static double Determinant(Matrix44 matrix, int order = 3)
		{
			double determinant = 0.0;
			if (order == 1)
			{
				return matrix.matrixElement[0, 0];
			}

			int sign = 1;
			for (int j = 0; j < order; j++)
			{
				determinant += matrix.matrixElement[0, j] * Determinant(Minor(matrix, 0, j), order - 1) * sign;
				sign *= -1;
			}

			return determinant;
		}

		/// <summary>
		/// The function returns the inverse of axes of given matrix
		/// </summary>
		/// <param name="matrix">Original matrix</param>
		/// <returns>Inversed matrix</returns>
		public static Matrix44 Inverse33(Matrix44 matrix)
		{
			IPoint3D origin = matrix.Origin;
			matrix.Origin = new Point3D();
			matrix = Inverse(matrix);
			origin.X *= -1;
			origin.Y *= -1;
			origin.Z *= -1;
			matrix.Origin = origin;
			return matrix;
		}

		/// <summary>
		/// The function returns the inverse of a given matrix
		/// </summary>
		/// <param name="matrix">Original matrix</param>
		/// <returns>Inversed matrix</returns>
		public static Matrix44 Inverse(Matrix44 matrix)
		{
			double determinant = Determinant(matrix, 4);
			if (determinant.IsZero())
			{
				throw new NotSupportedException("Inverse of singular matrix is not possible");
			}

			return Multiply(AdJoint(matrix), 1 / determinant);
		}

		/// <summary>
		/// The function return the Minor of element[Row,Col] of a Matrix object 
		/// </summary>
		/// <param name="matrix">Original matrix</param>
		/// <param name="row">Base row index to find minor</param>
		/// <param name="col">Base column index to find minor</param>
		/// <returns>Minor matrix</returns>
		public static Matrix44 Minor(Matrix44 matrix, int row, int col)
		{
			Matrix44 minor = new Matrix44();
			minor.SetToIdentity();

			int m = 0, n = 0;
			for (int i = 0; i < RowCount; i++)
			{
				if (i == row)
				{
					continue;
				}

				n = 0;
				for (int j = 0; j < ColumnCount; j++)
				{
					if (j == col)
					{
						continue;
					}

					minor.matrixElement[m, n] = matrix.matrixElement[i, j];
					n++;
				}

				m++;
			}

			return minor;
		}

		/// <summary>
		/// Multiply - Returns the product of two matrix.
		/// </summary>
		/// <param name="multiplyMatrix1">Matrix1 to be multiplied</param>
		/// <param name="multiplyMatrix2">Matrix2 to be multiplied</param>
		/// <returns>Returns the product of two matrix</returns>
		public static Matrix44 Multiply(Matrix44 multiplyMatrix1, Matrix44 multiplyMatrix2)
		{
			Matrix44 result = Matrix44.NullMatrix();
			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					for (int k = 0; k < ColumnCount; k++)
					{
						result.matrixElement[i, j] += multiplyMatrix1.matrixElement[i, k] * multiplyMatrix2.matrixElement[k, j];
					}
				}
			}

			return result;
		}

		/// <summary>
		/// NullMatrix - returns a Null Matrix of dimension ( Row x Col )
		/// </summary>
		/// <returns>return Null Matrix</returns>
		public static Matrix44 NullMatrix()
		{
			double zero = 0.0;
			Matrix44 matrix = new Matrix44();
			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					matrix.matrixElement[i, j] = zero;
				}
			}

			return matrix;
		}

		/// <summary>
		/// The function returns the transpose of a given matrix
		/// </summary>
		/// <param name="matrix">Original matrix</param>
		/// <returns>Transposed matrix</returns>
		public static Matrix44 Transpose(Matrix44 matrix)
		{
			Matrix44 transposed = new Matrix44();
			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					transposed.matrixElement[i, j] = matrix.matrixElement[j, i];
				}
			}

			return transposed;
		}

		/// <summary>
		/// Returns the multiplication result of the matrices.
		/// </summary>
		/// <param name="multiplyMatrix1">Matrix1 to be multiplied</param>
		/// <param name="multiplyMatrix2">Matrix2 to be multiplied</param>
		/// <returns>Returns the multiplication result of the matrices</returns>
		public static Matrix44 operator *(Matrix44 multiplyMatrix1, Matrix44 multiplyMatrix2)
		{
			return Matrix44.Multiply(multiplyMatrix1, multiplyMatrix2);
		}

		/// <summary>
		/// Returns the multiplication result of the matrix and the vector.
		/// </summary>
		/// <param name="multiplyMatrix">Matrix to be multiplied</param>
		/// <param name="multiplyVector">Vector bo be multiplied</param>
		/// <returns>Returns the multiplication result of the matrix and the vector</returns>
		public static Vector3D operator *(Matrix44 multiplyMatrix, Vector3D multiplyVector)
		{
			return Matrix44.Multiply(multiplyMatrix, multiplyVector);
		}

		/// <summary>
		/// Returns the multiplication result of the double value and the matrix.
		/// </summary>
		/// <param name="scaleValue">Value to be multiplied</param>
		/// <param name="multiplyMatrix">Matrix to be multiplied</param>
		/// <returns>Returns the multiplication result of the double value and the matrix</returns>
		public static Matrix44 operator *(double scaleValue, Matrix44 multiplyMatrix)
		{
			return Matrix44.Multiply(multiplyMatrix, scaleValue);
		}

		/// <summary>
		/// Returns equation result
		/// </summary>
		/// <param name="matrix">Matrix</param>
		/// <param name="tolerance">Matrix</param>
		/// <returns>Return true if the data are equlas</returns>
		public bool EqualsTo(IMatrix44 matrix, double tolerance = 1e-10)
		{
			if (matrix == null)
			{
				return false;
			}

			var matrix44 = matrix as Matrix44;
			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					if (!this.matrixElement[i, j].IsEqual(matrix44.matrixElement[i, j], tolerance))
					{
						return false;
					}
				}
			}

			return true;
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Determinant - Returns the determinent of the current matrix.
		/// </summary>
		/// <returns>Returns the determinent of the current matrix</returns>
		public double Determinant33()
		{
			return Determinant(this, 3);
		}

		/// <summary>
		/// Concatenates the rotation matrix about an axis to this matrix
		/// </summary>
		/// <param name="rotationAngle">The angle of rotation [Angle in radians]</param>
		/// <param name="rotationAxis">The orientation of the axis to rotate around</param>
		public void Rotate(double rotationAngle, Axis rotationAxis)
		{
			if (rotationAngle.IsZero() == false)
			{
				Matrix44 rotationMatrix = new Matrix44();
				rotationMatrix.SetToRotation(rotationAngle, rotationAxis);
				Transform(rotationMatrix);
			}
		}

		/// <summary>
		/// Concatenates a rotation matrix which rotates a vector around another vector to this matrix.
		/// </summary>
		/// <param name="rotationAngle">The angle of rotation in degrees</param>
		/// <param name="normalVector">The orientation of the axis to rotate around</param>
		public void Rotate(double rotationAngle, Vector3D normalVector)
		{
			if (rotationAngle.IsZero() == false)
			{
				double cosValue = System.Math.Cos(rotationAngle);
				double sineValue = System.Math.Sin(rotationAngle);
				AxisX = GeomOperation.Rotate(AxisX, normalVector, cosValue, sineValue);
				AxisY = GeomOperation.Rotate(AxisY, normalVector, cosValue, sineValue);
				AxisZ = GeomOperation.Rotate(AxisZ, normalVector, cosValue, sineValue);
			}
		}

		/// <summary>
		/// Scale - Scale a matrix for a given vector.
		/// </summary>
		/// <param name="scaleVector">vector to scale the matrix</param>
		public void Scale(Vector3D scaleVector)
		{
			Matrix44 scalingMatrix = new Matrix44();
			scalingMatrix.SetToScaling(scaleVector);
			Transform(scalingMatrix);
		}

		/// <summary>
		/// SetToIdentity - Resets the matrix to identity matrix
		/// </summary>
		public void SetToIdentity()
		{
			double zero = 0.0;
			double one = 1.0;

			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					if (i == j)
					{
						this.matrixElement[i, j] = one;
					}
					else
					{
						this.matrixElement[i, j] = zero;
					}
				}
			}
		}

		/// <summary>
		/// Is identity
		/// </summary>
		/// <param name="exceptOrigin">Exclude check origin point</param>
		/// <returns>True if identity</returns>
		public bool IsIdentity(bool exceptOrigin = true)
		{
			double zero = 0.0;
			double one = 1.0;

			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					if (i == j)
					{
						if (!this.matrixElement[i, j].IsEqual(one))
						{
							return false;
						}
					}
					else
					{
						if ((exceptOrigin) && (j == (ColumnCount - 1)))
						{
							continue;
						}

						if (!this.matrixElement[i, j].IsEqual(zero))
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// SetToRotation - Resets the matrix to a rotation matrix based on axis and angle
		/// </summary>
		/// <param name="rotationAngle">Angle to be used for rotation [Angle in radians]</param>
		/// <param name="rotationAxis">Axis to be used for rotation</param>
		public void SetToRotation(double rotationAngle, Axis rotationAxis)
		{
			SetToIdentity();

			double angleCos = Math.Cos(rotationAngle);
			double angleSin = Math.Sin(rotationAngle);

			switch (rotationAxis)
			{
				case Axis.XAxis:
					this.matrixElement[1, 1] = angleCos;
					this.matrixElement[1, 2] = angleSin;
					this.matrixElement[2, 1] = -angleSin;
					this.matrixElement[2, 2] = angleCos;
					break;

				case Axis.YAxis:
					this.matrixElement[0, 0] = angleCos;
					this.matrixElement[0, 2] = -angleSin;
					this.matrixElement[2, 0] = angleSin;
					this.matrixElement[2, 2] = angleCos;
					break;

				case Axis.ZAxis:
					this.matrixElement[0, 0] = angleCos;
					this.matrixElement[1, 0] = -angleSin;
					this.matrixElement[0, 1] = angleSin;
					this.matrixElement[1, 1] = angleCos;
					break;
			}
		}

		/// <summary>
		/// SetToRotation - Resets this matrix to a rotation matrix.  The resulting matrix
		/// will rotate a arcPoint counter-clockwise around the specified axis
		/// by the specified angle.
		/// </summary>
		/// <param name="angle">The angle of rotation in radians.</param>
		/// <param name="axisVector">The orientation of the axis to rotate around.</param>
		/// <param name="rotationPoint">rotationPoint on the axis of rotation.</param>
		public void SetToRotation(double angle, Vector3D axisVector, Point3D rotationPoint)
		{
			// translate the rotation center back to the origin
			SetToTranslation(-GeomOperation.ToVector(rotationPoint));

			// rotate about the axis at the origin
			Vector3D normalAxisVect = axisVector.Normalize;

			double angle2 = -0.5 * angle;

			double x = Math.Sin(angle2) * normalAxisVect.DirectionX;
			double y = Math.Sin(angle2) * normalAxisVect.DirectionY;
			double z = Math.Sin(angle2) * normalAxisVect.DirectionZ;
			double w = Math.Cos(angle2);

			double twoX = 2.0 * x;
			double twoY = 2.0 * y;
			double twoZ = 2.0 * z;

			double threeX = x * twoX;
			double threeY = y * twoY;
			double threeZ = z * twoZ;

			double wtwoX = w * twoX;
			double wtwoY = w * twoY;
			double wtwoZ = w * twoZ;

			double xtwoY = x * twoY;
			double xtwoZ = x * twoZ;

			double ytwoZ = y * twoZ;

			Matrix44 tempMat = new Matrix44();

			tempMat.matrixElement[0, 0] = 1.0 - (threeY + threeZ);
			tempMat.matrixElement[0, 1] = xtwoY - wtwoZ;
			tempMat.matrixElement[0, 2] = xtwoZ + wtwoY;
			tempMat.matrixElement[0, 3] = 0.0;

			tempMat.matrixElement[1, 0] = xtwoY + wtwoZ;
			tempMat.matrixElement[1, 1] = 1.0 - (threeX + threeZ);
			tempMat.matrixElement[1, 2] = ytwoZ - wtwoX;
			tempMat.matrixElement[1, 3] = 0.0;

			tempMat.matrixElement[2, 0] = xtwoZ - wtwoY;
			tempMat.matrixElement[2, 1] = ytwoZ + wtwoX;
			tempMat.matrixElement[2, 2] = 1.0 - (threeX + threeY);
			tempMat.matrixElement[2, 3] = 0.0;

			tempMat.matrixElement[3, 0] = 0.0;
			tempMat.matrixElement[3, 1] = 0.0;
			tempMat.matrixElement[3, 2] = 0.0;
			tempMat.matrixElement[3, 3] = 1.0;

			Matrix44 m1 = tempMat * this;
			this.Copy(m1);

			// translate back to the center
			tempMat.SetToTranslation(GeomOperation.ToVector(rotationPoint));

			Matrix44 m2 = tempMat * this;
			this.Copy(m2);
		}

		/// <summary>
		/// SetToScaling - Scale this matrix for a given vector.
		/// </summary>
		/// <param name="scaleVector">Scaling Vector</param>
		public void SetToScaling(Vector3D scaleVector)
		{
			SetToIdentity();
			this.matrixElement[0, 0] = scaleVector.DirectionX;
			this.matrixElement[1, 1] = scaleVector.DirectionY;
			this.matrixElement[2, 2] = scaleVector.DirectionZ;
		}

		/// <summary>
		/// SetToTranslation - Set a translation of a matrix along a given vector.
		/// </summary>
		/// <param name="transVector">Translate the matrix along the given vector</param>
		public void SetToTranslation(Vector3D transVector)
		{
			SetToIdentity();
			this.matrixElement[0, 3] = transVector.DirectionX;
			this.matrixElement[1, 3] = transVector.DirectionY;
			this.matrixElement[2, 3] = transVector.DirectionZ;
		}

		/// <summary>
		/// Tranforms the vector by the current matrix
		/// </summary>
		/// <param name="transformedVector">Vector to tranform the Matrix</param>
		public void Transform(ref Vector3D transformedVector)
		{
			Vector3D temp = this * transformedVector;
			transformedVector.DirectionX = temp.DirectionX;
			transformedVector.DirectionY = temp.DirectionY;
			transformedVector.DirectionZ = temp.DirectionZ;
		}

		/// <summary>
		/// Transform - trnsform given matrix
		/// </summary>
		/// <param name="transformMatrix">matrix to be transformed</param>
		public void Transform(Matrix44 transformMatrix)
		{
			this.Copy(this * transformMatrix);
		}

		/// <summary>
		/// Transforms a Point from LCS to GCS
		/// </summary>
		/// <param name="point">Point to be tranformed [In LCS].</param>
		/// <returns>Point in GCS</returns>
		public IPoint3D TransformToGCS(IPoint3D point)
		{
			return GeomOperation.Add(Multiply(point, this), this.Origin);
		}

		/// <summary>
		/// Transforms a Point from LCS to GCS
		/// </summary>
		/// <param name="point">Point to be tranformed [In LCS].</param>
		/// <returns>Point in GCS</returns>
		public WM.Point3D TransformToGCS(WM.Point3D point)
		{
			return GeomOperation.Add(Multiply(point, this), this.Origin);
		}

		/// <summary>
		/// Transforms a Point from GCS to LCS
		/// </summary>
		/// <param name="point">Point to be tranformed [In GCS].</param>
		/// <returns>Point in LCS</returns>
		public IPoint3D TransformToLCS(IPoint3D point)
		{
			return GeomOperation.ToPoint(Multiply(this, GeomOperation.Subtract(point, this.Origin)));
		}

		/// <summary>
		/// Transforms a vector from LCS to GCS
		/// </summary>
		/// <param name="vector">Point to be tranformed [In LCS].</param>
		/// <returns>Vector in GCS</returns>
		public Vector3D TransformToGCS(Vector3D vector)
		{
			return Multiply(vector, this);
		}

		/// <summary>
		/// Transforms a vector from LCS to GCS
		/// </summary>
		/// <param name="vector">Point to be tranformed [In LCS].</param>
		/// <returns>Vector in GCS</returns>
		public WM.Vector3D TransformToGCS(WM.Vector3D vector)
		{
			return Multiply(vector, this);
		}

		/// <summary>
		/// Transforms a Plane3D from LCS to GCS
		/// </summary>
		/// <param name="pln">Plane to be tranformed [In LCS].</param>
		/// <returns>Plane in GCS</returns>
		public Plane3D TransformToGCS(Plane3D pln)
		{
			return new Plane3D(TransformToGCS(pln.PointOnPlane), TransformToGCS(pln.NormalVector));
		}

		/// <summary>
		/// Transforms a vector from GCS to LCS
		/// </summary>
		/// <param name="vector">Point to be tranformed [In GCS].</param>
		/// <returns>Vector in LCS</returns>
		public Vector3D TransformToLCS(Vector3D vector)
		{
			return Multiply(this, vector);
		}

		/// <summary>
		/// Transforms a Point from GCS to LCS
		/// </summary>
		/// <param name="point">Point to be tranformed [In GCS].</param>
		/// <returns>Point in LCS</returns>
		public WM.Point3D TransformToLCS(WM.Point3D point)
		{
			return (WM.Point3D)Multiply(this, GeomOperation.SubWM(point, this.Origin));
		}

		/// <summary>
		/// Transforms a <paramref name="vector"/> from GCS to LCS
		/// </summary>
		/// <param name="vector">Vector to be tranformed [In GCS].</param>
		/// <returns>Vector in LCS</returns>
		public WM.Vector3D TransformToLCS(WM.Vector3D vector)
		{
			return Multiply(this, vector);
		}

		/// <summary>
		/// Transforms a Plane3D from GCS to LCS
		/// </summary>
		/// <param name="pln">Plane to be tranformed [In GCS].</param>
		/// <returns>Plane in LCS</returns>
		public Plane3D TransformToLCS(Plane3D pln)
		{
			return new Plane3D(TransformToLCS(pln.PointOnPlane), TransformToLCS(pln.NormalVector));
		}

		/// <summary>
		/// Translates the matrix along the given vector.
		/// </summary>
		/// <param name="translatedVector">Translation Vector</param>
		public void Translate(ref Vector3D translatedVector)
		{
			Matrix44 translationMatrix = new Matrix44();
			translationMatrix.SetToTranslation(translatedVector);
			Transform(translationMatrix);
		}

		/// <summary>
		/// Gets the rotation from matrix 
		/// </summary>
		/// <param name="matrix">Rotation from matrix</param>
		/// <param name="axis">Around axis</param>
		/// <returns>Angle of rotation (0 - 2PI)</returns>
		public double GetRotation(IMatrix44 matrix, Axis axis)
		{
			double rot = 0.0;
			double y = 0.0;

			switch (axis)
			{
				case Axis.XAxis:
					rot = GeomOperation.GetAngle(matrix.AxisZ, AxisZ);
					y = AxisZ | (matrix.AxisX * matrix.AxisZ).Normalize;
					break;
				case Axis.YAxis:
					rot = GeomOperation.GetAngle(matrix.AxisX, AxisX);
					y = AxisX | (matrix.AxisY * matrix.AxisX).Normalize;
					break;
				case Axis.ZAxis:
					rot = GeomOperation.GetAngle(matrix.AxisY, AxisY);
					y = AxisY | (matrix.AxisZ * matrix.AxisY).Normalize;
					break;
			}

			if (y.IsLesser(0))
			{
				rot = 2 * Math.PI - rot;
			}

			return rot;
		}

		/// <summary>
		/// Initialize matrix with origin and 2 perpendicular axis
		/// </summary>
		/// <param name="origin">Origin of coordinate system</param>
		/// <param name="axisX">Local X axis of the coordinate system</param>
		/// <param name="axisY">Local Y axis of the coordinate system</param>
		public void Set(IPoint3D origin, Vector3D axisX, Vector3D axisY)
		{
			Vector3D axisZ = (axisX * axisY).Normalize;
			SetMatrix(origin, axisX, axisY, axisZ);
		}

		#endregion

		#region Private static methods

		/// <summary>
		/// Multiply - Returns the vector multiplied with the matrix
		/// </summary>
		/// <param name="matrix">Matrix to be multiplied</param>
		/// <param name="vector">Vector to be multiplied</param>
		/// <returns>Returns the vector multiplied with the matrix</returns>
		internal static Vector3D Multiply(Matrix44 matrix, Vector3D vector)
		{
			Vector3D resVect = new Vector3D
			{
				DirectionX = (matrix.matrixElement[0, 0] * vector.DirectionX) + (matrix.matrixElement[0, 1] * vector.DirectionY) + (matrix.matrixElement[0, 2] * vector.DirectionZ),
				DirectionY = (matrix.matrixElement[1, 0] * vector.DirectionX) + (matrix.matrixElement[1, 1] * vector.DirectionY) + (matrix.matrixElement[1, 2] * vector.DirectionZ),
				DirectionZ = (matrix.matrixElement[2, 0] * vector.DirectionX) + (matrix.matrixElement[2, 1] * vector.DirectionY) + (matrix.matrixElement[2, 2] * vector.DirectionZ)
			};
			return resVect;
		}

		/// <summary>
		/// Multiply - Returns the point multiplied with the matrix
		/// </summary>
		/// <param name="matrix">Matrix to be multiplied</param>
		/// <param name="point">Vector to be multiplied</param>
		/// <returns>Returns the point multiplied with the matrix</returns>
		internal static IPoint3D Multiply(Matrix44 matrix, IPoint3D point)
		{
			IPoint3D resVect = new Point3D
			{
				X = (matrix.matrixElement[0, 0] * point.X) + (matrix.matrixElement[0, 1] * point.Y) + (matrix.matrixElement[0, 2] * point.Z),
				Y = (matrix.matrixElement[1, 0] * point.X) + (matrix.matrixElement[1, 1] * point.Y) + (matrix.matrixElement[1, 2] * point.Z),
				Z = (matrix.matrixElement[2, 0] * point.X) + (matrix.matrixElement[2, 1] * point.Y) + (matrix.matrixElement[2, 2] * point.Z)
			};
			return resVect;
		}

		/// <summary>
		/// Multiply - Returns the vector multiplied with the matrix
		/// </summary>
		/// <param name="vector">Vector to be multiplied</param>
		/// <param name="matrix">Matrix to be multiplied</param>
		/// <returns>Returns the vector multiplied with the matrix</returns>
		internal static Vector3D Multiply(Vector3D vector, Matrix44 matrix)
		{
			Vector3D newPoint = new Vector3D
			{
				DirectionX = (vector.DirectionX * matrix.matrixElement[0, 0]) + (vector.DirectionY * matrix.matrixElement[1, 0]) + (vector.DirectionZ * matrix.matrixElement[2, 0]),
				DirectionY = (vector.DirectionX * matrix.matrixElement[0, 1]) + (vector.DirectionY * matrix.matrixElement[1, 1]) + (vector.DirectionZ * matrix.matrixElement[2, 1]),
				DirectionZ = (vector.DirectionX * matrix.matrixElement[0, 2]) + (vector.DirectionY * matrix.matrixElement[1, 2]) + (vector.DirectionZ * matrix.matrixElement[2, 2])
			};
			return newPoint;
		}

		internal static WM.Vector3D Multiply(WM.Vector3D vector, Matrix44 matrix)
		{
			WM.Vector3D newPoint = new WM.Vector3D
			{
				X = (vector.X * matrix.matrixElement[0, 0]) + (vector.Y * matrix.matrixElement[1, 0]) + (vector.Z * matrix.matrixElement[2, 0]),
				Y = (vector.X * matrix.matrixElement[0, 1]) + (vector.Y * matrix.matrixElement[1, 1]) + (vector.Z * matrix.matrixElement[2, 1]),
				Z = (vector.X * matrix.matrixElement[0, 2]) + (vector.Y * matrix.matrixElement[1, 2]) + (vector.Z * matrix.matrixElement[2, 2])
			};
			return newPoint;
		}

		/// <summary>
		/// Multiply - Returns the point multiplied with the matrix
		/// </summary>
		/// <param name="point">Vector to be multiplied</param>
		/// <param name="matrix">Matrix to be multiplied</param>
		/// <returns>Returns the point multiplied with the matrix</returns>
		internal static IPoint3D Multiply(IPoint3D point, Matrix44 matrix)
		{
			IPoint3D newPoint = new Point3D
			{
				X = (point.X * matrix.matrixElement[0, 0]) + (point.Y * matrix.matrixElement[1, 0]) + (point.Z * matrix.matrixElement[2, 0]),
				Y = (point.X * matrix.matrixElement[0, 1]) + (point.Y * matrix.matrixElement[1, 1]) + (point.Z * matrix.matrixElement[2, 1]),
				Z = (point.X * matrix.matrixElement[0, 2]) + (point.Y * matrix.matrixElement[1, 2]) + (point.Z * matrix.matrixElement[2, 2])
			};
			return newPoint;
		}

		internal static WM.Point3D Multiply(WM.Point3D point, Matrix44 matrix)
		{
			WM.Point3D newPoint = new WM.Point3D
			{
				X = (point.X * matrix.matrixElement[0, 0]) + (point.Y * matrix.matrixElement[1, 0]) + (point.Z * matrix.matrixElement[2, 0]),
				Y = (point.X * matrix.matrixElement[0, 1]) + (point.Y * matrix.matrixElement[1, 1]) + (point.Z * matrix.matrixElement[2, 1]),
				Z = (point.X * matrix.matrixElement[0, 2]) + (point.Y * matrix.matrixElement[1, 2]) + (point.Z * matrix.matrixElement[2, 2])
			};
			return newPoint;
		}

		internal static WM.Vector3D Multiply(Matrix44 matrix, WM.Vector3D vector)
		{
			WM.Vector3D resVect = new WM.Vector3D
			{
				X = (matrix.matrixElement[0, 0] * vector.X) + (matrix.matrixElement[0, 1] * vector.Y) + (matrix.matrixElement[0, 2] * vector.Z),
				Y = (matrix.matrixElement[1, 0] * vector.X) + (matrix.matrixElement[1, 1] * vector.Y) + (matrix.matrixElement[1, 2] * vector.Z),
				Z = (matrix.matrixElement[2, 0] * vector.X) + (matrix.matrixElement[2, 1] * vector.Y) + (matrix.matrixElement[2, 2] * vector.Z)
			};
			return resVect;
		}

		/// <summary>
		/// Multiply - Returns the matrix multiplied with the integer
		/// </summary>
		/// <param name="multiplyMatrix">Matrix to be scaled</param>
		/// <param name="scaleValue">integer Value to scale Matrix</param>
		/// <returns>Returns the matrix multiplied with the integer</returns>
		internal static Matrix44 Multiply(Matrix44 multiplyMatrix, int scaleValue)
		{
			Matrix44 result = new Matrix44();
			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					result.matrixElement[i, j] = multiplyMatrix.matrixElement[i, j] * scaleValue;
				}
			}

			return result;
		}

		/// <summary>
		/// Multiply - Returns the matrix multiplied with the double
		/// </summary>
		/// <param name="multiplyMatrix">Matrix to be scaled</param>
		/// <param name="scaleValue">double Value to scale Matrix</param>
		/// <returns>Returns the matrix multiplied with the double</returns>
		public static Matrix44 Multiply(Matrix44 multiplyMatrix, double scaleValue)
		{
			Matrix44 result = new Matrix44();
			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					result.matrixElement[i, j] = multiplyMatrix.matrixElement[i, j] * scaleValue;
				}
			}

			return result;
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Copy - Copies the given matrix to the current matrix 
		/// </summary>
		/// <param name="copyMatrix">Matrix to be copied</param>
		public void Copy(Matrix44 copyMatrix)
		{
			for (int i = 0; i < RowCount; i++)
			{
				for (int j = 0; j < ColumnCount; j++)
				{
					this.matrixElement[i, j] = copyMatrix.matrixElement[i, j];
				}
			}
		}

		/// <summary>
		/// Set origin and axes
		/// </summary>
		/// <param name="origin">Origin of coordinate system</param>
		/// <param name="axisX">Local X axis of the coordinate system</param>
		/// <param name="axisY">Local Y axis of the coordinate system</param>
		/// <param name="axisZ">Local Z axis of the coordinate system</param>
		private void SetMatrix(IPoint3D origin, Vector3D axisX, Vector3D axisY, Vector3D axisZ)
		{
			matrixElement[0, 0] = axisX.DirectionX;
			matrixElement[0, 1] = axisX.DirectionY;
			matrixElement[0, 2] = axisX.DirectionZ;
			matrixElement[0, 3] = origin.X;

			matrixElement[1, 0] = axisY.DirectionX;
			matrixElement[1, 1] = axisY.DirectionY;
			matrixElement[1, 2] = axisY.DirectionZ;
			matrixElement[1, 3] = origin.Y;

			matrixElement[2, 0] = axisZ.DirectionX;
			matrixElement[2, 1] = axisZ.DirectionY;
			matrixElement[2, 2] = axisZ.DirectionZ;
			matrixElement[2, 3] = origin.Z;

			matrixElement[3, 0] = 0;
			matrixElement[3, 1] = 0;
			matrixElement[3, 2] = 0;
			matrixElement[3, 3] = 1;
		}

		/// <summary>
		/// Set origin and axes
		/// </summary>
		/// <param name="origin">Origin of coordinate system</param>
		/// <param name="axisX">Local X axis of the coordinate system</param>
		/// <param name="axisY">Local Y axis of the coordinate system</param>
		/// <param name="axisZ">Local Z axis of the coordinate system</param>
		private void SetMatrix(WM.Point3D origin, Vector3D axisX, Vector3D axisY, Vector3D axisZ)
		{
			matrixElement[0, 0] = axisX.DirectionX;
			matrixElement[0, 1] = axisX.DirectionY;
			matrixElement[0, 2] = axisX.DirectionZ;
			matrixElement[0, 3] = origin.X;

			matrixElement[1, 0] = axisY.DirectionX;
			matrixElement[1, 1] = axisY.DirectionY;
			matrixElement[1, 2] = axisY.DirectionZ;
			matrixElement[1, 3] = origin.Y;

			matrixElement[2, 0] = axisZ.DirectionX;
			matrixElement[2, 1] = axisZ.DirectionY;
			matrixElement[2, 2] = axisZ.DirectionZ;
			matrixElement[2, 3] = origin.Z;

			matrixElement[3, 0] = 0;
			matrixElement[3, 1] = 0;
			matrixElement[3, 2] = 0;
			matrixElement[3, 3] = 1;
		}

		/// <summary>
		/// Set sets matrix which is defined by 3 vectors and origin is at point [0,0,0]
		/// </summary>
		/// <param name="axisX">Local X axis of the coordinate system</param>
		/// <param name="axisY">Local Y axis of the coordinate system</param>
		/// <param name="axisZ">Local Z axis of the coordinate system</param>
		private void SetMatrix(Vector3D axisX, Vector3D axisY, Vector3D axisZ)
		{
			matrixElement[0, 0] = axisX.DirectionX;
			matrixElement[0, 1] = axisX.DirectionY;
			matrixElement[0, 2] = axisX.DirectionZ;
			matrixElement[0, 3] = 0;

			matrixElement[1, 0] = axisY.DirectionX;
			matrixElement[1, 1] = axisY.DirectionY;
			matrixElement[1, 2] = axisY.DirectionZ;
			matrixElement[1, 3] = 0;

			matrixElement[2, 0] = axisZ.DirectionX;
			matrixElement[2, 1] = axisZ.DirectionY;
			matrixElement[2, 2] = axisZ.DirectionZ;
			matrixElement[2, 3] = 0;

			matrixElement[3, 0] = 0;
			matrixElement[3, 1] = 0;
			matrixElement[3, 2] = 0;
			matrixElement[3, 3] = 1;
		}

		#endregion

		#endregion
	}
}