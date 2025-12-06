using CalculationBulkTool;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

namespace CalculationBulkTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// Plan (pseudocode):
		// - Add Serilog using and create a logger for this class using Log.ForContext<MainWindow>().
		// - Log lifecycle: constructor start/end, window closed disposal.
		// - Log actions: selecting IDEA path, selecting folder, processing and calculating files, saving CSV.
		// - Log counts and file paths involved.
		// - Log exceptions with context and stack traces.
		// - Keep code changes minimal and non-invasive to existing logic.

		private readonly ILogger _logger = Log.ForContext<MainWindow>();

		private string? ideaPath;
		private string? selectedFolderPath;
		private ObservableCollection<ProjectItem> projectFiles = new();
		private IConnectionApiClient? conClient;

		private ConnectionApiServiceRunner? service;

		public MainWindow()
		{
			_logger.Information("Initializing MainWindow");
			InitializeComponent();
			ProjectsList.ItemsSource = projectFiles;
			IdeaPathText.Text = ideaPath = @"C:\Program Files\IDEA StatiCa\StatiCa 25.1\";
			this.Closed += MainWindow_Closed;
			_logger.Information("MainWindow initialized with default IDEA path: {IdeaPath}", ideaPath);
		}

		private void MainWindow_Closed(object? sender, EventArgs e)
		{
			_logger.Information("MainWindow closed. Disposing services.");
			try
			{
				service?.Dispose();
				conClient?.Dispose();
				_logger.Information("Services disposed.");
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error disposing services on window close");
			}
		}

		private void LoadIdeaPath_Click(object sender, RoutedEventArgs e)
		{
			_logger.Information("Loading IDEA path via folder selection dialog.");
			var dialog = new System.Windows.Forms.FolderBrowserDialog()
			{
				Description = "Select folder where is API v25",
				SelectedPath = @"C:\Program Files\IDEA StatiCa\StatiCa 25.1\"
			};

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				ideaPath = dialog.SelectedPath;
				IdeaPathText.Text = ideaPath;  // Show selected path in the TextBlock
				_logger.Information("IDEA path selected: {IdeaPath}", ideaPath);
			}
			else
			{
				_logger.Information("IDEA path selection canceled.");
			}
		}

		private void SelectFolder_Click(object sender, RoutedEventArgs e)
		{
			_logger.Information("Selecting folder with projects to be calculated.");
			var dialog = new System.Windows.Forms.FolderBrowserDialog()
			{
				Description = "Select folder with projects to be calculated",
				SelectedPath = selectedFolderPath! // Default to last selected folder
			};

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				selectedFolderPath = dialog.SelectedPath;
				SelectedFolderTextBox.Text = selectedFolderPath;  // Show selected folder in the read-only TextBox

				_logger.Information("Selected folder: {SelectedFolder}", selectedFolderPath);

				// Clear and repopulate the ListBox with .ideaCon files
				projectFiles.Clear();

				try
				{
					var files = Directory
						.GetFiles(selectedFolderPath!, "*.ideaCon", SearchOption.AllDirectories)
						.OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
						.ToArray();

					_logger.Information("Found {Count} .ideaCon files under {Folder}", files.Length, selectedFolderPath);

					foreach (var file in files)
					{
						var fileName = System.IO.Path.GetFileName(file);
						projectFiles.Add(new ProjectItem { FilePath = file, FileName = fileName });
					}

					LoadItemsButton.IsEnabled = true;
				}
				catch (Exception ex)
				{
					_logger.Error(ex, "Error while enumerating .ideaCon files in {Folder}", selectedFolderPath);
					MessageBox.Show("Error loading project files. See logs for details.");
				}
			}
			else
			{
				_logger.Information("Project folder selection canceled.");
			}
		}

		private void SaveCsvToFile(CalculationResults results, string fileName)
		{
			_logger.Information("Saving CSV to file: {FileName} for project {Project}", fileName, results.FileName);
			bool fileExists = File.Exists(fileName);

			var csv = CsvExporter.ConvertToCsv(results, !fileExists);

			try
			{
				File.AppendAllLines(fileName, [csv]);
				_logger.Information("CSV appended to {FileName}. HeaderIncluded={HeaderIncluded}, ItemsCount={ItemsCount}",
					fileName,
					!fileExists,
					results.ProjectItems.Count);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Unexpected error while saving CSV to {FileName}", fileName);
				throw;
			}
		}

		private async void CalculateAll_Click(object sender, RoutedEventArgs e)
		{
			_logger.Information("Starting CalculateAll for {Count} projects", projectFiles.Count);
			LoadItemsButton.IsEnabled = false;
			CalculateButton.IsEnabled = false;

			foreach (var project in projectFiles)
			{
				foreach (var conn in project.Connections)
				{
					conn.IsReadOnly = true;
				}
			}

			if (service is null)
			{
				_logger.Information("Creating ConnectionApiServiceRunner with IDEA path: {IdeaPath}", ideaPath);
				service = new ConnectionApiServiceRunner(ideaPath);
			}

			using (var conClient = await service.CreateApiClient())
			{
				var fileName = Path.Combine(selectedFolderPath!, $"Export-{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.csv");
				var failedProjects = Path.Combine(selectedFolderPath!, $"FailedProjects-{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt");
				_logger.Information("Calculation export file: {ExportFile}, Failed list: {FailedFile}", fileName, failedProjects);

				MessageBox.Show($"Calculation starts. Export will be in the same folder: {fileName}");

				foreach (var file in projectFiles)
				{
					ConProject? project = null;
					try
					{
						_logger.Information("Opening project: {ProjectFile}", file.FilePath);
						CalculationResults results = new CalculationResults();
						project = await conClient.Project.OpenProjectAsync(file.FilePath);
						results.FileName = file.FileName;
						var connectionIds = file.Connections.Select(x => x.ConnectionId).ToList();

						_logger.Information("Project {Project} opened. {Count} connection(s) to process",
							file.FileName, connectionIds.Count);

						foreach (var connectionId in connectionIds)
						{
							try
							{
								_logger.Information("Calculating connectionId={ConnectionId} in projectId={ProjectId}", connectionId, project.ProjectId);
								var briefResults = await conClient.Calculation.CalculateAsync(project.ProjectId, new List<int> { connectionId });
								if (briefResults[0].Passed)
								{
									_logger.Information("ConnectionId={ConnectionId} passed.", connectionId);
									var connection = new Connection
									{
										Name = project.Connections.First(x => x.Id == connectionId).Name,
										AnalysisType = $"{project.Connections.First(x => x.Id == connectionId).AnalysisType}",
										Analysis = briefResults[0].ResultSummary.FirstOrDefault(x => x.Name == "Analysis")?.CheckValue ?? 0,
										Bolts = briefResults[0].ResultSummary.FirstOrDefault(x => x.Name == "Bolts")?.CheckValue ?? 0,
										Welds = briefResults[0].ResultSummary.FirstOrDefault(x => x.Name == "Welds")?.CheckValue ?? 0,
										Plates = briefResults[0].ResultSummary.FirstOrDefault(x => x.Name == "Plates")?.CheckValue ?? 0,
										PreloadedBolts = briefResults[0].ResultSummary.FirstOrDefault(x => x.Name == "Preloaded bolts")?.CheckValue ?? 0,
									};

									connection.Succes = connection.Bolts <= 100;

									var allResults = await conClient.Calculation.GetRawJsonResultsAsync(project.ProjectId, new ConCalculationParameter
									{
										ConnectionIds = [connectionId],
										AnalysisType = project.Connections.First(x => x.Id == connectionId).AnalysisType
									});

									JObject obj = JObject.Parse(allResults[0]);

									var bolts = (JObject?)obj["bolts"];
									foreach (var kvp in bolts!)
									{
										string key = kvp.Key;
										var bolt = (JObject?)kvp.Value;

										try
										{
											if (bolt is { })
											{
												connection.BoltResults.Add(new BoltResults
												{
													Item = bolt["name"]?.ToString(),
													UtilizationInInteraction = double.Parse(bolt["interactionTensionShear"]?.ToString() ?? "0"),
													UtilizationInShear = double.Parse(bolt["unityCheckShearMax"]?.ToString() ?? "0"),
													UtilizationInTension = double.Parse(bolt["unityCheckTension"]?.ToString() ?? "0")
												});
											}
										}
										catch (Exception boltEx)
										{
											_logger.Warning(boltEx, "Bolt results parsing issue for connectionId={ConnectionId}", connectionId);
											Console.WriteLine("Bolt results were not parsed correctly");
										}
									}

									results.ProjectItems.Add(connection);
								}
								else
								{
									_logger.Warning("ConnectionId={ConnectionId} failed in project {Project}", connectionId, file.FileName);
									File.AppendAllLines(failedProjects, [$"{file.FileName} - {file.Connections.First(x => x.ConnectionId == connectionId).Name}"]);
								}
							}
							catch (Exception ex)
							{
								_logger.Error(ex, "Error calculating connectionId={ConnectionId} in project {Project}", connectionId, file.FileName);
								File.AppendAllLines(failedProjects, [$"{file.FileName} - {file.Connections.First(x => x.ConnectionId == connectionId).Name} - {ex.Message}"]);
							}
						}

						if (!results.ProjectItems.Any() || results.ProjectItems.Any(x => !x.Succes))
						{
							file.IsProcessed = false;
							_logger.Information("Project {Project} marked as NOT processed.", file.FileName);
						}
						else
						{
							file.IsProcessed = true;
							_logger.Information("Project {Project} processed successfully.", file.FileName);
						}

						SaveCsvToFile(results, fileName);
					}
					catch (Exception ex)
					{
						file.IsProcessed = false;
						_logger.Error(ex, "Unexpected error while processing project {ProjectFile}", file.FilePath);
					}
					finally
					{
						if (project != null)
						{
							_logger.Information("Closing projectId={ProjectId}", project.ProjectId);
							await conClient.Project.CloseProjectAsync(project.ProjectId);
						}
					}
				}
			}

			try
			{
				service?.Dispose();
				conClient?.Dispose();
				_logger.Information("Disposed service and client after calculation.");
			}
			catch (Exception ex)
			{
				_logger.Error(ex, "Error disposing service/client after calculation");
			}

			LoadItemsButton.IsEnabled = true;
			CalculateButton.IsEnabled = true;
			MessageBox.Show("Process is finished");
			_logger.Information("CalculateAll finished.");
		}

		private async void ProcessFiles_Click(object sender, RoutedEventArgs e)
		{
			_logger.Information("Processing files to load connections. Files count: {Count}", projectFiles.Count);
			LoadItemsButton.IsEnabled = false;

			if (service is null)
			{
				_logger.Information("Creating ConnectionApiServiceRunner with IDEA path: {IdeaPath}", ideaPath);
				service = new ConnectionApiServiceRunner(ideaPath);
			}

			using (var conClient = await service.CreateApiClient())
			{
				foreach (var file in projectFiles)
				{
					try
					{
						_logger.Information("Opening project: {ProjectFile}", file.FilePath);
						var project = await conClient.Project.OpenProjectAsync(file.FilePath);
						file.Connections.Clear();
						foreach (var connection in project.Connections)
						{
							file.Connections.Add(new ConnectionItem
							{
								ConnectionId = connection.Id,
								Name = connection.Name
							});
						}
						_logger.Information("Loaded {Count} connections for project {Project}", file.Connections.Count, file.FileName);

						await conClient.Project.CloseProjectAsync(project.ProjectId);
						_logger.Information("Closed project {Project}", file.FileName);
					}
					catch (Exception ex)
					{
						_logger.Error(ex, "Error loading connections for project {ProjectFile}", file.FilePath);
					}
				}
			}

			LoadItemsButton.IsEnabled = true;
			CalculateButton.IsEnabled = true;
			_logger.Information("ProcessFiles completed. Buttons re-enabled.");
		}
	}

	public class ProjectItem : INotifyPropertyChanged
	{
		public string? FilePath { get; set; }

		public string? FileName { get; set; }

		private bool _isProcessed;
		public bool IsProcessed
		{
			get => _isProcessed;
			set
			{
				_isProcessed = value;
				OnPropertyChanged(nameof(IsProcessed));
			}
		}

		private bool _isFailed;
		public bool IsFailed
		{
			get => _isFailed;
			set
			{
				_isFailed = value;
				OnPropertyChanged(nameof(IsFailed));
			}
		}

		public ObservableCollection<ConnectionItem> Connections { get; set; } = new();

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

	public class ConnectionItem : INotifyPropertyChanged
	{
		public int ConnectionId { get; set; }

		private string _name = "";
		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		private bool _isReadOnly;
		public bool IsReadOnly
		{
			get => _isReadOnly;
			set
			{
				_isReadOnly = value;
				OnPropertyChanged(nameof(IsReadOnly));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}