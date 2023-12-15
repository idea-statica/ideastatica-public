using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApi;

namespace BimApiLinkFeaExample.BimApi
{
	internal class Segment3D : IdeaLineSegment3D
	{
		public override IIdeaNode StartNode => Get<IIdeaNode>(StartNodeNo);

		public override IIdeaNode EndNode => Get<IIdeaNode>(EndNodeNo);

		public int StartNodeNo { get; set; }

		public int EndNodeNo { get; set; }

		public Segment3D(int no)
			: base(no)
		{
		}
	}
}