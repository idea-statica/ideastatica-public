using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaMaterialSteel : AbstractIdeaObject<IIdeaMaterialSteel>, IIdeaMaterialSteel
	{
		public virtual IdeaRS.OpenModel.Material.MatSteel Material { get; set; } = null!;
		
		protected IdeaMaterialSteel(Identifier<IIdeaMaterialSteel> identifer)
			: base(identifer)
		{ }

		public IdeaMaterialSteel(int id)
			: this(new IntIdentifier<IIdeaMaterialSteel>(id))
		{ }

		public IdeaMaterialSteel(string id)
			: this(new StringIdentifier<IIdeaMaterialSteel>(id))
		{ }
	}
}
