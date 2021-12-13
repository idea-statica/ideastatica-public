using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	internal interface ILinesAndNodes
	{
		IEnumerable<int> GetConnectedMembers(int nodeNo);

		IEnumerable<int> GetNodesOnMember(int memberNo);

		void UpdateAll();

		void UpdateMembers(IEnumerable<int> members);
	}
}
