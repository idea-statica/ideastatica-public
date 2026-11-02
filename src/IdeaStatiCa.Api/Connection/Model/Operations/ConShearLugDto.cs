#nullable enable annotations


namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Shear lug welded under the base plate, transferring shear into the foundation block.
	/// Present on <see cref="ConFoundationBlockDto.ShearLug"/> only when the block carries one;
	/// null means no shear lug.
	/// </summary>
	public class ConShearLugDto
	{
		/// <summary>Cross-section catalog ID of the lug profile.</summary>
		public int? CrossSectionId { get; set; }

		/// <summary>Lug length below the base plate [m].</summary>
		public double Length { get; set; }

		/// <summary>Lug position along the X axis of the base plate [m].</summary>
		public double PositionX { get; set; }

		/// <summary>Lug position along the Y axis of the base plate [m].</summary>
		public double PositionY { get; set; }

		/// <summary>Lug rotation about the base-plate normal [rad].</summary>
		public double Rotation { get; set; }

		/// <summary>Weld of the lug flanges to the base plate.</summary>
		public ConWeldData? FlangesWeld { get; set; }

		/// <summary>Weld of the lug webs to the base plate.</summary>
		public ConWeldData? WebsWeld { get; set; }
	}
}
