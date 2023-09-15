using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApi;

namespace BimApiLinkCadExample.BimApi
{
	internal class Segment3D : IdeaLineSegment3D
	{
		public override IIdeaNode StartNode => Get<IIdeaNode>(StartNodeNo);

		public override IIdeaNode EndNode => Get<IIdeaNode>(EndNodeNo);

		public string StartNodeNo { get; set; }

		public string EndNodeNo { get; set; }

		public Segment3D(int no)
			: base(no)
		{
		}
	}
}