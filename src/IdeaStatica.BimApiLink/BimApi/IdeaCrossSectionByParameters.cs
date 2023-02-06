using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaCrossSectionByParameters : AbstractIdeaObject<IIdeaCrossSectionByParameters>, IIdeaCrossSectionByParameters
	{
		public virtual double Rotation { get; set; }
		
		public virtual IIdeaMaterial Material { get; set; } = null;

		public virtual IdeaRS.OpenModel.CrossSection.CrossSectionType Type { get; set; }
		
		public virtual HashSet<IdeaRS.OpenModel.CrossSection.Parameter> Parameters { get; set; } = null;

		public virtual bool IsInPrincipal { get; set; }

		public IdeaCrossSectionByParameters(Identifier<IIdeaCrossSectionByParameters> identifer)
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
