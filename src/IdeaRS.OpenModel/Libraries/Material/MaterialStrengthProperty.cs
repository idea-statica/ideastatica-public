using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Class defines the strength of material acc. to materieal thickness
	/// </summary>
	[OpenModelClass("IdeaRS.MprlModel.Material.MaterialStrengthProperty,IdeaStatiCa.BasicTypes")]
	[DataContract]
	public class MaterialStrengthProperty : OpenObject
	{
		/// <summary>
		/// Collection of material strength levels
		/// </summary>
		[DataMember]
		public List<MaterialStrength> List { get; set; } = new List<MaterialStrength>();
	}
}
