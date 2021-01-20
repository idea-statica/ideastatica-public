namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Coordinate system defined by vectors
	/// </summary>
	[OpenModelClass("CI.Geometry3D.CoordSystemByVector,CI.Geometry3D")]
	public class CoordSystemByVector : CoordSystem
	{
		/// <summary>
		/// Axis X unit vector
		/// </summary>
		public Vector3D VecX { get; set; }

		/// <summary>
		/// Axis Y unit vector
		/// </summary>
		public Vector3D VecY { get; set; }

		/// <summary>
		/// Axis Z unit vector
		/// </summary>
		public Vector3D VecZ { get; set; }
	}
}