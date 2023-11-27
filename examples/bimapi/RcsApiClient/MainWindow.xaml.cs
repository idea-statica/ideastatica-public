using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using IdeaStatiCa.RcsClient.Factory;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RcsApiClient
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool isProjectFilled;
		private string ideaStatiCaDir = Properties.Settings.Default.IdeaStatiCaDir;

		private IRcsClientFactory rcsClientFactory; 
		private IRcsApiController controller;

		private IProgress<string> progressReporter;
		private IProgress<int> progressBarValue;
		private IProgress<string> apiHeartbeat;

		public bool IsProjectFilled
		{
			get { return isProjectFilled; }
			set
			{
				isProjectFilled = value;
				NotifyPropertyChanged(nameof(IsProjectFilled));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public MainWindow()
		{
			InitializeComponent();
			progressReporter = new Progress<string>(UpdateProgress);
			progressBarValue = new Progress<int>(UpdateProgressbar);
			apiHeartbeat= new Progress<string>(ApiHeartbeatUpdate);

			DataContext = this;
			ProjectFileInputPath.TextChanged += (sender, e) =>
			{
				IsProjectFilled = !string.IsNullOrWhiteSpace(ProjectFileInputPath.Text);
				if (controller is { } && IsProjectFilled)
				{
					GetResultOnSections.IsEnabled = true;
					GetNonConformityIssues.IsEnabled = true;
					GetProjectOverview.IsEnabled = true;
				}
			};

			var nullLogger = new IdeaStatiCa.Plugin.NullLogger();
			rcsClientFactory = new RcsClientFactory(nullLogger, httpClientWrapper: null, ideaStatiCaDir);
			rcsClientFactory.StreamingLog = (msg, progress) =>
			{
				Application.Current.Dispatcher.Invoke(() => UpdateProgress(msg, progress));
			};
			rcsClientFactory.HeartbeatLog = (msg) => 
			{
				Application.Current.Dispatcher.Invoke(() => { ApiHeartbeatUpdate(msg); });
			};
			controller = rcsClientFactory.CreateRcsApiClient();
		
			ApplicationInformation.Text = controller != null ? $"{ideaStatiCaDir}\\IdeaStatiCa.RcsRestApi.exe" : "API is not running";

			AppDomain.CurrentDomain.ProcessExit += ProcessExit;
		}

		private void ApiHeartbeatUpdate(string heartbeatMsg)
		{
			ApiTextBox.Text = $"{heartbeatMsg} at {DateTime.Now}";
		}

		private void UpdateProgressbar(int percentage)
		{
			ProgressBar.Value = percentage;
		}

		private void UpdateProgress(string msg)
		{
			ProgressLog.Text = msg;
		}

		private void UpdateProgress(string msg, int percentage)
		{
			UpdateProgress(msg);
			UpdateProgressbar(percentage);
		}

		private async void GetNonConformityIssues_Click(object sender, RoutedEventArgs e)
		{
			CalculationResult.Text = "";
			UpdateProgress("", 0);
			var projectInfo = new RcsProjectInfo
			{
				IdeaProjectPath = ProjectFileInputPath.Text,
			};

			var result = await Task.Run(() => controller.GetNonConformityIssues(projectInfo, CancellationToken.None));

			if (result is { })
			{
				CalculationResult.Text = FormatJson(JsonConvert.SerializeObject(result));
			}
			else
			{
				MessageBox.Show($"Request failed.");
			}
		}

		private async void GetResultOnSections_Click(object sender, RoutedEventArgs e)
		{
			CalculationResult.Text = "";
			UpdateProgress("", 0);
			var projectInfo = new RcsProjectInfo
			{
				IdeaProjectPath = ProjectFileInputPath.Text,
			};

			var selectedSections = new List<int>();
			foreach(var selectedSection in MultiSelectListBox.SelectedItems)
			{
				selectedSections.Add(int.Parse(selectedSection.ToString()));
			}

			projectInfo.Sections = selectedSections;

			var result = await controller.CalculateProjectAsync(projectInfo, CancellationToken.None);

			if (result is { })
			{
				CalculationResult.Text = FormatJson(JsonConvert.SerializeObject(result.Sections));
			}
			else
			{
				MessageBox.Show($"Request failed.");
			}
		}

		private string FormatJson(string json)
		{
			try
			{
				JToken parsedJson = JToken.Parse(json);
				return parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);
			}
			catch (Exception)
			{
				return "Invalid JSON format.";
			}
		}

		private void BrowseForProject_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			// Set properties for the OpenFileDialog
			openFileDialog.Title = "Select a Project File";
			openFileDialog.Filter = "IdeaRcs Files(*.IdeaRcs)| *.IdeaRcs";

			// Show the file dialog and get the selected file
			if (openFileDialog.ShowDialog() == true)
			{
				string selectedFilePath = openFileDialog.FileName;

				// Do something with the selected file path (e.g., display it in a TextBox)
				ProjectFileInputPath.Text = selectedFilePath;
			}
		}

		private void ProcessExit(object? sender, EventArgs e)
		{
			controller.Dispose();
		}

		private async void GetProjectOverview_Click(object sender, RoutedEventArgs e)
		{
			MultiSelectListBox.Items.Clear();

			var projectInfo = new RcsProjectInfo
			{
				IdeaProjectPath = ProjectFileInputPath.Text
			};

			var result = await Task.Run(() => controller.GetProjectOverview(projectInfo, CancellationToken.None));
			if (result is { })
			{
				foreach(var section in result.Sections)
				{
					MultiSelectListBox.Items.Add(section.Id);
				}

				CalculationResult.Text = FormatJson(JsonConvert.SerializeObject(result));
			}
			else
			{
				MessageBox.Show($"Request failed.");
			}
		}
	}
}
