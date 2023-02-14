using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaTaper : AbstractIdeaObject<IIdeaTaper>, IIdeaTaper
	{
		public virtual IEnumerable<IIdeaSpan> Spans { get; set; } = null;

		public IdeaTaper(Identifier<IIdeaTaper> identifer)
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
