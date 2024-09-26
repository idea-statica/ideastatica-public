using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BimApiLinkFeaExample.FeaExampleApi
{
	public interface IFeaResultsApi
	{
		IEnumerable<IFeaMemberResult> GetMemberInternalForces(int memberId, int loadCaseId);		
	}

	internal class FeaResultsApi : IFeaResultsApi
	{
		private Dictionary<int, List<IFeaMemberResult>> _resultsForMembers = new Dictionary<int, List<IFeaMemberResult>>();

		public FeaResultsApi() 
		{
			LoadResultsFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResultsData.json"));
		}

		public IEnumerable<IFeaMemberResult> GetMemberInternalForces(int memberId, int loadCaseId)
		{
			return _resultsForMembers[memberId].Where(x => x.LoadCaseId == loadCaseId);			
		}

		private void LoadResultsFromFile(string filePath)
		{
			string jsonString = File.ReadAllText(filePath);
			List<FeaMemberResult> allResults = JsonConvert.DeserializeObject<List<FeaMemberResult>>(jsonString);
			_resultsForMembers = allResults.GroupBy(result => result.MemberId)
						.ToDictionary(group => group.Key, group => group.Cast<IFeaMemberResult>().ToList());
		}
	}		
}
