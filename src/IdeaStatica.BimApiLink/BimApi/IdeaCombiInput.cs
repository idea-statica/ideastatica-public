using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaCombiInput : AbstractIdeaObject<IIdeaCombiInput>, IIdeaCombiInput
	{
		public virtual IdeaRS.OpenModel.Loading.TypeOfCombiEC TypeCombiEC { get; set; }
		
		public virtual IdeaRS.OpenModel.Loading.TypeCalculationCombiEC TypeCalculationCombi { get; set; }
		
		public virtual List<IIdeaCombiItem> CombiItems { get; set; } = null!;
		
		protected IdeaCombiInput(Identifier<IIdeaCombiInput> identifer)
			: base(identifer)
		{ }

		public IdeaCombiInput(int id)
			: this(new IntIdentifier<IIdeaCombiInput>(id))
		{ }

		public IdeaCombiInput(string id)
			: this(new StringIdentifier<IIdeaCombiInput>(id))
		{ }
	}
}
