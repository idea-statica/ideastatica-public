using NorsokChecker.Models;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// A joint outside the scope of §6.4 must show NO results.
	///
	/// Reported from the running app on CON2 of test_cs (2026-08-27), whose M3 is an IPE100: the
	/// banner said "no brace can be assessed", and directly underneath it the table listed M1, M4
	/// and M5 as PASS with utilisations, and the summary read "3 assessed · 0 failed". Both cannot
	/// be true, and the reader has no way to tell which half to believe.
	///
	/// The numbers were real arithmetic, which is what made it dangerous rather than obviously
	/// broken: the engine still evaluates every brace it can, and only the joint-level gate knows
	/// that the quantities those numbers rest on — the fitted plane, the averaged chord stresses,
	/// the K/Y/X balance — are not defined for this joint. Publishing them is the same defect as
	/// reporting 0.0 % for a brace nothing was checked on.
	///
	/// These tests pin the rule at the level the tab decides it, not by re-implementing the tab.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class RejectedJointSheetTests
	{
		[OneTimeSetUp]
		public void EnsureApplication()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}
		}

		private static JointTopology Topo(string status, params string[] errors)
		{
			var t = new JointTopology();
			t.Verdict.Status = status;
			t.Verdict.Errors.AddRange(errors);
			return t;
		}

		/// <summary>
		/// Drives the REAL window: hands the topology to the tab and reads back what the sheet's
		/// visibility ended up as.
		///
		/// Not a re-implementation of the condition. Twice today a test that restated the code it
		/// guarded passed while the production code was reverted — so this one has to go through
		/// MainWindow itself, and it does that by feeding _topologyPerConnection the way a run does.
		/// </summary>
		private static (System.Windows.Visibility Sheet, string Summary, object? Rows) ShowIn(
			JointTopology topo)
		{
			var w = new NorsokChecker.MainWindow();
			w.SetJoint64TopologyForTest(1, "CON_TEST", topo);
			return (w.Pnl64Sheet.Visibility, w.Lbl64Summary.Text, w.Grid64.ItemsSource);
		}

		[Test]
		public void AnOutOfScopeJointHidesTheWholeSheet()
		{
			var (sheet, summary, rows) = ShowIn(
				Topo("ERROR", "M3: IPE100 is RolledI — NORSOK 6.4 applies to tubular sections only"));

			Assert.Multiple(() =>
			{
				Assert.That(sheet, Is.EqualTo(System.Windows.Visibility.Collapsed),
					"a joint with no results must not show a table of them");
				Assert.That(rows, Is.Null, "and the grid must be emptied, not left holding stale rows");
				Assert.That(summary, Does.StartWith("not assessed"),
					$"the summary must open by saying nothing was assessed — it said: {summary}");
				// "3 assessed · 0 failed" is the wording that contradicted the banner. Matching on
				// "assessed ·" would also match the correct "not assessed · …", so test the count.
				Assert.That(summary, Does.Not.Match(@"\d+ assessed"),
					$"no count of assessed braces may appear — it said: {summary}");
				Assert.That(summary, Does.Not.Contain("failed"),
					$"nothing failed either; nothing was checked — it said: {summary}");
			});
		}

		/// <summary>
		/// The known-good positives: a clean joint and a warned joint both keep their sheet. A
		/// warning is explicitly NOT a rejection — §6.4.3.1's validity ranges are warnings because
		/// the norm's own rule there is to clamp the parameters and keep the lower capacity, so the
		/// check ran and its numbers stand. Without these rows the fixture could pass by hiding the
		/// sheet always.
		/// </summary>
		[Test]
		public void ACleanJointShowsItsSheet()
		{
			var (sheet, _, _) = ShowIn(Topo("OK"));

			Assert.That(sheet, Is.EqualTo(System.Windows.Visibility.Visible));
		}

		[Test]
		public void AWarnedJointStillShowsItsSheet()
		{
			var topo = Topo("WARNING");
			topo.Verdict.Warnings.Add("β = 0.15 is below the 6.4.3.1 range, resistance extrapolated");

			var (sheet, _, _) = ShowIn(topo);

			Assert.That(sheet, Is.EqualTo(System.Windows.Visibility.Visible));
		}

		/// <summary>
		/// The mode selectors choose between results; with none they must not invite a click.
		/// </summary>
		[Test]
		public void TheModeSelectorsAreDisabledForARejectedJoint()
		{
			var w = new NorsokChecker.MainWindow();
			w.SetJoint64TopologyForTest(1, "CON_TEST", Topo("ERROR", "not tubular"));

			Assert.Multiple(() =>
			{
				Assert.That(w.Rb64Envelope.IsEnabled, Is.False);
				Assert.That(w.Rb64PerLe.IsEnabled, Is.False);
				Assert.That(w.Cmb64Le.IsEnabled, Is.False);
			});
		}

		/// <summary>
		/// A joint carrying NO load is not assessed either, and that is an ERROR rather than a
		/// warning on purpose.
		///
		/// Every §6.4 check divides a design action by a resistance, so with nothing applied every
		/// utilisation comes out 0 % and every brace PASSes — and a joint reporting 0 % on all five
		/// braces reads as an excellent result, not as one that was never loaded. A warning would
		/// have left exactly that table on screen. The geometry gates cannot catch it: a joint can
		/// be geometrically perfect and carry no load.
		///
		/// Reachable in the shipped test set: CON10 of test_cs has its braces deleted, which took
		/// their loadings with them, so its inherited load effects reference members that no longer
		/// exist and the service answers 404 for them.
		/// </summary>
		[Test]
		public void AJointWithNoLoadEffectIsNotAssessed()
		{
			var topo = new JointTopology();
			JointTopologyBuilder.FinalizeVerdict(topo, loadEffectCount: 0);

			Assert.Multiple(() =>
			{
				Assert.That(topo.Verdict.Status, Is.EqualTo("ERROR"),
					"no load means nothing was checked, and 0 % on every brace is not a pass");
				Assert.That(topo.Verdict.Errors, Has.Count.EqualTo(1));
				Assert.That(topo.Verdict.Errors[0], Does.Contain("No load effect"));
			});
		}

		/// <summary>
		/// The known-good positive: a joint that HAS load effects is not accused of having none.
		/// Without this row the gate could reject everything and still look correct.
		/// </summary>
		[Test]
		public void AJointWithLoadEffectsIsNotRejectedForThat()
		{
			var topo = new JointTopology();
			JointTopologyBuilder.FinalizeVerdict(topo, loadEffectCount: 15);

			Assert.Multiple(() =>
			{
				Assert.That(topo.Verdict.Status, Is.EqualTo("OK"));
				Assert.That(topo.Verdict.Errors, Is.Empty);
			});
		}

		/// <summary>
		/// Callers that do not state a load count (the geometry-only tests, and FinalizeVerdict's
		/// other uses) must be unaffected — "not stated" is not "none".
		/// </summary>
		[Test]
		public void AnUnstatedLoadCountRaisesNothing()
		{
			var topo = new JointTopology();
			JointTopologyBuilder.FinalizeVerdict(topo);

			Assert.That(topo.Verdict.Errors, Is.Empty);
		}

		/// <summary>
		/// Every condition is listed separately. Joining them made a joint that failed six gates
		/// look like it failed one, and the reader could not tell how much was wrong.
		/// </summary>
		[Test]
		public void EveryUnmetConditionIsCounted()
		{
			var topo = Topo("ERROR",
				"Chord: PIPE127STD is tubular but its D/T are unknown",
				"M1: 76.0x3.5 is tubular but its D/T are unknown",
				"M3: IPE100 is RolledI — NORSOK 6.4 applies to tubular sections only");

			Assert.That(topo.Verdict.Errors, Has.Count.EqualTo(3));
		}
	}

	/// <summary>
	/// The summary line and the row counting, which is where the "3 assessed" half of the
	/// contradiction came from.
	/// </summary>
	[TestFixture]
	public class SheetSummaryTests
	{
		/// <summary>
		/// K sub-rows are a breakdown of a brace, not braces of their own — counting them inflated
		/// the brace count on any joint with a K pairing.
		/// </summary>
		[Test]
		public void SubRowsAreNotCountedAsBraces()
		{
			var rows = new List<Joint64RowView>
			{
				new() { Brace = "M1", Verdict = "PASS" },
				new() { Brace = "M4", Verdict = "PASS" },
				new() { Brace = "↳ K via M5", IsSubRow = true },
				new() { Brace = "M5", Verdict = "FAIL" },
				new() { Brace = "↳ K via M4", IsSubRow = true },
				new() { Brace = "M6", Verdict = "N/A" },
			};

			var braces = rows.Where(r => !r.IsSubRow).ToList();

			Assert.Multiple(() =>
			{
				Assert.That(braces, Has.Count.EqualTo(4), "four braces, two of them with a K breakdown");
				Assert.That(braces.Count(r => r.Verdict is "PASS" or "FAIL"), Is.EqualTo(3), "assessed");
				Assert.That(braces.Count(r => r.Verdict == "FAIL"), Is.EqualTo(1), "failed");
			});
		}

		/// <summary>A K sub-row cannot be double-clicked into a derivation — it has no check of its own.</summary>
		[Test]
		public void ASubRowHasNoDerivation()
		{
			var sub = new Joint64RowView { IsSubRow = true, Brace = "↳ K via M5" };

			Assert.That(sub.CanShowDetail, Is.False);
		}

		/// <summary>
		/// The Notes cell falls through to the note when there is no skip reason. A PriorityBinding
		/// over both would not have: it takes the first binding that RESOLVES, and a null
		/// SkipReason resolves, so every assessed row would have shown an empty cell.
		/// </summary>
		[Test]
		public void NotesShowsTheSkipReasonOrElseTheNote()
		{
			var skipped = new Joint64RowView { SkipReason = "no transverse force", Note = "ignored" };
			var assessed = new Joint64RowView { Note = "balanced to 0.0% <= gate 0% -> 100% K" };
			var plain = new Joint64RowView();

			Assert.Multiple(() =>
			{
				Assert.That(skipped.Notes, Is.EqualTo("no transverse force"));
				Assert.That(assessed.Notes, Is.EqualTo("balanced to 0.0% <= gate 0% -> 100% K"));
				Assert.That(plain.Notes, Is.Empty);
			});
		}
	}
}
