using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public class SingleSelection : Selection
	{
		public IIdeaConnectionPoint ConnectionPoint { get; }

		public SingleSelection(ISet<IIdeaNode> nodes, ISet<IIdeaMember1D> members, IIdeaConnectionPoint connectionPoint = null, ISet<IIdeaMember2D> members2D = null) : base(nodes, members, members2D)
		{
			ConnectionPoint = connectionPoint;
		}
	}
}
