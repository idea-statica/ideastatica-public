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

	/// <summary>Evaluated outline geometry of a cross-section.</summary>
	public class ConCrossSectionGeometry
	{
		public List<ConCrossSectionGeometryComponent> Components { get; set; }
	}

	/// <summary>Evaluated geometry of one component: tessellated outline and openings.</summary>
	public class ConCrossSectionGeometryComponent
	{
		/// <summary>Closed outer boundary, tessellated to straight segments (arcs discretized).</summary>
		public List<ConCssPoint2D> Outline { get; set; }

		/// <summary>Holes inside the outline (hollow sections and openings), each a closed polygon.</summary>
		public List<List<ConCssPoint2D>> Openings { get; set; }

		/// <summary>Material of this component; null = the section's material.</summary>
		public string MaterialName { get; set; }
	}
}
