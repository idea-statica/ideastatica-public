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
	}
}
