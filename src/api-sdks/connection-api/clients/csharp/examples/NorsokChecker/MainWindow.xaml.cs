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
		private ConnectionApiServiceRunner? _runner;
		private IConnectionApiClient? _apiClient;
		private Guid _projectId;

		/// <summary>Raw JSON results per connection ID.</summary>
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
			var dialog = new OpenFolderDialog { Title = "Select IDEA StatiCa installation folder" };
			if (dialog.ShowDialog() == true)
				TxtApiPath.Text = dialog.FolderName;
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

				// Detect cross-section shapes (CHS / I-section / RHS)
				try
				{
					var cssDetector = new CrossSectionDetector(_apiClient, Log);
					var detectedCss = await cssDetector.DetectAsync(_projectId);

					var chsSections = detectedCss.Where(c => c.IsCHS).ToList();
					if (chsSections.Count > 0)
					{
						Log($"  Tubular sections detected: {chsSections.Count} CHS profile(s)");

						// Auto-populate geometry from largest and smallest CHS (chord vs brace)
						var sorted = chsSections.OrderByDescending(c => c.Diameter).ToList();
						var chordCss = sorted.First();
						var braceCss = sorted.Count > 1 ? sorted.Last() : sorted.First();

						if (chordCss.Diameter > 0)
						{
							TxtChordD.Text = chordCss.Diameter.ToString("F0", CultureInfo.InvariantCulture);
							TxtChordT.Text = chordCss.Thickness.ToString("F1", CultureInfo.InvariantCulture);
							TxtDiameter.Text = chordCss.Diameter.ToString("F0", CultureInfo.InvariantCulture);
							Log($"  Auto-filled chord: D={chordCss.Diameter}mm T={chordCss.Thickness}mm from '{chordCss.Name}'");
						}
						if (braceCss.Diameter > 0)
						{
							TxtBraceD.Text = braceCss.Diameter.ToString("F0", CultureInfo.InvariantCulture);
							TxtBraceT.Text = braceCss.Thickness.ToString("F1", CultureInfo.InvariantCulture);
							TxtThickness.Text = braceCss.Thickness.ToString("F1", CultureInfo.InvariantCulture);
							Log($"  Auto-filled brace: d={braceCss.Diameter}mm t={braceCss.Thickness}mm from '{braceCss.Name}'");
						}
					}
					else
					{
						Log($"  No CHS profiles detected — §6.3/§6.4 tubular checks will use manual geometry input");
					}
				}
				catch (Exception ex)
				{
					Log($"  WARNING: Cross-section detection failed: {ex.Message}");
				}

				// Read member geometry from API (fallback for wall thickness / fy)
				if (connections.Count > 0)
				{
					try
					{
						var geoReader = new MemberGeometryReader(_apiClient, Log);
						var memberInfos = await geoReader.ReadMembersAsync(_projectId, connections[0].Id, ct: default);

						var chord = memberInfos.FirstOrDefault(m => m.IsContinuous);
						var brace = memberInfos.FirstOrDefault(m => !m.IsContinuous);

						if (chord != null && chord.WallThickness > 0)
						{
							TxtChordT.Text = chord.WallThickness.ToString("F1", CultureInfo.InvariantCulture);
							if (chord.Fy > 0)
								TxtFyChord.Text = chord.Fy.ToString("F0", CultureInfo.InvariantCulture);
							Log($"  Auto-filled chord T={chord.WallThickness:F1}mm, fy={chord.Fy:F0}MPa from '{chord.Name}'");
						}

						if (brace != null && brace.WallThickness > 0)
						{
							TxtBraceT.Text = brace.WallThickness.ToString("F1", CultureInfo.InvariantCulture);
							TxtThickness.Text = brace.WallThickness.ToString("F1", CultureInfo.InvariantCulture);
							Log($"  Auto-filled brace t={brace.WallThickness:F1}mm from '{brace.Name}'");
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
				HideStatus();
			}
		}

		private async void RunCheck_Click(object sender, RoutedEventArgs e)
		{
			if (_apiClient == null)
				return;

			try
			{
				BtnRunCheck.IsEnabled = false;
				ValidateGeometryInputs();
				ShowStatus("Running NORSOK N-004 compliance check...");
				Log("Starting Norsok N-004 compliance check...");

				// ── Calculate all connections ──
				var connectionIds = _connections.Select(c => c.Id).ToList();

				foreach (var con in _connections)
					con.Status = "Calculating...";

				ShowStatus("Running CBFEM calculation...");
				Log("Running CBFEM calculation...");
				var calcResults = await _apiClient.Calculation.CalculateAsync(_projectId, connectionIds);

				ShowStatus("Retrieving raw results...");
				Log("Retrieving raw JSON results...");
				var rawResults = await _apiClient.Calculation.GetRawJsonResultsAsync(_projectId, connectionIds);

				// Store per-connection raw results
				_rawResultsPerConnection.Clear();
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

				// ── Parse geometry from UI ──
				TubularGeometry? geometry = ParseCHSGeometry();
				double memberLength = 0, kFactor = 0.7;
				if (geometry != null)
				{
					double.TryParse(TxtLength.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out memberLength);
					double.TryParse(TxtKFactor.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out kFactor);
					Log($"CHS geometry: D={geometry.D}mm, t={geometry.t}mm, L={memberLength}mm, k={kFactor}");
				}

				TubularJointGeometry? jointGeometry = ParseJointGeometry();
				if (jointGeometry != null)
					Log($"Joint geometry: Chord {jointGeometry.D}×{jointGeometry.T}, Brace {jointGeometry.d}×{jointGeometry.t}, θ={jointGeometry.ThetaDeg}°");

				// Design Classification — from dropdown (Table 5-1 decision tree)
				int dcIdx = CmbDesignClass.SelectedIndex;
				var dcInput = new DesignClassificationInput
				{
					SubstantialConsequences = dcIdx <= 1,          // DC1,DC2
					ResidualStrength = dcIdx == 2 || dcIdx == 3,   // DC3,DC4
					HighComplexity = dcIdx == 0 || dcIdx == 2,     // DC1,DC3
					HighFatigue = CmbFatigueUtil.SelectedIndex == 1,
					ThroughThickness = ChkThroughThickness.IsChecked == true,
				};

				// ── Evaluate Norsok per connection ──
				ShowStatus("Evaluating Norsok N-004 formulas...");
				Log("Evaluating Norsok N-004 formulas...");
				_formulaResults.Clear();

				foreach (var con in _connections)
				{
					if (!_rawResultsPerConnection.TryGetValue(con.Id, out var rawJson))
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
						loadEffects = await _apiClient.LoadEffect.GetLoadEffectsAsync(_projectId, con.Id);
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

					// Find chord member load effects for Qf calculation
					double[] chordStresses = ExtractChordStresses(loadEffects, jointGeometry);

					var checker = new NorsokCheckRunner(_apiClient, _projectId, Log);
					var formulaResults = checker.EvaluateNorsokFormulas(
						con.Id, rawJson, loadEffects, geometry, memberLength, kFactor,
						jointGeometry, dcInput, chordStresses);
					_formulaResults[con.Id] = formulaResults;

					// Determine worst-case Norsok utilization
					// Skip informational results (DC classification has Utilization=0)
					double maxNorsokUtil = 0;
					bool allPassed = true;
					foreach (var fr in formulaResults)
					{
						// Only count actual checks (not informational like DC)
						if (fr.Section != "5")
						{
							if (fr.Utilization > maxNorsokUtil)
								maxNorsokUtil = fr.Utilization;
							if (!fr.Passed)
								allPassed = false;
						}
						Log($"    {fr.Section} {fr.Title}: util={fr.Utilization * 100:F1}% {(fr.Passed ? "PASS" : "FAIL")}");
					}

					con.NorsokPass = allPassed ? "PASS" : "FAIL";
					con.MaxUtilization = maxNorsokUtil;
					con.Status = allPassed ? "Norsok OK" : "Norsok FAIL";
				}

				// Populate tabs
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
				HideStatus();
			}
		}

		/// <summary>
		/// Extract chord stresses from load effects for Qf calculation.
		/// Returns [sigmaA, sigmaMy, sigmaMz] in MPa.
		/// The chord is the continuous member (highest member ID or IsContinuous).
		/// </summary>
		private double[] ExtractChordStresses(List<ConLoadEffect>? loadEffects, TubularJointGeometry? joint)
		{
			if (loadEffects == null || joint == null || joint.D <= 0 || joint.T <= 0)
				return new double[] { 0, 0, 0 };

			// Chord cross-section properties for stress calculation
			var chordGeo = TubularGeometryCalc.Calculate(joint.D, joint.T);
			double sigmaA = 0, sigmaMy = 0, sigmaMz = 0;

			foreach (var le in loadEffects)
			{
				if (le.MemberLoadings == null) continue;

				// Find the chord member (typically the first/continuous member)
				// In a typical joint, member with the largest N is the chord
				foreach (var ml in le.MemberLoadings)
				{
					if (ml.SectionLoad == null) continue;
					var sl = ml.SectionLoad;

					// Convert N→kN, N·m→kNm, then to stress
					double N_kN = sl.N / 1000.0;
					double My_kNm = sl.My / 1000.0;
					double Mz_kNm = sl.Mz / 1000.0;

					// Axial stress = N/A [kN/mm² → MPa: ×1000/A]
					double sA = Math.Abs(N_kN * 1000.0 / chordGeo.A);
					double sMy = Math.Abs(My_kNm * 1e6 / chordGeo.W);
					double sMz = Math.Abs(Mz_kNm * 1e6 / chordGeo.W);

					// Take worst envelope
					if (sA > Math.Abs(sigmaA)) sigmaA = N_kN >= 0 ? sA : -sA; // Keep sign
					if (sMy > Math.Abs(sigmaMy)) sigmaMy = sMy;
					if (sMz > Math.Abs(sigmaMz)) sigmaMz = sMz;
				}
			}

			Log($"    Chord stresses for Qf: σ_a={sigmaA:F1} MPa, σ_my={sigmaMy:F1} MPa, σ_mz={sigmaMz:F1} MPa");
			return new double[] { sigmaA, sigmaMy, sigmaMz };
		}

		private TubularGeometry? ParseCHSGeometry()
		{
			if (double.TryParse(TxtDiameter.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var D) &&
				double.TryParse(TxtThickness.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var t) &&
				D > 0 && t > 0)
			{
				return TubularGeometryCalc.Calculate(D, t);
			}
			return null;
		}

		private TubularJointGeometry? ParseJointGeometry()
		{
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

				return new TubularJointGeometry
				{
					D = chordD, T = chordT,
					d = braceD, t = braceT,
					ThetaDeg = angle,
					FyChord = fyChord, FyBrace = fyChord,
					Gap = gap,
					JointType = jt
				};
			}
			return null;
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
						LoadCase = fr.LoadCaseId > 0 ? $"LC{fr.LoadCaseId}" : "envelope",
						Demand = Math.Round(fr.Demand, 2),
						Capacity = Math.Round(fr.Capacity, 2),
						Utilization = $"{fr.Utilization * 100:F1}%",
						Result = fr.Passed ? "PASS" : "FAIL"
					});
				}
			}
			ResultsGrid.ItemsSource = allFormulas;
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
				Log($"WARNING: WebView2 not available ({ex.Message}).");
			}
		}

		private bool ValidateGeometryInputs()
		{
			if (double.TryParse(TxtDiameter.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var D) && D > 0)
			{
				if (double.TryParse(TxtThickness.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var t) && t > 0)
				{
					if (t >= D / 2) Log($"WARNING: CHS wall thickness t={t}mm must be less than D/2={D / 2}mm");
					if (D / t >= 120) Log($"WARNING: CHS D/t={D / t:F0} exceeds limit of 120 (§6.3.1)");
				}
			}
			if (double.TryParse(TxtChordD.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var cD) && cD > 0 &&
				double.TryParse(TxtBraceD.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var bD) && bD > 0)
			{
				if (bD > cD) Log($"WARNING: Brace d={bD}mm cannot exceed chord D={cD}mm");
			}
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
			var gapVis = CmbJointType.SelectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
			LblGap.Visibility = gapVis;
			TxtGap.Visibility = gapVis;
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
				case 0: // K-joint (Fig. 6-5): two braces with gap
					double k1x = 85, k2x = 155;
					DrawBrace(k1x, cy, -55, 48, braceBrush, braceFill, dimBrush);
					DrawBrace(k2x, cy, -125, 48, braceBrush, braceFill, dimBrush);
					// Gap dimension
					AddLine(k1x + 3, cy - 3, k2x - 3, cy - 3, dimBrush, 1, dashStyle);
					AddLabel("g", (k1x + k2x) / 2 - 3, cy - 16, dimBrush, 10);
					// Labels
					AddLabel("D", 6, cy + 8, chordBrush, 9, true);
					AddLabel("T", 6, cy - 16, chordBrush, 9, true);
					AddLabel("dA, tA", 48, 2, braceBrush, 8, true);
					AddLabel("dB, tB", 170, 2, braceBrush, 8, true);
					AddLabel("θA", k1x - 18, cy - 20, dimBrush, 9, true);
					AddLabel("θB", k2x + 6, cy - 20, dimBrush, 9, true);
					// Formulas
					AddLabel("β = d/D", 175, cy + 10, dimBrush, 8);
					AddLabel("γ = D/2T", 175, cy + 20, dimBrush, 8);
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

				case 2: // X-joint (Fig. 6-4): brace through chord
					double xx = 120;
					DrawBrace(xx, cy, -70, 36, braceBrush, braceFill, dimBrush);  // top
					DrawBrace(xx, cy, -110, 36, braceBrush, braceFill, dimBrush); // bottom (opposite side)
					AddArc(xx, cy, 18, dimBrush);
					AddLabel("θ", xx + 14, cy - 22, dimBrush, 10, true);
					// Labels
					AddLabel("D", 6, cy + 8, chordBrush, 9, true);
					AddLabel("T", 6, cy - 16, chordBrush, 9, true);
					AddLabel("d", xx + 10, 4, braceBrush, 9, true);
					AddLabel("t", xx + 22, 12, braceBrush, 9, true);
					// Formulas
					AddLabel("β = d/D", 175, 8, dimBrush, 8);
					AddLabel("γ = D/(2T)", 175, 18, dimBrush, 8);
					AddLabel("τ = t/T", 175, 28, dimBrush, 8);
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
