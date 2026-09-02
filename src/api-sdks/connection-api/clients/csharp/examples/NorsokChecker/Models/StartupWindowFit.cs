using System.Windows;

namespace NorsokChecker.Models
{
	/// <summary>
	/// Fits the window's WANTED start-up size into the screen it will actually open on.
	///
	/// Reported from a laptop: the app started with its title bar above the top edge of the screen,
	/// so the window could not be moved, maximised or minimised — the only way out was Task Manager.
	///
	/// The cause is arithmetic, not a bug in WPF. The window asks for 1780 × 1040 and
	/// WindowStartupLocation="CenterScreen", so WPF places it at <c>Top = (screenHeight − 1040) / 2</c>.
	/// On a 1080 px display that is NEGATIVE, and a negative Top puts the title bar off-screen. The
	/// same happens horizontally on anything narrower than 1780.
	///
	/// The wanted size is kept as a WISH rather than replaced by something smaller: 1780 × 1040 was
	/// measured against the content — seven members listed without a scrollbar, six braces in the
	/// §6.4 table, the envelope caption unwrapped — so shrinking it by default would undo exactly
	/// that fitting. This gives up only as much as the screen forces.
	///
	/// Deliberately NOT maximising when the window does not fit. Maximising is the user's decision,
	/// and an app that does it silently leaves them unable to tell that the window was too big — it
	/// just looks odd once. A smaller window with a reachable title bar says what happened.
	///
	/// A plain function over a rectangle, so all of it is testable: the work area can be handed in,
	/// which is the only way to check the laptop case from a machine that does not have that screen.
	/// </summary>
	internal static class StartupWindowFit
	{
		/// <summary>
		/// The size and position to open at, given what the window wants and what the screen offers.
		///
		/// <paramref name="workArea"/> is the screen area excluding the taskbar — in WPF,
		/// <c>SystemParameters.WorkArea</c>. Note that it describes the PRIMARY screen only
		/// (SPI_GETWORKAREA); that is correct here because CenterScreen centres on the primary screen
		/// too, so the two agree about which screen is meant. A window opened on a secondary monitor
		/// is a different problem and this does not claim to solve it.
		/// </summary>
		internal static Rect Fit(Size wanted, Rect workArea)
		{
			// Never larger than the screen can show. Min, not a fixed fallback: a 1600-px-wide screen
			// keeps 1600, it does not drop to some arbitrary "safe" width.
			double w = Math.Min(wanted.Width, workArea.Width);
			double h = Math.Min(wanted.Height, workArea.Height);

			// Centred, then pushed back inside. Centring alone is what produced the defect: WPF
			// centres on the SCREEN while the taskbar eats part of it, so a window as tall as the
			// screen still lands with its top above the work area.
			double left = workArea.Left + (workArea.Width - w) / 2.0;
			double top = workArea.Top + (workArea.Height - h) / 2.0;

			// Clamp for the degenerate case only — with w ≤ workArea.Width the centring above cannot
			// go negative, but a work area reported as empty or inverted (it has been seen during a
			// display change) would. Cheaper than a special case for it.
			left = Math.Max(workArea.Left, left);
			top = Math.Max(workArea.Top, top);

			return new Rect(left, top, w, h);
		}
	}
}
