using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;
using SearchOption = System.IO.SearchOption;

namespace PublishBulkTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Reported to the service on every call, so usage of the API by this tool can be told apart
		/// from every other caller's. See <see cref="ClientApplicationIdentity"/>.
		/// </summary>
		private const string ClientApplicationName = "PublishBulkTool";

		private static readonly string ClientApplicationVersion =
			Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";

		private string? ideaPath;
		private string? selectedFolderPath;
		private ObservableCollection<ProjectItem> projectFiles = new();

		private ConnectionApiServiceRunner? service;

		public MainWindow()
		{
			InitializeComponent();
			ProjectsList.ItemsSource = projectFiles;

			ideaPath = IdeaInstallation.FindNewest();
			IdeaPathText.Text = ideaPath ?? string.Empty;
			if (ideaPath is null)
			{
				ShowStatus($"No IDEA StatiCa installation containing {IdeaInstallation.ServiceExecutableName} " +
					"was found. Use 'Set IDEA StatiCa API Path' to pick one.");
			}

			this.Closed += MainWindow_Closed;
		}

		private void MainWindow_Closed(object? sender, EventArgs e)
		{
			StopService();
		}

		private void LoadIdeaPath_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog()
			{
				Description = "Select the IDEA StatiCa installation folder",
				SelectedPath = ideaPath ?? string.Empty
			};

			if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}

			if (!IdeaInstallation.Contains(dialog.SelectedPath))
			{
				ShowError("Wrong installation folder",
					$"'{dialog.SelectedPath}' does not contain {IdeaInstallation.ServiceExecutableName}." +
					Environment.NewLine +
					"Pick the folder of an installed IDEA StatiCa version, for example " +
					@"C:\Program Files\IDEA StatiCa\StatiCa 26.1.");
				return;
			}

			// The path chosen here is what the service runner launches, so a service already running
			// from the previous path is no longer the one that was asked for.
			StopService();

			ideaPath = dialog.SelectedPath;
			IdeaPathText.Text = ideaPath;
			ShowStatus(string.Empty);
		}

		private void SelectFolder_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog()
			{
				Description = "Select folder with projects to be published",
				SelectedPath = selectedFolderPath ?? string.Empty // Default to last selected folder
			};

			if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}

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

			ShowStatus($"{files.Length} project(s) found.");
			LoadItemsButton.IsEnabled = files.Length > 0;
			PublishButton.IsEnabled = false;
		}

		private async void PublishAll_Click(object sender, RoutedEventArgs e)
		{
			// An unhandled exception in an 'async void' handler terminates the process, which is how
			// this tool used to answer a service that would not start (work item 37164). Everything
			// these handlers do is therefore inside a try/catch that reports instead.
			LoadItemsButton.IsEnabled = false;
			PublishButton.IsEnabled = false;

			foreach (var project in projectFiles)
			{
				foreach (var conn in project.Connections)
				{
					conn.IsReadOnly = true;
				}

				project.IsProcessed = false;
				project.IsFailed = false;
				project.Status = string.Empty;
			}

			try
			{
				var designSetType = ((ComboBoxItem)CompanyTypeComboBox.SelectedItem)?.Content?.ToString() switch
				{
					"Private set" => ConDesignSetType.Private,
					_ => ConDesignSetType.Company
				};

				using (var conClient = await CreateClientAsync())
				{
					int published = 0;
					foreach (var file in projectFiles)
					{
						ShowStatus($"Publishing {file.FileName}...");
						ConProject? project = null;
						try
						{
							project = await conClient.Project.OpenProjectAsync(file.FilePath);

							foreach (var connection in file.Connections)
							{
								var publishParams = new ConTemplatePublishParam
								{
									DesignSetType = designSetType,
									Name = connection.Name
								};

								await conClient.ConnectionLibrary.PublishConnectionAsync(
									project.ProjectId, connection.ConnectionId, publishParams);
								published++;
							}

							file.IsProcessed = true;
							file.IsFailed = false;
						}
						catch (Exception ex)
						{
							// One unpublishable project must not stop the batch, but it must say why -
							// the previous 'catch { }' left a red cross and no reason at all.
							file.IsProcessed = false;
							file.IsFailed = true;
							file.Status = Describe(ex);
						}
						finally
						{
							if (project != null)
							{
								try
								{
									await conClient.Project.CloseProjectAsync(project.ProjectId);
								}
								catch (Exception ex)
								{
									file.Status = string.IsNullOrEmpty(file.Status)
										? $"Project could not be closed: {Describe(ex)}"
										: file.Status;
								}
							}
						}
					}

					int failed = projectFiles.Count(p => p.IsFailed);
					ShowStatus($"Published {published} connection(s). {failed} project(s) failed.");
				}
			}
			catch (Exception ex)
			{
				ReportFailure("Publishing failed", ex);
			}
			finally
			{
				// The service is kept running between operations; it is stopped when the window closes.
				LoadItemsButton.IsEnabled = true;
				PublishButton.IsEnabled = projectFiles.Any(p => p.Connections.Count > 0);
			}
		}

		private async void ProcessFiles_Click(object sender, RoutedEventArgs e)
		{
			LoadItemsButton.IsEnabled = false;
			PublishButton.IsEnabled = false;

			try
			{
				using (var conClient = await CreateClientAsync())
				{
					foreach (var file in projectFiles)
					{
						ShowStatus($"Reading {file.FileName}...");
						file.Connections.Clear();
						file.IsProcessed = false;
						file.IsFailed = false;
						file.Status = string.Empty;

						try
						{
							var project = await conClient.Project.OpenProjectAsync(file.FilePath);
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
						catch (Exception ex)
						{
							// A single unreadable project used to abort the whole load.
							file.IsFailed = true;
							file.Status = Describe(ex);
						}
					}

					ShowStatus($"Loaded {projectFiles.Sum(p => p.Connections.Count)} connection(s) " +
						$"from {projectFiles.Count(p => !p.IsFailed)} project(s).");
				}
			}
			catch (Exception ex)
			{
				ReportFailure("Loading project items failed", ex);
			}
			finally
			{
				LoadItemsButton.IsEnabled = true;
				PublishButton.IsEnabled = projectFiles.Any(p => p.Connections.Count > 0);
			}
		}

		/// <summary>
		/// Starts the Connection API service if it is not running yet and connects a client to it.
		/// </summary>
		private async Task<IConnectionApiClient> CreateClientAsync()
		{
			if (!IdeaInstallation.Contains(ideaPath))
			{
				throw new InvalidOperationException(
					string.IsNullOrEmpty(ideaPath)
						? "No IDEA StatiCa installation is selected. Use 'Set IDEA StatiCa API Path'."
						: $"'{ideaPath}' does not contain {IdeaInstallation.ServiceExecutableName}. " +
						  "Use 'Set IDEA StatiCa API Path' to pick an installed version.");
			}

			if (service is null)
			{
				service = new ConnectionApiServiceRunner(ideaPath, ClientApplicationName, ClientApplicationVersion);
			}

			ShowStatus("Starting the IDEA StatiCa Connection API service...");
			return await service.CreateApiClient();
		}

		private void StopService()
		{
			// Dispose kills the service process. Clearing the field matters: a disposed runner left in
			// place would be reused on the next click and hand out a URL nothing is listening on.
			service?.Dispose();
			service = null;
		}

		private void ReportFailure(string title, Exception ex)
		{
			// A failed start leaves the runner holding no usable service; drop it so the next attempt
			// starts a fresh one instead of reusing the broken state.
			StopService();
			ShowStatus($"{title}: {Describe(ex)}");
			ShowError(title, ex.ToString());
		}

		private void ShowStatus(string message)
		{
			MessageLabel.Text = message;
		}

		private static void ShowError(string title, string detail)
		{
			MessageBox.Show(detail, title, MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		private static string Describe(Exception ex)
		{
			return ex.GetBaseException().Message;
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

		private string _status = "";
		/// <summary>
		/// Why this project failed, shown next to it in the list. Empty when it did not.
		/// </summary>
		public string Status
		{
			get => _status;
			set
			{
				_status = value;
				OnPropertyChanged(nameof(Status));
				OnPropertyChanged(nameof(HasStatus));
			}
		}

		public bool HasStatus => !string.IsNullOrEmpty(_status);

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
