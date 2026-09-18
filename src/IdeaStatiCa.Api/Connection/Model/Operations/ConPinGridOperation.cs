#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Pin grid operation. Connects two or more plates with a pin connection.
	/// Maps to the new-model <c>PinGridOperation</c>.
	/// </summary>
	public class ConPinGridOperation : ConOperation
	{
		public ConPinGridOperation() : base()
		{
			Active = true;
		}

		[JsonConstructor]
		public ConPinGridOperation(int id) : base(id)
		{
			Active = true;
		}

		/// <summary>Plates / members connected by the pin grid.</summary>
		public List<ConConnectedItem> ConnectedItems { get; set; } = new List<ConConnectedItem>();

		/// <summary>Pin catalog ID.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? PinOriginalId { get; set; }

		/// <summary>Whether the pin is exploded into individual entities for analysis.</summary>
		public bool IsExploded { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConGridGeometry? Geometry { get; set; }

		public ConDefinedBy DefinedBy { get; set; }

		/// <summary>Required when <see cref="DefinedBy"/> is <see cref="ConDefinedBy.CoordinateSystem"/> —
		/// a CAD/IOM-import-produced mode accepted only for exploded grids (<see cref="IsExploded"/> with
		/// positions in <see cref="ConGridGeometry.UserPositions"/>); it cannot author a new grid layout.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConLocalCoordinateSystem? CoordinateSystem { get; set; }
	}
}
