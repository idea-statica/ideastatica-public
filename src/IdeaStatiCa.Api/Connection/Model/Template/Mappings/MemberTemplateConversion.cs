
namespace IdeaStatiCa.Api.Connection.Model
{
	public class MemberTemplateConversion : BaseTemplateConversion
	{
		public bool IsBearing { get; set; } = false;

		public string OriginalMemberName { get; set; }

		public string NewMemberName { get; set; }
	}
}
