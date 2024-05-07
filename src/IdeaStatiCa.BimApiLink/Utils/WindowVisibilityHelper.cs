using System;
using System.Runtime.InteropServices;

namespace IdeaStatiCa.BimApiLink.Utils
{
	public static class WindowVisibilityHelper
	{
		[DllImport("user32.dll")]
		public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr WindowHandle);
		public const int SW_RESTORE = 9;


		public static void ForceForegroundWindow(IntPtr hWnd)
		{
			ShowWindowAsync(new HandleRef(null, hWnd), SW_RESTORE);
			SetForegroundWindow(hWnd);
		}
	}
}
