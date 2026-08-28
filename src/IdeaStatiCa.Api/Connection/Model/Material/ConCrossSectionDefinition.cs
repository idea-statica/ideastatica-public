using Newtonsoft.Json;
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
	/// <remarks>
	/// A dimension carries its value in the field that fits its type, named by
	/// <see cref="ValueKind"/>: <see cref="Value"/> for a number, <see cref="BoolValue"/> for a
	/// switch, <see cref="IntValue"/> for a count, <see cref="StringValue"/> for a choice. Only
	/// that one field is written on read and only that one is accepted on write — sending the
	/// wrong one answers 422 and says which is expected.
	/// </remarks>
	public class ConCrossSectionParameter
	{
		public int Id { get; set; }

		/// <summary>Stable non-localized dimension code of the shape (e.g. "wH", "fT").</summary>
		public string Name { get; set; }

		/// <summary>
		/// The value of a <see cref="ConCrossSectionParameterValueKind.Number"/> dimension, in SI
		/// units — a length or thickness in meters, an angle in radians.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public double? Value { get; set; }

		/// <summary>The value of a <see cref="ConCrossSectionParameterValueKind.Bool"/> dimension, e.g. mirroring.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public bool? BoolValue { get; set; }

		/// <summary>The value of an <see cref="ConCrossSectionParameterValueKind.Int"/> dimension, e.g. a polygon vertex count.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? IntValue { get; set; }

		/// <summary>
		/// The value of an <see cref="ConCrossSectionParameterValueKind.Enum"/> dimension: the
		/// chosen option under its stable, non-localized name (e.g. "Center" | "Left" | "Right"
		/// for a web alignment). Matched case-insensitively on write; an unknown option answers
		/// 422 listing the ones the shape offers.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string StringValue { get; set; }

		/// <summary>
		/// Which of the value fields this dimension uses. Always set on read. Optional on write —
		/// omitted means "whatever this dimension is", and a kind that contradicts the addressed
		/// dimension is rejected, so a switch cannot be written as if it were a length.
		/// </summary>
		public ConCrossSectionParameterValueKind? ValueKind { get; set; }
	}

	/// <summary>
	/// Which field of <see cref="ConCrossSectionParameter"/> carries the value. Most dimensions
	/// are SI numbers, but a shape's defining input can also be a switch (mirroring), a count
	/// (polygon vertices) or a choice (web alignment), and those have to round-trip too.
	/// </summary>
	public enum ConCrossSectionParameterValueKind
	{
		/// <summary>Uses <see cref="ConCrossSectionParameter.Value"/>.</summary>
		Number = 0,

		/// <summary>Uses <see cref="ConCrossSectionParameter.BoolValue"/>.</summary>
		Bool = 1,

		/// <summary>Uses <see cref="ConCrossSectionParameter.IntValue"/>.</summary>
		Int = 2,

		/// <summary>Uses <see cref="ConCrossSectionParameter.StringValue"/>, the option's stable name.</summary>
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
