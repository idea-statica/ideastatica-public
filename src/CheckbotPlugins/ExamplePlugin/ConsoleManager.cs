using System.Runtime.InteropServices;

namespace ExamplePlugin
{
	internal static class Kernel32Native
	{
		public const int AttachParentProcess = -1;

		[DllImport("kernel32.dll")]
		public static extern bool AttachConsole(int dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();
	}

	internal class ConsoleManager : IDisposable
	{
		private bool _consoleAllocated;
		private bool _disposed;

		public bool ConsoleShown => _consoleAllocated;

		public ConsoleManager(bool showConsole)
		{
			if (showConsole)
			{
				_consoleAllocated = Kernel32Native.AllocConsole();
			}
			else
			{
				_consoleAllocated = Kernel32Native.AttachConsole(Kernel32Native.AttachParentProcess);
			}
		}

		~ConsoleManager()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (_disposed)
			{
				if (disposing)
				{
					throw new ObjectDisposedException(nameof(ConsoleManager));
				}

				return;
			}

			_disposed = true;

			if (_consoleAllocated)
			{
				Kernel32Native.FreeConsole();
				_consoleAllocated = false;
			}
		}
	}
}