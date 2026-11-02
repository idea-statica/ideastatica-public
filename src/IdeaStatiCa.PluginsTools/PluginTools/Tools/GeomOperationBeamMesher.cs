using System;
using System.Collections.Generic;

namespace CI.Geometry3D
{

	/// <summary>
	/// Geometry operations related with Region3D
	/// </summary>
	public static partial class GeomOperation
	{

		// IMPORTANT !!!
		// copy file in "...\_Sources\Modelers\Common\IdeaStatiCa.PluginTools\Tools\" because reference missing

		#region VertexesTriangleMesh


		public static IPoint3D GetCentroidVertexesTriangleMesh(IVertexesTriangleMesh triangle)
		{
			if (null == triangle)
			{
				return null;
			}
			//
			return (new Point3D(
				(triangle.V1.X+ triangle.V2.X+ triangle.V3.X) / 3.0,
				(triangle.V1.Y + triangle.V2.Y + triangle.V3.Y) / 3.0,
				(triangle.V1.Z + triangle.V2.Z + triangle.V3.Z) / 3.0));
		}


		public static double GetAreaVertexesTriangleMesh(IVertexesTriangleMesh triangle)
		{
			// Heron method
			if (null == triangle)
			{
				return 0.0;
			}
			//
			double a = GetDistance(triangle.V1, triangle.V2);
			double b = GetDistance(triangle.V2, triangle.V3);
			double c = GetDistance(triangle.V3, triangle.V1);
			//
			double s = 0.5 * (a + b + c);
			double area = Math.Sqrt(s * (s - a) * (s - b) * (s - c));
			return area;
		}

		public static double GetPerimeterVertexesTriangleMesh(IVertexesTriangleMesh triangle)
		{
			// Heron method
			if (null == triangle)
			{
				return 0.0;
			}
			//
			double a = GetDistance(triangle.V1, triangle.V2);
			double b = GetDistance(triangle.V2, triangle.V3);
			double c = GetDistance(triangle.V3, triangle.V1);
			//
			return (a + b + c);
		}

		// counter clockwise direction
		// rigth hand rule
		public static Vector3D GetNormalVertexesTriangleMesh(IVertexesTriangleMesh triangle)
		{
			if (null == triangle)
			{
				return new Vector3D();
			}
			//
			return GetNormalByThreePoints(triangle.V1, triangle.V2, triangle.V3);
		}


		#endregion // VertexesTriangleMesh


		#region Point Inside


		public static bool IsPointInsidePrizm4(IPoint3D point,
			IPoint3D pt1, IPoint3D pt2, IPoint3D pt4, IPoint3D pt5)
		{
			// check if point is inside cuboid
			//
			// Rec cross section defined by pt1-pt2 
			// pt1 - left down at start section
			// pt2 - rigth down at start section (define 1 side)
			// pt4 - left up at start section (define 2 side)
			// pt5 - left down at end section (define 3 side) - length of the prizm

			var vi = Subtract(pt2, pt1);
			var vj = Subtract(pt4, pt1);
			var vk = Subtract(pt5, pt1);
			var vv = Subtract(point, pt1);

			double viLen = vi.Magnitude;
			double vjLen = vj.Magnitude;
			double vkLen = vk.Magnitude;

			var viNorm = vi.Normalize;
			var vjNorm = vj.Normalize;
			var vkNorm = vk.Normalize;

			double viL = vv | viNorm;
			double vjL = vv | vjNorm;
			double vkL = vv | vkNorm;

			if ((viL < 0) || (vjL < 0) || (vkL < 0))
			{
				return false;
			}

			if ((viL > viLen) || (vjL > vjLen) || (vkL > vkLen))
			{
				return false;
			}

			return true;
		}


