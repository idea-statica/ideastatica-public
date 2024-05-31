namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	public class ConLoadEffectMemberLoad
	{
		public int MemberId { get; set; }

		public ConLoadEffectSectionLoad Begin { get; set; }

		public ConLoadEffectSectionLoad End { get; set; }
	}
}