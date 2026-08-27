using NorsokChecker.Services.Norsok64;

namespace NorsokChecker.Models
{
	/// <summary>
	/// One brace's row on the §6.4 tab, shaped like the python reference's results table
	/// (ui.html classChecksCard): classification, resistances, the three utilisation shares, the
	/// verdict — and, in envelope mode, the load effect that governs this brace.
	///
	/// A view model rather than an anonymous type because the tab needs to bind columns explicitly
	/// (the flat grid that used to be here auto-generated them, which is why it showed strings) and
	/// because the row has to carry <see cref="Detail"/> back to the derivation window.
	/// </summary>
	public sealed class Joint64RowView
	{
		public string Brace { get; set; } = "";

		/// <summary>
		/// The design actions this row was checked for — "N_Sd=-10 kN · M_ip=-1.00 kNm · M_op=0.00 kNm".
		///
		/// Shown under the brace name, as the python table has it. Without it the tab reported
		/// resistances and utilisation ratios and not one single action, so there was no way to see
		/// WHAT the joint was checked for — only how much of it was used up.
		/// </summary>
		public string Actions { get; set; } = "";

		/// <summary>
		/// A K sub-row rather than a brace: "↳ K via M5". Indented, and it fills only the K column
		/// plus a note, because X / Y / resistances / utilisation do not apply to one K component.
		/// </summary>
		public bool IsSubRow { get; set; }

		/// <summary>
		/// For a K sub-row: the force this pairing balances and the gap it crosses. For a main row:
		/// the classifier's own explanation of the split, when it has one (e.g. "no transverse
		/// force", or that a near-balanced brace was rounded to 100 % K by the gate).
		/// </summary>
		public string Note { get; set; } = "";

		/// <summary>
		/// The one Notes cell: the skip reason when there is one, otherwise the note. A single
		/// property rather than a PriorityBinding over both — PriorityBinding takes the first
		/// binding that RESOLVES, and a null SkipReason resolves, so it would have shown an empty
		/// cell on every assessed row instead of falling through to the note.
		/// </summary>
		public string Notes => !string.IsNullOrEmpty(SkipReason) ? SkipReason! : Note;

		/// <summary>Governing load effect — envelope mode only; empty in per-LC mode.</summary>
		public string GoverningLe { get; set; } = "";

		// classification: the shares of the brace's axial force. Order K, X, Y as the python
		// table has it (its detail modal uses K, Y, X — a known inconsistency there, not copied).
		public string FrK { get; set; } = "";
		public string FrX { get; set; } = "";
		public string FrY { get; set; } = "";

		public string NRd { get; set; } = "";
		public string MRdIp { get; set; } = "";
		public string MRdOp { get; set; } = "";

		public string UtilAxial { get; set; } = "";
		public string UtilIpb { get; set; } = "";
		public string UtilOpb { get; set; } = "";

		/// <summary>Total utilisation, or an em dash when the brace was not assessed.</summary>
		public string Util { get; set; } = "—";

		/// <summary>PASS / FAIL / N/A — drives the row colour.</summary>
		public string Verdict { get; set; } = "";

		/// <summary>Why nothing could be checked; shown instead of the numbers.</summary>
		public string? SkipReason { get; set; }

		/// <summary>⚠ when the geometry is outside 6.4.3.1, ⛔ when the chord wall is overstressed.</summary>
		public string Flags { get; set; } = "";

		/// <summary>The engine result behind this row, for the derivation window.</summary>
		public JointCheckRow? Detail { get; set; }

		public bool CanShowDetail => !IsSubRow && Detail?.Skipped == false && Detail.Engine != null;
	}

	/// <summary>An entry in the §6.4 tab's load-effect selector.</summary>
	public sealed class Le64Option
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";

		/// <summary>Worst utilisation in this state, for the bar the python selector draws.</summary>
		public double MaxUtil { get; set; }

		/// <summary>True when at least one brace fails here — the ✗ mark in the python selector.</summary>
		public bool AnyFail { get; set; }

		public string Display => MaxUtil > 0
			? $"{Name}   {MaxUtil * 100:F1} %{(AnyFail ? "  ✗" : "")}"
			: Name;
	}
}
