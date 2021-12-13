using RAMDATAACCESSLib;
using IdeaStatiCa.RamToIdea.Providers;
using IdeaStatiCa.RamToIdea.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	internal class LinesAndNodes : ILinesAndNodes
	{
		private readonly IModelDataProvider _modelDataProvider;

		// (node, line)
		private readonly PairStorage<int, int> _nodeLine = new PairStorage<int, int>();

		// (member, line)
		private readonly PairStorage<int, int> _memberLine = new PairStorage<int, int>();

		public LinesAndNodes(IModelDataProvider dataProvider)
		{
			_modelDataProvider = dataProvider;
		}

		public void UpdateMembers(IEnumerable<int> members)
		{
			UpdateMembers(members.Select(x => _modelDataProvider.GetMember(x)));
		}

		public void UpdateAll()
		{
			_nodeLine.Clear();
			_memberLine.Clear();
			UpdateMembers(_modelDataProvider.GetMembers());
		}

		public IEnumerable<int> GetConnectedMembers(int nodeNo)
		{
			return _nodeLine
				.GetRights(nodeNo)
				.SelectMany(x => _memberLine.GetLefts(x));
		}

		public IEnumerable<int> GetNodesOnMember(int memberNo)
		{
			int lineNo = _memberLine
				.GetRights(memberNo)
				.First();

			return _nodeLine
				.GetLefts(lineNo);
		}

		private void UpdateMembers(IEnumerable<Member> members)
		{
			HashSet<int> addedNodes = new HashSet<int>();

			foreach (Member member in members)
			{
				_memberLine.RemoveLeft(member.No);
				_memberLine.Add(member.No, member.No);

				_nodeLine.RemoveRight(member.No);

				List<Node> nodes = new List<Node>();
				nodes.Add(_modelDataProvider.GetNode(member.StartNodeNo));
				nodes.Add(_modelDataProvider.GetNode(member.EndNodeNo));

				_nodeLine.Add(nodes.Select(x => x.No), member.No);

				foreach (Node node in nodes)
				{
					addedNodes.Add(node.No);
				}
			}
		}
	}
}
