using CI.DataModel;

namespace CI.Geometry3D
{
	public class CoordSystemByZup : CoordSystem
	{
		/// <summary>
		/// Get LCS based on local Z axis up
		/// In case local X is vertical then Y local axis follows Y global axis 
		/// </summary>
		/// <param name="originPoint">Origin point of returned LCS</param>
		/// <param name="pointAxisX">IPoint3D</param>
		/// <returns>Matrix44 as LCS</returns>
		public override IMatrix44 GetCoordinateSystemMatrix(IPoint3D originPoint, IPoint3D pointAxisX)
		{
			return GeomOperation.GetLCSMatrix(originPoint, pointAxisX);
		}

		/// <summary>
		/// Copies data
		/// </summary>
		/// <param name="to">it to is null, new object is created</param>
		public override void CopyTo(ref ICoordSystem to)
		{
			if ((to == null) || !(to is CoordSystemByZup))
			{
				to = new CoordSystemByZup();
			}
		}
	}
}
