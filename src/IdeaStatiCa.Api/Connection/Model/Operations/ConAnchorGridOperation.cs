#nullable enable annotations

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Anchor grid operation. Anchors a base plate to a foundation block.
	/// Maps to the new-model <c>AnchorGridOperation</c>.
	///
	/// <para><b>Field usage by anchor sub-type:</b></para>
	/// <list type="bullet">
	/// <item><b>ThreadedRod / GeneralAnchor / HookedAnchor</b>: <see cref="AnchorAssemblyId"/>,
	/// <see cref="EmbedmentDepth"/>, <see cref="HookLength"/> (HookedAnchor).</item>
	/// <item><b>HeadedStud</b>: <see cref="AnchorDiameter"/>, <see cref="HeadDiameter"/>,
	/// <see cref="EmbedmentDepth"/>, <see cref="HeadedStudMaterialId"/>.</item>
	/// <item><b>WasherPlate</b>: <see cref="AnchorAssemblyId"/>, <see cref="EmbedmentDepth"/>,
	/// <see cref="WasherPlateShape"/>, <see cref="WasherPlateSize"/>.</item>
	/// <item><b>Reinforcement</b>: <see cref="ReinforcementMaterialId"/>, <see cref="ReinforcementShape"/>,
	/// <see cref="AnchorDiameter"/> (rebar diameter), <see cref="EmbedmentDepth"/>,
	/// <see cref="MandrelDiameter"/>, <see cref="HookLength"/>, <see cref="HookRotations"/>.</item>
	/// </list>
	/// </summary>
	public class ConAnchorGridOperation : ConOperation
	{
		public ConAnchorGridOperation() : base()
		{
			Active = true;
		}

		[JsonConstructor]
		public ConAnchorGridOperation(int id) : base(id)
		{
			Active = true;
		}

		/// <summary>Plates / members connected by the anchor grid.</summary>
		public List<ConConnectedItem> ConnectedItems { get; set; } = new List<ConConnectedItem>();

		public ConAnchorType AnchorType { get; set; }

		/// <summary>Anchor assembly catalog ID (for ThreadedRod / GeneralAnchor / HookedAnchor / WasherPlate).</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? AnchorAssemblyId { get; set; }

		/// <summary>Embedment depth [m] (the GUI "Embedment depth"), for all anchor sub-types.</summary>
		public double EmbedmentDepth { get; set; }

		/// <summary>Hook length [m] for <see cref="ConAnchorType.HookedAnchor"/> and <see cref="ConAnchorType.Reinforcement"/>.</summary>
		public double HookLength { get; set; }

		/// <summary>Shaft diameter [m] for <see cref="ConAnchorType.HeadedStud"/>; rebar diameter for <see cref="ConAnchorType.Reinforcement"/>.</summary>
		public double AnchorDiameter { get; set; }

		/// <summary>Head diameter [m] for <see cref="ConAnchorType.HeadedStud"/>.</summary>
		public double HeadDiameter { get; set; }

		/// <summary>Headed-stud grade material ID for <see cref="ConAnchorType.HeadedStud"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? HeadedStudMaterialId { get; set; }

		// --- WasherPlate ---

		/// <summary>Required when <see cref="AnchorType"/> is <see cref="ConAnchorType.WasherPlate"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConWasherPlateShape? WasherPlateShape { get; set; }

		/// <summary>Washer plate size [m] — diameter for circular, edge length for rectangular.</summary>
		public double WasherPlateSize { get; set; }

		// --- Reinforcement ---

		/// <summary>Reinforcement material catalog ID. Used for <see cref="ConAnchorType.Reinforcement"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? ReinforcementMaterialId { get; set; }

		/// <summary>Hook shape (L / U). Used for <see cref="ConAnchorType.Reinforcement"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConReinforcementAnchorShape? ReinforcementShape { get; set; }

		/// <summary>Mandrel diameter [m]. Used for <see cref="ConAnchorType.Reinforcement"/>.</summary>
		public double MandrelDiameter { get; set; }

		/// <summary>Per-position hook rotations [radians]. Used for <see cref="ConAnchorType.Reinforcement"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<double>? HookRotations { get; set; }

		// --- Geometry ---

		public bool IsExploded { get; set; }

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
		/// Used by the slotted-hole-capable sub-types (ThreadedRod / GeneralAnchor / HookedAnchor / WasherPlate).
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<ConSlottedHole>? SlottedHoles { get; set; }

		// --- Foundation block ---

		/// <summary>New block properties. Required when <see cref="BlockType"/> is <see cref="ConBlockType.New"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConFoundationBlockDto? FoundationBlock { get; set; }

		public ConBlockType BlockType { get; set; } = ConBlockType.New;

		/// <summary>Operation ID of the existing block. Required when <see cref="BlockType"/> is <see cref="ConBlockType.Existing"/>.</summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? ExistingBlockOperationId { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public ConPlateSide? PlateSide { get; set; }
	}
}
