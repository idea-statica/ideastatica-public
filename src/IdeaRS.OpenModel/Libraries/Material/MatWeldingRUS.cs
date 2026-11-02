using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for russian code
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.RUS.WeldingMaterialRUS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]
	[DataContract]
	public class MatWeldingRUS : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		[DataMember]
		public double Rwun { get; set; }

		/// <summary>
		/// Welding type
		/// </summary>
		[DataMember]
		public WeldingTypeSNIP WeldingType { get; set; }

		/// <summary>
		/// Flat position
		/// </summary>
		[DataMember]
		public bool FlatPosition { get; set; }
	}
}