		public static bool IsPointInsideTriangle2D(IPoint3D p,
			IPoint3D p0, IPoint3D p1, IPoint3D p2)
		{
			// XY-plane

			var s = p0.Y * p2.X - p0.X * p2.Y + (p2.Y - p0.Y) * p.X + (p0.X - p2.X) * p.Y;
			var t = p0.X * p1.Y - p0.Y * p1.X + (p0.Y - p1.Y) * p.X + (p1.X - p0.X) * p.Y;

			if ((s < 0) != (t < 0))
				return false;

			var A = -p1.Y * p2.X + p0.Y * (p2.X - p1.X) + p0.X * (p1.Y - p2.Y) + p1.X * p2.Y;

			return A < 0 ?
							(s <= 0 && s + t >= A) :
							(s >= 0 && s + t <= A);
		}


		//public static bool IsPointInsideTriangle(IPoint3D point, 
		//	IPoint3D pt1, IPoint3D pt2, IPoint3D pt3)
		//{
		//	// 1. check point lie on plane
		//	// 2. transform to trianglr CSys
		//	// 3. check point is inside trinagle in 2D


		//}

		#endregion // Point Inside


		#region Intersection

		// intersect_RayTriangle(): intersect a ray with a 3D triangle
		//    Input:  a ray R, and a triangle T
		//    Output: *I = intersection point (when it exists)
		//    Return: -1 = triangle is degenerate (a segment or point)
		//             0 = disjoint (no intersect)
		//             1 = intersect in unique point I1 (inside triangle)
		//             2 = are in the same plane
		//             3 = intersect in unique point I1 (outside the triangle)
		public static int IntersectionPointPerpendicularToTriangle3D(IPoint3D pt0,
			IPoint3D trianglePt1, IPoint3D trianglePt2, IPoint3D trianglePt3,
			out IPoint3D intPt, out int normalDirection)
		{

			intPt = null;
			// Direction of the point in respect to triangle normal
			// = 0 lie on plane
			// = 1 point is on side of the normal (on front face)
			// = -1 point is on oposite side of the normal (on back face) -> inside solid if normal is pointing outside solid
			normalDirection = 0;

			Vector3D u, v, n;             // triangle vectors
			Vector3D dir, w0, w;          // ray vectors
			double r, a, b;             // params to calc ray-plane intersect

			double TOL9 = 0.00000001;

			// get triangle edge vectors and plane normal
			u = Subtract(trianglePt2, trianglePt1); //  T.V1 - T.V0;
			v = Subtract(trianglePt3, trianglePt1); // T.V2 - T.V0;
			n = u * v;             // cross product -> normal with direction to outside
														 // n = n.Normalize;

			if ((n.DirectionX == 0) &&
				(n.DirectionY == 0) &&
				(n.DirectionZ == 0)) // triangle is degenerate
				return -1;                 // do not deal with this case

			dir = new Vector3D(n); // R.P1 - R.P0;             // ray direction vector
			w0 = Subtract(pt0, trianglePt1); //  R.P0 - T.V0;
			a = -(n | w0);
			b = (n | dir);
			// in case where ray is not equal to normal direction (perpendicular to triangle)
			//if (Math.Abs(b) < TOL9) 
			//{
			//	// ray is parallel to triangle plane
			//	if (a == 0)
			//	{
			//		// ray lies in triangle plane
			//		return 2;
			//	}
			//	else
			//	{
			//		// ray disjoint from plane
			//		return 0;             
			//	}
			//}

			// get intersect point of ray with triangle plane
			r = a / b;

			// = 0 lie on plane
			// = 1 point is on side of the normal (on front face)
			// = -1 point is on oposite side of the normal (on back face)
			normalDirection = -1;
			if (r < 0.0)
			{
				normalDirection = 1;
				// ray goes away from triangle
				// => no intersect
				//	return 0;                  
			}
			if (Math.Abs(r) < TOL9)
			{
				// on plane
				normalDirection = 0;
			}
			// for a segment, also test if (r > 1.0) => no intersect

			// intersect point of ray and plane
			intPt = new Point3D(pt0);
			MultiplyPoint3DByVector3D(intPt, dir, r, false);  // R.P0 + r * dir; 

			// is I inside Triangle?
			double uu, uv, vv, wu, wv, D;
			uu = (u | u);
			uv = (u | v);
			vv = (v | v);
			w = Subtract(intPt, trianglePt1); // * I - T.V0;
			wu = (w | u);
			wv = (w | v);
			D = uv * uv - uu * vv;

			// get and test parametric coords
			double s, t;
			s = (uv * wv - vv * wu) / D;
			if (s < 0.0 || s > 1.0)
			{
				// I is outside T
				return 3;
			}
			t = (uv * wu - uu * wv) / D;
			if (t < 0.0 || (s + t) > 1.0)
			{
				// I is outside T
				return 3;
			}

			return 1;                      // I is in T
		}


