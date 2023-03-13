using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public class BulkSelection : Selection
	{
		public ISet<IIdeaConnectionPoint> ConnectionPoints { get; }

		public BulkSelection(ISet<IIdeaNode> nodes, ISet<IIdeaMember1D> members, ISet<IIdeaConnectionPoint> connectionPoints = null, ISet<IIdeaMember2D> members2D = null) : base(nodes, members, members2D)
		{
			ConnectionPoints = connectionPoints;
		}
	}
}
