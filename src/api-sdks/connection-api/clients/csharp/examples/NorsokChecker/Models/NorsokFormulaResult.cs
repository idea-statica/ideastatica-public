namespace NorsokChecker.Models
{
	/// <summary>
	/// Why a check was not carried out. Two states that are opposite in what the reader must do.
	/// </summary>
	public enum NotAssessedReason
	{
		/// <summary>The row is a check that ran, or a note. No reason applies.</summary>
		None = 0,

		/// <summary>
		/// The chapter does not cover this joint, and never will: a permanent property of the
		/// geometry or the section type (no through chord, an overlap joint, a non-tubular member,
		/// a brace out of the plane). The reader's move is to use another method — EN 1993-1-8, say.
		/// </summary>
		OutsideScope,

		/// <summary>
		/// The chapter may well apply, but the inputs could not be produced: load effects missing,
		/// a member list that would not read, no section data. The reader's move is to fix the model
		/// or the input and run again — nothing about the joint has been ruled out.
		/// </summary>
		NotEvaluated,
	}

	/// <summary>
	/// Result of evaluating a single Norsok formula (e.g., §6.3.2 Axial Tension).
	/// Contains the formula reference, all populated variable values, and the verdict.
	///
	/// Report output mimics IDEA StatiCa CHECK tab format:
	///   formula → substituted values → "Where:" block explaining each variable
	/// </summary>
	public class NorsokFormulaResult
	{
		/// <summary>Section reference, e.g. "6.3.2"</summary>
		public string Section { get; set; } = string.Empty;

		/// <summary>Equation number, e.g. "6.1"</summary>
		public string Equation { get; set; } = string.Empty;

		/// <summary>Title, e.g. "Axial Tension"</summary>
		public string Title { get; set; } = string.Empty;

		/// <summary>The check expression, e.g. "N_Sd ≤ N_t,Rd"</summary>
		public string CheckExpression { get; set; } = string.Empty;

		/// <summary>The formula with symbols, e.g. "N_t,Rd = A · f_y / γ_M"</summary>
		public string Formula { get; set; } = string.Empty;

		/// <summary>The formula with substituted numbers, e.g. "N_t,Rd = 30159 × 355 / 1.15 = 9310 kN"</summary>
		public string FormulaSubstituted { get; set; } = string.Empty;

		/// <summary>All variable values used in the formula evaluation.</summary>
		public List<FormulaVariable> Variables { get; set; } = new();

		/// <summary>The demand value (left-hand side), e.g. N_Sd</summary>
		public double Demand { get; set; }

		/// <summary>The capacity value (right-hand side), e.g. N_t,Rd</summary>
		public double Capacity { get; set; }

		/// <summary>Utilization ratio = Demand / Capacity</summary>
		public double Utilization { get; set; }

		/// <summary>True if check passes (utilization ≤ 1.0)</summary>
		public bool Passed { get; set; }

		/// <summary>
		/// The check was not carried out — the conditions for it are not met. This is a THIRD state,
		/// distinct from both pass and fail: "not assessed" and "FAIL" cannot both be true, and
		/// reporting one as the other is what made the results table self-contradictory. When this
		/// is set, <see cref="Passed"/> and <see cref="Utilization"/> carry no meaning.
		/// </summary>
		public bool NotAssessed { get; set; }

		/// <summary>
		/// An informational note rather than a check: an assumption the check rests on, or a
		/// parameter clamped to the §6.4.3.1 range. Distinct from <see cref="NotAssessed"/> — the
		/// joint WAS assessed, so a note must not make the connection read as unchecked.
		/// </summary>
		public bool IsNote { get; set; }

		/// <summary>
		/// WHY the check was not carried out — only meaningful when <see cref="NotAssessed"/> is set.
		///
		/// Two reasons that look identical in the data and are opposite in what the reader must do,
		/// which is why they need separating. The report said BOTH about the same connection: the
		/// overview row read "Outside §6.4 scope" while the detail card read "could not be
		/// evaluated". A reviewer cannot act on that — the first says use another method, the second
		/// says fix the model and run again.
		/// </summary>
		public NotAssessedReason Reason { get; set; }

		/// <summary>
		/// A check that RAN but whose result carries a caveat — today, geometry outside the §6.4.3.1
		/// validity ranges. Null when there is none.
		///
		/// A FIELD, deliberately, and not re-derived from the card title. The qualifier used to exist
		/// only as text appended to <see cref="Title"/>, so the roll-up could not see it: a connection
		/// whose brace sits at θ = 20° reported "PASS / Norsok OK" in the overview while its own card
		/// said "outside validity range (6.4.3.1)" sixty pages later. The overview is what an engineer
		/// scans, so that is where the caveat has to arrive; a state carried in a sentence cannot be
		/// rolled up, only re-parsed, and re-parsing a display string is how the two drifted apart.
		///
		/// Names the parameter and its value ("M1: θ = 20.0°, outside 30–90°") rather than saying
		/// "outside range": the reader's next question is always WHICH parameter, and the answer is
		/// already known where this is set.
		/// </summary>
		public string? RangeQualifier { get; set; }

		/// <summary>
		/// True when the check ran and produced a usable result, but <see cref="RangeQualifier"/>
		/// qualifies it. Neither a pass nor a failure on its own — it modifies a pass.
		/// </summary>
		public bool IsQualified => !IsNote && !NotAssessed && !string.IsNullOrEmpty(RangeQualifier);

		/// <summary>PASS / FAIL / NOTE / N/A — the single place that decides the wording.</summary>
		public string Verdict => IsNote ? "NOTE" : NotAssessed ? "N/A" : Passed ? "PASS" : "FAIL";

		/// <summary>Load case ID (0 = envelope/all). For per-LC breakdown.</summary>
		public int LoadCaseId { get; set; }

		/// <summary>
		/// Display name of the governing load effect. Names are user-editable and not guaranteed
		/// unique, so <see cref="LoadCaseId"/> stays the key — this is only what gets shown.
		/// </summary>
		public string? LoadCaseName { get; set; }

		/// <summary>
		/// Full §6.4 auto-topology check detail (engine result, classification, chord-stress trail).
		/// Set only by the auto-topology path; the HTML report renders the derivation blocks from it.
		/// </summary>
		public Services.Norsok64.JointCheckRow? JointDetail { get; set; }

		/// <summary>Generates a report string mimicking IDEA StatiCa CHECK tab format.</summary>
		public string ToReportString()
		{
			var sb = new System.Text.StringBuilder();
			string passSymbol = Passed ? "✓" : "✗";
			string passText = Passed ? "PASS" : "FAIL";

			// ── Header ──
			sb.AppendLine($"┌─────────────────────────────────────────────────────────");
			sb.AppendLine($"│ NORSOK N-004 §{Section} — {Title}   (Eq. {Equation})");
			sb.AppendLine($"├─────────────────────────────────────────────────────────");

			// ── Check condition ──
			sb.AppendLine($"│");
			sb.AppendLine($"│  Check:  {CheckExpression}");

			// ── Formula (symbolic) ──
			if (!string.IsNullOrEmpty(Formula))
			{
				sb.AppendLine($"│");
				sb.AppendLine($"│  {Formula}");
			}

			// ── Formula (substituted values) ──
			if (!string.IsNullOrEmpty(FormulaSubstituted))
			{
				sb.AppendLine($"│  {FormulaSubstituted}");
			}

			// ── Where block ──
			if (Variables.Count > 0)
			{
				sb.AppendLine($"│");
				sb.AppendLine($"│  Where:");
				foreach (var v in Variables)
				{
					sb.AppendLine($"│    {v.Symbol,-16} = {v.FormattedValue,-16}  — {v.Description}");
				}
			}

			// ── Result ──
			sb.AppendLine($"│");
			sb.AppendLine($"│  Utilization:  {Demand:G5} / {Capacity:G5} = {Utilization:F4}");
			sb.AppendLine($"│  Result:       {Utilization:F4} ≤ 1.0  →  {passSymbol} {passText}");
			sb.AppendLine($"└─────────────────────────────────────────────────────────");

			return sb.ToString();
		}
	}

	public class FormulaVariable
	{
		public string Symbol { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public double Value { get; set; }
		public string Unit { get; set; } = string.Empty;

		public string FormattedValue => Unit switch
		{
			"MPa" => $"{Value:F1} {Unit}",
			"kN" => $"{Value:F1} {Unit}",
			"kNm" => $"{Value:F2} {Unit}",
			"mm" => $"{Value:F1} {Unit}",
			"mm²" => $"{Value:F0} {Unit}",
			"mm³" => $"{Value:F0} {Unit}",
			"mm⁴" => $"{Value:F0} {Unit}",
			"-" => $"{Value:F4}",
			_ => $"{Value:G6} {Unit}".Trim()
		};
	}
}
