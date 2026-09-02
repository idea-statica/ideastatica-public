using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using NorsokChecker.Models;

namespace NorsokChecker.Services.Chapters
{
	/// <summary>
	/// One chapter of the code — a set of checks the user can switch on, evaluated per connection.
	///
	/// Designed against TWO chapters rather than one, deliberately: §6.4 (live) and §6.3 (written,
	/// mothballed, and described in CHAPTER_63_REVISIT.md). They differ in every way that matters,
	/// and the interface has to survive both:
	///
	///   | | §6.4 | §6.3 |
	///   |---|---|---|
	///   | subject | the JOINT as a whole | each MEMBER |
	///   | rows | one per brace | one per member |
	///   | inputs | all derivable from the model | needs an unbraced length the model has no field for |
	///   | completeness | assess the joint, or reject it | 9 of its 11 checks are length-free; the
	///     compression buckling check cannot run at all |
	///
	/// That last row is why <see cref="ChapterOutcome.NotPerformed"/> exists. A chapter that can only
	/// say "passed / failed / rejected" cannot express "I checked nine things and deliberately did
	/// not check the tenth" — and CHAPTER_63_REVISIT.md:100-115 is explicit that omitting the
	/// buckling check must be visible where the result appears, not only in documentation, because
	/// the gap reaches 28 % of capacity with nothing in the number to show it.
	/// </summary>
	internal interface IChapter
	{
		/// <summary>
		/// Stable key, e.g. "6.4". Used to route results and to identify the chapter in settings —
		/// never shown to the user, so it must not change once results exist that carry it.
		/// </summary>
		string Key { get; }

		/// <summary>The toggle's label, e.g. "§6.4 Joints".</summary>
		string DisplayName { get; }

		/// <summary>
		/// The heading this chapter's rows appear under in the report. Replaces the hardcoded group
		/// list in NorsokHtmlReportGenerator, where a chapter that was not listed fell silently into
		/// "Other Checks".
		/// </summary>
		string ReportGroup { get; }

		/// <summary>
		/// True when the chapter has a tab of its own beside Results — as §6.4 does, with its
		/// per-load-effect sheet, the classification table and the derivation window.
		///
		/// Not every chapter earns one: §6.3 is a list of member checks, and rows in Results plus the
		/// report say everything there is to say about it. The flag lets the registry describe that
		/// difference instead of the app pretending every chapter is shaped like §6.4.
		/// </summary>
		bool HasOwnTab { get; }

		/// <summary>Evaluate this chapter for one connection.</summary>
		Task<ChapterOutcome> EvaluateAsync(ChapterContext context, CancellationToken ct);
	}

	/// <summary>
	/// What a chapter is given for one connection.
	///
	/// One type rather than a long parameter list, so a chapter that needs something new adds a
	/// property here instead of changing every implementation's signature. Everything on it is
	/// already fetched by the run — a chapter must not go looking for its own data behind the run's
	/// back, or two chapters in one pass would read the model twice.
	/// </summary>
	internal sealed class ChapterContext
	{
		public required IConnectionApiClient Client { get; init; }
		public required Guid ProjectId { get; init; }
		public required int ConnectionId { get; init; }
		public required string ConnectionName { get; init; }

		/// <summary>
		/// The load effects to assess — already filtered by the "active only" toggle, so a chapter
		/// must not filter again. Null when they could not be read, which is a reason not to run
		/// rather than an empty set: see <see cref="ChapterOutcome.NotPerformed"/>.
		/// </summary>
		public List<ConLoadEffect>? LoadEffects { get; init; }

		/// <summary>
		/// How many load effects the connection has IN THE FILE, before the active-only filter.
		///
		/// Needed because DEACTIVATION and ABSENCE are different facts about the model and reach a
		/// chapter identically — both leave <see cref="LoadEffects"/> empty. Measured on the shipped
		/// project: CON8 and CON15 carry 15 states of which 7 are active, so the difference is real
		/// and not hypothetical, while an unreadable connection (CON10) answers HTTP 404 and is a
		/// third case again.
		///
		/// Without this a report can only say "no load effect", which would tell an engineer who
		/// switched every state off that their model is empty. Zero means genuinely none defined.
		/// </summary>
		public int LoadEffectsInFile { get; init; }

		/// <summary>Cross-section id → D/T/fy for the project, or empty when it could not be read.</summary>
		public required IReadOnlyDictionary<int, Norsok64.JointSectionInfo> SectionMap { get; init; }

		/// <summary>Where a chapter writes progress. The app's log; a chapter must not touch the UI.</summary>
		public required Action<string> Log { get; init; }
	}

	/// <summary>
	/// What a chapter produces for one connection: the checks it made, and the checks it did not.
	///
	/// The second half is the part that is easy to leave out and expensive to leave out. A chapter
	/// that silently omits a check produces a result that looks complete and is not — the exact
	/// failure this app already fixed once, when a connection nothing could be checked on reported
	/// "PASS, 0.0 %".
	/// </summary>
	internal sealed class ChapterOutcome
	{
		/// <summary>The checks that were made, as report rows.</summary>
		public required IReadOnlyList<NorsokFormulaResult> Rows { get; init; }

		/// <summary>
		/// Checks this chapter deliberately did NOT make, each with its reason — a §6.4 joint outside
		/// the scope of the chapter, or §6.3's buckling check on a member with no length.
		///
		/// These reach the results and the report as rows in their own right, so the reader sees the
		/// gap rather than inferring it from a number that is missing.
		/// </summary>
		public IReadOnlyList<NotPerformed> NotPerformed { get; init; } = Array.Empty<NotPerformed>();

		/// <summary>Nothing was checked at all — every row is a gap.</summary>
		public bool NothingAssessed => Rows.Count == 0;

		internal static ChapterOutcome Nothing(string reason) => new()
		{
			Rows = Array.Empty<NorsokFormulaResult>(),
			NotPerformed = new[] { new NotPerformed("the chapter could not be evaluated", reason) },
		};
	}

	/// <summary>
	/// A check that was not made, and why. <paramref name="What"/> names the check in the norm's own
	/// terms ("§6.3.3 axial compression, eq 6.2"); <paramref name="Why"/> says what was missing.
	/// </summary>
	internal sealed record NotPerformed(string What, string Why);
}
