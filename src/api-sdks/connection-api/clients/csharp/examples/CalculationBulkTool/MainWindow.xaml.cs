using CalculationBulkTool;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

				var files = Directory.GetFiles(selectedFolderPath!, "*.ideaCon", SearchOption.AllDirectories);
				foreach (var file in files)
				{
					var fileName = System.IO.Path.GetFileName(file);
					projectFiles.Add(new ProjectItem { FilePath = file, FileName = fileName });
				}

				LoadItemsButton.IsEnabled = true;
			}
		}

		private void ExportProjectCsv_Click(object sender, RoutedEventArgs e)
		{
			if (ProjectsList.SelectedItem is not ProjectItem selected)
			{
				MessageBox.Show("Select a project first.");
				return;
			}

			// Collect only this project's results
			var results = selected.CalculationResults; // store this from your CalculateAll method!

			if (results is null)
			{
				MessageBox.Show("This project has no results yet.");
				return;
			}

			SaveCsvToFile(new[] { selected });
		}

		private void ExportAllCsv_Click(object sender, RoutedEventArgs e)
		{
			var all = projectFiles
				.Where(p => p.CalculationResults is not null)
				.ToList();

			if (!all.Any())
			{
				MessageBox.Show("No processed results to export.");
				return;
			}

			SaveCsvToFile(all);
		}

		private void SaveCsvToFile(IEnumerable<ProjectItem> results)
		{
			var dialog = new Microsoft.Win32.SaveFileDialog
			{
				Filter = "CSV Files (*.csv)|*.csv",
				FileName = results.Count() == 1 ? $"{results.First().FileName} - results.csv" : "AllResults.csv"
			};

			if (dialog.ShowDialog() == true)
			{
				var csv = CsvExporter.ConvertToCsv(results.Select(x => x.CalculationResults!));
				File.WriteAllText(dialog.FileName, csv);
				MessageBox.Show("CSV exported successfully.");
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
							var briefResults = await conClient.Calculation.CalculateAsync(project.ProjectId, new List<int> { connectionId });
							var connection = new Connection
							{
								Name = project.Connections.First(x => x.Id == connectionId).Name,
								Analysis = briefResults[0].ResultSummary.First(x => x.Name == "Analysis").CheckValue,
								Bolts = briefResults[0].ResultSummary.First(x => x.Name == "Bolts").CheckValue,
							};

							connection.Succes = connection.Bolts <=100;

							var allResults = await conClient.Calculation.GetRawJsonResultsAsync(project.ProjectId, new ConCalculationParameter { ConnectionIds = new List<int> { connectionId } });
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

						file.CalculationResults = results;

						if (file.CalculationResults.ProjectItems.Any(x => !x.Succes))
						{		
							file.IsProcessed = false;
							file.IsFailed = true;
						}
						else
						{
							// Mark as success
							file.IsProcessed = true;
							file.IsFailed = false;
						}
					}
					catch
					{
						file.IsProcessed = false;
						file.IsFailed = true;
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

		public CalculationResults? CalculationResults { get; set; }

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