using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaMaterialByName : AbstractIdeaObject<IIdeaMaterialByName>, IIdeaMaterialByName
	{
		public virtual MaterialType MaterialType { get; set; }
		
		protected IdeaMaterialByName(Identifier<IIdeaMaterialByName> identifer)
			: base(identifer)
		{ }

		public IdeaMaterialByName(int id)
			: this(new IntIdentifier<IIdeaMaterialByName>(id))
		{ }

		public IdeaMaterialByName(string id)
			: this(new StringIdentifier<IIdeaMaterialByName>(id))
		{ }
	}
}
