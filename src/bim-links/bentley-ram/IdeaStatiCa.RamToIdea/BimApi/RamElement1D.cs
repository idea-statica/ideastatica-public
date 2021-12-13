using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamElement1D : IIdeaElement1D
	{
		//TODO
		//private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.ram.bimapi");

		public IIdeaCrossSection StartCrossSection => _objectFactory.GetCrossSection(_startCssNo);

		public IIdeaCrossSection EndCrossSection
		{
			get
			{
				if (_endCssNo == 0)
				{
					return StartCrossSection;
				}
				else
				{
					return _objectFactory.GetCrossSection(_endCssNo);
				}
			}
		}

		public IdeaVector3D EccentricityBegin { get; set; } = new IdeaVector3D(0.0, 0.0, 0.0);

		public IdeaVector3D EccentricityEnd { get; set; } = new IdeaVector3D(0.0, 0.0, 0.0);

		public double RotationRx { get; }

		public IIdeaSegment3D Segment { get; }

		public string Id => "element-" + Name;

		public string Name { get; }

		private readonly IObjectFactory _objectFactory;
		private readonly int _startCssNo;
		private readonly int _endCssNo;

		public RamElement1D(IObjectFactory objectFactory, CoordSystem coordSystem, int startNodeNo, int endNodeNo,
			int startCrossSectionNo)
		{
			_objectFactory = objectFactory;
			_startCssNo = startCrossSectionNo;
			//_endCssNo = endCrossSectionNo;

			Segment = new RamLineSegment3D(objectFactory, coordSystem, startNodeNo, endNodeNo);
			RotationRx = 0.0;

			if (startNodeNo < endNodeNo)
			{
				Name = $"line-{startNodeNo}-{endNodeNo}";
			}
			else
			{
				Name = $"line-{endNodeNo}-{startNodeNo}";
			}

			//TODO
			//_logger.LogDebug($"Created {nameof(RamElement1D)} with id {Id}");
		}

		public IEnumerable<IIdeaResult> GetResults()
		{
			return Enumerable.Empty<IIdeaResult>();
		}
	}
}
