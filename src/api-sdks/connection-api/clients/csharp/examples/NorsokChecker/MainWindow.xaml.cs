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

				// Read members and cross-sections (GET only — no calculation)
				_members.Clear();
				if (connections.Count > 0)
				{
					try
					{
						ShowStatus("Reading members and cross-sections...");

						// GET members
						var geoReader = new MemberGeometryReader(_apiClient, Log);
						var memberInfos = await geoReader.ReadMembersAsync(
							_projectId, connections[0].Id, rawResults: null, ct: default);

						// GET cross-section catalog names for diameter detection
						var cssDetector = new CrossSectionDetector(_apiClient, Log);
						var detectedCss = await cssDetector.DetectAsync(_projectId);

						foreach (var info in memberInfos)
						{
							double diameter = 0;
							double wallThickness = info.WallThickness;
							string shape = info.ShapeType; // Will be "Other" without raw results

							// Try to detect shape from cross-section catalog
							DetectedCrossSection? matchCss = null;
							if (detectedCss.Count > 0)
							{
								matchCss = info.IsContinuous
									? detectedCss.OrderByDescending(c => c.Diameter).FirstOrDefault()
									: detectedCss.OrderBy(c => c.Diameter).FirstOrDefault();

								if (matchCss != null)
								{
									shape = matchCss.ShapeType;
									if (matchCss.Diameter > 0) diameter = matchCss.Diameter;
									if (matchCss.Thickness > 0) wallThickness = matchCss.Thickness;
								}
							}

							_members.Add(new MemberDisplayInfo
							{
								Id = info.Id,
								Name = info.Name,
								Role = info.IsContinuous ? "Chord" : "Brace",
								Shape = shape,
								Profile = matchCss?.Name ?? "",
								Diameter = diameter,
								WallThickness = wallThickness,
								Fy = info.Fy > 0 ? info.Fy : 355,
								MaterialName = info.MaterialName,
							});
						}

						Log($"  Members loaded: {_members.Count}");
						UpdateTubularState();
					}
					catch (Exception ex)
					{
						Log($"  WARNING: Could not read members: {ex.Message}");
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

				// ── Refine member shapes from raw results plate names ──
				if (rawResults.Count > 0)
				{
					try
					{
						var parsed = RawResultsParser.Parse(rawResults[0]);
						Log($"  Raw results: {parsed.Plates.Count} plates, {parsed.Welds.Count} welds, {parsed.Bolts.Count} bolts");

						// Detect shape per member from plate names
						foreach (var member in _members)
						{
							string prefix = $"{member.Name}-";
							var memberPlates = parsed.Plates
								.Where(p => p.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
								.ToList();

							if (memberPlates.Count > 0)
							{
								bool hasArc = memberPlates.Any(p => p.Name.Contains("arc", StringComparison.OrdinalIgnoreCase));
								if (hasArc && member.Shape != "CHS")
								{
									member.Shape = "CHS";
									Log($"  Member '{member.Name}' detected as CHS from plate names");
								}

								// Update wall thickness and fy from plates
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

						// Refresh grid
						MembersGrid.Items.Refresh();
						UpdateTubularState();
					}
					catch (Exception ex)
					{
						Log($"  WARNING: Shape refinement failed: {ex.Message}");
					}
				}

				// ── Chapter toggles — disabled chapters are skipped entirely ──
				bool includeCh5 = ChkChapter5.IsChecked == true;
				bool includeCbfem = ChkChapterCbfem.IsChecked == true;
				bool includeCh63 = ChkChapter63.IsChecked == true;
				bool includeCh64 = ChkChapter64.IsChecked == true;
				Log($"Chapters: §5={(includeCh5 ? "on" : "off")}, CBFEM={(includeCbfem ? "on" : "off")}, §6.3={(includeCh63 ? "on" : "off")}, §6.4={(includeCh64 ? "on" : "off")}");

				// ── Fallback geometry — §6.3 checks use per-member D/t/fy/L/k from the grid ──
				TubularGeometry? geometry = includeCh63 ? ParseCHSGeometry() : null;
				var chsMember = _members.FirstOrDefault(m => m.IsCHS);
				double memberLength = chsMember?.L ?? 5000;
				double kFactor = chsMember?.K ?? 0.7;
				if (geometry != null)
					Log("§6.3 member checks: per-member D/t/fy/L/k taken from the Members grid");

				TubularJointGeometry? jointGeometry = includeCh64 ? ParseJointGeometry() : null;
				if (jointGeometry != null)
					Log($"Joint geometry (manual fallback): Chord {jointGeometry.D}×{jointGeometry.T}, Brace {jointGeometry.d}×{jointGeometry.t}, θ={jointGeometry.ThetaDeg}°");

				// §6.4 auto-topology: section map (id → D/T/fy) for chord/brace identification
				Dictionary<int, Services.Norsok64.JointSectionInfo> sectionMap = new();
				if (includeCh64)
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

				// Design Classification — from dropdown (Table 5-1 decision tree)
				int dcIdx = CmbDesignClass.SelectedIndex;
				DesignClassificationInput? dcInput = !includeCh5 ? null : new DesignClassificationInput
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
						}
						catch (Exception ex)
						{
							Log($"    WARNING: §6.4 member fetch failed ({ex.Message}) — manual joint parameters used");
						}
					}

					var checker = new NorsokCheckRunner(_apiClient, _projectId, Log);

					bool autoJointDone = false;
					var autoJointResults = new List<NorsokFormulaResult>();
					if (topoMembers != null)
						autoJointDone = checker.EvaluateJointChecksFromTopology(topoMembers, loadEffects, autoJointResults);
					if (topoMembers != null && !autoJointDone)
						Log("    §6.4 auto-topology rejected the joint (verdict ERROR) — manual joint parameters used");

					// Find chord member load effects for Qf calculation (manual §6.4 path only)
					double[] chordStresses = ExtractChordStresses(loadEffects, autoJointDone ? null : jointGeometry);

					var formulaResults = checker.EvaluateNorsokFormulas(
						con.Id, rawJson, loadEffects, geometry, memberLength, kFactor,
						autoJointDone ? null : jointGeometry, dcInput, chordStresses, _members.ToList(), includeCbfem);
					formulaResults.AddRange(autoJointResults);
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

		/// <summary>Parse CHS geometry from the largest CHS member. Only for tubular connections.</summary>
		private TubularGeometry? ParseCHSGeometry()
		{
			// §6.3 tubular member formulas require CHS members
			var chsMember = _members.Where(m => m.IsCHS && m.Diameter > 0 && m.WallThickness > 0)
				.OrderByDescending(m => m.Diameter)
				.FirstOrDefault();
			if (chsMember != null)
				return TubularGeometryCalc.Calculate(chsMember.Diameter, chsMember.WallThickness);
			return null;
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
				MessageBox.Show($"Exported:\n• {norsokPdf}\n• {ideaPdf}", "PDF Export",
					MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
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
