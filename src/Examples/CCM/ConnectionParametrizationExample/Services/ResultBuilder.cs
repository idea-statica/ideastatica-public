using IdeaRS.OpenModel.Connection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConnectionParametrizationExample.Services
{
	/// <summary>
	/// Store and build results
	/// </summary>
	public class ResultBuilder
	{
		List<string> results;
		List<string> resultsSummaryItems = new List<string> { "Analysis", "Plates", "Loc. deformation", "Bolts", "Anchors", "Preloaded bolts", "Welds", "Concrete block", "Shear", "Buckling" };
		string resultSeperator;

		public ResultBuilder(string resultSeperator = ";")
		{
			results = new List<string>();
			this.resultSeperator = resultSeperator;
		}

		//public void AddResult(string connectionName, double calculationTime, List<CheckResSummary> resultSummary, List<KeyValuePair<string, object>> combination, int combinationIndex)
		public void AddResult(ConnectionResultInfo resultInfo)
		{
			if (!results.Any())
			{
				// Create new list for a key
				results = new List<string>();

				// Add headers
				List<string> headers = new List<string>();
				headers.Add("Connection name");
				headers.Add("Combination index");
				headers.Add("Time [s]");
				headers.Add("Load coefficient");
				headers.AddRange(resultsSummaryItems);
				headers.AddRange(resultInfo.CombinationValues.Select(y => y.Key));
				results.Add(string.Join(resultSeperator, headers));
			}

			// Add values
			List<string> resultValues = new List<string>();
			resultValues.Add(resultInfo.ConnectionName);
			resultValues.Add(resultInfo.CombinationIndex.ToString());
			resultValues.Add(resultInfo.CalculationTime.ToString());
			resultValues.Add(resultInfo.LoadCoefficient.ToString());
			AddResultValues(resultValues, resultInfo.Summary);
			resultValues.AddRange(resultInfo.CombinationValues.Select(y => y.Value.ToString()));
			results.Add(string.Join(resultSeperator, resultValues));
		}

		private void AddResultValues(List<string> resultValues, List<CheckResSummary> resultSummary)
		{
			foreach(string key in resultsSummaryItems)
			{
				var result = resultSummary.FirstOrDefault(y => y.Name == key);
				if (result != null)
				{
					resultValues.Add(result.CheckValue.ToString());
				}
				else
				{
					resultValues.Add(string.Empty);
				}
			}
		}

		public void WriteAllResultsToCsv(string path)
		{
			string resultFilePath = Path.Combine(path, "Results.csv");
			File.WriteAllLines(resultFilePath, results);
		}

		public void WriteDataToCsv(Dictionary<string, List<string>> data, string csvPath)
		{
			var csvText = GetDictionaryKeysAsHeaders(data);
			File.WriteAllLines(csvPath, csvText);
		}

		private IEnumerable<string> GetDictionaryKeysAsHeaders(Dictionary<string, List<string>> data)
		{
			int rowCount = data.Max(pair => pair.Value.Count);

			// Headers
			yield return string.Join(",", data.Select(pair => pair.Key));

			// Rows
			for (int r = 0; r < rowCount; ++r)
			{
				yield return string.Join(",", data.Select(pair => r < pair.Value.Count ? pair.Value[r] : ""));
			}
		}
	}
}
