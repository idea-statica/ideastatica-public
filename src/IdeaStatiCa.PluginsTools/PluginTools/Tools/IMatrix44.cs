using WM = System.Windows.Media.Media3D;

namespace CI.Geometry3D
{
	/// <summary>
	/// Three basic axis of coordinate system
	/// </summary>
	public enum Axis : int
	{
		/// <summary>
		/// X axis - Vector along axis - X
		/// </summary>
		XAxis,

		/// <summary>
		/// Y axis - Vector along axis - Y
		/// </summary>
		YAxis,

		/// <summary>
		/// Z axis - Vector along axis - Z
		/// </summary>
		ZAxis
	}

	public interface IMatrix44
	{
		#region Properties

		/// <summary>
		/// Origin
		/// </summary>
		IPoint3D Origin
		{
			get;
			set;
		}

		/// <summary>
		/// Local X axis of the coordinate system
		/// </summary>
		Vector3D AxisX { get; }

		/// <summary>
		/// Local Y axis of the coordinate system
		/// </summary>
		Vector3D AxisY { get; }

		/// <summary>
		/// Local Z axis of the coordinate system
		/// </summary>
		Vector3D AxisZ { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Determinant 3*3
		/// </summary>
		/// <returns>Determinant value</returns>
		double Determinant33();

		/// <summary>
		/// Set Matrix to Identity
		/// </summary>
		void SetToIdentity();

		/// <summary>
		/// Is identity
		/// </summary>
		/// <param name="exceptOrigin">Exclude check origin point</param>
		/// <returns>True if identity</returns>
		bool IsIdentity(bool exceptOrigin = true);

		/// <summary>
		/// Transforms a Point from GCS to LCS
		/// </summary>
		/// <param name="point">Point to be tranformed [In GCS].</param>
		/// <returns>Point in LCS</returns>
		IPoint3D TransformToLCS(IPoint3D point);

		/// <summary>
		/// Transforms a Point from GCS to LCS
		/// </summary>
		/// <param name="point">Point to be tranformed [In GCS].</param>
		/// <returns>Point in LCS</returns>
		WM.Point3D TransformToLCS(WM.Point3D point);

		/// <summary>
		/// Transforms a <paramref name="vect"/> from GCS to LCS
		/// </summary>
		/// <param name="vect">Vector to be tranformed [In GCS].</param>
		/// <returns>Point in LCS</returns>
		WM.Vector3D TransformToLCS(WM.Vector3D vect);

		/// <summary>
		/// Transforms a vector from GCS to LCS
		/// </summary>
		/// <param name="vector">Point to be tranformed [In GCS].</param>
		/// <returns>Vector in LCS</returns>
		Vector3D TransformToLCS(Vector3D vector);

		/// <summary>
		/// Transforms a Plane3D from GCS to LCS
		/// </summary>
		/// <param name="pln">Plane to be tranformed [In GCS].</param>
		/// <returns>Plane in LCS</returns>
		Plane3D TransformToLCS(Plane3D pln);

		/// <summary>
		/// Transforms a Point from LCS to GCS
		/// </summary>
		/// <param name="point">Point to be tranformed [In LCS].</param>
		/// <returns>Point in GCS</returns>
		IPoint3D TransformToGCS(IPoint3D point);

		/// <summary>
		/// Transforms a Point from LCS to GCS
		/// </summary>
		/// <param name="point">Point to be tranformed [In LCS].</param>
		/// <returns>Point in GCS</returns>
		WM.Point3D TransformToGCS(WM.Point3D point);

		/// <summary>
		/// Transforms <paramref name="vector"/> from LCS to GCS
		/// </summary>
		/// <param name="vector">Point to be tranformed [In LCS].</param>
		/// <returns>Vector in GCS</returns>
		WM.Vector3D TransformToGCS(WM.Vector3D vector);

		/// <summary>
		/// Transforms a vector from LCS to GCS
		/// </summary>
		/// <param name="vector">Point to be tranformed [In LCS].</param>
		/// <returns>Vector in GCS</returns>
		Vector3D TransformToGCS(Vector3D vector);

		/// <summary>
		/// Transforms a Plane3D from LCS to GCS
		/// </summary>
		/// <param name="pln">Plane to be tranformed [In LCS].</param>
		/// <returns>Plane in GCS</returns>
		Plane3D TransformToGCS(Plane3D pln);

		/// <summary>
		/// Concatenates the rotation matrix about an axis to this matrix
		/// </summary>
		/// <param name="rotationAngle">The angle of rotation [Angle in radians]</param>
		/// <param name="rotationAxis">The orientation of the axis to rotate around</param>
		void Rotate(double rotationAngle, Axis rotationAxis);

		/// <summary>
		/// Concatenates a rotation matrix which rotates a vector around another vector to this matrix.
		/// </summary>
		/// <param name="rotationAngle">The angle of rotation in degrees</param>
		/// <param name="normalVector">The orientation of the axis to rotate around</param>
		void Rotate(double rotationAngle, Vector3D normalVector);
		
		/// <summary>
		/// Gets the rotation from matrix 
		/// </summary>
		/// <param name="matrix">Rotation from matrix</param>
		/// <param name="axis">Around axis</param>
		/// <returns>Angle of rotation (0 - 2PI)</returns>
		double GetRotation(IMatrix44 matrix, Axis axis);

		/// <summary>
		/// Returns equation result
		/// </summary>
		/// <param name="matrix">Matrix</param>
		/// <param name="tolerance">Matrix</param>
		/// <returns>Return true if the data are equlas</returns>
		bool EqualsTo(IMatrix44 matrix, double tolerance = 1e-10);

		/// <summary>
		/// Initialize matrix with origin and 2 perpendicular axis
		/// </summary>
		/// <param name="origin">Origin of coordinate system</param>
		/// <param name="axisX">Local X axis of the coordinate system</param>
		/// <param name="axisY">Local Y axis of the coordinate system</param>
		void Set(IPoint3D origin, Vector3D axisX, Vector3D axisY);

		#endregion
	}
}
