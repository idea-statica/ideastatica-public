using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaStatiCa.RcsClient.Services;
using System.Linq;

namespace RcsApiClient.Services
{
	/// <summary>
	/// Implementation of <see cref="IReinfCssSelector"/>
	/// It allows to select a reinforced cross-section in a dialog
	/// </summary>
	public class DialogReinforcedCssSelector : IReinfCssSelector
	{
		/// <inheritdoc cref="IReinfCssSelector.Select(RcsProjectModel)"/>
		public int Select(RcsProjectSummaryModel rcsProject)
		{
			return rcsProject.ReinforcedCrossSections.First().Id;

		}
	}
}
