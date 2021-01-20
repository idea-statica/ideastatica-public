namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material steel IND
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.IND.MatSteelIND,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	public class MatSteelIND : MatSteel
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
		public double GammaOVfu { get; set; }

		/// <summary>
		/// Overstrength coefficient for fy
		/// </summary>
		public double GammaOVfy { get; set; }

		/// <summary>
		/// Material strength for specific thickness of plate
		/// </summary>
		public MaterialStrengthProperty MaterialStrength { get; set; }
	}
}