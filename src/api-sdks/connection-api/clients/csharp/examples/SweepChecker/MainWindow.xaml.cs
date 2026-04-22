using IdeaStatiCa.ConnectionApi;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using Microsoft.Win32;
using SweepChecker.Charts;
using SweepChecker.Models;
using SweepChecker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SweepChecker
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private ConnectionApiServiceRunner? _runner;
		private readonly ObservableCollection<SweepParameter> _params = new();
		private SweepOutput? _sweepOutput;

		/// <summary>Build a fresh API client per call. Honours the spawn-vs-attach radio buttons in the UI.</summary>
		private async Task<IConnectionApiClient> CreateApiClientAsync()
		{
			bool attach = Dispatcher.Invoke(() => ModeAttachRadio.IsChecked == true);
			if (attach)
			{
				var url = Dispatcher.Invoke(() => ApiUrlBox.Text.Trim());
				if (string.IsNullOrWhiteSpace(url)) throw new InvalidOperationException("API base URL is empty.");
				Log($"Attaching to running API at {url}");
				return await new ConnectionApiServiceAttacher(url).CreateApiClient();
			}
			else
			{
				var dir = Dispatcher.Invoke(() => IdeaPathBox.Text.Trim());
				_runner ??= new ConnectionApiServiceRunner(dir);
				return await _runner.CreateApiClient();
			}
		}

		// Postprocess state
		private List<LoadCaseRow> _allRows = new();

		public MainWindow()
		{
			InitializeComponent();
			ParamGrid.ItemsSource = _params;
			DataContext = this;
		}

		protected override void OnClosed(EventArgs e)
		{
			try { _runner?.Dispose(); _runner = null; } catch { }
			base.OnClosed(e);
		}

		private void Log(string msg) =>
			Dispatcher.Invoke(() => { LogBox.AppendText(msg + "\n"); LogBox.ScrollToEnd(); });

		// ──────────────────────────── Load parameters ────────────────────────────

		private void ApiMode_Changed(object sender, RoutedEventArgs e)
		{
			// XAML rows may not be initialized yet during InitializeComponent
			if (SpawnRow == null || AttachRow == null) return;
			bool attach = ModeAttachRadio.IsChecked == true;
			SpawnRow.Visibility = attach ? Visibility.Collapsed : Visibility.Visible;
			AttachRow.Visibility = attach ? Visibility.Visible : Visibility.Collapsed;

			// If switching mode, drop any pre-spawned local runner so the next call honours the new choice
			if (attach && _runner != null)
			{
				try { _runner.Dispose(); } catch { }
				_runner = null;
			}
		}

		private void BrowseIdeaPathButton_Click(object sender, RoutedEventArgs e)
		{
			// Use OpenFileDialog targeting the API exe so the user picks the right folder in one step
			var current = IdeaPathBox.Text.Trim();
			var dlg = new OpenFileDialog
			{
				Title = "Select IdeaStatiCa.ConnectionRestApi.exe (or any file in that folder)",
				Filter = "IdeaStatiCa.ConnectionRestApi.exe|IdeaStatiCa.ConnectionRestApi.exe|All files|*.*",
				CheckFileExists = true,
			};
			if (!string.IsNullOrWhiteSpace(current) && Directory.Exists(current))
				dlg.InitialDirectory = current;

			if (dlg.ShowDialog() == true)
			{
				var dir = Path.GetDirectoryName(dlg.FileName);
				if (!string.IsNullOrEmpty(dir))
				{
					IdeaPathBox.Text = dir;
					// Switching folder invalidates any spawned runner from the previous folder
					if (_runner != null) { try { _runner.Dispose(); } catch { } _runner = null; }
				}
			}
		}

		private void BrowseProjectButton_Click(object sender, RoutedEventArgs e)
		{
			var current = ProjectPathBox.Text.Trim();
			var dlg = new OpenFileDialog
			{
				Title = "Select IDEA Connection project",
				Filter = "IDEA Connection project (*.ideaCon)|*.ideaCon|All files|*.*",
				DefaultExt = ".ideaCon",
				CheckFileExists = true,
			};
			if (!string.IsNullOrWhiteSpace(current) && File.Exists(current))
				dlg.InitialDirectory = Path.GetDirectoryName(current);

			if (dlg.ShowDialog() == true)
				ProjectPathBox.Text = dlg.FileName;
		}

		private async void LoadProjectButton_Click(object sender, RoutedEventArgs e)
		{
			var projectPath = ProjectPathBox.Text.Trim();
			if (string.IsNullOrWhiteSpace(projectPath))
			{
				Log("ERROR: pick a project file first (Browse...).");
				return;
			}
			if (!File.Exists(projectPath)) { Log($"ERROR: project file not found: {projectPath}"); return; }

			LoadProjectButton.IsEnabled = false;
			_params.Clear();

			try
			{
				await Task.Run(() => LoadParametersAsync(projectPath));
			}
			catch (Exception ex) { Log($"FATAL: {ex.Message}"); }
			finally
			{
				LoadProjectButton.IsEnabled = true;
				RunSweepButton.IsEnabled = _params.Count > 0;
			}
		}

		private async Task LoadParametersAsync(string projectPath)
		{
			await using var client = await CreateApiClientAsync();
			Log("API ready.");

			var project = await client.Project.OpenProjectAsync(projectPath);
			try
			{
				var conns = project.Connections ?? new();
				if (conns.Count == 0) { Log("No connections in project."); return; }

				// Read visible parameters from the first connection as the master list
				var first = conns[0];
				Log($"Reading parameters from connection {first.Name} (id={first.Id})");
				var parameters = await client.Parameter.GetParametersAsync(project.ProjectId, first.Id, includeHidden: false);

				Dispatcher.Invoke(() =>
				{
					foreach (var p in parameters)
					{
						var valStr = p.Value == null ? "" : Convert.ToString(p.Value, CultureInfo.InvariantCulture) ?? "";
						double start = 0, end = 0, step = 0.05;
						if (double.TryParse(valStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double v))
						{
							start = v; end = v * 1.5 + 0.05; step = Math.Max(0.01, Math.Abs(v) * 0.25);
						}
						_params.Add(new SweepParameter
						{
							Key = p.Key ?? "",
							Description = p.Description ?? "",
							Unit = p.Unit ?? "",
							ParameterType = p.ParameterType ?? "",
							CurrentValue = valStr,
							SingleValue = valStr,
							Start = start,
							End = end,
							Step = step,
							Sweep = false,
						});
					}
				});

				Log($"Loaded {parameters.Count} visible parameter(s).");
			}
			finally { try { await client.Project.CloseProjectAsync(project.ProjectId); } catch { } }
		}

		// ──────────────────────────── Run sweep ────────────────────────────

		private async void RunSweepButton_Click(object sender, RoutedEventArgs e)
		{
			var projectPath = ProjectPathBox.Text.Trim();
			if (!File.Exists(projectPath)) { Log("ERROR: load a project first."); return; }

			var sweptCount = _params.Count(p => p.Sweep);
			if (sweptCount == 0) { Log("ERROR: tick at least one 'Sweep' checkbox."); return; }

			RunSweepButton.IsEnabled = false; LoadProjectButton.IsEnabled = false;
			try
			{
				var snapshot = _params.ToList();
				await Task.Run(async () =>
				{
					await using var client = await CreateApiClientAsync();
					var runner = new SweepRunner(client, Log);
					_sweepOutput = await runner.RunAsync(projectPath, snapshot);
				});

				if (_sweepOutput != null)
				{
					_allRows = SweepIO.Flatten(_sweepOutput);
					Dispatcher.Invoke(() =>
					{
						PopulateConnectionChoice();
						ResultsTab.IsEnabled = true;
						RefreshPostprocess(this, new RoutedEventArgs());
						MainTabs.SelectedItem = ResultsTab;   // jump straight to charts
					});
					Log($"Sweep finished. Total iterations: {_sweepOutput.connections.Sum(c => c.iteration_count)}");
				}
			}
			catch (Exception ex) { Log($"FATAL: {ex.Message}"); }
			finally
			{
				RunSweepButton.IsEnabled = true;
				LoadProjectButton.IsEnabled = true;
			}
		}

		// ──────────────────────────── Postprocess ────────────────────────────

		private void PopulateConnectionChoice()
		{
			ConnectionChoice.Items.Clear();
			if (_sweepOutput == null) return;
			foreach (var c in _sweepOutput.connections)
				ConnectionChoice.Items.Add($"{c.connection_id} — {c.connection_name}");
			if (ConnectionChoice.Items.Count > 0) ConnectionChoice.SelectedIndex = 0;
		}

		private void ChartCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
			=> RefreshPostprocess(sender, new RoutedEventArgs());

		private void HoleCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
			=> RefreshPostprocess(sender, new RoutedEventArgs());

		private void RefreshPostprocess(object sender, RoutedEventArgs e)
		{
			if (_allRows.Count == 0) return;

			int? connectionId = null;
			if (ConnectionChoice.SelectedItem is string s && s.Split('—')[0].Trim() is string idStr
				&& int.TryParse(idStr, out var cid)) connectionId = cid;

			var rows = _allRows.AsEnumerable();
			if (connectionId.HasValue) rows = rows.Where(r => r.ConnectionId == connectionId.Value);
			if (PassedOnlyCheck.IsChecked == true) rows = rows.Where(r => r.OverallPassed);
			var filtered = rows.ToList();

			string section = (SectionChoice.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Plates";
			Func<LoadCaseRow, double> selector;
			string valueLabel;
			switch (section)
			{
				case "Welds": selector = r => r.MaxWeldUtilization; valueLabel = "Max weld utilization"; break;
				case "Bolts": selector = r => r.MaxBoltUtilizationIterationScope; valueLabel = "Max bolt utilization"; break;
				case "Summary": selector = r => r.MaxSummaryUtilization; valueLabel = "Max summary utilization"; break;
				default: selector = r => r.MaxPlateStress; valueLabel = "Max plate stress"; break;
			}

			// Stats label
			StatsLabel.Text = filtered.Count == 0
				? "no rows match filters"
				: $"{filtered.Count} rows · {filtered.Select(r => r.Iteration).Distinct().Count()} iterations · "
				  + $"{filtered.Select(r => r.LoadCaseId).Distinct().Count()} load cases · "
				  + $"max {valueLabel.ToLowerInvariant()} = {filtered.Max(selector):F3}";

			// Canvas chart
			var paramKeys = filtered.Count == 0 ? Array.Empty<string>() : filtered[0].Parameters.Keys.ToArray();
			var names = CanvasCharts.DetectParamNames(paramKeys);
			CanvasCharts.Render(ChartCanvas, filtered, section, selector, valueLabel, names);

			// Hole visualization (uses the globally critical row for the selected section)
			var criticalRow = filtered.OrderByDescending(selector).FirstOrDefault();
			string critSubtitle = criticalRow == null ? "" : $"{valueLabel} = {selector(criticalRow):F3}";
			CanvasCharts.RenderHole(HoleCanvas, criticalRow, names, critSubtitle);

			// LiveCharts title — shared wording with Canvas Panel 1
			LiveChartTitle = CanvasCharts.CriticalByLcTitle(section);
			OnPropertyChanged(nameof(LiveChartTitle));

			// ───────── Critical info cards (Plate / Weld / Bolt) ─────────
			UpdateCriticalCard(PlateCritLabel, filtered, r => r.MaxPlateStress,
				"Stress", r => r.MaxStressPlateName, names);
			UpdateCriticalCard(WeldCritLabel, filtered, r => r.MaxWeldUtilization,
				"Utilization", r => r.CriticalWeldName, names);
			UpdateCriticalCard(BoltCritLabel, filtered, r => r.MaxBoltUtilizationIterationScope,
				"Utilization", r => r.CriticalBoltNameIterationScope, names);

			// LiveCharts
			var (series, xAxes, yAxes) = LiveChartsBuilder.BuildCriticalByLoadCase(filtered, selector, valueLabel);
			LiveChartSeries = series;
			LiveChartXAxes = xAxes;
			LiveChartYAxes = yAxes;
			OnPropertyChanged(nameof(LiveChartSeries));
			OnPropertyChanged(nameof(LiveChartXAxes));
			OnPropertyChanged(nameof(LiveChartYAxes));

			// Raw rows grid: project to a flattened anonymous type so AutoGenerateColumns works nicely
			RowsGrid.ItemsSource = filtered.Select(r => new
			{
				r.ConnectionId,
				r.ConnectionName,
				r.Iteration,
				Params = r.ParameterLabel,
				r.OverallPassed,
				r.LoadCaseId,
				r.MaxSummaryUtilization,
				r.CriticalSummaryCheckName,
				r.MaxPlateUtilization,
				r.MaxPlateStress,
				r.MaxPlateStrain,
				r.CriticalPlateUtilizationName,
				r.MaxWeldUtilization,
				r.CriticalWeldName,
				r.MaxBoltUtilizationIterationScope,
				r.CriticalBoltNameIterationScope,
			}).ToList();
		}

		// ──────────────────────────── LiveCharts binding surface ────────────────────────────

		public ISeries[] LiveChartSeries { get; set; } = Array.Empty<ISeries>();
		public ICartesianAxis[] LiveChartXAxes { get; set; } = Array.Empty<ICartesianAxis>();
		public ICartesianAxis[] LiveChartYAxes { get; set; } = Array.Empty<ICartesianAxis>();
		public string LiveChartTitle { get; set; } = "";

		private static void UpdateCriticalCard(
			System.Windows.Controls.TextBlock label,
			IReadOnlyList<LoadCaseRow> rows,
			Func<LoadCaseRow, double> selector,
			string valueName,
			Func<LoadCaseRow, string> detailName,
			CanvasCharts.ParamNames names)
		{
			var top = rows.OrderByDescending(selector).FirstOrDefault();
			if (top == null || selector(top) <= 0) { label.Text = "—"; return; }

			string paramBit = "";
			if (names.Width != null || names.Depth != null || names.Rotation != null)
			{
				var parts = new List<string>();
				if (names.Width != null && top.Parameters.TryGetValue(names.Width, out var w))
					parts.Add($"{names.Width}={w:F3}");
				if (names.Depth != null && top.Parameters.TryGetValue(names.Depth, out var d))
					parts.Add($"{names.Depth}={d:F3}");
				if (names.Rotation != null && top.Parameters.TryGetValue(names.Rotation, out var r))
				{
					double deg = Math.Abs(r) < Math.PI * 2 ? r * 180.0 / Math.PI : r;
					parts.Add($"{names.Rotation}={deg:F1}°");
				}
				if (parts.Count > 0) paramBit = string.Join(" / ", parts);
			}

			label.Text =
				$"{valueName}: {selector(top):F3}\n" +
				$"LC: {top.LoadCaseId?.ToString() ?? "—"}\n" +
				(string.IsNullOrEmpty(paramBit) ? "" : $"{paramBit}\n") +
				$"item: {detailName(top)}";
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string? name = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
