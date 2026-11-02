using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaConnectionPoint : AbstractIdeaObject<IIdeaConnectionPoint>, IIdeaConnectionPoint, IIdeaPersistentObject
	{
		protected IdeaConnectionPoint(Identifier<IIdeaConnectionPoint> identifer)
			: base(identifer)
		{
		}

		public IdeaConnectionPoint(int id)
			: this(new IntIdentifier<IIdeaConnectionPoint>(id))
		{ }

		public IdeaConnectionPoint(string id)
			: this(new StringIdentifier<IIdeaConnectionPoint>(id))
		{ }

		public IdeaConnectionPoint(double X, double Y, double Z)
			: this(new ConnectionIdentifier<IIdeaConnectionPoint>(X, Y, Z))
		{ }
		public virtual IIdeaNode Node { get; set; }

		public IEnumerable<IIdeaConnectedMember> ConnectedMembers { get; set; } = new List<IIdeaConnectedMember>();

		public IEnumerable<IIdeaPlate> Plates { get; set; } = new List<IIdeaPlate>();

		public IEnumerable<IIdeaFoldedPlate> FoldedPlates { get; set; } = new List<IIdeaFoldedPlate>();

		public IEnumerable<IIdeaAnchorGrid> AnchorGrids { get; set; } = new List<IIdeaAnchorGrid>();

		public IEnumerable<IIdeaBoltGrid> BoltGrids { get; set; } = new List<IIdeaBoltGrid>();

		public IEnumerable<IIdeaPinGrid> PinGrids { get; set; } = new List<IIdeaPinGrid>();

		public IEnumerable<IIdeaWeld> Welds { get; set; } = new List<IIdeaWeld>();

		public IEnumerable<IIdeaCut> Cuts { get; set; } = new List<IIdeaCut>();

		public virtual IIdeaPersistenceToken Token
		{
			get => new ConnectionIdentifier<IIdeaConnectionPoint>(Node.Vector.X, Node.Vector.Y, Node.Vector.Z)
			{
				ConnectedMembers = ConnectedMembers?.Select(cp => cp.Token as ImmutableIdentifier<IIdeaConnectedMember>).ToList(),
				Plates = Plates?.Select(p => p.Token as ImmutableIdentifier<IIdeaPlate>).ToList(),
				FoldedPlates = FoldedPlates?.Select(fp => fp.Token as ImmutableIdentifier<IIdeaFoldedPlate>).ToList(),
				AnchorGrids = AnchorGrids?.Select(ag => ag.Token as ImmutableIdentifier<IIdeaAnchorGrid>).ToList(),
				BoltGrids = BoltGrids?.Select(bg => bg.Token as ImmutableIdentifier<IIdeaBoltGrid>).ToList(),
				PinGrids = PinGrids?.Select(pg => pg.Token as ImmutableIdentifier<IIdeaPinGrid>).ToList(),
				Welds = Welds?.Select(w => w.Token as ImmutableIdentifier<IIdeaWeld>).ToList(),
				Cuts = Cuts?.Select(c => c.Token as ImmutableIdentifier<IIdeaCut>).ToList(),
			};
		}

	}
}
