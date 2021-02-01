using System;
using System.Diagnostics;
using WM = System.Windows.Media.Media3D;

namespace CI.Geometry3D
{
	public static class Plane3DExtensions
	{
		/// <summary>
		/// Return the new instance of the paralel plane
		/// </summary>
		/// <param name="src">Source plane</param>
		/// <param name="offset">Distance between planes.
		/// Positive value is in the direction of normal vector of this plane</param>
		/// <returns>The new instance of the plane</returns>
		public static Plane3D Offset(this Plane3D src, double offset)
		{
			if (offset.IsZero())
			{
				return new Plane3D(src);
			}

			var v = WM.Vector3D.Multiply(src.NormalVector, offset);
			var n = v;
			n.Normalize();
			return new Plane3D(src.PointOnPlane + v, n);
		}

		/// <summary>
		/// Gets parameter d in plane equation ax + by + cz + d = 0
		/// </summary>
		/// <param name="src">Source plane</param>
		/// <returns>Returns d parameter</returns>
		public static double GetD(this Plane3D src)
		{
			return (-1* WM.Vector3D.DotProduct(src.NormalVector, (WM.Vector3D)src.PointOnPlane));
		}

		/// <summary>
		/// Caluclates distance from a point to a plane
		/// </summary>
		/// <param name="src">Plane</param>
		/// <param name="p">Point</param>
		/// <returns>Distance from a point to a plane (positive value is in the direction of the normal vector)</returns>
		public static double GetPointDistance(this Plane3D src, ref WM.Point3D p)
		{
			return (WM.Vector3D.DotProduct(src.NormalVector, (WM.Vector3D)p) + src.GetD()) / src.NormalVector.Length;
		}

		/// <summary>
		/// Caluclates distance from a point to a plane
		/// </summary>
		/// <param name="src">Plane</param>
		/// <param name="p">Point</param>
		/// <returns>Distance from a point to a plane (positive value is in the direction of the normal vector)</returns>
		public static double GetPointDistance(this Plane3D src, IPoint3D p)
		{
			return (WM.Vector3D.DotProduct(src.NormalVector, new WM.Vector3D(p.X, p.Y, p.Z)) + src.GetD()) / src.NormalVector.Length;
		}

		/// <summary>
		/// Calculates intersection of the plane and line which is defined by direction vector <paramref name="l"/> and point <paramref name="l0"/>
		/// </summary>
		/// <param name="src">Plane</param>
		/// <param name="l0">Point on line</param>
		/// <param name="l">Direction vector of line</param>
		/// <returns>Found intersection. If intersection doesn't NaN is set</returns>
		public static WM.Point3D GetIntersection(this Plane3D src, WM.Point3D l0, WM.Vector3D l)
		{
			double d = WM.Vector3D.DotProduct(src.NormalVector, l);
			if(d.IsZero())
			{
				return new WM.Point3D(double.NaN, double.NaN, double.NaN);
			}

			double dd = WM.Vector3D.DotProduct((src.PointOnPlane - l0), src.NormalVector) / d;
			return (l0 + dd * l);
		}

		/// <summary>
		/// Calculates intersection of the plane and line which is defined by direction vector <paramref name="l"/> and point <paramref name="l0"/>
		/// </summary>
		/// <param name="src">Plane</param>
		/// <param name="ray">Straight line</param>
		/// <returns>Found intersection. If intersection doesn't NaN is set</returns>
		public static WM.Point3D GetIntersection(this Plane3D src, ref StraightLine ray)
		{
			double d = WM.Vector3D.DotProduct(src.NormalVector, ray.Direction);
			if (d.IsZero())
			{
				return new WM.Point3D(double.NaN, double.NaN, double.NaN);
			}

			double dd = WM.Vector3D.DotProduct((src.PointOnPlane - ray.Point), src.NormalVector) / d;
			return (ray.Point + dd * ray.Direction);
		}

		public static WM.Point3D GetIntersection(ref StraightLine planeNormal, ref StraightLine ray)
		{
			double d = WM.Vector3D.DotProduct(planeNormal.Direction, ray.Direction);
			if (d.IsZero())
			{
				return new WM.Point3D(double.NaN, double.NaN, double.NaN);
			}

			double dd = WM.Vector3D.DotProduct((planeNormal.Point - ray.Point), planeNormal.Direction) / d;
			return (ray.Point + dd * ray.Direction);
		}

