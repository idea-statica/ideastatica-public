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
		/// Governing row for <paramref name="braceName"/>, or null when every load effect skipped it.
		/// </summary>
		public static Governing? Pick(
			IEnumerable<PerLoadEffect<JointCheckRow>> perLoadEffect,
			string braceName)
		{
			Governing? best = null;
			foreach (var le in perLoadEffect)
			{
				var row = le.Rows.FirstOrDefault(x => x.Name == braceName);
				if (row == null || row.Skipped) continue;
				// NaN compares false against everything, so a NaN row never displaces a real one —
				// it can still be picked first, which is the honest outcome when it is all there is.
				if (best != null && !(row.Util > best.Row.Util)) continue;

				best = new Governing
				{
					Row = row,
					LeId = le.Id,
					LeName = string.IsNullOrEmpty(le.Name) ? $"LE{le.Id}" : le.Name,
				};
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
