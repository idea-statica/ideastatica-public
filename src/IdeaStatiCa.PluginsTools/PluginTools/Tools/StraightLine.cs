using WM = System.Windows.Media.Media3D;

namespace CI.Geometry3D
{
	/// <summary>
	/// Represents the straight line
	/// </summary>
	public struct StraightLine
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="point">Point on straight line</param>
		/// <param name="direction">Direction vector of the straight line</param>
		public StraightLine(ref WM.Point3D point, ref WM.Vector3D direction)
		{
			this.Point = point;
			this.Direction = direction;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pt1">Start point</param>
		/// <param name="pt2">Second point</param>
		public StraightLine(ref WM.Point3D pt1, ref WM.Point3D pt2)
		{
			this.Point = pt1;
			this.Direction = pt2 - pt1;
			this.Direction.Normalize();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		public StraightLine(IPoint3D pt1, IPoint3D pt2)
		{
			this.Point = new WM.Point3D(pt1.X, pt1.Y, pt1.Z);
			this.Direction = new WM.Vector3D(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z);
			this.Direction.Normalize();
		}

		/// <summary>
		/// Point on straight line
		/// </summary>
		public WM.Point3D Point;

		/// <summary>
		/// Direction vector of the straight line
		/// </summary>
		public WM.Vector3D Direction;
	}
}
