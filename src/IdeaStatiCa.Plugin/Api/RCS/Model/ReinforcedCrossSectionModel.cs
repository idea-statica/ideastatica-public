namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	/// <summary>
	/// Reinforced cross-section in RCS project
	/// </summary>
	public class ReinforcedCrossSectionModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int CrossSectionId { get; set; }
	}
}
