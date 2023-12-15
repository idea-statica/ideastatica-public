using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;


namespace BimApiLinkCadExample.BimApi
{
	internal class BoltAssembly : IdeaBoltAssembly
	{
		public BoltAssembly(Identifier<IIdeaBoltAssembly> identifier) : base(identifier)
		{ }

		public BoltAssembly(string id) : base(id)
		{ }

		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialNo);

		public int MaterialNo { get; set; }
	}
}