		public static bool IntersectionLineTriangle(IPoint3D linePt1, IPoint3D linePt2,
		IPoint3D trianglePt1, IPoint3D trianglePt2, IPoint3D trianglePt3,
		out IPoint3D intPoint)
		{
			// https://stackoverflow.com/questions/53962225/how-to-know-if-a-line-segment-intersects-a-triangle-in-3d-space
			// https://cboard.cprogramming.com/c-programming/101484-triangle-line-segment-intersection.html
			// http://geomalgorithms.com/a06-_intersect-2.html#intersect_RayTriangle()
			// return intersection point line-triangle

			double TOL9 = 0.00000001;

			intPoint = null;

			var e0 = Subtract(trianglePt2, trianglePt1);
			var e1 = Subtract(trianglePt3, trianglePt1);

			var dir = Subtract(linePt2, linePt1);
			var dir_norm = dir.Normalize;

			var h = dir_norm * e1;
			double a = e0 | h;

			if (a > -TOL9 && a < TOL9)
			{
				return false;
			}

			var s = Subtract(linePt1, trianglePt1);
			double f = 1.0 / a;
			double u = f * (s | h);

			if (u < 0.0 || u > 1.0)
			{
				return false;
			}

			var q = s * e0;
			double v = f * (dir_norm | q);

			if (v < 0.0 || u + v > 1.0)
			{
				return false;
			}

			double t = f * (e1 | q);

			if (t > TOL9 && t < Math.Sqrt(dir | dir))
			{ // segment intersection
				intPoint = new Point3D(linePt1);
				MultiplyPoint3DByVector3D(intPoint, dir_norm, t, false);
				return true;
			}

			return false;

		}


		#endregion // Intersection


		#region CreateBeamMesh

		public static bool CreateBeamMesh(List<CI.Geometry3D.IVertexesTriangleMesh> triangleMesh,
			out IBeamMesh beamMesh)
		{
			beamMesh = null;

			if ((null == triangleMesh) || (triangleMesh.Count <= 0))
			{
				return false;
			}
			var verticesList = new List<IPoint3D>(3 * triangleMesh.Count);
			var indexTrianglesList = new List<IIndexTriangleMesh>(triangleMesh.Count);

			int i1 = 0;
			for (int i = 0; i < triangleMesh.Count; i++)
			{
				var item = triangleMesh[i];
				verticesList.Add(new Point3D(item.V1));
				verticesList.Add(new Point3D(item.V2));
				verticesList.Add(new Point3D(item.V3));

				indexTrianglesList.Add(new IndexTriangleMesh(i1, i1 + 1, i1 + 2));
				i1 = i1 + 3;
			}

			beamMesh = new BeamMesh(verticesList.ToArray(), indexTrianglesList.ToArray());

			return true;
		}


		#endregion // CreateBeamMesh


		#region Rectangular beam mesher


