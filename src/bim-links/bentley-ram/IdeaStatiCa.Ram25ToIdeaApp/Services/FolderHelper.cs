using IdeaStatiCa.Plugin;
using System.IO;

namespace IdeaStatiCa.RamToIdeaApp.Services
{
	internal static class FolderHelper
	{
		public static string FindNetFolder(IPluginLogger ideaLogger, string rootFolder, string file)
		{
			ideaLogger.LogDebug($"FindNetFolder : rootFolder : '{rootFolder}' file : '{file}'");

			// posible locations of the file in case of the setup

			#region usecases for setup
			//file is in the root folder
			var foundLocation = Path.Combine(rootFolder, file);
			if (File.Exists(foundLocation))
			{
				ideaLogger.LogDebug($"FindNetFolder : found in '{foundLocation}'");
				return foundLocation;
			}

			// file is in parent folder, executing assembly is in net48
			var parentFolder = Directory.GetParent(rootFolder).FullName;
			foundLocation = Path.Combine(parentFolder, file);

			if (File.Exists(foundLocation))
			{
				ideaLogger.LogDebug($"FindNetFolder : found in '{foundLocation}'");
				return foundLocation;
			}

			//is the file is in net48 subdir ?
			foundLocation = Path.Combine(rootFolder, "net48", file);
			if (File.Exists(foundLocation))
			{
				ideaLogger.LogDebug($"FindNetFolder : found in '{foundLocation}'");
				return foundLocation;
			}

			//is the file is in net8.0-windows subdir ?
			foundLocation = Path.Combine(rootFolder, "net8.0-windows", file);
			if (File.Exists(foundLocation))
			{
				ideaLogger.LogDebug($"FindNetFolder : found in '{foundLocation}'");
				return foundLocation;
			}
			#endregion

			#region usecases for development
			DirectoryInfo directoryInfo = new DirectoryInfo(rootFolder);

			if (directoryInfo != null && directoryInfo.Parent != null && directoryInfo.Parent.FullName != null)
			{
				var rootFolderDeve = directoryInfo.Parent.FullName;

				//file is in the root folder
				foundLocation = Path.Combine(rootFolderDeve, file);
				if (File.Exists(foundLocation))
				{
					ideaLogger.LogDebug($"FindNetFolder : found in '{foundLocation}'");
					return foundLocation;
				}

				// is file in root and executing assembly is in net48 ?
				if (File.Exists(foundLocation))
				{
					ideaLogger.LogDebug($"FindNetFolder : found in '{foundLocation}'");
					return foundLocation;
				}

				//is the file is in net48 subdir ?
				foundLocation = Path.Combine(rootFolderDeve, "net48", file);
				if (File.Exists(foundLocation))
				{
					ideaLogger.LogDebug($"FindNetFolder : found in '{foundLocation}'");
					return foundLocation;
				}

				//is the file is in net8.0-windows subdir ?
				foundLocation = Path.Combine(rootFolderDeve, "net8.0-windows", file);
				if (File.Exists(foundLocation))
				{
					ideaLogger.LogDebug($"FindNetFolder : found in '{foundLocation}'");
					return foundLocation;
				}


			}
			#endregion

			// file not found
			ideaLogger.LogDebug($"FindNetFolder : WARNING NOT FOUND rootFolder  : '{rootFolder}' file : '{file}'");
			throw new FileNotFoundException($"FindNetFolder : file not found {Path.Combine(rootFolder, file)} ");
		}
	}
}
