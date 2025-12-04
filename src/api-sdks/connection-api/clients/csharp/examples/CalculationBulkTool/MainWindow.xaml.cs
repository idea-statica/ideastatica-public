using CalculationBulkTool;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using Newtonsoft.Json.Linq;
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
		private string? ideaPath;
		private string? selectedFolderPath;
		private ObservableCollection<ProjectItem> projectFiles = new();
		private IConnectionApiClient? conClient;

		private ConnectionApiServiceRunner? service;

		public MainWindow()
		{
			InitializeComponent();
			ProjectsList.ItemsSource = projectFiles;
			IdeaPathText.Text = ideaPath = @"C:\Program Files\IDEA StatiCa\StatiCa 25.1\";
			this.Closed += MainWindow_Closed;
		}

		private void MainWindow_Closed(object? sender, EventArgs e)
		{
			service?.Dispose();
			conClient?.Dispose();
		}

		private void LoadIdeaPath_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog()
			{
				Description = "Select folder where is API v25",
				SelectedPath = @"C:\Program Files\IDEA StatiCa\StatiCa 25.1\"
			};

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				ideaPath = dialog.SelectedPath;
				IdeaPathText.Text = ideaPath;  // Show selected path in the TextBlock
			}
		}


		private void SelectFolder_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog()
			{
				Description = "Select folder with projects to be calculated",
				SelectedPath = selectedFolderPath! // Default to last selected folder
			};

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				selectedFolderPath = dialog.SelectedPath;
				SelectedFolderTextBox.Text = selectedFolderPath;  // Show selected folder in the read-only TextBox

				// Clear and repopulate the ListBox with .ideaCon files
				projectFiles.Clear();

				var files = Directory
						.GetFiles(selectedFolderPath!, "*.ideaCon", SearchOption.AllDirectories)
						.OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase)
						.ToArray();

				foreach (var file in files)
				{
					var fileName = System.IO.Path.GetFileName(file);
					projectFiles.Add(new ProjectItem { FilePath = file, FileName = fileName });
				}

				LoadItemsButton.IsEnabled = true;
			}
		}

		private void SaveCsvToFile(CalculationResults results, string fileName)
		{
			bool fileExists = File.Exists(fileName);

			var csv = CsvExporter.ConvertToCsv(results, !fileExists);

			try
			{
				File.AppendAllLines(fileName, [csv]);
			}
			catch (IOException)
			{
				MessageBox.Show("Cannot save because the file is open in another application. Please close it and try again.");
			}
		}

		private async void CalculateAll_Click(object sender, RoutedEventArgs e)
		{
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
				service = new ConnectionApiServiceRunner(ideaPath);
			}

			using (var conClient = await service.CreateApiClient())
			{
				var fileName = Path.Combine(selectedFolderPath!, $"Export-{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.csv");
				var failedProjects = Path.Combine(selectedFolderPath!, $"FailedProjects-{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.txt");
				MessageBox.Show($"Calculation starts. Export will be in the same folder: {fileName}");

				foreach (var file in projectFiles)
				{
					ConProject? project = null;
					try
					{
						CalculationResults results = new CalculationResults();
						project = await conClient.Project.OpenProjectAsync(file.FilePath);
						results.FileName = file.FileName;
						var connectionIds = file.Connections.Select(x => x.ConnectionId).ToList();

						foreach (var connectionId in connectionIds)
						{
							try
							{
								var briefResults = await conClient.Calculation.CalculateAsync(project.ProjectId, new List<int> { connectionId });
								if (briefResults[0].Passed)
								{
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
										catch
										{
											Console.WriteLine("Bolt results were not parsed correctly");
										}
									}

									results.ProjectItems.Add(connection);
								}
								else
								{
									// failed project
									File.AppendAllLines(failedProjects, [$"{file.FileName} - {file.Connections.First(x => x.ConnectionId == connectionId).Name}"]);
								}
							}
							catch (Exception ex)
							{
								File.AppendAllLines(failedProjects, [$"{file.FileName} - {file.Connections.First(x => x.ConnectionId == connectionId).Name} - {ex.Message}"]);
							}
						}

						if (!results.ProjectItems.Any() || results.ProjectItems.Any(x => !x.Succes))
						{
							file.IsProcessed = false;
						}
						else
						{
							// Mark as success
							file.IsProcessed = true;
						}

						SaveCsvToFile(results, fileName);
					}
					catch
					{
						file.IsProcessed = false;
					}
					finally
					{
						if (project != null)
						{
							await conClient.Project.CloseProjectAsync(project.ProjectId);
						}
					}
				}
			}

			service?.Dispose();
			conClient?.Dispose();
			LoadItemsButton.IsEnabled = true;
			CalculateButton.IsEnabled = true;
			MessageBox.Show("Process is finished"); 
		}

		private async void ProcessFiles_Click(object sender, RoutedEventArgs e)
		{
			LoadItemsButton.IsEnabled = false;

			if (service is null)
			{
				service = new ConnectionApiServiceRunner(ideaPath);
			}

			using (var conClient = await service.CreateApiClient())
			{
				foreach (var file in projectFiles)
				{
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

					await conClient.Project.CloseProjectAsync(project.ProjectId);
				}
			}

			LoadItemsButton.IsEnabled = true;
			CalculateButton.IsEnabled = true;
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