using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using BimApiLinkCadExample.BimApi;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;
using System.Linq;
using BimApiLinkCadExample.CadExampleApi;

namespace BimApiLinkCadExample.Importers
{
	internal class ConnectionImporter : ImporterConnectionIdentifier<IIdeaConnectionPoint>
	{
		protected ICadGeometryApi Model { get; }

		public ConnectionImporter(ICadGeometryApi model)
			: base()
		{
			Model = model;
		}

		public override IIdeaConnectionPoint Create(ConnectionIdentifier<IIdeaConnectionPoint> id)
		{
			var connectionPoint = new ConnectionPoint(id.GetStringId().ToString())
			{
				Node = Get(id.Node as Identifier<IIdeaNode>),
				ConnectedMembers = id.ConnectedMembers?.Select(cm => Get(cm)) ?? new List<IIdeaConnectedMember>(),
				Plates = id.Plates?.Select(p => Get(p as Identifier<IIdeaPlate>)) ?? new List<IIdeaPlate>(),
				BoltGrids = id.BoltGrids?.Select(bg => Get(bg as Identifier<IIdeaBoltGrid>)) ?? new List<IIdeaBoltGrid>(),
				Welds = id.Welds?.Select(bg => Get(bg as Identifier<IIdeaWeld>)) ?? new List<IIdeaWeld>(),
				Cuts = id.Cuts?.Select(bg => Get(bg as Identifier<IIdeaCut>)) ?? new List<IIdeaCut>(),

				//Possible additional items.
				//AnchorGrids = id.AnchorGrids?.Select(bg => Get(bg as Identifier<IIdeaAnchorGrid>)) ?? new List<IIdeaAnchorGrid>(),
				//FoldedPlates = id.FoldedPlates?.Select(bg => Get(bg as Identifier<IIdeaFoldedPlate>)) ?? new List<IIdeaFoldedPlate>(),
			};
			return connectionPoint;
		}
	}
}
