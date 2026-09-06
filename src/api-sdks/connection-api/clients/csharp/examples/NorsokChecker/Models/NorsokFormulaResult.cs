namespace NorsokChecker.Models
{
	/// <summary>
	/// Why a check was not carried out — and, where the app can tell them apart, WHICH kind of
	/// "nothing to check" it was. States that differ in what the reader must do next.
	///
	/// The three NotEvaluated refinements exist because the roll-up needs them. They were once
	/// carried only in the row's sentence, and <see cref="Services.CheckWorkflow"/> recovered them
	/// by matching that sentence (`Contains("switched off")`). Rewording a sentence then changed a
	/// verdict, and swapping two sentences swapped two verdicts with every test still green — so
	/// the reason a joint went unassessed now travels as this enum and the sentence is display text.
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

		/// <summary>
		/// The model read perfectly well and every load effect in it is switched OFF. Nothing
		/// failed; the engineer disabled them. Distinct from <see cref="NoLoadEffectDefined"/>
		/// because telling someone who deliberately disabled every state that their model has no
		/// load effect is false about their model and sends them looking for what is not missing.
		/// A refinement of <see cref="NotEvaluated"/>: still fixed by editing the model.
		/// </summary>
		AllSwitchedOff,

		/// <summary>
		/// The model read perfectly well and holds no load effect at all — a legitimate state of a
		/// model someone is still building. A refinement of <see cref="NotEvaluated"/>.
		/// </summary>
		NoLoadEffectDefined,

		/// <summary>
		/// The load effects themselves would not read (CON10: states referencing deleted members).
		/// The one case of the three where something genuinely failed. A refinement of
		/// <see cref="NotEvaluated"/>.
		/// </summary>
		Unreadable,
	}

	/// <summary>
	/// Grouping over <see cref="NotAssessedReason"/>, so that adding a refinement cannot silently
	/// re-classify a row. Every consumer that used to write `Reason == NotEvaluated` or
	/// `!= NotEvaluated` must ask here instead: the three refinements ARE not-evaluated cases, and
	/// a `!=` test would have counted each of them as a scope rejection — which is what the report's
	/// gap counters did the moment the enum grew.
	/// </summary>
	public static class NotAssessedReasonExtensions
	{
		/// <summary>
		/// The inputs could not be produced, in any of its forms. The reader's move is to fix the
		/// model and run again; nothing about the joint has been ruled out.
		/// </summary>
		public static bool IsBlockedInput(this NotAssessedReason r) =>
			r is NotAssessedReason.NotEvaluated
				or NotAssessedReason.AllSwitchedOff
				or NotAssessedReason.NoLoadEffectDefined
				or NotAssessedReason.Unreadable;

		/// <summary>§6.4 does not cover this joint and no edit to the model will change that.</summary>
		public static bool IsOutsideScope(this NotAssessedReason r) =>
			r == NotAssessedReason.OutsideScope;
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

		/// <summary>
		/// Title, e.g. "Axial Tension" — for a row that has no <see cref="JointDetail"/>. A §6.4 card
		/// leaves this empty and is titled by <see cref="TitleFor"/> when it is printed.
		/// </summary>
		public string Title { get; set; } = string.Empty;

		/// <summary>
		/// The title as it is printed under <paramref name="display"/>.
		///
		/// A §6.4 card's title carries three percentages, so it is composed here, at print time, from
		/// the row the card keeps — not stored. Stored, it was frozen in the precision of the RUN, and
		/// a settings change then produced one header row in two precisions.
		/// </summary>
		public string TitleFor(DisplaySettings display) =>
			JointDetail != null ? Services.Norsok64.Joint64ReportAdapter.TitleOf(JointDetail, display) : Title;

		/// <summary>
		/// What identifies the row, with the per-chapter detail left off — for an overview column.
		/// A §6.4 card answers from its row; any other row answers with its title.
		/// </summary>
		public string Subject =>
			JointDetail != null ? Services.Norsok64.Joint64ReportAdapter.SubjectOf(JointDetail) : Title;

		/// <summary>
		/// The check expression, e.g. "N_Sd ≤ N_t,Rd" — or, on a note or a rejection row, the
		/// condition in words. A row whose condition came from a topology gate carries it as
		/// <see cref="Gate"/> instead and leaves this empty.
		/// </summary>
		public string CheckExpression { get; set; } = string.Empty;

		/// <summary>
		/// The topology gate this row reports, as data — the measured length, angle or ratio in SI,
		/// so the sentence can be written in the reader's units when the row is shown.
		/// </summary>
		public Services.Norsok64.GateMessage? Gate { get; set; }

		/// <summary>The condition as printed under <paramref name="display"/>.</summary>
		public string CheckExpressionFor(DisplaySettings? display) =>
			Gate?.Render(display) ?? CheckExpression;

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
		///
		/// For a row WITHOUT <see cref="JointDetail"/>. A §6.4 card composes it at print time in
		/// <see cref="RangeQualifierFor"/>, because the value it names carries the reader's precision.
		/// </summary>
		public string? RangeQualifier { get; set; }

		/// <summary>The qualifier as printed under <paramref name="display"/>; null when there is none.</summary>
		public string? RangeQualifierFor(DisplaySettings? display) =>
			JointDetail is { Engine: { } engine } row
				? Services.Norsok64.Joint64ReportAdapter.RangeQualifierOf(row.Name, engine, display)
				: RangeQualifier;

		/// <summary>
		/// True when the check ran and produced a usable result, but <see cref="RangeQualifierFor"/>
		/// qualifies it. Neither a pass nor a failure on its own — it modifies a pass. Whether a
		/// qualifier exists does not depend on the display setting, only its wording does.
		/// </summary>
		public bool IsQualified => !IsNote && !NotAssessed && !string.IsNullOrEmpty(RangeQualifierFor(null));

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
			sb.AppendLine($"│ NORSOK N-004 §{Section} — {TitleFor(new DisplaySettings())}   (Eq. {Equation})");
			sb.AppendLine($"├─────────────────────────────────────────────────────────");

			// ── Check condition ──
			sb.AppendLine($"│");
			sb.AppendLine($"│  Check:  {CheckExpressionFor(null)}");

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

		/// <summary>
		/// The value already carries its unit — <see cref="Value"/> is converted and
		/// <see cref="Unit"/> labelled by whoever built the variable. This only decides how many
		/// digits to show and appends the label.
		///
		/// It used to switch on the unit STRING, with an arm each for "MPa", "kN", "kNm" and "mm".
		/// Once the builders started emitting the reader's units, "kip" and "ksi" matched no arm
		/// and fell to the `G6` default — six significant figures, `D = 5.55118 in`. The "kNm" arm
		/// had already been dead for longer: the label produced is "kN·m".
		/// </summary>
		public string FormattedValue
		{
			get
			{
				var c = System.Globalization.CultureInfo.InvariantCulture;
				return Unit == "-"
					? Value.ToString("F4", c)
					: $"{Value.ToString("F" + DecimalsFor(Unit), c)} {Unit}".Trim();
			}
		}

		/// <summary>Digits by KIND of quantity, inferred from the label rather than matched to it.</summary>
		private static int DecimalsFor(string unit) =>
			unit.EndsWith("²") || unit.EndsWith("³") || unit.EndsWith("⁴") ? 0
			: unit is "kN·m" or "N·m" or "MN·m" or "kip·ft" or "kip·in" ? 2
			: 1;
	}
}
