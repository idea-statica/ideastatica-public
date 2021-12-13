using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	internal class RamGeometry : IGeometry
	{
		private readonly ILinesAndNodes _linesAndNodes;
		private readonly IObjectFactory _objectFactory;

		public RamGeometry(ILinesAndNodes linesAndNodes, IObjectFactory objectFactory)
		{
			_linesAndNodes = linesAndNodes;
			_objectFactory = objectFactory;
		}

		public IEnumerable<IIdeaMember1D> GetConnectedMembers(IIdeaNode node)
		{
			if (!(node is RamNode ramNode))
			{
				throw new ArgumentException();
			}

			return _linesAndNodes
				.GetConnectedMembers(ramNode.No)
				.Select(x => _objectFactory.GetMember(x));
		}

		public IEnumerable<IIdeaNode> GetNodesOnMember(IIdeaMember1D member)
		{
			if (!(member is RamMember ramMember))
			{
				throw new ArgumentException();
			}

			return _linesAndNodes
				.GetNodesOnMember(ramMember.No)
				.Select(x => _objectFactory.GetNode(x));
		}
	}
}
