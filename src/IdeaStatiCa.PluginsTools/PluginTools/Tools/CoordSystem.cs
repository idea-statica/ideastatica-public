using CI.DataModel;

namespace CI.Geometry3D
{
	public abstract class CoordSystem : ICoordSystem
	{
		public int Id { get; set; }
		public abstract void CopyTo(ref ICoordSystem to);

		public abstract IMatrix44 GetCoordinateSystemMatrix(IPoint3D originPoint, IPoint3D pointAxisX);
	}
}
