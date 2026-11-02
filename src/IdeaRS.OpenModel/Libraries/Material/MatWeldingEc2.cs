using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for ECEN
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.ECEN.WeldingMaterialECEN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]
	[DataContract]
	public class MatWeldingEc2 : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		[DataMember]
		public double fu { get; set; }

		/// <summary>
		/// BetaW value
		/// </summary>
		[DataMember]
		public double BetaW { get; set; }
	}
}
