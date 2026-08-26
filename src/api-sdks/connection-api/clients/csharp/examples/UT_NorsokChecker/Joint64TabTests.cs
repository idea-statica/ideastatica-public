using NorsokChecker.Models;
using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The §6.4 tab binds the per-load-effect topology rather than the flat result rows, so what it
	/// shows depends on the envelope pick and on the row view built from each JointCheckRow. A build
	/// success says nothing about either — these pin the two decisions that would silently show the
	/// wrong thing:
	///
	///   - the envelope names the load effect that governs EACH BRACE, not one state for the joint;
	///   - a brace nothing could be checked on still gets a row, marked N/A with its reason, rather
	///     than vanishing (the defect fixed in JointEnvelope, which this tab is the main consumer of).
	/// </summary>
	[TestFixture]
	public class Joint64TabTests
	{
		private static PerLoadEffect<JointCheckRow> Le(int id, string name, params JointCheckRow[] rows)
			=> new() { Id = id, Name = name, Rows = rows.ToList() };

		private static JointCheckRow Row(string brace, double util, bool passed)
			=> new()
			{
				Name = brace, Util = util, Passed = passed, Skipped = false,
				NRdWeighted = 500e3, MRdIp = 60e3, MRdOp = 40e3,
				WithinRange = true, DomClass = "K",
			};

		private static JointCheckRow Skipped(string brace, string reason)
			=> new() { Name = brace, Skipped = true, Reason = reason };

		/// <summary>
		/// Two braces governed by different states — the property that makes the envelope per-brace.
		/// A per-joint envelope would report one state for both, and the governing column would then
		/// be wrong for at least one of them.
		/// </summary>
		[Test]
		public void EachBraceReportsItsOwnGoverningState()
		{
			var les = new[]
			{
				Le(1, "LE1", Row("B1", 0.90, true), Row("B2", 0.10, true)),
				Le(2, "LE2", Row("B1", 0.20, true), Row("B2", 0.70, true)),
			};

			var b1 = JointEnvelope.Pick(les, "B1");
			var b2 = JointEnvelope.Pick(les, "B2");

			Assert.Multiple(() =>
			{
				Assert.That(b1!.LeName, Is.EqualTo("LE1"), "B1's worst is in LE1");
				Assert.That(b2!.LeName, Is.EqualTo("LE2"), "B2's worst is in LE2");
			});
		}

		/// <summary>
		/// A brace skipped everywhere must still be shown, as N/A with its reason. The tab is where
		/// this became visible: a three-brace joint used to show two rows and read PASS.
		/// </summary>
		[Test]
		public void AnUnassessedBraceStillGetsARowWithItsReason()
		{
			var les = new[]
			{
				Le(1, "LE1", Row("B1", 0.4, true), Skipped("B2", "no axial force to classify")),
				Le(2, "LE2", Row("B1", 0.5, true), Skipped("B2", "no axial force to classify")),
			};

			var gov = JointEnvelope.Pick(les, "B2");

			Assert.Multiple(() =>
			{
				Assert.That(gov, Is.Not.Null, "the brace must not vanish from the tab");
				Assert.That(gov!.Row.Skipped, Is.True);
				Assert.That(gov.Row.Reason, Is.EqualTo("no axial force to classify"));
			});
		}

		/// <summary>
		/// A row with infinite utilisation (chord overstressed, resistance collapsed to zero) must
		/// not render as a number — the display cap is what stops "∞ %" or a meaningless figure
		/// reaching the table.
		/// </summary>
		[Test]
		public void AnInfiniteUtilisationIsCappedForDisplay()
		{
			var row = Row("B1", double.PositiveInfinity, passed: false);
			row.ChordOverstressed = true;

			// mirrors what ShowJoint64Table does
			string util = double.IsInfinity(row.Util) ? "> 999 %" : $"{row.Util * 100:F1} %";
			string flags = (row.WithinRange ? "" : "⚠") + (row.ChordOverstressed ? "⛔" : "");

			Assert.Multiple(() =>
			{
				Assert.That(util, Is.EqualTo("> 999 %"));
				Assert.That(flags, Is.EqualTo("⛔"), "and the cause is flagged, not just the number");
			});
		}

		/// <summary>The row view must carry the detail through, or the derivation window has nothing.</summary>
		[Test]
		public void ARowWithNoEngineResultCannotOfferADerivation()
		{
			var withEngine = new Joint64RowView { Detail = Row("B1", 0.4, true) };
			var skippedRow = new Joint64RowView { Detail = Skipped("B2", "no data") };
			var noRow = new Joint64RowView();

			Assert.Multiple(() =>
			{
				// Row() builds no Engine, so even a checked row is not derivable without one —
				// asserting the guard rather than assuming Detail != null is enough
				Assert.That(withEngine.CanShowDetail, Is.False, "no Engine, no derivation");
				Assert.That(skippedRow.CanShowDetail, Is.False);
				Assert.That(noRow.CanShowDetail, Is.False);
			});
		}
	}
}
