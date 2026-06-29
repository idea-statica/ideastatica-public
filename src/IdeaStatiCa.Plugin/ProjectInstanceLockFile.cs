using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Shared definition of the per-project, machine-local, per-user lock file that ensures a single Checkbot instance
	/// owns a project directory. Both the Checkbot application (which holds the lock for its lifetime) and the BIM
	/// plugin launcher (which probes it before spawning Checkbot) derive the identical lock-file path and owner format
	/// from here, so the two sides can never disagree.
	/// </summary>
	/// <remarks>
	/// The lock is a Win32 byte-range lock (<see cref="FileStream.Lock(long, long)"/>) on a file under the user's
	/// LocalApplicationData. Unlike a named mutex it has no thread affinity and is released by the OS on process death.
	/// The scope is machine-local and per Windows user; it does not coordinate across machines (network share / cloud
	/// folder opened from two PCs) - that is intentionally out of scope.
	/// </remarks>
	public static class ProjectInstanceLockFile
	{
		private const string TempDirName = "IDEA_RS";
		private const string LockDirName = "checkbot-locks";

		// The mutual-exclusion byte; the owner process id is stored beyond it so it stays readable by other processes.
		private const long LockOffset = 0;
		private const long LockLength = 1;
		private const long OwnerInfoOffset = 8;
		private const int OwnerInfoLength = 4;

		/// <summary>
		/// The absolute path of the lock file for <paramref name="projectDir"/>. Identity is the canonical (absolute,
		/// case-insensitive, trailing-separator-trimmed) project path, hashed because it contains path separators and
		/// exceeds file-name limits.
		/// </summary>
		public static string GetLockFilePath(string projectDir)
		{
			if (string.IsNullOrEmpty(projectDir))
			{
				throw new ArgumentException("Project directory must be provided.", nameof(projectDir));
			}

			string key = Path.GetFullPath(projectDir)
				.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
				.ToUpperInvariant();

			string hash;
			using (SHA256 sha = SHA256.Create())
			{
				byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(key));

				// 128 bits is plenty to avoid collisions; a real hash (never String.GetHashCode) so two different
				// projects can never collide onto the same lock.
				StringBuilder sb = new StringBuilder(32);
				for (int i = 0; i < 16; i++)
				{
					sb.Append(bytes[i].ToString("x2"));
				}
				hash = sb.ToString();
			}

			string dir = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				TempDirName,
				LockDirName);

			return Path.Combine(dir, hash + ".lock");
		}

		/// <summary>
		/// Opens the project's lock file and tries to take the exclusive lock, recording the current process as owner.
		/// Returns the held stream on success (the caller keeps it open for as long as it owns the project), or
		/// <c>null</c> if another process already owns it.
		/// </summary>
		public static FileStream TryAcquire(string projectDir)
		{
			string path = GetLockFilePath(projectDir);
			Directory.CreateDirectory(Path.GetDirectoryName(path));

			FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
			try
			{
				stream.Lock(LockOffset, LockLength);
			}
			catch (IOException)
			{
				// already owned by another process
				stream.Dispose();
				return null;
			}
			catch
			{
				stream.Dispose();
				throw;
			}

			try
			{
				WriteOwnerProcessId(stream);
			}
			catch
			{
				// owner info is a best-effort diagnostic / focus aid - never fail the acquisition because of it
			}

			return stream;
		}

		/// <summary>Releases a lock previously returned by <see cref="TryAcquire"/>.</summary>
		public static void Release(FileStream stream)
		{
			if (stream == null)
			{
				return;
			}

			try
			{
				stream.Unlock(LockOffset, LockLength);
			}
			catch
			{
				// already released / not locked
			}

			stream.Dispose();
		}

		/// <summary>
		/// Best-effort check (used by the BIM launcher before spawning) of whether another process currently owns the
		/// project. Does NOT keep the lock. Returns <c>false</c> on any error so a probe failure never blocks launching.
		/// </summary>
		public static bool IsHeldByAnotherProcess(string projectDir)
		{
			try
			{
				string path = GetLockFilePath(projectDir);
				if (!File.Exists(path))
				{
					return false;
				}

				using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
				{
					try
					{
						stream.Lock(LockOffset, LockLength);
					}
					catch (IOException)
					{
						return true;
					}

					stream.Unlock(LockOffset, LockLength);
					return false;
				}
			}
			catch
			{
				return false;
			}
		}

		/// <summary>The process id of the current owner of <paramref name="projectDir"/>, or <c>null</c> if unknown.</summary>
		public static int? TryGetOwnerProcessId(string projectDir)
		{
			try
			{
				string path = GetLockFilePath(projectDir);
				if (!File.Exists(path))
				{
					return null;
				}

				using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					if (stream.Length < OwnerInfoOffset + OwnerInfoLength)
					{
						return null;
					}

					stream.Seek(OwnerInfoOffset, SeekOrigin.Begin);
					byte[] buffer = new byte[OwnerInfoLength];
					int read = stream.Read(buffer, 0, OwnerInfoLength);
					if (read < OwnerInfoLength)
					{
						return null;
					}

					int pid = BitConverter.ToInt32(buffer, 0);
					return pid > 0 ? pid : (int?)null;
				}
			}
			catch
			{
				return null;
			}
		}

		private static void WriteOwnerProcessId(FileStream stream)
		{
			int pid;
			using (Process current = Process.GetCurrentProcess())
			{
				pid = current.Id;
			}

			stream.Seek(OwnerInfoOffset, SeekOrigin.Begin);
			stream.Write(BitConverter.GetBytes(pid), 0, OwnerInfoLength);
			stream.Flush();
		}
	}
}
