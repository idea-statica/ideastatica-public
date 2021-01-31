using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CI.Common
{
	/// <summary>
	/// OS helper
	/// </summary>
	public static class OSHelper
	{
		private static int? dpi;
		private static object _dpiLock = new object();

		/// <summary>
		/// static contructor
		/// </summary>
		static OSHelper()
		{
			DisableWeakEventListening = true;
		}

		/// <summary>
		/// Returns is the process is 64bit
		/// </summary>
		/// <returns>true if the 64bit process</returns>
		public static bool IsProcess64()
		{
			const int size64 = 8;
			return (IntPtr.Size == size64);
		}

		/// <summary>
		/// Indicates, whether Weak event listener will be registered.
		/// </summary>
		public static bool DisableWeakEventListening { get; set; }

		/// <summary>
		/// Calculates hash code of the string which is not platform dependent
		/// </summary>
		/// <param name="value">input string</param>
		/// <returns>Hash code of the <paramref name="value"/>.</returns>
		public static Int32 GetHashCodeOfString(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}

			unchecked
			{
				Int32 hash = 23;
				foreach (char c in value)
				{
					hash = (Int32)(hash * 31 + c);
				}

				return hash;
			}
		}

		/// <summary>
		/// Gets the Pixels Per Density Independent Pixel.
		/// </summary>
		public static float PixelsPerDip
		{
			get
			{
				return Dpi / 96f;
			}
		}

		/// <summary>
		/// Gets the DPI.
		/// </summary>
		public static int Dpi
		{
			get
			{
				if (!dpi.HasValue)
				{
					object dpiLock = _dpiLock;
					lock (dpiLock)
					{
						if (!dpi.HasValue)
						{
							HandleRef hWnd = new HandleRef(null, IntPtr.Zero);
							IntPtr dC = IntGetDC(hWnd);
							if (dC == IntPtr.Zero)
							{
								//throw new Win32Exception();
								dpi = 96;
								return dpi.Value;
							}

							try
							{
								dpi = GetDeviceCaps(new HandleRef(null, dC), 90);
							}
							finally
							{
								IntReleaseDC(hWnd, new HandleRef(null, dC));
							}
						}
					}
				}

				return dpi.Value;
			}
		}

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int GetDeviceCaps(HandleRef hDC, int nIndex);

		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetDC", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr IntGetDC(HandleRef hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "ReleaseDC", ExactSpelling = true)]
		private static extern int IntReleaseDC(HandleRef hWnd, HandleRef hDC);
	}
}
