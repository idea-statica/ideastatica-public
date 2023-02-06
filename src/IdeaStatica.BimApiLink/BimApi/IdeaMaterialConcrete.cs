using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaMaterialConcrete : AbstractIdeaObject<IIdeaMaterialConcrete>, IIdeaMaterialConcrete
	{
		public virtual IdeaRS.OpenModel.Material.MatConcrete Material { get; set; } = null;

		public IdeaMaterialConcrete(Identifier<IIdeaMaterialConcrete> identifer)
			: base(identifer)
		{ }

		public IdeaMaterialConcrete(int id)
			: this(new IntIdentifier<IIdeaMaterialConcrete>(id))
		{ }

		public IdeaMaterialConcrete(string id)
			: this(new StringIdentifier<IIdeaMaterialConcrete>(id))
		{ }
	}
}
