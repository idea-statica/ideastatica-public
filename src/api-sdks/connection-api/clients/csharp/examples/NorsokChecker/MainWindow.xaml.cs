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
		private IConnectionApiClient? _apiClient;
		private Guid _projectId;

		/// <summary>Raw JSON results per connection ID.</summary>
		private readonly Dictionary<int, string> _rawResultsPerConnection = new();

		/// <summary>All formula evaluation results, keyed by connection ID.</summary>
		private readonly Dictionary<int, List<NorsokFormulaResult>> _formulaResults = new();

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
			Log("Norsok Checker ready. Configure API path and load a project.");
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
				TxtApiPath.Text = @"C:\Program Files\IDEA StatiCa\StatiCa 26.0";
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

		private async Task<IConnectionApiClient> CreateApiClientAsync()
		{
			if (RbSpawn.IsChecked == true)
			{
				var setupDir = TxtApiPath.Text.Trim();
				_runner ??= new ConnectionApiServiceRunner(setupDir);
				return await _runner.CreateApiClient();
			}
			else
			{
				var url = TxtApiPath.Text.Trim();
				return await new ConnectionApiServiceAttacher(url).CreateApiClient();
			}
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

		private async void RunCheck_Click(object sender, RoutedEventArgs e)
		{
			if (_apiClient == null)
				return;

			try
			{
				Telemetry.CheckClicked();

				BtnRunCheck.IsEnabled = false;
				ValidateGeometryInputs();
				ShowStatus("Running NORSOK N-004 compliance check...");
				Log("Starting Norsok N-004 compliance check...");

				// ── Chapter toggles — read first: they decide whether a calculation is needed ──
				bool includeCbfem = ChkChapterCbfem.IsChecked == true;
				bool includeCh64 = ChkChapter64.IsChecked == true;
				Log($"Chapters: CBFEM={(includeCbfem ? "on" : "off")}, §6.4={(includeCh64 ? "on" : "off")}");

				var connectionIds = _connections.Select(c => c.Id).ToList();
				_rawResultsPerConnection.Clear();

				// ── Calculate only for the CBFEM plate/weld/bolt group ──
				// §6.4 needs load effects and geometry only, so with CBFEM off the calculation is
				// skipped entirely — the engine run is by far the most expensive step here.
				if (includeCbfem)
				{
					foreach (var con in _connections)
						con.Status = "Calculating...";

					ShowStatus("Running CBFEM calculation...");
					Log("Running CBFEM calculation...");
					var calcResults = await _apiClient.Calculation.CalculateAsync(_projectId, connectionIds);

					ShowStatus("Retrieving raw results...");
					Log("Retrieving raw JSON results...");
					var rawResults = await _apiClient.Calculation.GetRawJsonResultsAsync(_projectId, connectionIds);

					// Store per-connection raw results
					for (int idx = 0; idx < connectionIds.Count && idx < rawResults.Count; idx++)
						_rawResultsPerConnection[connectionIds[idx]] = rawResults[idx];

					// Update connection status from structured results
					for (int idx = 0; idx < _connections.Count && idx < calcResults.Count; idx++)
					{
						var con = _connections[idx];
						var summary = calcResults[idx];
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
					if (rawResults.Count > 0)
					{
						try
						{
							var parsed = RawResultsParser.Parse(rawResults[0]);
							Log($"  Raw results: {parsed.Plates.Count} plates, {parsed.Welds.Count} welds, {parsed.Bolts.Count} bolts");

							foreach (var member in _members)
							{
								string prefix = $"{member.Name}-";
								var memberPlates = parsed.Plates
									.Where(p => p.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
									.ToList();

								if (memberPlates.Count > 0)
								{
									var thicknesses = memberPlates.Where(p => p.Thickness > 0).Select(p => p.Thickness).ToList();
									if (thicknesses.Count > 0)
										member.WallThickness = thicknesses.GroupBy(t => Math.Round(t, 1)).OrderByDescending(g => g.Count()).First().Key;

									var refPlate = memberPlates.FirstOrDefault(p => p.MaterialFy > 0);
									if (refPlate != null)
									{
										member.Fy = refPlate.MaterialFy;
										member.MaterialName = refPlate.MaterialName;
									}
								}
							}

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
					foreach (var con in _connections)
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
						var crossSections = await _apiClient.Material.GetCrossSectionsAsync(_projectId);
						sectionMap = Services.Norsok64.JointSectionMap.FromCrossSections(crossSections.Cast<object>());
						Log($"§6.4: section map with {sectionMap.Count} cross-section(s)");
					}
					catch (Exception ex)
					{
						Log($"WARNING: §6.4 section map failed ({ex.Message}) — §6.4 cannot be checked");
					}
				}

				// ── Evaluate Norsok per connection ──
				ShowStatus("Evaluating Norsok N-004 formulas...");
				Log("Evaluating Norsok N-004 formulas...");
				_formulaResults.Clear();

				foreach (var con in _connections)
				{
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
						Log($"    Load effects: {loadEffects.Count} load case(s)");

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
					catch (Exception ex)
					{
						Log($"    WARNING: Could not fetch load effects: {ex.Message}");
					}

					// §6.4 AUTO-TOPOLOGY (preferred): typed members carry origin/axes/offsets → build the
					// joint topology, auto-classify K/Y/X from the force balance, check every brace.
					// Falls back to the manual dropdown parameters when the topology gate rejects.
					List<Services.Norsok64.JointMemberData>? topoMembers = null;
					if (includeCh64 && sectionMap.Count > 0 && loadEffects != null)
					{
						try
						{
							var conMembers = await _apiClient.Member.GetMembersAsync(_projectId, con.Id);
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
						catch (Exception ex)
						{
							Log($"    WARNING: §6.4 member fetch failed ({ex.Message}) — manual joint parameters used");
						}
					}

					var checker = new NorsokCheckRunner(_apiClient, _projectId, Log);

					bool autoJointDone = false;
					bool topologyRejected = false;
					var autoJointResults = new List<NorsokFormulaResult>();
					if (topoMembers != null)
					{
						autoJointDone = checker.EvaluateJointChecksFromTopology(topoMembers, loadEffects, autoJointResults);
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

				Telemetry.CheckCompleted(
					allPassed: _connections.All(c => c.NorsokPass == "PASS"),
					governingUtilization: _connections.Count == 0 ? 0 : _connections.Max(c => c.MaxUtilization));
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
				HideStatus();
			}
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
			if (Joint3D == null || _apiClient == null || _projectId == Guid.Empty) return;

			if (!_meshesPerConnection.TryGetValue(con.Id, out var meshes))
			{
				try
				{
					// presentations/text is the app's own drawing of the joint — see
					// JointPresentationReader for the payload's shape
					string json = await _apiClient.Presentation.GetDataScene3DTextAsync(_projectId, con.Id);
					meshes = JointPresentationReader.ReadMembers(json, Log);
				}
				catch (Exception ex)
				{
					Log($"  3D view unavailable for {con.Name}: {ex.Message}");
					meshes = new List<MemberMesh>();
				}
				_meshesPerConnection[con.Id] = meshes;
			}

			Joint3D.Load(meshes);
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

			var beams = Services.Norsok64.TubeFromIom.TubularBeamsByName(iom);
			if (beams.Count == 0)
			{
				Log("    IOM: no tubular beams in the model — D/T stay as parsed from the section names");
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
			Grid64.ItemsSource = joint;
			GridCbfem.ItemsSource = cbfem;

			// a tab with nothing in it is worse than no tab: it invites a click that shows nothing
			Tab64.IsEnabled = joint.Count > 0;
			TabCbfem.IsEnabled = cbfem.Count > 0;

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
				var conIds = _connections.Select(c => c.Id).ToList();
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
			try { _runner?.Dispose(); _runner = null; } catch { }
			base.OnClosed(e);
		}

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
