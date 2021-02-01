using CI.Geometry2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WM = System.Windows.Media.Media3D;

namespace CI.Geometry3D
{
	public static class Polygon3DExtensions
	{
		#region Intersection

		public static List<Line3D> GetIntersection(this Polygon3D p1, Polygon3D p2)
		{
			var p1Plane = p1.GetPlane();
			var p2Plane = p2.GetPlane();

			////if (WM.Vector3D.CrossProduct(p1Plane.NormalVector, p2Plane.NormalVector).LengthSquared.IsZero())
			////{
			////	// polygons are parallel, use clipper
			////	var axisX = (p1[1] - p1[0]).ToIndoVector3D().Normalize;
			////	var axisZ = p1Plane.NormalVector.ToIndoVector3D();
			////	var axisY = (axisX * axisZ).Normalize;
			////	var lcs = new Matrix44(p1[0], axisX, axisY, axisZ);
			////	var p12D = new Polygon2D(p1.Select(p =>
			////	{
			////		var pt = lcs.TransformToLCS(p);
			////		return new Point(pt.X, pt.Y);
			////	}));
			////	var p22D = new Polygon2D(p2.Select(p =>
			////	{
			////		var pt = lcs.TransformToLCS(p);
			////		return new Point(pt.X, pt.Y);
			////	}));
			////	var clipper = new ClipperController();
			////	clipper.Add(p12D, ClipperLib.PolyType.ptSubject);
			////	clipper.Add(p22D, ClipperLib.PolyType.ptClip);
			////	var inters2 = clipper.IntersectionPolygons();
			////	var xxx = new List<Line3D>();
			////	foreach (var i in inters2)
			////	{
			////	}

			////	throw new NotImplementedException();
			////}

			var p1Inters = GetIntersection(p1, p2Plane);
			var p2Inters = GetIntersection(p2, p1Plane);

			List<Line3D> inters = GetIntersection(p1Inters, p2Inters);
			if(inters.Count > 0)
			{
				var item0 = inters[0];
				if (item0.P1.IsEqual(item0.P2, 1e-6)) // line points are the same
				{
					return new List<Line3D>();
				}
			}

			return inters;
		}

		public static List<Line3D> GetIntersection(this Polygon3D polygon, Plane3D plane)
		{
			var points = new List<WM.Point3D>(2);

			var count = polygon.Count;
			for (var i = 1; i < count; ++i)
			{
				var l0 = polygon[i - 1];
				var l = polygon[i] - l0;
				var d = WM.Vector3D.DotProduct(l, plane.NormalVector);
				if (!d.IsZero(1e-12))
				{
					d = GetIntersecionDistance(l0, l, plane);
					if (d.IsGreaterOrEqual(0) && d.IsLesserOrEqual(1)) // d >= 0 && d <= 1
					{
						var intersection = d * l + l0;
						points.Add(intersection);
					}
				}
			}

			RemoveDuplicates(points);
			count = points.Count;
			// System.Diagnostics.Debug.Assert(!(count > 2 && count % 2 != 0));
			var lines = new List<Line3D>(count / 2);
			for (var i = 1; i < count; i += 2)
			{
				var line = new Line3D(points[i - 1], points[i]);
				lines.Add(line);
			}

			return lines;
		}

