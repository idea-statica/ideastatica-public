using IdeaRS.OpenModel;
using System.Runtime.Serialization;

namespace IdeaStatiCa.Plugin
{
	[DataContract]
	public class BIMProject
	{
		[DataMember]
		public CountryCode CountryCode { get; set; }
	}
}