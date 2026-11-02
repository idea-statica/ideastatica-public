using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Best-effort helper to bring another process's main window to the foreground - used to focus an already-running
	/// Checkbot instance instead of opening the same project twice. Foregrounding is inherently best-effort on Windows
	/// (SetForegroundWindow can be refused), so callers must tolerate a <c>false</c> result.
	/// </summary>
	public static class WindowActivation
	{
		private const int SW_RESTORE = 9;

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		private static extern bool IsIconic(IntPtr hWnd);

		/// <summary>
		/// Restores (if minimised) and foregrounds the main window of the process with the given id.
		/// Returns <c>true</c> if a window was found and the foreground call succeeded.
		/// </summary>
		public static bool TryBringProcessToFront(int processId)
		{
			try
			{
				using (Process process = Process.GetProcessById(processId))
				{
					IntPtr hWnd = process.MainWindowHandle;
					if (hWnd == IntPtr.Zero)
					{
						return false;
					}

					if (IsIconic(hWnd))
					{
						ShowWindow(hWnd, SW_RESTORE);
					}

					return SetForegroundWindow(hWnd);
				}
			}
			catch
			{
				return false;
			}
		}
	}
}
