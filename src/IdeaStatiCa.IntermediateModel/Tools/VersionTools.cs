namespace IdeaStatiCa.IntermediateModel.Tools
{
	public class VersionTools
	{
		public static Version GetVersion(string version)
		{
			if (int.TryParse(version, out int mainVersion))
			{
				return new Version(mainVersion, 0, 0);
			}
			return Version.Parse(version);
		}

		public static Version GetLatestVersion(ICollection<Version> version)
		{
			return version.Max();
		}
	}
}
