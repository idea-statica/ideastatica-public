using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.RcsClient.Services;
using System.Linq;

namespace RcsApiClient.Services
{
	/// <summary>
	/// Implementation of <see cref="IReinforcedCrosssSectionSelector"/>
	/// It allows to select a reinforced cross-section in a dialog
	/// </summary>
	public class DialogReinforcedCrossSectionSelector : IReinforcedCrosssSectionSelector
	{
		/// <inheritdoc cref="IReinforcedCrosssSectionSelector.Select(RcsProjectModel)"/>
		public int Select(RcsProjectSummary rcsProject)
		{
			return rcsProject.ReinforcedCrossSections.First().Id;

		}
	}
}
