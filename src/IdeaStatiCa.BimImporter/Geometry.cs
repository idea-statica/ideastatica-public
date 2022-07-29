using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter
{
	/// <inheritdoc cref="IGeometry"/>
	public class Geometry : IGeometry
	{
		private static readonly IIdeaObjectComparer _comparer = new IIdeaObjectComparer();

		private sealed class Vertex
		{
			public HashSet<Edge> Edges { get; } = new HashSet<Edge>();
			public IIdeaNode Node { get; }

			public Vertex(IIdeaNode node)
			{
				Node = node;
			}
		}

		private sealed class Edge
		{
			public HashSet<Vertex> Vertices { get; } = new HashSet<Vertex>();
			public IIdeaMember1D Member { get; }

			public Edge(IIdeaMember1D member)
			{
				Member = member;
			}
		}

		private readonly Dictionary<IIdeaNode, Vertex> _vertices = new Dictionary<IIdeaNode, Vertex>(_comparer);
		private readonly Dictionary<IIdeaMember1D, Edge> _edges = new Dictionary<IIdeaMember1D, Edge>(_comparer);
		private readonly IPluginLogger _logger;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <exception cref="ArgumentNullException">If some argument is null.</exception>
		public Geometry(IPluginLogger logger, IIdeaModel model)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Build(model ?? throw new ArgumentNullException(nameof(model)));
		}

		/// <inheritdoc cref="IGeometry.GetConnectedMembers(IIdeaNode)"/>
		/// <exception cref="ArgumentNullException">If <paramref name="node"/> is null.</exception>
		public IEnumerable<IIdeaMember1D> GetConnectedMembers(IIdeaNode node)
		{
			if (node is null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			if (_vertices.TryGetValue(node, out Vertex vertex))
			{
				return vertex.Edges.Select(x => x.Member);
			}

			_logger.LogWarning($"Node '{node}' was not found.");
			throw new ArgumentException($"Unknown node with id {node.Id}.", nameof(node));
		}

		/// <inheritdoc cref="IGeometry.GetNodesOnMember(IIdeaMember1D)"/>
		/// <exception cref="ArgumentNullException">If <paramref name="member"/> is null.</exception>
		public IEnumerable<IIdeaNode> GetNodesOnMember(IIdeaMember1D member)
		{
			if (member is null)
			{
				throw new ArgumentNullException(nameof(member));
			}

			if (_edges.TryGetValue(member, out Edge edge))
			{
				return edge.Vertices.Select(x => x.Node);
			}

			_logger.LogWarning($"Member '{edge}' was not found.");
			throw new ArgumentException($"Unknown member with id {member.Id}.", nameof(member));
		}

		private void Build(IIdeaModel model)
		{
			_logger.LogInformation("Building model geometry.");

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