using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Utils;
using System.Linq;


namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	public class ConnectionPoint : IdeaConnectionPoint
	{
		public ConnectionPoint(int id) : base(id)
		{
		}

		public ConnectionPoint(string id) : base(id)
		{
		}

		public override IIdeaPersistenceToken Token
		{
			get => new ConnectionIdentifier<IIdeaConnectionPoint>(Node.Vector.X.MetersToMilimeters(), Node.Vector.Y.MetersToMilimeters(), Node.Vector.Z.MetersToMilimeters())
			{
				ConnectedMembers = ConnectedMembers?.Select(cp => cp.Token as ImmutableIdentifier<IIdeaConnectedMember>).ToList(),
				Plates = Plates?.Select(p => p.Token as ImmutableIdentifier<IIdeaPlate>).ToList(),
				FoldedPlates = FoldedPlates?.Select(fp => fp.Token as ImmutableIdentifier<IIdeaFoldedPlate>).ToList(),
				AnchorGrids = AnchorGrids?.Select(ag => ag.Token as ImmutableIdentifier<IIdeaAnchorGrid>).ToList(),
				BoltGrids = BoltGrids?.Select(bg => bg.Token as ImmutableIdentifier<IIdeaBoltGrid>).ToList(),
				Welds = Welds?.Select(w => w.Token as ImmutableIdentifier<IIdeaWeld>).ToList(),
				Cuts = Cuts?.Select(c => c.Token as ImmutableIdentifier<IIdeaCut>).ToList(),
			};
		}
	}
}
