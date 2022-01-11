using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using MathNet.Spatial.Euclidean;
using System;

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
			IdeaVector3D startNodeVec = StartNode.Vector;
			IdeaVector3D endNodeVec = EndNode.Vector;

			double x = endNodeVec.X - startNodeVec.X;
			double y = endNodeVec.Y - startNodeVec.Y;
			double z = endNodeVec.Z - startNodeVec.Z;

			UnitVector3D axisX = UnitVector3D.Create(x, y, z);
			UnitVector3D axisY = GetNormalVector(axisX);
			UnitVector3D axisZ = axisX.CrossProduct(axisY);

			return new CoordSystemByVector()
			{
				VecX = ConvertVector(axisX),
				VecY = ConvertVector(axisY),
				VecZ = ConvertVector(axisZ),
			};
		}

		private UnitVector3D GetNormalVector(UnitVector3D directionVector)
		{
			if (Math.Abs(directionVector.Z) < 1e-6)
			{
				return UnitVector3D.Create(0, 0, 1.0);
			}

			if (directionVector.Z < 0)
			{
				directionVector = directionVector.Negate();
			}

			UnitVector3D locationVectorXYProj = UnitVector3D.Create(directionVector.X, directionVector.Y, 0);
			UnitVector3D axisY = locationVectorXYProj.CrossProduct(directionVector);

			return axisY.CrossProduct(directionVector);
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