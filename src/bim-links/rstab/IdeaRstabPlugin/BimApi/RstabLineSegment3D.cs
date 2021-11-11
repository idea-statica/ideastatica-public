using IdeaRS.OpenModel.Geometry3D;
using IdeaRstabPlugin.Factories;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaRstabPlugin.BimApi
{
	/// <inheritdoc cref="IIdeaLineSegment3D"/>
	internal class RstabLineSegment3D : IIdeaLineSegment3D
	{
		private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

		public IIdeaNode StartNode => _objectFactory.GetNode(_startNodeNo);

		public IIdeaNode EndNode => _objectFactory.GetNode(_endNodeNo);

		public CoordSystem LocalCoordinateSystem { get; }

		public string Id => "segment-" + Name;

		public string Name { get; }

		private readonly IObjectFactory _objectFactory;
		private readonly int _startNodeNo;
		private readonly int _endNodeNo;

		public RstabLineSegment3D(IObjectFactory objectFactory, CoordSystem cs, int startNodeNo, int endNodeNo)
		{
			_objectFactory = objectFactory;

			_startNodeNo = startNodeNo;
			_endNodeNo = endNodeNo;

			if (startNodeNo < endNodeNo)
			{
				Name = $"line-{startNodeNo}-{endNodeNo}";
			}
			else
			{
				Name = $"line-{endNodeNo}-{startNodeNo}";
			}

			LocalCoordinateSystem = cs;

			_logger.LogDebug($"Created {nameof(RstabLineSegment3D)} with id {Id}");
		}
	}
}