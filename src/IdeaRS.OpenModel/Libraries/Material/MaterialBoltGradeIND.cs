using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material bolt grade
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.MaterialBoltGrade CI.Material", "CI.StructModel.Libraries.Material.IMatBoltGrade, CI.BasicTypes", typeof(MaterialBoltGrade))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MaterialBoltGradeIND : MaterialBoltGrade
	{
		#region Properties
		#endregion Properties
	}
}