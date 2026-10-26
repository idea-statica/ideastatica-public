using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;


namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class AnchorGrid : IdeaAnchorGrid
	{
		public AnchorGrid(Identifier<IIdeaAnchorGrid> identifier) : base(identifier)
		{ }

		public string No { get; }

		public override IIdeaNode Origin => Get<IIdeaNode>(OriginNo);

		public string OriginNo { get; set; }

		public AnchorGrid(string no)
			: base(no)
		{
			No = no;
		}
	}
}
