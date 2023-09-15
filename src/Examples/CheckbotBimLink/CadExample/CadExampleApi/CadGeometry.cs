using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BimApiLinkCadExample.CadExampleApi
{
	public class CadPoint3D
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public CadPoint3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}

	public class CadPoint2D
	{
		public double X { get; set; }
		public double Y { get; set; }

		public CadPoint2D(double x, double y)
		{
			X = x;
			Y = y;
		}

		public static CadPoint3D Get2DPointInWorldCoords(CadPoint2D point2D, CadPlane3D plane)
		{

			// Create a translation matrix to move the 2D point to the global XY plane
			var translationMatrix = DenseMatrix.OfArray(new double[,] {
			{ plane.X.X, plane.Y.X, plane.Z.X },
			{ plane.X.Y, plane.Y.Y, plane.Z.Y },
			{ plane.X.Z, plane.Y.Z, plane.Z.Z }
				});

			// Create a 2D vector from the 2D point
			var vector2D = Vector<double>.Build.DenseOfArray(new double[] { point2D.X, point2D.Y, 0 });

			// Apply the translation matrix to the 2D vector to obtain the 3D vector
			var vector3D = translationMatrix * vector2D;

			// Add the origin point of the plane to the 3D vector to get the final 3D point
			vector3D[0] += plane.Origin.X;
			vector3D[1] += plane.Origin.Y;
			vector3D[2] += plane.Origin.Z;

			// Extract the X, Y, and Z coordinates from the resulting 3D vector
			double x3D = vector3D[0];
			double y3D = vector3D[1];
			double z3D = vector3D[2];

			return new CadPoint3D(x3D, y3D, z3D);
		}
	}

	public class CadVector3D
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }

		public CadVector3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}
	}

	public class CadPlane3D
	{
		public CadPoint3D Origin { get; set; }
		public CadVector3D X { get; set; }
		public CadVector3D Y { get; set; }
		public CadVector3D Z { get; set; }

		public CadPlane3D(CadPoint3D origin, CadVector3D x, CadVector3D y, CadVector3D z)
		{
			Origin = origin;
			X = x;
			Y = y;
			Z = z;
		}
	}

	public class CadOutline2D
	{
		public CadPlane3D Plane { get; set; }
		public List<CadPoint2D> Points { get; set; }

		public CadOutline2D(CadPlane3D plane, List<CadPoint2D> polyLinePoints)
		{
			Plane = plane;
			Points = polyLinePoints;
		}

		public void AddPoint(CadPoint2D point)
		{
			Points.Add(point);
		}
	}

}
