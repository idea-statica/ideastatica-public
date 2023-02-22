using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaConcreteBlock : AbstractIdeaObject<IIdeaConcreteBlock>, IIdeaConcreteBlock
	{
		public IdeaConcreteBlock(Identifier<IIdeaConcreteBlock> identifier) : base(identifier)
		{ }

		public IdeaConcreteBlock(string id) : this(new StringIdentifier<IIdeaConcreteBlock>(id))
		{ }

		public double Lenght { get; set; }

		public double Width { get; set; }

		public double Height { get; set; }

		public virtual IIdeaMaterial Material { get; set; }
	}
}
