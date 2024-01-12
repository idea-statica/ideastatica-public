using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class Segment3D : IdeaLineSegment3D
	{
		public override IIdeaNode StartNode => Get<IIdeaNode>(StartNodeNo);

		public override IIdeaNode EndNode => Get<IIdeaNode>(EndNodeNo);

		public string StartNodeNo { get; set; }

		public string EndNodeNo { get; set; }

		public Segment3D(string no)
			: base(no)
		{
		}
	}
}
