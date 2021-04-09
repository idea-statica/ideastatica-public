using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	internal interface IGeometryGraph
	{
		IEnumerable<IIdeaMember1D> GetConnectedMembers(IIdeaNode node);

		IEnumerable<IIdeaNode> GetNodesOnMember(IIdeaMember1D member);
	}
}