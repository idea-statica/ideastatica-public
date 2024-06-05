using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class ConnectionImporter : ImporterConnectionIdentifier<IIdeaConnectionPoint>
	{
		protected IModelClient Model { get; }

		public ConnectionImporter(IModelClient model)
			: base()
		{
			Model = model;
		}

		public override IIdeaConnectionPoint Create(ConnectionIdentifier<IIdeaConnectionPoint> id)
		{
			var connectionPoint = new ConnectionPoint(id.GetStringId().ToString())
			{
				Node = Get(id.Node as Identifier<IIdeaNode>),
				ConnectedMembers = id.ConnectedMembers?.Select(cm => GetMaybe(cm)).Where(x => x != null).ToList() ?? new List<IIdeaConnectedMember>(),
				Plates = id.Plates?.Select(p => GetMaybe(p as Identifier<IIdeaPlate>)).Where(x => x != null).ToList() ?? new List<IIdeaPlate>(),
				BoltGrids = id.BoltGrids?.Select(bg => GetMaybe(bg as Identifier<IIdeaBoltGrid>)).Where(x => x != null).ToList() ?? new List<IIdeaBoltGrid>(),
				AnchorGrids = id.AnchorGrids?.Select(bg => GetMaybe(bg as Identifier<IIdeaAnchorGrid>)).Where(x => x != null).ToList() ?? new List<IIdeaAnchorGrid>(),
				Welds = id.Welds?.Select(bg => GetMaybe(bg as Identifier<IIdeaWeld>)).Where(x => x != null).ToList() ?? new List<IIdeaWeld>(),
				Cuts = id.Cuts?.Select(bg => GetMaybe(bg as Identifier<IIdeaCut>)).Where(x => x != null).ToList() ?? new List<IIdeaCut>(),
				FoldedPlates = id.FoldedPlates?.Select(bg => GetMaybe(bg as Identifier<IIdeaFoldedPlate>)).Where(x => x != null).ToList() ?? new List<IIdeaFoldedPlate>(),
			};

			Model.CacheCreatedObject(id, connectionPoint);
			return connectionPoint;
		}

		public override IIdeaConnectionPoint Check(ConnectionIdentifier<IIdeaConnectionPoint> id)
		{
			var cachedOject = Model.GetCachedObject(id);
			return cachedOject is IIdeaConnectionPoint ? cachedOject as IIdeaConnectionPoint : null;
		}
	}
}
