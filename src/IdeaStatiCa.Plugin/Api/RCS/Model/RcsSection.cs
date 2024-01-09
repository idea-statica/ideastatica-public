namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	/// <summary>
	/// Section in a RCS project
	/// </summary>
	public class RcsSection
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int? CheckMemberId { get; set; }
		public int? RCSId { get; set; }
	}
}
