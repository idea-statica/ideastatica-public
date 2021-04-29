using IdeaStatiCa.BimApi;
using IdeaStatiCa.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter
{
	public class Geometry : IGeometry
	{
		private static readonly IIdeaLogger _logger = IdeaDiagnostics.GetLogger("ideastatica.bimimporter.geometry");
		private static readonly IIdeaObjectComparer _comparer = new IIdeaObjectComparer();

		private class Vertex
		{
			public HashSet<Edge> Edges { get; } = new HashSet<Edge>();
			public IIdeaNode Node { get; }

			public Vertex(IIdeaNode node)
			{
				Node = node;
			}
		}

		private class Edge
		{
			public HashSet<Vertex> Vertices { get; } = new HashSet<Vertex>();
			public IIdeaMember1D Member { get; }

			public Edge(IIdeaMember1D member)
			{
				Member = member;
			}

			public override bool Equals(object obj)
			{
				return obj is Edge edge && _comparer.Equals(Member, edge.Member);
			}

			public override int GetHashCode()
			{
				return _comparer.GetHashCode(Member);
			}
		}

		private readonly Dictionary<IIdeaNode, Vertex> _vertices = new Dictionary<IIdeaNode, Vertex>(_comparer);
		private readonly Dictionary<IIdeaMember1D, Edge> _edges = new Dictionary<IIdeaMember1D, Edge>(_comparer);

		public IEnumerable<IIdeaMember1D> GetConnectedMembers(IIdeaNode node)
		{
			if (_vertices.TryGetValue(node, out Vertex vertex))
			{
				return vertex.Edges.Select(x => x.Member);
			}

			_logger.LogWarning($"Node '{node}' was not found.");

			return Enumerable.Empty<IIdeaMember1D>();
		}

		public IEnumerable<IIdeaNode> GetNodesOnMember(IIdeaMember1D member)
		{
			if (_edges.TryGetValue(member, out Edge edge))
			{
				return edge.Vertices.Select(x => x.Node);
			}

			_logger.LogWarning($"Node '{edge}' was not found.");

			return Enumerable.Empty<IIdeaNode>();
		}

		public void Build(IIdeaModel model)
		{
			_vertices.Clear();
			_edges.Clear();

			_logger.LogInformation("Bulding model geometry.");

			foreach (IIdeaMember1D member in model.GetMembers())
			{
				Edge edge = new Edge(member);
				_edges.Add(member, edge);

				foreach (IIdeaNode node in EnumNodes(member))
				{
					Vertex vertex = GetOrCreateVertex(node);
					vertex.Edges.Add(edge);
					edge.Vertices.Add(vertex);
				}
			}

			_logger.LogInformation($"Got {_vertices.Count} vertices and {_edges.Count} edges.");
		}

		public IEnumerable<IIdeaMember1D> GetMembers() => _edges.Keys;

		public IEnumerable<IIdeaNode> GetNodes() => _vertices.Keys;

		private IEnumerable<IIdeaNode> EnumNodes(IIdeaMember1D member)
		{
			foreach (IIdeaElement1D element in member.Elements)
			{
				IIdeaSegment3D segment = element.Segment;

				yield return segment.StartNode;

				if (segment is IIdeaArcSegment3D arcSegment)
				{
					yield return arcSegment.ArcPoint;
				}

				yield return segment.EndNode;
			}
		}

		private Vertex GetOrCreateVertex(IIdeaNode node)
		{
			if (!_vertices.TryGetValue(node, out Vertex vertex))
			{
				vertex = new Vertex(node);
				_vertices.Add(node, vertex);
			}

			return vertex;
		}
	}
}