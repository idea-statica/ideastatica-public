using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class BoltAssembly : IdeaBoltAssembly
	{
		public BoltAssembly(Identifier<IIdeaBoltAssembly> identifier) : base(identifier)
		{ }

		public BoltAssembly(string id) : base(id)
		{ }

		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialNo);

		public string MaterialNo { get; set; }
	}
}
