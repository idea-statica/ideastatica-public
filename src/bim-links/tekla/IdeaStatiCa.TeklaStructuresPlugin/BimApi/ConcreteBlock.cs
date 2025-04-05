using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	public class ConcreteBlock : IdeaConcreteBlock
	{
		public ConcreteBlock(Identifier<IIdeaConcreteBlock> identifier) : base(identifier)
		{
		}

		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialNo);

		public string MaterialNo { get; set; }

		public string No { get; }

		public ConcreteBlock(string no)
			: base(no)
		{
			No = no;
		}
	}
}
