using Dlubal.RSTAB8;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Providers
{
	internal interface ILoadsProvider
	{
		IEnumerable<LoadCase> GetLoadCases();

		IEnumerable<LoadCombination> GetLoadCombinations();

		IEnumerable<ResultCombination> GetResultCombinations();

		LoadCase GetLoadCase(int no);

		LoadCombination GetLoadCombination(int no);

		ResultCombination GetResultCombination(int no);
	}
}