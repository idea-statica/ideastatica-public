#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Simplified grid positions — a flat list of values. Each value is placed once
	/// (repeat count = 1). The underlying new-model type supports multi-layer repeated
	/// values; that advanced form is not exposed here yet.
	/// </summary>
	public class ConGridPositions
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<double>? Values { get; set; }
	}
}
