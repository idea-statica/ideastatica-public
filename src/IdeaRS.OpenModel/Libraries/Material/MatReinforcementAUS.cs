using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material reinforcement AUS
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.American.MatReinforcementAUS,CI.Material", "CI.StructModel.Libraries.Material.IMatReinforcement,CI.BasicTypes", typeof(MatReinforcement))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatReinforcementAUS : MatReinforcement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatReinforcementAUS()
		{
		}

		/// <summary>
		/// Characteristic strain of reinforcement
		/// </summary>
		public double Epssu { get; set; }

		/// <summary>
		/// Characteristic yield strength of reinforcement
		/// </summary>
		public double Fy { get; set; }
	}
}
