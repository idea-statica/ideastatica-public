using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaSpan : AbstractIdeaObject<IIdeaSpan>, IIdeaSpan
	{
		public virtual IIdeaCrossSection StartCrossSection { get; set; } = null;
		
		public virtual IIdeaCrossSection EndCrossSection { get; set; } = null;
		
		public virtual double StartPosition { get; set; }
		
		public virtual double EndPosition { get; set; }

		public IdeaSpan(Identifier<IIdeaSpan> identifer)
			: base(identifer)
		{ }

		public IdeaSpan(int id)
			: this(new IntIdentifier<IIdeaSpan>(id))
		{ }

		public IdeaSpan(string id)
			: this(new StringIdentifier<IIdeaSpan>(id))
		{ }
	}
}
