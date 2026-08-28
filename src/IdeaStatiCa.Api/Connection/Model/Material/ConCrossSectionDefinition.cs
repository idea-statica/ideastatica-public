using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaStatiCa.Api.Connection.Model.Material
{
	/// <summary>
	/// How a cross-section is defined — the editable facet of <see cref="ConCrossSectionDetail"/>.
	/// The concrete subtype is discriminated by <see cref="DefinitionType"/>.
	/// </summary>
	[KnownType(typeof(ConCrossSectionLibraryDefinition))]
	[KnownType(typeof(ConCrossSectionParametricDefinition))]
	[KnownType(typeof(ConCrossSectionCustomDefinition))]
	public abstract class ConCrossSectionDefinition
	{
		/// <summary>Discriminator: "library" | "parametric" | "custom"</summary>
		public abstract string DefinitionType { get; }

		/// <summary>Name of the cross-section's material.</summary>
		public string MaterialName { get; set; }
	}

	/// <summary>A rolled section taken from the MPRL library by name.</summary>
	public class ConCrossSectionLibraryDefinition : ConCrossSectionDefinition
	{
		public override string DefinitionType => "library";

		/// <summary>MPRL name of the section (e.g. "HEA200").</summary>
		public string MprlName { get; set; }

		public bool MirrorY { get; set; }

		public bool MirrorZ { get; set; }
	}

	/// <summary>A parametric section (welded, boxed, cold-formed, parametric rolled) defined by named dimensions.</summary>
	public class ConCrossSectionParametricDefinition : ConCrossSectionDefinition
	{
		public override string DefinitionType => "parametric";

		/// <summary>Shape type identifier (e.g. "Iw", "Tw", "BoxFl", "CHSPar").</summary>
		public string ShapeType { get; set; }

		/// <summary>Named dimensions of the shape, values in SI units.</summary>
		public List<ConCrossSectionParameter> Parameters { get; set; }
	}

	/// <summary>A general section defined by explicit polygonal components.</summary>
	public class ConCrossSectionCustomDefinition : ConCrossSectionDefinition
	{
		public override string DefinitionType => "custom";

		public List<ConCrossSectionCustomComponent> Components { get; set; }
	}

	/// <summary>
	/// One dimension of a parametric cross-section, identified by the shape's stable numeric
	/// parameter id and its stable non-localized code name (e.g. "wH" — the engine's parameter
	/// identifier; display captions are localized and deliberately not part of the contract).
	/// On input either <see cref="Name"/> or <see cref="Id"/> is enough; when both are given
	/// they must identify the same dimension.
	/// </summary>
	public class ConCrossSectionParameter
	{
		public int Id { get; set; }

		/// <summary>Stable non-localized dimension code of the shape (e.g. "wH", "fT").</summary>
		public string Name { get; set; }

		/// <summary>
		/// The dimension's value, interpreted according to <see cref="ValueKind"/>: an SI double
		/// for <see cref="ConCrossSectionParameterValueKind.Number"/>, 0/1 for
		/// <see cref="ConCrossSectionParameterValueKind.Bool"/>, a whole number for
		/// <see cref="ConCrossSectionParameterValueKind.Int"/> and a zero-based option index for
		/// <see cref="ConCrossSectionParameterValueKind.Enum"/>.
		/// </summary>
		public double Value { get; set; }

		/// <summary>
		/// What <see cref="Value"/> means. Always set on read. Optional on write — omitted means
		/// "whatever this dimension is", and a kind that contradicts the addressed dimension is
		/// rejected, so a switch cannot be written as if it were a length.
		/// </summary>
		public ConCrossSectionParameterValueKind? ValueKind { get; set; }
	}

	/// <summary>
	/// What a parametric dimension's value carries. Most dimensions are SI doubles, but a
	/// shape's defining input can also be a switch (mirroring), a count (polygon vertices) or
	/// a choice (web alignment), and those have to round-trip too.
	/// </summary>
	public enum ConCrossSectionParameterValueKind
	{
		/// <summary>SI double — a length or thickness in meters, an angle in radians.</summary>
		Number = 0,

		/// <summary>Switch; the value is 0 (off) or 1 (on).</summary>
		Bool = 1,

		/// <summary>Whole number; the value must have no fractional part.</summary>
		Int = 2,

		/// <summary>
		/// Choice; the value is the zero-based index into the shape's option list for this
		/// dimension. Sending an index out of range answers 422 and lists the options.
		/// </summary>
		Enum = 3,
	}

	/// <summary>One polygonal component of a custom cross-section.</summary>
	public class ConCrossSectionCustomComponent
	{
		/// <summary>Outer boundary of the component in the section plane.</summary>
		public List<ConCssPoint2D> Outline { get; set; }

		/// <summary>Holes inside the outline (each a closed polygon).</summary>
		public List<List<ConCssPoint2D>> Openings { get; set; }

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
