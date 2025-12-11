using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace CalculationBulkTool
{
	public static class CsvExporter
	{
		public static string ConvertToCsv(CalculationResults project, bool includeHeader = false)
		{
			var sb = new StringBuilder();

			if (includeHeader)
			{
				sb.AppendLine("File,Project item,Check status for bolts,Analysis type, Summary - Analysis, Summary - Bolts, Summary - Welds, Summary - Plates, Summary - Preloaded bolts, Item (bolt),Interaction of tension and shear,Utilization in shear,Utilization in tension");
			}

			bool firstProjectLine = true;

			foreach (var conn in project.ProjectItems)
			{
				bool firstConnectionLine = true;

				if (conn.BoltResults.Any())
				{
					foreach (var bolt in conn.BoltResults)
					{
						sb.AppendLine(string.Join(",",
						new[]
						{
							firstProjectLine ? project?.FileName ?? "" : "",
							firstConnectionLine ? conn.Name : "",
							firstConnectionLine ? (conn.Succes ? "OK" : "NOT OK!") : "",
							firstConnectionLine ? $"{conn.AnalysisType}" : "",
							firstConnectionLine ? $"{conn.Analysis}" : "",
							firstConnectionLine ? $"{conn.Bolts}" : "",
							firstConnectionLine ? $"{conn.Welds}" : "",
							firstConnectionLine ? $"{conn.Plates}" : "",
							firstConnectionLine ? $"{conn.PreloadedBolts}" : "",
							bolt.Item?.Replace(",", " "),
							bolt.UtilizationInInteraction.ToString("0.###"),
							bolt.UtilizationInShear.ToString("0.###"),
							bolt.UtilizationInTension.ToString("0.###")
						}));

						firstProjectLine = false;
						firstConnectionLine = false;
					}
				}
				else
				{
					sb.AppendLine(string.Join(",",
						new[]
						{
						project?.FileName ?? "",
						conn.Name,
						conn.Succes ? "OK" : "NOT OK!",
						$"{conn.AnalysisType}",
						$"{conn.Analysis}",
						$"{conn.Bolts}",
						$"{conn.Welds}",
						$"{conn?.Plates}",
						$"{conn?.PreloadedBolts}",
						"",
						"",
						"",
						""
						}));
				}
			}

			return sb.ToString();
		}

	}

}
