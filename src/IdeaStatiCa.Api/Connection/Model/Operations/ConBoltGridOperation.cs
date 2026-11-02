#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Bolt grid operation. Connects two or more plates with a grid of bolts.
	/// Maps to the new-model <c>BoltGridOperation</c>.
	/// </summary>
	public class ConBoltGridOperation : ConOperation
	{
		public ConBoltGridOperation() : base()
		{
			Active = true;
		}

		[JsonConstructor]
		public ConBoltGridOperation(int id) : base(id)
		{
			Active = true;
		}

		/// <summary>Plates / members connected by the bolt grid.</summary>
		public List<ConConnectedItem> ConnectedItems { get; set; } = new List<ConConnectedItem>();

		/// <summary>Bolt assembly catalog ID (e.g. "M16 8.8").</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? BoltAssemblyId { get; set; }

		public ConBoltShearTransfer BoltShearForceTransfer { get; set; }

		/// <summary>Whether the bolts are exploded into individual entities for analysis.</summary>
		public bool IsExploded { get; set; }

		/// <summary>Grid layout and positions.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConGridGeometry? Geometry { get; set; }

		public ConDefinedBy DefinedBy { get; set; }

		/// <summary>Required when <see cref="DefinedBy"/> is <see cref="ConDefinedBy.CoordinateSystem"/> —
		/// a CAD/IOM-import-produced mode accepted only for exploded grids (<see cref="IsExploded"/> with
		/// positions in <see cref="ConGridGeometry.UserPositions"/>); it cannot author a new grid layout.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConLocalCoordinateSystem? CoordinateSystem { get; set; }

		/// <summary>
		/// Grid-level default slotted holes, one entry per connected plate. Applies to every
		/// position; a position can override them via <see cref="ConFastenerPosition.SlottedHoles"/>.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<ConSlottedHole>? SlottedHoles { get; set; }
	}
}
