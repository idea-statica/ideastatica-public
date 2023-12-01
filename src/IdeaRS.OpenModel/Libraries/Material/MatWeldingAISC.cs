namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for AISC
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.American.WeldingMaterialAISC,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]
	public class MatWeldingAISC : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		public double Fexx { get; set; }
	}
}
