using Dlubal.RSTAB8;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaRstabPlugin.Providers
{
	/// <summary>
	/// This class loads results from RSTAB in load-by-load matter, i.e. give me results for all members for this load,
	/// instead of in member-by-member, i.e. give me results for all loads on this member.
	///
	/// The load-by-load approach is much faster.
	///
	/// To get results for a member you must first call Prefetch.
	/// </summary>
	internal class ResultsProvider : IResultsProvider
	{
		private readonly Dictionary<int, List<(Loading, MemberForces)>> _results = new Dictionary<int, List<(Loading, MemberForces)>>();
		private readonly List<int> _membersToFetch = new List<int>();
		private readonly ILoadsProvider _loadsProvider;
		private readonly ICalculation _calculation;

		public ResultsProvider(ILoadsProvider loadsProvider, ICalculation calculation)
		{
			_loadsProvider = loadsProvider;
			_calculation = calculation;
		}

		public void Prefetch(int memberNo)
		{
			_membersToFetch.Add(memberNo);
		}

		public void Clear()
		{
			_results.Clear();
		}

		public IEnumerable<(Loading, MemberForces)> GetInternalForces(int memberNo)
		{
			FetchAll();

			if (_results.TryGetValue(memberNo, out List<(Loading, MemberForces)> cachedResults))
			{
				return cachedResults;
			}

			throw new InvalidOperationException($"Did not receive a prefetch call for member '{memberNo}'");
		}

		private void FetchAll()
		{
			foreach (int memberNo in _membersToFetch)
			{
				_results[memberNo] = new List<(Loading, MemberForces)>();
			}

			foreach (Loading loading in GetLoadings())
			{
				IResults rstabResults = _calculation.GetResults(loading.Type, loading.No);
				//rstabResults.AddRibsToMembers(true);

				foreach (int memberNo in _membersToFetch)
				{
					foreach (MemberForces memberForces in rstabResults.GetMemberInternalForces(memberNo, ItemAt.AtNo, true))
					{
						_results[memberNo].Add((loading, memberForces));
					}
				}
			}

			_membersToFetch.Clear();
		}

		private IEnumerable<Loading> GetLoadings()
		{
			return _loadsProvider.GetLoadCases().Select(x => x.Loading)
				.Concat(_loadsProvider.GetLoadCombinations().Select(x => x.Loading));
		}
	}
}