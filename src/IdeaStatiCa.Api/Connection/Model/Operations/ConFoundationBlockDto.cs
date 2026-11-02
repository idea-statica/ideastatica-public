#nullable enable annotations

using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Foundation (concrete) block supporting the base plate: either a new block defined here, or an
	/// existing block produced by another operation. The definition fields apply to a new block only.
	/// </summary>
	public class ConFoundationBlockDto
	{
		/// <summary>Whether the operation creates a new block or uses an existing one.</summary>
		public ConBlockType BlockType { get; set; } = ConBlockType.New;

		/// <summary>
		/// Operation that produced the existing block (e.g. a base plate with a block). Required when
		/// <see cref="BlockType"/> is <see cref="ConBlockType.Existing"/>, not allowed otherwise.
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? ExistingBlockOperationId { get; set; }

		/// <summary>Concrete material catalog ID.</summary>
		public int ConcreteMaterialId { get; set; }

		/// <summary>Block offset on the top side [m].</summary>
		public double OffsetTop { get; set; }

		/// <summary>Block offset on the bottom side [m].</summary>
		public double OffsetBottom { get; set; }

		/// <summary>Block offset on the left side [m].</summary>
		public double OffsetLeft { get; set; }

		/// <summary>Block offset on the right side [m].</summary>
		public double OffsetRight { get; set; }

		/// <summary>Block height [m].</summary>
		public double Height { get; set; }

		public ConShearForceTransferMethod ShearForceTransfer { get; set; }

		public ConBasePlateContactType ContactType { get; set; }

		/// <summary>Mortar thickness [m] (used when <see cref="ContactType"/> is <see cref="ConBasePlateContactType.Mortar"/>).</summary>
		public double MortarThickness { get; set; }

		/// <summary>Shear lug welded under the base plate, or null when the block has none.</summary>
		public ConShearLugDto? ShearLug { get; set; }
	}
}
