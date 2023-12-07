using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RcsApiClient.ViewModels
{
	public class MainWindowViewModel : ObservableObject
	{
		private readonly CancellationTokenSource cancellationTokenSource;
		private readonly IPluginLogger pluginLogger;
		private readonly IRcsClientFactory rcsClientFactory;
		private IRcsApiController? controller;

		public MainWindowViewModel(IPluginLogger pluginLogger, IRcsClientFactory rcsClientFactory)
		{
			OpenProjectCmdAsync = new AsyncRelayCommand(OpenProjectAsync, CanOpenProject);
			GetProjectOverviewCmdAsync = new AsyncRelayCommand(GetProjectOverviewAsync, CanGetProjectOverview);

			Sections = new ObservableCollection<SectionViewModel>();

			this.cancellationTokenSource = new CancellationTokenSource();
			this.OpenedProjectId = Guid.Empty;
			this.pluginLogger = pluginLogger;
			this.rcsClientFactory = rcsClientFactory;



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

		public IAsyncRelayCommand GetProjectOverviewCmdAsync
		{
			get;
			private set;
		}

		private bool CanOpenProject()
		{
			return (this.Controller != null) && !IsRcsProjectOpen();
		}

		private async Task OpenProjectAsync()
		{
			if(this.controller == null)
			{
				throw new Exception("Service is not running");
			}

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
					OpenedProjectId = await controller.OpenProjectAsync(selectedFilePath, cancellationTokenSource.Token);
					this.RcsProjectPath = selectedFilePath;
					//MultiSelectListBox.Items.Clear();
					CalculationResult = $"Project is opened with ID '{OpenedProjectId}'";
				}
			}
			catch(Exception ex)
			{
				ApiMessage = ex.Message;
			}
		}

		private RcsProjectModel? rcsProject;
		public RcsProjectModel? RcsProject
		{
			get => rcsProject;
			set
			{
				rcsProject = value;
				OnPropertyChanged(nameof(RcsProject));
			}
		}


		private ObservableCollection<SectionViewModel> sections;
		public ObservableCollection<SectionViewModel> Sections
		{
			get => sections;
			set
			{
				sections = value;
				OnPropertyChanged(nameof(Sections));
			}
		}

		private SectionViewModel selectedSection;
		public SectionViewModel SelectedSection
		{
			get => selectedSection;
			set
			{
				selectedSection = value;
				OnPropertyChanged(nameof(SelectedSection));
			}
		}

		private bool CanGetProjectOverview()
		{
			return IsRcsProjectOpen();
		}

		private async Task GetProjectOverviewAsync()
		{
			try
			{
				if (Controller == null)
				{
					throw new Exception("Service is not running");
				}

				RcsProject = await Controller.GetProjectOverviewAsync(OpenedProjectId, cancellationTokenSource.Token);
				Sections = new ObservableCollection<SectionViewModel>(RcsProject.Sections.Select(s => new SectionViewModel(s)));
			}
			catch(Exception ex)
			{
			}
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
				GetProjectOverviewCmdAsync.NotifyCanExecuteChanged();
			}
		}

		private string calculationResult = string.Empty;
		public string CalculationResult
		{
			get => calculationResult;
			set
			{
				calculationResult = value;
				OnPropertyChanged(nameof(CalculationResult));
			}
		}

		private string apiMessage = string.Empty;

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

		private string progressMsg = string.Empty ;
		public string ProgressMsg
		{
			get => progressMsg;
			set
			{
				progressMsg = value;
				OnPropertyChanged(nameof(ProgressMsg));
			}
		}

		public IRcsApiController? Controller
		{
			get => controller;
			set
			{
				controller = value;
				OnPropertyChanged(nameof(Controller));
				OpenProjectCmdAsync.NotifyCanExecuteChanged();
			}
		}

		private string rcsProjectPath = string.Empty ;

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

		private bool IsRcsProjectOpen()
		{
			return OpenedProjectId != Guid.Empty;
		}

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
