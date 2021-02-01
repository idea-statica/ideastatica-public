using CI;
using CI.Geometry2D;
using CI.Geometry3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WM = System.Windows.Media.Media3D;

namespace IdeaRS.Connections.Commands
{
	public static class GeometryTools
	{
		/// <summary>
		/// Transforms given polyline 3D to LCS using matrix and converts to 2D geometry.
		/// </summary>
		/// <param name="polyline3D">Polyline to convert.</param>
		/// <param name="lcs">Local coordinate system.</param>
		/// <returns>2D geometry.</returns>
		public static IPolyLine2D ConvertTo2D(IPolyLine3D polyline3D, IMatrix44 lcs)
		{
			var polylineCopy = new PolyLine3D(polyline3D);
			GeomOperation.TransformToLCS(lcs, polylineCopy);
			var polyline2D = ConvertTo2D(polylineCopy);
			return polyline2D;
		}

		public static ISegment2D ConvertTo2D(ISegment3D segment3D, IMatrix44 lcs)
		{
			var segmentCopy = segment3D.CloneSegment();
			GeomOperation.TransformToLCS(lcs, segmentCopy);
			var segment2D = ConvertTo2D(segmentCopy);
			return segment2D;
		}

		public static IRegion2D ConvertTo2D(IRegion3D region3D, IMatrix44 lcs)
		{
			var regionCopy = new Region3D(region3D);
			GeomOperation.TransformToLCS(lcs, regionCopy);
			var outline2D = ConvertTo2D(regionCopy.Outline);
			var region2D = new Region2D(outline2D);
			if (regionCopy.OpeningsCount > 0)
			{
				foreach (var opening3D in regionCopy.Openings)
				{
					region2D.Openings.Add(ConvertTo2D(opening3D));
				}
			}

			return region2D;
		}

		internal static IPolyLine2D ConvertTo2D(IPolyLine3D polyline3D)
		{
			IPolyLine2D polyline2D = new PolyLine2D();
			foreach (ISegment3D segment3D in polyline3D.Segments)
			{
				var segment2D = ConvertTo2D(segment3D);
				if (segment2D != null)
				{
					polyline2D.Segments.Add(segment2D);
				}
			}

			if (polyline3D.IsClosed)
				polyline2D.StartPoint = new IdaComPoint2D(polyline2D.Segments[polyline2D.Segments.Count - 1].EndPoint.X, polyline2D.Segments[polyline2D.Segments.Count - 1].EndPoint.Y);
			else
				polyline2D.StartPoint = new IdaComPoint2D(polyline3D[0].StartPoint.X, polyline3D[0].StartPoint.Y);

			return polyline2D;
		}

		private static ISegment2D ConvertTo2D(ISegment3D segment3D)
		{
			if (segment3D.SegmentType == SegmentType.Line)
			{
				LineSegment2D lineSegment2D = new LineSegment2D(new Point(segment3D.EndPoint.X, segment3D.EndPoint.Y));
				return lineSegment2D;
			}
			else if (segment3D.SegmentType == SegmentType.CircularArc)
			{
				CircularArcSegment2D arcSegment2D = new CircularArcSegment2D(new Point(segment3D.EndPoint.X, segment3D.EndPoint.Y), new Point((segment3D as IArcSegment3D).IntermedPoint.X, (segment3D as IArcSegment3D).IntermedPoint.Y));
				return arcSegment2D;
			}

			return null;
		}

		internal static IPolygon2D ConvertTo2D(Polygon3D polygon3D, IMatrix44 lcs)
		{
			var pts = polygon3D.Select(pt =>
			{
				var p = lcs.TransformToLCS(pt);
				return new Point(p.X, p.Y);
			});

			return new Polygon2D(pts);
		}

