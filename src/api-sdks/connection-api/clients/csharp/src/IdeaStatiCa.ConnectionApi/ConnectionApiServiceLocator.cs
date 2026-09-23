using IdeaStatiCa.Api.Connection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// One installation of the Connection REST API service found on this machine.
	/// </summary>
	public sealed class ConnectionApiServiceInstallation
	{
		internal ConnectionApiServiceInstallation(string directory, string executablePath, Version version)
		{
			Directory = directory;
			ExecutablePath = executablePath;
			Version = version;
		}

		/// <summary>The directory to hand to <see cref="ConnectionApiServiceRunner"/>.</summary>
		public string Directory { get; }

		/// <summary>Full path of the service executable inside <see cref="Directory"/>.</summary>
		public string ExecutablePath { get; }

		/// <summary>
		/// The version of the executable itself, read from its file version - the build included, which
		/// is what a folder name cannot tell. <c>0.0</c> when it could not be read.
		/// </summary>
		public Version Version { get; }

		/// <inheritdoc/>
		public override string ToString() => $"{Version} ({Directory})";
	}

	/// <summary>
	/// Finds the Connection REST API service on this machine, and asks a running one which version it is.
	///
	/// <see cref="ConnectionApiServiceRunner"/> needs a directory and offers nothing to compute one, so
	/// every application that spawns a service has had to answer these two questions for itself:
	///
	///   - WHICH service to start. Hard-coding a path breaks on a machine with a different version
	///     installed, and the failure surfaces late: a service too old for the endpoints in use answers
	///     404, and one too new can answer everything while returning a model the client cannot read.
	///   - WHETHER to start one at all. A service HOLDS AN IDEA StatiCa LICENCE SEAT while it runs, so
	///     attaching to one that is already listening costs nothing where starting a second one costs a
	///     seat.
	///
	/// Choosing between installations is left to the caller: which versions an application can work with
	/// is a property of the application, not of the machine.
	/// </summary>
	public static class ConnectionApiServiceLocator
	{
		/// <summary>Name of the service executable.</summary>
		public const string ExecutableName = "IdeaStatiCa.ConnectionRestApi.exe";

		/// <summary>
		/// A full path to a service executable in this environment variable overrides everything the
		/// search below would find - the escape hatch for a portable copy or a build laid down by hand.
		/// </summary>
		public const string ExecutableOverrideVariable = "IDEA_CONNECTION_REST_EXE";

		/// <summary>The port a service that nobody configured listens on.</summary>
		public const int DefaultPort = 5000;

		private const string VersionEndpoint = "api/4/" + ConRestApiConstants.Client + "/idea-service-version";

		/// <summary>
		/// Every installation the search finds, newest first.
		///
		/// Where it looks: the executable named by <see cref="ExecutableOverrideVariable"/>, then
		/// <paramref name="root"/> or - when that is null - the conventional roots
		/// (<c>%ProgramFiles%\IDEA StatiCa</c> and its 64-bit form), both as a directory that may itself
		/// hold the executable and as a parent of the per-version directories. Both shapes occur: a full
		/// installation ("StatiCa 26.0") and the service on its own ("Connection API 26.1").
		///
		/// The registry is deliberately not read. It would find an installation outside these roots, but
		/// it would also make this package depend on Microsoft.Win32.Registry for a netstandard target -
		/// and its other advantage, the build number, is available here from the executable itself.
		/// An installation in an unconventional place is what the override variable and
		/// <paramref name="root"/> are for.
		/// </summary>
		/// <param name="root">A directory to search instead of the conventional roots. Optional.</param>
		public static IReadOnlyList<ConnectionApiServiceInstallation> FindInstallations(string root = null)
		{
			var found = new List<ConnectionApiServiceInstallation>();
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			string overridden = Environment.GetEnvironmentVariable(ExecutableOverrideVariable);
			if (!string.IsNullOrWhiteSpace(overridden) && File.Exists(overridden))
			{
				Add(Path.GetDirectoryName(overridden));
			}

			foreach (string searchRoot in Roots(root))
			{
				Add(searchRoot);                     // the root may itself be an installation
				try
				{
					foreach (string directory in Directory.GetDirectories(searchRoot))
					{
						Add(directory);
					}
				}
				catch (Exception)
				{
					// a root that does not exist or cannot be listed is simply not a source
				}
			}

			return found.OrderByDescending(i => i.Version).ToList();

			void Add(string directory)
			{
				if (string.IsNullOrWhiteSpace(directory))
				{
					return;
				}

				string executable = Path.Combine(directory, ExecutableName);
				if (!File.Exists(executable) || !seen.Add(executable))
				{
					return;
				}

				found.Add(new ConnectionApiServiceInstallation(directory, executable, VersionOf(executable)));
			}
		}

		/// <summary>
		/// Whether a directory holds the service executable, i.e. whether it can be handed to
		/// <see cref="ConnectionApiServiceRunner"/>.
		/// </summary>
		public static bool IsServiceInstallation(string directory)
			=> !string.IsNullOrWhiteSpace(directory)
				&& File.Exists(Path.Combine(directory, ExecutableName));

		/// <summary>
		/// The version a service listening at this base URL reports, or null when nothing answers there
		/// or what answers is not this service. Never throws - "is one running" and "which one" are the
		/// same question.
		/// </summary>
		/// <param name="baseUrl">For example <c>http://localhost:5000</c>.</param>
		/// <param name="timeout">How long to wait. Defaults to three seconds.</param>
		/// <param name="cancellationToken">Token to cancel the probe.</param>
		public static async Task<string> GetRunningServiceVersionAsync(string baseUrl,
			TimeSpan? timeout = null, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrWhiteSpace(baseUrl))
			{
				return null;
			}

			try
			{
				using (var http = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(3) })
				{
					var response = await http
						.GetAsync($"{baseUrl.TrimEnd('/')}/{VersionEndpoint}", cancellationToken)
						.ConfigureAwait(false);
					if (!response.IsSuccessStatusCode)
					{
						return null;
					}

					// the endpoint answers a bare JSON string
					string version = (await response.Content.ReadAsStringAsync().ConfigureAwait(false))
						.Trim().Trim('"');
					return string.IsNullOrWhiteSpace(version) ? null : version;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// The version of a service already listening on <see cref="DefaultPort"/>, or null.
		///
		/// Worth asking before starting one: a service that is already running can be used through
		/// <see cref="ConnectionApiServiceAttacher"/>, and starting another takes a second licence seat.
		/// </summary>
		public static Task<string> GetRunningServiceVersionOnDefaultPortAsync(
			CancellationToken cancellationToken = default)
			=> GetRunningServiceVersionAsync($"http://localhost:{DefaultPort}",
				cancellationToken: cancellationToken);

		private static IEnumerable<string> Roots(string root)
		{
			if (!string.IsNullOrWhiteSpace(root))
			{
				yield return root;
				yield break;
			}

			// ProgramW6432 is the 64-bit Program Files whatever the bitness of this process; ProgramFiles
			// is the same directory for a 64-bit process and the x86 one for a 32-bit process, so both
			// are searched and the duplicate is dropped by the executable path.
			foreach (string variable in new[] { "ProgramW6432", "ProgramFiles" })
			{
				string programFiles = Environment.GetEnvironmentVariable(variable);
				if (!string.IsNullOrWhiteSpace(programFiles))
				{
					yield return Path.Combine(programFiles, "IDEA StatiCa");
				}
			}
		}

		private static Version VersionOf(string executablePath)
		{
			try
			{
				var info = FileVersionInfo.GetVersionInfo(executablePath);
				if (Version.TryParse(info.FileVersion, out Version version))
				{
					return version;
				}

				return new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart,
					info.FilePrivatePart);
			}
			catch (Exception)
			{
				// an executable whose version cannot be read is still an executable
				return new Version(0, 0);
			}
		}
	}
}
