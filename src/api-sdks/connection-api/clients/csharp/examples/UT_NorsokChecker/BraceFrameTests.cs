using NorsokChecker.Services.Norsok64;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The per-brace frame the §6.4 forces are resolved into.
	///
	/// It has to satisfy four things at once, and they constrain each other:
	///   x = the brace axis, away from the node, so +N_Sd is TENSION;
	///   y = the joint plane's NORMAL, because eq (6.57) calls the moment about it M_y = in-plane;
	///   z = IN the plane, so M_z is out-of-plane bending and V_z is the in-plane shear;
	///   and the triple must be RIGHT-handed.
	///
	/// It was left-handed until 2026-09-02 (`ip = nb × bx`, triple product −1) — found when the user
	/// noticed that labelling the shears y/z alongside the moments asserts a shared axis that a
	/// moment (about an axis) and a force (along one) do not share.
	///
	/// Only the sense of z changed. That is safe because no result depends on it — asserted below,
	/// not merely argued — while the sense of y does change Q_f through σ_my and is guarded by the
	/// signed sigma_my values in LiveValidationTests' oracle.
	/// </summary>
	[TestFixture]
	public class BraceFrameTests
	{
		/// <summary>
		/// A brace and chord deliberately off every global plane.
		///
		/// An axis-aligned case can make a wrong frame look right: with ex = global X and a brace in
		/// the XY plane, several wrong cross products coincide with the correct one. Every vector
		/// here has three non-zero components for that reason.
		/// </summary>
		private static (Vec3 Ex, JointMemberData Brace) OffAxisJoint()
		{
			var ex = new Vec3(0.3, 0.9, 0.32).Unit();
			var bx = new Vec3(0.5, -0.62, 0.6).Unit();

			return (ex, new JointMemberData
			{
				Name = "B1",
				// The DIRECTION is off-axis, which is what matters for the frame.
				AxisX = bx,
				// The loading axes are deliberately the GLOBAL ones, so a section load (N, Vy, Vz)
				// assembles into exactly that global vector — which is what lets the tests below
				// point a unit force in a chosen global direction and read the frame's response.
				// (The resolver takes the member's direction from AxisX and the load frame from all
				// three, so these two need not agree.)
				AxisY = new Vec3(0.0, 1.0, 0.0),
				AxisZ = new Vec3(0.0, 0.0, 1.0),
				IsContinuous = false,
			});
		}

		/// <summary>
		/// THE frame test: (x, y, z) = (bx, nb, ip) is right-handed, i.e. x·(y×z) > 0.
		///
		/// Recovers the axes from what BraceForceInPlane RETURNS, by resolving three unit loads. A
		/// first version computed `ip` itself and then asserted on its own arithmetic — it passed
		/// against the reverted resolver, which is the restatement trap: a test that redoes the
		/// production formula tests nothing but itself. (Caught by the oracle.)
		///
		/// The recovery works because the resolver's outputs ARE the projections onto the frame: a
		/// unit force along a global axis gives that axis's components as (V_z, V_y, N_Sd) — so three
		/// of them reconstruct the frame exactly as production built it.
		/// </summary>
		[Test]
		public void TheBraceFrameIsRightHanded()
		{
			var (ex, brace) = OffAxisJoint();
			var nPlane = Vec3.Cross(ex, JointForceResolver.EffDir(brace)).Unit();

			// One unit force along each of the member's LOCAL axes (which is what a section load
			// addresses: F = AxisX·N + AxisY·Vy + AxisZ·Vz). Each returns that local axis expressed
			// in the brace frame, so the three together are the change-of-basis matrix — and its
			// determinant is the frame's handedness, since both bases are orthonormal.
			Vec3 Column(double n, double vy, double vz)
			{
				var row = JointForceResolver.BraceForceInPlane(brace,
					new IdeaStatiCa.Api.Connection.Model.ConLoadEffectSectionLoad
					{ N = n, Vy = vy, Vz = vz },
					ex, nPlane);
				return new Vec3(row.NSd, row.Vop, row.Vip);   // (x, y, z) = (bx, nb, ip)
			}

			Vec3 gx = Column(1, 0, 0), gy = Column(0, 1, 0), gz = Column(0, 0, 1);

			// det of the frame matrix: +1 right-handed, −1 left-handed (the axes are orthonormal,
			// so the determinant is exactly the triple product of the frame vectors).
			double det =
				gx.X * (gy.Y * gz.Z - gy.Z * gz.Y)
				- gx.Y * (gy.X * gz.Z - gy.Z * gz.X)
				+ gx.Z * (gy.X * gz.Y - gy.Y * gz.X);

			Assert.Multiple(() =>
			{
				// The SIGN, not the magnitude: this fixture's load frame is deliberately not
				// orthonormal (AxisX is the off-axis brace, AxisY/AxisZ are global), so the
				// determinant is scaled — measured ±0.5014 here. The sign is what carries the
				// handedness, and it flips cleanly with the axis.
				Assert.That(det, Is.GreaterThan(0),
					$"det = {det:F4} — the brace frame (x,y,z) = (brace axis, plane normal, in-plane) "
					+ "must be right-handed; ip = nb × bx makes it negative");

				// The axes are still what they are supposed to BE: reversing a sense must change the
				// direction, not which line the axis lies on.
				var (bx, nb) = JointForceResolver.BraceSubplaneNormal(brace, ex, nPlane);
				Assert.That(Math.Abs(Vec3.Dot(nb, bx)), Is.LessThan(1e-9), "y is transverse to the brace");
				Assert.That(Math.Abs(Vec3.Dot(nb, nPlane)), Is.GreaterThan(0.99), "y IS the plane normal");
			});
		}

		/// <summary>
		/// The in-plane shear is the one the frame calls V_z, and the out-of-plane one V_y.
		///
		/// Asserted through the resolver, on a load built to be purely in-plane: it is the fact the
		/// §6.4 tab's column bindings depend on, and the reason V_z and V_y bind to Vip and Vop
		/// respectively rather than name-for-name.
		/// </summary>
		[Test]
		public void TheInPlaneShearIsTheOneCalledVz()
		{
			var (ex, brace) = OffAxisJoint();
			var nPlane = Vec3.Cross(ex, JointForceResolver.EffDir(brace)).Unit();
			var (bx, nb) = JointForceResolver.BraceSubplaneNormal(brace, ex, nPlane);

			// A force along the plane normal is, by definition, entirely out of plane.
			var outOfPlane = ForceAlong(brace, ex, nPlane, nb);
			// A force along bx × nb lies in the plane and is transverse to the brace.
			var inPlane = ForceAlong(brace, ex, nPlane, Vec3.Cross(bx, nb).Unit());

			Assert.Multiple(() =>
			{
				Assert.That(Math.Abs(outOfPlane.Vop), Is.GreaterThan(0.99),
					"a force along the normal shows up as Vop — the column headed V_y");
				Assert.That(Math.Abs(outOfPlane.Vip), Is.LessThan(1e-9), "and not as Vip");

				Assert.That(Math.Abs(inPlane.Vip), Is.GreaterThan(0.99),
					"a force in the plane shows up as Vip — the column headed V_z");
				Assert.That(Math.Abs(inPlane.Vop), Is.LessThan(1e-9), "and not as Vop");
			});
		}

		// The §6.4 tab's column BINDINGS are checked in CheckTabDefaultsTests, not here: that fixture
		// is [Apartment(STA)] as a whole and creates the Application in OneTimeSetUp. Putting one STA
		// test inside this non-STA fixture broke three unrelated WPF fixtures (rendering, member
		// colouring) — measured, and green again once the test moved.

		/// <summary>
		/// Resolve a unit force pointing along the GLOBAL direction <paramref name="dir"/>.
		///
		/// A section load is expressed in the member's own axes (`F = AxisX·N + AxisY·Vy + AxisZ·Vz`),
		/// and this fixture's AxisX is the off-axis brace — so the global direction has to be
		/// decomposed into that basis first. Solving the 3×3 rather than assuming the axes are
		/// global: they are not, and assuming it made the first version of these tests measure a
		/// different force from the one they named.
		/// </summary>
		private static BraceForceRow ForceAlong(JointMemberData m, Vec3 ex, Vec3 nPlane, Vec3 dir)
		{
			// dir = AxisX·n + AxisY·vy + AxisZ·vz  ->  solve for (n, vy, vz) by Cramer's rule.
			Vec3 a = m.AxisX, b = m.AxisY, c = m.AxisZ;
			double Det(Vec3 u, Vec3 v, Vec3 w) => Vec3.Dot(u, Vec3.Cross(v, w));

			double d = Det(a, b, c);
			double n = Det(dir, b, c) / d;
			double vy = Det(a, dir, c) / d;
			double vz = Det(a, b, dir) / d;

			return JointForceResolver.BraceForceInPlane(m,
				new IdeaStatiCa.Api.Connection.Model.ConLoadEffectSectionLoad
				{ N = n, Vy = vy, Vz = vz },
				ex, nPlane);
		}

		/// <summary>
		/// The check is INVARIANT to the sense of z — the claim the whole change rests on.
		///
		/// Argued from three places in the engine (M_z enters as |M_z|, σ_mz only squared,
		/// classification from N_Sd alone), but an argument is not a guard: this runs the engine on
		/// the same joint with M_z and σ_mz negated and requires every result to match.
		/// </summary>
		[Test]
		public void FlippingTheOutOfPlaneSenseChangesNoResult()
		{
			Joint64Input Make(double sign) => Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 55.0, g: 0.047,
				frK: 0.6, frY: 0.0, frX: 0.4,
				nSd: -18e3,
				mipSd: -1.4e3,
				mopSd: sign * 0.9e3,            // M_z: the quantity whose sign the flip changes
				sigmaASd: 9.27e6, sigmaMySd: -25.48e6,
				sigmaMzSd: sign * 4.0e6,        // σ_mz: likewise
				gammaM: 1.15);

			var a = Norsok64Engine.CheckJoint(Make(+1));
			var b = Norsok64Engine.CheckJoint(Make(-1));

			Assert.Multiple(() =>
			{
				Assert.That(b.UtilWeighted, Is.EqualTo(a.UtilWeighted).Within(1e-12), "utilisation");
				Assert.That(b.Passed, Is.EqualTo(a.Passed), "verdict");
				Assert.That(b.NRdWeighted, Is.EqualTo(a.NRdWeighted).Within(1e-9), "N_Rd");
				Assert.That(b.MRdIp, Is.EqualTo(a.MRdIp).Within(1e-9), "M_y,Rd");
				Assert.That(b.MRdOp, Is.EqualTo(a.MRdOp).Within(1e-9), "M_z,Rd");
				Assert.That(b.QfMoment, Is.EqualTo(a.QfMoment).Within(1e-12), "Q_f (moment)");
				Assert.That(b.QfMomentA2, Is.EqualTo(a.QfMomentA2).Within(1e-12), "A²");
			});
		}

		/// <summary>
		/// The sense of the IN-PLANE bending sign, by contrast, DOES change a resistance — so the
		/// invariance above must not be mistaken for "signs do not matter here".
		///
		/// σ_my enters Q_f linearly through C₂ (eq 6.54). Measured: 1.8 % on this joint. This test
		/// exists to keep that asymmetry visible: the frame's y sense is load-bearing, its z sense is
		/// not, and only the latter was changed.
		/// </summary>
		[Test]
		public void FlippingTheInPlaneStressSignDoesChangeQf()
		{
			Joint64Input Make(double sigmaMy) => Joint64Input.FromSI(
				D: 0.141, T: 0.0065, fyChord: 355e6,
				d: 0.076, t: 0.0035, fyBrace: 355e6,
				thetaDeg: 55.0, g: 0.047,
				frK: 1.0, frY: 0.0, frX: 0.0,
				nSd: -18e3, mipSd: -1.4e3, mopSd: 0.9e3,
				sigmaASd: 9.27e6, sigmaMySd: sigmaMy, sigmaMzSd: 4.0e6,
				gammaM: 1.15);

			var neg = Norsok64Engine.CheckJoint(Make(-25.48e6));
			var pos = Norsok64Engine.CheckJoint(Make(+25.48e6));

			// The K axial row of Table 6-4 has C2 != 0, so the axial Q_f differs.
			double qfNeg = neg.PerClass[Joint64Class.K].QfAxial;
			double qfPos = pos.PerClass[Joint64Class.K].QfAxial;

			Assert.That(Math.Abs(qfNeg - qfPos), Is.GreaterThan(1e-6),
				$"σ_my's sign must reach Q_f (got {qfNeg:F6} vs {qfPos:F6}) — if this ever becomes "
				+ "invariant, either Table 6-4's C₂ was lost or σ_my stopped being signed, and the "
				+ "oracle's signed sigma_my values are what protect it");
		}
	}
}
