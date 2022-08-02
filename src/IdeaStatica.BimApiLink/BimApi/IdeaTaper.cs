using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaTaper : AbstractIdeaObject<IIdeaTaper>, IIdeaTaper
	{
		public virtual IEnumerable<IIdeaSpan> Spans { get; set; } = null!;
		
		protected IdeaTaper(Identifier<IIdeaTaper> identifer)
			: base(identifer)
		{ }

		public IdeaTaper(int id)
			: this(new IntIdentifier<IIdeaTaper>(id))
		{ }

		public IdeaTaper(string id)
			: this(new StringIdentifier<IIdeaTaper>(id))
		{ }
	}
}
