using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace BimApiLinkCadExample.BimApi
{
	internal class BoltGrid : IdeaBoltGrid
	{
		public BoltGrid(Identifier<IIdeaBoltGrid> identifier) : base(identifier)
		{ }

		public int No { get; }

		public override IIdeaNode Origin => Get<IIdeaNode>(OriginNo);

		public string OriginNo { get; set; }

		public BoltGrid(int no)
			: base(no)
		{
			No = no;
		}
	}
}
