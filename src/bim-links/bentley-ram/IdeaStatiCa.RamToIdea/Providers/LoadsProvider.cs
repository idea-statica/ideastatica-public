using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Providers
{
	internal class LoadsProvider : ILoadsProvider
	{
		private readonly IModel _model;
		private readonly List<ILoadCase> _loadCases;

		public LoadsProvider(IModel model)
		{
			_model = model;
			_loadCases = ReadLoadCases(model);
		}

		private List<ILoadCase> ReadLoadCases(IModel model)
		{
			var ramLoadCases = model.GetLoadCases(EAnalysisResultType.RAMFrameResultType);
			var count = ramLoadCases.GetCount();
			var loadCases = new List<ILoadCase>(count);
			System.Diagnostics.Debug.WriteLine($"Read {count} load cases");
			for (int i = 0; i < count; i++)
			{
				var lc = ramLoadCases.GetAt(i);
				if(lc.eAnalyzedState == EStateStatus.eStateNotAvail || lc.eAnalyzedState == EStateStatus.eStateNotCurrent || lc.lAnalyzeNo == -1)
				{
					System.Diagnostics.Debug.WriteLine($"Skipping load case {lc.lUID}");
					continue;
				}

				System.Diagnostics.Debug.WriteLine($"Load case {lc.lUID} - {lc.eAnalyzedState}");
				loadCases.Add(lc);
			}

			return loadCases;
		}

		public IEnumerable<ILoadCase> GetLoadCases() => _loadCases;

		public IEnumerable<ILoadCombination> GetLoadCombinations()
		{
			var combinations = new List<ILoadCombination>();

			combinations.AddRange(GetLoadCombinationsInternal(COMBO_MATERIAL_TYPE.ANALYSIS_CUSTOM));
			combinations.AddRange(GetLoadCombinationsInternal(COMBO_MATERIAL_TYPE.GRAV_STEEL));
			combinations.AddRange(GetLoadCombinationsInternal(COMBO_MATERIAL_TYPE.ADVANCED_ANALYSIS_SERVICE));
			combinations.AddRange(GetLoadCombinationsInternal(COMBO_MATERIAL_TYPE.ADVANCED_ANALYSIS_STRENGTH));
			combinations.AddRange(GetLoadCombinationsInternal(COMBO_MATERIAL_TYPE.STEEL_CUSTOM));

			return combinations;
		}

		public ILoadCase GetLoadCase(int uid)
		{
			return _loadCases.Find(lc => lc.lUID == uid);
		}

		private IEnumerable<ILoadCombination> GetLoadCombinationsInternal(COMBO_MATERIAL_TYPE type)
		{
			var combinations = _model.GetLoadCombinations(type);
			var count = combinations.GetCount();
			System.Diagnostics.Debug.WriteLine($"Read combination: {type}, number of combi: {count}");
			for (int i = 0; i < count; i++)
			{
				yield return combinations.GetAt(i);
			}
		}
	}
}