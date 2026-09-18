using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class ConnectionImporter : ImporterConnectionIdentifier<IIdeaConnectionPoint>
	{
		protected IModelClient Model { get; }

		protected IPluginLogger Logger { get; }

		public ConnectionImporter(IModelClient model, IPluginLogger logger)
			: base()
		{
			Model = model;
			Logger = logger;
		}

		public override IIdeaConnectionPoint Create(ConnectionIdentifier<IIdeaConnectionPoint> id)
		{
			// CAD re-sync: the token is a frozen import-time snapshot, so components added in Tekla after the import are
			// missing from it. Re-discover components connected to the joint's members and merge them back into the token
			// before the ConnectionPoint is rebuilt. The structural member-set stays frozen (see ConnectionTokenReconciler),
			// so this can never re-cluster joints the way the reverted whole-model re-discovery did.
			RefreshData(id);

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

		/// <summary>
		/// Re-discovers components connected to the joint's members (via near-joint welds/bolts) and merges the newly
		/// added ones into the token. Best-effort: any failure falls back to the frozen token so a re-sync never breaks.
		/// </summary>
		private void RefreshData(ConnectionIdentifier<IIdeaConnectionPoint> id)
		{
			try
			{
				if (id?.ConnectedMembers == null || id.ConnectedMembers.Count == 0 || id.Node == null)
				{
					return;
				}

				var jointPoint = Model.GetPoint3D(id.Node.GetId().ToString());
				if (jointPoint == null)
				{
					return;
				}

				var memberHandles = id.ConnectedMembers
					.Select(m => m.GetId()?.ToString())
					.Where(h => h != null);

				List<IIdentifier> discovered = IdentifierHelper.GetConnectedIdentifiers(Model, memberHandles, jointPoint);
				ConnectionTokenReconciler.Apply(id, discovered, Logger);
			}
			catch (Exception ex)
			{
				Logger?.LogWarning($"CAD re-sync re-discovery failed for '{id?.GetStringId()}'; using the frozen token.", ex);
			}
		}
	}
}
