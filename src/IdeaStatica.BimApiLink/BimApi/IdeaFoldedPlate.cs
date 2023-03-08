using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaFoldedPlate : AbstractIdeaObject<IIdeaFoldedPlate>, IIdeaFoldedPlate
	{
		public virtual IIdeaPersistenceToken Token { get; set; }

		public IEnumerable<IIdeaPlate> Plates { get; set; }

		public IEnumerable<IIdeaBend> Bends { get; set; }


		protected IdeaFoldedPlate(Identifier<IIdeaFoldedPlate> identifer)
		: base(identifer)
		{
			Token = identifer;
		}

		public IdeaFoldedPlate(int id)
			: this(new IntIdentifier<IIdeaFoldedPlate>(id))
		{ }

		public IdeaFoldedPlate(string id)
			: this(new StringIdentifier<IIdeaFoldedPlate>(id))
		{ }
	}
}
