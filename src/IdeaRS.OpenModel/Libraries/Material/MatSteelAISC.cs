using IdeaRS.OpenModel.Geometry2D;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{

	/// <summary>
	/// Material steel AISC
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.American.MatSteelASTM,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatSteelAISC : MatSteel
	{
		/// <summary>
		/// Yield strength for nominal thickness of the element &lt;= 40mm - f<sub>y</sub>
		/// </summary>
		public double fy { get; set; }

		/// <summary>
		/// Ultimate strength  for nominal thickness of the element &lt;= 40mm - f<sub>u</sub>
		/// </summary>
		public double fu { get; set; }

		/// <summary>
		/// Yield strength for nominal thickness of the element &gt; 40mm and &lt;= 100mm - f<sub>y,(&gt;40)</sub>
		/// </summary>
		public double fy40 { get; set; }

		/// <summary>
		/// Ultimate strength for nominal thickness of the element &gt; 40mm and &lt;= 100mm - f<sub>u,(&gt;40)</sub>
		/// </summary>
		public double fu40 { get; set; }

	}
}