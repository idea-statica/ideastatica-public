using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaMember2D : AbstractIdeaObject<IIdeaMember2D>, IIdeaMember2D
	{
		public virtual IIdeaPersistenceToken Token { get; set; }

		public virtual List<IIdeaElement2D> Elements2D { get; set; }

		public IdeaMember2D(Identifier<IIdeaMember2D> identifer)
			: base(identifer)
		{
			Token = identifer;
		}

		public IdeaMember2D(int id)
			: this(new IntIdentifier<IIdeaMember2D>(id))
		{ }

		public IdeaMember2D(string id)
			: this(new StringIdentifier<IIdeaMember2D>(id))
		{ }
	}
}
