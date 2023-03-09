using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public class SingleSelection : Selection
	{
		public IIdeaConnectionPoint ConnectionPoint { get; }

		public SingleSelection(ISet<IIdeaNode> nodes, ISet<IIdeaMember1D> members, IIdeaConnectionPoint connectionPoint) : base(nodes, members)
		{
			ConnectionPoint = connectionPoint;
		}
	}
}
