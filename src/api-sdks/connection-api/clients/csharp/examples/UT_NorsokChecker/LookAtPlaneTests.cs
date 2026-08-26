using System.Windows.Media.Media3D;
using NorsokChecker.Controls;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Proves the "look at the joint plane" view really faces the plane, for any plane.
	///
	/// The first attempt turned the MODEL, reusing the two drag angles (RotateZ about global Z, then
	/// RotateTilt about global X, clamped to +-89 deg). These tests killed it: a joint in the global
	/// XY plane — the commonest case there is, and the one whose answer can be reasoned out by hand —
	/// reached only |dot| = 0.84 between the plane normal and the line of sight, i.e. it never faced
	/// the plane. Two angles against a fixed oblique camera cannot span it, and a separate check
	/// confirmed the "other equivalent solution" the code fell back on was not equivalent at all
	/// (facing dropped from 1.00 to 0.60). Moving the camera instead lands exactly.
	///
	/// The XY case is kept as the leading row for exactly that reason.
	///
	/// STA is required: Joint3DView is a WPF control.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class LookAtPlaneTests
	{
		private static (Point3D Position, Vector3D Look, Vector3D Up) Orient(
			Vector3D normal, Vector3D chord)
		{
			var view = new Joint3DView();
			view.LookAtPlane(normal, chord);
			Assert.That(view.HomeCameraForTest, Is.Not.Null, "LookAtPlane set no camera frame");
			return view.HomeCameraForTest!.Value;
		}

		/// <summary>
		/// The point of the whole thing: the plane is seen face-on, so its normal lies along the line
		/// of sight. |dot| because either face may be the one toward us.
		/// </summary>
		[TestCase(0.0, 0.0, 1.0, 1.0, 0.0, 0.0, TestName = "global XY plane, chord along X")]
		[TestCase(0.0, 0.0, 1.0, 0.0, 1.0, 0.0, TestName = "global XY plane, chord along Y")]
		[TestCase(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, TestName = "global YZ plane")]
		[TestCase(0.0, 1.0, 0.0, 1.0, 0.0, 0.0, TestName = "global XZ plane")]
		[TestCase(0.660, -0.660, -0.358, 1.0, 0.0, 0.0, TestName = "CON8 oblique plane")]
		[TestCase(0.3, 0.5, 0.81, 0.8, -0.6, 0.0, TestName = "arbitrary oblique plane")]
		public void ThePlaneEndsUpFacingTheCamera(
			double nx, double ny, double nz, double cx, double cy, double cz)
		{
			var normal = new Vector3D(nx, ny, nz);

			var cam = Orient(normal, new Vector3D(cx, cy, cz));

			normal.Normalize();
			var look = cam.Look;
			look.Normalize();
			double alignment = Math.Abs(Vector3D.DotProduct(look, normal));

			Assert.That(alignment, Is.GreaterThan(0.9999),
				$"the line of sight must lie along the plane normal; got |dot| = {alignment:F6}");
		}

		/// <summary>
		/// The chord is the joint's main direction, so it must run ACROSS the view, not up it. With
		/// up = look x chord, the chord ends up along screen-right by construction — asserted rather
		/// than assumed, because the cross-product order is easy to get backwards and the symptom
		/// (a vertical chord) looks like a plausible view rather than a bug.
		/// </summary>
		[TestCase(0.0, 0.0, 1.0, 1.0, 0.0, 0.0)]
		[TestCase(0.0, 0.0, 1.0, 0.0, 1.0, 0.0)]
		[TestCase(0.660, -0.660, -0.358, 1.0, 0.0, 0.0)]
		[TestCase(0.3, 0.5, 0.81, 0.8, -0.6, 0.0)]
		public void TheChordRunsAcrossTheView(
			double nx, double ny, double nz, double cx, double cy, double cz)
		{
			var chord = new Vector3D(cx, cy, cz);
			var cam = Orient(new Vector3D(nx, ny, nz), chord);

			// screen axes of the resulting camera
			var look = cam.Look; look.Normalize();
			var up = cam.Up; up.Normalize();
			var right = Vector3D.CrossProduct(up, look);
			right.Normalize();

			chord.Normalize();
			double alongRight = Math.Abs(Vector3D.DotProduct(chord, right));
			double alongUp = Math.Abs(Vector3D.DotProduct(chord, up));

			Assert.That(alongRight, Is.GreaterThan(alongUp),
				$"the chord should lie across the screen, not up it: |right| = {alongRight:F3} "
				+ $"vs |up| = {alongUp:F3}");
		}

		/// <summary>The camera frame must be orthogonal, or the render skews.</summary>
		[TestCase(0.0, 0.0, 1.0, 1.0, 0.0, 0.0)]
		[TestCase(0.660, -0.660, -0.358, 1.0, 0.0, 0.0)]
		public void TheCameraFrameIsOrthogonal(
			double nx, double ny, double nz, double cx, double cy, double cz)
		{
			var cam = Orient(new Vector3D(nx, ny, nz), new Vector3D(cx, cy, cz));
			var look = cam.Look; look.Normalize();
			var up = cam.Up; up.Normalize();

			Assert.That(Math.Abs(Vector3D.DotProduct(look, up)), Is.LessThan(1e-9),
				"up must be perpendicular to the line of sight");
		}

		/// <summary>
		/// A chord parallel to its own plane normal is not a real joint, but it must not produce a
		/// NaN camera — the fallback perpendicular has to kick in.
		/// </summary>
		[Test]
		public void AChordParallelToTheNormalStillYieldsAUsableCamera()
		{
			var cam = Orient(new Vector3D(0, 0, 1), new Vector3D(0, 0, 1));

			Assert.Multiple(() =>
			{
				Assert.That(double.IsNaN(cam.Up.X) || double.IsNaN(cam.Up.Y) || double.IsNaN(cam.Up.Z),
					Is.False, "up must not be NaN");
				Assert.That(cam.Up.Length, Is.GreaterThan(0.5), "up must be a real direction");
				Assert.That(Math.Abs(Vector3D.DotProduct(cam.Up, cam.Look)), Is.LessThan(1e-9));
			});
		}

		/// <summary>
		/// A degenerate normal must leave the view alone — a rejected joint may have no fitted plane,
		/// and the tab still shows its 3D view.
		/// </summary>
		[Test]
		public void AZeroNormalLeavesTheViewAlone()
		{
			var view = new Joint3DView();

			view.LookAtPlane(new Vector3D(0, 0, 0), new Vector3D(1, 0, 0));

			Assert.That(view.HomeCameraForTest, Is.Null,
				"no plane, no plane view — the oblique default must survive");
		}
	}
}
