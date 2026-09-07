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

		/// <summary>
		/// The dimensions of the shape, each of the concrete kind its value has (see
		/// <see cref="ConCssDimension"/>). On read every dimension of the shape is listed; on write
		/// the ones not named keep the shape's defaults.
		/// </summary>
		public List<ConCssDimension> Dimensions { get; set; }
	}

	/// <summary>A general section defined by explicit polygonal components.</summary>
	public class ConCrossSectionCustomDefinition : ConCrossSectionDefinition
	{
		public override string DefinitionType => "custom";

		public List<ConCrossSectionCustomComponent> Components { get; set; }
	}

	/// <summary>
	/// One dimension of a parametric cross-section, identified by the shape's stable numeric
	/// dimension id and its stable non-localized code name (e.g. "wH" — the engine's parameter
	/// identifier; display captions are localized and deliberately not part of the contract).
	/// On input either <see cref="Name"/> or <see cref="Id"/> is enough; when both are given
	/// they must identify the same dimension.
	/// </summary>
	/// <remarks>
	/// The concrete subtype says what the value is, discriminated by <see cref="DimensionType"/>:
	/// a <see cref="ConCssNumberDimension"/> (an SI number), a <see cref="ConCssCountDimension"/>
	/// (a whole number), a <see cref="ConCssSwitchDimension"/> (true/false) or a
	/// <see cref="ConCssChoiceDimension"/> (one of the options it lists). A shape defines each of
	/// its dimensions as exactly one kind; writing a dimension as another kind answers 422 and
	/// says which is expected. The canonical workflow is to read the shape's template, change the
	/// values and send the same objects back.
	/// </remarks>
	[KnownType(typeof(ConCssNumberDimension))]
	[KnownType(typeof(ConCssCountDimension))]
	[KnownType(typeof(ConCssSwitchDimension))]
	[KnownType(typeof(ConCssChoiceDimension))]
	public abstract class ConCssDimension
	{
		/// <summary>Discriminator: "number" | "count" | "switch" | "choice"</summary>
		public abstract string DimensionType { get; }

		/// <summary>Stable numeric id of the dimension within the shape.</summary>
		public int Id { get; set; }

		/// <summary>Stable non-localized dimension code of the shape (e.g. "wH", "fT").</summary>
		public string Name { get; set; }
	}

	/// <summary>A dimension with an SI number: a length or thickness in meters, an angle in radians.</summary>
	public class ConCssNumberDimension : ConCssDimension
	{
		public override string DimensionType => "number";

		public double Value { get; set; }
	}

	/// <summary>A dimension with a whole number, e.g. a polygon vertex count.</summary>
	public class ConCssCountDimension : ConCssDimension
	{
		public override string DimensionType => "count";

		public int Value { get; set; }
	}

	/// <summary>A dimension that is on or off, e.g. mirroring.</summary>
	public class ConCssSwitchDimension : ConCssDimension
	{
		public override string DimensionType => "switch";

		public bool Value { get; set; }
	}

	/// <summary>
	/// A dimension that is one of a fixed set of options, e.g. a web alignment. The option
	/// travels under its stable, non-localized name; <see cref="Options"/> lists the ones the
	/// shape offers so a caller never has to guess them.
	/// </summary>
	public class ConCssChoiceDimension : ConCssDimension
	{
		public override string DimensionType => "choice";

		/// <summary>
		/// The chosen option's stable name (e.g. "Center"). Matched case-insensitively on write;
		/// an option the shape does not offer answers 422 listing the ones it does.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// The options the shape offers for this dimension, in the shape's order; <see cref="Value"/>
		/// is always one of them. Set on read, ignored on write.
		/// </summary>
		public List<ConCssOption> Options { get; set; }
	}

	/// <summary>One option of a <see cref="ConCssChoiceDimension"/>.</summary>
	public class ConCssOption
	{
		/// <summary>The option's stable, non-localized name — what <see cref="ConCssChoiceDimension.Value"/> carries.</summary>
		public string Value { get; set; }
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
