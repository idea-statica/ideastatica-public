using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaLoadGroup : AbstractIdeaObject<IIdeaLoadGroup>, IIdeaLoadGroup
	{
		public virtual IdeaRS.OpenModel.Loading.Relation Relation { get; set; }
		
		public virtual IdeaRS.OpenModel.Loading.LoadGroupType GroupType { get; set; }
		
		public virtual double GammaQ { get; set; }
		
		public virtual double Dzeta { get; set; }
		
		public virtual double GammaGInf { get; set; }
		
		public virtual double GammaGSup { get; set; }
		
		public virtual double Psi0 { get; set; }
		
		public virtual double Psi1 { get; set; }
		
		public virtual double Psi2 { get; set; }
		
		protected IdeaLoadGroup(Identifier<IIdeaLoadGroup> identifer)
			: base(identifer)
		{ }

		public IdeaLoadGroup(int id)
			: this(new IntIdentifier<IIdeaLoadGroup>(id))
		{ }

		public IdeaLoadGroup(string id)
			: this(new StringIdentifier<IIdeaLoadGroup>(id))
		{ }
	}
}
