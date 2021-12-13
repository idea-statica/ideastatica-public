using RAMDATAACCESSLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdea.Providers
{
	internal interface ILoadsProvider
	{
		IEnumerable<ILoadCase> GetLoadCases();

		IEnumerable<ILoadCombination> GetLoadCombinations();

		//IEnumerable<ResultCombination> GetResultCombinations();

		ILoadCase GetLoadCase(int no);

		ILoadCombination GetLoadCombination(int no);

		//IResultCombination GetResultCombination(int no);
	}
}
