using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material headed stud grade
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.MaterialHeadedStudGrade,CI.Material", "CI.StructModel.Libraries.Material.IMatHeadedStudGrade,CI.BasicTypes", typeof(MaterialHeadedStudGrade))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MaterialHeadedStudGradeAUS : MaterialHeadedStudGrade
	{
		#region Properties
		#endregion Properties
	}
}