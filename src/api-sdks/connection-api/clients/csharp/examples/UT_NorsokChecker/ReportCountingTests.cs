using System.Globalization;
using NorsokChecker.Models;
using NorsokChecker.Services;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The summary's counters, the two kinds of "not assessed", and the number formatting.
	///
	/// All three were changed together because all three were the same defect in different guises:
	/// the report stated something that was not true of the model. "Total Checks: 55" added 30 real
	/// results to 25 unmet scope conditions; the overview said "Outside §6.4 scope" about a
	/// connection whose own detail card said "could not be evaluated"; and the headline percentage
	/// used the machine's decimal separator while the derivations below it used a point.
	///
	/// Nothing guarded any of it — the whole rework landed with 362 tests green.
	/// </summary>
	[TestFixture]
	public class ReportCountingTests
	{
		/// <summary>A check that ran and passed.</summary>
		private static NorsokFormulaResult Passed(double util = 0.5) => new()
		{
			Section = "6.4.3.6", Equation = "6.57", Title = "Tubular Joint — M1",
			Utilization = util, Passed = true,
		};

		/// <summary>A check that ran and failed.</summary>
		private static NorsokFormulaResult Failed(double util = 1.3) => new()
		{
			Section = "6.4.3.6", Equation = "6.57", Title = "Tubular Joint — M2",
			Utilization = util, Passed = false,
		};

		/// <summary>One unmet scope condition — the chapter does not cover this joint.</summary>
		private static NorsokFormulaResult Gate(string why) => new()
		{
			Section = "6.4", Title = "Outside the scope of §6.4", CheckExpression = why,
			NotAssessed = true, Reason = NotAssessedReason.OutsideScope,
		};

		/// <summary>The inputs would not read — the chapter might well apply.</summary>
		private static NorsokFormulaResult Blocked(string why) => new()
		{
			Section = "6.4", Title = "Could not be evaluated", CheckExpression = why,
			NotAssessed = true, Reason = NotAssessedReason.NotEvaluated,
		};

		private static string Report(params (string Con, NorsokFormulaResult[] Rows)[] cons) =>
			NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
				cons.Select(c => (c.Con, c.Rows.ToList())).ToList());

		/// <summary>
		/// Checks and unmet conditions are counted in SEPARATE numbers.
		///
		/// The fixture is the shipped shape: two joints assessed, one rejected on six conditions.
		/// The old total would read 8 (2 checks + 6 conditions) and call all of it "Total Checks".
		/// </summary>
		[Test]
		public void ChecksAndUnmetConditionsAreCountedSeparately()
		{
			string html = Report(
				("CON1", new[] { Passed() }),
				("CON8", new[] { Passed() }),
				("CON5", new[]
				{
					Gate("M4-M6: feet overlap"), Gate("M1: 20° off plane"),
					Gate("M6: ecc. 10 mm"), Gate("no through chord"),
					Gate("M2 is not tubular"), Gate("chord ambiguous"),
				}));

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Not.Contain("Total Checks"),
					"the label that invited two units into one number");
				// Read each counter's OWN value, by its label. A bare Does.Contain(">2<") passes on
				// any '2' anywhere in a 600 kB document — measured: it stayed green with the counter
				// reverted to include the six conditions, which is the whole defect.
				Assert.That(CounterValue(html, "Checks performed"), Is.EqualTo("2"),
					"two checks were performed; the six conditions are not checks");
				Assert.That(CounterValue(html, "Outside &sect;6.4 scope"), Is.EqualTo("6"),
					"and the conditions are counted as conditions");
				Assert.That(CounterValue(html, "Connections assessed"), Is.EqualTo("2 / 3"),
					"plus the unit a reviewer counts in");
			});
		}

		/// <summary>
		/// The value of one summary counter, found by its label.
		///
		/// The counters are &lt;span class='stat-value'&gt;N&lt;/span&gt; followed by their label, so
		/// the label is what identifies which number is which. Matching a bare number instead finds
		/// any occurrence of it in the document and asserts nothing.
		/// </summary>
		private static string CounterValue(string html, string label)
		{
			var m = System.Text.RegularExpressions.Regex.Match(html,
				@"<span class='stat-value'>([^<]*)</span>\s*<span class='stat-label'>"
				+ System.Text.RegularExpressions.Regex.Escape(label) + "</span>");
			Assert.That(m.Success, Is.True, $"no counter labelled '{label}'");
			return m.Groups[1].Value.Trim();
		}

		/// <summary>
		/// "Outside scope" and "not evaluated" are counted apart, and both are named.
		///
		/// They are opposite instructions to the reader — use another method, versus fix the model
		/// and run again — and the report used to print the first about both.
		/// </summary>
		[Test]
		public void TheTwoKindsOfGapAreCountedApart()
		{
			string html = Report(
				("CON5", new[] { Gate("no through chord") }),
				("CON10", new[] { Blocked("the load effects of this connection could not be read") }));

			Assert.Multiple(() =>
			{
				Assert.That(CounterValue(html, "Outside &sect;6.4 scope"), Is.EqualTo("1"),
					"the scope gate, counted as one");
				Assert.That(CounterValue(html, "Not evaluated"), Is.EqualTo("1"),
					"and the unreadable input, counted apart from it");
			});
		}

		/// <summary>
		/// The APP tags a blocked chapter as NotEvaluated, not as a scope rejection.
		///
		/// The tests above build their own rows, so they say nothing about what production sets —
		/// measured: flipping Chapter64 to OutsideScope left every one of them green. The reason
		/// flag is only worth having if the code that emits the row sets it correctly.
		///
		/// On the SOURCE, because reaching Chapter64's blocked path needs a failing API call, and
		/// NorsokCheckRunner's rejection path needs a real topology. Ugly, and it is what fails when
		/// the wiring is undone.
		/// </summary>
		[Test]
		public void TheAppTagsEachGapWithTheRightReason()
		{
			string chapter = ReadAppSource("Services/Chapters/Chapter64.cs");
			string runner = ReadAppSource("Services/NorsokCheckRunner.cs");

			Assert.Multiple(() =>
			{
				// The chapter now emits BOTH, and that is the point: unreadable inputs are
				// NotEvaluated (fix the model and re-run), a joint with no brace is OutsideScope
				// (nothing to fix — §6.4 checks a brace against a chord). This assertion used to
				// forbid OutsideScope here, which was right only while the chapter had a single way
				// to fail, and became wrong the moment the brace-less case got its own answer.
				Assert.That(chapter, Does.Contain("Reason = NotAssessedReason.NotEvaluated"),
					"unreadable inputs are not a scope verdict");
				Assert.That(chapter, Does.Contain("Reason = NotAssessedReason.OutsideScope"),
					"and a joint with no brace is not an input problem");

				// The brace-less GATE, live. Both enum values stay in the file whatever the
				// condition guarding them does, so the strings above cannot see the gate being
				// switched off — measured: `if (false && ...)` left them green.
				Assert.That(chapter, Does.Match(
					@"if\s*\(\s*blocked == null && members is \{ Count: > 0 \}"),
					"the brace-less check runs, and runs before the load effects");

				// The topology rejection: a property of the geometry, so the chapter's scope IS the
				// reason. Both flags appear in the runner — the joint gate and the per-brace gap.
				Assert.That(runner, Does.Contain("Reason = NotAssessedReason.OutsideScope"),
					"a joint the topology rejected is genuinely outside §6.4");
			});
		}

		/// <summary>One of the app's own source files, with comments stripped.</summary>
		private static string ReadAppSource(string relative)
		{
			var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !System.IO.Directory.Exists(
				System.IO.Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Ignore("cannot locate the NorsokChecker source from the test output");

			string code = System.IO.File.ReadAllText(
				System.IO.Path.Combine(dir!.FullName, "NorsokChecker", relative));

			// The comments discuss both enum values, so a raw match finds the explanation.
			return System.Text.RegularExpressions.Regex.Replace(code, @"//[^\n]*", "");
		}

		/// <summary>
		/// A connection whose inputs would not read is NOT reported as outside the chapter's scope.
		///
		/// THE defect this split exists for. CON10's braces are deleted in the shipped test project,
		/// so its inherited load effects reference members that no longer exist and the service
		/// answers 404 — nothing about that says §6.4 fails to cover the joint. The overview row
		/// said it did, while the detail card two inches below said the opposite.
		/// </summary>
		[Test]
		public void AConnectionThatCouldNotBeReadIsNotCalledOutsideScope()
		{
			var verdict = CheckWorkflow.Roll(new List<NorsokFormulaResult>
			{
				Blocked("the load effects of this connection could not be read"),
			});

			Assert.Multiple(() =>
			{
				Assert.That(verdict.Pass, Is.EqualTo("N/A"), "still nothing was assessed");
				Assert.That(verdict.Status, Does.Not.Contain("Outside"),
					"but the chapter's scope is not what stopped it");
				Assert.That(verdict.Status, Does.Contain("Not evaluated"),
					"the model could not be read — that is the reader's next move");
			});
		}

		/// <summary>
		/// A genuine scope rejection still says so, with its condition count.
		///
		/// The other half: a split that reported everything as "not evaluated" would satisfy the
		/// test above while losing the distinction in the opposite direction.
		/// </summary>
		[Test]
		public void AGenuineScopeRejectionStillSaysOutsideScope()
		{
			var verdict = CheckWorkflow.Roll(new List<NorsokFormulaResult>
			{
				Gate("no through chord"), Gate("M2 is not tubular"),
			});

			Assert.Multiple(() =>
			{
				Assert.That(verdict.Pass, Is.EqualTo("N/A"));
				Assert.That(verdict.Status, Is.EqualTo("Outside §6.4 scope (2 conditions)"),
					"the chapter does not cover it, and how many conditions failed");
			});
		}

		/// <summary>
		/// A blocked input wins over a scope gate when a connection carries both.
		///
		/// Deliberate, and worth pinning: the scope verdict was reached on inputs we know are
		/// incomplete, so reporting "outside scope" would state a conclusion drawn from data that
		/// was never read.
		/// </summary>
		[Test]
		public void ABlockedInputOutranksAScopeGate()
		{
			var verdict = CheckWorkflow.Roll(new List<NorsokFormulaResult>
			{
				Gate("no through chord"),
				Blocked("no cross-section data was available"),
			});

			Assert.That(verdict.Status, Does.Contain("Not evaluated"),
				"a scope verdict reached on unread inputs is not trustworthy");
		}

		/// <summary>
		/// Failed checks are counted, not derived from a total that includes non-checks.
		///
		/// `failed = total - passed - notAssessed` was arithmetically right only while `total`
		/// counted rows; the moment "checks performed" stopped including gaps, the same expression
		/// would have started reporting the gaps as failures.
		/// </summary>
		[Test]
		public void AFailingCheckIsCountedAsFailedAndTheGapsAreNot()
		{
			string html = Report(
				("CON1", new[] { Passed(), Failed() }),
				("CON5", new[] { Gate("no through chord"), Gate("M2 is not tubular") }));

			Assert.Multiple(() =>
			{
				Assert.That(html, Does.Contain("NON-COMPLIANT"), "a failing check fails the report");
				// 2 checks performed, 1 passed, 1 failed — and NOT 4 / 1 / 3, which is what
				// `failed = total - passed - notAssessed` would give once `total` stopped counting
				// the gaps.
				Assert.That(CounterValue(html, "Checks performed"), Is.EqualTo("2"));
				Assert.That(CounterValue(html, "Passed"), Is.EqualTo("1"));
				Assert.That(CounterValue(html, "Failed"), Is.EqualTo("1"),
					"the two unmet conditions are not failures");
			});
		}

		/// <summary>
		/// Percentages use ONE decimal separator, whatever the machine's locale.
		///
		/// Measured on a printed report from a cs-CZ machine: the summary read "73,7%" with a comma
		/// while every derivation step below read "73.70" with a point — the steps go through an
		/// InvariantCulture helper and the headline figures did not. A document that renders
		/// differently per machine cannot be a deliverable.
		///
		/// The culture is switched on the thread here rather than trusted: on an en-US agent the
		/// defect is invisible, so a test that only read the output would have passed on the broken
		/// code. (That is exactly how this one escaped — the machine that found it was Czech.)
		/// </summary>
		[Test]
		public void PercentagesDoNotFollowTheMachinesLocale()
		{
			var before = System.Threading.Thread.CurrentThread.CurrentCulture;
			try
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
				string html = Report(("CON1", new[] { Passed(0.737) }));

				Assert.Multiple(() =>
				{
					Assert.That(html, Does.Contain("73.7%"), "a point, as the derivations use");
					Assert.That(html, Does.Not.Contain("73,7"),
						"not the comma this thread's culture would give");
				});
			}
			finally
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = before;
			}
		}

		/// <summary>
		/// NO number anywhere in the report uses the machine's decimal separator.
		///
		/// The test above covers the utilisation figures; this covers the whole document, because
		/// they were not the only offenders. Measured on a printed report after fixing them: 126
		/// comma decimals remained — "100,00" and "0,7370" from the K/Y/X split and the per-mode
		/// fractions, `:P2`/`:P1` interpolations with no culture that predate this work entirely.
		///
		/// A whole-document sweep rather than a list of call sites: the next one added will be caught
		/// without anyone remembering to extend a list.
		/// </summary>
		[Test]
		public void NoNumberInTheReportUsesTheMachinesDecimalSeparator()
		{
			var before = System.Threading.Thread.CurrentThread.CurrentCulture;
			try
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");

				// A row WITH its derivation, or the K/Y/X and per-mode blocks never render and the
				// sweep passes over a document that contains none of the offending numbers.
				string html = NorsokHtmlReportGenerator.GenerateReport("test.ideaCon",
					new List<(string, List<NorsokFormulaResult>)>
						{ ("CON1", new List<NorsokFormulaResult> { WithDerivation() }) });

				// The embedded KaTeX script and stylesheet are third-party verbatim, and minified JS
				// is full of "1,2" sequences that are not decimals at all.
				int bodyAt = html.IndexOf("</head>", StringComparison.Ordinal);
				string body = html[bodyAt..];

				var commas = System.Text.RegularExpressions.Regex.Matches(body, @"\d,\d")
					.Select(m => m.Value)
					.Distinct()
					.ToList();

				Assert.That(commas, Is.Empty,
					"comma decimals reached the page: " + string.Join(", ", commas));
			}
			finally
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = before;
			}
		}

		/// <summary>
		/// A joint with NO BRACE is outside §6.4's scope, not an input problem.
		///
		/// Measured on CON10 of the shipped project: it holds ONE member, M2, continuous — no brace.
		/// §6.4 checks a brace against a chord, so there is no d, hence no β = d/D, no θ, no Q_u:
		/// eq (6.52)/(6.53)/(6.57) are undefined rather than unsatisfied, and no edit to the model
		/// changes that.
		///
		/// The report used to call this "the load effects of this connection could not be read" —
		/// true (the inherited states still reference the deleted braces, so the service answers
		/// 404) but a CONSEQUENCE of the same edit, and as NotEvaluated it told the reader to fix the
		/// model and re-run, which cannot help.
		/// </summary>
		[Test]
		public void AJointWithNoBraceIsOutsideScopeNotUnreadable()
		{
			var verdict = CheckWorkflow.Roll(new List<NorsokFormulaResult>
			{
				new()
				{
					Section = "6.4", Title = "Outside the scope of §6.4",
					CheckExpression = "the connection holds one continuous member (M2) and no brace "
						+ "— §6.4 checks a brace against a chord, so there is nothing to assess",
					NotAssessed = true, Reason = NotAssessedReason.OutsideScope,
				},
			});

			Assert.Multiple(() =>
			{
				Assert.That(verdict.Pass, Is.EqualTo("N/A"));
				Assert.That(verdict.Status, Does.Contain("Outside"),
					"the chapter does not cover a joint with no brace");
				Assert.That(verdict.Status, Does.Not.Contain("could not be read"),
					"and nothing failed to read — the geometry is simply not §6.4's");
			});
		}

		/// <summary>
		/// A valid joint carrying no load effect says SO — it does not report a read failure.
		///
		/// The third state, and it was collapsed into the second. A joint whose geometry §6.4 covers
		/// but which has no load effect — none defined, or every one switched off in the model (the
		/// app filters to active-only by default) — read perfectly well. Reporting "the model could
		/// not be read" about it states a fault that does not exist and sends the reader hunting for
		/// it. It stays NotEvaluated, because editing the model IS the fix here.
		/// </summary>
		/// <summary>
		/// DEACTIVATION and ABSENCE read differently, because they are different facts.
		///
		/// The API distinguishes them and the app nearly threw it away: it filters to active-only
		/// before a chapter sees anything, so both arrive as "nothing to check". Measured on the
		/// shipped project — CON8 and CON15 carry 15 states of which 7 are active, so this is a real
		/// configuration, not a hypothetical one. (An unreadable connection is a third case: CON10
		/// answers HTTP 404.)
		///
		/// Telling an engineer who deliberately switched every state off that their model has no
		/// load effect would be wrong about their model, and would send them looking for something
		/// that is not missing.
		/// </summary>
		[TestCase("all 15 load effect(s) of this connection are switched off in the model",
			"switched off", "no load effect defined",
			TestName = "every state switched off — say so, and do not call the model empty")]
		[TestCase("this connection has no load effect defined",
			"no load effect defined", "switched off",
			TestName = "no state defined — say that instead")]
		public void DeactivationAndAbsenceAreReportedDifferently(
			string checkExpression, string expected, string notExpected)
		{
			var verdict = CheckWorkflow.Roll(new List<NorsokFormulaResult>
			{
				new()
				{
					Section = "6.4", Title = "Nothing to assess",
					CheckExpression = checkExpression,
					NotAssessed = true, Reason = NotAssessedReason.NotEvaluated,
				},
			});

			Assert.Multiple(() =>
			{
				Assert.That(verdict.Pass, Is.EqualTo("N/A"));
				Assert.That(verdict.Status, Does.Contain(expected), "the reader is told which it is");
				Assert.That(verdict.Status, Does.Not.Contain(notExpected),
					"and not told the other one");
				Assert.That(verdict.Status, Does.Not.Contain("could not be read"),
					"the model read fine either way");
				Assert.That(verdict.Status, Does.Not.Contain("Outside"),
					"nor is the joint outside the chapter's scope — its geometry is fine");
			});
		}

		/// <summary>
		/// The app HANDS the chapter the pre-filter count, or the distinction above cannot be made.
		///
		/// `LoadEffectsInFile` is only meaningful if the run sets it: the filter happens in
		/// MainWindow.Run.cs, so a chapter that is handed only the filtered list can never tell
		/// "switched off" from "never defined". A model class nothing populates is the wiring defect
		/// that has appeared at every step of this work.
		/// </summary>
		[Test]
		public void TheRunPassesThePreFilterLoadEffectCount()
		{
			string run = ReadAppSource("MainWindow.Run.cs");
			string chapter = ReadAppSource("Services/Chapters/Chapter64.cs");

			Assert.Multiple(() =>
			{
				Assert.That(run, Does.Contain("LoadEffectsInFile = con.TotalLoadEffects"),
					"the count before the active-only filter reaches the chapter context");
				// It must DECIDE something, not merely appear. A bare Does.Contain passed with the
				// comparison replaced by `false`, because the property is named in the comment
				// above it as well.
				Assert.That(chapter, Does.Match(@"ctx\.LoadEffectsInFile\s*>\s*0"),
					"and the chapter branches on it to choose its wording");
			});
		}

		/// <summary>
		/// A load effect's name reaches the card intact, whatever it is called.
		///
		/// Worth a test because the names are FREE TEXT — a load effect is named in IDEA StatiCa
		/// exactly like a connection, so "LE12" is not a convention to rely on; it is only this
		/// app's fallback for a state the model left unnamed (JointEnvelope.cs:56). Diacritics and
		/// an ampersand are the two cases a tidy "LE1" fixture never exercises: the first has to
		/// survive the round trip, the second has to be escaped or it breaks the document.
		/// </summary>
		[Test]
		public void ALoadEffectsOwnNameReachesTheCard()
		{
			NorsokFormulaResult At(string le)
			{
				var r = Passed(0.5);
				r.LoadCaseName = le;
				return r;
			}

			string html = Report(
				("CON1", new[] { At("Vítr Y+") }),
				("CON2", new[] { At("ULS 3 & wind") }));

			// The BADGE, not the document: `&amp;` occurs all over the page (&mdash;, &sect;), so
			// asserting on it document-wide can never fail — measured, removing Esc() from the badge
			// left that version of this test green.
			var badges = System.Text.RegularExpressions.Regex
				.Matches(html, @"<span class='lc-badge'>(.*?)</span>")
				.Select(m => m.Groups[1].Value)
				.ToList();

			Assert.Multiple(() =>
			{
				Assert.That(badges, Has.Count.EqualTo(2), "one badge per check");
				Assert.That(badges, Does.Contain("V&#237;tr Y+"),
					"diacritics reach the badge, encoded");
				Assert.That(badges, Does.Contain("ULS 3 &amp; wind"),
					"and the ampersand is escaped IN THE BADGE — raw, it would open an entity");
			});
		}

		/// <summary>An assessed row with a full §6.4 derivation, so every block renders.</summary>
		private static NorsokFormulaResult WithDerivation()
		{
			var inputs = NorsokChecker.Services.Norsok64.Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 60.0, g: 0.047,
				frK: 1.0, frY: 0.0, frX: 0.0,
				nSd: -10e3, mipSd: -1e3, mopSd: 0,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0,
				gammaM: 1.15);

			var row = Passed(0.737);
			row.JointDetail = new NorsokChecker.Services.Norsok64.JointCheckRow
			{
				Name = "M1", Skipped = false, Util = 0.737, Passed = true,
				Engine = NorsokChecker.Services.Norsok64.Norsok64Engine.CheckJoint(inputs),
				Inputs = inputs, DomClass = "K",
				Classification = new NorsokChecker.Services.Norsok64.KyxClass
				{
					Name = "M1", FrK = 1.0, FrY = 0, FrX = 0,
				},
			};
			return row;
		}
	}
}
