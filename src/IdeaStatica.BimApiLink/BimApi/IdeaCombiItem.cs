using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaCombiItem : AbstractIdeaObject<IIdeaCombiItem>, IIdeaCombiItem
	{
		public virtual IIdeaLoadCase LoadCase { get; set; } = null!;
		
		public virtual double Coeff { get; set; }
		
		public virtual IIdeaCombiInput Combination { get; set; } = null!;
		
		protected IdeaCombiItem(Identifier<IIdeaCombiItem> identifer)
			: base(identifer)
		{ }

		public IdeaCombiItem(int id)
			: this(new IntIdentifier<IIdeaCombiItem>(id))
		{ }

		public IdeaCombiItem(string id)
			: this(new StringIdentifier<IIdeaCombiItem>(id))
		{ }
	}
}
