namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material steel CHN
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.CHN.MatSteelCHN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	public class MatSteelCHN : MatSteel
	{
		/// <summary>
		/// Yield strength
		/// </summary>
		public double fy { get; set; }

		/// <summary>
		/// Ultimate strength
		/// </summary>
		public double fu { get; set; }

		/// <summary>
		/// Overstrength coefficient for fu
		/// </summary>
		public double PhiOMFu { get; set; }

		/// <summary>
		/// Overstrength coefficient for fy
		/// </summary>
		public double PhiOMFy { get; set; }

		/// <summary>
		/// Material strength for specific thickness of plate
		/// </summary>
		public MaterialStrengthProperty MaterialStrength { get; set; }
	}
}