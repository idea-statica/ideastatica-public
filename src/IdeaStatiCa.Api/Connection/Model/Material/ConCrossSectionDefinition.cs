using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaStatiCa.Api.Connection.Model.Material
{
	/// <summary>
	/// How a cross-section is defined — the editable facet of <see cref="ConCrossSectionDetail"/>.
	/// Polymorphic on the wire: every element is one of the known subtypes and carries the
	/// <c>$type</c> discriminator.
	/// </summary>
	[KnownType(typeof(ConCrossSectionLibraryDefinition))]
	[KnownType(typeof(ConCrossSectionParametricDefinition))]
	[KnownType(typeof(ConCrossSectionCustomDefinition))]
	public abstract class ConCrossSectionDefinition
	{
	}

	/// <summary>A rolled section taken from the MPRL library by name.</summary>
	public class ConCrossSectionLibraryDefinition : ConCrossSectionDefinition
	{
		/// <summary>MPRL name of the section (e.g. "HEA200").</summary>
		public string MprlName { get; set; }

		/// <summary>Name of the material the section is made of.</summary>
		public string MaterialName { get; set; }

		public bool MirrorY { get; set; }

		public bool MirrorZ { get; set; }
	}

	/// <summary>A parametric section (welded, boxed, cold-formed, parametric rolled) defined by named dimensions.</summary>
	public class ConCrossSectionParametricDefinition : ConCrossSectionDefinition
	{
		/// <summary>Shape type identifier (e.g. "Iw", "Tw", "BoxFl", "CHSPar").</summary>
		public string ShapeType { get; set; }

		/// <summary>Name of the material the section is made of.</summary>
		public string MaterialName { get; set; }

		/// <summary>
		/// The dimensions of the shape, each of the concrete kind its value has (see
		/// <see cref="ConCssDimension"/>). On read every dimension of the shape is listed; on write
		/// the ones not named keep the shape's defaults.
		/// </summary>
		public List<ConCssDimension> Dimensions { get; set; }
	}

	/// <summary>
	/// A general section defined by explicit polygonal components. There is no material of the
	/// section as a whole: each component carries its own.
	/// </summary>
	public class ConCrossSectionCustomDefinition : ConCrossSectionDefinition
	{
		public List<ConCrossSectionCustomComponent> Components { get; set; }
	}

	/// <summary>
	/// One component of a custom cross-section. Read-only facet: custom sections are authored in
	/// the desktop application — the parametric POST/PUT endpoints do not accept them.
	/// </summary>
	public class ConCrossSectionCustomComponent
	{
		/// <summary>
		/// Outer boundary of the component as an ordered chain of typed segments — the same
		/// contract as the evaluated geometry (see <see cref="ConCssSegment"/>). A hand-drawn
		/// polygon arrives as line segments, but construction geometry (e.g. a plate trimmed by
		/// a tube) carries circular arcs, so a consumer must not assume lines only.
		/// </summary>
		public List<ConCssSegment> Outline { get; set; }

		/// <summary>Holes inside the outline, each a closed segment chain with the same contract as <see cref="Outline"/>.</summary>
		public List<List<ConCssSegment>> Openings { get; set; }

		/// <summary>Material of this component; null = the section's material.</summary>
		public string MaterialName { get; set; }
	}

	/// <summary>Point in the cross-section plane (IDEA convention: Y horizontal, Z vertical), in meters.</summary>
	public class ConCssPoint2D
	{
		public double Y { get; set; }

		public double Z { get; set; }
	}
}
