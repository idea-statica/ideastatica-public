
using System.Runtime.Serialization;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class MemberTemplateConversion : BaseTemplateConversion
	{
		[DataMember]
		public bool IsBearing { get; set; } = false;

		[DataMember]
		public string OriginalMemberName { get; set; }

		[DataMember]
		public string NewMemberName { get; set; }
	}
}
