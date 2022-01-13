using IdeaStatiCa.RamToIdea.BimApi;
using MathNet.Spatial.Euclidean;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	internal class Line
	{
		public RamNode Start { get; }

		public RamNode End { get; }

		public Vector3D Vector { get; }

		public IReadOnlyCollection<(double Position, RamNode RamNode)> IntermediateNodes => _intermediateNodes;

		private readonly List<(double, RamNode)> _intermediateNodes = new List<(double, RamNode)>();

		public Line(RamNode start, RamNode end)
		{
			Start = start;
			End = end;

			Vector = end.Position - start.Position;
		}

		public void AddIntermediateNode(double position, RamNode node)
		{
			_intermediateNodes.Add((position, node));
		}
	}
}