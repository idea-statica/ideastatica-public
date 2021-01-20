namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Defines basic planes
	/// </summary>
	public enum Plane
	{
		/// <summary>
		/// XY Plane - Plane normal to Z axis
		/// </summary>
		XY,

		/// <summary>
		/// YZ Plane - Plane normal to X axis
		/// </summary>
		YZ,

		/// <summary>
		/// ZX Plane - Plane normal to Y axis
		/// </summary>
		ZX
	}

	/// <summary>
	/// Coordinates system defid by the point and the plane
	/// </summary>
	[OpenModelClass("CI.Geometry3D.CoordSystemByPoint,CI.Geometry3D")]
	public class CoordSystemByPoint : CoordSystem
	{
		/// <summary>
		/// Point where Y o Z axis is located
		/// </summary>
		public Point3D Point { get; set; }

		/// <summary>
		/// Point is in Plane
		/// </summary>
		public Plane InPlane { get; set; }
	}
}