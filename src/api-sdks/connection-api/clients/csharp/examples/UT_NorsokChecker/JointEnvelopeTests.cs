using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Pins the per-brace envelope selection rule.
	///
	/// These have no python counterpart and no oracle entry: the python reference returns the full
	/// per-LE matrix and lets its UI envelope it, so the C# envelope is code the 1e-6 oracle never
	/// touched. That is exactly why a defect could live in it — the rule used to order by
	/// utilisation AND pass/fail at once, which could drop a failing state or report a governing
	/// utilisation far below the real worst.
	/// </summary>
	[TestFixture]
	public class JointEnvelopeTests
	{
		private static PerLoadEffect<JointCheckRow> Le(int id, string name, params JointCheckRow[] rows)
			=> new() { Id = id, Name = name, Rows = rows.ToList() };

		private static JointCheckRow Row(string brace, double util, bool passed)
			=> new() { Name = brace, Util = util, Passed = passed, Skipped = false };

		private static JointCheckRow Skipped(string brace, string reason)
			=> new() { Name = brace, Skipped = true, Reason = reason };

		/// <summary>
		/// The regression this class exists for. A failing state between two passing ones used to
		/// be promoted on its FAIL alone and then overwritten by the next higher-util PASS, so the
		/// envelope reported 0.60 PASS while 0.80 existed. Highest utilisation must govern.
		/// </summary>
		[Test]
		public void Pick_HighestUtilGoverns_EvenWhenALowUtilRowFailed()
		{
			var les = new[]
			{
				Le(1, "LE1", Row("B1", 0.80, passed: true)),
				Le(2, "LE2", Row("B1", 0.05, passed: false)),
				Le(3, "LE3", Row("B1", 0.60, passed: true)),
			};

			var gov = JointEnvelope.Pick(les, "B1");

			Assert.That(gov, Is.Not.Null);
			Assert.Multiple(() =>
			{
				Assert.That(gov!.Row.Util, Is.EqualTo(0.80).Within(1e-12), "the worst utilisation must govern");
				Assert.That(gov.LeId, Is.EqualTo(1), "and the pointer must name the state it came from");
				Assert.That(gov.LeName, Is.EqualTo("LE1"));
			});
		}

		/// <summary>Order must not change the winner — the same three rows, reversed.</summary>
		[Test]
		public void Pick_IsOrderIndependent()
		{
			var forward = new[]
			{
				Le(1, "LE1", Row("B1", 0.80, passed: true)),
				Le(2, "LE2", Row("B1", 0.05, passed: false)),
				Le(3, "LE3", Row("B1", 0.60, passed: true)),
			};
			var reversed = forward.Reverse().ToArray();

			Assert.That(JointEnvelope.Pick(reversed, "B1")!.LeId,
				Is.EqualTo(JointEnvelope.Pick(forward, "B1")!.LeId));
		}

		/// <summary>
		/// A chord-overstressed / out-of-range row carries +infinity and must win on utilisation
		/// alone — this is why the rule needs no pass/fail term.
		/// </summary>
		[Test]
		public void Pick_InfiniteUtilGoverns()
		{
			var les = new[]
			{
				Le(1, "LE1", Row("B1", 0.95, passed: true)),
				Le(2, "LE2", Row("B1", double.PositiveInfinity, passed: false)),
			};

			var gov = JointEnvelope.Pick(les, "B1");

			Assert.That(gov!.LeId, Is.EqualTo(2));
			Assert.That(double.IsPositiveInfinity(gov.Row.Util));
		}

		/// <summary>A skipped row must never win over a real one, whatever the order.</summary>
		[Test]
		public void Pick_SkippedNeverBeatsAReal()
		{
			var les = new[]
			{
				Le(1, "LE1", Skipped("B1", "missing section data")),
				Le(2, "LE2", Row("B1", 0.10, passed: true)),
				Le(3, "LE3", Skipped("B1", "missing section data")),
			};

			var gov = JointEnvelope.Pick(les, "B1");

			Assert.That(gov!.LeId, Is.EqualTo(2));
			Assert.That(gov.Row.Skipped, Is.False);
		}

		/// <summary>
		/// Skipped everywhere → the SKIPPED row is still returned, carrying its reason.
		///
		/// This test used to assert null, which is what the code did — and it pinned a real defect
		/// rather than the reference rule. With null, the caller added no result of any kind: a
		/// three-brace joint where one brace carries no force in any state published two rows,
		/// counted two checks, and the connection read PASS with a brace nobody had assessed,
		/// invisible in the grid, the §6.4 tab and the report alike. The python reference
		/// (ui.html envelopeData) accepts the first row unconditionally for exactly this reason.
		/// </summary>
		[Test]
		public void Pick_ReturnsTheSkippedRowWhenSkippedEverywhere()
		{
			var les = new[]
			{
				Le(1, "LE1", Skipped("B1", "no axial force to classify")),
				Le(2, "LE2", Skipped("B1", "no axial force to classify")),
			};

			var gov = JointEnvelope.Pick(les, "B1");

			Assert.Multiple(() =>
			{
				Assert.That(gov, Is.Not.Null, "a brace must never vanish from the envelope");
				Assert.That(gov!.Row.Skipped, Is.True, "and it must be recognisable as unassessed");
				Assert.That(gov.Row.Reason, Is.EqualTo("no axial force to classify"));
				Assert.That(JointEnvelope.SkipReason(les, "B1"), Is.EqualTo("no axial force to classify"));
			});
		}

		/// <summary>A brace with no row at all in any state is the one case that yields null.</summary>
		[Test]
		public void Pick_ReturnsNullOnlyWhenTheBraceHasNoRowAtAll()
		{
			var les = new[]
			{
				Le(1, "LE1", Row("B1", 0.5, true)),
				Le(2, "LE2", Row("B1", 0.6, true)),
			};

			Assert.That(JointEnvelope.Pick(les, "B2"), Is.Null);
		}

		/// <summary>
		/// The consequence that made the null case a defect rather than a detail: EVERY brace of the
		/// joint must come back from the envelope, so the caller can publish one row each. With the
		/// third brace dropped, the connection counted two checks out of three and read PASS.
		///
		/// Asserted as a count over the brace set, not per brace, because that is the property that
		/// was violated — each individual Pick call looked reasonable in isolation.
		/// </summary>
		[Test]
		public void Pick_YieldsARowForEveryBrace_EvenTheUnassessedOne()
		{
			var braces = new[] { "B1", "B2", "B3" };
			var les = new[]
			{
				Le(1, "LE1", Row("B1", 0.40, true), Row("B2", 0.70, true),
					Skipped("B3", "no axial force to classify (K/Y/X = 0) and no bending load")),
				Le(2, "LE2", Row("B1", 0.55, true), Row("B2", 0.30, true),
					Skipped("B3", "no axial force to classify (K/Y/X = 0) and no bending load")),
			};

			var governing = braces
				.Select(b => new { Brace = b, Gov = JointEnvelope.Pick(les, b) })
				.ToList();

			Assert.Multiple(() =>
			{
				Assert.That(governing.Count(g => g.Gov != null), Is.EqualTo(3),
					"a three-brace joint must produce three envelope entries, assessed or not");
				Assert.That(governing.Single(g => g.Brace == "B3").Gov!.Row.Skipped, Is.True,
					"and the unassessed one must be marked, not omitted");
				Assert.That(governing.Where(g => g.Brace != "B3").All(g => !g.Gov!.Row.Skipped), Is.True,
					"while the loaded braces keep their real governing rows");
			});
		}

		/// <summary>
		/// Braces are enveloped independently — two braces may be governed by different states.
		/// This is what makes the pointer per-brace rather than per-connection.
		/// </summary>
		[Test]
		public void Pick_BracesGovernIndependently()
		{
			var les = new[]
			{
				Le(1, "LE1", Row("B1", 0.90, true), Row("B2", 0.10, true)),
				Le(2, "LE2", Row("B1", 0.20, true), Row("B2", 0.70, true)),
			};

			Assert.Multiple(() =>
			{
				Assert.That(JointEnvelope.Pick(les, "B1")!.LeId, Is.EqualTo(1));
				Assert.That(JointEnvelope.Pick(les, "B2")!.LeId, Is.EqualTo(2));
			});
		}

		/// <summary>An unnamed load effect falls back to a readable "LE{id}" label.</summary>
		[Test]
		public void Pick_UnnamedLoadEffectGetsAFallbackLabel()
		{
			var les = new[] { Le(7, "", Row("B1", 0.5, true)) };

			Assert.That(JointEnvelope.Pick(les, "B1")!.LeName, Is.EqualTo("LE7"));
		}
	}
}
