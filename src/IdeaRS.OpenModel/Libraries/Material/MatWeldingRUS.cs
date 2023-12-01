namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for russian code
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.RUS.WeldingMaterialRUS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]	 
	public class MatWeldingRUS : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		public double Rwun { get; set; }

		/// <summary>
		/// Welding type
		/// </summary>
		public WeldingTypeSNIP WeldingType { get; set; }

		/// <summary>
		/// Flat position
		/// </summary>
		public bool FlatPosition { get; set; }
	}
}
