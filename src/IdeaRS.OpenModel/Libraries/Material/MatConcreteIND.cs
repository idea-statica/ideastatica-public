using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete IND
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.IND.MatConcreteIND,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatConcreteIND : MatConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatConcreteIND()
		{

		}

		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double Fck { get; set; }
	}
}