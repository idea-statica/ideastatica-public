using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// Makes sure a service process started by <see cref="ConnectionApiServiceRunner"/> never outlives
	/// the process that started it.
	///
	/// Why it matters: a running Connection REST API service HOLDS AN IDEA StatiCa LICENCE SEAT. A
	/// service that is left behind costs the user that seat until someone finds the process and ends it,
	/// and the loss compounds - the runner takes a free port for every service it starts, so the next
	/// run does not notice the orphan and starts another beside it.
	///
	/// <see cref="ConnectionApiServiceRunner.Dispose"/> covers an orderly exit. It does NOT cover a hard
	/// kill of the calling process, a crash, or a power loss: no managed code runs then. A Windows Job
	/// Object does, because WINDOWS does the killing - with JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE every
	/// process in the job is terminated when the last handle to the job closes, and that happens however
	/// the owning process goes away.
	///
	/// Best-effort throughout: this is a safety net, and failing to set it up must never stop a service
	/// from being used. Every failure is reported through the runner's log callback and swallowed, and
	/// <see cref="IsActive"/> says whether the guarantee actually holds.
	/// </summary>
	internal sealed class ServiceJobObject : IDisposable
	{
		private readonly Action<string> _log;
		private IntPtr _job = IntPtr.Zero;

		internal ServiceJobObject(Action<string> log)
		{
			_log = log ?? (_ => { });

			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				// The service is a Windows executable, so this is not a case that arises today - but the
				// package targets netstandard and the P/Invoke below would throw on anything else.
				return;
			}

			try
			{
				_job = CreateJobObject(IntPtr.Zero, null);
				if (_job == IntPtr.Zero)
				{
					_log("The Connection API service could not be put under a Job Object: a hard kill of"
						+ " this process would leave the service running and holding a licence seat.");
					return;
				}

				var info = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
				{
					BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
					{
						LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE,
					},
				};

				int size = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
				IntPtr buffer = Marshal.AllocHGlobal(size);
				try
				{
					Marshal.StructureToPtr(info, buffer, false);
					if (!SetInformationJobObject(_job, JobObjectExtendedLimitInformation, buffer, (uint)size))
					{
						_log("The Job Object rejected KILL_ON_JOB_CLOSE: the Connection API service is"
							+ " only shut down on an orderly exit.");
						Close();
					}
				}
				finally
				{
					Marshal.FreeHGlobal(buffer);
				}
			}
			catch (Exception e)
			{
				_log($"The Connection API service could not be put under a Job Object ({e.Message}):"
					+ " it is only shut down on an orderly exit.");
				Close();
			}
		}

		/// <summary>
		/// Whether the job exists, i.e. whether adopted processes are actually guaranteed to be taken
		/// down with this one.
		/// </summary>
		internal bool IsActive => _job != IntPtr.Zero;

		/// <summary>
		/// Put a process under the job. False means the guarantee does not hold for it and the orderly
		/// path is the only cleanup - the caller should carry on, not fail.
		/// </summary>
		internal bool Adopt(Process process)
		{
			if (_job == IntPtr.Zero || process == null)
			{
				return false;
			}

			try
			{
				if (AssignProcessToJobObject(_job, process.Handle))
				{
					return true;
				}

				// 5 = ACCESS_DENIED, which is what a process already assigned to another job returns on
				// Windows 7. Nested jobs work from Windows 8 on, so this is unlikely but real.
				_log($"The Connection API service (pid {process.Id}) could not be assigned to the Job"
					+ $" Object (Win32 error {Marshal.GetLastWin32Error()}): a hard kill of this process"
					+ " would leave it running.");
			}
			catch (Exception e)
			{
				_log($"The Connection API service could not be assigned to the Job Object ({e.Message}).");
			}

			return false;
		}

		/// <summary>
		/// Closing the handle is what terminates the job's processes, so this IS the cleanup - not a
		/// release of something already cleaned up.
		/// </summary>
		public void Dispose() => Close();

		private void Close()
		{
			if (_job == IntPtr.Zero)
			{
				return;
			}

			IntPtr job = _job;
			_job = IntPtr.Zero;
			CloseHandle(job);
		}

		private const int JobObjectExtendedLimitInformation = 9;
		private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000;

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern IntPtr CreateJobObject(IntPtr attributes, string name);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetInformationJobObject(IntPtr job, int infoClass, IntPtr info,
			uint infoLength);

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
