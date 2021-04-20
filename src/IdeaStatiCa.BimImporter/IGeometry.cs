using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// 
	/// </summary>
	public interface IGeometry
	{
		void Build(IIdeaModel model);

		IEnumerable<IIdeaMember1D> GetConnectedMembers(IIdeaNode node);

		IEnumerable<IIdeaNode> GetNodesOnMember(IIdeaMember1D member);
	}
}