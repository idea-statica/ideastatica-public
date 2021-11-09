using Dlubal.RSTAB8;
using System;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Providers
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

		public IEnumerable<LoadCase> GetLoadCases()
		{
			foreach (LoadCase loadCase in _loads.GetLoadCases())
			{
				if (!_loads.HasLoadingResults(loadCase.Loading))
				{
					continue;
				}

				Set(loadCase.Loading.No, DataType.LoadCase, loadCase);
				yield return loadCase;
			}
		}

		public IEnumerable<LoadCombination> GetLoadCombinations()
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
			foreach (ResultCombination resultCombination in _loads.GetResultCombinations())
			{
				if (!_loads.HasLoadingResults(resultCombination.Loading))
				{
					continue;
				}

				Set(resultCombination.Loading.No, DataType.ResultCombination, resultCombination);
				yield return resultCombination;
			}
		}

		public LoadCase GetLoadCase(int no)
		{
			return GetOrCreate(no, DataType.LoadCase, () => _loads.GetLoadCase(no, ItemAt.AtNo).GetData());
		}

		public LoadCombination GetLoadCombination(int no)
		{
			return GetOrCreate(no, DataType.LoadCombination, () => _loads.GetLoadCombination(no, ItemAt.AtNo).GetData());
		}

		public ResultCombination GetResultCombination(int no)
		{
			return GetOrCreate(no, DataType.ResultCombination, () => _loads.GetResultCombination(no, ItemAt.AtNo).GetData());
		}
	}
}