using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	[OpenModelClass("CI.StructModel.Libraries.CrossSection.CrossSection,CI.CrossSection", "CI.StructModel.Libraries.CrossSection.ICrossSection,CI.BasicTypes", typeof(CrossSection))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class GeneralCrossSection : CrossSection
	{
		public List<GcssComponent> Components { get; set; }

		public GeneralCrossSection()
		{
			Components = new List<GcssComponent>();
		}
	}
}
