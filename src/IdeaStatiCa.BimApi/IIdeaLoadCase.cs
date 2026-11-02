using IdeaRS.OpenModel.Loading;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaLoadCase : IIdeaLoading
	{
		/// <summary>
		/// Load case type
		/// </summary>
		LoadCaseType LoadType { get; }

		/// <summary>
		/// Sub type
		/// </summary>
		LoadCaseSubType Type { get; }

		/// <summary>
		/// Variable type
		/// </summary>
		VariableType Variable { get; }

		/// <summary>
		/// Load group
		/// </summary>
		IIdeaLoadGroup LoadGroup { get; }

		/// <summary>
		/// Additional info
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Loads on surface assigned to this load case
		/// </summary>
		IEnumerable<IIdeaLoadOnSurface> LoadsOnSurface { get; }
	}
}