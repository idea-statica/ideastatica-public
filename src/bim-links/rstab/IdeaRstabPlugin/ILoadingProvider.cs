//using Dlubal.RSTAB8;
//using System.Collections.Generic;

//namespace IdeaRstabPlugin
//{
//	internal class LoadingProvider
//	{
//		private readonly List<Loading> _loadings;

//		public LoadingProvider(ILoads loads)
//		{
//			foreach (LoadCase loadCase in loads.GetLoadCases())
//			{
//				_loadings.Add(loadCase.Loading);
//			}

//			foreach (LoadCombination loadCombination in loads.GetLoadCombinations())
//			{
//				_loadings.Add(loadCombination.Loading);
//			}
//		}

//		public IEnumerable<Loading> GetLoadings()
//		{
//			return _loadings;
//		}
//	}
//}