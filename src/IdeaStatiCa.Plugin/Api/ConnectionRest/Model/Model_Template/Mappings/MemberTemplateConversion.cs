
namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Template
{
	public class MemberTemplateConversion : BaseTemplateConversion
	{
		public bool IsBearing { get; set; } = false;

		public string OriginalMemberName { get; set; }

		public string NewMemberName { get; set; }
	}
}
