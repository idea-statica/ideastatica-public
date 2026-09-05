using System.Windows;
using NorsokChecker.Models;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The window opens inside the screen, with its title bar reachable.
	///
	/// Reported from a laptop: the app started with the title bar above the top edge, so the window
	/// could not be moved, maximised or minimised — Task Manager was the only way out. The cause is
	/// arithmetic: the XAML asks for 1780 × 1040 with CenterScreen, and WPF places it at
	/// Top = (screenHeight − 1040) / 2, which is NEGATIVE below 1080 px.
	///
	/// Every case is asserted against a work area handed IN, which is the point of testing the
	/// function rather than the window: this machine is 1920 × 1200 (measured), so the window fits
	/// and the defect cannot be reproduced here at all. A test that read the real screen would pass
	/// on this machine whatever the code did.
	/// </summary>
	[TestFixture]
	public class StartupWindowFitTests
	{
		/// <summary>The size the XAML asks for — measured against the content, not chosen freely.</summary>
		private static readonly Size Wanted = new(1780, 1040);

		/// <summary>A work area with no taskbar reserved, for arithmetic that does not need one.</summary>
		private static Rect Screen(double w, double h) => new(0, 0, w, h);

		/// <summary>
		/// THE reported case: a laptop too small for the wanted size. The title bar must be on screen.
		///
		/// 1366 × 768 with a 40 px taskbar — a common laptop, and narrower AND shorter than wanted,
		/// so both dimensions have to give.
		/// </summary>
		[Test]
		public void OnALaptopTooSmallForTheWantedSizeTheTitleBarStaysOnScreen()
		{
			var work = new Rect(0, 0, 1366, 728);   // 768 less a 40 px taskbar

			var r = StartupWindowFit.Fit(Wanted, work);

			Assert.Multiple(() =>
			{
				Assert.That(r.Top, Is.GreaterThanOrEqualTo(work.Top),
					"the title bar must not be above the top edge — this is the defect");
				Assert.That(r.Left, Is.GreaterThanOrEqualTo(work.Left), "nor off the left edge");

				// And the whole window inside, or the buttons at the right are unreachable even with
				// the title bar visible.
				Assert.That(r.Right, Is.LessThanOrEqualTo(work.Right), "the minimise/close buttons are reachable");
				Assert.That(r.Bottom, Is.LessThanOrEqualTo(work.Bottom));
			});
		}

		/// <summary>
		/// Where it fits, the wanted size is KEPT — the whole point of clamping rather than replacing.
		///
		/// 1780 × 1040 was measured against the content (seven members without a scrollbar, six
		/// braces in the §6.4 table, the envelope caption unwrapped). A fix that shrank the window
		/// on every machine would undo that fitting while making this suite green.
		/// </summary>
		[Test]
		public void OnALargeScreenTheWantedSizeIsUnchanged()
		{
			var work = new Rect(0, 0, 1920, 1152);   // this machine, measured

			var r = StartupWindowFit.Fit(Wanted, work);

			Assert.Multiple(() =>
			{
				Assert.That(r.Width, Is.EqualTo(1780), "the measured width survives");
				Assert.That(r.Height, Is.EqualTo(1040), "and the height");
				Assert.That(r.Left, Is.EqualTo(70), "centred: (1920 − 1780) / 2");
				Assert.That(r.Top, Is.EqualTo(56), "centred in the WORK area: (1152 − 1040) / 2");
			});
		}

		/// <summary>
		/// Only the dimension that does not fit gives way.
		///
		/// Two separate cases because a fix that clamped both together — say, scaling to preserve the
		/// aspect ratio — would satisfy "it fits" while throwing away width the screen could show.
		/// </summary>
		[TestCase(1600, 1200, 1600, 1040, TestName = "too narrow: only the width gives")]
		[TestCase(1920, 900, 1780, 900, TestName = "too short: only the height gives")]
		public void OnlyTheDimensionThatDoesNotFitIsReduced(
			double screenW, double screenH, double expectW, double expectH)
		{
			var r = StartupWindowFit.Fit(Wanted, Screen(screenW, screenH));

			Assert.Multiple(() =>
			{
				Assert.That(r.Width, Is.EqualTo(expectW));
				Assert.That(r.Height, Is.EqualTo(expectH));
			});
		}

		/// <summary>
		/// The work area's own offset is respected — a taskbar at the TOP or on the LEFT moves the
		/// origin, and a window placed at 0,0 would sit under it.
		///
		/// Windows allows the taskbar on any edge, and the reported symptom (an unreachable title
		/// bar) is exactly what a top taskbar produces even on a screen large enough.
		/// </summary>
		[Test]
		public void AnOffsetWorkAreaIsHonoured()
		{
			// A 48 px taskbar along the TOP: the work area starts at y = 48.
			var work = new Rect(0, 48, 1920, 1104);

			var r = StartupWindowFit.Fit(Wanted, work);

			Assert.Multiple(() =>
			{
				Assert.That(r.Top, Is.GreaterThanOrEqualTo(48),
					"below the taskbar, not at the screen's own y = 0");
				Assert.That(r.Bottom, Is.LessThanOrEqualTo(work.Bottom));
			});
		}

		/// <summary>
		/// A screen exactly the wanted size: the window fills it, flush at the origin.
		///
		/// The boundary between "fits" and "does not", where an off-by-one in the comparison would
		/// show up as a window one pixel off the edge.
		/// </summary>
		[Test]
		public void AScreenExactlyTheWantedSizeIsFilledExactly()
		{
			var r = StartupWindowFit.Fit(Wanted, Screen(1780, 1040));

			Assert.Multiple(() =>
			{
				Assert.That(r.Width, Is.EqualTo(1780));
				Assert.That(r.Height, Is.EqualTo(1040));
				Assert.That(r.Left, Is.EqualTo(0));
				Assert.That(r.Top, Is.EqualTo(0));
			});
		}

		/// <summary>
		/// The window ACTUALLY USES the fit — WPF's own centring is turned off.
		///
		/// Without WindowStartupLocation.Manual, WPF recomputes the position from the SCREEN
		/// (taskbar included) when the window is shown, silently undoing everything above. Measured:
		/// removing that one line left all 346 other tests green, which is how a fix like this ships
		/// looking complete and changes nothing.
		///
		/// On the SOURCE, because the alternative is showing a real window and reading its Top on a
		/// machine whose screen does not reproduce the defect.
		/// </summary>
		[Test]
		public void TheWindowTurnsOffWpfsOwnCentring()
		{
			var dir = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().Location)!);
			while (dir != null && !System.IO.Directory.Exists(System.IO.Path.Combine(dir.FullName, "NorsokChecker")))
				dir = dir.Parent;
			if (dir == null) Assert.Fail("cannot locate the NorsokChecker source from the test output — this test "
				+ "reads the source, and skipping it would report a pass it did not earn");

			// Comments stripped: the prose in FitToScreen names CenterScreen and Manual while
			// explaining the defect, so a raw match would find the explanation.
			string code = System.Text.RegularExpressions.Regex.Replace(
				System.IO.File.ReadAllText(System.IO.Path.Combine(
					dir!.FullName, "NorsokChecker", "MainWindow.xaml.cs")),
				@"//[^\n]*", "");

			Assert.Multiple(() =>
			{
				Assert.That(code, Does.Contain("WindowStartupLocation = WindowStartupLocation.Manual"),
					"WPF must stop centring, or it re-places the window on the SCREEN and the fit is lost");
				Assert.That(code, Does.Contain("StartupWindowFit.Fit("),
					"and the position comes from the tested function, not from a copy of its rules");
				foreach (string p in new[] { "Width =", "Height =", "Left =", "Top =" })
					Assert.That(code, Does.Contain($"{p} fitted."), $"{p} comes from the fit");
			});
		}

		/// <summary>
		/// The window does not maximise itself when it does not fit.
		///
		/// Deliberate, and worth pinning because "just maximise it" is the tempting fix: maximising
		/// is the user's decision, and an app that does it silently leaves them unable to tell the
		/// window was too big — it only ever looks odd. A smaller window with a reachable title bar
		/// says what happened. This function returns a rectangle and no window state, which is how
		/// that decision stays out of it.
		/// </summary>
		[Test]
		public void TheFitNeverAsksForMaximised()
		{
			var r = StartupWindowFit.Fit(Wanted, new Rect(0, 0, 1024, 600));

			Assert.Multiple(() =>
			{
				Assert.That(r.Width, Is.EqualTo(1024), "as wide as the screen, not maximised state");
				Assert.That(r.Height, Is.EqualTo(600));
				Assert.That(r.Width, Is.LessThan(Wanted.Width), "it did have to give way");
			});
		}
	}
}
