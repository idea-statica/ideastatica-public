using IdeaStatiCa.BimApi;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter
{
	internal class GeometryGraph : IGeometryGraph
	{
		private class Vertex
		{
			public List<Edge> Edges { get; } = new List<Edge>();
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
		}

		private readonly Dictionary<IIdeaNode, Vertex> _vertices = new Dictionary<IIdeaNode, Vertex>(new IIdeaObjectComparer());
		private readonly Dictionary<IIdeaMember1D, Edge> _edges = new Dictionary<IIdeaMember1D, Edge>(new IIdeaObjectComparer());

		public GeometryGraph(ISet<IIdeaMember1D> members)
		{
			Build(members);
		}

		public IEnumerable<IIdeaMember1D> GetConnectedMembers(IIdeaNode node)
		{
			return _vertices[node].Edges.Select(x => x.Member);
		}

		public IEnumerable<IIdeaNode> GetNodesOnMember(IIdeaMember1D member)
		{
			return _edges[member].Vertices.Select(x => x.Node);
		}

		private void Build(ISet<IIdeaMember1D> members)
		{
			foreach (IIdeaMember1D member in members)
			{
				Edge edge = new Edge(member);
				_edges.Add(member, edge);

				foreach (IIdeaNode node in EnumNodes(member))
				{
					Vertex vertex = GetOrCreateVertex(node);
					edge.Vertices.Add(vertex);
				}
			}
		}

		private IEnumerable<IIdeaNode> EnumNodes(IIdeaMember1D member)
		{
			foreach (IIdeaElement1D element in member.Elements)
			{
				yield return element.Segment.StartNode;

				if (element.Segment is IIdeaArcSegment3D arcSegment)
				{
					yield return arcSegment.ArcPoint;
				}

				yield return element.Segment.EndNode;
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