using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete AUS
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.AUS.MatConcreteAUS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatConcreteAUS : MatConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatConcreteAUS()
		{

		}

		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double Fcc { get; set; }

		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double Fcm { get; set; }
		
		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double EpsilonCp { get; set; }
	}
}