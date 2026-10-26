using System.Collections.Generic;

namespace IdeaRstabPlugin.Geometry
{
	internal interface ILinesAndNodes
	{
		IEnumerable<int> GetConnectedMembers(int nodeNo);

		IEnumerable<int> GetNodesOnMember(int memberNo);

		void UpdateAll();

		void UpdateMembers(IEnumerable<int> members);
	}
}