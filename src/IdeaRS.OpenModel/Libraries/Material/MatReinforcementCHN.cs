using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material reinforcement CHN
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.American.MatReinforcementCHN,CI.Material", "CI.StructModel.Libraries.Material.IMatReinforcement,CI.BasicTypes", typeof(MatReinforcement))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatReinforcementCHN : MatReinforcement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatReinforcementCHN()
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
