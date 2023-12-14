using IdeaStatiCa.Plugin.Api.RCS.Model;

namespace IdeaStatiCa.RcsClient.Services
{
	/// <summary>
	/// Provide  reinforced cross-section from RCS project
	/// </summary>
	public interface IReinfCssSelector
	{
		/// <summary>
		/// Returns ID of a selected reinforced cross-section from <paramref name="rcsProject"/>
		/// </summary>
		/// <param name="rcsProject">RCS project</param>
		/// <returns>Id of a selecte reinforced cross-section. The retun valu -1 means that nothing was selected</returns>
		int Select(RcsProjectSummaryModel rcsProject);
	}
}
