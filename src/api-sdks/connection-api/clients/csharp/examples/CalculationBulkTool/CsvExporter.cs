using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationBulkTool
{
	public static class CsvExporter
	{
		public static string ConvertToCsv(IEnumerable<CalculationResults> results)
		{
			var sb = new StringBuilder();
			sb.AppendLine("File,Project item,Check status for bolts,Summary - Analysis, Summary - Bolts, Item (bolt),Interaction of tension and shear,Utilization in shear,Utilization in tension");

			foreach (var project in results)
			{
				bool firstProjectLine = true;

				foreach (var conn in project.ProjectItems)
				{
					bool firstConnectionLine = true;

					foreach (var bolt in conn.BoltResults)
					{
						sb.AppendLine(string.Join(",",
						new[]
						{
					firstProjectLine ? project.FileName : "",
					firstConnectionLine ? conn.Name : "",
					firstConnectionLine ? (conn.Succes ? "OK" : "NOT OK!") : "",	
					firstConnectionLine ? $"Analysis: {conn.Analysis}" : "",
					firstConnectionLine ? $"Bolts: {conn.Bolts}" : "",
					bolt.Item?.Replace(",", " "),
					bolt.UtilizationInInteraction.ToString("0.###"),
					bolt.UtilizationInShear.ToString("0.###"),
					bolt.UtilizationInTension.ToString("0.###")
						}));

						firstProjectLine = false;
						firstConnectionLine = false;
					}
				}
			}

			return sb.ToString();
		}

	}

}
