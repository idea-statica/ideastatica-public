using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	/// <inheritdoc cref="IIdeaLineSegment3D"/>
	internal class RamLineSegment3D : IIdeaLineSegment3D
	{
		private const double Tolerance = 1e-6;

		//TODO
		//private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.ramss.bimapi");

		public IIdeaNode StartNode { get; }

		public IIdeaNode EndNode { get; }

		public CoordSystem LocalCoordinateSystem { get; }

		public string Id => $"segment-{StartNode.Id}-{EndNode.Id}";

		public string Name { get; }

		public RamLineSegment3D(IIdeaNode startNode, IIdeaNode endNode, CoordSystem localCoordinateSystem)
		{
			StartNode = startNode;
			EndNode = endNode;
			LocalCoordinateSystem = localCoordinateSystem;
		}
	}
}