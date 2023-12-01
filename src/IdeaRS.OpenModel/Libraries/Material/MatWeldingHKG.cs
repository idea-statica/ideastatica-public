
namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for hong kongese code
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.HKG.WeldingMaterialHKG,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]	 
	public class MatWeldingHKG : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		public double Ue { get; set; }

		public bool EcenElectrode { get; set; }
	}
}
