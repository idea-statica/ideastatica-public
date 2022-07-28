using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaCrossSectionByParameters : AbstractIdeaObject<IIdeaCrossSectionByParameters>, IIdeaCrossSectionByParameters
	{
		public virtual double Rotation { get; set; }
		
		public virtual IIdeaMaterial Material { get; set; } = null!;
		
		public virtual IdeaRS.OpenModel.CrossSection.CrossSectionType Type { get; set; }
		
		public virtual HashSet<IdeaRS.OpenModel.CrossSection.Parameter> Parameters { get; set; } = null!;
		
		protected IdeaCrossSectionByParameters(Identifier<IIdeaCrossSectionByParameters> identifer)
			: base(identifer)
		{ }

		public IdeaCrossSectionByParameters(int id)
			: this(new IntIdentifier<IIdeaCrossSectionByParameters>(id))
		{ }

		public IdeaCrossSectionByParameters(string id)
			: this(new StringIdentifier<IIdeaCrossSectionByParameters>(id))
		{ }
	}
}
