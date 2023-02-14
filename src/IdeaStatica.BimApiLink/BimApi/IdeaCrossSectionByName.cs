using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaCrossSectionByName : AbstractIdeaObject<IIdeaCrossSectionByName>, IIdeaCrossSectionByName
	{
		public virtual double Rotation { get; set; }
		
		public virtual IIdeaMaterial Material { get; set; } = null;

		public virtual bool IsInPrincipal { get; set; }

		public IdeaCrossSectionByName(Identifier<IIdeaCrossSectionByName> identifer)
			: base(identifer)
		{ }

		public IdeaCrossSectionByName(int id)
			: this(new IntIdentifier<IIdeaCrossSectionByName>(id))
		{ }

		public IdeaCrossSectionByName(string id)
			: this(new StringIdentifier<IIdeaCrossSectionByName>(id))
		{ }
	}
}
