using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Providers
{
	internal interface ILoadsProvider
	{
		IEnumerable<ILoadCase> GetLoadCases();

		IEnumerable<ILoadCombination> GetLoadCombinations();

		ILoadCase GetLoadCase(int uid);

		ILoadCombination GetLoadCombination(int index);
	}
}