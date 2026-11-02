using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material bolt grade
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.HKG.MaterialBoltGradeHKG, CI.Material", "CI.StructModel.Libraries.Material.IMatBoltGradeHKG, CI.BasicTypes", typeof(MaterialBoltGrade))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MaterialBoltGradeHKG : MaterialBoltGrade
	{
		#region Properties

		/// <summary>
		/// Design shear strength
		/// </summary>
		public double Ps { get; set; }

		/// <summary>
		/// Design bearing strength
		/// </summary>
		public double Pbb { get; set; }

		/// <summary>
		/// Design tension strength
		/// </summary>
		public double Pt { get; set; }

		#endregion Properties
	}
}