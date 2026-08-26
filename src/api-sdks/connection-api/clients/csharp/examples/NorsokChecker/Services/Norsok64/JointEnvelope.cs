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
			foreach (var le in perLoadEffect)
			{
				var row = le.Rows.FirstOrDefault(x => x.Name == braceName);
				if (row == null) continue;

				var candidate = new Governing
				{
					Row = row,
					LeId = le.Id,
					LeName = string.IsNullOrEmpty(le.Name) ? $"LE{le.Id}" : le.Name,
				};

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
			return best;
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
