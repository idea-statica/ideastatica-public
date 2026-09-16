#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Fastener grid geometry. Fill the block that matches <see cref="Type"/>:
	/// <list type="bullet">
	/// <item>Orthogonal: <see cref="TopLayers"/>/<see cref="BottomLayers"/>/<see cref="LeftLayers"/>/<see cref="RightLayers"/>.
	/// Distances are in metres.</item>
	/// <item>Polar: <see cref="Radii"/> (metres) plus either <see cref="PolarCounts"/> (when <see cref="PolarInput"/> = ByCount)
	/// or <see cref="Angles"/> (in <b>radians</b>, when <see cref="PolarInput"/> = ByAngle).</item>
	/// </list>
	/// <para>Angles are radians to match the desktop LibData convention — the desktop property grid
	/// displays them in degrees. Send <c>π/4</c> (~0.7854) for 45°, not <c>45</c>.</para>
	/// <para><b>Mutually exclusive inputs:</b> <see cref="UserPositions"/> takes precedence over the
	/// orthogonal layer / polar radius+angle inputs. Set one or the other, not both. The mapper rejects
	/// requests that mix them so the engine doesn't silently produce zero fasteners.</para>
	/// </summary>
	public class ConGridGeometry
	{
		public ConGridGeometryType Type { get; set; }

		/// <summary>Shear plane passes through the threaded part of the fastener. Maps to FastenerGeometry.ShearPlaneInThread.</summary>
		public bool ShearPlaneInThread { get; set; }

		/// <summary>
		/// Row-shift pattern for orthogonal grids. Defaults to <see cref="ConGridLayout.Regular"/>.
		/// </summary>
		public ConGridLayout RowsGridLayout { get; set; }

		// Orthogonal
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConGridPositions? TopLayers { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConGridPositions? BottomLayers { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConGridPositions? LeftLayers { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConGridPositions? RightLayers { get; set; }

		// Polar
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConPolarInputType? PolarInput { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConGridPositions? Radii { get; set; }

		/// <summary>
		/// Per-radius fastener counts for <see cref="ConPolarInputType.ByCount"/>. One value per radius —
		/// e.g. <c>[6, 12]</c> for two concentric rings of 6 and 12 anchors respectively.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<int>? PolarCounts { get; set; }

		/// <summary>Polar angles in <b>radians</b> (not degrees). 45° = π/4 ≈ 0.7854.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConGridPositions? Angles { get; set; }

		/// <summary>
		/// Optional explicit positions that replace the regular grid. When set, the engine uses
		/// these instead of the row/radius layout. For bolt grids each position can carry slotted-hole
		/// overrides; for reinforcement anchors each position can carry a hook rotation.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<ConFastenerPosition>? UserPositions { get; set; }
	}
}
