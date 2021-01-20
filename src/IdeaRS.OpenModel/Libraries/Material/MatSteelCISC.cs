using IdeaRS.OpenModel.Geometry2D;

namespace IdeaRS.OpenModel.Material
{

	/// <summary>
	/// Material steel CISC
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.Canadian.MatSteelCAN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
  public class MatSteelCISC : MatSteel
	{
		/// <summary>
		/// Yield strength for nominal thickness of the element &lt;= 40mm - f<sub>y</sub>
		/// </summary>
		public double fy { get; set; }

		/// <summary>
		/// Ultimate strength  for nominal thickness of the element &lt;= 40mm - f<sub>u</sub>
		/// </summary>
		public double fu { get; set; }
	}
}