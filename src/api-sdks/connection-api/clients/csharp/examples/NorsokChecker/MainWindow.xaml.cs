using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using Microsoft.Win32;
using NorsokChecker.Models;
using NorsokChecker.Services;

namespace NorsokChecker
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private readonly ObservableCollection<ConnectionCheckResult> _connections = new();
		private readonly ObservableCollection<MemberDisplayInfo> _members = new();
		private ConnectionApiServiceRunner? _runner;

		/// <summary>
		/// True once we have started a service of our own — the only case in which shutting one down
		/// is ours to do. A service the user was already running must be left alone: taking it down
		/// would remove something this app does not own.
		/// </summary>
		private bool _startedOwnService;

		/// <summary>
		/// Kills our service even if this app is killed outright, which Dispose cannot. Held for the
		/// window's lifetime because closing its handle IS the kill.
		/// </summary>
		private ServiceReaper? _reaper;
		private IConnectionApiClient? _apiClient;
		private Guid _projectId;

		/// <summary>
		/// Cancels the running check. Non-null only while a check is in progress.
		///
		/// The token reaches the API calls themselves: this client version DOES take a
		/// cancellationToken on CalculateAsync and GetRawJsonResultsAsync (verified by compiling a
		/// named-argument call — reflection could not answer it, the assembly's dependencies do not
		/// load standalone, and there is no XML doc beside the DLL). So a stop aborts the in-flight
		/// request rather than only landing between steps.
		///
		/// NOT verified: what the SERVICE does with an aborted request — whether the engine drops
		/// the calculation or finishes it unread. That cannot be read off the method signature.
		/// The ThrowIfCancellationRequested checkpoints below therefore stay: they are what makes a
		/// stop clean for the steps this app runs itself.
		/// </summary>
		private CancellationTokenSource? _checkCts;

		/// <summary>Raw JSON results per connection ID.</summary>
		private readonly Dictionary<int, string> _rawResultsPerConnection = new();

		/// <summary>All formula evaluation results, keyed by connection ID.</summary>
		private readonly Dictionary<int, List<NorsokFormulaResult>> _formulaResults = new();

		/// <summary>
		/// The §6.4 topology per connection: every load effect's brace checks, classification and
		/// chord stresses. The results table above holds only the envelope (the governing state per
		/// brace), which cannot answer "show me LE7" or "how was this number reached".
		/// Present even for a rejected joint — its errors are what the tab then lists.
		/// </summary>
		private readonly Dictionary<int, Services.Norsok64.JointTopology> _topologyPerConnection = new();

		/// <summary>
		/// Members per connection, read once when the project is opened. Switching connections then
		/// costs nothing — it used to re-read members and re-export the IOM on every click.
		/// </summary>
		private readonly Dictionary<int, List<MemberDisplayInfo>> _membersPerConnection = new();

		/// <summary>
		/// Drawn member bodies per connection, for the 3D view. Fetched on first selection rather
		/// than up front: the presentation payload is around 1.7 MB per connection.
		/// </summary>
		private readonly Dictionary<int, List<MemberMesh>> _meshesPerConnection = new();

		public event PropertyChangedEventHandler? PropertyChanged;

		public MainWindow()
		{
			InitializeComponent();
			ConnectionsGrid.ItemsSource = _connections;
			MembersGrid.ItemsSource = _members;
			DataContext = this;
			PrefillServicePath(ServiceRootForTest);
			Log("Norsok Checker ready. Configure API path and load a project.");
		}

		/// <summary>
		/// Put the installation this app would actually use into the path box.
		///
		/// The box used to hold a hardcoded "…\StatiCa 26.0". That was a guess, and the app already
		/// knows better: ServiceLocator resolves the same rule against the registry. On a machine
		/// without 26.0 the hardcoded path pointed at a folder that does not exist, and the app only
		/// recovered by failing the File.Exists test in ResolveSetupDir and searching then — so the
		/// box showed a path that was never going to be used, and the user had no way to see which
		/// version would be.
		///
		/// Preferred version first, otherwise newest — the same order ResolveSetupDir applies, so
		/// what is shown is what will run. The hardcoded value in the XAML stays as the last resort
		/// for a machine where nothing is found: an empty box would say less than a wrong path does.
		///
		/// <param name="rootOverride">
		/// A directory to search instead of the machine's real installs — for tests only. It exists
		/// because on a machine that HAS the preferred version the prefilled value coincides with
		/// the old hardcoded one, so a test could not tell a derived value from a constant: it
		/// compared the path with itself and passed either way (measured 2026-08-27 — the first
		/// version of these tests survived removing the prefill entirely).
		/// </param>
		/// </summary>
		/// <summary>
		/// Set before constructing the window to make it search a synthetic tree instead of the
		/// machine's real installs. Null in the app.
		///
		/// A static rather than a constructor parameter because the window is created by WPF from
		/// XAML, and because the prefill has to be exercised THROUGH the constructor: a test that
		/// calls PrefillServicePath itself cannot notice that the constructor stopped calling it —
		/// measured, that is exactly what the first version of these tests missed.
		/// </summary>
		internal static string? ServiceRootForTest;

		internal void PrefillServicePath(string? rootOverride = null)
		{
			// A URL in the box means we are coming back from attach mode, and it must not stay in a
			// box that now means a folder. Falls back to the conventional root, which ResolveSetupDir
			// then reports properly if it holds no exe.
			const string fallback = ServiceLocator.DefaultRoot + @"\StatiCa 26.0";
			bool boxHoldsUrl = TxtApiPath.Text.TrimStart()
				.StartsWith("http", StringComparison.OrdinalIgnoreCase);

			try
			{
				var installs = ServiceLocator.FindInstalls(rootOverride)
					.Where(i => ServiceLocator.IsUsableVersion(i.Version))
					.ToList();
				if (installs.Count == 0)
				{
					if (boxHoldsUrl) TxtApiPath.Text = fallback;
					return;
				}

				TxtApiPath.Text = installs[0].Directory;
				Log($"Service: {installs[0].Directory} (v{installs[0].Label})"
					+ (installs.Count > 1
						? $" — also installed: {string.Join(", ", installs.Skip(1).Select(i => i.Label))}"
						: ""));
			}
			catch (Exception ex)
			{
				// never block start-up over this
				if (boxHoldsUrl) TxtApiPath.Text = fallback;
				Log($"Could not detect the installed service ({ex.Message}) — using the default path.");
			}
		}

		private void Log(string message)
		{
			Dispatcher.Invoke(() =>
			{
				var timestamp = DateTime.Now.ToString("HH:mm:ss");
				LogBox.AppendText($"[{timestamp}] {message}\n");
				LogBox.ScrollToEnd();
			});
		}

		private void BrowseApiPath_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFolderDialog { Title = "Select IDEA StatiCa installation folder" };
			if (dialog.ShowDialog() == true)
				TxtApiPath.Text = dialog.FolderName;
		}

		/// <summary>
		/// The one text box serves both modes, so it has to say — and hold — the right thing.
		/// Attaching used to send whatever was in the box to ConnectionApiServiceAttacher, and the
		/// box defaults to the installation folder, so "Attach to running service" always failed:
		/// an install path is not a URL.
		/// </summary>
		private void ServiceMode_Changed(object sender, RoutedEventArgs e)
		{
			if (LblApiPath == null || TxtApiPath == null) return;   // fires during XAML init

			bool attach = RbAttach.IsChecked == true;
			LblApiPath.Text = attach ? "Service URL:" : "IDEA StatiCa folder:";
			if (BtnBrowseApiPath != null)
				BtnBrowseApiPath.IsEnabled = !attach;

			string cur = TxtApiPath.Text.Trim();
			bool looksUrl = cur.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
				|| cur.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
			if (attach && !looksUrl)
				TxtApiPath.Text = "http://localhost:5000";
			else if (!attach && looksUrl)
				// switching back to spawn: the detected installation, not a hardcoded guess —
				// PrefillServicePath leaves the XAML default in place when nothing is found
				PrefillServicePath(ServiceRootForTest);
		}

		private void BrowseProject_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				Filter = "IDEA Connection files (*.ideaCon)|*.ideaCon|All files (*.*)|*.*",
				Title = "Select Connection project file"
			};
			if (dialog.ShowDialog() == true)
				TxtProjectFile.Text = dialog.FileName;
		}

		/// <summary>
		/// Connect to the Connection REST API: reuse one that is already running, or start our own.
		///
		/// The SDK's ConnectionApiServiceRunner picks a free port, waits for the heartbeat and
		/// reports a missing exe clearly — but it always starts a NEW service, never notices one
		/// already running, and never looks at the version. Both gaps cost something real:
		///
		///   - a service this app starts holds an IDEA StatiCa LICENCE SEAT for its lifetime, so
		///     starting a second one beside a perfectly good first takes a seat for nothing;
		///   - a setup folder pointing at 25.1 launches a service with no /api/4 at all, and the
		///     failure only appears on the first call as a bare 404.
		///
		/// So: probe the default port first, check the version of whatever answers, and when a new
		/// service has to be started, resolve the folder against what is actually installed.
		/// </summary>
		private async Task<IConnectionApiClient> CreateApiClientAsync()
		{
			if (RbAttach.IsChecked == true)
			{
				var url = TxtApiPath.Text.Trim();
				string? version = await ServiceLocator.RunningVersionAsync(url);
				if (version == null)
					throw new InvalidOperationException(
						$"No Connection REST API is answering at {url}.\n\n"
						+ "Either start one there, or switch to \"Spawn local API\" to have this app "
						+ "start its own.");

				// A hard stop, not a warning. A service outside the supported range answers every
				// call, so the only symptom is that the IOM export comes back empty — and §6.4 then
				// reports "feet overlap" for a joint whose feet are clear, which is a false
				// statement about the user's model rather than a visible failure. Refusing to
				// connect is the only way the user finds out at all. See ServiceLocator.MaxVersion.
				if (!ServiceLocator.IsSupported(version))
					throw new InvalidOperationException(
						$"The service at {url} reports version {version}, which this app cannot use.\n\n"
						+ $"Supported: {ServiceLocator.MinVersion.Major}.{ServiceLocator.MinVersion.Minor}"
						+ $" to {ServiceLocator.MaxVersion.Major}.{ServiceLocator.MaxVersion.Minor}.x.\n\n"
						+ (ServiceLocator.VersionOf(version) is { } v && v > ServiceLocator.MaxVersion
							? "This one is NEWER than the API client this app is built against. It "
							+ "would answer every call, but the IOM export would come back empty, and "
							+ "§6.4 would then reject sound joints for overlapping feet — a wrong "
							+ "answer rather than an obvious failure, which is why this is refused "
							+ "rather than warned about."
							: "The /api/4 endpoints this app uses do not exist in versions before "
							+ $"{ServiceLocator.MinVersion.Major}.{ServiceLocator.MinVersion.Minor}.")
						+ "\n\nSwitch to \"Spawn local API\" to have this app start a supported one.");

				Log($"Attached to the service at {url} (v{version})");
				return await new ConnectionApiServiceAttacher(url).CreateApiClient();
			}

			// ── spawn: start our own, and do NOT go looking for a running one ──
			// The two radio buttons mean two different things, and mixing them was a defect: with
			// "Spawn local API" chosen the user asked for their own service, so probing port 5000
			// belongs to "Attach to running service" and nowhere else. Reusing whatever happened to
			// be listening also picked up a service NEWER than this app's API client, and the IOM
			// export then deserialises to null — D/T unreadable, every gap negative, §6.4 rejecting
			// a sound joint for "feet overlap". See ServiceLocator.MaxVersion.
			var setupDir = ResolveSetupDir(TxtApiPath.Text.Trim());

			// noted BEFORE the launch: the process is identified by having started after this moment,
			// so a service the user already had running can never be mistaken for ours
			var launchedAfter = DateTime.Now.AddSeconds(-1);

			_runner ??= new ConnectionApiServiceRunner(setupDir);
			var client = await _runner.CreateApiClient();
			_startedOwnService = true;

			// Adopt it into a Job Object so Windows takes it down with us even on a hard kill —
			// otherwise a killed app leaves the service holding a licence seat. The SDK runner keeps
			// its Process private, hence the search.
			_reaper ??= new ServiceReaper(Log);
			var proc = ServiceReaper.FindServiceStartedAfter(launchedAfter);
			if (proc == null)
				Log("  note: the service process could not be identified, so only an orderly close "
					+ "will shut it down");
			else if (_reaper.Adopt(proc))
				Log($"Started our own service (pid {proc.Id}) — it is shut down when this app closes, "
					+ "even if this app is killed.");
			else
				Log($"Started our own service (pid {proc.Id}) — it is shut down when this app closes "
					+ "normally.");

			return client;
		}

		/// <summary>
		/// The folder to launch the service from: what the user typed, if it holds the exe;
		/// otherwise the best installed version, so a stale or empty path still works.
		/// </summary>
		private string ResolveSetupDir(string typed)
		{
			if (!string.IsNullOrWhiteSpace(typed)
				&& File.Exists(Path.Combine(typed, ServiceLocator.ExeName)))
			{
				// A typed path outside the supported range is refused, not warned about. It used to
				// log and start anyway ("the first call will say") — but on a NEWER service nothing
				// fails: every call is answered and only the IOM comes back empty, so §6.4 quietly
				// reports overlapping feet for a sound joint. And starting it takes a licence seat
				// for a service that cannot be used.
				var v = ServiceLocator.VersionOfFolder(typed);
				if (v.Major > 0 && !ServiceLocator.IsUsableVersion(v))
					throw new InvalidOperationException(
						$"{typed} looks like version {v.Major}.{v.Minor}, which this app cannot use.\n\n"
						+ $"Supported: {ServiceLocator.MinVersion.Major}.{ServiceLocator.MinVersion.Minor}"
						+ $" to {ServiceLocator.MaxVersion.Major}.{ServiceLocator.MaxVersion.Minor}.x.\n\n"
						+ (v > ServiceLocator.MaxVersion
							? "A newer service answers every call, but this app's API client cannot "
							+ "read the model from it — §6.4 would then reject sound joints for "
							+ "overlapping feet instead of failing visibly."
							: "The /api/4 endpoints this app uses do not exist in earlier versions.")
						+ "\n\nClear the box to let the app pick an installed version itself.");
				return typed;
			}

			var installs = ServiceLocator.FindInstalls();
			// IsUsableVersion, not just MinVersion: a 26.1 install is present on developer
			// machines and would otherwise be picked, and this app's API client cannot read the IOM
			// from it (see ServiceLocator.MaxVersion).
			var usable = installs.Where(i => ServiceLocator.IsUsableVersion(i.Version)).ToList();

			if (usable.Count == 0)
			{
				string detail = installs.Count == 0
					? $"No {ServiceLocator.ExeName} was found under {ServiceLocator.DefaultRoot}"
					  + (ServiceLocator.RegistryInstallDir() is { } r ? $" or {r}" : "") + "."
					// naming what IS installed matters: "nothing found" sends the user looking for a
					// missing install when the real problem is the version. And the range has TWO
					// ends — on a machine with only 26.1 the answer is "too new", and saying "too
					// old" there would send them to install something even newer.
					: "None of the installed versions can be used by this app: "
					  + string.Join(", ", installs.Select(i => i.Label))
					  + $". Supported: {ServiceLocator.MinVersion.Major}.{ServiceLocator.MinVersion.Minor}"
					  + $" to {ServiceLocator.MaxVersion.Major}.{ServiceLocator.MaxVersion.Minor}.x"
					  + " — /api/4 does not exist before that, and this app's API client cannot read"
					  + " the model from a newer service.";
				throw new InvalidOperationException(
					$"Cannot start the Connection REST API.\n\n{detail}\n\n"
					+ $"You can also set {ServiceLocator.OverrideVariable} to the exe, or point the "
					+ "folder box at an installation yourself.");
			}

			var best = usable[0];
			Log($"Service folder: {best.Directory} (v{best.Label})"
				+ (usable.Count > 1
					? " — also installed: " + string.Join(", ", usable.Skip(1).Select(i => i.Label))
					: ""));
			return best.Directory;
		}

		private async void LoadProject_Click(object sender, RoutedEventArgs e)
		{
			var projectPath = TxtProjectFile.Text.Trim();
			if (string.IsNullOrEmpty(projectPath) || !File.Exists(projectPath))
			{
				MessageBox.Show("Please select a valid .ideaCon project file.", "Invalid File",
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				Telemetry.ProjectLoadClicked();

				BtnLoadProject.IsEnabled = false;
				ShowStatus("Connecting to API...");
				Log("Connecting to Connection API...");

				_apiClient = await CreateApiClientAsync();
				Log("API ready.");

				Log($"Opening project: {Path.GetFileName(projectPath)}");
				var project = await _apiClient.Project.OpenProjectAsync(projectPath);
				_projectId = project.ProjectId;
				Log($"Project opened. ID = {_projectId}");

				// Apply Norsok code factors
				var settingsService = new ProjectSettingsService(_apiClient, Log);
				await settingsService.ApplyNorsokFactorsAsync(_projectId);

				var connections = project.Connections ?? new();
				_connections.Clear();
				_rawResultsPerConnection.Clear();
				_formulaResults.Clear();
				_topologyPerConnection.Clear();
				_meshesPerConnection.Clear();
				Joint3D?.Clear();

				foreach (var con in connections)
				{
					_connections.Add(new ConnectionCheckResult
					{
						Id = con.Id,
						Name = con.Name ?? $"Connection {con.Id}",
						Status = "Loaded",
						MaxUtilization = 0,
						NorsokPass = "-"
					});
				}

				Log($"Found {connections.Count} connection(s).");

				// Every connection's members are read here, once, so switching between them later is
				// instant and silent.
				_members.Clear();
				if (connections.Count > 0)
				{
					await LoadAllConnectionMembersAsync();
					await LoadLoadEffectCountsAsync();
					ConnectionsGrid.SelectedIndex = 0;
					ShowMembersOf(_connections[0]);
					await ShowJoint3DAsync(_connections[0]);
				}

				BtnRunCheck.IsEnabled = true;

				Telemetry.ProjectLoaded(_connections.Count, _members.Count > 0 && _members.All(m => m.IsCHS));
			}
			catch (Exception ex)
			{
				Telemetry.ProjectLoadFailed(ex);
				AppLog.ReportFailure("Opening the project failed", ex);
				Log($"ERROR: {ex.Message}");
				MessageBox.Show(ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				BtnLoadProject.IsEnabled = true;
				HideStatus();
			}
		}

		/// <summary>
		/// Stops the running check — see <see cref="_checkCts"/> for how far the token reaches.
		/// </summary>
		private void CancelCheck_Click(object sender, RoutedEventArgs e)
		{
			if (_checkCts == null || _checkCts.IsCancellationRequested) return;
			Log("Cancel requested.");
			ShowStatus("Cancelling…");
			BtnCancelCheck.IsEnabled = false;   // one press is enough; it cannot be sped up
			_checkCts.Cancel();
		}

		private async void RunCheck_Click(object sender, RoutedEventArgs e)
		{
			if (_apiClient == null)
				return;

			_checkCts?.Dispose();
			_checkCts = new CancellationTokenSource();
			var ct = _checkCts.Token;

			// Outside the try because the cancellation handler needs it: on a stop it marks the
			// connections THIS run was working on, and must not touch the ones left unticked.
			var selected = _connections.Where(c => c.Selected).ToList();

			try
			{
				Telemetry.CheckClicked();

				BtnRunCheck.IsEnabled = false;
				BtnCancelCheck.Visibility = Visibility.Visible;
				BtnCancelCheck.IsEnabled = true;
				ValidateGeometryInputs();
				ShowStatus("Running NORSOK N-004 compliance check...");
				Log("Starting Norsok N-004 compliance check...");

				// ── Chapter toggles — read first: they decide whether a calculation is needed ──
				bool includeCbfem = ChkChapterCbfem.IsChecked == true;
				bool includeCh64 = ChkChapter64.IsChecked == true;
				bool activeLoadEffectsOnly = ChkActiveLoadEffectsOnly.IsChecked == true;
				Log($"Chapters: CBFEM={(includeCbfem ? "on" : "off")}, §6.4={(includeCh64 ? "on" : "off")}"
					+ $", load effects: {(activeLoadEffectsOnly ? "active only" : "all in the file")}");

				// ── Which connections? The per-row checkbox, not the whole project ──
				// Everything downstream works off this list: the engine is asked to calculate only
				// these, only these are evaluated, and only these are reported. A connection left
				// unticked keeps whatever it said before rather than being cleared — the user
				// excluded it from THIS run, which is not the same as having no result.
				if (selected.Count == 0)
				{
					Log("No connection is ticked for assessment — nothing to check.");
					MessageBox.Show("No connection is selected for assessment.\n\n"
						+ "Tick at least one connection in the list.",
						"Nothing to check", MessageBoxButton.OK, MessageBoxImage.Information);
					return;
				}
				if (selected.Count < _connections.Count)
					Log($"Connections: {selected.Count} of {_connections.Count} ticked for assessment "
						+ $"({string.Join(", ", selected.Select(c => c.Name))})");
				else
					Log($"Connections: all {selected.Count} ticked for assessment");

				var connectionIds = selected.Select(c => c.Id).ToList();
				_rawResultsPerConnection.Clear();

				// ── Calculate only for the CBFEM plate/weld/bolt group ──
				// §6.4 needs load effects and geometry only, so with CBFEM off the calculation is
				// skipped entirely — the engine run is by far the most expensive step here.
				if (includeCbfem)
				{
					// With the toggle OFF the user asked for every load effect in the file to be
					// assessed, so switch them all on before the engine runs. Calculate takes no
					// load-effect selector in this client, so the model's own flags are the only way
					// to say it — and without this the CBFEM side would silently keep honouring the
					// flags while §6.4 ignored them, and the two halves of one report would disagree.
					//
					// Only the copy in the service's memory is touched. The user's .ideaCon is
					// written only by GET /download, which this app never calls.
					if (!activeLoadEffectsOnly)
						await ActivateAllLoadEffectsAsync(connectionIds, ct);

					foreach (var con in selected)
						con.Status = "Calculating...";

					ShowStatus("Running CBFEM calculation...");
					Log("Running CBFEM calculation...");
					var calcResults = await _apiClient.Calculation.CalculateAsync(_projectId, connectionIds, cancellationToken: ct);

					ct.ThrowIfCancellationRequested();
					ShowStatus("Retrieving raw results...");
					Log("Retrieving raw JSON results...");
					var rawResults = await _apiClient.Calculation.GetRawJsonResultsAsync(_projectId, connectionIds, cancellationToken: ct);

					// Store per-connection raw results
					for (int idx = 0; idx < connectionIds.Count && idx < rawResults.Count; idx++)
						_rawResultsPerConnection[connectionIds[idx]] = rawResults[idx];

					// Update connection status from structured results.
					//
					// Paired by ID, not by position. calcResults comes back for the connections that
					// were SENT, so indexing it against _connections was already only correct while
					// every connection was sent — with the per-row assess-me checkbox that is no
					// longer true, and the mismatch would have written each result onto the wrong
					// connection (silently: every field involved is plausible on any row).
					var byId = calcResults.Where(r => r != null).ToDictionary(r => r.Id);
					foreach (var con in selected)
					{
						if (!byId.TryGetValue(con.Id, out var summary)) continue;
						double maxUtil = 0;
						foreach (var s in summary.ResultSummary ?? new())
						{
							if (!s.Skipped && s.CheckValue > maxUtil)
								maxUtil = s.CheckValue;
						}
						con.MaxUtilization = maxUtil;
						con.Status = summary.Passed ? "Calculated" : "Failed (EC)";
					}

					// ── Refine member wall thickness and f_y from raw results plate names ──
					// The shape itself comes from CrossSectionType when the members are read, which
					// needs no calculation; only t and f_y are refined from the modelled plates.
					//
					// Every connection is refined from ITS OWN results. This used to parse
					// rawResults[0] — connection index 0 — and write into _members, which holds
					// whichever connection is currently selected: with connection 2 showing, any
					// member whose name prefix matched a plate in connection 1 took connection 1's
					// thickness and steel. Member names repeat across connections ("C", "B1", "B2"),
					// so the match usually succeeded, and because ShowMembersOf hands out the same
					// objects held in _membersPerConnection, the wrong values were then cached for
					// every later selection.
					if (rawResults.Count > 0)
					{
						try
						{
							int refined = 0;
							foreach (var (conId, rawJson) in _rawResultsPerConnection)
							{
								if (!_membersPerConnection.TryGetValue(conId, out var conMembers)) continue;
								var parsed = RawResultsParser.Parse(rawJson);
								if (conId == connectionIds[0])
									Log($"  Raw results: {parsed.Plates.Count} plates, "
										+ $"{parsed.Welds.Count} welds, {parsed.Bolts.Count} bolts");

								foreach (var member in conMembers)
								{
									string prefix = $"{member.Name}-";
									var memberPlates = parsed.Plates
										.Where(p => p.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
										.ToList();
									if (memberPlates.Count == 0) continue;

									var thicknesses = memberPlates.Where(p => p.Thickness > 0)
										.Select(p => p.Thickness).ToList();
									if (thicknesses.Count > 0)
									{
										// the most common thickness, NOT the rounded key: grouping by
										// Round(t, 1) buckets 8.05 and 8.14 together, and taking the
										// key would report 8.1 for a wall that is neither
										member.WallThickness = thicknesses
											.GroupBy(t => Math.Round(t, 1))
											.OrderByDescending(g => g.Count())
											.First().First();
									}

									var refPlate = memberPlates.FirstOrDefault(p => p.MaterialFy > 0);
									if (refPlate != null)
									{
										member.Fy = refPlate.MaterialFy;
										member.MaterialName = refPlate.MaterialName;
									}
									refined++;
								}
							}
							Log($"  refined t / f_y on {refined} member(s) from their own connection's results");

							MembersGrid.Items.Refresh();
							UpdateTubularState();
						}
						catch (Exception ex)
						{
							Log($"  WARNING: Member refinement from raw results failed: {ex.Message}");
						}
					}
				}
				else
				{
					Log("CBFEM checks off — skipping the calculation entirely (§6.4 needs load effects only)");
					foreach (var con in selected)
						con.Status = "Not calculated";
				}

				// §6.4 topology: section map (id → D/T/fy) for chord/brace identification.
				// There is no manual alternative any more: a single joint type / θ / gap for the whole
				// joint contradicts §6.4, where K/Y/X is resolved per brace from where the forces
				// flow, and the path it fed was unreachable in any case.
				Dictionary<int, Services.Norsok64.JointSectionInfo> sectionMap = new();
				if (includeCh64)
				{
					try
					{
						var crossSections = await _apiClient.Material.GetCrossSectionsAsync(_projectId, cancellationToken: ct);
						sectionMap = Services.Norsok64.JointSectionMap.FromCrossSections(crossSections.Cast<object>());
						Log($"§6.4: section map with {sectionMap.Count} cross-section(s)");
					}
					// A cancelled run is not a failed section map — see the note on the same pattern
					// in ActivateAllLoadEffectsAsync.
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						Log($"WARNING: §6.4 section map failed ({ex.Message}) — §6.4 cannot be checked");
					}
				}

				// ── Evaluate Norsok per connection ──
				ShowStatus("Evaluating Norsok N-004 formulas...");
				Log("Evaluating Norsok N-004 formulas...");
				// Drop the results of the connections being re-run ONLY. A blanket Clear() would
				// also erase the stored §6.4 detail of connections the user left unticked, whose
				// rows still show a verdict — the tab and the report would then have a row with
				// nothing behind it.
				foreach (var con in selected)
				{
					_formulaResults.Remove(con.Id);
					_topologyPerConnection.Remove(con.Id);
				}

				foreach (var con in selected)
				{
					// Per connection: the §6.4 evaluation is the loop that is worth interrupting
					// without the engine in the way.
					ct.ThrowIfCancellationRequested();

					// Null when no calculation was run — §6.4 does not need it.
					_rawResultsPerConnection.TryGetValue(con.Id, out var rawJson);
					if (includeCbfem && rawJson == null)
					{
						con.Status = "No results";
						con.NorsokPass = "N/A";
						continue;
					}

					Log($"  ── Connection: {con.Name} ──");
					ShowStatus($"Evaluating: {con.Name}...");

					// Fetch load effects for this connection
					List<ConLoadEffect>? loadEffects = null;
					try
					{
						// isPercentage: false is MANDATORY — a user may have stored load effects as
						// percentages (σ/fy ratios ~0.003); without the flag the service returns them
						// as saved and every downstream check would silently collapse to util≈0.
						loadEffects = await _apiClient.LoadEffect.GetLoadEffectsAsync(_projectId, con.Id, isPercentage: false);

						// Refresh the grid's active/total column from what was just read. It is
						// filled at load time, but the run can change it: with the toggle off,
						// ActivateAllLoadEffectsAsync switches every state on, and a column still
						// reading "4 / 15" would then contradict what was actually assessed.
						con.TotalLoadEffects = loadEffects.Count;
						con.ActiveLoadEffects = loadEffects.Count(le => le.Active);

						// Honour the model's own on/off switches. A load effect the engineer disabled
						// in IDEA StatiCa is one they decided not to design for, so assessing it
						// anyway reports utilisations for a state that is not part of the design —
						// and on an envelope it can make a disabled state the governing one.
						// Per connection, because the switches are per connection.
						//
						// This filters the §6.4 side only. The CBFEM side is calculated by the engine
						// from the project itself, and this client version (26.0.4) exposes no
						// load-effect selector on Calculate — verified: loadEffectIds appears nowhere
						// in its surface — so there is nothing to pass. Whether the engine itself
						// honours the active flags is NOT verified here; if a CBFEM utilisation ever
						// disagrees with the §6.4 set under this toggle, that is the thing to check
						// first.
						if (activeLoadEffectsOnly)
						{
							int total = loadEffects.Count;
							var active = loadEffects.Where(le => le.Active).ToList();
							if (active.Count < total)
								Log($"    Load effects: {active.Count} of {total} active "
									+ $"({total - active.Count} switched off in the model, skipped)");
							else
								Log($"    Load effects: {total} load case(s), all active");
							loadEffects = active;
						}
						else
						{
							int off = loadEffects.Count(le => !le.Active);
							Log($"    Load effects: {loadEffects.Count} load case(s)"
								+ (off > 0 ? $" — including {off} switched off in the model" : ""));
						}

						// Log per member per LC
						foreach (var le in loadEffects)
						{
							foreach (var ml in le.MemberLoadings ?? new())
							{
								if (ml.SectionLoad == null) continue;
								var sl = ml.SectionLoad;
								Log($"      LC[{le.Id}] Member {ml.MemberId}: N={sl.N:F0} Vy={sl.Vy:F0} Vz={sl.Vz:F0} Mx={sl.Mx:F0} My={sl.My:F0} Mz={sl.Mz:F0}");
							}
						}
					}
					catch (OperationCanceledException) { throw; }
					catch (Exception ex)
					{
						Log($"    WARNING: Could not fetch load effects: {ex.Message}");
					}

					// §6.4 topology: typed members carry origin/axes/offsets → build the joint
					// topology, classify K/Y/X from the force balance, check every brace. This is the
					// ONLY §6.4 path; the manual dropdown parameters it used to fall back to were
					// removed along with the Joint Configuration panel, so a failure here means no
					// §6.4 result at all rather than a degraded one.
					List<Services.Norsok64.JointMemberData>? topoMembers = null;
					string? fetchFailure = null;

					// Say WHY §6.4 is not going to run, when it is not going to run.
					//
					// Each of these three used to leave §6.4 silently absent: the load-effect fetch
					// logs a warning and leaves loadEffects null, and the section map logs one and
					// leaves the map empty — and then this condition just skipped, publishing no row
					// at all. The connection's verdict was then decided by its CBFEM rows alone, so
					// a joint whose §6.4 was never attempted could read PASS. Reachable in the
					// shipped test set: CON10 of test_cs has its braces deleted, so its inherited
					// load effects reference members that no longer exist and the service answers
					// 404 for them.
					if (includeCh64)
					{
						if (loadEffects == null)
							fetchFailure = "the load effects of this connection could not be read";
						else if (loadEffects.Count == 0)
							fetchFailure = "this connection has no load effect — nothing to check";
						else if (sectionMap.Count == 0)
							fetchFailure = "no cross-section data was available for the project";
					}

					if (includeCh64 && fetchFailure == null && loadEffects != null)
					{
						try
						{
							var conMembers = await _apiClient.Member.GetMembersAsync(_projectId, con.Id, cancellationToken: ct);
							topoMembers = conMembers
								.Select(m => Services.Norsok64.JointMemberData.FromConMember(m,
									sectionMap.GetValueOrDefault(m.CrossSectionId ?? -1)
										?? new Services.Norsok64.JointSectionInfo()))
								.ToList();

							// D/T from the connection's OWN model, not from the section name. The
							// section map is per project and name-derived, which is wrong for 96 % of
							// catalogue circular profiles; the IOM facet ring is per connection and
							// measured. See TubeFromIom.
							await EnrichSectionsFromIomAsync(con.Id, topoMembers);
						}
						catch (OperationCanceledException) { throw; }
						catch (Exception ex)
						{
							// No fallback exists, so this is a GAP, not a degraded check — and it has
							// to reach the results, or the connection reads PASS off its CBFEM rows
							// with §6.4 silently absent. The log used to claim "manual joint
							// parameters used", which was doubly wrong: that path was removed, and
							// nothing was checked at all.
							Log($"    WARNING: §6.4 member fetch failed ({ex.Message}) "
								+ "— no §6.4 check was performed for this connection");
							topoMembers = null;
							fetchFailure = ex.Message;
						}
					}

					var checker = new NorsokCheckRunner(_apiClient, _projectId, Log);

					bool autoJointDone = false;
					bool topologyRejected = false;
					var autoJointResults = new List<NorsokFormulaResult>();
					if (fetchFailure != null)
					{
						autoJointResults.Add(new NorsokFormulaResult
						{
							Section = "6.4",
							Equation = "6.4.3",
							Title = "§6.4 could not be evaluated",
							CheckExpression = $"the joint's members could not be read: {fetchFailure}",
							Formula = "-",
							FormulaSubstituted = "no §6.4 check was performed for this joint",
							NotAssessed = true,
						});
					}
					if (topoMembers != null)
					{
						// The topology is kept per connection: the §6.4 tab shows any single load
						// effect, not just the envelope the results table carries.
						autoJointDone = checker.EvaluateJointChecksFromTopology(
							topoMembers, loadEffects, autoJointResults,
							topology: t => _topologyPerConnection[con.Id] = t);
						topologyRejected = !autoJointDone;
					}

					// A joint that fails the §6.4 conditions is not assessed per brace either: the
					// quantities the check rests on (the joint plane, the averaged chord stresses,
					// the K/Y/X balance) are not meaningful, so nothing downstream is published.
					if (topologyRejected)
						Log("    §6.4 topology rejected the joint — no §6.4 check is performed");

					var formulaResults = checker.EvaluateNorsokFormulas(
						con.Id, rawJson, loadEffects, _members.ToList(), includeCbfem);
					formulaResults.AddRange(autoJointResults);
					_formulaResults[con.Id] = formulaResults;

					// Three outcomes, not two. A "not assessed" row is neither a pass nor a failure,
					// so it must not be counted as either — and a connection that carries one cannot
					// be reported as PASS, because part of it was never checked.
					double maxNorsokUtil = 0;
					bool anyFailed = false;
					bool anyNotAssessed = false;
					int assessed = 0;
					foreach (var fr in formulaResults)
					{
						// a note qualifies a check that DID run; it is neither a result nor a gap
						if (fr.IsNote)
						{
							Log($"    {fr.Section} NOTE: {fr.CheckExpression}");
							continue;
						}
						if (fr.NotAssessed)
						{
							anyNotAssessed = true;
						}
						else
						{
							assessed++;
							if (fr.Utilization > maxNorsokUtil) maxNorsokUtil = fr.Utilization;
							if (!fr.Passed) anyFailed = true;
						}
						Log($"    {fr.Section} {fr.Title}: util={fr.Utilization * 100:F1}% {fr.Verdict}");
					}

					if (anyFailed)
					{
						con.NorsokPass = "FAIL";
						con.MaxUtilization = maxNorsokUtil;
						con.Status = "Norsok FAIL";
					}
					else if (assessed == 0)
					{
						// nothing was checked at all — an empty result set used to leave the
						// connection reading as "Norsok OK / PASS / 0.0 %"
						// Check says only THAT §6.4 does not apply, and how many conditions failed.
						// The conditions themselves are one row each in Results and in the report —
						// this tab is the overview.
						int gates = formulaResults.Count(f => !f.IsNote && f.NotAssessed);
						con.NorsokPass = "N/A";
						con.MaxUtilization = 0;
						con.Status = anyNotAssessed
							? (gates > 1 ? $"Outside §6.4 scope ({gates} conditions)" : "Outside §6.4 scope")
							: "Not assessed";
						Log("    nothing was assessed for this connection — reported as N/A, not as a pass"
							+ "; see the Results tab for the conditions that were not met");
					}
					else if (anyNotAssessed)
					{
						con.NorsokPass = "PARTIAL";
						con.MaxUtilization = maxNorsokUtil;
						con.Status = "Partly assessed";
						Log($"    {assessed} check(s) passed, but part of this connection was not assessed");
					}
					else
					{
						con.NorsokPass = "PASS";
						con.MaxUtilization = maxNorsokUtil;
						con.Status = "Norsok OK";
					}
				}

				// Populate tabs
				PopulateResultsTab();
				TabResults.IsEnabled = true;
				TabReport.IsEnabled = true;
				Log("Norsok check completed.");

				// Reports on what this run assessed. Measuring "all passed" over the whole project
				// would let an untouched connection from an earlier run decide the verdict.
				Telemetry.CheckCompleted(
					allPassed: selected.All(c => c.NorsokPass == "PASS"),
					governingUtilization: selected.Max(c => c.MaxUtilization));
			}
			// Cancelling is a decision, not a failure: no error dialog, no failure telemetry.
			// Whatever finished before the stop keeps its verdict; the rest is left as it stood,
			// which is why the log says the results are partial rather than pretending it ran.
			catch (OperationCanceledException)
			{
				Log("Check cancelled. Results are partial — connections not reached keep their previous state.");
				foreach (var con in selected)
				{
					if (con.Status == "Calculating...")
						con.Status = "Cancelled";
				}
			}
			catch (Exception ex)
			{
				Telemetry.CheckFailed(ex);
				AppLog.ReportFailure("The NORSOK check failed", ex);
				Log($"ERROR: {ex.Message}");
				MessageBox.Show(ex.Message, "Check Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				BtnRunCheck.IsEnabled = true;
				BtnCancelCheck.Visibility = Visibility.Collapsed;
				BtnCancelCheck.IsEnabled = false;
				_checkCts?.Dispose();
				_checkCts = null;
				HideStatus();
			}
		}

		/// <summary>
		/// Read how many load effects each connection has, and how many are switched on, so the
		/// connections table can show it before anything is run — that count is what the
		/// "Active load effects only" toggle decides between.
		///
		/// A connection whose load effects cannot be read keeps its counts unset (-1), which the
		/// grid shows as an em dash: not knowing is not the same as having none.
		/// </summary>
		private async Task LoadLoadEffectCountsAsync()
		{
			if (_apiClient == null || _projectId == Guid.Empty) return;

			foreach (var con in _connections)
			{
				try
				{
					// isPercentage is irrelevant here — only Active and the count are read — but the
					// flag is passed explicitly so this call cannot be mistaken for one that reads forces.
					var les = await _apiClient.LoadEffect.GetLoadEffectsAsync(_projectId, con.Id, isPercentage: false);
					con.TotalLoadEffects = les.Count;
					con.ActiveLoadEffects = les.Count(le => le.Active);
				}
				catch (Exception ex)
				{
					Log($"  WARNING: could not read load effects of {con.Name}: {ex.Message}");
				}
			}

			int known = _connections.Count(c => c.TotalLoadEffects >= 0);
			if (known > 0)
				Log($"  load effects: {_connections.Where(c => c.ActiveLoadEffects >= 0).Sum(c => c.ActiveLoadEffects)}"
					+ $" active of {_connections.Where(c => c.TotalLoadEffects >= 0).Sum(c => c.TotalLoadEffects)}"
					+ $" across {known} connection(s)");
		}

		/// <summary>
		/// Read the members of EVERY connection once, at load time, and cache them. Switching
		/// connections then only swaps the grid contents — no API calls, no log noise, no waiting.
		/// </summary>
		private async Task LoadAllConnectionMembersAsync()
		{
			if (_apiClient == null || _projectId == Guid.Empty) return;

			_membersPerConnection.Clear();

			// project-wide, so it is fetched once rather than per connection
			var detectedCss = await new CrossSectionDetector(_apiClient, Log).DetectAsync(_projectId);

			foreach (var con in _connections)
			{
				ShowStatus($"Reading members of {con.Name}...");
				try
				{
					_membersPerConnection[con.Id] = await ReadMembersAsync(con, detectedCss);
					Log($"  {con.Name}: {_membersPerConnection[con.Id].Count} member(s)");
				}
				catch (Exception ex)
				{
					_membersPerConnection[con.Id] = new List<MemberDisplayInfo>();
					Log($"  WARNING: could not read members of {con.Name}: {ex.Message}");
				}
			}
		}

		/// <summary>Show a cached connection's members. No API traffic.</summary>
		private void ShowMembersOf(ConnectionCheckResult con)
		{
			_members.Clear();
			foreach (var m in _membersPerConnection.GetValueOrDefault(con.Id) ?? new List<MemberDisplayInfo>())
				_members.Add(m);

			// The chord count is a §6.4 condition, so the check reports it per connection in the
			// Status column and one row per condition in Results. Repeating it above the grid only
			// duplicated it — the Role column already shows which member is the chord.
			int chords = _members.Count(m => m.Role == "Chord");
			if (chords != 1)
				Log($"  {con.Name}: {(chords == 0 ? "no continuous member" : $"{chords} continuous members")}"
					+ " — §6.4 needs exactly one");

			MembersGrid.Items.Refresh();
			UpdateTubularState();
		}

		/// <summary>
		/// The members of ONE connection, with each member matched to its own cross-section and its
		/// D/t taken from the connection's own IOM model where that can be read.
		/// </summary>
		private async Task<List<MemberDisplayInfo>> ReadMembersAsync(
			ConnectionCheckResult con, List<DetectedCrossSection> detectedCss)
		{
			var result = new List<MemberDisplayInfo>();
			var geoReader = new MemberGeometryReader(_apiClient!, Log);
			var memberInfos = await geoReader.ReadMembersAsync(
				_projectId, con.Id, rawResults: null, ct: default);

			foreach (var info in memberInfos)
			{
				double diameter = 0;
				double wallThickness = info.WallThickness;
				string shape = info.ShapeType;

				// The member's OWN cross-section, matched by id. This used to sort the project's
				// sections by diameter and take the largest for a continuous member and the
				// smallest for every other — so on a joint with several profiles every member
				// but one got the wrong section. Measured on test_cs CON1: four different braces
				// all reported PIPE127STD, the chord got the smallest section, and D/t came out
				// 0 wherever that name did not parse.
				var matchCss = detectedCss.FirstOrDefault(c => c.Id == info.CrossSectionId);
				if (matchCss != null)
				{
					shape = matchCss.ShapeType;
					if (matchCss.Diameter > 0) diameter = matchCss.Diameter;
					if (matchCss.Thickness > 0) wallThickness = matchCss.Thickness;
				}
				else if (info.CrossSectionId != null)
				{
					Log($"  WARNING: member '{info.Name}' references cross-section "
						+ $"{info.CrossSectionId}, which was not read — D/t unknown");
				}

				result.Add(new MemberDisplayInfo
				{
					Id = info.Id,
					Name = info.Name,
					Role = info.IsContinuous ? "Chord" : "Brace",
					Shape = shape,
					Profile = matchCss?.Name ?? "",
					Diameter = diameter,
					WallThickness = wallThickness,
					// material and fy come from the cross-section, so they are known before any
					// calculation; the raw-results values (when a run happens) refine them
					Fy = info.Fy > 0 ? info.Fy : matchCss?.Fy > 0 ? matchCss.Fy : 355,
					MaterialName = !string.IsNullOrEmpty(info.MaterialName)
						? info.MaterialName
						: matchCss?.MaterialName ?? "",
				});
			}

			// D/T from the connection's own model wherever it can be read — the section name is
			// wrong for most catalogue circular profiles. See TubeFromIom.
			await EnrichFromIomAsync(con.Id, result);
			return result;
		}

		/// <summary>
		/// Overwrite the grid's D/t with the values measured from the IOM facet ring, matched by
		/// member name. Same source as the §6.4 path uses — without this the grid would keep showing
		/// the name-parsed values (or 0) while the check ran on different numbers.
		/// </summary>
		private async Task EnrichFromIomAsync(int connectionId, List<MemberDisplayInfo> grid)
		{
			IdeaRS.OpenModel.Connection.ConnectionData? iom;
			try
			{
				iom = await _apiClient!.Export.ExportIomConnectionDataAsync(_projectId, connectionId);
			}
			catch (Exception ex)
			{
				Log($"  IOM export failed ({ex.Message}) — D/t stay as read from the cross-sections");
				return;
			}

			var beams = Services.Norsok64.TubeFromIom.TubularBeamsByName(iom);
			foreach (var m in grid)
			{
				if (!beams.TryGetValue(m.Name, out var beam)) continue;
				var (d, t, why) = Services.Norsok64.TubeFromIom.FromBeam(beam);
				if (d is not > 0 || t is not > 0)
				{
					Log($"  IOM: '{m.Name}' D/t not readable ({why})");
					continue;
				}
				bool changed = Math.Abs(m.Diameter - d.Value) > 0.05 || Math.Abs(m.WallThickness - t.Value) > 0.01;
				if (changed)
					Log($"  IOM: '{m.Name}' Ø{d:F1}/{t:F1} mm from the model "
						+ $"(cross-section said Ø{m.Diameter:F1}/{m.WallThickness:F1})");
				m.Diameter = d.Value;
				m.WallThickness = t.Value;
				m.Shape = "CHS";
			}
		}

		/// <summary>
		/// The API configuration and the log belong to setting the run up, so they are shown on the
		/// Check tab only. They used to sit outside the tab control and take vertical space on
		/// Results and Report, where neither is any use.
		/// </summary>
		private void MainTabs_SelectionChanged(object sender,
			System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (!ReferenceEquals(e.OriginalSource, MainTabs)) return;   // ignore inner grids' events
			if (ConfigCard == null || LogCard == null) return;          // fires during XAML init

			// Check is the first tab; the rest (§6.4, CBFEM, Results, Report) show results only
			bool onCheck = MainTabs.SelectedIndex == 0;
			ConfigCard.Visibility = onCheck ? Visibility.Visible : Visibility.Collapsed;
			LogCard.Visibility = onCheck ? Visibility.Visible : Visibility.Collapsed;
		}

		/// <summary>
		/// Selecting a connection shows ITS members — from the cache, so the grid swap costs nothing.
		/// The 3D bodies are fetched on first selection and cached the same way.
		/// </summary>
		private async void ConnectionsGrid_SelectionChanged(object sender,
			System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (!IsLoaded) return;
			// this event bubbles up to the tab control, so it must not be mistaken for a tab change
			e.Handled = true;
			if (ConnectionsGrid.SelectedItem is not ConnectionCheckResult con) return;

			ShowMembersOf(con);
			await ShowJoint3DAsync(con);
		}

		/// <summary>
		/// Fill the 3D view with the connection's member bodies, fetching them the first time only.
		/// A failure here never blocks anything: the view is a picture, not a result.
		/// </summary>
		private async Task ShowJoint3DAsync(ConnectionCheckResult con)
		{
			if (Joint3D == null) return;
			Joint3D.Load(await MeshesForAsync(con.Id, con.Name));
		}

		/// <summary>
		/// One connection's member bodies, fetched the first time and cached.
		///
		/// Shared by both views deliberately. The §6.4 tab used to read the cache directly and show
		/// "0 members" for any connection the user had not first selected on the Check tab — the
		/// cache was only ever filled from there. Worse than an empty picture: the tables beside it
		/// still held the PREVIOUS connection's numbers, so the sheet showed one joint's forces
		/// under another joint's name.
		/// </summary>
		private async Task<List<MemberMesh>> MeshesForAsync(int connectionId, string? connectionName = null)
		{
			if (_apiClient == null || _projectId == Guid.Empty) return new List<MemberMesh>();
			if (_meshesPerConnection.TryGetValue(connectionId, out var cached)) return cached;

			List<MemberMesh> meshes;
			try
			{
				// presentations/text is the app's own drawing of the joint — see
				// JointPresentationReader for the payload's shape
				string json = await _apiClient.Presentation.GetDataScene3DTextAsync(_projectId, connectionId);
				meshes = JointPresentationReader.ReadMembers(json, Log);
			}
			catch (Exception ex)
			{
				Log($"  3D view unavailable for {connectionName ?? $"connection {connectionId}"}: {ex.Message}");
				meshes = new List<MemberMesh>();
			}
			_meshesPerConnection[connectionId] = meshes;
			return meshes;
		}

		/// <summary>
		/// Switch every load effect on, so the CBFEM engine calculates all of them.
		///
		/// Needed because Calculate takes no load-effect selector in this client version: the model's
		/// own active flags are the only way to tell the engine what to include. Without this, turning
		/// the "active only" toggle off would widen the §6.4 set while the CBFEM set stayed narrow,
		/// and one report would carry two different load-effect sets.
		///
		/// Only the project in the service's memory is changed — the user's .ideaCon is written only
		/// by GET /download, which this app never calls. A failure is logged and does not stop the
		/// run: the calculation then covers the active states only, which is the narrower answer, and
		/// the log says so.
		/// </summary>
		private async Task ActivateAllLoadEffectsAsync(List<int> connectionIds, CancellationToken ct = default)
		{
			if (_apiClient == null) return;

			int switched = 0, failed = 0;
			foreach (int conId in connectionIds)
			{
				ct.ThrowIfCancellationRequested();
				try
				{
					var les = await _apiClient.LoadEffect.GetLoadEffectsAsync(
						_projectId, conId, isPercentage: false, cancellationToken: ct);
					foreach (var le in les.Where(l => !l.Active))
					{
						le.Active = true;
						await _apiClient.LoadEffect.UpdateLoadEffectAsync(_projectId, conId, le, cancellationToken: ct);
						switched++;
					}
				}
				// Cancelling must not be swallowed by the catch below and reported as a connection
				// that could not be switched on — the run is stopping, nothing failed.
				catch (OperationCanceledException)
				{
					throw;
				}
				catch (Exception ex)
				{
					failed++;
					Log($"    WARNING: could not switch on the load effects of connection {conId} "
						+ $"({ex.Message}) — CBFEM will cover its active states only");
				}
			}

			if (switched > 0)
				Log($"  switched on {switched} load effect(s) so CBFEM covers every state in the file");
			else if (failed == 0)
				Log("  every load effect in the file was already active");
		}

		/// <summary>Hovering a member row highlights its body in the 3D view.</summary>
		private void MembersGrid_MemberHighlight(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (Joint3D == null) return;
			if (sender is System.Windows.Controls.DataGridRow { Item: MemberDisplayInfo m })
				Joint3D.HighlightMember(m.Id);
		}

		/// <summary>
		/// Leaving a row falls back to the SELECTED member rather than clearing outright — otherwise
		/// picking a member in the 3D view lost its highlight the moment the pointer left the table.
		/// </summary>
		private void MembersGrid_ClearHighlight(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (Joint3D == null) return;
			Joint3D.HighlightMember(MembersGrid.SelectedItem is MemberDisplayInfo sel ? sel.Id : -1);
		}

		/// <summary>
		/// Clicking a body in the 3D view selects its row — the reverse of the hover highlight, so
		/// the two views agree whichever one the user points at. A click on nothing clears both.
		/// </summary>
		private void Joint3D_MemberClicked(object? sender, int memberId)
		{
			if (memberId < 0)
			{
				MembersGrid.SelectedItem = null;
				Joint3D?.HighlightMember(-1);
				return;
			}

			var row = MembersGrid.ItemsSource?.OfType<MemberDisplayInfo>()
				.FirstOrDefault(m => m.Id == memberId);
			if (row == null) return;      // a body with no row (a weld, say) — leave the table alone

			MembersGrid.SelectedItem = row;
			MembersGrid.ScrollIntoView(row);
			Joint3D?.HighlightMember(memberId);
		}

		/// <summary>
		/// Replace each tubular member's D/T with the values measured from the connection's own IOM
		/// model. Port of extract.py enrich_sections_from_iom.
		///
		/// The section map this overrides is built per project from the cross-section name, which is
		/// wrong for most catalogue profiles and can be confidently wrong (PIPE127STD is D = 141.3,
		/// not 127). The IOM facet ring is the modelled geometry, so it is what the check should
		/// stand on; the name survives only as a cross-check in the log.
		///
		/// Never fatal: if the export or a beam cannot be read, the member keeps whatever the name
		/// gave it and the existing gates still decide whether that is good enough.
		/// </summary>
		private async Task EnrichSectionsFromIomAsync(
			int connectionId, List<Services.Norsok64.JointMemberData> members)
		{
			IdeaRS.OpenModel.Connection.ConnectionData? iom;
			try
			{
				iom = await _apiClient!.Export.ExportIomConnectionDataAsync(_projectId, connectionId);
			}
			catch (Exception ex)
			{
				Log($"    WARNING: IOM export failed ({ex.Message}) — D/T stay as parsed from the section names");
				return;
			}

			// An export that comes back empty is NOT the same as a model without tubes, and saying
			// "no tubular beams in the model" for it is a false statement about the user's model.
			//
			// Measured 2026-08-27 against service 26.1.0.2007, and the cause is the CLIENT, not the
			// service: on one open project, in one session, the endpoint returns HTTP 200 with
			// 418 389 characters and all six beams over raw HTTP, while
			// ExportIomConnectionDataAsync deserialises that same response to null. The two payloads
			// (26.0 and 26.1) are structurally identical — same top-level keys, same beam keys, same
			// plate keys — so what breaks is inside the generated client. Against 26.0 the same
			// client returns all six beams. See IomExportVersionTests, which pins the distinction.
			//
			// An earlier version of this comment blamed the service. It was wrong, and the direction
			// matters: a broken service means waiting for a service build, a broken client means an
			// upgrade or reading the payload ourselves.
			//
			// Either way D/T then come from nowhere, and because the gaps are computed from the
			// diameters, every gap goes negative and §6.4 rejects the joint for "feet overlap" —
			// a conclusion about geometry drawn from the absence of geometry. Say plainly that the
			// model could not be read.
			if (iom == null || iom.Beams == null || iom.Beams.Count == 0)
			{
				Log("    WARNING: the IOM export returned no model for this connection"
					+ $" ({(iom == null ? "no data at all" : "no beams")})"
					+ " — D/T cannot be read from the model, so any tube whose section name does not"
					+ " spell out its dimensions will be reported as unreadable");
				return;
			}

			var beams = Services.Norsok64.TubeFromIom.TubularBeamsByName(iom);
			if (beams.Count == 0)
			{
				Log($"    IOM: the model has {iom.Beams.Count} beam(s) but none of a tubular type"
					+ " — D/T stay as parsed from the section names");
				return;
			}

			foreach (var m in members)
			{
				// only tubular members: the facet formula would return a plausible-looking number
				// for an I-section too, and that is worse than no number at all
				if (!beams.TryGetValue(m.Name ?? "", out var beam)) continue;

				var (d, t, why) = Services.Norsok64.TubeFromIom.FromBeam(beam);
				if (d is not > 0 || t is not > 0)
				{
					Log($"    IOM: '{m.Name}' D/T not readable ({why}) — keeping the name-derived values");
					continue;
				}

				double? nameD = m.Section.D, nameT = m.Section.T;
				m.Section.D = d;
				m.Section.T = t;
				m.Section.IsCHS = true;

				// Cross-check the name against the model. This is the whole reason for reading the
				// facet ring: "PIPE127STD" is really Ø141.3, because 127 is the nominal size. Over
				// 2 % apart, the disagreement is recorded so the report can say so rather than
				// silently using a different number than the name implies.
				if (nameD is > 0 && Math.Abs(nameD.Value - d.Value) / d.Value > 0.02)
				{
					m.Section.GeomNote = $"the section name suggests D = {nameD:F1} mm but the model "
						+ $"has D = {d:F1} mm — using the model";
					Log($"    IOM: '{m.Name}' {m.Section.GeomNote}");
				}

				string cross = nameD is > 0 && nameT is > 0
					? $" (name said Ø{nameD:F1}/{nameT:F1})"
					: " (name gave nothing)";
				Log($"    IOM: '{m.Name}' Ø{d:F1}/{t:F1} mm from {beam.Plates.Count} facets{cross}");
			}
		}



		/// <summary>
		/// Fill the results grids. Results holds every check; the per-chapter tabs hold the same rows
		/// grouped, so §6.4 detail is not buried among the plate and weld checks.
		///
		/// The rows are ordered so that a joint's conditions and assumptions come before its checks —
		/// reading "outside the scope" after a table of utilisations is the wrong way round.
		/// </summary>
		private void PopulateResultsTab()
		{
			var all = new List<object>();
			var joint = new List<object>();
			var cbfem = new List<object>();

			foreach (var (conId, formulas) in _formulaResults)
			{
				var conName = _connections.FirstOrDefault(c => c.Id == conId)?.Name ?? $"Con {conId}";
				// notes and unassessed conditions first, then the checks
				foreach (var fr in formulas.OrderBy(f => f.IsNote || f.NotAssessed ? 0 : 1))
				{
					var row = new
					{
						Connection = conName,
						fr.Section,
						fr.Title,
						fr.Equation,
						LoadCase = !string.IsNullOrEmpty(fr.LoadCaseName) ? fr.LoadCaseName
							: fr.LoadCaseId > 0 ? $"LC{fr.LoadCaseId}" : "envelope",
						Demand = Math.Round(fr.Demand, 2),
						Capacity = Math.Round(fr.Capacity, 2),
						// a utilisation of "0.0 %" next to "not assessed" or a note reads as a result
						Utilization = fr.IsNote || fr.NotAssessed ? "—" : $"{fr.Utilization * 100:F1}%",
						Result = fr.Verdict
					};
					all.Add(row);

					if (fr.Section.StartsWith("6.4", StringComparison.Ordinal))
						joint.Add(row);
					else if (fr.Section is "Plate" or "Weld" or "Bolt" or "CBFEM")
						cbfem.Add(row);
				}
			}

			ResultsGrid.ItemsSource = all;
			GridCbfem.ItemsSource = cbfem;

			// a tab with nothing in it is worse than no tab: it invites a click that shows nothing
			TabCbfem.IsEnabled = cbfem.Count > 0;

			// The §6.4 tab is not fed from these flat rows any more — it binds the per-load-effect
			// topology, so it can show a single state as well as the envelope, the K/X/Y split and
			// the derivation. It enables itself from what it finds.
			PopulateJoint64Tab();

			PopulateReportTab();
		}

		private string BuildReportHtml(bool expandAll = false)
		{
			var allResults = new List<(string connectionName, List<NorsokFormulaResult> formulas)>();
			foreach (var (conId, formulas) in _formulaResults)
			{
				var conName = _connections.FirstOrDefault(c => c.Id == conId)?.Name ?? $"Connection {conId}";
				allResults.Add((conName, formulas));
			}

			return NorsokHtmlReportGenerator.GenerateReport(
				Path.GetFileName(TxtProjectFile.Text), allResults, expandAll);
		}

		private async void PopulateReportTab()
		{
			var html = BuildReportHtml();

			try
			{
				await ReportWebView.EnsureCoreWebView2Async();
				ReportWebView.NavigateToString(html);
			}
			catch (Exception ex)
			{
				Log($"WARNING: WebView2 not available ({ex.Message}).");
			}
		}

		/// <summary>
		/// Export both PDF reports: the official IDEA StatiCa CBFEM report via the
		/// Connection API, and the NORSOK compliance report printed from the HTML
		/// report via WebView2.
		/// </summary>
		private async void ExportPdf_Click(object sender, RoutedEventArgs e)
		{
			if (_apiClient == null || _projectId == Guid.Empty || _connections.Count == 0 || _formulaResults.Count == 0)
			{
				MessageBox.Show("Run the Norsok check first.", "PDF Export", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var dlg = new Microsoft.Win32.SaveFileDialog
			{
				Filter = "PDF files (*.pdf)|*.pdf",
				FileName = $"{Path.GetFileNameWithoutExtension(TxtProjectFile.Text)}-NORSOK-report.pdf",
				Title = "Save NORSOK report (the IDEA StatiCa CBFEM report is saved alongside)"
			};
			if (dlg.ShowDialog() != true) return;

			string dir = Path.GetDirectoryName(dlg.FileName) ?? ".";
			string norsokPdf = dlg.FileName;
			string ideaPdf = Path.Combine(dir, Path.GetFileNameWithoutExtension(dlg.FileName) + "-IDEA-CBFEM.pdf");

			BtnExportPdf.IsEnabled = false;
			try
			{
				Telemetry.ReportExportClicked();

				// 1. Official IDEA StatiCa report via Connection API — one section per connection
				ShowStatus("Generating IDEA StatiCa PDF report via API...");
				Log("Generating IDEA StatiCa CBFEM PDF report via Connection API...");
				// Only the connections that actually have a result. Asking the API to report on one
				// that was never assessed (left unticked, or excluded by a cancelled run) either
				// fails or produces an empty section.
				var conIds = _connections.Where(c => _formulaResults.ContainsKey(c.Id))
					.Select(c => c.Id).ToList();
				await _apiClient.Report.SaveMultipleReportsPdfAsync(_projectId, conIds, ideaPdf);
				Log($"  IDEA StatiCa report: {ideaPdf}");

				// 2. NORSOK compliance report — render the HTML report and print to PDF
				ShowStatus("Exporting NORSOK compliance report to PDF...");
				await ReportWebView.EnsureCoreWebView2Async();

				var navigated = new TaskCompletionSource<bool>();
				void OnNavCompleted(object? s, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs a)
				{
					ReportWebView.NavigationCompleted -= OnNavCompleted;
					navigated.TrySetResult(a.IsSuccess);
				}
				// All cards expanded — the customer must see every formula in the PDF
				ReportWebView.NavigationCompleted += OnNavCompleted;
				ReportWebView.NavigateToString(BuildReportHtml(expandAll: true));
				await navigated.Task;
				await Task.Delay(1200); // allow KaTeX formulas and web fonts to settle (all cards render)

				bool ok = await ReportWebView.CoreWebView2.PrintToPdfAsync(norsokPdf, null);
				if (!ok)
					throw new InvalidOperationException("WebView2 PrintToPdf reported failure.");
				Log($"  NORSOK report: {norsokPdf}");

				// Restore the interactive (collapsible) report view in the app
				PopulateReportTab();

				Log("PDF export completed.");
				Telemetry.ReportExported();

				MessageBox.Show($"Exported:\n• {norsokPdf}\n• {ideaPdf}", "PDF Export",
					MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				Telemetry.ReportExportFailed(ex);
				AppLog.ReportFailure("Exporting the PDF report failed", ex);
				Log($"ERROR exporting PDF: {ex.Message}");
				MessageBox.Show(ex.Message, "PDF Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				BtnExportPdf.IsEnabled = true;
				HideStatus();
			}
		}

		/// <summary>
		/// Report how many members are tubular. §6.4 needs every one of them to be, and the topology
		/// gates say so per member when the check runs — this is only the up-front note. The chord
		/// itself is named in the members-grid header (see ShowMembersOf).
		/// </summary>
		private void UpdateTubularState()
		{
			int chsCount = _members.Count(m => m.IsCHS);
			int other = _members.Count - chsCount;

			if (_members.Count > 0 && other == 0)
				Log($"  all {_members.Count} members are tubular");
			else if (chsCount > 0)
				Log($"  {chsCount} tubular + {other} other section(s) — §6.4 needs every member tubular");
			else
				Log("  no tubular members — §6.4 does not apply");
		}

		private bool ValidateGeometryInputs()
		{
			foreach (var m in _members.Where(m => m.IsCHS))
			{
				if (m.WallThickness >= m.Diameter / 2)
					Log($"WARNING: {m.Name} t={m.WallThickness}mm must be < D/2={m.Diameter / 2}mm");
				if (m.Diameter / m.WallThickness >= 120)
					Log($"WARNING: {m.Name} D/t={m.Diameter / m.WallThickness:F0} exceeds limit of 120 (§6.3.1)");
			}
			var chord = _members.FirstOrDefault(m => m.Role == "Chord" && m.IsCHS);
			var brace = _members.FirstOrDefault(m => m.Role == "Brace" && m.IsCHS);
			if (chord != null && brace != null && brace.Diameter > chord.Diameter)
				Log($"WARNING: Brace d={brace.Diameter}mm cannot exceed chord D={chord.Diameter}mm");
			// θ is no longer entered by hand — it is derived per brace from the member geometry, and
			// the topology gates report it per brace (θ < 5° an error, outside 30–90° a warning).
			return true;
		}

		private void ShowStatus(string text)
		{
			Dispatcher.Invoke(() =>
			{
				StatusText.Text = text;
				StatusBar.Visibility = Visibility.Visible;
				var sb = (System.Windows.Media.Animation.Storyboard)StatusBar.Resources["SpinAnimation"];
				sb.Begin();
			});
		}

		private void HideStatus()
		{
			Dispatcher.Invoke(() =>
			{
				var sb = (System.Windows.Media.Animation.Storyboard)StatusBar.Resources["SpinAnimation"];
				sb.Stop();
				StatusBar.Visibility = Visibility.Collapsed;
			});
		}








		protected override void OnClosed(EventArgs e)
		{
			// Only ours. A service the user was already running is left alone — see
			// _startedOwnService; taking it down would remove something this app does not own.
			if (_startedOwnService)
			{
				try { _runner?.Dispose(); } catch { }
			}
			_runner = null;

			// Closing the job's handle is what kills its processes, so this must run whether or not
			// Dispose above succeeded — it is the mechanism that survives a kill.
			try { _reaper?.Dispose(); } catch { }
			_reaper = null;

			base.OnClosed(e);
		}

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