		internal static IPolyLine3D ConvertTo3D(IPolygon2D polygon2D, IMatrix44 lcs = null)
		{
			if (polygon2D == null)
			{
				return null;
			}

			var count = polygon2D.Count;
			if (count > 1)
			{
				var polyline3D = new PolyLine3D();
				var beg = new Point3D(polygon2D[0].X, polygon2D[0].Y, 0);
				for (var i = 1; i < count; ++i)
				{
					var end = new Point3D(polygon2D[i].X, polygon2D[i].Y, 0);
					var seg = new LineSegment3D(beg, end);
					polyline3D.Add(seg);
					beg = end;
				}

				if (polygon2D.IsClosed)
				{
					polyline3D.Segments.Last().EndPoint = polyline3D.Segments.First().StartPoint;
				}

				if (lcs != null)
				{
					GeomOperation.TransformToGCS(lcs, polyline3D);
				}

				return polyline3D;
			}

			return null;
		}

		internal static IPolyLine3D ConvertTo3D(IPolygon2D polygon2D, Func<double, double, IPoint3D> createPoint, IMatrix44 lcs = null)
		{
			if (polygon2D == null)
			{
				return null;
			}

			var count = polygon2D.Count;
			if (count > 1)
			{
				var polyline3D = new PolyLine3D();
				var beg = createPoint(polygon2D[0].X, polygon2D[0].Y);
				for (var i = 1; i < count; ++i)
				{
					var end = createPoint(polygon2D[i].X, polygon2D[i].Y);
					var seg = new LineSegment3D(beg, end);
					polyline3D.Add(seg);
					beg = end;
				}

				if (polygon2D.IsClosed)
				{
					polyline3D.Segments.Last().EndPoint = polyline3D.Segments.First().StartPoint;
				}

				if (lcs != null)
				{
					GeomOperation.TransformToGCS(lcs, polyline3D);
				}

				return polyline3D;
			}

			return null;
		}

		public static IPolyLine3D ConvertTo3D(IPolyLine2D polyline, IMatrix44 lcs)
		{
			var geom = GeomOperation.ConvertTo3D(polyline);
			GeomOperation.TransformToGCS(lcs, geom);
			return geom;
		}

		internal static IPolygon2D ToPolygon2D(IPolyLine2D polyline, int numberOfArcTiles = 2)
		{
			var discr = new PolyLine2DDiscretizator { Angle = 180, NumberOfArcTiles = numberOfArcTiles, LengthOfTile = double.PositiveInfinity, };
			IPolygon2D polygon = new Polygon2D();
			discr.Discretize(polyline, ref polygon, null);
			return polygon;
		}

		internal static IPolygon2D ToPolygon2D(IPolyLine3D polyline3D, IMatrix44 lcs, int numberOfArcTiles)
		{
			var polyline = ConvertTo2D(polyline3D, lcs);
			var polygon = ToPolygon2D(polyline, numberOfArcTiles);
			return polygon;
		}

		internal static Polygon3D ToPolygon3D(IPolyLine2D outline, IMatrix44 lcs)
		{
			/// pozor - konvertuje pouze koncove body - tzn. zadne obloucky
			var count = outline.Segments.Count;
			var polygon = new Polygon3D(count + 1);

			var point = outline.StartPoint;
			var point3D = lcs.TransformToGCS(new WM.Point3D(0, point.X, point.Y));
			polygon.Add(point3D);

			for (var i = 0; i < count; ++i)
			{
				point = outline.Segments[i].EndPoint;
				point3D = lcs.TransformToGCS(new WM.Point3D(0, point.X, point.Y));
				polygon.Add(point3D);
			}

			return polygon;
		}

		internal static IPolyLine2D ToPolyline2D(Rect rect)
		{
			var p = new PolyLine2D
			{
				StartPoint = rect.BottomLeft,
			};

			p.Segments.Add(new LineSegment2D(rect.BottomRight));
			p.Segments.Add(new LineSegment2D(rect.TopRight));
			p.Segments.Add(new LineSegment2D(rect.TopLeft));
			p.Segments.Add(new LineSegment2D(rect.BottomLeft));

			return p;
		}

		internal static double AngleBetween(WM.Vector3D va, WM.Vector3D vb, WM.Vector3D vn, bool correctTo90_90 = false)
		{
			var angle = Math.Atan2(WM.Vector3D.DotProduct(WM.Vector3D.CrossProduct(vb, va), vn), WM.Vector3D.DotProduct(va, vb));

			if (correctTo90_90)
			{
				if (angle.IsEqual(Math.PI) || angle.IsEqual(-Math.PI))
					return 0;

				if (angle > 0)
				{
					while (angle > Math.PI / 2)
					{
						angle -= Math.PI / 2;
					}
				}
				else
				{
					while (angle < -Math.PI / 2)
					{
						angle += Math.PI / 2;
					}
				}
			}

			return angle;
		}


