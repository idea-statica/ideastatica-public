using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaWeld : AbstractIdeaObject<IIdeaWeld>, IIdeaWeld
	{
		public virtual IIdeaPersistenceToken Token { get; set; }
		public double Thickness { get; set; }

		public virtual IIdeaMaterial Material { get; set; }

		public virtual IIdeaNode Start { get; set; }

		public virtual IIdeaNode End { get; set; }

		public IEnumerable<IIdeaObjectConnectable> ConnectedParts { get; set; }

		public WeldType WeldType { get; set; }

		protected IdeaWeld(Identifier<IIdeaWeld> identifer)
			: base(identifer)
		{
			Token = identifer;
		}

		public IdeaWeld(int id)
			: this(new IntIdentifier<IIdeaWeld>(id))
		{ }

		public IdeaWeld(string id)
			: this(new StringIdentifier<IIdeaWeld>(id))
		{ }
	}
}
