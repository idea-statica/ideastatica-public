using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Makes sure a REST API service this app started never outlives it.
	///
	/// Why it matters: a service we start holds an IDEA StatiCa LICENCE SEAT for as long as it
	/// runs, so a leaked one costs the user a seat until they find it in Task Manager. And the leak
	/// compounds — the SDK runner starts each service on a free port while a reuse is only detected
	/// on 5000, so the next launch does not find the orphan and starts another beside it.
	///
	/// Disposing the runner covers an orderly close. It does NOT cover a hard kill of this process,
	/// a crash, or a power loss: nothing in managed code runs then. A Job Object does, because
	/// WINDOWS does the killing — when the last handle to the job closes, which happens whatever
	/// takes the process down, every process in the job goes with it.
	///
	/// Port of the python reference's _create_kill_on_close_job / _assign_to_job. Its third
	/// mechanism, a PID file reaped at the next startup, is deliberately NOT ported: it only exists
	/// there to catch what the interpreter's atexit misses, and the job already covers that case
	/// here. Best-effort throughout — failing to set this up must never stop the app from running,
	/// so every failure is logged and swallowed.
	/// </summary>
	public sealed class ServiceReaper : IDisposable
	{
		private IntPtr _job = IntPtr.Zero;
		private readonly Action<string> _log;

		public ServiceReaper(Action<string> log)
		{
			_log = log;
			try
			{
				_job = CreateJobObject(IntPtr.Zero, null);
				if (_job == IntPtr.Zero)
				{
					_log("  service reaper: could not create a Job Object — a hard kill of this app "
						+ "would leave the service running and holding a licence seat");
					return;
				}

				var info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
				{
					BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
					{
						LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE,
					},
				};
				int size = Marshal.SizeOf<JOBOBJECT_EXTENDED_LIMIT_INFORMATION>();
				IntPtr buffer = Marshal.AllocHGlobal(size);
				try
				{
					Marshal.StructureToPtr(info, buffer, false);
					if (!SetInformationJobObject(_job, JobObjectExtendedLimitInformation, buffer, (uint)size))
						_log("  service reaper: the Job Object rejected KILL_ON_JOB_CLOSE — "
							+ "the service is only cleaned up on an orderly exit");
				}
				finally
				{
					Marshal.FreeHGlobal(buffer);
				}
			}
			catch (Exception ex)
			{
				_log($"  service reaper unavailable ({ex.Message}) — the service is only cleaned up "
					+ "on an orderly exit");
				_job = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Put a process under the job, so Windows kills it when this app's last job handle closes.
		///
		/// Returns false when it could not be done, and the caller should treat that as "the orderly
		/// path is the only cleanup" rather than as an error — the app still works, it just leaks a
		/// seat if it is killed outright.
		/// </summary>
		public bool Adopt(Process process)
		{
			if (_job == IntPtr.Zero) return false;
			try
			{
				if (AssignProcessToJobObject(_job, process.Handle)) return true;
				int err = Marshal.GetLastWin32Error();
				// 5 = ACCESS_DENIED: on older Windows a process already in another job cannot be
				// re-assigned. Nested jobs work from Windows 8 on, so this is unlikely but real.
				_log($"  service reaper: process {process.Id} could not be adopted (error {err}) — "
					+ "a hard kill of this app would leave it running");
				return false;
			}
			catch (Exception ex)
			{
				_log($"  service reaper: could not adopt process ({ex.Message})");
				return false;
			}
		}

		/// <summary>
		/// Find the service process the SDK runner started, so it can be adopted.
		///
		/// The runner keeps its Process private and exposes nothing, so the process is identified by
		/// image name and start time: the newest IdeaStatiCa.ConnectionRestApi.exe that began after
		/// the moment we asked for one. Matching on "newest" alone would be wrong — it could adopt a
		/// service the USER started, and killing that on exit would take away something we do not
		/// own.
		/// </summary>
		public static Process? FindServiceStartedAfter(DateTime moment)
		{
			try
			{
				Process? best = null;
				foreach (var p in Process.GetProcessesByName(
					System.IO.Path.GetFileNameWithoutExtension(ServiceLocator.ExeName)))
				{
					try
					{
						if (p.StartTime < moment) { p.Dispose(); continue; }
						if (best == null || p.StartTime > best.StartTime)
						{
							best?.Dispose();
							best = p;
						}
						else p.Dispose();
					}
					catch (Exception)
					{
						p.Dispose();   // exited between the listing and the read, or access denied
					}
				}
				return best;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public void Dispose()
		{
			if (_job == IntPtr.Zero) return;
			// Closing the handle is what kills the job's processes, so this IS the cleanup.
			CloseHandle(_job);
			_job = IntPtr.Zero;
		}

		// ── Win32 ──
		private const int JobObjectExtendedLimitInformation = 9;
		private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000;

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern IntPtr CreateJobObject(IntPtr attributes, string? name);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetInformationJobObject(IntPtr job, int infoClass,
			IntPtr info, uint infoLength);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr handle);

		[StructLayout(LayoutKind.Sequential)]
		private struct JOBOBJECT_BASIC_LIMIT_INFORMATION
		{
			public long PerProcessUserTimeLimit;
			public long PerJobUserTimeLimit;
			public uint LimitFlags;
			public UIntPtr MinimumWorkingSetSize;
			public UIntPtr MaximumWorkingSetSize;
			public uint ActiveProcessLimit;
			public UIntPtr Affinity;
			public uint PriorityClass;
			public uint SchedulingClass;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct IO_COUNTERS
		{
			public ulong ReadOperationCount, WriteOperationCount, OtherOperationCount;
			public ulong ReadTransferCount, WriteTransferCount, OtherTransferCount;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
		{
			public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
			public IO_COUNTERS IoInfo;
			public UIntPtr ProcessMemoryLimit;
			public UIntPtr JobMemoryLimit;
			public UIntPtr PeakProcessMemoryUsed;
			public UIntPtr PeakJobMemoryUsed;
		}
	}
}
