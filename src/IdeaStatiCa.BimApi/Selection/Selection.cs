using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public abstract class Selection
	{
		public ISet<IIdeaNode> Nodes { get; }
		public ISet<IIdeaMember1D> Members { get; }

		protected Selection(ISet<IIdeaNode> nodes, ISet<IIdeaMember1D> members)
		{
			Nodes = nodes;
			Members = members;
		}
	}
}
