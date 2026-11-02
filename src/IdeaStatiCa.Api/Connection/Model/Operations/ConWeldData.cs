#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Weld data — size, type, material, plus optional intermittent geometry. <see cref="Kind"/>
	/// selects between continuous and intermittent; the intermittent fields
	/// (<see cref="BeginOffset"/>, <see cref="EndOffset"/>, <see cref="Length"/>, <see cref="Gap"/>)
	/// are read only when <see cref="Kind"/> is <see cref="ConWeldDataKind.Intermittent"/>.
	///
	/// <para>Note: when <see cref="Kind"/> is <see cref="ConWeldDataKind.Intermittent"/> with
	/// <see cref="Length"/> = 0, libdata writes this as a partial (non-segmented) weld bounded
	/// by the offsets only.</para>
	/// </summary>
	public class ConWeldData
	{
		/// <summary>Continuous (default) or intermittent.</summary>
		public ConWeldDataKind Kind { get; set; } = ConWeldDataKind.Continuous;

		public double Size { get; set; }

		public ConWeldType Type { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? MaterialId { get; set; }

		/// <summary>Distance [m] from the start of the edge to the first weld segment. Used when
		/// <see cref="Kind"/> is <see cref="ConWeldDataKind.Intermittent"/>.</summary>
		public double BeginOffset { get; set; }

		/// <summary>Distance [m] from the end of the edge to the last weld segment. Used when
		/// <see cref="Kind"/> is <see cref="ConWeldDataKind.Intermittent"/>.</summary>
		public double EndOffset { get; set; }

		/// <summary>Length [m] of each weld segment. <c>0</c> = partial weld (no repetition).
		/// Used when <see cref="Kind"/> is <see cref="ConWeldDataKind.Intermittent"/>.</summary>
		public double Length { get; set; }

		/// <summary>Gap [m] between consecutive weld segments. Used when
		/// <see cref="Kind"/> is <see cref="ConWeldDataKind.Intermittent"/>.</summary>
		public double Gap { get; set; }
	}
}
