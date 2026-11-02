using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model.Material
{
	/// <summary>
	/// Evaluated outline geometry of a cross-section, in the exact line/arc segment form (arcs
	/// preserved, not discretized). The same shape for every definition kind, read-only, and
	/// enough on its own to draw the section.
	/// </summary>
	public class ConCrossSectionGeometry
	{
		public List<ConCrossSectionGeometryComponent> Components { get; set; }
	}

	/// <summary>Evaluated geometry of one component: outline and openings as segment chains.</summary>
	public class ConCrossSectionGeometryComponent
	{
		/// <summary>
		/// Closed outer boundary as an ordered chain of typed segments (straight lines and
		/// circular arcs — see <see cref="ConCssSegment"/> for the chain and coordinate contract).
		/// The chain is closed: the last segment's End equals the first segment's Start.
		/// </summary>
		public List<ConCssSegment> Outline { get; set; }

		/// <summary>
		/// Holes inside the outline (hollow sections and openings), each a closed segment chain
		/// with the same contract as <see cref="Outline"/>.
		/// </summary>
		public List<List<ConCssSegment>> Openings { get; set; }

		/// <summary>
		/// The material this component is made of: its own where the section assigns one per
		/// component, otherwise the material of the section.
		/// </summary>
		public string MaterialName { get; set; }
	}
}
