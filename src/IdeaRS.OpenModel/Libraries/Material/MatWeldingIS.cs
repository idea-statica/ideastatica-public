using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for indian code
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.IS.WeldingMaterialIS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]
	[DataContract]
	public class MatWeldingIS : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		[DataMember]
		public double fu { get; set; }
	}
}
