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
	/// decides PASS / FAIL / PARTIAL / N/A, and it has to be checkable on its own.
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
				// The status names only THAT §6.4 does not apply and how many conditions failed; the
				// conditions themselves are one row each in Results and in the report.
				int gates = results.Count(f => !f.IsNote && f.NotAssessed);
				string status = anyNotAssessed
					? (gates > 1 ? $"Outside §6.4 scope ({gates} conditions)" : "Outside §6.4 scope")
					: "Not assessed";
				return new ConnectionVerdict("N/A", 0, status);
			}

			if (anyNotAssessed)
				return new ConnectionVerdict("PARTIAL", maxUtil, "Partly assessed");

			return new ConnectionVerdict("PASS", maxUtil, "Norsok OK");
		}
	}
}
