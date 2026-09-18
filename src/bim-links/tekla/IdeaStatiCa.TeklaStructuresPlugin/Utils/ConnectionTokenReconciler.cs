using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utils
{
	/// <summary>
	/// Merges the live CAD re-discovery result into a connection's persisted identifier token during a re-sync.
	/// The token is rebuilt from a frozen import-time snapshot (see <c>ConnectionImporter</c>), so a component added
	/// in Tekla AFTER the import is missing from it; this adds the freshly discovered components back in.
	/// </summary>
	/// <remarks>
	/// Policy is reliability-first, so the reverted whole-model re-discovery (which added structural members and
	/// re-clustered joints) cannot recur:
	/// <list type="bullet">
	/// <item>non-member components found by discovery (plates incl. plate-stiffeners, bolt grids, welds, cuts, anchor
	/// grids, folded plates) are ADDED if not already present (dedup by id);</item>
	/// <item>the STRUCTURAL member-set is FROZEN: a newly discovered <see cref="ConnectedMemberIdentifier{T}"/> not
	/// already in the token is NOT added (a member contributes joint-clustering endpoints) — it is logged as a topology
	/// change so a full re-detect can pick it up manually;</item>
	/// <item>existing token entries are never removed here (keep-if-live); a component deleted in Tekla drops out later
	/// because <c>ConnectionImporter.Create</c> filters ids that no longer resolve.</item>
	/// </list>
	/// Append-only + dedup ⇒ idempotent: a re-sync with no model change leaves the token unchanged.
	/// </remarks>
	public static class ConnectionTokenReconciler
	{
		public static void Apply(ConnectionIdentifier<IIdeaConnectionPoint> id, IReadOnlyList<IIdentifier> discovered, IPluginLogger logger)
		{
			if (id is null || discovered is null || discovered.Count == 0)
			{
				return;
			}

			var frozenMembers = new HashSet<string>(
				(id.ConnectedMembers ?? new List<ImmutableIdentifier<IIdeaConnectedMember>>())
					.Select(m => m.GetId()?.ToString())
					.Where(s => s != null),
				StringComparer.OrdinalIgnoreCase);

			foreach (IIdentifier item in discovered)
			{
				switch (item)
				{
					case ConnectedMemberIdentifier<IIdeaConnectedMember> member:
						string memberId = member.GetId()?.ToString();
						if (memberId != null && !frozenMembers.Contains(memberId))
						{
							logger?.LogInformation(
								$"CAD re-sync: member '{memberId}' is connected at joint '{id.GetStringId()}' but was not part of the imported connection. " +
								"The member-set is kept frozen to preserve connection detection/merge; run a full re-detect to include it.");
						}

						break;
					case StringIdentifier<IIdeaPlate> plate:
						AddIfNew(id.Plates, plate);
						break;
					case StringIdentifier<IIdeaBoltGrid> bolt:
						AddIfNew(id.BoltGrids, bolt);
						break;
					case StringIdentifier<IIdeaWeld> weld:
						AddIfNew(id.Welds, weld);
						break;
					case StringIdentifier<IIdeaCut> cut:
						AddIfNew(id.Cuts, cut);
						break;
					case StringIdentifier<IIdeaAnchorGrid> anchor:
						AddIfNew(id.AnchorGrids, anchor);
						break;
					case StringIdentifier<IIdeaFoldedPlate> folded:
						AddIfNew(id.FoldedPlates, folded);
						break;
				}
			}
		}

		private static void AddIfNew<T>(IList<ImmutableIdentifier<T>> collection, ImmutableIdentifier<T> item)
			where T : IIdeaObject
		{
			if (collection is null)
			{
				return;
			}

			string newId = item.GetId()?.ToString();
			if (!collection.Any(e => string.Equals(e.GetId()?.ToString(), newId, StringComparison.OrdinalIgnoreCase)))
			{
				collection.Add(item);
			}
		}
	}
}
