
using System;

namespace IdeaRS
{
	/// <summary>
	/// Represents definition of constants which are used accross all Idea applications
	/// </summary>
	public static class Constants
	{
		public static readonly string AppLogger = "AppLogger";
		public static readonly string WsAppLogger = "WsAppLogger";
		public static readonly string SecretEthParam = "/eth";
		public static readonly string CustomCommandParam = "-cmd:";
		public static readonly string SharedRepositoryFilename = @"libraryRepo.ideaLib";
		public static readonly string LibraryEntityDir = @"Library";
		public static readonly string ProjectTempDirName = @"Temp";
		public static readonly string SharedReposParam = @"-libReposPath:";
		public static readonly string ProjectDirectoryParam = @"-projectDirPath:";
		public static readonly string DesignCodeParam = @"-designCode:";
		public static readonly string IdeaRsTempDirName = "IDEA_RS";
		public static readonly string WsTempDirName = "TempWS";
		public static readonly string SettingsTemplateFolder = "IdeaSharedSettings_Ver20";
		public static readonly string CCM_ProjectSettingsDir = "ApplicationSettings";
	}

	[Flags]
	public enum AppUserModeType
	{
		None = 0,
		DeveMode = 16,
		CustomUI = 32,
		NoRecentFiles = 64,
		StructureMode = 128,
		SaveBitmap = 256,
	}
}
