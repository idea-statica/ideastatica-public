namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConLoadEffectMemberLoad
	{
		public int MemberId { get; set; }

		public ConLoadEffectPositionEnum Position { get; set; }

		public ConLoadEffectSectionLoad SectionLoad { get; set; }
	}
}