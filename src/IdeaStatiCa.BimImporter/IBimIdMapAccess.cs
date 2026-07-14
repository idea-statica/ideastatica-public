using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// Access to the project's native-to-IOM id map for transfer to and from an external owner. Used by the
	/// BIM Link Hub sync: Model Coordinator owns the map, stores it in its project, and re-seeds the stateless
	/// link each session. Each entry pairs an IOM id with an opaque <c>SourceIdToken</c> string that packs
	/// everything the link needs to resolve the native entity (its BimApi id and serialized persistence token).
	/// The string is produced and consumed only inside the framework; external owners store and return it verbatim.
	/// </summary>
	public interface IBimIdMapAccess
	{
		/// <summary>Exports the current id map as (IOM id, opaque token) pairs.</summary>
		IReadOnlyCollection<(int IomId, string SourceIdToken)> ExportIdMap();

		/// <summary>
		/// Re-seeds the id map from previously exported (IOM id, opaque token) pairs, rebuilding the lookup and
		/// the next-id watermark so subsequently minted ids never collide with restored ones.
		/// </summary>
		void ImportIdMap(IEnumerable<(int IomId, string SourceIdToken)> entries);
	}
}
