using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Providers
{
	public interface ILoadsProvider
	{
		IEnumerable<ILoadCase> GetLoadCases();

		IEnumerable<ILoadCombination> GetLoadCombinations();

		ILoadCase GetLoadCase(int uid);
	}
}