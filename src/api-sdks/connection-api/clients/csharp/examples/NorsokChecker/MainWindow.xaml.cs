using System.Collections.ObjectModel;
using System.ComponentModel;
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
			var timestamp = DateTime.Now.ToString("HH:mm:ss");
			LogBox.AppendText($"[{timestamp}] {message}\n");
			LogBox.ScrollToEnd();
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

				if (RbSpawn.IsChecked == true)
				{
					var setupDir = TxtApiPath.Text.Trim();
					_runner ??= new ConnectionApiServiceRunner(setupDir);
					_apiClient = await _runner.CreateApiClient();
				}
				else
				{
					var url = TxtApiPath.Text.Trim();
					_apiClient = await new ConnectionApiServiceAttacher(url).CreateApiClient();
				}

				Log($"Opening project: {Path.GetFileName(projectPath)}");
				Log("API ready.");
				var project = await _apiClient.Project.OpenProjectAsync(projectPath);
				_projectId = project.ProjectId;
				Log($"Project opened. ID = {_projectId}");

				var connections = project.Connections ?? new();
				_connections.Clear();

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

			try
			{
				BtnRunCheck.IsEnabled = false;
				Log("Starting Norsok M-001 compliance check...");

				var checker = new NorsokCheckRunner(_apiClient, _projectId, Log);

				foreach (var con in _connections)
				{
					con.Status = "Calculating...";
					Log($"  Calculating connection: {con.Name}");

					var result = await checker.CheckConnectionAsync(con.Id);

					con.MaxUtilization = result.MaxUtilization;
					con.NorsokPass = result.Passed ? "PASS" : "FAIL";
					con.Status = result.Passed ? "Passed" : "Failed";

					Log($"  {con.Name}: utilization={result.MaxUtilization:F3}, {con.NorsokPass}");
				}

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
