using CommunityToolkit.Mvvm.ComponentModel;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using System.Windows.Controls;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Threading;

namespace RcsApiClient.ViewModels
{
	public class MainWindowViewModel : ObservableObject
	{
		private readonly IPluginLogger pluginLogger;
		private readonly IRcsClientFactory rcsClientFactory;
		private IRcsApiController controller;

		public MainWindowViewModel(IPluginLogger pluginLogger, IRcsClientFactory rcsClientFactory)
		{
			this.pluginLogger = pluginLogger;
			this.rcsClientFactory = rcsClientFactory;

			OpenProjectCmdAsync = new AsyncRelayCommand(OpenProjectAsync, CanOpenProject);

			rcsClientFactory.StreamingLog = (msg, progress) =>
			{
				System.Windows.Application.Current.Dispatcher.Invoke(() => UpdateProgress(msg, progress));
			};

			rcsClientFactory.HeartbeatLog = (msg) =>
			{
				System.Windows.Application.Current.Dispatcher.Invoke(() => { ApiHeartbeatUpdate(msg); });
			};

			CreateClientAsync();
		}

		public IAsyncRelayCommand OpenProjectCmdAsync
		{
			get;
			private set;
		}

		
		private bool CanOpenProject()
		{
			return (this.Controller != null) && (string.IsNullOrEmpty(RcsProjectPath));
		}


		private async Task OpenProjectAsync()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			// Set properties for the OpenFileDialog
			openFileDialog.Title = "Select a Project File";
			openFileDialog.Filter = "IDEARCS Files (*.idearcs)|*.idearcs|XML Files (*.xml)|*.xml";

			try
			{
				// Show the file dialog and get the selected file
				if (openFileDialog.ShowDialog() == true)
				{
					string selectedFilePath = openFileDialog.FileName;

					ApiMessage = "Opening RCS project";
					OpenedProjectId = await controller.OpenProjectAsync(selectedFilePath, CancellationToken.None);
					this.RcsProjectPath = selectedFilePath;
					//MultiSelectListBox.Items.Clear();
					CalculationResult = $"Project is opened with ID '{OpenedProjectId}'";
				}
			}
			catch(Exception ex)
			{
				ApiMessage = ex.Message;
			}

;
		}

		private async void CreateClientAsync()
		{
			ApiMessage = "Starting RCS Service";
			var x = await rcsClientFactory.CreateRcsApiClient();
			this.Controller = x;
			ApiMessage = "RCS Service is running";
		}

		private Guid openedProjectId;
		public Guid OpenedProjectId
		{
			get => openedProjectId;
			set
			{
				openedProjectId = value;
				OnPropertyChanged(nameof(OpenedProjectId));
			}
		}

		private string calculationResult;
		public string CalculationResult
		{
			get => calculationResult;
			set
			{
				calculationResult = value;
				OnPropertyChanged(nameof(CalculationResult));
			}
		}

		private string apiMessage;

		public string ApiMessage
		{
			get => apiMessage;
			set
			{ 
				apiMessage = value;
				OnPropertyChanged(nameof(ApiMessage));
			}
		}

		private double progress;
		public double Progress
		{
			get => progress;
			set
			{
				progress = value;
				OnPropertyChanged(nameof(Progress));
			}
		}

		private string progressMsg;
		public string ProgressMsg
		{
			get => progressMsg;
			set
			{
				progressMsg = value;
				OnPropertyChanged(nameof(ProgressMsg));
			}
		}

		public IRcsApiController Controller
		{
			get => controller;
			set
			{
				controller = value;
				OnPropertyChanged(nameof(Controller));
				OpenProjectCmdAsync.NotifyCanExecuteChanged();
			}
		}

		public string RcsProjectPath
		{
			get => rcsProjectPath;
			set
			{
				rcsProjectPath = value;
				OnPropertyChanged(nameof(RcsProjectPath));
				OpenProjectCmdAsync.NotifyCanExecuteChanged();
			}
		}

		private string rcsProjectPath;

		private void ApiHeartbeatUpdate(string heartbeatMsg)
		{
			ApiMessage = $"{heartbeatMsg} at {DateTime.Now}";
		}

		private void UpdateProgressbar(int percentage)
		{
			Progress = percentage;
		}

		private void UpdateProgress(string msg)
		{
			ProgressMsg = msg;
		}

		private void UpdateProgress(string msg, int percentage)
		{
			UpdateProgress(msg);
			UpdateProgressbar(percentage);
		}
	}
}
