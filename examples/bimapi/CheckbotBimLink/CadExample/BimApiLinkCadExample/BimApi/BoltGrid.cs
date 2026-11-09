using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkCadExample.BimApi
{
	/// <summary>
	/// A bolt grid whose origin node is imported when BimApiLink reads it.
	/// </summary>
	internal class BoltGrid : IdeaBoltGrid
	{
		public BoltGrid(int id)
			: base(id)
		{
		}

		public string OriginNodeId { get; set; }

		public override IIdeaNode Origin => Get<IIdeaNode>(OriginNodeId);
	}
}
