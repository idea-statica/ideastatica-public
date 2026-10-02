#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Grid positions in two forms. <see cref="Values"/> is the flat list: every value placed once,
	/// in one row. <see cref="Rows"/> is the full form the underlying model uses — several rows, each
	/// value optionally repeated — and is the only one that can express a repeated or multi-row
	/// layout.
	/// <para>
	/// A GET fills in both: <see cref="Values"/> flattened for readers that only understand it, and
	/// <see cref="Rows"/> carrying the layout as stored. A PUT that sends <see cref="Rows"/> is
	/// mapped from it and <see cref="Values"/> is ignored; a PUT that sends only <see cref="Values"/>
	/// behaves exactly as before. Sending an unmodified GET body therefore preserves the layout.
	/// </para>
	/// </summary>
	public class ConGridPositions
	{
		/// <summary>Flat list of spacing values [m]; each placed once. Ignored when <see cref="Rows"/> is set.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<double>? Values { get; set; }

		/// <summary>Rows of spacing values with repeat counts. Takes precedence over <see cref="Values"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<List<ConRepeatedValue>>? Rows { get; set; }
	}
}