		private static List<Line3D> GetIntersection(List<Line3D> lines1, List<Line3D> lines2)
		{
			List<Line3D> _lines1, _lines2;
			if (lines1.Count <= lines2.Count)
			{
				_lines1 = lines1;
				_lines2 = lines2;
			}
			else
			{
				_lines1 = lines2;
				_lines2 = lines1;
			}

			var lines = new List<Line3D>();
			foreach (var l1 in _lines1)
			{
				var v1 = l1.P2 - l1.P1;
				var d1 = WM.Vector3D.DotProduct(v1, v1);
				foreach (var l2 in _lines2)
				{
					var v2 = l2.P1 - l1.P1;
					var v3 = l2.P2 - l1.P1;

					var d2 = WM.Vector3D.DotProduct(v2, v1);
					var d3 = WM.Vector3D.DotProduct(v3, v1);

					if ((d2 < 0 && d3 < 0) || (d2 > d1 && d3 > d1))
					{
						continue;
					}

					var p = new List<double>() { 0, d2, d1, d3 };
					p.Sort();
					WM.Point3D p1, p2;
					if (d1 != 0)
					{
						p1 = l1.P1 + p[1] * v1 / d1;
						p2 = l1.P1 + p[2] * v1 / d1;
					}
					else
					{
						p1 = l1.P1 + p[1] * v1;
						p2 = l1.P1 + p[2] * v1;
					}

					var l = new Line3D(p1, p2);
					lines.Add(l);
				}
			}

			return lines;
		}

		private static double GetIntersecionDistance(WM.Point3D linePoint, WM.Vector3D lineDir, Plane3D plane)
		{
			var d = WM.Vector3D.DotProduct(plane.PointOnPlane - linePoint, plane.NormalVector) / WM.Vector3D.DotProduct(lineDir, plane.NormalVector);
			return d;
		}

		/// <summary>
		/// Pokud jsou za sebou dva stejne body (porovnavaji se souradnice), tak ten druhy smaze.
		/// </summary>
		/// <param name="points">Body ke kontrole.</param>
		private static void RemoveDuplicates(IList<WM.Point3D> points, double tolerance = 1e-9)
		{
			var count = points.Count;
			for (var i = count - 1; i > 0; --i)
			{
				if (Equals(points[i], points[i - 1], tolerance))
				{
					points.RemoveAt(i);
				}
			}

			// nakonec jeste prvni a posledni bod
			count = points.Count;
			if (count > 1 && Equals(points[0], points[count - 1], tolerance))
			{
				points.RemoveAt(count - 1);
			}
		}

		private static void RemoveDuplicates2(IList<WM.Point3D> points, double tolerance = 1e-9)
		{
			var count = points.Count;
			for (var i = count - 1; i > 0; --i)
			{
				if (Equals2(points[i], points[i - 1], tolerance))
				{
					points.RemoveAt(i);
				}
			}

			// nakonec jeste prvni a posledni bod
			count = points.Count;
			if (count > 1 && Equals2(points[0], points[count - 1], tolerance))
			{
				points.RemoveAt(count - 1);
			}
		}

		private static bool Equals(WM.Point3D pt1, WM.Point3D pt2, double tolerance)
		{
			var equal = pt1.X.IsEqual(pt2.X, tolerance) && pt1.Y.IsEqual(pt2.Y, tolerance) && pt1.Z.IsEqual(pt2.Z, tolerance);
			return equal;
		}

		private static bool Equals2(WM.Point3D pt1, WM.Point3D pt2, double tolerance)
		{
			var equal = Math.Abs(pt1.X).IsEqual( Math.Abs(pt2.X), tolerance) && Math.Abs(pt1.Y).IsEqual(Math.Abs(pt2.Y), tolerance);
			return equal;
		}

		#endregion Intersection

		#region Conversion

		public static Polygon3D Convert(IPolyLine3D polyline)
		{
			var points = new List<IPoint3D>(polyline.Count);
			GeomOperation.ConvertPolylineToPointList(polyline, points, false, 5);
			var polygon = new Polygon3D(points.Select(p => p.ToMediaPoint()));
			return polygon;
		}

		#endregion Conversion

		public static void Move(this Polygon3D p, WM.Vector3D dir)
		{
			for (var i = p.Count - 1; i >= 0; --i)
			{
				p[i] += dir;
			}
		}

		public static StraightLine ToStraightLine(this Line3D src)
		{
			return new StraightLine(ref src.P1, ref src.P2);
		}
	}
}