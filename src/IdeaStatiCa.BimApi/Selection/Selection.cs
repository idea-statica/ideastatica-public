using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public abstract class Selection
	{
		public ISet<IIdeaNode> Nodes { get; }
		public ISet<IIdeaMember1D> Members { get; }
		public ISet<IIdeaMember2D> Members2D { get; }

		protected Selection(ISet<IIdeaNode> nodes, ISet<IIdeaMember1D> members, ISet<IIdeaMember2D> members2D)
		{
			Nodes = nodes;
			Members = members;
			Members2D = members2D;
		}
	}
}
