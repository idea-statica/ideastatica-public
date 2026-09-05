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

		// `AnInfiniteUtilisationIsCappedForDisplay` was here and is gone. It copied the production
		// ternary into the test body ("mirrors what ShowJoint64Table does") and asserted the copy,
		// so it called no app code and would have stayed green through any change to the real one.
		// The cap lives inline in ShowJoint64Table (MainWindow.Joint64.cs:586) with no seam to call,
		// and adding one to test a number's formatting buys less than it risks.

		/// <summary>
		/// A derivation is offered when, and only when, there is one to show.
		///
		/// `CanShowDetail` has four conditions and this used to assert three absences and nothing
		/// else — no case where it returns true. It was green against `CanShowDetail => false`,
		/// which is the state where the app offers no derivation at all.
		///
		/// The positive case is built by giving the row a real engine result, so the fourth
		/// condition (IsSubRow) can be varied against it and each absence measured one at a time.
		/// </summary>
		[Test]
		public void ADerivationIsOfferedOnlyWhenThereIsOneToShow()
		{
			var checkedRow = Row("B1", 0.4, true);
			checkedRow.Engine = Norsok64Engine.CheckJoint(Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6, d: 0.102, t: 0.0065, fyBrace: 355e6,
				thetaDeg: 45.0, g: 0.047, frK: 1.0, frY: 0.0, frX: 0.0,
				nSd: -88.8e3, mipSd: -1.2e3, mopSd: 2.4e3,
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6, sigmaMzSd: 0.0, gammaM: 1.15));

			var derivable = new Joint64RowView { Detail = checkedRow };
			var asSubRow = new Joint64RowView { Detail = checkedRow, IsSubRow = true };
			var noEngine = new Joint64RowView { Detail = Row("B1", 0.4, true) };
			var skippedRow = new Joint64RowView { Detail = Skipped("B2", "no data") };
			var noRow = new Joint64RowView();

			Assert.Multiple(() =>
			{
				// FIRST: the case that must be true. Without it every assertion below is satisfied
				// by a property that is false for everything.
				Assert.That(derivable.CanShowDetail, Is.True,
					"a checked row with an engine result HAS a derivation to show");

				// Then each condition removed on its own, from that same row.
				Assert.That(asSubRow.CanShowDetail, Is.False, "a sub-row is part of another row");
				Assert.That(noEngine.CanShowDetail, Is.False, "no Engine, nothing to derive");
				Assert.That(skippedRow.CanShowDetail, Is.False, "a skipped brace was not checked");
				Assert.That(noRow.CanShowDetail, Is.False, "and no detail at all");
			});
		}
	}
}
