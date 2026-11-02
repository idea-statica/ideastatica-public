
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for hong kongese code
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.HKG.WeldingMaterialHKG,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatWelding))]
	[DataContract]
	public class MatWeldingHKG : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		[DataMember]
		public double Ue { get; set; }

		[DataMember]
		public bool EcenElectrode { get; set; }
	}
}
