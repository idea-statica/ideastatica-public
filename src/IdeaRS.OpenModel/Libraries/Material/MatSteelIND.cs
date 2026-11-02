using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material steel IND
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.IND.MatSteelIND,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	[DataContract]
	public class MatSteelIND : MatSteel
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
		public double GammaOVfu { get; set; }

		/// <summary>
		/// Overstrength coefficient for fy
		/// </summary>
		[DataMember]
		public double GammaOVfy { get; set; }

		/// <summary>
		/// Material strength for specific thickness of plate
		/// </summary>
		[DataMember]
		public MaterialStrengthProperty MaterialStrength { get; set; }
	}
}