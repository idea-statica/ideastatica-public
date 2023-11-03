using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.RcsClient.Client;
using IdeaStatiCa.RcsClient.Factory;
using IdeaStatiCa.RcsClient.HttpWrapper;
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
		
			ApplicationInformation.Text = controller != null ? $"{ideaStatiCaDir}/IdeaStatiCa.RcsRestApi.exe.exe" : "API is not running";

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
				NonConformities = new List<Guid> 
				{
					Guid.Parse("81a0b61d-09a3-4a16-9cfc-80138a778743"),
					Guid.Parse("0934a621-039c-4397-8fec-382a780b98c0"),
					Guid.Parse("2f9571d5-85cb-4643-a6e3-7ca6e9e5c460")
				}
			};

			var result = await Task.Run(() => controller.GetNonConformityIssues(projectInfo));

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
				NonConformities = new List<Guid>
				{
					Guid.Parse("81a0b61d-09a3-4a16-9cfc-80138a778743"),
					Guid.Parse("0934a621-039c-4397-8fec-382a780b98c0"),
					Guid.Parse("2f9571d5-85cb-4643-a6e3-7ca6e9e5c460")
				}
			};

			var result = await Task.Run(() => controller.GetResultOnSections(projectInfo));

			if (result is { })
			{
				CalculationResult.Text = FormatJson(JsonConvert.SerializeObject(result));
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
	}
}
