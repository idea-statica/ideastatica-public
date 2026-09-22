#nullable enable annotations


namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Foundation (concrete) block supporting the base plate. Required for
	/// <see cref="ConFastenerType.Anchors"/> and <see cref="ConFastenerType.Contact"/>
	/// when <see cref="ConFastenerGridOrContactOperation.BlockType"/> is <see cref="ConBlockType.New"/>.
	/// </summary>
	public class ConFoundationBlockDto
	{
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
