namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for indian code
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.IS.WeldingMaterialIS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]
	public class MatWeldingIS : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		public double fu { get; set; }
	}
}
