using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public class BulkSelection : Selection
	{
		public ISet<IIdeaConnectionPoint> ConnectionPoints { get; }

		public BulkSelection(ISet<IIdeaNode> nodes, ISet<IIdeaMember1D> members, ISet<IIdeaConnectionPoint> connectionPoints) : base(nodes, members)
		{
			ConnectionPoints = connectionPoints;
		}
	}
}
