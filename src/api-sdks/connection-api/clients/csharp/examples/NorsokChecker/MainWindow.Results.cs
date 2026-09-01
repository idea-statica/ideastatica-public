using NorsokChecker.Models;

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
		/// <summary>
		/// Put one connection's results on the tab as a completed run would, so a test can read back
		/// what the table decided.
		///
		/// The rows are anonymous types built inside PopulateResultsTab and exposed only through the
		/// grid, so there is no other way in. The alternative — a test that rebuilds the same rows —
		/// would keep passing after the real one changed.
		/// </summary>
		internal void SetResultsForTest(int connectionId, string name, List<NorsokFormulaResult> results)
		{
			_connections.Add(new ConnectionCheckResult { Id = connectionId, Name = name });
			_formulaResults[connectionId] = results;
			PopulateResultsTab();
		}

		/// <summary>
		/// A result title with the per-chapter detail taken off, for the overview.
		///
		/// A §6.4 row arrives as "Tubular Joint — M1 (K 0% / Y 0% / X 100%) — outside validity range
		/// (6.4.3.1)". Both trailers belong to the §6.4 tab, which has a column for the K/Y/X split
		/// and states the validity range in its own right; repeated down an overview they make the
		/// widest column in the table out of the part nobody reads across fifteen rows.
		///
		/// Cuts at the em dash rather than pattern-matching the contents: whatever a chapter appends
		/// after one is its own elaboration, so this works for a chapter that has not been written
		/// yet. The subject before the first dash is what identifies the row.
		/// </summary>
		internal static string ShortTitle(string? title)
		{
			if (string.IsNullOrEmpty(title)) return "";

			// Two things can start the elaboration: a bracket ("(K 0% / …") or a SECOND em dash
			// ("— outside validity range"). The first em dash is part of the subject
			// ("Tubular Joint — M1"), so it is skipped before looking for the second.
			int bracket = title.IndexOf('(');

			int firstDash = title.IndexOf(" — ", StringComparison.Ordinal);
			int secondDash = firstDash < 0
				? -1
				: title.IndexOf(" — ", firstDash + 3, StringComparison.Ordinal);

			int cut = (bracket, secondDash) switch
			{
				(< 0, < 0) => -1,
				(< 0, _) => secondDash,
				(_, < 0) => bracket,
				_ => Math.Min(bracket, secondDash),
			};

			return cut < 0 ? title.Trim() : title[..cut].Trim();
		}

		private void PopulateResultsTab()
		{
			var all = new List<object>();

			foreach (var (conId, formulas) in _formulaResults)
			{
				var conName = _connections.FirstOrDefault(c => c.Id == conId)?.Name ?? $"Con {conId}";

				// A joint outside the scope of a chapter gets ONE row, not one per unmet condition.
				//
				// It used to get one each, so CON6 filled seven rows and CON2 one — a difference that
				// looks like seven checks against one, when in both cases the answer is the same: the
				// chapter does not apply. This table is the overview; the conditions themselves are
				// listed in the §6.4 tab's banner and as their own cards in the report, which is where
				// someone asking "why not?" is going to look.
				var rejections = formulas.Where(f => f.NotAssessed && !f.IsNote).ToList();
				var rows = formulas.AsEnumerable();
				if (rejections.Count > 1)
				{
					all.Add(new
					{
						Connection = conName,
						rejections[0].Section,
						// The COUNT, as the orientation it is — "a lot" or "one thing" — and nothing
						// more. Which conditions is a question for the §6.4 tab and the report; here it
						// filled seven rows for one joint and one for another, reading as seven checks
						// against one when the answer in both cases is that the chapter does not apply.
						Title = $"Outside the scope of §{rejections[0].Section}"
							+ $" — {rejections.Count} conditions not met",
						LoadCase = "—",
						rejections[0].Equation,
						Utilization = "—",
						Result = rejections[0].Verdict,
					});
					rows = rows.Except(rejections);
				}

				// notes and unassessed conditions first, then the checks
				foreach (var fr in rows.OrderBy(f => f.IsNote || f.NotAssessed ? 0 : 1))
				{
					bool noResult = fr.IsNote || fr.NotAssessed;

					var row = new
					{
						Connection = conName,
						fr.Section,

						// Title only, stripped of what other columns already carry. A §6.4 row's title
						// arrives as "Tubular Joint — M1 (K 0% / Y 0% / X 100%) — outside validity
						// range (6.4.3.1)": the K/Y/X split is a column of its own on the §6.4 tab and
						// the validity note is that tab's business too, so here they made the widest
						// column in an overview out of detail nobody reads across fifteen rows.
						Title = ShortTitle(fr.Title),

						// A load case only where there IS one, and next to the Section it qualifies
						// rather than after the equation number. "envelope" was printed for every row
						// with no load-case id, including rows for a joint that was never assessed —
						// an envelope is a set of load cases, so naming one on a row that has none
						// states something that did not happen.
						LoadCase = !string.IsNullOrEmpty(fr.LoadCaseName) ? fr.LoadCaseName
							: fr.LoadCaseId > 0 ? $"LC{fr.LoadCaseId}"
							: noResult ? "—" : "envelope",

						fr.Equation,

						// Demand and Capacity are gone. For a §6.4 row the adapter sets Demand to the
						// utilisation itself and Capacity to a constant 1 (Joint64ReportAdapter), so
						// the pair restated the Utilization column in a second form and added a column
						// that is the same on every row. On an unassessed row they were "0 / 0", which
						// reads as a measurement.
						Utilization = noResult ? "—" : $"{fr.Utilization * 100:F1}%",
						Result = fr.Verdict
					};
					all.Add(row);
				}
			}

			ResultsGrid.ItemsSource = all;
		}

	}
}
