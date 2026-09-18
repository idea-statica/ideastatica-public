#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Explicit fastener position override — used when <see cref="ConGridGeometry.UserPositions"/>
	/// is set, in place of the regular grid (top/bottom/left/right or radii/angles).
	/// </summary>
	public class ConFastenerPosition
	{
		/// <summary>X coordinate [m] in the grid's local system.</summary>
		public double X { get; set; }

		/// <summary>Y coordinate [m] in the grid's local system.</summary>
		public double Y { get; set; }

		/// <summary>Optional grouping index used by the solver. Defaults to 0.</summary>
		public int GridIndex { get; set; }

		/// <summary>Optional slotted-hole overrides at this position (bolt grids only).</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<ConSlottedHole>? SlottedHoles { get; set; }

		/// <summary>Hook rotation [radians] — used only for <see cref="ConAnchorType.Reinforcement"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public double? HookRotation { get; set; }
	}
}
