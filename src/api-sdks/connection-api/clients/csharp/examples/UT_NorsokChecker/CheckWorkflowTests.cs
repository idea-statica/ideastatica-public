using NorsokChecker.Models;
using NorsokChecker.Services;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The connection verdict — the headline result of the whole app.
	///
	/// It had no test until it was extracted, because it lived inside RunCheck_Click and could only
	/// be reached by building a window and clicking. These pin the rules that are NOT obvious from
	/// reading the code, and each one guards a way of being wrong that produces a plausible answer:
	/// awarding a pass for the absence of a check, hiding a partial assessment behind a green
	/// verdict, or letting a warning count as a result.
	/// </summary>
	[TestFixture]
	public class CheckWorkflowTests
	{
		private static NorsokFormulaResult Pass(double util) =>
			new() { Section = "6.4", Title = "brace", Utilization = util, Passed = true };

		private static NorsokFormulaResult Fail(double util) =>
			new() { Section = "6.4", Title = "brace", Utilization = util, Passed = false };

		/// <summary>
		/// An unassessed row. The REASON is required, not defaulted.
		///
		/// It used to be `NotAssessed(string why)` with no Reason, which left every row at
		/// `NotAssessedReason.None` — so every test in this file fell through the blocked / allOff /
		/// noLoad branches to the bare count branch. Four tests asserted the scope sentence while
		/// never once producing an OutsideScope row, and the three reason-driven branches
		/// CheckWorkflow was rewritten to add had no test at all.
		///
		/// The same trap the production comment at CheckWorkflow.cs:78-82 records, approached from
		/// the other side: there the code read the sentence it had just written, here the fixture
		/// wrote a sentence and no reason.
		/// </summary>
		private static NorsokFormulaResult NotAssessed(NotAssessedReason reason, string why) =>
			new()
			{
				Section = "6.4", Title = "brace", NotAssessed = true,
				Reason = reason, CheckExpression = why,
			};

		private static NorsokFormulaResult Note(string what) =>
			new() { Section = "6.4", Title = "note", IsNote = true, CheckExpression = what };

		/// <summary>A check that RAN and PASSED, on geometry outside a §6.4.3.1 validity range.</summary>
		private static NorsokFormulaResult Qualified(double util, string qualifier) =>
			new()
			{
				Section = "6.4", Title = "brace", Utilization = util, Passed = true,
				RangeQualifier = qualifier,
			};

		/// <summary>A check that RAN and PASSED, on a joint missing a "should" of the standard.</summary>
		private static NorsokFormulaResult WithRecommendation(double util, string rec) =>
			new()
			{
				Section = "6.4", Title = "brace", Utilization = util, Passed = true,
				Recommendation = rec,
			};

		[Test]
		public void AllPassing()
		{
			var v = CheckWorkflow.Roll(new[] { Pass(0.42), Pass(0.88), Pass(0.10) });

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("PASS"));
				Assert.That(v.MaxUtilisation, Is.EqualTo(0.88).Within(1e-9), "the governing one");
				Assert.That(v.Status, Is.EqualTo("Norsok OK"));
			});
		}

		/// <summary>One failure decides the connection, whatever else passed.</summary>
		[Test]
		public void OneFailureFailsTheConnection()
		{
			var v = CheckWorkflow.Roll(new[] { Pass(0.42), Fail(1.30), Pass(0.10) });

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("FAIL"));
				Assert.That(v.MaxUtilisation, Is.EqualTo(1.30).Within(1e-9));
			});
		}

		/// <summary>
		/// THE defect this logic exists to prevent: a connection where nothing could be checked used
		/// to report "Norsok OK / PASS / 0.0 %" — a pass awarded for the absence of a check. It must
		/// be N/A, and the status must say why rather than showing a utilisation of zero.
		/// </summary>
		[Test]
		public void NothingAssessedIsNotAPass()
		{
			var v = CheckWorkflow.Roll(new[]
			{
				NotAssessed(NotAssessedReason.OutsideScope, "overlap joint"),
				NotAssessed(NotAssessedReason.OutsideScope, "θ = 0°"),
			});

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("N/A"), "an unchecked connection is not a passing one");
				Assert.That(v.MaxUtilisation, Is.Zero);
				Assert.That(v.Status, Is.EqualTo("Outside §6.4 scope (2 conditions)"),
					"and the count tells the reader how much was not met");
			});
		}

		/// <summary>Singular when exactly one condition failed — the count is information, not decoration.</summary>
		[Test]
		public void OneUnmetConditionIsNotCounted()
		{
			var v = CheckWorkflow.Roll(new[]
				{ NotAssessed(NotAssessedReason.OutsideScope, "overlap joint") });

			Assert.That(v.Status, Is.EqualTo("Outside §6.4 scope"));
		}

		/// <summary>
		/// A connection that is PARTLY assessed is neither PASS nor FAIL. Reporting it as PASS would
		/// state that the whole connection was checked; reporting FAIL would state that something
		/// failed. Both are false, and the first is the dangerous one.
		/// </summary>
		[Test]
		public void PassingChecksPlusAGapArePartial()
		{
			var v = CheckWorkflow.Roll(new[]
				{ Pass(0.42), NotAssessed(NotAssessedReason.OutsideScope, "no transverse force") });

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("PARTIAL"));
				Assert.That(v.MaxUtilisation, Is.EqualTo(0.42).Within(1e-9),
					"the utilisation of what WAS assessed");
				Assert.That(v.Status, Is.EqualTo("Partly assessed"));
			});
		}

		/// <summary>A failure outranks a gap: FAIL is the stronger statement and both are true.</summary>
		[Test]
		public void AFailureBeatsAGap()
		{
			var v = CheckWorkflow.Roll(new[]
				{ Fail(1.10), NotAssessed(NotAssessedReason.OutsideScope, "θ = 0°") });

			Assert.That(v.Pass, Is.EqualTo("FAIL"));
		}

		/// <summary>
		/// A NOTE qualifies a check that ran; it is neither a result nor a gap. Counting one as a gap
		/// would turn every warned-about connection PARTIAL, and counting it as a result would let a
		/// warning with no utilisation report a pass.
		/// </summary>
		[Test]
		public void ANoteIsNeitherAResultNorAGap()
		{
			var withNote = CheckWorkflow.Roll(new[] { Pass(0.42), Note("plane fitted from 3 braces") });
			var withoutNote = CheckWorkflow.Roll(new[] { Pass(0.42) });

			Assert.Multiple(() =>
			{
				Assert.That(withNote.Pass, Is.EqualTo("PASS"), "a note must not make a connection partial");
				Assert.That(withNote, Is.EqualTo(withoutNote), "and must not change the verdict at all");
			});
		}

		/// <summary>
		/// Notes alone are still "nothing assessed" — but NOT "outside scope", because no condition
		/// was reported unmet. The two read differently to an engineer and the status must not claim
		/// a rejection that did not happen.
		/// </summary>
		[Test]
		public void NotesAloneAreNotAssessedButNotRejected()
		{
			var v = CheckWorkflow.Roll(new[] { Note("something happened") });

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("N/A"));
				Assert.That(v.Status, Is.EqualTo("Not assessed"),
					"no condition was unmet, so this is not an out-of-scope rejection");
			});
		}

		/// <summary>
		/// WHICH kind of "nothing was checked" — the three branches that had no test at all.
		///
		/// `CheckWorkflow.cs:83-92` distinguishes four cases, and until now this file exercised one
		/// of them: its fixture never set a Reason, so every row arrived as `None` and fell through
		/// to the bare "Outside §6.4 scope" count. The branches that read the enum were unreachable
		/// from any test.
		///
		/// The distinction is not cosmetic. Telling an engineer who deliberately switched every
		/// state off that their model has no load effect sends them looking for something that is
		/// not missing; telling someone whose model would not read that §6.4 does not cover their
		/// joint is a false statement about their geometry.
		/// </summary>
		[TestCase(NotAssessedReason.AllSwitchedOff,
			"Not assessed — every load effect switched off")]
		[TestCase(NotAssessedReason.NoLoadEffectDefined,
			"Not assessed — no load effect defined")]
		[TestCase(NotAssessedReason.Unreadable,
			"Not evaluated — the model could not be read")]
		[TestCase(NotAssessedReason.NotEvaluated,
			"Not evaluated — the model could not be read")]
		public void ABlockedInputSaysWhichKindOfNothing(NotAssessedReason reason, string expected)
		{
			var v = CheckWorkflow.Roll(new[] { NotAssessed(reason, "n/a") });

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("N/A"));
				Assert.That(v.Status, Is.EqualTo(expected));
				Assert.That(v.Status, Does.Not.Contain("Outside"),
					"a model that could not be read has NOT been ruled outside the chapter's scope — "
					+ "that verdict would send the reader to a different design method for a joint "
					+ "§6.4 may well cover");
			});
		}

		/// <summary>
		/// A blocked input outranks a scope gate when both are present.
		///
		/// The scope verdict was reached on inputs we already know are incomplete, so it is not
		/// trustworthy — `CheckWorkflow.cs:90-92` says so and nothing tested it. Ordering the two
		/// rows the other way must not change the answer, or the rule is "whichever came first".
		/// </summary>
		[Test]
		public void ABlockedInputOutranksAScopeGate()
		{
			var scopeFirst = CheckWorkflow.Roll(new[]
			{
				NotAssessed(NotAssessedReason.OutsideScope, "overlap joint"),
				NotAssessed(NotAssessedReason.Unreadable, "load effects would not read"),
			});
			var blockedFirst = CheckWorkflow.Roll(new[]
			{
				NotAssessed(NotAssessedReason.Unreadable, "load effects would not read"),
				NotAssessed(NotAssessedReason.OutsideScope, "overlap joint"),
			});

			Assert.Multiple(() =>
			{
				Assert.That(scopeFirst.Status, Is.EqualTo("Not evaluated — the model could not be read"));
				Assert.That(blockedFirst.Status, Is.EqualTo(scopeFirst.Status),
					"and the order of the rows does not decide it");
			});
		}

		/// <summary>
		/// An unmet RECOMMENDATION reaches the overview and changes no verdict.
		///
		/// Both halves are asserted, and the pass is asserted FIRST. A test that only checked for
		/// the note would pass if the verdict had flipped to FAIL or QUALIFIED — which is the thing
		/// that must not happen: §6.4.1's gap provision says "should", and §3.1 defines that as a
		/// recommendation rather than a requirement for conformity. A joint that misses it conforms.
		///
		/// The reviewed report had the opposite failure: the provision was evaluated in the card
		/// renderer and discarded, so seven connections read "Norsok OK" in the overview while their
		/// own detail pages recorded it unmet — twenty times, once at g = 1.5 mm against 50.
		/// </summary>
		[Test]
		public void AnUnmetRecommendationIsReportedAndChangesNoVerdict()
		{
			const string rec = "M3: g = 1.5 mm, §6.4.1 recommends 50 mm < g < D (141 mm)";
			var v = CheckWorkflow.Roll(new[] { Pass(0.42), WithRecommendation(0.55, rec) });
			var without = CheckWorkflow.Roll(new[] { Pass(0.42), Pass(0.55) });

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("PASS"),
					"a recommendation is not a condition of conformity");
				Assert.That(v.Status, Is.EqualTo(without.Status),
					"and the status is the one it would have had anyway");
				Assert.That(v.MaxUtilisation, Is.EqualTo(without.MaxUtilisation).Within(1e-9));

				Assert.That(v.Recommendations, Is.EqualTo(rec),
					"but it IS carried, so the overview row can show it");
				Assert.That(without.Recommendations, Is.Null,
					"and a joint that meets it carries nothing");
			});
		}

		/// <summary>
		/// A recommendation rides along with every verdict, not only with a pass.
		///
		/// It is attached around the decision rather than inside it, so that a "should" cannot alter
		/// the outcome. The consequence worth pinning: a FAILING joint that also misses the gap
		/// provision reports both.
		/// </summary>
		[Test]
		public void ARecommendationSurvivesAFailingVerdict()
		{
			const string rec = "M3: g = 1.5 mm, §6.4.1 recommends 50 mm < g < D (141 mm)";
			var v = CheckWorkflow.Roll(new[]
			{
				Fail(1.30),
				WithRecommendation(0.55, rec),
			});

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("FAIL"), "the failure decides the verdict");
				Assert.That(v.Recommendations, Is.EqualTo(rec), "and the recommendation still reaches the row");
			});
		}

		/// <summary>An empty result set behaves like no assessment, not like a pass.</summary>
		[Test]
		public void NoResultsAtAllIsNotAPass()
		{
			var v = CheckWorkflow.Roll(Array.Empty<NorsokFormulaResult>());

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("N/A"));
				Assert.That(v.Status, Is.EqualTo("Not assessed"));
			});
		}

		/// <summary>
		/// A pass on geometry outside the §6.4.3.1 ranges must NOT report as a plain pass.
		///
		/// This is the defect the round-2 review called the most serious remaining one: the detail
		/// card said "outside validity range (6.4.3.1)" while the overview row said "PASS / Norsok
		/// OK", and the caveat was sixty pages from the row an engineer scans. The qualifier reaches
		/// the roll-up only because it travels as a FIELD; while it lived in the card's title text,
		/// nothing here could see it.
		/// </summary>
		[Test]
		public void OutsideValidityRangeQualifiesThePass()
		{
			var v = CheckWorkflow.Roll(new[]
			{
				Pass(0.42),
				Qualified(0.737, "M1: θ = 20.0°, outside 30–90°"),
			});

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("QUALIFIED"),
					"neither a plain PASS nor a FAIL — every check passed, on extrapolated formulas");
				Assert.That(v.MaxUtilisation, Is.EqualTo(0.737).Within(1e-9),
					"the utilisation is real and still governs");
				Assert.That(v.Status, Does.Contain("θ = 20.0°"),
					"the overview names the parameter and its value, not just 'outside range'");
				Assert.That(v.Status, Does.Contain("M1"), "and which brace it was");
			});
		}

		/// <summary>
		/// A failure outranks the qualifier. A qualified FAIL is still a FAIL, and reporting
		/// QUALIFIED for it would soften a connection that does not comply.
		/// </summary>
		[Test]
		public void AFailureOutranksTheQualifier()
		{
			var v = CheckWorkflow.Roll(new[]
			{
				Qualified(0.50, "M1: β = 0.180, outside 0.2–1.0"),
				Fail(1.20),
			});

			Assert.That(v.Pass, Is.EqualTo("FAIL"));
		}

		/// <summary>
		/// An unassessed brace outranks the qualifier too: "part of this joint was never checked" is
		/// a bigger claim than "what was checked rests on an extrapolation".
		/// </summary>
		[Test]
		public void AnUnassessedBraceOutranksTheQualifier()
		{
			var v = CheckWorkflow.Roll(new[]
			{
				Qualified(0.50, "M1: θ = 20.0°, outside 30–90°"),
				NotAssessed(NotAssessedReason.OutsideScope, "no transverse force"),
			});

			Assert.That(v.Pass, Is.EqualTo("PARTIAL"));
		}

		/// <summary>
		/// Two braces outside their ranges are both named. Reporting one would understate the caveat
		/// exactly where the reader is scanning for it — and a count ("2 conditions") would send them
		/// hunting instead of answering.
		/// </summary>
		[Test]
		public void EveryQualifiedBraceIsNamed()
		{
			var v = CheckWorkflow.Roll(new[]
			{
				Qualified(0.50, "M1: θ = 20.0°, outside 30–90°"),
				Qualified(0.61, "M3: β = 0.180, outside 0.2–1.0"),
			});

			Assert.Multiple(() =>
			{
				Assert.That(v.Pass, Is.EqualTo("QUALIFIED"));
				Assert.That(v.Status, Does.Contain("M1"));
				Assert.That(v.Status, Does.Contain("M3"));
			});
		}

		/// <summary>
		/// A note is not a qualifier. Notes already existed and take no part in the roll-up; if the
		/// new branch keyed on them instead of on the range field, every joint carrying a plane-fit
		/// note would suddenly read QUALIFIED.
		/// </summary>
		[Test]
		public void ANoteDoesNotQualifyThePass()
		{
			var v = CheckWorkflow.Roll(new[] { Pass(0.42), Note("plane fitted from 3 braces") });

			Assert.That(v.Pass, Is.EqualTo("PASS"));
			Assert.That(v.Status, Is.EqualTo("Norsok OK"));
		}
	}
}
