using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace NorsokChecker.Services
{
	/// <summary>
	/// One installed copy of the Connection REST API service.
	/// </summary>
	public sealed record ServiceInstall(Version Version, string Directory, string ExePath)
	{
		/// <summary>"26.0" / "26.1", or the folder name when the version could not be read.</summary>
		public string Label => Version.Major > 0 ? $"{Version.Major}.{Version.Minor}"
			: Path.GetFileName(Directory);

		public override string ToString() => $"{Label}  ({Directory})";
	}

	/// <summary>
	/// Finds the Connection REST API service on this machine, and asks a running one what version
	/// it is. Port of the service-lifecycle half of the python reference's app.py.
	///
	/// Why the app needs this at all: <c>ConnectionApiServiceRunner</c> (the SDK's own launcher)
	/// picks a free port, waits for the heartbeat and reports a missing exe clearly — but it always
	/// starts a NEW service, never notices one already running, and never looks at the version. Two
	/// consequences measured on this machine:
	///
	///   - Starting a second service takes a second IDEA StatiCa LICENCE SEAT. The python
	///     reference records the same finding and reuses a running instance for exactly this reason.
	///   - A path pointing at 25.1 launches a service with no /api/4 at all, and the failure only
	///     surfaces on the first call, as a 404 with nothing to explain it. 26.0 is the floor.
	/// </summary>
	public static class ServiceLocator
	{
		public const string ExeName = "IdeaStatiCa.ConnectionRestApi.exe";

		/// <summary>/api/4 does not exist before 26.0 — a 25.1 service answers UnsupportedApiVersion.</summary>
		public static readonly Version MinVersion = new(26, 0);

		/// <summary>The version this app was developed and verified against; preferred when present.</summary>
		public static readonly Version PreferredVersion = new(26, 0);

		public const string DefaultRoot = @"C:\Program Files\IDEA StatiCa";

		/// <summary>The port a service nobody configured listens on — the one to probe for a reuse.</summary>
		public const int DefaultPort = 5000;

		/// <summary>Set this to a full exe path to override everything below.</summary>
		public const string OverrideVariable = "IDEA_CONNECTION_REST_EXE";

		/// <summary>
		/// The install directory IDEA StatiCa records as its CURRENT one, or null. One path, not a
		/// list — see <see cref="FromRegistry"/> for the enumeration.
		/// </summary>
		public static string? RegistryInstallDir()
		{
			foreach (string path in new[] { @"SOFTWARE\IDEA StatiCa", @"SOFTWARE\WOW6432Node\IDEA StatiCa" })
			{
				try
				{
					using var key = Registry.LocalMachine.OpenSubKey(path);
					if (key?.GetValue("CurrentInstallDir") is string dir
						&& !string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
						return dir;
				}
				catch (Exception)
				{
					// an unreadable key is not an error here — the scan still covers the machine
				}
			}
			return null;
		}

		/// <summary>
		/// Every installed version the registry knows, with its real path and its FULL version.
		///
		/// The layout, measured on this machine 2026-08-27 (four installs present):
		///   HKLM\SOFTWARE\IDEAStatiCa            &lt;- no space, unlike the "IDEA StatiCa" key
		///     25.1\IDEAStatiCa\Designer   InstallDir64 = …\StatiCa 25.1\   Version64 = 25.1.5.1504
		///     26.0\IDEAStatiCa\Designer   InstallDir64 = …\StatiCa 26.0\   Version64 = 26.0.6.0235
		///     26.1\IDEAStatiCa\Designer   InstallDir64 = …\StatiCa 26.1\   Version64 = 26.1.0.2007
		///     ConnectionApi\26.1          InstallDir   = …\Connection API 26.1\  Version = 26.1.0.0022
		///
		/// This is better than a directory scan on three counts: it lists every version rather than
		/// one, it gives the BUILD (26.0.6.0235 — a folder name cannot), and it finds the standalone
		/// Connection API install, whose folder does not match the "StatiCa &lt;maj&gt;.&lt;min&gt;" shape at all.
		///
		/// The python reference does not use it and its comment says these keys "carry user details,
		/// not paths" — measured wrong: the paths are one level deeper, under \IDEAStatiCa\Designer,
		/// past the CompanyName/UserName values that the comment describes.
		///
		/// The directory scan is still kept as a fallback: a portable copy, a build laid down by
		/// hand, or a machine whose registry has been cleaned all leave the exe on disk with nothing
		/// in the registry to point at it.
		/// </summary>
		public static List<ServiceInstall> FromRegistry()
		{
			var found = new List<ServiceInstall>();
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach (string root in new[] { @"SOFTWARE\IDEAStatiCa", @"SOFTWARE\WOW6432Node\IDEAStatiCa" })
			{
				RegistryKey? rootKey = null;
				try { rootKey = Registry.LocalMachine.OpenSubKey(root); } catch (Exception) { }
				if (rootKey == null) continue;

				using (rootKey)
				{
					foreach (string branch in SafeSubKeys(rootKey))
					{
						// "25.1" / "26.0" — a full install; or "ConnectionApi", which nests one more level
						if (branch.Equals("ConnectionApi", StringComparison.OrdinalIgnoreCase))
						{
							using var apiKey = SafeOpen(rootKey, branch);
							if (apiKey == null) continue;
							foreach (string ver in SafeSubKeys(apiKey))
							{
								using var vk = SafeOpen(apiKey, ver);
								Add(vk?.GetValue("InstallDir") as string, vk?.GetValue("Version") as string);
							}
							continue;
						}

						using var designer = SafeOpen(rootKey, $@"{branch}\IDEAStatiCa\Designer");
						if (designer == null) continue;
						Add(designer.GetValue("InstallDir64") as string ?? designer.GetValue("InstallDir") as string,
							designer.GetValue("Version64") as string ?? designer.GetValue("Version") as string);
					}
				}
			}

			void Add(string? dir, string? versionText)
			{
				if (string.IsNullOrWhiteSpace(dir)) return;
				dir = dir!.TrimEnd('\\', '/');
				string exe = Path.Combine(dir, ExeName);
				// the registry can name an install whose files are gone, or one without the service
				if (!File.Exists(exe) || !seen.Add(exe)) return;
				found.Add(new ServiceInstall(ParseVersion(versionText) ?? VersionOfFolder(dir), dir, exe));
			}

			return found;
		}

		private static IEnumerable<string> SafeSubKeys(RegistryKey key)
		{
			try { return key.GetSubKeyNames(); }
			catch (Exception) { return Array.Empty<string>(); }
		}

		private static RegistryKey? SafeOpen(RegistryKey parent, string name)
		{
			try { return parent.OpenSubKey(name); }
			catch (Exception) { return null; }
		}

		/// <summary>Major.minor out of a full version string ("26.0.6.0235"), or null.</summary>
		private static Version? ParseVersion(string? text)
		{
			if (string.IsNullOrWhiteSpace(text)) return null;
			var m = Regex.Match(text, @"(\d+)\.(\d+)", RegexOptions.None, TimeSpan.FromSeconds(1));
			return m.Success
				? new Version(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value))
				: null;
		}

		/// <summary>
		/// Every installed service, best first: the preferred version, then newest to oldest.
		/// Versions below <see cref="MinVersion"/> are listed too — the caller has to be able to say
		/// "25.1 is installed but too old" rather than "nothing found", which sends the user looking
		/// for a missing install.
		/// </summary>
		public static List<ServiceInstall> FindInstalls(string? root = null)
		{
			// THE REGISTRY FIRST: it enumerates every version, with its real path and its full
			// build number — none of which a folder name can give. The scan below is the fallback
			// for what the registry cannot see: a portable copy, a hand-placed build, a cleaned
			// registry. (Skipped when the caller names a root, which only the tests do.)
			var found = root == null ? FromRegistry() : new List<ServiceInstall>();
			var seen = new HashSet<string>(found.Select(i => i.ExePath), StringComparer.OrdinalIgnoreCase);

			var roots = new List<string>();
			if (!string.IsNullOrWhiteSpace(root)) roots.Add(root!);
			else
			{
				roots.Add(DefaultRoot);
				// CurrentInstallDir is the VERSIONED folder, so its parent is the root holding the
				// siblings. Both are scanned: the parent finds the other versions, the directory
				// itself covers an install outside any conventional root.
				if (RegistryInstallDir() is { } reg)
				{
					roots.Add(reg);
					if (Path.GetDirectoryName(reg.TrimEnd('\\', '/')) is { } parent) roots.Add(parent);
				}
			}

			void Consider(string dir)
			{
				string exe = Path.Combine(dir, ExeName);
				if (!File.Exists(exe) || !seen.Add(exe)) return;
				found.Add(new ServiceInstall(VersionOfFolder(dir), dir, exe));
			}

			foreach (string r in roots)
			{
				Consider(r);                                  // r may BE a versioned install
				try
				{
					foreach (string dir in Directory.GetDirectories(r)) Consider(dir);
				}
				catch (Exception)
				{
					// a root that does not exist or cannot be listed is simply not a source
				}
			}

			// preferred first, then newest down. A folder whose version could not be read sorts
			// last rather than being dropped: it has the exe, so it is a real candidate.
			found.Sort((a, b) =>
			{
				bool pa = a.Version == PreferredVersion, pb = b.Version == PreferredVersion;
				if (pa != pb) return pa ? -1 : 1;
				return b.Version.CompareTo(a.Version);
			});
			return found;
		}

		/// <summary>
		/// The version a folder name states, or 0.0 when it states none.
		///
		/// Two shapes are accepted, both measured on this machine: "StatiCa 26.0" (the full install)
		/// and "Connection API 26.1" (the service on its own). The python reference matches only the
		/// first, so it would miss a machine that has just the API installed.
		/// </summary>
		public static Version VersionOfFolder(string directory)
		{
			var m = Regex.Match(Path.GetFileName(directory.TrimEnd('\\', '/')) ?? "",
				@"(\d+)\.(\d+)", RegexOptions.None, TimeSpan.FromSeconds(1));
			return m.Success ? new Version(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value))
				: new Version(0, 0);
		}

		/// <summary>
		/// The version string a service at this base URL reports, or null when nothing answers.
		/// Never throws: "is one running" and "which one" are the same question here.
		/// </summary>
		public static async Task<string?> RunningVersionAsync(string baseUrl, int timeoutSeconds = 3)
		{
			try
			{
				using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutSeconds) };
				var response = await http.GetAsync($"{baseUrl.TrimEnd('/')}/api/4/clients/idea-service-version");
				if (!response.IsSuccessStatusCode) return null;
				string text = (await response.Content.ReadAsStringAsync()).Trim().Trim('"');
				return string.IsNullOrWhiteSpace(text) ? null : text;
			}
			catch (Exception)
			{
				return null;      // nothing listening, or something that is not this service
			}
		}

		/// <summary>
		/// Is a usable service already listening on the default port? Returns its version, or null.
		///
		/// Reusing it is the point: a service this app starts holds a licence seat of its own, and
		/// the seat is not released until the process ends.
		/// </summary>
		public static Task<string?> RunningOnDefaultPortAsync()
			=> RunningVersionAsync($"http://localhost:{DefaultPort}");

		/// <summary>
		/// Whether a reported version string ("26.0.5.1259") meets <see cref="MinVersion"/>.
		/// An unparseable string is accepted: refusing to run over a version format nobody has seen
		/// would be worse than trying and reporting whatever the service then says.
		/// </summary>
		public static bool IsSupported(string? versionText)
		{
			if (string.IsNullOrWhiteSpace(versionText)) return true;
			var m = Regex.Match(versionText, @"(\d+)\.(\d+)", RegexOptions.None, TimeSpan.FromSeconds(1));
			if (!m.Success) return true;
			return new Version(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)) >= MinVersion;
		}
	}
}