		//Find the line of intersection between two planes.
		//The inputs are two game objects which represent the planes.
		//The outputs are a point on the line and a vector which indicates it's direction.
		internal static bool PlanesIntersection(Plane3D plane1, Plane3D plane2, out StraightLine intersection)
		{
			var linePoint = new WM.Point3D();
			var lineVec = new WM.Vector3D();

			//Get the normals of the planes.
			var plane1Normal = plane1.NormalVector;
			var plane2Normal = plane2.NormalVector;

			//We can get the direction of the line of intersection of the two planes by calculating the
			//cross product of the normals of the two planes. Note that this is just a direction and the line
			//is not fixed in space yet.
			lineVec = WM.Vector3D.CrossProduct(plane1Normal, plane2Normal);

			//Next is to calculate a point on the line to fix it's position. This is done by finding a vector from
			//the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
			//errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
			//the cross product of the normal of plane2 and the lineDirection.
			var ldir = WM.Vector3D.CrossProduct(plane2Normal, lineVec);

			var numerator = WM.Vector3D.DotProduct(plane1Normal, ldir);

			//Prevent divide by zero.
			if (Math.Abs(numerator) > 0.000001)
			{
				var plane1ToPlane2 = plane1.PointOnPlane - plane2.PointOnPlane;
				var t = WM.Vector3D.DotProduct(plane1Normal, plane1ToPlane2) / numerator;
				linePoint = plane2.PointOnPlane + t * ldir;
				intersection = new StraightLine(ref linePoint, ref lineVec);
				return true;
			}

			intersection = default(StraightLine);
			return false;
		}

		public static void SortSegments(IPolyLine2D polyline, IPolyLine2D pattern)
		{
			var patternStart = (Point)pattern.StartPoint;
			var patternCount = pattern.Segments.Count;
			int patternInx = -1, inx = -1;
			for (var i = 0; i < patternCount; ++i)
			{
				var patternSegment = pattern.Segments[i];

				var start = (Point)polyline.StartPoint;
				var count = polyline.Segments.Count;
				for (var j = 0; j < count; ++j)
				{
					var segment = polyline.Segments[j];

					if (start.IsEqualWithTolerance(patternStart, 1e-6) &&
						segment.EndPoint.IsEqualWithTolerance(patternSegment.EndPoint, 1e-6) &&
						segment.GetType() == patternSegment.GetType())
					{
						patternInx = i;
						inx = j;
						break;
					}

					start = segment.EndPoint;
				}

				if (inx >= 0)
					break;

				patternStart = patternSegment.EndPoint;
			}

			if (patternInx != inx)
			{
				GeomTools2D.RotateSegments(polyline, inx - patternInx);
			}
		}

		/// <summary>
		/// Returns mininum distance between regions
		/// </summary>
		/// <param name="inner"></param>
		/// <param name="outer"></param>
		/// <returns></returns>
		public static double GetDistanceBetweenPolylines3D(IPolyLine3D inner, IPolyLine3D outer)
		{
			IList<IPoint3D> innerPoints = inner.Segments.Select(s => s.StartPoint).ToList();
			IList<ISegment3D> outerSegments = outer.Segments.ToList();

			double minDistance = double.MaxValue;
			Point3D intersection = new Point3D();

			foreach (var innerPoint in innerPoints)
			{
				Plane3D tempPlane = new Plane3D
				{
					PointOnPlane = new WM.Point3D(innerPoint.X, innerPoint.Y, 0)
				};

				foreach (var outerSegment in outerSegments)
				{
					tempPlane.NormalVector = outerSegment.GetDirection();

					if (outerSegment is ILineSegment3D)
					{
						intersection = new Point3D(tempPlane.GetIntersection(outerSegment as ILineSegment3D));
					}

					var vect = intersection - (innerPoint as Point3D);
					var tempDist = vect.Magnitude;

					minDistance = Math.Min(minDistance, tempDist);
				}
			}

			return minDistance;
		}
	}
}