using System.Globalization;
using System.Text;

namespace CalculationBulkTool
{
	public static class CsvExporter
	{
		private static string SanitizeName(string? input)
		{
			if(string.IsNullOrWhiteSpace(input))
			{
				return string.Empty;
			}

			return input.Replace(",", "-");
		}

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
						[
							firstProjectLine ? SanitizeName(project?.FileName) : "",
							firstConnectionLine ? SanitizeName(conn.Name) : "",
							firstConnectionLine ? (conn.Succes ? "OK" : "NOT OK!") : "",
							firstConnectionLine ? $"{conn.AnalysisType}" : "",
							firstConnectionLine ? conn.Analysis?.ToString("0.###", CultureInfo.InvariantCulture) : "",
							firstConnectionLine ? conn.Bolts?.ToString("0.###", CultureInfo.InvariantCulture) : "",
							firstConnectionLine ? conn.Welds?.ToString("0.###", CultureInfo.InvariantCulture) : "",
							firstConnectionLine ? conn.Plates?.ToString("0.###", CultureInfo.InvariantCulture) : "",
							firstConnectionLine ? conn.PreloadedBolts?.ToString("0.###", CultureInfo.InvariantCulture) : "",
							bolt.Item?.Replace(",", " "),
							bolt.UtilizationInInteraction.ToString("0.###", CultureInfo.InvariantCulture),
							bolt.UtilizationInShear.ToString("0.###", CultureInfo.InvariantCulture),
							bolt.UtilizationInTension.ToString("0.###", CultureInfo.InvariantCulture)
						]));

						firstProjectLine = false;
						firstConnectionLine = false;
					}
				}
				else
				{
					sb.AppendLine(string.Join(",",
						[
						SanitizeName(project?.FileName),
						SanitizeName(conn.Name),
						conn.Succes ? "OK" : "NOT OK!",
						$"{conn.AnalysisType}",
						conn.Analysis?.ToString("0.###", CultureInfo.InvariantCulture),
						conn.Bolts?.ToString("0.###", CultureInfo.InvariantCulture),
						conn.Welds?.ToString("0.###", CultureInfo.InvariantCulture),
						conn?.Plates?.ToString("0.###", CultureInfo.InvariantCulture),
						conn?.PreloadedBolts?.ToString("0.###", CultureInfo.InvariantCulture),
						"",
						"",
						"",
						""
						]));
				}
			}

			return sb.ToString();
		}

	}

}
