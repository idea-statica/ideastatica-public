﻿using System;
using System.Runtime.InteropServices;

namespace IdeaStatiCa.BimApiLink.Utils
{
	public static class WindowVisibilityHelper
	{
		// When you don't want the ProcessId, use this overload and pass
		// IntPtr.Zero for the second parameter
		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

		[DllImport("kernel32.dll")]
		public static extern uint GetCurrentThreadId();

		// The GetForegroundWindow function returns a handle to the foreground window.
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern bool AttachThreadInput(uint idAttach,
			uint idAttachTo, bool fAttach);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool BringWindowToTop(HandleRef hWnd);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsZoomed(IntPtr hWnd);

		public static void ForceForegroundWindow(IntPtr hWnd)
		{
			uint foreThread = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
			uint appThread = GetCurrentThreadId();
			uint SW_SHOW = 5;
			if (IsZoomed(hWnd))
			{
				//SW_SHOWMAXIMIZED = 3
				SW_SHOW = 3;
			}

			if (foreThread != appThread)
			{
				AttachThreadInput(foreThread, appThread, true);
				BringWindowToTop(hWnd);
				ShowWindow(hWnd, SW_SHOW);
				AttachThreadInput(foreThread, appThread, false);
			}
			else
			{
				BringWindowToTop(hWnd);
				ShowWindow(hWnd, SW_SHOW);
			}
		}
	}
}
