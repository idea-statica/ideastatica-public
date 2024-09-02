using IdeaRS.OpenModel.Libraries.Material;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for chinese code
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.CHN.WeldingMaterialCHN,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]
	[DataContract]
	public class MatWeldingCHN : MatWelding
	{
		[DataMember]
		public double Ffw { get; set; }

		[DataMember]
		public double fuw { get; set; }

		[DataMember]
		public WeldLoadTypeCHN Loading { get; set; }
	}
}
