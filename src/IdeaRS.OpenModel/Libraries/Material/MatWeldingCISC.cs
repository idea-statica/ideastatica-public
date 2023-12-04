namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for CISC
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.Canadian.WeldingMaterialCAN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]	 
	public class MatWeldingCISC : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		public double Xu { get; set; }
	}
}
