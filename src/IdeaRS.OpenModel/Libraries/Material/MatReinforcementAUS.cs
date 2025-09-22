using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Ductility class
	/// </summary>
	public enum DuctilityClass
	{
		/// <summary>
		/// Ductility class N type
		/// </summary>
		N,

		/// <summary>
		/// Ductility class L type
		/// </summary>
		L,
	}

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
		/// Epsilon Sy
		/// </summary>
		public double Epssy { get; set; }

		/// <summary>
		/// Characteristic strain of reinforcement
		/// </summary>
		public double Epssu { get; set; }

		/// <summary>
		/// Characteristic yield strength of reinforcement
		/// </summary>
		public double Fy { get; set; }

		/// <summary>
		/// Characteristic tensile strength of reinforcement
		/// </summary>
		public double Fu { get; set; }

		/// <summary>
		/// Characteristic tensile strength of reinforcement
		/// </summary>
		public DuctilityClass DuctilityClass { get; set; }
	}
}
