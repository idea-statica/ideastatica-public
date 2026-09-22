#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// One spacing value of a grid row, optionally repeated. <c>{ value: 0.05, repeatCount: 3 }</c>
	/// is the same layout as three consecutive <c>0.05</c> entries in <see cref="ConGridPositions.Values"/>.
	/// </summary>
	public class ConRepeatedValue
	{
		/// <summary>Spacing value [m].</summary>
		public double Value { get; set; }

		/// <summary>How many times the value repeats. Defaults to 1.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? RepeatCount { get; set; }
	}
}
