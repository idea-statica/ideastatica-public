using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Factories;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaRstabPlugin.Geometry
{
	internal class RstabGeometry : IGeometry
	{
		private readonly ILinesAndNodes _linesAndNodes;
		private readonly IObjectFactory _objectFactory;

		public RstabGeometry(ILinesAndNodes linesAndNodes, IObjectFactory objectFactory)
		{
			_linesAndNodes = linesAndNodes;
			_objectFactory = objectFactory;
		}

		public IEnumerable<IIdeaMember1D> GetConnectedMembers(IIdeaNode node)
		{
			if (!(node is RstabNode rstabNode))
			{
				throw new ArgumentException();
			}

			return _linesAndNodes
				.GetConnectedMembers(rstabNode.No)
				.Select(x => _objectFactory.GetMember(x));
		}

		public IEnumerable<IIdeaNode> GetNodesOnMember(IIdeaMember1D member)
		{
			if (!(member is RstabMember rstabMember))
			{
				throw new ArgumentException();
			}

			return _linesAndNodes
				.GetNodesOnMember(rstabMember.No)
				.Select(x => _objectFactory.GetNode(x));
		}
	}
}