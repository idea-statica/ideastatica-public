using IdeaStatica.BimApiLink.BimApi;
using IdeaStatiCa.BimApi;

namespace BimApiExample.Plugin.BimApi
{
	internal class Segment3D : IdeaLineSegment3D
	{
		public override IIdeaNode StartNode => Get<IIdeaNode>(StartNodeNo);

		public override IIdeaNode EndNode => Get<IIdeaNode>(EndNodeNo);

		public int StartNodeNo { get; init; }

		public int EndNodeNo { get; init; }

		public Segment3D(int no)
			: base(no)
		{
		}
	}
}