using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Regression tests pinning the C# NORSOK §6.4 engine (<see cref="Norsok64Engine"/>) to the
	/// verified Python reference. Every expected number was produced by running the ground-truth
	/// scripts (n64.py self-test + Lukáš J. per-joint verification scripts) under Python 3.13:
	///   python_prototype/norsok/n64.py
	///   PYTHON_SCRIPTS_VERIFICATIONS_LUKAS_J/**/*.py
	/// If these fail, the C# port has diverged from the reference — fix the C#, not the numbers.
	/// </summary>
	[TestFixture]
	public class Norsok64EngineTests
	{
		// tolerances (values printed to 3–4 decimals by the Python scripts)
		private const double QuTol = 0.01;
		private const double NRdTol = 0.5;    // kN
		private const double MRdTol = 0.5;    // kNm
		private const double UtilTol = 0.002;
		private const double QfTol = 0.001;

		private static double Kn(double n) => n / 1e3;    // N   → kN
		private static double Knm(double nm) => nm / 1e3;  // N·m → kNm

		[Test]
		public void N64SelfTest_KDefault_MatchesReference()
		{
			// python_prototype/norsok/n64.py __main__ self-test (default K-joint)
			var inp = Joint64Input.FromKn(
				D: 508, T: 16, fyChord: 355, d: 300, t: 12, fyBrace: 355, thetaDeg: 45, g: 60,
				frK: 1.0, frY: 0.0, frX: 0.0,
				nSdKn: -800, mipSdKnm: 40, mopSdKnm: 10);
			var r = Norsok64Engine.CheckJoint(inp);

			Assert.Multiple(() =>
			{
				Assert.That(r.Beta, Is.EqualTo(0.591).Within(0.001), "β");
				Assert.That(r.Gamma, Is.EqualTo(15.88).Within(0.01), "γ");
				Assert.That(r.Tau, Is.EqualTo(0.750).Within(0.001), "τ");
				Assert.That(r.QBeta, Is.EqualTo(1.000).Within(0.001), "Qβ");
				Assert.That(r.Qg, Is.EqualTo(1.060).Within(0.001), "Qg");
				Assert.That(r.LoadAxial, Is.EqualTo("compression"));
				Assert.That(r.WithinRange, Is.True);

				Assert.That(Knm(r.MRdIp), Is.EqualTo(287.1).Within(MRdTol), "M_Rd,ip");
				Assert.That(Knm(r.MRdOp), Is.EqualTo(149.2).Within(MRdTol), "M_Rd,op");

				Assert.That(r.PerClass[Joint64Class.K].QuAxial, Is.EqualTo(19.746).Within(QuTol), "Qu K");
				Assert.That(r.PerClass[Joint64Class.Y].QuAxial, Is.EqualTo(16.879).Within(QuTol), "Qu Y");
				Assert.That(r.PerClass[Joint64Class.X].QuAxial, Is.EqualTo(10.824).Within(QuTol), "Qu X");
				Assert.That(Kn(r.PerClass[Joint64Class.K].NRd), Is.EqualTo(2206.9).Within(NRdTol), "N_Rd K");
				Assert.That(Kn(r.PerClass[Joint64Class.Y].NRd), Is.EqualTo(1886.3).Within(NRdTol), "N_Rd Y");
				Assert.That(Kn(r.PerClass[Joint64Class.X].NRd), Is.EqualTo(1209.7).Within(NRdTol), "N_Rd X");

				Assert.That(Kn(r.NRdWeighted), Is.EqualTo(2206.9).Within(NRdTol), "N_Rd weighted");
				Assert.That(r.UtilWeighted, Is.EqualTo(0.449).Within(UtilTol), "util");
				Assert.That(r.Passed, Is.True);
			});
		}

		[Test]
		public void Lukas_KJoint_MatchesReference()
		{
			// PYTHON_SCRIPTS_VERIFICATIONS_LUKAS_J/NORSOK CHAPTER 6.4 K AND KT-JOINTS/norsok_64_K_joint.py
			var inp = Joint64Input.FromKn(
				D: 457, T: 16, fyChord: 355, d: 273, t: 12, fyBrace: 355, thetaDeg: 45, g: 257,
				frK: 1.0,
				nSdKn: 250, mipSdKnm: 40, mopSdKnm: 20,
				sigmaASdMpa: 5.549, sigmaMySdMpa: 37.604, sigmaMzSdMpa: -0.762);
			var r = Norsok64Engine.CheckJoint(inp);
			var k = r.PerClass[Joint64Class.K];

			Assert.Multiple(() =>
			{
				Assert.That(r.Beta, Is.EqualTo(0.5974).Within(0.0005), "β");
				Assert.That(r.Gamma, Is.EqualTo(14.281).Within(0.01), "γ");
				Assert.That(r.Qg, Is.EqualTo(1.0).Within(0.001), "Qg");
				Assert.That(r.QBeta, Is.EqualTo(1.0).Within(0.001), "Qβ");
				Assert.That(k.QuAxial, Is.EqualTo(17.8573).Within(QuTol), "Qu axial");
				Assert.That(r.QuIpb, Is.EqualTo(8.0816).Within(QuTol), "Qu ipb");
				Assert.That(r.QuOpb, Is.EqualTo(4.4271).Within(QuTol), "Qu opb");
				Assert.That(k.QfAxialA2, Is.EqualTo(0.00717).Within(0.0001), "A²");
				Assert.That(k.QfAxial, Is.EqualTo(0.9879).Within(QfTol), "Qf axial");
				Assert.That(r.QfMoment, Is.EqualTo(1.0003).Within(QfTol), "Qf moment");
				Assert.That(Kn(r.NRdWeighted), Is.EqualTo(1971.57).Within(NRdTol), "N_Rd");
				Assert.That(Knm(r.MRdIp), Is.EqualTo(246.64).Within(MRdTol), "M_ip,Rd");
				Assert.That(Knm(r.MRdOp), Is.EqualTo(135.11).Within(MRdTol), "M_op,Rd");
				Assert.That(k.UtilAxialTerm, Is.EqualTo(0.1268).Within(UtilTol), "axial term");
				Assert.That(k.UtilIpTerm, Is.EqualTo(0.0263).Within(UtilTol), "ip term");
				Assert.That(k.UtilOpTerm, Is.EqualTo(0.1480).Within(UtilTol), "op term");
				Assert.That(r.UtilWeighted, Is.EqualTo(0.3011).Within(UtilTol), "util");
				Assert.That(r.Passed, Is.True);
			});
		}

		[Test]
		public void Lukas_XJoint_MatchesReference()
		{
			// PYTHON_SCRIPTS_VERIFICATIONS_LUKAS_J/NORSOK CHAPTER 6.4 X CONNECTION/norsok_64_X_joint.py
			var inp = Joint64Input.FromKn(
				D: 457, T: 16, fyChord: 355, d: 273, t: 12, fyBrace: 355, thetaDeg: 60, g: 0,
				frK: 0.0, frY: 0.0, frX: 1.0,
				nSdKn: 1355, mipSdKnm: 0, mopSdKnm: 0);
			var r = Norsok64Engine.CheckJoint(inp);
			var x = r.PerClass[Joint64Class.X];

			Assert.Multiple(() =>
			{
				Assert.That(r.Beta, Is.EqualTo(0.5974).Within(0.0005), "β");
				Assert.That(x.QuAxial, Is.EqualTo(11.3092).Within(QuTol), "Qu axial (6.4·γ^(0.6β²))");
				Assert.That(r.QuIpb, Is.EqualTo(8.0816).Within(QuTol), "Qu ipb");
				Assert.That(r.QuOpb, Is.EqualTo(4.4271).Within(QuTol), "Qu opb");
				Assert.That(x.QfAxial, Is.EqualTo(1.0).Within(QfTol), "Qf axial");
				Assert.That(Kn(r.NRdWeighted), Is.EqualTo(1031.98).Within(NRdTol), "N_Rd");
				Assert.That(Knm(r.MRdIp), Is.EqualTo(201.33).Within(MRdTol), "M_ip,Rd");
				Assert.That(Knm(r.MRdOp), Is.EqualTo(110.29).Within(MRdTol), "M_op,Rd");
				Assert.That(r.UtilWeighted, Is.EqualTo(1.3130).Within(UtilTol), "util");
				Assert.That(r.Passed, Is.False, "intentionally overloaded → FAIL");
			});
		}

		private static IEnumerable<TestCaseData> TyCases()
		{
			// All six share geometry D=168.3,T=8,fy=355,d=114.3,t=6.3,θ=60 (Lukáš T/Y scripts).
			// Args: name, N_kN, Mip_kNm, Mop_kNm, σa, σmy, σmz, QuAx, N_Rd, M_ip_Rd, M_op_Rd, util
			yield return new TestCaseData("PURE_TENSION", 250.0, 0.0, 0.0, -15.513, 0.0, 0.0,
				20.3743, 457.99, 20.07, 12.69, 0.5459);
			yield return new TestCaseData("PURE_COMPRESSION", -150.0, 0.0, 0.0, 9.308, 0.0, 0.0,
				18.0998, 415.93, 20.36, 12.88, 0.3606);
			yield return new TestCaseData("IN_PLANE_BENDING", 0.0, 20.0, 0.0, 0.0, -64.867, 0.0,
				20.3743, 457.13, 20.10, 12.71, 0.9904);
			yield return new TestCaseData("OUT_OF_PLANE_BENDING", 0.0, 0.0, -12.0, 0.0, 0.0, 19.460,
				20.3743, 464.11, 20.25, 12.81, 0.9371);
			yield return new TestCaseData("INTERACTION_TENSION", 100.0, 10.0, -5.0, -6.205, -32.433, 8.433,
				20.3743, 460.20, 20.15, 12.74, 0.8561);
			yield return new TestCaseData("INTERACTION_COMPRESSION", -100.0, 10.0, -5.0, 6.205, -32.433, 8.433,
				18.0998, 413.16, 20.29, 12.83, 0.8747);
		}

		[TestCaseSource(nameof(TyCases))]
		public void Lukas_TYJoint_MatchesReference(string name, double nKn, double mipKnm, double mopKnm,
			double sa, double smy, double smz,
			double quAx, double nRd, double mIpRd, double mOpRd, double util)
		{
			var inp = Joint64Input.FromKn(
				D: 168.3, T: 8, fyChord: 355, d: 114.3, t: 6.3, fyBrace: 355, thetaDeg: 60, g: 0,
				frK: 0.0, frY: 1.0, frX: 0.0,
				nSdKn: nKn, mipSdKnm: mipKnm, mopSdKnm: mopKnm,
				sigmaASdMpa: sa, sigmaMySdMpa: smy, sigmaMzSdMpa: smz);
			var r = Norsok64Engine.CheckJoint(inp);
			var y = r.PerClass[Joint64Class.Y];

			Assert.Multiple(() =>
			{
				Assert.That(r.Beta, Is.EqualTo(0.6791).Within(0.0005), $"{name} β");
				Assert.That(r.Gamma, Is.EqualTo(10.5188).Within(0.01), $"{name} γ");
				Assert.That(r.QuIpb, Is.EqualTo(7.7711).Within(QuTol), $"{name} Qu ipb");
				Assert.That(r.QuOpb, Is.EqualTo(4.9149).Within(QuTol), $"{name} Qu opb");
				Assert.That(y.QuAxial, Is.EqualTo(quAx).Within(QuTol), $"{name} Qu axial");
				Assert.That(Kn(r.NRdWeighted), Is.EqualTo(nRd).Within(NRdTol), $"{name} N_Rd");
				Assert.That(Knm(r.MRdIp), Is.EqualTo(mIpRd).Within(MRdTol), $"{name} M_ip,Rd");
				Assert.That(Knm(r.MRdOp), Is.EqualTo(mOpRd).Within(MRdTol), $"{name} M_op,Rd");
				Assert.That(r.UtilWeighted, Is.EqualTo(util).Within(UtilTol), $"{name} util");
				Assert.That(r.Passed, Is.True, $"{name} verdict");
			});
		}

		/// <summary>
		/// The overview's range qualifier is BUILT from the engine's own validity dictionary, so the
		/// row and the derivation table can never disagree about which condition failed.
		///
		/// Guards the production half of the round-2 §4.1 fix. The consuming half (CheckWorkflow.Roll)
		/// has its own tests; without this one, `RangeQualifierOf` could return null for every joint
		/// and the roll-up tests would still pass on their fixtures.
		/// </summary>
		[Test]
		public void RangeQualifier_NamesTheParameterAndItsValue()
		{
			// θ = 20° — CON11/M1 in the reviewed report: inside every other range, outside 30–90°.
			var outOfRange = Joint64Input.FromKn(
				D: 141, T: 6.5, fyChord: 355, d: 76, t: 3.5, fyBrace: 355, thetaDeg: 20, g: 50,
				frK: 0.0, frY: 0.0, frX: 1.0,
				nSdKn: 33.7, mipSdKnm: 0.32, mopSdKnm: 1.23);
			var rOut = Norsok64Engine.CheckJoint(outOfRange);

			// the same joint at 45° — inside every range
			var inRange = Joint64Input.FromKn(
				D: 141, T: 6.5, fyChord: 355, d: 76, t: 3.5, fyBrace: 355, thetaDeg: 45, g: 50,
				frK: 0.0, frY: 0.0, frX: 1.0,
				nSdKn: 33.7, mipSdKnm: 0.32, mopSdKnm: 1.23);
			var rIn = Norsok64Engine.CheckJoint(inRange);

			string? qOut = Joint64ReportAdapter.RangeQualifierOf("M1", rOut);
			string? qIn = Joint64ReportAdapter.RangeQualifierOf("M1", rIn);

			Assert.Multiple(() =>
			{
				Assert.That(rOut.WithinRange, Is.False, "control: 20° really is out of range");
				Assert.That(rIn.WithinRange, Is.True, "control: 45° really is inside");

				Assert.That(qIn, Is.Null, "a joint inside every range carries no qualifier");

				Assert.That(qOut, Is.Not.Null);
				Assert.That(qOut, Does.StartWith("M1:"), "the brace it belongs to");
				Assert.That(qOut, Does.Contain("20.0"), "the value that breached");
				Assert.That(qOut, Does.Contain("30–90"), "the range it breached");
				// θ is the ONLY breach here, so no other parameter may be named — a qualifier that
				// listed every condition would be as unhelpful as one that named none.
				Assert.That(qOut, Does.Not.Contain("β"), "β is inside; do not name it");
				Assert.That(qOut, Does.Not.Contain("γ"), "γ is inside; do not name it");
			});
		}

		/// <summary>
		/// Decimal points, not the machine's comma. The qualifier goes into the overview row of an
		/// exported report, and this project has re-introduced a locale bug five separate times.
		/// </summary>
		[Test]
		public void RangeQualifier_UsesDecimalPoints()
		{
			var inp = Joint64Input.FromKn(
				D: 141, T: 6.5, fyChord: 355, d: 76, t: 3.5, fyBrace: 355, thetaDeg: 20, g: 50,
				frK: 0.0, frY: 0.0, frX: 1.0,
				nSdKn: 33.7, mipSdKnm: 0.32, mopSdKnm: 1.23);

			var prev = System.Threading.Thread.CurrentThread.CurrentCulture;
			try
			{
				System.Threading.Thread.CurrentThread.CurrentCulture =
					new System.Globalization.CultureInfo("cs-CZ");
				string? q = Joint64ReportAdapter.RangeQualifierOf("M1", Norsok64Engine.CheckJoint(inp));

				Assert.That(q, Does.Contain("20.0"), "point, even under a comma-decimal culture");
				Assert.That(q, Does.Not.Contain("20,0"));
			}
			finally
			{
				System.Threading.Thread.CurrentThread.CurrentCulture = prev;
			}
		}

		[Test]
		public void OutOfRange_AppliesLesserCapacity_641()
		{
			// γ = 60 (> 50) — §6.4.3.1: usable strength = LESSER of actual-vs-clamped capacity.
			// Oracle from n64.check_joint vs _check_joint_once (γ clamped to 50 lowers M_Rd; the
			// N_Rd here is governed by the γ-independent 40·β^1.2 min term, so it is unchanged).
			var inp = Joint64Input.FromKn(
				D: 1200, T: 10, fyChord: 355, d: 300, t: 10, fyBrace: 355, thetaDeg: 45, g: 100,
				frK: 1.0, nSdKn: -500, mipSdKnm: 30, mopSdKnm: 10);

			var actual = Norsok64Engine.CheckJointOnce(inp);
			var check = Norsok64Engine.CheckJoint(inp);

			Assert.Multiple(() =>
			{
				// geometry fields ALWAYS reflect actual, never the clamped pass
				Assert.That(check.Beta, Is.EqualTo(0.25).Within(0.001), "β actual");
				Assert.That(check.Gamma, Is.EqualTo(60.0).Within(0.01), "γ actual");
				Assert.That(check.WithinRange, Is.False, "out of range");
				// lesser capacity: clamped M_Rd ≤ actual M_Rd
				Assert.That(check.MRdIp, Is.LessThanOrEqualTo(actual.MRdIp + 1e-6), "M_ip,Rd lesser");
				Assert.That(check.MRdOp, Is.LessThanOrEqualTo(actual.MRdOp + 1e-6), "M_op,Rd lesser");
				// pinned oracle numbers
				Assert.That(Kn(check.NRdWeighted), Is.EqualTo(360.67).Within(NRdTol), "N_Rd");
				Assert.That(Knm(check.MRdIp), Is.EqualTo(99.26).Within(MRdTol), "M_ip,Rd");
				Assert.That(Knm(check.MRdOp), Is.EqualTo(37.91).Within(MRdTol), "M_op,Rd");
				Assert.That(check.UtilWeighted, Is.EqualTo(1.7415).Within(UtilTol), "util");
				Assert.That(check.Passed, Is.False);
			});
		}

		[Test]
		public void ChordOverstress_ForcesFail_EvenWhenUtilLow()
		{
			// Heavy chord stress drives Qf (no floor in the norm) → the active class's N_Rd < 0.
			// The app-level guard must FAIL the joint even though util_weighted < 1.
			var inp = Joint64Input.FromKn(
				D: 508, T: 16, fyChord: 355, d: 300, t: 12, fyBrace: 355, thetaDeg: 45, g: 60,
				frK: 0.0, frY: 1.0, frX: 0.0,
				nSdKn: -200, mipSdKnm: 10, mopSdKnm: 5,
				sigmaASdMpa: -320, sigmaMySdMpa: 330, sigmaMzSdMpa: 250);
			var r = Norsok64Engine.CheckJoint(inp);

			Assert.Multiple(() =>
			{
				Assert.That(r.PerClass[Joint64Class.Y].NRd, Is.LessThan(0.0), "Y N_Rd negative");
				Assert.That(Kn(r.PerClass[Joint64Class.Y].NRd), Is.EqualTo(-1116.87).Within(NRdTol));
				Assert.That(r.ChordOverstressed, Is.True);
				Assert.That(r.UtilWeighted, Is.EqualTo(0.2589).Within(UtilTol), "util is low...");
				Assert.That(r.Passed, Is.False, "...but chord overstress forces FAIL");
			});
		}

		// ── Q_g in the interpolation band, note (b) under Table 6-3 ────────────────
		//
		// Round 3 of the report review reported this as the highest-priority finding: a printed
		// Q_g = 1.188 at g/D = 0.011 "cannot be an interpolated value", because interpolation is
		// bounded above by the first expression at g/D = 0.05, which is 1.127. The estimated
		// consequence was N_Rd 241.9 -> 171 kN and a utilisation 47.6 % -> 67 %, non-conservative.
		//
		// It is not a defect: the bound is inverted. It holds only when Qg_neg < Qg_pos, and on this
		// geometry Qg_neg = 1.283 > Qg_pos = 1.127, so across the band interpolation DECREASES with
		// g/D and every value in it sits ABOVE 1.127. 1.188 is the correct interpolated value.
		//
		// The reviewer could not have known — the report prints Q_g as a label and a number with no
		// substitution and no statement of the branch, unlike every other quantity beside it. These
		// tests exist because that exchange cost a round: the band was covered only through whole
		// joints, and not one fixture landed inside it.

		/// <summary>
		/// The exact case from the review, computed by the engine rather than by hand.
		///
		/// CON1/M1: D = 141, T = 6.5, t = 3.5, both materials 355 MPa, g/D = 0.011.
		/// </summary>
		[Test]
		public void QgInterpolatesInsideTheBandAndMayExceedTheUpperExpression()
		{
			const double d = 0.141, t = 0.0035, bigT = 0.0065, fy = 355e6;
			double gamma = d / (2 * bigT);

			double qgBand = Norsok64Engine.Qg(0.011 * d, d, t, bigT, fy, fy, gamma);
			double qgAtPlus = Norsok64Engine.Qg(0.05 * d, d, t, bigT, fy, fy, gamma);
			double qgAtMinus = Norsok64Engine.Qg(-0.05 * d, d, t, bigT, fy, fy, gamma);

			Assert.Multiple(() =>
			{
				Assert.That(qgBand, Is.EqualTo(1.1878).Within(0.0005),
					"the value the report prints as 1.188");
				Assert.That(qgAtPlus, Is.EqualTo(1.1272).Within(0.0005),
					"the g/D >= 0.05 expression at the band edge");
				Assert.That(qgAtMinus, Is.EqualTo(1.2827).Within(0.0005),
					"and the g/D <= -0.05 expression, which is the LARGER of the two here");
				Assert.That(qgBand, Is.GreaterThan(qgAtPlus),
					"so a value in the band legitimately exceeds the upper-branch edge value — the "
					+ "assumption that it cannot is what produced the round-3 finding");
			});
		}

		/// <summary>
		/// Continuous at both edges and monotonic across the band — the property that makes the
		/// interpolation an interpolation, and the one a reader cannot check from the report today.
		/// </summary>
		[Test]
		public void QgIsContinuousAtBothBandEdgesAndMonotonicBetweenThem()
		{
			const double d = 0.141, t = 0.0035, bigT = 0.0065, fy = 355e6;
			double gamma = d / (2 * bigT);
			double Q(double gD) => Norsok64Engine.Qg(gD * d, d, t, bigT, fy, fy, gamma);

			Assert.Multiple(() =>
			{
				// no step where the branches meet: just inside vs just outside
				Assert.That(Q(0.0499), Is.EqualTo(Q(0.0501)).Within(0.002), "continuous at +0.05");
				Assert.That(Q(-0.0501), Is.EqualTo(Q(-0.0499)).Within(0.002), "continuous at -0.05");

				double[] gd = { -0.05, -0.03, -0.01, 0.0, 0.011, 0.03, 0.05 };
				for (int i = 1; i < gd.Length; i++)
					Assert.That(Q(gd[i]), Is.LessThan(Q(gd[i - 1])),
						$"Qg must fall from g/D {gd[i - 1]} to {gd[i]} on this geometry");
			});
		}

		/// <summary>
		/// The other direction, so the test above is not passing on a coincidence of one geometry:
		/// where the NEGATIVE branch is the smaller of the two, the band rises with g/D instead and
		/// a band value is below the upper edge. Both orderings are legitimate; that is the point.
		/// </summary>
		[Test]
		public void WhichWayTheBandRunsDependsOnTheGeometry()
		{
			// A thin brace on a stocky chord makes phi small, so Qg_neg drops below Qg_pos.
			const double d = 0.5, t = 0.004, bigT = 0.04, fy = 355e6;
			double gamma = d / (2 * bigT);

			double qgAtMinus = Norsok64Engine.Qg(-0.05 * d, d, t, bigT, fy, fy, gamma);
			double qgAtPlus = Norsok64Engine.Qg(0.05 * d, d, t, bigT, fy, fy, gamma);
			double qgBand = Norsok64Engine.Qg(0.0, d, t, bigT, fy, fy, gamma);

			Assert.Multiple(() =>
			{
				Assert.That(qgAtMinus, Is.LessThan(qgAtPlus),
					"this geometry is the opposite ordering to the case above");
				Assert.That(qgBand, Is.GreaterThan(qgAtMinus).And.LessThan(qgAtPlus),
					"and the band value lies between the two edges, as interpolation requires");
			});
		}
	}
}
