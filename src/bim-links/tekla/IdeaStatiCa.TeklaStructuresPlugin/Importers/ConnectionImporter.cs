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
			// Lookup the pre-computed connection GUID set for this CP node location.
			// The set was stored by GetCadUserSelection (in Model.cs) before import started.
			var nodeKey = id.Node?.GetId()?.ToString() ?? id.GetStringId();
			Model.SetCurrentConnectionGuidsByKey(nodeKey);

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
			if (cachedOject is IIdeaConnectionPoint cp)
			{
				// Activate the GUID set for this CP so importers filter correctly.
				var nodeKey = id.Node?.GetId()?.ToString() ?? id.GetStringId();
				Model.SetCurrentConnectionGuidsByKey(nodeKey);

				// Reset mutable lists so ProcessConnectionObjects doesn't accumulate
				// items across multiple import sessions for the same cached object.
				(cp.BoltGrids as System.Collections.Generic.List<IIdeaBoltGrid>)?.Clear();
				(cp.AnchorGrids as System.Collections.Generic.List<IIdeaAnchorGrid>)?.Clear();
				(cp.Welds as System.Collections.Generic.List<IIdeaWeld>)?.Clear();
				(cp.Plates as System.Collections.Generic.List<IIdeaPlate>)?.Clear();
				(cp.FoldedPlates as System.Collections.Generic.List<IIdeaFoldedPlate>)?.Clear();
				(cp.Cuts as System.Collections.Generic.List<IIdeaCut>)?.Clear();
				(cp.ConnectedMembers as System.Collections.Generic.List<IIdeaConnectedMember>)?.Clear();
				return cp;
			}
			return null;
		}
	}
}
