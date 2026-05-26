using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete CAN
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.Canadian.MatConcreteCAN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatConcreteCAN : MatConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatConcreteCAN()
		{
		}

		/// <summary>
		/// F'c - compressive strength fo concrete
		/// </summary>
		public double Fcc { get; set; }

		/// <summary>
		/// Fr - flexural tensile strength of concrete
		/// </summary>
		public double Fr { get; set; }

		/// <summary>
		/// Fct - axial tensile strength of concrete
		/// </summary>
		public double Fct { get; set; }

		/// <summary>
		/// ε_cp - strain at peak stress
		/// </summary>
		public double Epscp { get; set; }

		/// <summary>
		/// ε_cu - limit strain
		/// </summary>
		public double Epscu { get; set; }
	}
}
