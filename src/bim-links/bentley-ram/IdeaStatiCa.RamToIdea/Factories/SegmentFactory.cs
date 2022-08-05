using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Geometry;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal class SegmentFactory : ISegmentFactory
	{
		public List<RamLineSegment3D> CreateSegments(Line line)
		{
			List<RamLineSegment3D> segments = new List<RamLineSegment3D>();

			RamNode prevNode = line.Start;

			if (line.AllowsIntermediateNodes)
			{
				IEnumerable<(double Position, RamNode RamNode)> nodes = line.IntermediateNodes
					.OrderBy(x => x.Position)
					.Distinct(new PositionNodeEqualityComparer());

				foreach ((_, RamNode node) in nodes)
				{
					segments.Add(CreateSegment(prevNode, node, line.LCS));
					prevNode = node;
				}
			}

			segments.Add(CreateSegment(prevNode, line.End, line.LCS));

			return segments;
		}

		private RamLineSegment3D CreateSegment(RamNode start, RamNode end, IdeaRS.OpenModel.Geometry3D.CoordSystem lcs)
		{
			return new RamLineSegment3D(start, end, lcs);
		}

		private class PositionNodeEqualityComparer : IEqualityComparer<(double Position, RamNode RamNode)>
		{
			private const double Tolerance = 1e-6;

			public bool Equals((double Position, RamNode RamNode) x, (double Position, RamNode RamNode) y)
			{
				return x.Position.AlmostEqual(y.Position, Tolerance);
			}

			public int GetHashCode((double Position, RamNode RamNode) obj)
			{
				return Math.Round(obj.Position, 3).GetHashCode();
			}
		}
	}
}