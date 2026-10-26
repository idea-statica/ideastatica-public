using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;


namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class BoltGrid : IdeaBoltGrid
	{
		public BoltGrid(Identifier<IIdeaBoltGrid> identifier) : base(identifier)
		{ }

		public string No { get; }

		public override IIdeaNode Origin => Get<IIdeaNode>(OriginNo);

		public string OriginNo { get; set; }

		public BoltGrid(string no)
			: base(no)
		{
			No = no;
		}
	}
}
