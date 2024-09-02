using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete HKG
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.HKG.MatConcreteHKG,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatConcreteHKG : MatConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatConcreteHKG()
		{

		}

		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double Fcu { get; set; }

		/// <summary>
		/// Ultimate compressive strain in the concrete
		/// </summary>
		public double Epscu2 { get; set; }
	}
}