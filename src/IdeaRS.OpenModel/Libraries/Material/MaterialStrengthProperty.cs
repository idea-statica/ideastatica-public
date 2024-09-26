using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Class defines the strength of material acc. to materieal thickness
	/// </summary>
	[OpenModelClass("IdeaRS.MprlModel.Material.MaterialStrengthProperty,CI.BasicTypes")]
	[DataContract]
	public class MaterialStrengthProperty : OpenObject
	{
		/// <summary>
		/// Collection of material strength levels
		/// </summary>
		[DataMember]
		public List<MaterialStrength> List { get; set; }
	}
}
