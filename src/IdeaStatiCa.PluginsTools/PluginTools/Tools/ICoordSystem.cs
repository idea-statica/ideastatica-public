
namespace CI.Geometry3D
{
	public interface ICoordSystem
	{
		/// <summary>
		/// Get LCS based on inner properties
		/// </summary>
		/// <param name="originPoint">Origin point of returned LCS</param>
		/// <param name="pointAxisX">Point on Axis X</param>
		/// <returns>Matrix44 as LCS</returns>
		IMatrix44 GetCoordinateSystemMatrix(IPoint3D originPoint, IPoint3D pointAxisX);

		/// <summary>
		/// Copies data
		/// </summary>
		/// <param name="to">it to is null, new object is created</param>
		void CopyTo(ref ICoordSystem to);
	}
}