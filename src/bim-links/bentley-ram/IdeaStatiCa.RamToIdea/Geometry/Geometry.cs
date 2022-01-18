using IdeaStatiCa.RamToIdea.BimApi;
using MathNet.Numerics;
using MathNet.Spatial.Euclidean;
using RAMDATAACCESSLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	internal class Geometry : IGeometry
	{
		private const double Precision = 1e-6;

		private readonly List<RamNode> _nodes = new List<RamNode>();
		private readonly List<Line> _lines = new List<Line>();

		public Line CreateLine(SCoordinate start, SCoordinate end, bool allowsIntermediateNodes)
		{
			Line line = new Line(
				GetOrCreateAt(start),
				GetOrCreateAt(end),
				allowsIntermediateNodes);
			_lines.Add(line);

			return line;
		}

		public void AddNode(INode node)
		{
			RamNode ramNode = new RamNode(node);
			_nodes.Add(ramNode);

			AddRamNodeToIntermediates(ramNode);
		}

		public void AddNodeToLine(Line line, SCoordinate position)
		{
			RamNode node = GetOrCreateAt(position.ToVector3D());

			if (line.Start == node || line.End == node)
			{
				return;
			}

			if (line.IntermediateNodes.Any(x => x.RamNode == node))
			{
				return;
			}

			double projection = GetRamNodeOnLineProjection(node, line);
			line.AddIntermediateNode(projection, node);
		}

		private RamNode GetOrCreateAt(SCoordinate position)
		{
			return GetOrCreateAt(position.ToVector3D());
		}

		private RamNode GetOrCreateAt(Vector3D vec)
		{
			RamNode resultNode = null;

			foreach (RamNode node in _nodes)
			{
				if (GetDistanceSquared(node.Position, vec) <= Precision)
				{
					resultNode = node;
					break;
				}
			}

			if (resultNode is null)
			{
				resultNode = new RamNode(vec);
				_nodes.Add(resultNode);

				AddRamNodeToIntermediates(resultNode);
			}

			return resultNode;
		}

		private double GetDistanceSquared(Vector3D a, Vector3D b)
		{
			Vector3D c = a - b;
			return c.DotProduct(c);
		}

		private void AddRamNodeToIntermediates(RamNode node)
		{
			foreach (Line line in _lines)
			{
				if (!line.AllowsIntermediateNodes)
				{
					continue;
				}

				if (GetRamNodeToLineDistance(node, line) > Precision)
				{
					continue;
				}

				double projection = GetRamNodeOnLineProjection(node, line);

				if (projection >= 0 && projection <= 1)
				{
					line.AddIntermediateNode(projection, node);
				}
			}
		}

		private double GetRamNodeToLineDistance(RamNode node, Line line)
		{
			Vector3D lineStartToRamNode = line.Start.Position - node.Position;
			double lineStartToRamNodeLength = lineStartToRamNode.Length;

			double cosTheta = lineStartToRamNode.DotProduct(line.Vector) / (lineStartToRamNodeLength * line.Vector.Length);

			if (Math.Abs(cosTheta).AlmostEqual(1.0, Precision))
			{
				return 0.0;
			}

			double sinTheta = Math.Sqrt(1.0 - cosTheta * cosTheta);
			return sinTheta * lineStartToRamNodeLength;
		}

		private double GetRamNodeOnLineProjection(RamNode node, Line line)
		{
			double length = line.Vector.Length;

			Vector3D lineStartToRamNode = node.Position - line.Start.Position;
			return line.Vector.Normalize().DotProduct(lineStartToRamNode) / length;
		}
	}
}