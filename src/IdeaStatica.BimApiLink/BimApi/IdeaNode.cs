using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaNode : AbstractIdeaObject<IIdeaNode>, IIdeaNode
	{
		public virtual IIdeaPersistenceToken Token { get; set; }
		
		public virtual IdeaVector3D Vector { get; set; } = null;

		public IdeaNode(Identifier<IIdeaNode> identifer)
			: base(identifer)
		{
			Token = identifer;
		}

		public IdeaNode(int id)
			: this(new IntIdentifier<IIdeaNode>(id))
		{ }

		public IdeaNode(string id)
			: this(new StringIdentifier<IIdeaNode>(id))
		{ }
	}
}
