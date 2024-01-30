namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	/// <summary>
	/// Data for updating loading in a RCS section
	/// </summary>
	public class RcsSectionLoading
	{
		/// <summary>
		/// Id of a section to update in a RCS section
		/// </summary>
		public int SectionId { get; set; }

		/// <summary>
		/// XML string which represent list of extremes in a section
		/// </summary>
		public string LoadingXml { get; set; }
	}
}
