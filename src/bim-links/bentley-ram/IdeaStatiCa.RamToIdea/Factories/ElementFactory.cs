using RAMDATAACCESSLib;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Geometry;
using IdeaStatiCa.RamToIdea.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal class ElementFactory : IElementFactory
	{
		//TODO
		//private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.factories");

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
			//IMember member = _modelDataProvider.GetIMember(memberNo);
			IMember member = _modelDataProvider.GetMember(memberNo);

			//beam.get

			//IList<int> nodes = new List<int>();
			//nodes.Add(memberData.StartNodeNo);
			//nodes.Add(memberData.EndNodeNo);

			CoordSystem cs = member.GetCoordSystem(); //ConvertLCS(member.GetLocalCoordinateSystem(0.0));
			
			//TODO Co-ordinate System


			List<IIdeaElement1D> elements = new List<IIdeaElement1D>();
			
			//foreach ((int startNodeNo, int endNodeNo) in _linesAndNodes.GetNodesOnMember(memberNo).SelectPairs((x, y) => (x, y)))
			{
				RamElement1D element = new RamElement1D(
					_objectFactory,
					cs,
					member.StartNodeUID,
					member.EndNodeUID,
					member.CrossSectionId);
					//member.EndCrossSectionNo);
				elements.Add(element);
			}

			//((RamElement1D)elements[0]).EccentricityBegin = GetEccentricity(member, 0.0);

			// End eccentricity is already "included" in LCS
			//((RSTABElement1D)elements[elements.Count - 1]).EccentricityEnd = GetEccentricity(member, 1.0);

			return elements;
		}

		//private CoordSystem ConvertLCS(ICoordinateSystem cs)
		//{
		//	CoordinateSystem csData = cs.GetData();

		//	return new CoordSystemByVector()
		//	{
		//		VecX = Point2Vector(csData.AxisX),
		//		VecY = Point2Vector(csData.AxisY),
		//		VecZ = Point2Vector(csData.AxisZ),
		//	};
		//}

		//private Vector3D Point2Vector(Dlubal.RSTAB8.Point3D point)
		//{
		//	if (_importSession.IsGCSOrientedUpwards)
		//	{
		//		return new Vector3D
		//		{
		//			X = point.X,
		//			Y = point.Y,
		//			Z = point.Z,
		//		};
		//	}
		//	else
		//	{
		//		return new Vector3D
		//		{
		//			X = point.X,
		//			Y = -point.Y,
		//			Z = -point.Z,
		//		};
		//	}
		//}

		//private IdeaVector3D GetEccentricity(IMember member, double param)
		//{
		//	Dlubal.RSTAB8.Point3D point = member.GetEccentricity(param);
		//	if (_importSession.IsGCSOrientedUpwards)
		//	{
		//		return new IdeaVector3D(point.X, point.Y, point.Z);
		//	}
		//	else
		//	{
		//		return new IdeaVector3D(point.X, -point.Y, -point.Z);
		//	}
		//}
	}
}
