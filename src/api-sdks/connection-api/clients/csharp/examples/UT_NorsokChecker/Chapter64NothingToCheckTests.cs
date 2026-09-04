using IdeaStatiCa.Api.Connection.Model;
using NorsokChecker.Models;
using NorsokChecker.Services.Chapters;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// §6.4 has NOTHING TO CHECK, in each of the three ways that can happen — and each says
	/// something different about the model, so each must reach the reader as itself.
	///
	/// Until this file existed, Chapter64.EvaluateAsync had no behavioural test at all: the three
	/// branches were guarded only by a regex over its source text, and by tests that hand-wrote the
	/// sentences it prints. That left a mutation nobody would catch — swap the "could not be read"
	/// and "no load effect defined" literals and every test stayed green while the report told an
	/// engineer who had switched every state off that their model was empty.
	///
	/// What each test asserts is the PAIRING — reason and sentence and title together. Asserting the
	/// sentence alone is what the old tests did, and the enum change made that assertion pass for
	/// the wrong reason; asserting the reason alone would let the printed prose drift away from it.
	///
	/// HOW it runs without a service, and why the two obvious ways do not work. A non-empty
	/// SectionMap sends the chapter to `Client.Member.GetMembersAsync`, which on a null client
	/// throws NullReferenceException — and the chapter CATCHES that as a blocked input. An empty
	/// SectionMap blocks it even earlier. Both leave `blocked` set, and every branch below is
	/// guarded by `blocked == null`, so NEITHER reaches the load effects: the first version of this
	/// file took the second route and its tests measured the section branch while claiming to
	/// measure this one ("the joint's members could not be read: Object reference not set").
	///
	/// So the chapter got a seam, `Chapter64.MembersSource`, which is null in the app. That is the
	/// finding behind this file: these branches were not merely untested, they were untestable, and
	/// that is why swapping two of their sentences was invisible.
	/// </summary>
	[TestFixture]
	public class Chapter64NothingToCheckTests
	{
		/// <summary>
		/// A chord and one brace — enough geometry that §6.4 applies, so the chapter proceeds to the
		/// load effects instead of stopping at "no brace". Only IsContinuous is read on this path.
		/// </summary>
		private static List<JointMemberData> ChordAndBrace() => new()
		{
			new JointMemberData { Id = 1, Name = "M1", IsContinuous = true },
			new JointMemberData { Id = 2, Name = "M2", IsContinuous = false },
		};

		/// <summary>
		/// The chapter, run with no service behind it: the members arrive through the seam, so
		/// `Client` is never dereferenced and may be null.
		/// </summary>
		private static async Task<NorsokFormulaResult> RunAsync(
			List<ConLoadEffect>? loadEffects, int loadEffectsInFile,
			List<JointMemberData>? members = null)
		{
			var chapter = new Chapter64
			{
				MembersSource = (_, _) => Task.FromResult(members ?? ChordAndBrace()),
			};

			var outcome = await chapter.EvaluateAsync(new ChapterContext
			{
				Client = null!,
				ProjectId = Guid.Empty,
				ConnectionId = 1,
				ConnectionName = "CON1",
				LoadEffects = loadEffects,
				LoadEffectsInFile = loadEffectsInFile,
				SectionMap = new Dictionary<int, JointSectionInfo> { [1] = new() },
				Log = _ => { },
			}, CancellationToken.None);

			Assert.That(outcome.Rows, Has.Exactly(1).Items, "one row states the whole outcome");
			return outcome.Rows.First();
		}

		/// <summary>
		/// Every state switched off. The model read perfectly well — 15 rows came back, all
		/// inactive, and the app filters to active-only before a chapter sees anything, so the
		/// chapter is handed an EMPTY list plus the pre-filter count of 15.
		///
		/// The count must appear in the sentence: "all 15 load effect(s) … are switched off" is
		/// actionable, "no load effect" is a false statement about their model.
		/// </summary>
		[Test]
		public async Task EveryStateSwitchedOffIsItsOwnCaseAndNamesTheCount()
		{
			var row = await RunAsync(new List<ConLoadEffect>(), loadEffectsInFile: 15);

			Assert.Multiple(() =>
			{
				Assert.That(row.Reason, Is.EqualTo(NotAssessedReason.AllSwitchedOff));
				Assert.That(row.Reason.IsBlockedInput(), Is.True,
					"editing the model is the fix, so it must not read as a scope rejection");
				Assert.That(row.Reason.IsOutsideScope(), Is.False);
				Assert.That(row.Title, Is.EqualTo("All load effects switched off"));
				Assert.That(row.CheckExpression, Does.Contain("15"),
					"the pre-filter count is the actionable part");
				Assert.That(row.CheckExpression, Does.Contain("switched off"));
				Assert.That(row.CheckExpression, Does.Not.Contain("could not be read"),
					"nothing failed to read — saying so sends the reader hunting for a fault");
				Assert.That(row.NotAssessed, Is.True);
			});
		}

		/// <summary>
		/// No state defined at all: 200 with an empty collection, and nothing in the file either.
		/// A legitimate state of a model someone is still building.
		/// </summary>
		[Test]
		public async Task NoStateDefinedSaysThatAndNotThatSomethingFailed()
		{
			var row = await RunAsync(new List<ConLoadEffect>(), loadEffectsInFile: 0);

			Assert.Multiple(() =>
			{
				Assert.That(row.Reason, Is.EqualTo(NotAssessedReason.NoLoadEffectDefined));
				Assert.That(row.Reason.IsBlockedInput(), Is.True);
				Assert.That(row.Title, Is.EqualTo("No load effect defined"));
				Assert.That(row.CheckExpression, Does.Contain("no load effect defined"));
				Assert.That(row.CheckExpression, Does.Not.Contain("switched off"),
					"nobody switched anything off — there was nothing to switch");
				Assert.That(row.CheckExpression, Does.Not.Contain("could not be read"));
			});
		}

		/// <summary>
		/// The load effects would not read — null, not empty. The one case of the three where
		/// something genuinely failed (CON10 of the shipped project: its states reference members
		/// that were deleted, and the service answers 404).
		/// </summary>
		[Test]
		public async Task UnreadableStatesAreTheOnlyCaseThatReportsAFailureToRead()
		{
			var row = await RunAsync(loadEffects: null, loadEffectsInFile: 0);

			Assert.Multiple(() =>
			{
				Assert.That(row.Reason, Is.EqualTo(NotAssessedReason.Unreadable));
				Assert.That(row.Reason.IsBlockedInput(), Is.True);
				Assert.That(row.Title, Is.EqualTo("Could not be evaluated"));
				Assert.That(row.CheckExpression, Does.Contain("could not be read"));
				Assert.That(row.CheckExpression, Does.Not.Contain("switched off"));
			});
		}

		/// <summary>
		/// The three cases are DISTINCT in all three of the things the reader sees — reason, title
		/// and sentence.
		///
		/// Measured against the swap-the-literals mutation (exchange the "could not be read" and
		/// "no load effect defined" strings in Chapter64): the two tests above that name those
		/// sentences go red, and THIS one stays green — a swap is a permutation, so three distinct
		/// sentences remain three distinct sentences. Distinctness is therefore worth asserting for
		/// a copy-paste, which is the likelier slip, but it does not catch a swap. The pairing
		/// assertions above are what catch that, and that is why each test names its own sentence
		/// rather than leaving it to this one.
		/// </summary>
		[Test]
		public async Task TheThreeCasesShareNoReasonNoTitleAndNoSentence()
		{
			var rows = new[]
			{
				await RunAsync(new List<ConLoadEffect>(), 15),
				await RunAsync(new List<ConLoadEffect>(), 0),
				await RunAsync(null, 0),
			};

			Assert.Multiple(() =>
			{
				Assert.That(rows.Select(r => r.Reason).Distinct().Count(), Is.EqualTo(3),
					"three reasons");
				Assert.That(rows.Select(r => r.Title).Distinct().Count(), Is.EqualTo(3),
					"three titles");
				Assert.That(rows.Select(r => r.CheckExpression).Distinct().Count(), Is.EqualTo(3),
					"three sentences — a swap or a copy-paste collapses this");
			});
		}

		/// <summary>
		/// A joint with NO BRACE is outside scope, and it outranks the missing states: it is decided
		/// from the members alone and no edit to the load effects changes it.
		///
		/// This is the ordering that CON10 of the shipped project got wrong — its inherited states
		/// reference deleted braces, so reading them first reported the 404 as the reason while the
		/// real reason was permanent. Kept here because this branch sits immediately before the
		/// three above and an edit to either can capture the other: the same inputs that would give
		/// "switched off" must give the scope answer once the brace is gone.
		/// </summary>
		[Test]
		public async Task NoBraceOutranksTheMissingStatesAndIsOutsideScope()
		{
			var chordOnly = new List<JointMemberData>
			{
				new() { Id = 1, Name = "M1", IsContinuous = true },
			};

			var row = await RunAsync(new List<ConLoadEffect>(), 15, chordOnly);

			Assert.Multiple(() =>
			{
				Assert.That(row.Reason, Is.EqualTo(NotAssessedReason.OutsideScope),
					"no brace is permanent — the reader's move is another method, not another state");
				Assert.That(row.Reason.IsBlockedInput(), Is.False);
				Assert.That(row.CheckExpression ?? "", Does.Contain("brace"));
				Assert.That(row.CheckExpression ?? "", Does.Not.Contain("switched off"),
					"the states are beside the point when there is no brace to check");
			});
		}
	}
}
