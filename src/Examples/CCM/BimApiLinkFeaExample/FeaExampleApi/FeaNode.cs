using MathNet.Spatial.Euclidean;

namespace BimApiLinkFeaExample.FeaExampleApi
{
	public interface IFeaNode
	{
		int Id { get; }
		double X { get; }
		double Y { get; }
		double Z { get; }

		Point3D Point { get; }
	}

	internal class FeaNode : IFeaNode
	{
		public FeaNode(int id, double x, double y, double z)
		{
			Id = id;
			X = x;
			Y = y;
			Z = z;
		}

		public int Id { get; set; }

		public double X { get; set; }

		public double Y { get; set; }
		public double Z { get; set; }

		public Point3D Point => new Point3D(X, Y, Z);
	}
}
