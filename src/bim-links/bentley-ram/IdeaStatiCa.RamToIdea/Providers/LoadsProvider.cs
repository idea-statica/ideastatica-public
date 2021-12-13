using RAMDATAACCESSLib;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Providers
{
	internal class LoadsProvider : ILoadsProvider, IDataCache
	{
		private enum DataType
		{
			LoadCase,
			LoadCombination,
			ResultCombination
		}

		private readonly Dictionary<(DataType, int), object> _data = new Dictionary<(DataType, int), object>();
		private readonly ILoads _loads;

		public LoadsProvider(IModel model)
		{
			_loads = model.GetLoads();
		}

		public void Clear()
		{
			_data.Clear();
		}

		private T GetOrCreate<T>(int no, DataType dataType, Func<T> factory)
		{
			if (_data.TryGetValue((dataType, no), out object data))
			{
				return (T)data;
			}

			data = factory();
			Set(no, dataType, data);

			return (T)data;
		}

		private void Set(int no, DataType dataType, object data)
		{
			_data[(dataType, no)] = data;
		}

		public IEnumerable<ILoadCase> GetLoadCases()
		{
			foreach (ILoadCase loadCase in _loads.GetLoadCases())
			{
				if (!_loads.HasLoadingResults(loadCase.eAnalyzedState))
				{
					continue;
				}

				Set(loadCase.Loading.No, DataType.LoadCase, loadCase);
				yield return loadCase;
			}
		}

		public IEnumerable<ILoadCombination> GetLoadCombinations()
		{
			LoadCombination[] combis = _loads.GetLoadCombinations();
			foreach (LoadCombination loadCombination in combis)
			{
				if (!_loads.HasLoadingResults(loadCombination.Loading))
				{
					continue;
				}

				Set(loadCombination.Loading.No, DataType.LoadCombination, loadCombination);
				yield return loadCombination;
			}
		}

		public IEnumerable<ResultCombination> GetResultCombinations()
		{
			for (int i = 0; i < _loads.GetResultCombinations().Length; i++)
			{
				ResultCombination resultCombination = _loads.GetResultCombinations()[i];
				if (!_loads.HasLoadingResults(resultCombination.Loading))
				{
					continue;
				}

				CombinationLoading[] rfCombiItems = _loads.GetResultCombination(i, ItemAt.AtIndex).GetLoadings();
				int numCombiItems = rfCombiItems.Length;
				bool ex = false;
				for (int j = 0; j < numCombiItems; j++)
				{
					if (rfCombiItems[j].Loading.Type == Dlubal.RSTAB8.LoadingType.LoadCaseType || (rfCombiItems[j].Loading.Type == LoadingType.LoadCombinationType))
					{
						ex = true;
					}
				}
				if (ex)
				{
					Set(resultCombination.Loading.No, DataType.ResultCombination, resultCombination);
					yield return resultCombination;
				}
			}
		}

		public ILoadCase GetLoadCase(int no)
		{
			return GetOrCreate(no, DataType.LoadCase, () => _loads.GetLoadCase(no, ItemAt.AtNo).GetData());
		}

		public ILoadCombination GetLoadCombination(int no)
		{
			return GetOrCreate(no, DataType.LoadCombination, () => _loads.GetLoadCombination(no, ItemAt.AtNo).GetData());
		}

		//public ResultCombination GetResultCombination(int no)
		//{
		//	return GetOrCreate(no, DataType.ResultCombination, () => _loads.GetResultCombination(no, ItemAt.AtNo).GetData());
		//}
	}
}
