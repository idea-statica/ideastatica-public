using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete CAN
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.RUS.MatConcreteRUS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatConcreteRUS : MatConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatConcreteRUS()
		{

		}

		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double Fck { get; set; }
	}
}