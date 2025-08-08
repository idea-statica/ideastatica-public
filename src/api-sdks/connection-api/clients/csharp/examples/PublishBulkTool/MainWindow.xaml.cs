using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace PublishBulkTool
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
			IdeaPathText.Text = ideaPath = @"C:\Program Files\IDEA StatiCa\StatiCa 25.0\";
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
				SelectedPath = @"C:\Program Files\IDEA StatiCa\StatiCa 25.0\"
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
				Description = "Select folder with projects to be published",
				SelectedPath = selectedFolderPath // Default to last selected folder
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

		private async void PublishAll_Click(object sender, RoutedEventArgs e)
		{
			LoadItemsButton.IsEnabled = false;
			PublishButton.IsEnabled = false;

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
						project = await conClient.Project.OpenProjectAsync(file.FilePath);

						var publishParams = new ConTemplatePublishParam
						{
							Author = AuthorTextBox.Text,
							CompanyName = CompanyTextBox.Text
						};

						foreach (var connection in file.Connections)
						{
							publishParams.Name = connection.Name;
							await conClient.Template.PublishConnectionAsync(project.ProjectId, connection.ConnectionId, publishParams);
						}

						// Mark as success
						file.IsProcessed = true;
						file.IsFailed = false;
					}
					catch
					{
						// Mark as success
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

			LoadItemsButton.IsEnabled = true;
			PublishButton.IsEnabled = true;
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
			PublishButton.IsEnabled = true;
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