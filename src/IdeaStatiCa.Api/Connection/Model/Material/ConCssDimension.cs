using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaStatiCa.Api.Connection.Model.Material
{
	/// <summary>
	/// One dimension of a parametric cross-section, identified by the shape's stable numeric
	/// dimension id and its stable non-localized code name (e.g. "wH" — the engine's parameter
	/// identifier; display captions are localized and deliberately not part of the contract).
	/// On input either <see cref="Name"/> or <see cref="Id"/> is enough; when both are given
	/// they must identify the same dimension.
	/// </summary>
	/// <remarks>
	/// Polymorphic on the wire (<c>$type</c> discriminator): the concrete subtype says what the
	/// value is — a <see cref="ConCssNumberDimension"/> (an SI number), a
	/// <see cref="ConCssCountDimension"/> (a whole number), a <see cref="ConCssSwitchDimension"/>
	/// (true/false) or a <see cref="ConCssChoiceDimension"/> (one of the options it lists). A shape
	/// defines each of its dimensions as exactly one kind; writing a dimension as another kind
	/// answers 422 and says which is expected. The canonical workflow is to read the shape's
	/// template, change the values and send the same objects back.
	/// </remarks>
	[KnownType(typeof(ConCssNumberDimension))]
	[KnownType(typeof(ConCssCountDimension))]
	[KnownType(typeof(ConCssSwitchDimension))]
	[KnownType(typeof(ConCssChoiceDimension))]
	public abstract class ConCssDimension
	{
		/// <summary>Stable numeric id of the dimension within the shape.</summary>
		public int Id { get; set; }

		/// <summary>Stable non-localized dimension code of the shape (e.g. "wH", "fT").</summary>
		public string Name { get; set; }
	}

	/// <summary>A dimension with an SI number: a length or thickness in meters, an angle in radians.</summary>
	public class ConCssNumberDimension : ConCssDimension
	{
		public double Value { get; set; }
	}

	/// <summary>A dimension with a whole number, e.g. a polygon vertex count.</summary>
	public class ConCssCountDimension : ConCssDimension
	{
		public int Value { get; set; }
	}

	/// <summary>A dimension that is on or off, e.g. mirroring.</summary>
	public class ConCssSwitchDimension : ConCssDimension
	{
		public bool Value { get; set; }
	}

	/// <summary>
	/// A dimension that is one of a fixed set of options, e.g. a web alignment. The option
	/// travels under its stable, non-localized name; <see cref="Options"/> lists the ones the
	/// shape offers so a caller never has to guess them.
	/// </summary>
	public class ConCssChoiceDimension : ConCssDimension
	{
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
}
