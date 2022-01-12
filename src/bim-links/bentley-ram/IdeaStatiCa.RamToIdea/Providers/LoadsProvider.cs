using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Providers
{
	internal class LoadsProvider : ILoadsProvider
	{
		private readonly ILoadCases _loadCases;
		private readonly ILoadCombinations _combinations;

		public LoadsProvider(IModel model)
		{
			_loadCases = model.GetLoadCases(EAnalysisResultType.RAMFrameResultType);
			_combinations = model.GetLoadCombinations(COMBO_MATERIAL_TYPE.ADVANCED_ANALYSIS_STRENGTH);
		}

		public IEnumerable<ILoadCase> GetLoadCases()
		{
			var count = _loadCases.GetCount();
			for (int i = 0; i < count; i++)
			{
				yield return _loadCases.GetAt(i);
			}
		}

		public IEnumerable<ILoadCombination> GetLoadCombinations()
		{
			var count = _combinations.GetCount();
			for (int i = 0; i < count; i++)
			{
				yield return _combinations.GetAt(i);
			}
		}

		public ILoadCase GetLoadCase(int uid)
		{
			return _loadCases.Get(uid);
		}

		public ILoadCombination GetLoadCombination(int index)
		{
			return _combinations.GetAt(index);
		}
	}
}