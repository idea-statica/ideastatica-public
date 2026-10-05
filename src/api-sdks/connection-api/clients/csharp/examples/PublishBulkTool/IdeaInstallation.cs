using System.IO;

namespace PublishBulkTool
{
	/// <summary>
	/// Finds an IDEA StatiCa installation that carries the Connection REST API service.
	/// </summary>
	/// <remarks>
	/// The tool used to open with a hard-coded path to one particular product version. Anybody running
	/// a different version got that wrong path handed to the service runner, and the failure surfaced
	/// far away from its cause - which is half of what work item 37164 reports. Discovery keeps the
	/// default honest, and the user can still override it.
	/// </remarks>
	internal static class IdeaInstallation
	{
		internal const string ServiceExecutableName = "IdeaStatiCa.ConnectionRestApi.exe";

		/// <summary>
		/// Directory of the newest installation that contains the service executable, or <c>null</c>
		/// when none was found. The directory holding this tool wins, because the tool is shipped into
		/// an installation folder and is then certainly matched to the product next to it.
		/// </summary>
		internal static string? FindNewest()
		{
			var ownDirectory = Path.GetDirectoryName(Environment.ProcessPath ?? string.Empty);
			if (Contains(ownDirectory))
			{
				return ownDirectory;
			}

			var root = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				"IDEA StatiCa");

			if (!Directory.Exists(root))
			{
				return null;
			}

			// Folder names are "StatiCa 25.1", "StatiCa 26.0", ... - ordinal descending puts the highest
			// version first for every shape these names have taken so far.
			var candidates = Directory.GetDirectories(root);
			Array.Sort(candidates, (a, b) => string.CompareOrdinal(b, a));

			foreach (var candidate in candidates)
			{
				if (Contains(candidate))
				{
					return candidate;
				}
			}

			return null;
		}

		/// <summary>
		/// True when <paramref name="directory"/> holds the Connection REST API service executable.
		/// </summary>
		internal static bool Contains(string? directory)
		{
			return !string.IsNullOrEmpty(directory)
				&& File.Exists(Path.Combine(directory!, ServiceExecutableName));
		}
	}
}
