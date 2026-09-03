namespace NorsokChecker.Services.Norsok64
{
	/// <summary>
	/// Picks the governing load effect per brace — the envelope the results table and the report
	/// are built on. With many load cases the per-LE matrix is unreadable, so the envelope shows
	/// the worst state per brace and points at which one it was; the engineer then goes and looks
	/// at that state.
	///
	/// The rule is the python reference's (ui.html envelopeData), and it is two-stage:
	///   1. a real (non-skipped) row beats a skipped one;
	///   2. among real rows, the highest utilisation governs.
	///
	/// There is deliberately NO `Passed` term. Ordering by utilisation and pass/fail at the same
	/// time — what the previous implementation did — could drop a failing state entirely: a
	/// low-util failing row was promoted over a passing one, then overwritten again by any later
	/// higher-util passing row. Rows that cannot be checked at all (chord overstressed, resistance
	/// ≤ 0) carry +infinity and therefore win on utilisation alone, so no special case is needed.
	/// </summary>
	public static class JointEnvelope
	{
		/// <summary>The governing row for one brace, and the load effect it came from.</summary>
		public sealed class Governing
		{
			public JointCheckRow Row { get; set; } = null!;
			/// <summary>Stable key — load-effect names are user-editable and not guaranteed unique.</summary>
			public int LeId { get; set; }
			/// <summary>What the user reads.</summary>
			public string LeName { get; set; } = "";

			/// <summary>
			/// The state with the SECOND-highest real, finite utilisation on this brace, and its
			/// value — or null when there is no second candidate.
			///
			/// The point is not the state, it is the MARGIN: a reviewer wants to know whether the
			/// selection is robust. A 0.3-point gap means a small change to the model hands the
			/// joint to a different state and the result deserves another look; a 30-point gap means
			/// it does not. That signal cannot be recovered from a dump of every state — nobody
			/// reads 270 rows looking for a near tie — which is why one column carries it and the
			/// dump stays out of the document.
			///
			/// Real and finite, deliberately: a skipped row never governs and so can never be the
			/// runner-up either, and a row that could not be checked at all carries +infinity, which
			/// would otherwise "win" second place while meaning nothing.
			/// </summary>
			public int? RunnerUpLeId { get; set; }
			public string? RunnerUpLeName { get; set; }
			public double? RunnerUpUtil { get; set; }

			/// <summary>
			/// Why there is no runner-up — so the report can say WHICH of three different facts it
			/// is looking at, instead of printing one dash for all of them.
			/// </summary>
			public RunnerUpAbsence Absence { get; set; }
		}

		/// <summary>
		/// Why a brace has no runner-up. Three facts that print identically as a bare dash and mean
		/// different things to a reader deciding whether to trust the governing state.
		/// </summary>
		public enum RunnerUpAbsence
		{
			/// <summary>There is one — <see cref="Governing.RunnerUpUtil"/> holds it.</summary>
			None = 0,

			/// <summary>Only one load effect was evaluated at all, so nothing could come second.</summary>
			SingleState,

			/// <summary>Other states exist but none produced a usable check on this brace.</summary>
			OthersSkipped,
		}

		/// <summary>
		/// Governing row for <paramref name="braceName"/>, or null only when NO load effect has a row
		/// for it at all.
		///
		/// A brace that was skipped in every state still returns its skipped row, carrying the reason.
		/// This used to return null instead, and the caller then added no result of any kind: a
		/// three-brace joint where one brace carries no force in any state published two rows,
		/// counted two checks, and the connection read PASS — with a brace that was never assessed
		/// invisible in the grid, the §6.4 tab and the report alike. Returning the row lets the caller
		/// publish it as NotAssessed, which is what it is.
		/// </summary>
		public static Governing? Pick(
			IEnumerable<PerLoadEffect<JointCheckRow>> perLoadEffect,
			string braceName)
		{
			Governing? best = null;
			// Every state that produced a REAL, FINITE check on this brace, so the runner-up can be
			// found without a second pass over the load effects. Collected here rather than derived
			// afterwards because this loop already visits exactly the right rows.
			var usable = new List<(int Id, string Name, double Util)>();
			int rowsSeen = 0;

			foreach (var le in perLoadEffect)
			{
				var row = le.Rows.FirstOrDefault(x => x.Name == braceName);
				if (row == null) continue;
				rowsSeen++;

				string leName = string.IsNullOrEmpty(le.Name) ? $"LE{le.Id}" : le.Name;
				var candidate = new Governing { Row = row, LeId = le.Id, LeName = leName };

				if (!row.Skipped && !double.IsNaN(row.Util) && !double.IsInfinity(row.Util))
					usable.Add((le.Id, leName, row.Util));

				// Two stages, as the python reference does it (ui.html envelopeData):
				//   1. anything beats nothing — so a skipped row is kept when it is all there is;
				//   2. a real row beats a skipped one, whatever their utilisations;
				//   3. among real rows, the higher utilisation governs.
				if (best == null) { best = candidate; continue; }
				if (row.Skipped) continue;                      // never displaces anything
				if (best.Row.Skipped) { best = candidate; continue; }
				// NaN compares false against everything, so a NaN row never displaces a real one —
				// it can still be picked first, which is the honest outcome when it is all there is.
				if (row.Util > best.Row.Util) best = candidate;
			}

			if (best != null)
				AttachRunnerUp(best, usable, rowsSeen);
			return best;
		}

		/// <summary>
		/// The second-highest usable utilisation, and — when there is none — which of the three
		/// reasons applies. Separate from the loop above so the selection rules stay readable.
		/// </summary>
		private static void AttachRunnerUp(Governing best,
			List<(int Id, string Name, double Util)> usable, int rowsSeen)
		{
			// Exclude the governing state by ID, not by utilisation: two states can tie exactly, and
			// filtering on the value would then drop both and report no runner-up on the one joint
			// where the margin is zero — precisely the case this column exists to surface.
			var others = usable.Where(u => u.Id != best.LeId)
				.OrderByDescending(u => u.Util)
				.ToList();

			if (others.Count > 0)
			{
				var second = others[0];
				best.RunnerUpLeId = second.Id;
				best.RunnerUpLeName = second.Name;
				best.RunnerUpUtil = second.Util;
				best.Absence = RunnerUpAbsence.None;
				return;
			}

			// No second candidate. WHY differs, and the reader acts differently on each.
			best.Absence = rowsSeen <= 1
				? RunnerUpAbsence.SingleState
				: RunnerUpAbsence.OthersSkipped;
		}

		/// <summary>Why this brace has no governing row — the first skip reason found, for the log.</summary>
		public static string? SkipReason(
			IEnumerable<PerLoadEffect<JointCheckRow>> perLoadEffect,
			string braceName)
			=> perLoadEffect
				.SelectMany(le => le.Rows)
				.FirstOrDefault(x => x.Name == braceName && x.Skipped)?.Reason;
	}
}
