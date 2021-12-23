using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using MathNet.Numerics;
using MathNet.Spatial.Euclidean;
using System.Numerics;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	/// <inheritdoc cref="IIdeaLineSegment3D"/>
	internal class RamLineSegment3D : IIdeaLineSegment3D
	{
		private const double Tolerance = 1e-6;

		//TODO
		//private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.ramss.bimapi");

		public IIdeaNode StartNode { get; set; }

		public IIdeaNode EndNode { get; set; }

		public CoordSystem LocalCoordinateSystem => CalculateLCS();

		public string Id => $"segment-{MemberUID}";

		public string Name { get; }

		public int MemberUID { get; set; }

		private CoordSystem CalculateLCS()
		{
			var startNodeVec = StartNode.Vector;
			var endNodeVec = EndNode.Vector;

			double x = endNodeVec.X - startNodeVec.X;
			double y = endNodeVec.Y - startNodeVec.Y;
			double z = endNodeVec.Z - startNodeVec.Z;

			UnitVector3D zBasis;
			UnitVector3D vecX = UnitVector3D.Create(x, y, z);

			if(vecX.Z.AlmostEqual(1, 1e-6))
			{
				zBasis = UnitVector3D.Create(0, 1, 0);
			}
			else if (vecX.Z.AlmostEqual(-1, 1e-6))
			{
				zBasis = UnitVector3D.Create(0, -1, 0);
			}
			else
			{
				zBasis = UnitVector3D.Create(0, 0, 1);
			}

			UnitVector3D vecY = vecX.CrossProduct(zBasis);
			UnitVector3D vecZ = vecX.CrossProduct(vecY);

			return new CoordSystemByVector()
			{
				VecX = ConvertVector(vecX),
				VecY = ConvertVector(vecY),
				VecZ = ConvertVector(vecZ),
			};
		}

		private IdeaRS.OpenModel.Geometry3D.Vector3D ConvertVector(UnitVector3D vec)
		{
			return new IdeaRS.OpenModel.Geometry3D.Vector3D()
			{
				X = vec.X,
				Y = vec.Y,
				Z = vec.Z
			};
		}
	}
}