		public static bool CreateBeamMeshRec(IPoint3D pt1, IPoint3D pt2, IPoint3D pt4, IPoint3D pt5,
		out IBeamMesh beamMesh)
		{
			// check if point is inside cuboid
			//
			// Rec cross section defined by pt1-pt2 
			// pt1 - left down at start section
			// pt2 - rigth down at start section (define 1 side)
			// pt4 - left up at start section (define 2 side)
			// pt5 - left down at end section (define 3 side) - length of the prizm

			//double TOL9 = 0.00000001;

			beamMesh = null;

			var vi = Subtract(pt2, pt1);
			var vj = Subtract(pt4, pt1);
			var vk = Subtract(pt5, pt1);

			double viLen = vi.Magnitude;
			double vjLen = vj.Magnitude;
			double vkLen = vk.Magnitude;

			var viNorm = vi.Normalize;
			var vjNorm = vj.Normalize;
			var vkNorm = vk.Normalize;

			var verticesList = new List<IPoint3D>(8);

			verticesList.Add(new Point3D(pt1));
			verticesList.Add(new Point3D(pt2));
			var pt3 = new Point3D(pt2);
			MultiplyPoint3DByVector3D(pt3, vjNorm, vjLen, false);
			verticesList.Add(new Point3D(pt3));
			verticesList.Add(pt4);
			verticesList.Add(pt5);
			var pt6 = new Point3D(verticesList[1]);
			MultiplyPoint3DByVector3D(pt6, vkNorm, vkLen, false);
			verticesList.Add(new Point3D(pt6));
			var pt7 = new Point3D(verticesList[2]);
			MultiplyPoint3DByVector3D(pt7, vkNorm, vkLen, false);
			verticesList.Add(new Point3D(pt7));
			var pt8 = new Point3D(verticesList[3]);
			MultiplyPoint3DByVector3D(pt8, vkNorm, vkLen, false);
			verticesList.Add(new Point3D(pt8));

			int verticesLength = verticesList.Count;

			//  MESH: counter clockwise direction  ("Right-Hand-Rule")
			//int numTriangles = 12;
			var indexTrianglesList = new List<IIndexTriangleMesh>(12);

			int slices = 4;
			int v1;
			int v2;
			int v3;
			int v4;

			// start face
			int v10 = 0;
			int v11 = v10 + 1;
			int v12 = v10 + 2;
			for (int m = 0; m < slices - 2; m++)
			{
				indexTrianglesList.Add(new IndexTriangleMesh(v10, v11, v12));
				v11 = v12;
				v12++;
			}

			// sides
			int startVertexIndex = 0;
			for (int m = 0; m < slices; m++)
			{
				v1 = m + startVertexIndex;
				v2 = v1 + 1;
				if (m == slices - 1)
				{
					v2 = 0 + startVertexIndex;
				}
				v3 = v1 + slices;
				v4 = v2 + slices;
				indexTrianglesList.Add(new IndexTriangleMesh(v1, v3, v4));
				indexTrianglesList.Add(new IndexTriangleMesh(v1, v4, v2));
			}

			// end face
			v10 = verticesLength - slices;
			v11 = v10 + 1;
			v12 = v10 + 2;
			for (int m = 0; m < slices - 2; m++)
			{
				indexTrianglesList.Add(new IndexTriangleMesh(v10, v11, v12));
				v11 = v12;
				v12++;
			}

			beamMesh = new BeamMesh(verticesList.ToArray(), indexTrianglesList.ToArray());

			return true;
		}


		#endregion // Rectangular beam mesher


		#region Reinforcement3D


		public static bool IsReinforcementInsideBeamMesh(IBeamMesh beamMesh, IList<IPoint3D> rebarGeometry)
		{

			if (null == beamMesh)
			{
				return false;
			}
			if (null == rebarGeometry)
			{
				return false;
			}

			var mesh = new List<CI.Geometry3D.IVertexesTriangleMesh>(beamMesh.IndexTriangles.Length);

			for (int j = 0; j < beamMesh.IndexTriangles.Length; j++)
			{
				var Tpt1 = beamMesh.Vertices[beamMesh.IndexTriangles[j].V1];
				var Tpt2 = beamMesh.Vertices[beamMesh.IndexTriangles[j].V2];
				var Tpt3 = beamMesh.Vertices[beamMesh.IndexTriangles[j].V3];
				mesh.Add(new CI.Geometry3D.VertexesTriangleMesh(Tpt1, Tpt2, Tpt3));
			}

			return IsReinforcementInsideBeamMesh(mesh, rebarGeometry);
		}

