using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaCut : AbstractIdeaObject<IIdeaCut>, IIdeaCut
	{
		public virtual IIdeaPersistenceToken Token { get; set; }

		public virtual IIdeaObject ModifiedObject { get; set; }

		public virtual IIdeaObject CuttingObject { get; set; }

		public double Offset { get; set; }

		public virtual IIdeaWeld Weld { get; set; }

		public CutMethod CutMethod { get; set; }

		public CutOrientation CutOrientation { get; set; }

		public DistanceComparison DistanceComparison { get; set; }

		public CutPart CutPart { get; set; }

		public bool ExtendBeforeCut { get; set; }

		protected IdeaCut(Identifier<IIdeaCut> identifer)
			: base(identifer)
		{
			Token = identifer;
		}

		public IdeaCut(int id)
			: this(new IntIdentifier<IIdeaCut>(id))
		{ }

		public IdeaCut(string id)
			: this(new StringIdentifier<IIdeaCut>(id))
		{ }

	}
}
