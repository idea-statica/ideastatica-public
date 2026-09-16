using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model.Material
{
	/// <summary>
	/// Full description of a cross-section in the project: the editable definition
	/// (how it is defined) plus the evaluated outline geometry (what it looks like).
	/// </summary>
	public class ConCrossSectionDetail
	{
		/// <summary>Id of the cross-section in the project.</summary>
		public int Id { get; set; }

		/// <summary>Display name of the cross-section.</summary>
		public string Name { get; set; }

		/// <summary>How the section is defined (library / parametric / custom).</summary>
		public ConCrossSectionDefinition Definition { get; set; }

		/// <summary>Evaluated geometry — same shape for every definition kind, read-only.</summary>
		public ConCrossSectionGeometry Geometry { get; set; }
	}

	/// <summary>Evaluated outline geometry of a cross-section, in the exact line/arc segment form (arcs preserved, not discretized).</summary>
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

		/// <summary>Material of this component; null = the section's material.</summary>
		public string MaterialName { get; set; }
	}
}
