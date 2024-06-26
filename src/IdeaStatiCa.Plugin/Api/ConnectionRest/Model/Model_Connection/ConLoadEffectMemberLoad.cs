namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	public class ConLoadEffectMemberLoad
	{
		public int MemberId { get; set; }

		public ConLoadEffectPositionEnum Position { get; set; }

		public ConLoadEffectSectionLoad SectionLoad { get; set; }
	}
}