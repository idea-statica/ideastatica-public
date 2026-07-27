using Dlubal.RSTAB8;
using IdeaRS.OpenModel.Geometry3D;
using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Geometry;
using IdeaRstabPlugin.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using MathNet.Numerics;
using System.Collections.Generic;
using NMVector3D = MathNet.Spatial.Euclidean.Vector3D;
using UnitVector3D = MathNet.Spatial.Euclidean.UnitVector3D;

namespace IdeaRstabPlugin.Factories
{
	internal class ElementFactory : IElementFactory
	{
		private const double Tolerance = 1e-6;

		private static readonly IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.factories");

		private readonly IModelDataProvider _modelDataProvider;
		private readonly ILinesAndNodes _linesAndNodes;
		private readonly IObjectFactory _objectFactory;
		private readonly IImportSession _importSession;

		public ElementFactory(IModelDataProvider modelDataProvider, ILinesAndNodes linesAndNodes,
			IObjectFactory objectFactory, IImportSession importSession)
		{
			_modelDataProvider = modelDataProvider;
			_linesAndNodes = linesAndNodes;
			_objectFactory = objectFactory;
			_importSession = importSession;
		}

		public List<IIdeaElement1D> GetElements(int memberNo)
		{
			IMember member = _modelDataProvider.GetIMember(memberNo);
			Member memberData = _modelDataProvider.GetMember(memberNo);

			(CoordSystem lcs, double rotationRad) = GetLCSAndRotation(memberData);

			RstabElement1D element = new RstabElement1D(
				_objectFactory,
				lcs,
				memberData.StartNodeNo,
				memberData.EndNodeNo,
				memberData.StartCrossSectionNo,
				memberData.EndCrossSectionNo,
				rotationRad);

			return new List<IIdeaElement1D>() { element };
		}

		private (CoordSystem LCS, double Rotation) GetLCSAndRotation(Member memberData)
		{
			if (memberData.Rotation.Type == RotationType.HelpNode)
			{
				var lcs = GetLCSByPlane(memberData.Rotation, memberData.StartNodeNo, memberData.EndNodeNo);
				if (!(lcs is null))
				{
					return (lcs, 0);
				}
			}

			return (GetMemberLCS(memberData.StartNodeNo, memberData.EndNodeNo), GetRotation(memberData.Rotation));
		}

		private CoordSystem GetLCSByPlane(Rotation rotation, int startNodeNo, int endNodeNo)
		{
			if (rotation.Plane != PlaneType.PlaneXY && rotation.Plane != PlaneType.PlaneXZ)
			{
				return null;
			}

			NMVector3D helpNodePos = Vector2MNVector(_objectFactory.GetNode(rotation.HelpNodeNo).Vector);
			UnitVector3D axisX = GetAxisX(startNodeNo, endNodeNo);

			if (axisX.DotProduct(helpNodePos).AlmostEqual(1.0, 1e-6))
			{
				return null;
			}

			IIdeaNode startNode = _objectFactory.GetNode(startNodeNo);

			UnitVector3D axisY;
			UnitVector3D axisZ;

			if (rotation.Plane == PlaneType.PlaneXY)
			{
				axisY = (helpNodePos - Vector2MNVector(startNode.Vector)).Normalize();
				axisZ = axisX.CrossProduct(axisY);
				axisY = axisX.CrossProduct(axisZ).Negate();
			}
			//PlaneXZ
			else
			{
				axisZ = (helpNodePos - Vector2MNVector(startNode.Vector)).Normalize();
				axisY = axisX.CrossProduct(axisZ);
				axisZ = axisX.CrossProduct(axisY);
			}

			return new CoordSystemByVector()
			{
				VecX = MNVector2Vector(axisX),
				VecY = MNVector2Vector(axisY),
				VecZ = MNVector2Vector(axisZ),
			};
		}

		private double GetRotation(Rotation rotation)
		{
			if (rotation.Type == RotationType.Angle)
			{
				return rotation.Angle;
			}

			return 0.0;
		}

		private CoordSystem GetMemberLCS(int startNodeNo, int endNodeNo)
		{
			UnitVector3D axisX = GetAxisX(startNodeNo, endNodeNo);

			// #35544 RSTAB classifies member orientation against the coordinate planes with an angular
			// tolerance; a horizontal direction component below sin(tolAngle) must not steer the
			// LCS azimuth, otherwise a mm-scale nodal offset rotates the whole LCS of an
			// almost-vertical member.
			double snapTol = System.Math.Sqrt(2 * Tolerance);
			double refX = System.Math.Abs(axisX.X) < snapTol ? 0.0 : axisX.X;
			double refY = System.Math.Abs(axisX.Y) < snapTol ? 0.0 : axisX.Y;

			UnitVector3D axisZ, axisY;
			UnitVector3D axisXRef = axisX;
			
			// column behavior
			if (refX == 0.0 && refY == 0.0)
			{
				axisZ = axisX.Z > 0 ? UnitVector3D.XAxis.Negate() : UnitVector3D.XAxis;
			}
			else
			{
				axisZ = UnitVector3D.ZAxis;
				axisXRef = UnitVector3D.Create(refX, refY, axisX.Z);
			}		

			axisY = axisZ.CrossProduct(axisXRef);
			axisZ = axisX.CrossProduct(axisY);
			axisY = axisZ.CrossProduct(axisX);

			return new CoordSystemByVector()
			{
				VecX = MNVector2Vector(axisX),
				VecY = MNVector2Vector(axisY.Negate()),
				VecZ = MNVector2Vector(axisZ.Negate()),
			};
		}

		private UnitVector3D GetAxisX(int startNodeNo, int endNodeNo)
		{
			IIdeaNode startNode = _objectFactory.GetNode(startNodeNo);
			IIdeaNode endNode = _objectFactory.GetNode(endNodeNo);

			IdeaVector3D startNodePos = startNode.Vector;
			IdeaVector3D endNodePos = endNode.Vector;

			return UnitVector3D.Create(
					endNodePos.X - startNodePos.X,
					endNodePos.Y - startNodePos.Y,
					endNodePos.Z - startNodePos.Z);
		}

		private Vector3D MNVector2Vector(UnitVector3D vector3D)
		{
			return new Vector3D()
			{
				X = vector3D.X,
				Y = vector3D.Y,
				Z = vector3D.Z
			};
		}

		private NMVector3D Vector2MNVector(IdeaVector3D vector3D)
		{
			return new NMVector3D(vector3D.X, vector3D.Y, vector3D.Z);
		}		
	}
}