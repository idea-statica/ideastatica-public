using IdeaRS.OpenModel.Libraries.Material;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for chinese code
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.CHN.WeldingMaterialCHN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]
	public class MatWeldingCHN : MatWelding
	{
		public double Ffw { get; set; }		
		public double fuw { get; set; }
		public WeldLoadTypeCHN Loading { get; set; }
	}
}
