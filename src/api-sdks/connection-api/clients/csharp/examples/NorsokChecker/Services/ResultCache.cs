using System.IO;

namespace NorsokChecker.Services
{
	/// <summary>
	/// Caches raw CBFEM JSON results to disk so repeated runs don't require recalculation.
	/// Cache file is stored next to the .ideaCon project file.
	/// </summary>
	public static class ResultCache
	{
		private const string CacheSuffix = "_rawresults.json";

		public static string GetCachePath(string projectFilePath)
		{
			var dir = Path.GetDirectoryName(projectFilePath) ?? ".";
			var name = Path.GetFileNameWithoutExtension(projectFilePath);
			return Path.Combine(dir, name + CacheSuffix);
		}

		public static bool Exists(string projectFilePath)
		{
			return File.Exists(GetCachePath(projectFilePath));
		}

		public static string Load(string projectFilePath)
		{
			return File.ReadAllText(GetCachePath(projectFilePath));
		}

		public static void Save(string projectFilePath, string rawJsonResults)
		{
			File.WriteAllText(GetCachePath(projectFilePath), rawJsonResults);
		}

		public static void Delete(string projectFilePath)
		{
			var path = GetCachePath(projectFilePath);
			if (File.Exists(path))
				File.Delete(path);
		}
	}
}
