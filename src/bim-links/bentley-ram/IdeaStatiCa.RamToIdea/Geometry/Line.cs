using IdeaStatiCa.RamToIdea.BimApi;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	internal class Line
	{
		public RamNode Start { get; }

		public RamNode End { get; }

		public Vector3D Vector { get; }

		public bool AllowsIntermediateNodes => !(_intermediateNodes is null);

		public IReadOnlyCollection<(double Position, RamNode RamNode)> IntermediateNodes => _intermediateNodes;

		private readonly List<(double, RamNode)> _intermediateNodes = new List<(double, RamNode)>();

		public Line(RamNode start, RamNode end, bool allowsIntermediateNode)
		{
			Start = start;
			End = end;

			Vector = end.Position - start.Position;

			if (!allowsIntermediateNode)
			{
				_intermediateNodes = null;
			}
		}

		public void AddIntermediateNode(double position, RamNode node)
		{
			if (_intermediateNodes is null)
			{
				throw new InvalidOperationException();
			}

			_intermediateNodes.Add((position, node));
		}
	}
}