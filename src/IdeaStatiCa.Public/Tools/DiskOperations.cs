using System;
using System.IO;
using System.Threading;

namespace IdeaStatiCa.Public.Tools
{
	public static class DiskOperations
	{
		/// <summary>
		/// Delete recursively directory
		/// https://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
		/// </summary>
		/// <param name="target_dir">Directory to delete</param>
		/// <exception cref="System.IO.IOException">Exception is throws if any disk operation fails</exception>
		public static void DeleteDirectory(string target_dir)
		{
			if (!Directory.Exists(target_dir))
			{
				return;
			}

			string[] files = Directory.GetFiles(target_dir);
			string[] dirs = Directory.GetDirectories(target_dir);

			File.SetAttributes(target_dir, FileAttributes.Normal); //If folder for some magic reasons was read only

			foreach (string file in files)
			{
				DeleteFile(file);
			}

			foreach (string dir in dirs)
			{
				DeleteDirectory(dir);
			}
			try
			{
				Directory.Delete(target_dir, false);
			}
			catch (IOException)
			{
				Thread.Sleep(0); //The RemoveDirectory function marks a directory for deletion on close. Therefore, the directory is not removed until the last handle to the directory is closed.
				Directory.Delete(target_dir, true);
			}
			catch (UnauthorizedAccessException)
			{
				Thread.Sleep(0); //The RemoveDirectory function marks a directory for deletion on close. Therefore, the directory is not removed until the last handle to the directory is closed.
				Directory.Delete(target_dir, true);
			}

		}

		/// <summary>
		/// Delete file
		/// </summary>
		/// <param name="fileName">The filename of file to delete</param>
		/// <exception cref="System.IO.IOException">Exception is throws if any disk operation fails</exception>
		public static void DeleteFile(string fileName)
		{
			File.SetAttributes(fileName, FileAttributes.Normal); //If file for some magic reasons was read only
			File.Delete(fileName);
		}

		/// <summary>
		/// Copy all directory <paramref name="sourceDirName"/> to <paramref name="destDirName"/>
		/// If parameter <paramref name="copySubDirs"/> is true all subdirectories and their content is copied as well
		/// </summary>
		/// <param name="sourceDirName">The source directory</param>
		/// <param name="destDirName">The destination directory</param>
		/// <param name="copySubDirs">True if subdirectories and their contents should be copied as well</param>
		/// <exception cref="System.IO.IOException">Exception is throws if any disk operation fails</exception>
		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
						"Source directory does not exist or could not be found: "
						+ sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, true);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
	}
}