		/// <summary>
		/// Calculates intersection of the plane and line <paramref name="lineSeg"/>
		/// </summary>
		/// <param name="src">Plane</param>
		/// <param name="lineSeg">Line segment</param>
		/// <returns></returns>
		public static WM.Point3D GetIntersection(this Plane3D src, ILineSegment3D lineSeg)
		{
			var l = new WM.Vector3D(lineSeg.EndPoint.X - lineSeg.StartPoint.X,
				lineSeg.EndPoint.Y - lineSeg.StartPoint.Y,
				lineSeg.EndPoint.Z - lineSeg.StartPoint.Z);
			l.Normalize();

			double d = WM.Vector3D.DotProduct(src.NormalVector, l);
			if (d.IsZero())
			{
				return new WM.Point3D(double.NaN, double.NaN, double.NaN);
			}

			WM.Point3D l0 = lineSeg.StartPoint.ToMediaPoint();

			double dd = WM.Vector3D.DotProduct((src.PointOnPlane - l0), src.NormalVector) / d;
			return (l0 + dd * l);
		}

		/// <summary>
		/// Gets intersection of two planes
		/// </summary>
		/// <param name="plane1">The first plane</param>
		/// <param name="plane2">The second plane</param>
		/// <param name="pointOnLine">Point on line where <paramref name="plane1"/> and <paramref name="plane1"/> are intersecting</param>
		/// <param name="dirVect">Direction vector of the intersection</param>
		/// <returns>True if intersection exists</returns>
		public static bool GetIntersection(Plane3D plane1, Plane3D plane2, out WM.Point3D pointOnLine, out WM.Vector3D dirVect)
		{
#if DEBUG
			// Normal vectors should be normalized
			WM.Vector3D testVect1 = plane1.NormalVector;
			testVect1.Normalize();
			WM.Vector3D testVect2 = plane2.NormalVector;
			testVect2.Normalize();
			Debug.Assert(WM.Vector3D.DotProduct(plane1.NormalVector, plane2.NormalVector).IsEqual(WM.Vector3D.DotProduct(testVect1, testVect2)));
#endif

			double normalDot = WM.Vector3D.DotProduct(plane1.NormalVector, plane2.NormalVector);
			double D = (1 - normalDot * normalDot);
			if (D.IsZero())
			{
				pointOnLine = new WM.Point3D(double.NaN, double.NaN, double.NaN);
				dirVect = new WM.Vector3D(double.NaN, double.NaN, double.NaN);
				return false;
			}

			double h1 = -1 * plane1.GetD();
			double h2 = -1 * plane2.GetD();

			double c1 = (h1 - h2 * normalDot) / D;
			double c2 = (h2 - h1 * normalDot) / D;

			dirVect = WM.Vector3D.CrossProduct(plane1.NormalVector, plane2.NormalVector);
			dirVect.Normalize();
			pointOnLine = (WM.Point3D)(c1 * plane1.NormalVector + c2 * plane2.NormalVector);
			return true;
		}

		/// <summary>
		/// Returns the ordered tuple. Point in Item1 is closer to the plane
		/// </summary>
		/// <param name="src">Plane</param>
		/// <param name="pt1">Tested point1</param>
		/// <param name="pt2">Tested point2</param>
		/// <returns>The ordered tuple</returns>
		public static Tuple<WM.Point3D, WM.Point3D> OrderPointsByDistance(this Plane3D src, ref WM.Point3D pt1, ref WM.Point3D pt2)
		{
			double distPt1 = Math.Abs(src.GetPointDistance(ref pt1));
			double distPt2 = Math.Abs(src.GetPointDistance(ref pt2));

			if (distPt1 > distPt2)
			{
				return new Tuple<WM.Point3D, WM.Point3D>(pt2, pt1);
			}

			return new Tuple<WM.Point3D, WM.Point3D>(pt1, pt2);
		}

		/// <summary>
		/// Returns the ordered tuple. Point in Item1 is closer to the plane
		/// </summary>
		/// <param name="src">Plane</param>
		/// <param name="pt1">Tested point1</param>
		/// <param name="pt2">Tested point2</param>
		/// <returns>The ordered tuple</returns>
		public static Tuple<IPoint3D, IPoint3D> OrderPointsByDistance(this Plane3D src, IPoint3D pt1, IPoint3D pt2)
		{
			double distPt1 = Math.Abs(src.GetPointDistance(pt1));
			double distPt2 = Math.Abs(src.GetPointDistance(pt2));

			if (distPt1 > distPt2)
			{
				return new Tuple<IPoint3D, IPoint3D>(pt2, pt1);
			}

			return new Tuple<IPoint3D, IPoint3D>(pt1, pt2);
		}

		/// <summary>
		/// Normalizes the specified Plane3D.NormalVector.
		/// </summary>
		/// <param name="src">Plane</param>
		public static void Normalize(this Plane3D src)
		{
			var v = src.NormalVector;
			v.Normalize();
			src.NormalVector = v;
		}
	}
}
