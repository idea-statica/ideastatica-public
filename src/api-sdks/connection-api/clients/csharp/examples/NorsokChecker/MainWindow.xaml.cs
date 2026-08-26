using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

		public event PropertyChangedEventHandler? PropertyChanged;

		public MainWindow()
		{
			InitializeComponent();
			ConnectionsGrid.ItemsSource = _connections;
			MembersGrid.ItemsSource = _members;
			DataContext = this;
			Log("Norsok Checker ready. Configure API path and load a project.");

			// Draw initial joint schematic for default selection (T/Y)
			Loaded += (_, _) => DrawJointSchematic(CmbJointType.SelectedIndex);
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

				TubularJointGeometry? jointGeometry = includeCh64 ? ParseJointGeometry() : null;
				if (jointGeometry != null)
					Log($"Joint geometry (manual fallback): Chord {jointGeometry.D}×{jointGeometry.T}, Brace {jointGeometry.d}×{jointGeometry.t}, θ={jointGeometry.ThetaDeg}°");

				// §6.4 auto-topology: section map (id → D/T/fy) for chord/brace identification
				bool autoTopology = ChkAutoTopology.IsChecked == true;
				if (includeCh64)
					Log($"§6.4 mode: {(autoTopology ? "AUTO topology (manual parameters = fallback only)" : "MANUAL joint parameters")}");
				Dictionary<int, Services.Norsok64.JointSectionInfo> sectionMap = new();
				if (includeCh64 && autoTopology)
				{
					try
					{
						var crossSections = await _apiClient.Material.GetCrossSectionsAsync(_projectId);
						sectionMap = Services.Norsok64.JointSectionMap.FromCrossSections(crossSections.Cast<object>());
						Log($"§6.4 auto-topology: section map with {sectionMap.Count} cross-section(s)");
					}
					catch (Exception ex)
					{
						Log($"WARNING: §6.4 section map failed ({ex.Message}) — manual joint parameters will be used");
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

					// A joint that fails the §6.4 conditions is not assessed per brace either. The
					// manual parameters used to run as a fallback here, so a rejected joint reported
					// "outside the scope of §6.4" AND a per-brace interaction at 205 % AND a valid
					// geometry, all at once — three rows contradicting each other. Once the joint is
					// out of scope, the quantities the check rests on (the joint plane, the averaged
					// chord stresses, the K/Y/X balance) are not meaningful, so nothing downstream
					// of them is published.
					var manualJoint = topologyRejected ? null : jointGeometry;
					if (topologyRejected)
						Log("    §6.4 auto-topology rejected the joint — no §6.4 check is performed "
							+ "(the manual joint parameters are NOT used as a fallback)");

					// Find chord member load effects for Qf calculation (manual §6.4 path only)
					double[] chordStresses = ExtractChordStresses(
						loadEffects, autoJointDone ? null : manualJoint, topoMembers);

					var formulaResults = checker.EvaluateNorsokFormulas(
						con.Id, rawJson, loadEffects,
						autoJointDone ? null : manualJoint, chordStresses, _members.ToList(), includeCbfem);
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
						con.NorsokPass = "N/A";
						con.MaxUtilization = 0;
						con.Status = anyNotAssessed ? "Outside §6.4 scope" : "Not assessed";
						Log("    nothing was assessed for this connection — reported as N/A, not as a pass");
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

			if (MembersOfLabel != null)
				MembersOfLabel.Text = $"  — {con.Name}";
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

			bool onCheck = MainTabs.SelectedIndex == 0;
			ConfigCard.Visibility = onCheck ? Visibility.Visible : Visibility.Collapsed;
			LogCard.Visibility = onCheck ? Visibility.Visible : Visibility.Collapsed;
		}

		/// <summary>
		/// Selecting a connection shows ITS members — from the cache, so this is a grid swap and
		/// nothing else. No API call, no log output, no calculation.
		/// </summary>
		private void ConnectionsGrid_SelectionChanged(object sender,
			System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (!IsLoaded) return;
			// this event bubbles up to the tab control, so it must not be mistaken for a tab change
			e.Handled = true;
			if (ConnectionsGrid.SelectedItem is ConnectionCheckResult con)
				ShowMembersOf(con);
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

				string cross = nameD is > 0 && nameT is > 0
					? $" (name said Ø{nameD:F1}/{nameT:F1})"
					: " (name gave nothing)";
				Log($"    IOM: '{m.Name}' Ø{d:F1}/{t:F1} mm from {beam.Plates.Count} facets{cross}");
			}
		}

		/// <summary>
		/// Chord stresses [σ_a, σ_my, σ_mz] in MPa for Qf, on the MANUAL §6.4 path only — the
		/// auto-topology path derives them properly per load effect and per brace
		/// (JointForceResolver.ChordAvgLoad / ChordStressAtBrace), in the brace's own frame.
		///
		/// This crude version cannot do that: with the topology rejected there is no joint plane
		/// and no per-brace frame. It stays deliberately conservative but keeps two properties the
		/// previous version broke:
		///   - only the CHORD's loadings are read. It used to iterate every member, so a brace's
		///     forces could end up reported as chord stress.
		///   - the three components come from ONE load effect — the one with the largest resultant.
		///     They used to be enveloped independently, producing a stress state that occurred in
		///     no single load case.
		/// The chord's own Begin/End loadings are averaged, per NORSOK p.31.
		/// </summary>
		private double[] ExtractChordStresses(
			List<ConLoadEffect>? loadEffects,
			TubularJointGeometry? joint,
			IReadOnlyList<Services.Norsok64.JointMemberData>? topoMembers)
		{
			if (loadEffects == null || joint == null || joint.D <= 0 || joint.T <= 0)
				return new double[] { 0, 0, 0 };

			var (chord, _) = topoMembers != null && topoMembers.Count > 0
				? Services.Norsok64.JointTopologyBuilder.IdentifyChord(topoMembers)
				: (null, null);
			if (chord == null)
			{
				Log("    Chord stresses for Qf: chord unknown (no member geometry) — Qf falls back to 1.0");
				return new double[] { 0, 0, 0 };
			}

			var chordGeo = TubularGeometryCalc.Calculate(joint.D, joint.T);
			double sigmaA = 0, sigmaMy = 0, sigmaMz = 0, worstResultant = -1;

			foreach (var le in loadEffects)
			{
				var chordLoads = le.MemberLoadings?
					.Where(ml => ml.MemberId == chord.Id && ml.SectionLoad != null)
					.Select(ml => ml.SectionLoad!)
					.ToList();
				if (chordLoads == null || chordLoads.Count == 0) continue;

				// average the chord's sections either side of the intersection (NORSOK p.31)
				double n = chordLoads.Average(sl => sl.N) / 1000.0;      // N → kN
				double my = chordLoads.Average(sl => sl.My) / 1000.0;    // N·m → kNm
				double mz = chordLoads.Average(sl => sl.Mz) / 1000.0;

				double sA = n * 1000.0 / chordGeo.A;                     // kN, mm² → MPa
				double sMy = Math.Abs(my * 1e6 / chordGeo.W);
				double sMz = Math.Abs(mz * 1e6 / chordGeo.W);

				// one load effect governs all three components — never mix them across states
				double resultant = Math.Sqrt(sA * sA + sMy * sMy + sMz * sMz);
				if (resultant > worstResultant)
				{
					worstResultant = resultant;
					sigmaA = sA; sigmaMy = sMy; sigmaMz = sMz;
				}
			}

			Log($"    Chord stresses for Qf (chord '{chord.Name}', worst single LE): " +
				$"σ_a={sigmaA:F1} MPa, σ_my={sigmaMy:F1} MPa, σ_mz={sigmaMz:F1} MPa");
			return new double[] { sigmaA, sigmaMy, sigmaMz };
		}

		/// <summary>Parse joint geometry from chord + brace members. Returns null if not all CHS.</summary>
		private TubularJointGeometry? ParseJointGeometry()
		{
			// §6.4 only applies when ALL members are CHS
			bool allCHS = _members.Count > 0 && _members.All(m => m.IsCHS);
			if (!allCHS) return null;

			var chord = _members.FirstOrDefault(m => m.Role == "Chord" && m.IsCHS);
			var brace = _members.FirstOrDefault(m => m.Role == "Brace" && m.IsCHS);

			if (chord == null || brace == null || chord.Diameter <= 0 || brace.Diameter <= 0)
				return null;

			double.TryParse(TxtBraceAngle.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var angle);
			double.TryParse(TxtGap.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var gap);
			if (angle <= 0) angle = 90;

			var jtStr = (CmbJointType.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "T/Y";
			JointType jt = jtStr switch { "K" => JointType.K, "X" => JointType.X, _ => JointType.T_Y };

			return new TubularJointGeometry
			{
				D = chord.Diameter, T = chord.WallThickness,
				d = brace.Diameter, t = brace.WallThickness,
				ThetaDeg = angle,
				FyChord = chord.Fy, FyBrace = brace.Fy,
				Gap = gap,
				JointType = jt
			};
		}

		private void PopulateResultsTab()
		{
			var allFormulas = new List<object>();
			foreach (var (conId, formulas) in _formulaResults)
			{
				var conName = _connections.FirstOrDefault(c => c.Id == conId)?.Name ?? $"Con {conId}";
				foreach (var fr in formulas)
				{
					allFormulas.Add(new
					{
						Connection = conName,
						fr.Section,
						fr.Title,
						fr.Equation,
						LoadCase = !string.IsNullOrEmpty(fr.LoadCaseName) ? fr.LoadCaseName
							: fr.LoadCaseId > 0 ? $"LC{fr.LoadCaseId}" : "envelope",
						Demand = Math.Round(fr.Demand, 2),
						Capacity = Math.Round(fr.Capacity, 2),
						// a utilisation of "0.0 %" next to "not assessed" reads as a result; it is not
						Utilization = fr.NotAssessed ? "—" : $"{fr.Utilization * 100:F1}%",
						Result = fr.Verdict
					});
				}
			}
			ResultsGrid.ItemsSource = allFormulas;
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
		/// Check if all members are CHS. Enable/disable §6.4 joint UI accordingly.
		/// </summary>
		private void UpdateTubularState()
		{
			bool allCHS = _members.Count > 0 && _members.All(m => m.IsCHS);
			int chsCount = _members.Count(m => m.IsCHS);

			JointConfigExpander.IsEnabled = allCHS;

			if (allCHS)
			{
				JointConfigStatus.Text = $"  — all {_members.Count} members are CHS ✓";
				JointConfigStatus.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromRgb(0x2E, 0x7D, 0x32));
				Log($"  All members CHS → §6.3 (member) + §6.4 (joint) checks enabled");
			}
			else if (chsCount > 0)
			{
				JointConfigStatus.Text = $"  — mixed sections ({chsCount} CHS, {_members.Count - chsCount} other) — §6.4 disabled";
				JointConfigStatus.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromRgb(0xF5, 0x7C, 0x00));
				Log($"  Mixed sections: {chsCount} CHS + {_members.Count - chsCount} other → §6.3 only, §6.4 disabled");
			}
			else
			{
				JointConfigStatus.Text = $"  — not all members are tubular — §6.4 not applicable";
				JointConfigStatus.Foreground = new System.Windows.Media.SolidColorBrush(
					System.Windows.Media.Color.FromRgb(0x9E, 0x9E, 0x9E));
				Log($"  No CHS members → plate/weld/bolt checks only, §6.3/§6.4 disabled");
			}
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
			if (double.TryParse(TxtBraceAngle.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var angle))
			{
				if (angle < 30 || angle > 90) Log($"WARNING: Brace angle θ={angle}° outside range 30°–90° (§6.4.3.1)");
			}
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

		private void JointType_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (JointSchematic == null) return;
			DrawJointSchematic(CmbJointType.SelectedIndex);

			// Gap only applies to K-joints
			GapPanel.Visibility = CmbJointType.SelectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
		}

		private void AutoTopology_Changed(object sender, RoutedEventArgs e)
		{
			// Manual joint parameters are editable only when auto-topology is off; with auto ON they
			// remain visible (greyed) as the fallback used when the topology gate rejects the joint.
			if (ManualJointPanel != null)
				ManualJointPanel.IsEnabled = ChkAutoTopology.IsChecked != true;

			// The checkbox is declared IsChecked="True", so this handler also runs while the XAML is
			// being initialized. That is the default state, not a user action — do not report it.
			if (!IsLoaded)
				return;

			Telemetry.AutoTopologyToggled(ChkAutoTopology.IsChecked == true);
		}

		private void DrawJointSchematic(int jointTypeIndex)
		{
			JointSchematic.Children.Clear();

			var chordBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x60, 0x7D, 0x8B));
			var chordFill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(30, 0x60, 0x7D, 0x8B));
			var braceBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xF5, 0x7C, 0x00));
			var braceFill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(30, 0xF5, 0x7C, 0x00));
			var dimBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x9E, 0x9E, 0x9E));
			var dashStyle = new System.Windows.Media.DoubleCollection { 3, 2 };

			// Chord — double-line tubular representation
			double cy = 38; // chord centerline Y
			double cw = 6;  // chord half-width (wall representation)
			AddLine(8, cy - cw, 232, cy - cw, chordBrush, 1.5);    // top wall
			AddLine(8, cy + cw, 232, cy + cw, chordBrush, 1.5);    // bottom wall
			AddLine(8, cy, 232, cy, dimBrush, 0.5, dashStyle);      // centerline

			switch (jointTypeIndex)
			{
				case 0: // K-joint (Fig. 6-5): two braces on same side, angled apart
					double k1x = 90, k2x = 150;
					// Brace A leans left, Brace B leans right — both go UP from chord
					DrawBrace(k1x, cy, -120, 46, braceBrush, braceFill, dimBrush); // A: up-left
					DrawBrace(k2x, cy, -60, 46, braceBrush, braceFill, dimBrush);  // B: up-right
					// Gap dimension
					AddLine(k1x + 2, cy - 4, k2x - 2, cy - 4, dimBrush, 0.8, dashStyle);
					AddLabel("g", (k1x + k2x) / 2 - 3, cy - 15, dimBrush, 9);
					// Labels
					AddLabel("D", 6, cy + 8, chordBrush, 9, true);
					AddLabel("T", 6, cy - 16, chordBrush, 9, true);
					AddLabel("dA", 52, 2, braceBrush, 8, true);
					AddLabel("dB", 168, 2, braceBrush, 8, true);
					AddLabel("θA", k1x + 4, cy - 22, dimBrush, 8, true);
					AddLabel("θB", k2x - 20, cy - 22, dimBrush, 8, true);
					AddLabel("β = d/D", 185, cy + 10, dimBrush, 8);
					AddLabel("γ = D/2T", 185, cy + 20, dimBrush, 8);
					JointTypeLabel.Text = "K-joint — Fig. 6-5";
					break;

				case 1: // T/Y-joint (Fig. 6-3): single brace at angle
					double bx = 120;
					DrawBrace(bx, cy, -60, 52, braceBrush, braceFill, dimBrush);
					// Angle arc
					AddArc(bx, cy, 18, dimBrush);
					AddLabel("θ", bx + 14, cy - 22, dimBrush, 10, true);
					// Dimension labels
					AddLabel("D", 6, cy + 8, chordBrush, 9, true);
					AddLabel("T", 6, cy - 16, chordBrush, 9, true);
					AddLabel("d", bx - 30, 4, braceBrush, 9, true);
					AddLabel("t", bx - 18, 12, braceBrush, 9, true);
					AddLabel("crown", bx + 8, cy - 8, dimBrush, 7);
					AddLabel("saddle", bx - 4, cy + 10, dimBrush, 7);
					// Formulas
					AddLabel("β = d/D", 175, 8, dimBrush, 8);
					AddLabel("γ = D/(2T)", 175, 18, dimBrush, 8);
					AddLabel("τ = t/T", 175, 28, dimBrush, 8);
					JointTypeLabel.Text = "T/Y-joint — Fig. 6-3";
					break;

				case 2: // X-joint (Fig. 6-4): brace passes straight through chord
					double xx = 120;
					// One continuous brace through the chord — top and bottom are the same member
					DrawBrace(xx, cy, -65, 44, braceBrush, braceFill, dimBrush);   // top half (up-right)
					DrawBrace(xx, cy, 115, 44, braceBrush, braceFill, dimBrush);   // bottom half (down-left, same angle)
					AddArc(xx, cy, 18, dimBrush);
					AddLabel("θ", xx + 14, cy - 22, dimBrush, 10, true);
					AddLabel("D", 6, cy + 8, chordBrush, 9, true);
					AddLabel("T", 6, cy - 16, chordBrush, 9, true);
					AddLabel("d", xx + 16, 2, braceBrush, 9, true);
					AddLabel("t", xx + 26, 10, braceBrush, 9, true);
					AddLabel("β = d/D", 185, 8, dimBrush, 8);
					AddLabel("γ = D/(2T)", 185, 18, dimBrush, 8);
					AddLabel("τ = t/T", 185, 28, dimBrush, 8);
					JointTypeLabel.Text = "X-joint — Fig. 6-4";
					break;
			}
		}

		private void DrawBrace(double baseX, double baseY, double angleDeg, double length,
			System.Windows.Media.Brush stroke, System.Windows.Media.Brush fill, System.Windows.Media.Brush dimBrush)
		{
			double rad = angleDeg * Math.PI / 180.0;
			double ex = baseX + length * Math.Cos(rad);
			double ey = baseY + length * Math.Sin(rad);
			double bw = 3; // brace half-width
			double nx = -Math.Sin(rad) * bw;
			double ny = Math.Cos(rad) * bw;

			// Brace as a parallelogram (two walls)
			var poly = new System.Windows.Shapes.Polygon
			{
				Points = new System.Windows.Media.PointCollection
				{
					new(baseX - nx, baseY - ny), new(ex - nx, ey - ny),
					new(ex + nx, ey + ny), new(baseX + nx, baseY + ny)
				},
				Stroke = stroke,
				StrokeThickness = 1.2,
				Fill = fill
			};
			JointSchematic.Children.Add(poly);

			// Centerline
			AddLine(baseX, baseY, ex, ey, dimBrush, 0.4,
				new System.Windows.Media.DoubleCollection { 2, 2 });
		}

		private void AddLine(double x1, double y1, double x2, double y2,
			System.Windows.Media.Brush stroke, double thickness,
			System.Windows.Media.DoubleCollection? dash = null)
		{
			var line = new System.Windows.Shapes.Line
			{
				X1 = x1, Y1 = y1, X2 = x2, Y2 = y2,
				Stroke = stroke, StrokeThickness = thickness
			};
			if (dash != null) line.StrokeDashArray = dash;
			JointSchematic.Children.Add(line);
		}

		private void AddArc(double cx, double cy, double radius, System.Windows.Media.Brush stroke)
		{
			var arc = new System.Windows.Shapes.Path
			{
				Stroke = stroke, StrokeThickness = 0.8,
				Data = new System.Windows.Media.StreamGeometry()
			};
			using (var ctx = ((System.Windows.Media.StreamGeometry)arc.Data).Open())
			{
				ctx.BeginFigure(new System.Windows.Point(cx + radius, cy), false, false);
				ctx.ArcTo(new System.Windows.Point(cx, cy - radius),
					new System.Windows.Size(radius, radius), 0, false,
					System.Windows.Media.SweepDirection.Counterclockwise, true, false);
			}
			JointSchematic.Children.Add(arc);
		}

		private void AddLabel(string text, double x, double y, System.Windows.Media.Brush foreground,
			double fontSize = 10, bool italic = false)
		{
			var tb = new System.Windows.Controls.TextBlock
			{
				Text = text, FontSize = fontSize, Foreground = foreground,
				FontStyle = italic ? FontStyles.Italic : FontStyles.Normal
			};
			System.Windows.Controls.Canvas.SetLeft(tb, x);
			System.Windows.Controls.Canvas.SetTop(tb, y);
			JointSchematic.Children.Add(tb);
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
