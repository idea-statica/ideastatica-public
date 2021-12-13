using System;
using IdeaStatiCa.BimApi;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	/// <inheritdoc cref="IIdeaLineSegment3D"/>
	internal class RamLineSegment3D : IIdeaLineSegment3D
	{
		//TODO
		//private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.ramss.bimapi");

		public IIdeaNode StartNode => _objectFactory.GetNode(_startNodeNo);

		public IIdeaNode EndNode => _objectFactory.GetNode(_endNodeNo);

		public CoordSystem LocalCoordinateSystem { get; }

		public string Id => "segment-" + Name;

		public string Name { get; }

		private readonly IObjectFactory _objectFactory;
		private readonly int _startNodeNo;
		private readonly int _endNodeNo;

		public RamLineSegment3D(IObjectFactory objectFactory, CoordSystem cs, int startNodeNo, int endNodeNo)
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

			//TODO
			//_logger.LogDebug($"Created {nameof(RamLineSegment3D)} with id {Id}");
		}
	}
}
