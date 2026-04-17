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

				// Evaluate Norsok formulas on raw results
				Log("Evaluating Norsok N-004 §6.3 formulas...");
				var checker = new NorsokCheckRunner(_apiClient, _projectId, Log);

				foreach (var con in _connections)
				{
					if (_rawResultsPerConnection.TryGetValue(con.Id, out var rawJson))
					{
						Log($"  Evaluating formulas for: {con.Name}");

						// Fetch load effects (internal forces) from API
						List<IdeaStatiCa.Api.Connection.Model.ConLoadEffect>? loadEffects = null;
						if (!useCache)
						{
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
						}

						var formulaResults = checker.EvaluateNorsokFormulas(
							con.Id, rawJson, loadEffects, geometry, memberLength, kFactor);
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
						Demand = fr.Demand,
						Capacity = fr.Capacity,
						Utilization = fr.Utilization,
						Result = fr.Passed ? "PASS" : "FAIL"
					});
				}
			}

			ResultsGrid.ItemsSource = allFormulas;

			// Populate report text
			PopulateReportTab();
		}

		private void PopulateReportTab()
		{
			var sb = new System.Text.StringBuilder();
			sb.AppendLine("═══════════════════════════════════════════════════════════════");
			sb.AppendLine("  NORSOK N-004 COMPLIANCE REPORT");
			sb.AppendLine("  Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			sb.AppendLine("  Project: " + Path.GetFileName(TxtProjectFile.Text));
			sb.AppendLine("═══════════════════════════════════════════════════════════════");
			sb.AppendLine();

			foreach (var (conId, formulas) in _formulaResults)
			{
				var conName = _connections.FirstOrDefault(c => c.Id == conId)?.Name ?? $"Connection {conId}";
				sb.AppendLine($"─── {conName} ───");
				sb.AppendLine();

				foreach (var fr in formulas)
				{
					sb.AppendLine(fr.ToReportString());
					sb.AppendLine();
				}
			}

			ReportText.Text = sb.ToString();
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