			public static bool IsReinforcementInsideBeamMesh(IList<CI.Geometry3D.IVertexesTriangleMesh> beamMesh, IList<IPoint3D> rebarGeometry)
		{

			if (null == beamMesh)
			{
				return false;
			}
			if (null == rebarGeometry)
			{
				return false;
			}

			for (int i = 0; i < rebarGeometry.Count; i++)
			{
				for (int j = 0; j < beamMesh.Count; j++)
				{
					var Tpt1 = beamMesh[j].V1;
					var Tpt2 = beamMesh[j].V2;
					var Tpt3 = beamMesh[j].V3;

					var b1 = GeomOperation.IntersectionPointPerpendicularToTriangle3D(rebarGeometry[i],
						Tpt1, Tpt2, Tpt3,
						out IPoint3D intPt, out int normalDirection);
					//    Return: -1 = triangle is degenerate (a segment or point)
					//             0 = disjoint (no intersect)
					//             1 = intersect in unique point I1 (inside triangle)
					//             2 = are in the same plane
					//             3 = intersect in unique point I1 (outside the triangle)
					if ((b1 != 1) && (b1 != 3))
					{
						return false;
					}
					// normalDirection
					// = 0 lie on plane
					// = 1 point is on side of the normal (on front face)
					// = -1 point is on oposite side of the normal (on back face)
					if (normalDirection == 1)
					{
						return false;
					}
				}
			}
			return true;
		}


		public static bool IsReinforcementCrossingBeamMesh(IBeamMesh beamMesh, IList<IPoint3D> rebarGeometry)
		{

			if (null == beamMesh)
			{
				return false;
			}
			if (null == rebarGeometry)
			{
				return false;
			}

			var mesh = new List<CI.Geometry3D.IVertexesTriangleMesh>(beamMesh.IndexTriangles.Length);

			for (int j = 0; j < beamMesh.IndexTriangles.Length; j++)
			{
				var Tpt1 = beamMesh.Vertices[beamMesh.IndexTriangles[j].V1];
				var Tpt2 = beamMesh.Vertices[beamMesh.IndexTriangles[j].V2];
				var Tpt3 = beamMesh.Vertices[beamMesh.IndexTriangles[j].V3];
				mesh.Add(new CI.Geometry3D.VertexesTriangleMesh(Tpt1, Tpt2, Tpt3));
			}

			return IsReinforcementCrossingBeamMesh(mesh, rebarGeometry);
		}


		public static bool IsReinforcementCrossingBeamMesh(IList<CI.Geometry3D.IVertexesTriangleMesh> beamMesh, IList<IPoint3D> rebarGeometry)
		{
			if (null == beamMesh)
			{
				return false;
			}
			if (null == rebarGeometry)
			{
				return false;
			}
			for (int j = 0; j < beamMesh.Count; j++)
			{
				var Tpt1 = beamMesh[j].V1;
				var Tpt2 = beamMesh[j].V2;
				var Tpt3 = beamMesh[j].V3;
				for (int i = 0; i < rebarGeometry.Count - 1; i++)
				{
					var b2 = IntersectionLineTriangle(rebarGeometry[i], rebarGeometry[i + 1],
						Tpt1, Tpt2, Tpt3,
						out IPoint3D ptOut);
					if (b2)
					{
						// if exist any crossing;
						return true;
					}
				}
			}
			return false;
		}


		#endregion // Reinforcement3D


	}

}
 