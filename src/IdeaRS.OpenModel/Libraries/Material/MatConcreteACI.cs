using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete ACI
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.American.MatConcreteACI,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatConcreteACI : MatConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatConcreteACI()
		{

		}

		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double Fcc { get; set; }

		/// <summary>
		/// ε_0
		/// </summary>
		public double Eps0 { get; set; }
	}
}
