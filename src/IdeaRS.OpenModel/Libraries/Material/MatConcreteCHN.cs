
namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete CHN
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.CHN.MatConcreteCHN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	public class MatConcreteCHN : MatConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatConcreteCHN()
		{

		}

		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double Fck { get; set; }
	}
}