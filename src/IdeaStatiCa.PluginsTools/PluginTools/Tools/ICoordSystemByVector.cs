
namespace CI.Geometry3D
{
	public interface ICoordSystemByVector
	{
		Vector3D VecX { get; set; }

		Vector3D VecY { get; set; }

		Vector3D VecZ { get; set; }

		/// <summary>
		/// Get LCS based on axis vectors
		/// </summary>
		/// <param name="originPoint">Origin point of returned LCS</param>
		/// <returns>Matrix44 as LCS</returns>
		IMatrix44 GetCoordinateSystemMatrix(IPoint3D originPoint);

		/// <summary>
		/// Copies data
		/// </summary>
		/// <param name="to">it to is null, new object is created</param>
		void CopyTo(ref ICoordSystemByVector to);
	}
}
