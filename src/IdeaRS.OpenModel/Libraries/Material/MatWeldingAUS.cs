namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for australian code
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.AUS.WeldingMaterialAUS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]
	public class MatWeldingAUS : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		public double fu { get; set; }
	}
}
