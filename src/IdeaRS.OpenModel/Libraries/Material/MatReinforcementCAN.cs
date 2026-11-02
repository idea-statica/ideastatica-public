using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Grade type
	/// </summary>
	public enum GradeType
	{
		/// <summary>
		/// R
		/// </summary>
		Regular,

		/// <summary>
		/// W
		/// </summary>
		Weldable
	}


	/// <summary>
	/// Material reinforcement CAN
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.American.MatReinforcementCAN,CI.Material", "CI.StructModel.Libraries.Material.IMatReinforcement,CI.BasicTypes", typeof(MatReinforcement))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatReinforcementCAN : MatReinforcement
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatReinforcementCAN()
		{
		}

		/// <summary>
		/// Characteristic yield strength of reinforcement
		/// </summary>
		public double Fsy { get; set; }

		/// <summary>
		/// Characteristic tensile strength of reinforcement
		/// </summary>
		public double Fsu { get; set; }

		/// <summary>
		/// Strain at tensile strength ε su
		/// </summary>
		public double Epssu { get; set; }

		/// <summary>
		/// Grade type
		/// </summary>
		public GradeType GradeType { get; set; }
	}
}
