using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WM = System.Windows.Media.Media3D;

namespace CI.Geometry3D
{
	[System.Diagnostics.DebuggerDisplay("{P1}; {P2}")]
	public struct Line3D
	{
		public Line3D(WM.Point3D p1, WM.Point3D p2)
		{
			P1 = p1;
			P2 = p2;
		}

		public WM.Point3D P1;
		public WM.Point3D P2;

		public void Reverse()
		{
			var p = P1;
			P1 = P2;
			P2 = p;
		}
	}

	public class Polygon3D : List<WM.Point3D>
	{
		public Polygon3D(int capacity)
			: base(capacity)
		{
		}

		public Polygon3D(IEnumerable<WM.Point3D> collection)
			: base(collection)
		{
		}

		public Plane3D GetPlane()
		{
			if (this.Count < 3)
			{
				return null;
			}

			var v1 = this[0] - this[1];
			var v2 = this[2] - this[1];
			var n = WM.Vector3D.CrossProduct(v1, v2);
			n.Normalize();
			var p = new Plane3D(this[0], n);
			return p;
		}
	}
}
