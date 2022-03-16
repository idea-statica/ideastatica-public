using Dlubal.RSTAB8;
using IdeaRS.OpenModel.Geometry3D;
using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Geometry;
using IdeaRstabPlugin.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
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

			CoordSystem cs = GetMemberLCS(memberData);

			RstabElement1D element = new RstabElement1D(
				_objectFactory,
				cs,
				memberData.StartNodeNo,
				memberData.EndNodeNo,
				memberData.StartCrossSectionNo,
				memberData.EndCrossSectionNo)
			{
				EccentricityBegin = GetEccentricity(member, 0.0),
				EccentricityEnd = GetEccentricity(member, 1.0)
			};

			return new List<IIdeaElement1D>() { element };
		}

		private CoordSystem GetMemberLCS(Member memberData)
		{
			double rotationRad = Utils.ConvertRotation(memberData.Rotation, _objectFactory, memberData.StartNodeNo, memberData.EndNodeNo);
			return CreateLCS(memberData.StartNodeNo, memberData.EndNodeNo, rotationRad);
		}

		private CoordSystem CreateLCS(int startNodeNo, int endNodeNo, double rotation)
		{
			IIdeaNode startNode = _objectFactory.GetNode(startNodeNo);
			IIdeaNode endNode = _objectFactory.GetNode(endNodeNo);

			IdeaVector3D startNodePos = startNode.Vector;
			IdeaVector3D endNodePos = endNode.Vector;

			UnitVector3D axisX = UnitVector3D.Create(
				endNodePos.X - startNodePos.X,
				endNodePos.Y - startNodePos.Y,
				endNodePos.Z - startNodePos.Z);

			UnitVector3D axisZ, axisY;
			if (Math.Abs(axisX.Z).AlmostEqual(1.0, Tolerance))
			{
				axisZ = UnitVector3D.XAxis;
			}
			else
			{
				axisZ = UnitVector3D.ZAxis;
			}

			axisY = axisZ.CrossProduct(axisX).Rotate(axisX, MathNet.Spatial.Units.Angle.FromRadians(rotation));
			axisZ = axisX.CrossProduct(axisY);

			return new CoordSystemByVector()
			{
				VecX = MNVector2Vector(axisX),
				VecY = MNVector2Vector(axisY),
				VecZ = MNVector2Vector(axisZ),
			};
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

		private IdeaVector3D GetEccentricity(IMember member, double param)
		{
			Dlubal.RSTAB8.Point3D point = member.GetEccentricity(param);

			return new IdeaVector3D(point.X, -point.Y, -point.Z);
		}
	}
}