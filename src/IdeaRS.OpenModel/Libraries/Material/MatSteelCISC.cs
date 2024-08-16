using IdeaRS.OpenModel.Geometry2D;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{

	/// <summary>
	/// Material steel CISC
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.Canadian.MatSteelCAN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	[DataContract]
	public class MatSteelCISC : MatSteel
	{
		/// <summary>
		/// Yield strength for nominal thickness of the element &lt;= 40mm - f<sub>y</sub>
		/// </summary>
		[DataMember]
		public double fy { get; set; }

		/// <summary>
		/// Ultimate strength  for nominal thickness of the element &lt;= 40mm - f<sub>u</sub>
		/// </summary>
		[DataMember]
		public double fu { get; set; }
	}
}