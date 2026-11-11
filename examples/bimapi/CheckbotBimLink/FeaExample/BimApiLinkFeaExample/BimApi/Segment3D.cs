using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkFeaExample.BimApi
{
	/// <summary>
	/// A straight member axis whose end nodes are imported when BimApiLink reads them.
	/// </summary>
	internal class Segment3D : IdeaLineSegment3D
	{
		public Segment3D(int id)
			: base(id)
		{
		}

		public int StartNodeId { get; set; }

		public int EndNodeId { get; set; }

		public override IIdeaNode StartNode => Get<IIdeaNode>(StartNodeId);

		public override IIdeaNode EndNode => Get<IIdeaNode>(EndNodeId);
	}
}
