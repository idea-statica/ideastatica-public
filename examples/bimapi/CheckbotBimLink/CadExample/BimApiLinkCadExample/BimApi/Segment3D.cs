using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkCadExample.BimApi
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

		public string StartNodeId { get; set; }

		public string EndNodeId { get; set; }

		public override IIdeaNode StartNode => Get<IIdeaNode>(StartNodeId);

		public override IIdeaNode EndNode => Get<IIdeaNode>(EndNodeId);
	}
}
