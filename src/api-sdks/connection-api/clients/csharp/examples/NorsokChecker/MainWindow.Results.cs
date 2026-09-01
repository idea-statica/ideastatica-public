namespace NorsokChecker
{
	/// <summary>
	/// The Results tab: every check from every chapter, in one flat table.
	///
	/// It reads the results and nothing else. It used to also drive the §6.4 and Report tabs, which
	/// meant neither could refresh without it.
	/// </summary>
	public partial class MainWindow
	{
		private void PopulateResultsTab()
		{
			var all = new List<object>();

			foreach (var (conId, formulas) in _formulaResults)
			{
				var conName = _connections.FirstOrDefault(c => c.Id == conId)?.Name ?? $"Con {conId}";
				// notes and unassessed conditions first, then the checks
				foreach (var fr in formulas.OrderBy(f => f.IsNote || f.NotAssessed ? 0 : 1))
				{
					var row = new
					{
						Connection = conName,
						fr.Section,
						fr.Title,
						fr.Equation,
						LoadCase = !string.IsNullOrEmpty(fr.LoadCaseName) ? fr.LoadCaseName
							: fr.LoadCaseId > 0 ? $"LC{fr.LoadCaseId}" : "envelope",
						Demand = Math.Round(fr.Demand, 2),
						Capacity = Math.Round(fr.Capacity, 2),
						// a utilisation of "0.0 %" next to "not assessed" or a note reads as a result
						Utilization = fr.IsNote || fr.NotAssessed ? "—" : $"{fr.Utilization * 100:F1}%",
						Result = fr.Verdict
					};
					all.Add(row);
				}
			}

			ResultsGrid.ItemsSource = all;
		}

	}
}
