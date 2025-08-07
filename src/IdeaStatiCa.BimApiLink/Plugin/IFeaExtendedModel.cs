using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	/// <summary>
	/// Extends standard IFeaModel by possibility to import also load cases not included in combinations
	/// </summary>
	public interface IFeaExtendedModel : IFeaModel
	{
		/// <summary>
		/// Gets all load cases in current model in Fea application
		/// </summary>
		/// <returns>Identifiers of load cases</returns>
		IEnumerable<Identifier<IIdeaLoadCase>> GetAllLoadCases();
	}
}
