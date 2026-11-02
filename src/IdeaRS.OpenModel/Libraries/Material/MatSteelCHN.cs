using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material steel CHN
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.CHN.MatSteelCHN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	[DataContract]
	public class MatSteelCHN : MatSteel
	{
		/// <summary>
		/// Yield strength
		/// </summary>
		[DataMember]
		public double fy { get; set; }

		/// <summary>
		/// Ultimate strength
		/// </summary>
		[DataMember]
		public double fu { get; set; }

		/// <summary>
		/// Overstrength coefficient for fu
		/// </summary>
		[DataMember]
		public double PhiOMFu { get; set; }

		/// <summary>
		/// Overstrength coefficient for fy
		/// </summary>
		[DataMember]
		public double PhiOMFy { get; set; }

		/// <summary>
		/// Material strength for specific thickness of plate
		/// </summary>
		[DataMember]
		public MaterialStrengthProperty MaterialStrength { get; set; }
	}
}