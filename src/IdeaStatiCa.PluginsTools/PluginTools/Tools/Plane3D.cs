using System.Runtime.Serialization;
using WM = System.Windows.Media.Media3D;

namespace CI.Geometry3D
{
	/// <summary>
	/// Represents plane in 3D
	/// </summary>
	[DataContract]
	public class Plane3D
	{
		#region Fields
		protected WM.Point3D point;
		protected WM.Vector3D normal;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public Plane3D()
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="src">Source</param>
		public Plane3D(Plane3D src)
		{
			this.point = src.point;
			this.normal = src.normal;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="point">Point on plane</param>
		/// <param name="normal">Normal vector</param>
		public Plane3D(WM.Point3D point, WM.Vector3D normal)
		{
			this.point = point;
			this.normal = normal;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="pointX">The X value of the point on plane</param>
		/// <param name="pointY">The Y value of the point on plane</param>
		/// <param name="pointZ">The Z value of the point on plane</param>
		/// <param name="normalX">The X value of the normal vector of the plane</param>
		/// <param name="normalY">The Y value of the normal vector of the plane</param>
		/// <param name="normalZ">The Z value of the normal vector of the plane</param>
		public Plane3D(double pointX, double pointY, double pointZ,
			double normalX, double normalY, double normalZ)
		{
			this.point = new WM.Point3D(pointX, pointY, pointZ);
			this.normal = new WM.Vector3D(normalX, normalY, normalZ);
		}

		/// <summary>
		/// Creates a new instance of Plane3D, that is given by 3 points.
		/// </summary>
		/// <param name="p1">First point laying in the plane. This one is taken as origin point.</param>
		/// <param name="p2">Second point laying in the plane.</param>
		/// <param name="p3">Third point laying in the plane.</param>
		public Plane3D(WM.Point3D p1, WM.Point3D p2, WM.Point3D p3)
		{
			point = p1;
			var v1 = p2 - p1;
			var v2 = p3 - p1;
			normal = WM.Vector3D.CrossProduct(v1, v2);
			normal.Normalize();
		}

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the point on plane
		/// </summary>
		[DataMember]
		public WM.Point3D PointOnPlane
		{
			get { return point; }
			set { point = value; }
		}

		/// <summary>
		/// Gets or sets the normal vector fot the plane
		/// </summary>
		[DataMember]
		public WM.Vector3D NormalVector
		{
			get { return normal; }
			set { normal = value; }
		}
		#endregion
	}
}
