using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatica.BimApiLink.BimApi
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

		public IEnumerable<IIdeaConnectedMember> ConnectedMembers { get; set; }

		public IEnumerable<IIdeaPlate> Plates { get; set; }

		public IEnumerable<IIdeaFoldedPlate> FoldedPlates { get; set; }

		public IEnumerable<IIdeaAnchorGrid> AnchorGrids { get; set; }

		public IEnumerable<IIdeaBoltGrid> BoltGrids { get; set; }

		public IEnumerable<IIdeaWeld> Welds { get; set; }

		public IEnumerable<IIdeaCut> Cuts { get; set; }

		public IIdeaPersistenceToken Token
		{
			get => new ConnectionIdentifier<IIdeaConnectionPoint>(Node.Vector.X, Node.Vector.Y, Node.Vector.Z)
			{
				//ConnectedMembers = ConnectedMembers.Select(cm => cm.),
				ConnectedMembers = new List<ImmutableIdentifier<IIdeaConnectedMember>>(),
				Plates = Plates?.Select(p => p.Token as ImmutableIdentifier<IIdeaPlate>) ?? new List<ImmutableIdentifier<IIdeaPlate>>(),
				FoldedPlates = FoldedPlates?.Select(fp => fp.Token as ImmutableIdentifier<IIdeaFoldedPlate>) ?? new List<ImmutableIdentifier<IIdeaFoldedPlate>>(),
				AnchorGrids = AnchorGrids?.Select(ag => ag.Token as ImmutableIdentifier<IIdeaAnchorGrid>) ?? new List<ImmutableIdentifier<IIdeaAnchorGrid>>(),
				BoltGrids = BoltGrids?.Select(bg => bg.Token as ImmutableIdentifier<IIdeaBoltGrid>) ?? new List<ImmutableIdentifier<IIdeaBoltGrid>>(),
				Welds = Welds?.Select(w => w.Token as ImmutableIdentifier<IIdeaWeld>) ?? new List<ImmutableIdentifier<IIdeaWeld>>(),
				Cuts = Cuts?.Select(c => c.Token as ImmutableIdentifier<IIdeaCut>) ?? new List<ImmutableIdentifier<IIdeaCut>>(),
			};
		}
	}
}
