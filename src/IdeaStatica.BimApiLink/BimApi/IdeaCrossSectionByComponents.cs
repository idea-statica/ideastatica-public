using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaCrossSectionByComponents : AbstractIdeaObject<IIdeaCrossSectionByComponents>, IIdeaCrossSectionByComponents
	{
		public virtual double Rotation { get; set; }
		
		public virtual HashSet<IIdeaCrossSectionComponent> Components { get; set; } = null!;
		
		protected IdeaCrossSectionByComponents(Identifier<IIdeaCrossSectionByComponents> identifer)
			: base(identifer)
		{ }

		public IdeaCrossSectionByComponents(int id)
			: this(new IntIdentifier<IIdeaCrossSectionByComponents>(id))
		{ }

		public IdeaCrossSectionByComponents(string id)
			: this(new StringIdentifier<IIdeaCrossSectionByComponents>(id))
		{ }
	}
}
