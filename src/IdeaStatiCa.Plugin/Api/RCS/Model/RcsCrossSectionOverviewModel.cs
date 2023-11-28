namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsCrossSectionOverviewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int? CheckMemberId { get; set; }
		public int? RCSId { get; set; }
	}
}
