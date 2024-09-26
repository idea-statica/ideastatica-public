using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Strength of material with specific thickness range
	/// </summary>
	[OpenModelClass("IdeaRS.MprlModel.Material.MaterialStrength,CI.BasicTypes")]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MaterialStrength : OpenObject
	{
		/// <summary>
		/// higher value of the thickness range
		/// </summary>
		public double MaxThickness { get; set; }

		/// <summary>
		/// Yield strength
		/// </summary>
		public double Fy { get; set; }

		/// <summary>
		/// Tension strength
		/// </summary>
		public double Fu { get; set; }

		/// <summary>
		/// Design strength
		/// </summary>
		public double F { get; set; }
	}
}
