using System.Diagnostics;
using System.Xml.Serialization;
using WM = System.Windows.Media.Media3D;
using System;

namespace CI.Geometry3D
{
	[Serializable, XmlInclude(typeof(Point3D))]
	[DebuggerDisplay("[{X}; {Y}; {Z}]")]
	public class Point3D : IPoint3D
	{
		#region Fields
		double x;
		double y;
		double z;
		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public Point3D()
		{
		}

		/// <summary>
		/// Create a Point3D from 3 double values
		/// </summary>
		/// <param name="x">X value</param>
		/// <param name="y">Y value</param>
		/// <param name="z">Z value</param>
		public Point3D(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		/// <summary>
		/// Create a Point3D from WM.Point3D
		/// </summary>
		/// <param name="src"></param>
		public Point3D(ref WM.Point3D src)
		{
			this.x = src.X;
			this.y = src.Y;
			this.z = src.Z;
		}

		/// <summary>
		/// Create a Point3D from WM.Point3D
		/// </summary>
		/// <param name="src"></param>
		public Point3D(WM.Point3D src)
		{
			this.x = src.X;
			this.y = src.Y;
			this.z = src.Z;
		}

		
		/// <summary>
		/// Create a Point3D from IPoint3D
		/// </summary>
		/// <param name="src"></param>
		public Point3D(IPoint3D src)
		{
			this.x = src.X;
			this.y = src.Y;
			this.z = src.Z;
		}


		#endregion

		#region Overloaded Operators

		/// <summary>
		/// First point - Second point = Resultant Vector
		/// </summary>
		/// <param name="ptFirst">First point to subrtact from</param>
		/// <param name="ptSecond">Second point</param>
		/// <returns>Resultant vector</returns>
		public static Vector3D operator -(Point3D ptFirst, Point3D ptSecond)
		{
			return new Vector3D(ptFirst.X - ptSecond.X, ptFirst.Y - ptSecond.Y, ptFirst.Z - ptSecond.Z);
		}

		#endregion

		#region IPoint3D Members

		public double X
		{
			get
			{
				return x;
			}

			set
			{
				x = value;
			}
		}

		public double Y
		{
			get
			{
				return y;
			}

			set
			{
				y = value;
			}
		}

		public double Z
		{
			get
			{
				return z;
			}

			set
			{
				z = value;
			}
		}


		#endregion
	}
}