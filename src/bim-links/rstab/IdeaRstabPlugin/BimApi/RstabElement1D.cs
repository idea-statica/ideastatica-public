using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Model;
using IdeaRstabPlugin.Factories;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System.Collections.Generic;
using System.Linq;

namespace IdeaRstabPlugin.BimApi
{
	/// <inheritdoc cref="IIdeaElement1D"/>
	internal class RstabElement1D : IIdeaElement1D
	{
		private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

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

		public InsertionPoints CardinalPoint { get; set; }

		public EccentricityReference EccentricityReference { get; set; }

		public double RotationRx { get; }

		public IIdeaSegment3D Segment { get; }

		public string Id => "element-" + Name;

		public string Name { get; }

		private readonly IObjectFactory _objectFactory;
		private readonly int _startCssNo;
		private readonly int _endCssNo;

		public RstabElement1D(IObjectFactory objectFactory, CoordSystem coordSystem, int startNodeNo, int endNodeNo,
			int startCrossSectionNo, int endCrossSectionNo, double rotation)
		{
			_objectFactory = objectFactory;
			_startCssNo = startCrossSectionNo;
			_endCssNo = endCrossSectionNo;

			Segment = new RstabLineSegment3D(objectFactory, coordSystem, startNodeNo, endNodeNo);
			RotationRx = rotation;

			if (startNodeNo < endNodeNo)
			{
				Name = $"line-{startNodeNo}-{endNodeNo}";
			}
			else
			{
				Name = $"line-{endNodeNo}-{startNodeNo}";
			}

			_logger.LogDebug($"Created {nameof(RstabElement1D)} with id {Id}");
		}

		public IEnumerable<IIdeaResult> GetResults()
		{
			return Enumerable.Empty<IIdeaResult>();
		}
	}
}