using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using IdeaStatiCa.ConnectionApi;
using Microsoft.Win32;
using NorsokChecker.Models;
using NorsokChecker.Services;

namespace NorsokChecker
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private readonly ObservableCollection<ConnectionCheckResult> _connections = new();
		private ConnectionApiServiceRunner? _runner;
		private IConnectionApiClient? _apiClient;
		private Guid _projectId;

		/// <summary>Raw JSON results per connection ID (from API or cache).</summary>
		private readonly Dictionary<int, string> _rawResultsPerConnection = new();

		/// <summary>All formula evaluation results, keyed by connection ID.</summary>
		private readonly Dictionary<int, List<NorsokFormulaResult>> _formulaResults = new();

		public event PropertyChangedEventHandler? PropertyChanged;

		public MainWindow()
		{
			InitializeComponent();
			ConnectionsGrid.ItemsSource = _connections;
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
			var dialog = new OpenFolderDialog
			{
				Title = "Select IDEA StatiCa installation folder"
			};

			if (dialog.ShowDialog() == true)
			{
				TxtApiPath.Text = dialog.FolderName;
			}
		}

		private void BrowseProject_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				Filter = "IDEA Connection files (*.ideaCon)|*.ideaCon|All files (*.*)|*.*",
				Title = "Select Connection project file"
			};

			if (dialog.ShowDialog() == true)
			{
				TxtProjectFile.Text = dialog.FileName;
			}
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
				BtnLoadProject.IsEnabled = false;
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

				// Read member geometry from API
				if (connections.Count > 0)
				{
					try
					{
						var geoReader = new MemberGeometryReader(_apiClient, Log);
						var memberInfos = await geoReader.ReadMembersAsync(_projectId, connections[0].Id, ct: default);

						// Auto-populate CHS fields from member data
						var chord = memberInfos.FirstOrDefault(m => m.IsContinuous);
						var brace = memberInfos.FirstOrDefault(m => !m.IsContinuous);

						if (chord != null && chord.WallThickness > 0)
						{
							TxtChordT.Text = chord.WallThickness.ToString("F1", CultureInfo.InvariantCulture);
							if (chord.Fy > 0)
								TxtFyChord.Text = chord.Fy.ToString("F0", CultureInfo.InvariantCulture);
							Log($"  Auto-filled chord T={chord.WallThickness:F1}mm, fy={chord.Fy:F0}MPa from member '{chord.Name}'");
						}

						if (brace != null && brace.WallThickness > 0)
						{
							TxtBraceT.Text = brace.WallThickness.ToString("F1", CultureInfo.InvariantCulture);
							// Also set CHS member thickness
							TxtThickness.Text = brace.WallThickness.ToString("F1", CultureInfo.InvariantCulture);
							Log($"  Auto-filled brace t={brace.WallThickness:F1}mm from member '{brace.Name}'");
						}
					}
					catch (Exception ex)
					{
						Log($"  WARNING: Could not read member geometry: {ex.Message}");
					}
				}

				BtnRunCheck.IsEnabled = true;
			}
			catch (Exception ex)
			{
				Log($"ERROR: {ex.Message}");
				MessageBox.Show(ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				BtnLoadProject.IsEnabled = true;
			}
		}

		private async void RunCheck_Click(object sender, RoutedEventArgs e)
		{
			if (_apiClient == null)
				return;

			var projectPath = TxtProjectFile.Text.Trim();

			try
			{
				BtnRunCheck.IsEnabled = false;
				Log("Starting Norsok N-004 compliance check...");

				// Check for cached results
				bool useCache = false;
				if (ResultCache.Exists(projectPath))
				{
					var answer = MessageBox.Show(
						"Stored results found on disk.\n\nUse cached results instead of running a new calculation?",
						"Cached Results Available",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question);

					useCache = (answer == MessageBoxResult.Yes);
				}

				if (useCache)
				{
					Log("Loading cached raw results from disk...");
					var cached = ResultCache.Load(projectPath);
					// For now, store as single blob for connection 0.
					// TODO: When we have multi-connection caching, split per connection.
					foreach (var con in _connections)
					{
						_rawResultsPerConnection[con.Id] = cached;
					}
					Log($"Cached results loaded ({cached.Length:N0} chars).");
				}
				else
				{
					// Run calculation and get raw results
					var connectionIds = _connections.Select(c => c.Id).ToList();

					foreach (var con in _connections)
					{
						con.Status = "Calculating...";
					}

					Log("Running CBFEM calculation...");
					var calcResults = await _apiClient.Calculation.CalculateAsync(
						_projectId, connectionIds);

					Log("Retrieving raw JSON results...");
					var rawResults = await _apiClient.Calculation.GetRawJsonResultsAsync(
						_projectId, connectionIds);

					// Store per-connection raw results
					for (int i = 0; i < connectionIds.Count && i < rawResults.Count; i++)
					{
						_rawResultsPerConnection[connectionIds[i]] = rawResults[i];
					}

					// Cache to disk (save the first connection's raw results for now)
					if (rawResults.Count > 0)
					{
						ResultCache.Save(projectPath, rawResults[0]);
						Log($"Raw results cached to: {ResultCache.GetCachePath(projectPath)}");
					}

					// Also get structured results for utilization summary
					var detailedResults = await _apiClient.Calculation.GetResultsAsync(
						_projectId, connectionIds);

					// Update connection status from structured results
					for (int i = 0; i < _connections.Count && i < calcResults.Count; i++)
					{
						var con = _connections[i];
						var summary = calcResults[i];

						double maxUtil = 0;
						foreach (var s in summary.ResultSummary ?? new())
						{
							if (!s.Skipped && s.CheckValue > maxUtil)
								maxUtil = s.CheckValue;
						}

						con.MaxUtilization = maxUtil;
						con.Status = summary.Passed ? "Calculated" : "Failed (EC)";
					}
				}

				// Parse tubular geometry from UI (optional)
				TubularGeometry? geometry = null;
				double memberLength = 0;
				double kFactor = 0.7;

				if (double.TryParse(TxtDiameter.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var D) &&
					double.TryParse(TxtThickness.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var t) &&
					D > 0 && t > 0)
				{
					geometry = TubularGeometryCalc.Calculate(D, t);
					double.TryParse(TxtLength.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out memberLength);
					double.TryParse(TxtKFactor.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out kFactor);
					Log($"Tubular geometry: D={D}mm, t={t}mm, L={memberLength}mm, k={kFactor}");
				}

				// Parse tubular joint geometry (optional, for §6.4 joint checks)
				TubularJointGeometry? jointGeometry = null;
				if (double.TryParse(TxtChordD.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var chordD) &&
					double.TryParse(TxtChordT.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var chordT) &&
					double.TryParse(TxtBraceD.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var braceD) &&
					double.TryParse(TxtBraceT.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var braceT) &&
					chordD > 0 && chordT > 0 && braceD > 0 && braceT > 0)
				{
					double.TryParse(TxtBraceAngle.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var angle);
					double.TryParse(TxtFyChord.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var fyChord);
					double.TryParse(TxtGap.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var gap);
					if (angle <= 0) angle = 90;
					if (fyChord <= 0) fyChord = 355;

					var jtStr = (CmbJointType.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "T/Y";
					JointType jt = jtStr switch { "K" => JointType.K, "X" => JointType.X, _ => JointType.T_Y };

					jointGeometry = new TubularJointGeometry
					{
						D = chordD, T = chordT,
						d = braceD, t = braceT,
						ThetaDeg = angle,
						FyChord = fyChord, FyBrace = fyChord,
						Gap = gap,
						JointType = jt
					};

					Log($"Joint geometry: Chord {chordD}×{chordT}, Brace {braceD}×{braceT}, θ={angle}°, {jtStr}, gap={gap}mm");
				}

				// Design Classification inputs
				var dcInput = new DesignClassificationInput
				{
					SubstantialConsequences = ChkSubstantialConsequences.IsChecked == true,
					ResidualStrength = ChkResidualStrength.IsChecked == true,
					HighComplexity = ChkHighComplexity.IsChecked == true,
					HighFatigue = ChkHighFatigue.IsChecked == true,
					ThroughThickness = ChkThroughThickness.IsChecked == true,
				};

				// Evaluate Norsok formulas on raw results
				Log("Evaluating Norsok N-004 §6.3 formulas...");
				var checker = new NorsokCheckRunner(_apiClient, _projectId, Log);

				foreach (var con in _connections)
				{
					if (_rawResultsPerConnection.TryGetValue(con.Id, out var rawJson))
					{
						Log($"  Evaluating formulas for: {con.Name}");

						// Fetch load effects (internal forces) from API — always fetch, even with cached results
						List<IdeaStatiCa.Api.Connection.Model.ConLoadEffect>? loadEffects = null;
						try
						{
							loadEffects = await checker.GetLoadEffectsAsync(con.Id);
							Log($"    Load effects: {loadEffects.Count} load case(s)");
							foreach (var le in loadEffects)
							{
								foreach (var ml in le.MemberLoadings ?? new())
								{
									if (ml.SectionLoad == null) continue;
									var sl = ml.SectionLoad;
									Log($"      Member {ml.MemberId}: N={sl.N:F1} Vy={sl.Vy:F1} Vz={sl.Vz:F1} Mx={sl.Mx:F2} My={sl.My:F2} Mz={sl.Mz:F2}");
								}
							}
						}
						catch (Exception ex)
						{
							Log($"    WARNING: Could not fetch load effects: {ex.Message}");
						}

						var formulaResults = checker.EvaluateNorsokFormulas(
							con.Id, rawJson, loadEffects, geometry, memberLength, kFactor, jointGeometry, dcInput);
						_formulaResults[con.Id] = formulaResults;

						// Determine worst-case Norsok utilization
						double maxNorsokUtil = 0;
						bool allPassed = true;

						foreach (var fr in formulaResults)
						{
							if (fr.Utilization > maxNorsokUtil)
								maxNorsokUtil = fr.Utilization;
							if (!fr.Passed)
								allPassed = false;

							Log($"    {fr.Section} {fr.Title}: util={fr.Utilization:F4} {(fr.Passed ? "PASS" : "FAIL")}");
						}

						con.NorsokPass = allPassed ? "PASS" : "FAIL";
						if (maxNorsokUtil > con.MaxUtilization)
							con.MaxUtilization = maxNorsokUtil;
						con.Status = allPassed ? "Norsok OK" : "Norsok FAIL";
					}
					else
					{
						con.Status = "No results";
						con.NorsokPass = "N/A";
					}
				}

				// Populate results tab
				PopulateResultsTab();

				TabResults.IsEnabled = true;
				TabReport.IsEnabled = true;
				Log("Norsok check completed.");
			}
			catch (Exception ex)
			{
				Log($"ERROR: {ex.Message}");
				MessageBox.Show(ex.Message, "Check Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				BtnRunCheck.IsEnabled = true;
			}
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
						Demand = Math.Round(fr.Demand, 2),
						Capacity = Math.Round(fr.Capacity, 2),
						Utilization = $"{fr.Utilization * 100:F1}%",
						Result = fr.Passed ? "PASS" : "FAIL"
					});
				}
			}

			ResultsGrid.ItemsSource = allFormulas;

			// Populate report text
			PopulateReportTab();
		}

		private async void PopulateReportTab()
		{
			var allResults = new List<(string connectionName, List<NorsokFormulaResult> formulas)>();

			foreach (var (conId, formulas) in _formulaResults)
			{
				var conName = _connections.FirstOrDefault(c => c.Id == conId)?.Name ?? $"Connection {conId}";
				allResults.Add((conName, formulas));
			}

			var html = NorsokHtmlReportGenerator.GenerateReport(
				Path.GetFileName(TxtProjectFile.Text), allResults);

			try
			{
				await ReportWebView.EnsureCoreWebView2Async();
				ReportWebView.NavigateToString(html);
			}
			catch (Exception ex)
			{
				Log($"WARNING: WebView2 not available ({ex.Message}). Report tab may not render.");
			}
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
