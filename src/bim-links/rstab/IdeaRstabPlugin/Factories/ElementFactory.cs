using Dlubal.RSTAB8;
using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Geometry;
using IdeaRstabPlugin.Providers;
using IdeaRstabPlugin.Utilities;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Diagnostics;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Factories
{
	internal class ElementFactory : IElementFactory
	{
		private readonly static IIdeaLogger _logger = IdeaDiagnostics.GetLogger("bim.rstab.factories");

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
			//IList<int> nodes = new List<int>();
			//nodes.Add(memberData.StartNodeNo);
			//nodes.Add(memberData.EndNodeNo);

			CoordSystem cs = ConvertLCS(member.GetLocalCoordinateSystem(0.0));

			List<IIdeaElement1D> elements = new List<IIdeaElement1D>();
			//foreach ((int startNodeNo, int endNodeNo) in _linesAndNodes.GetNodesOnMember(memberNo).SelectPairs((x, y) => (x, y)))
			{
				RstabElement1D element = new RstabElement1D(
					_objectFactory,
					cs,
					memberData.StartNodeNo,
					memberData.EndNodeNo,
					memberData.StartCrossSectionNo,
					memberData.EndCrossSectionNo);
				elements.Add(element);
			}

			((RstabElement1D)elements[0]).EccentricityBegin = GetEccentricity(member, 0.0);

			// End eccentricity is already "included" in LCS
			//((RSTABElement1D)elements[elements.Count - 1]).EccentricityEnd = GetEccentricity(member, 1.0);

			return elements;
		}

		private CoordSystem ConvertLCS(ICoordinateSystem cs)
		{
			CoordinateSystem csData = cs.GetData();

			return new CoordSystemByVector()
			{
				VecX = Point2Vector(csData.AxisX),
				VecY = Point2Vector(csData.AxisY),
				VecZ = Point2Vector(csData.AxisZ),
			};
		}

		private Vector3D Point2Vector(Dlubal.RSTAB8.Point3D point)
		{
			if (_importSession.IsGCSOrientedUpwards)
			{
				return new Vector3D
				{
					X = point.X,
					Y = point.Y,
					Z = point.Z,
				};
			}
			else
			{
				return new Vector3D
				{
					X = point.X,
					Y = -point.Y,
					Z = -point.Z,
				};
			}
		}

		private IdeaVector3D GetEccentricity(IMember member, double param)
		{
			Dlubal.RSTAB8.Point3D point = member.GetEccentricity(param);
			if (_importSession.IsGCSOrientedUpwards)
			{
				return new IdeaVector3D(point.X, point.Y, point.Z);
			}
			else
			{
				return new IdeaVector3D(point.X, -point.Y, -point.Z);
			}
		}
	}
}