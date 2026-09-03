using NorsokChecker.Models;

namespace NorsokChecker.Services
{
	/// <summary>
	/// One connection's headline verdict, rolled up from the individual check results.
	///
	/// This is the number the app exists to produce, and until it was extracted it lived inside a
	/// click handler (MainWindow's RunCheck_Click), where it could not be tested without building a
	/// window and simulating a click — so it never was.
	/// </summary>
	internal sealed record ConnectionVerdict(string Pass, double MaxUtilisation, string Status);

	/// <summary>
	/// The rules that turn a connection's check results into its verdict.
	///
	/// A pure function over the results: no UI, no API, no state. That is the point — the roll-up
	/// decides PASS / FAIL / PARTIAL / QUALIFIED / N/A, and it has to be checkable on its own.
	/// </summary>
	internal static class CheckWorkflow
	{
		/// <summary>
		/// Roll one connection's results up into its verdict.
		///
		/// THREE outcomes, not two. A "not assessed" row is neither a pass nor a failure, so it is
		/// counted as neither — and a connection carrying one cannot be reported as PASS, because
		/// part of it was never checked. That distinction is the reason this is not a one-liner:
		/// treating an unassessed brace as passing would report a green connection nobody checked.
		///
		/// A NOTE qualifies a check that DID run (a warning, an assumption). It is neither a result
		/// nor a gap, so it takes no part in the roll-up at all.
		/// </summary>
		internal static ConnectionVerdict Roll(IReadOnlyList<NorsokFormulaResult> results)
		{
			double maxUtil = 0;
			bool anyFailed = false;
			bool anyNotAssessed = false;
			int assessed = 0;

			foreach (var fr in results)
			{
				if (fr.IsNote) continue;

				if (fr.NotAssessed)
				{
					anyNotAssessed = true;
				}
				else
				{
					assessed++;
					if (fr.Utilization > maxUtil) maxUtil = fr.Utilization;
					if (!fr.Passed) anyFailed = true;
				}
			}

			if (anyFailed)
				return new ConnectionVerdict("FAIL", maxUtil, "Norsok FAIL");

			if (assessed == 0)
			{
				// Nothing was checked at all. An empty result set used to leave the connection
				// reading "Norsok OK / PASS / 0.0 %" — a pass awarded for the absence of a check.
				//
				// WHICH kind of "nothing" matters, and the roll-up used to flatten it: every
				// unassessed row was reported as "Outside §6.4 scope" regardless of why, so a
				// connection whose load effects would not read was told the chapter did not cover
				// it — while its own detail card said "could not be evaluated". The two disagreed in
				// the shipped report, and only one of them was true.
				var gateRows = results.Where(f => !f.IsNote && f.NotAssessed).ToList();
				int gates = gateRows.Count;
				bool blocked = gateRows.Any(f => f.Reason == NotAssessedReason.NotEvaluated);

				// Nothing to check, and WHICH nothing. All three are NotEvaluated rows — each is
				// fixed by editing the model — but they say different things about what the reader
				// will find there, and "could not be read" about a joint that read perfectly well is
				// a false statement about their model.
				bool allOff = gateRows.Any(f =>
					(f.CheckExpression ?? "").Contains("switched off",
						StringComparison.OrdinalIgnoreCase));
				bool noLoad = gateRows.Any(f =>
					(f.CheckExpression ?? "").Contains("no load effect",
						StringComparison.OrdinalIgnoreCase));

				string status =
					!anyNotAssessed ? "Not assessed"
					: allOff ? "Not assessed — every load effect switched off"
					: noLoad ? "Not assessed — no load effect defined"
					// A blocked input wins over a scope gate when both are present: the scope verdict
					// was reached on inputs we know are incomplete, so it is not trustworthy.
					: blocked ? "Not evaluated — the model could not be read"
					: gates > 1 ? $"Outside §6.4 scope ({gates} conditions)"
					: "Outside §6.4 scope";

				return new ConnectionVerdict("N/A", 0, status);
			}

			if (anyNotAssessed)
				return new ConnectionVerdict("PARTIAL", maxUtil, "Partly assessed");

			// A check that RAN but whose geometry lies outside the §6.4.3.1 validity ranges. The
			// resistance is an extrapolation of formulas fitted inside those ranges, so the pass is
			// real but qualified — and the qualifier has to reach the row an engineer scans, not only
			// the card sixty pages in. Until this branch existed the overview read a clean
			// "PASS / Norsok OK" for a joint whose own detail card said "outside validity range".
			//
			// Named parameters, joined, rather than a count: "outside validity range (2 conditions)"
			// tells the reader to go looking, whereas "M1: θ = 20.0°, outside 30–90°" is the answer.
			var qualifiers = results
				.Where(f => f.IsQualified)
				.Select(f => f.RangeQualifier!)
				.Distinct()
				.ToList();

			if (qualifiers.Count > 0)
				return new ConnectionVerdict(
					"QUALIFIED", maxUtil,
					"Outside §6.4.3.1 validity range — " + string.Join(" · ", qualifiers));

			return new ConnectionVerdict("PASS", maxUtil, "Norsok OK");
		}
	}
}
