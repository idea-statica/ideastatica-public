using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for australian code
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.AUS.WeldingMaterialAUS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]
	[DataContract]
	public class MatWeldingAUS : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		[DataMember]
		public double fu { get; set; }
	}
}
