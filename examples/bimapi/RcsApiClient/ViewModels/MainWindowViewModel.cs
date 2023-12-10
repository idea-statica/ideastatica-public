using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RcsApiClient.ViewModels
{
	public class MainWindowViewModel : ObservableObject
	{
		private CancellationTokenSource cancellationTokenSource;
		private readonly IPluginLogger pluginLogger;
		private readonly IRcsClientFactory rcsClientFactory;
		private IRcsApiController? controller;

		public MainWindowViewModel(IPluginLogger pluginLogger, IRcsClientFactory rcsClientFactory)
		{
			OpenProjectCmdAsync = new AsyncRelayCommand(OpenProjectAsync, CanOpenProject);
			CancelCalculationCmd = new RelayCommand(CancelCalculation, CanCancel);

			GetProjectOverviewCmdAsync = new AsyncRelayCommand(GetProjectOverviewAsync, CanGetProjectOverview);
			CalculateResultsCmdAsync = new AsyncRelayCommand(CalculateResultsAsync, CanCalculateResultsAsync);
			GetResultsCmdAsync = new AsyncRelayCommand(GetResultsAsync, CanGetResultsAsync);

			GetSectionsCmdAsync = new AsyncRelayCommand(GetSections, CanGetProjectOverview);
			GetMembersCmdAsync = new AsyncRelayCommand(GetMembers, CanGetProjectOverview);
			GetReinforcedCrossSectionsCmdAsync = new AsyncRelayCommand(GetReinforcedCrossSections, CanGetProjectOverview);

			SetReinforcedCssCmdAsync = new AsyncRelayCommand(SetReinforcedCssAsync, CanSetReinforcedCss);

			sections = new ObservableCollection<SectionViewModel>();

			this.cancellationTokenSource = new CancellationTokenSource();
			this.ProjectOpened = false;
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


		/// <summary>
		/// A command for changing reinforced cross-section in a selected section
		/// </summary>
		public IAsyncRelayCommand SetReinforcedCssCmdAsync
		{
			get;
			private set;
		}

		public IRelayCommand CancelCalculationCmd
		{
			get;
			private set;
		}

		public IAsyncRelayCommand GetProjectOverviewCmdAsync
		{
			get;
			private set;
		}

		public IAsyncRelayCommand CalculateResultsCmdAsync
		{
			get;
			private set;
		}

		public IAsyncRelayCommand GetResultsCmdAsync
		{
			get;
			private set;
		}

		public IAsyncRelayCommand SelectionChangedCmdAsync
		{
			get;
			private set;
		}

		public IAsyncRelayCommand GetMembersCmdAsync
		{
			get;
			private set;
		}

		public IAsyncRelayCommand GetSectionsCmdAsync
		{
			get;
			private set;
		}

		public IAsyncRelayCommand GetReinforcedCrossSectionsCmdAsync
		{
			get;
			private set;
		}

		private bool CanOpenProject()
		{
			return this.Controller != null;
		}

		private bool CanCancel()
		{
			return this.Controller != null && CalculationInProgress;
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
					ProjectOpened = await controller.OpenProjectAsync(selectedFilePath, cancellationTokenSource.Token);
					this.RcsProjectPath = selectedFilePath;
					CalculationResult = "Project is opened";
					await GetProjectOverviewAsync();
				}
			}
			catch(Exception ex)
			{
				ApiMessage = ex.Message;
			}
		}

		private void CancelCalculation()
		{
			cancellationTokenSource.Cancel();
		}

		private bool calculationInProgress;
		public bool CalculationInProgress
		{
			get => calculationInProgress;
			set
			{
				calculationInProgress = value;
				OnPropertyChanged(nameof(CalculationInProgress));
				CancelCalculationCmd.NotifyCanExecuteChanged();
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

		private SectionViewModel? selectedSection;
		public SectionViewModel? SelectedSection
		{
			get => selectedSection;
			set
			{
				selectedSection = value;
				OnPropertyChanged(nameof(SelectedSection));
				SetReinforcedCssCmdAsync.NotifyCanExecuteChanged();
			}
		}

		private bool CanGetProjectOverview()
		{
			return IsRcsProjectOpen();
		}

		private bool CanCalculateResultsAsync()
		{
			return IsRcsProjectOpen();
		}

		private bool CanGetResultsAsync()
		{
			return IsRcsProjectOpen();
		}

		private bool CanSetReinforcedCss()
		{
			return (IsRcsProjectOpen() && SelectedSection != null);
		}


		private async Task SetReinforcedCssAsync()
		{
			pluginLogger.LogDebug("MainWindowViewModel.SetReinforcedCssAsync");
			try
			{
				if (Controller == null)
				{
					throw new Exception("Service is not running");
				}

				int sectionId = 6;
				int reinforcedSection = 2;

				var updatedSection = await Controller.SetReinforcementAsync(sectionId, reinforcedSection);

			}
			catch (Exception ex)
			{
				pluginLogger.LogWarning(ex.Message);
			}
		}

		private async Task GetReinforcedCrossSections()
		{
			pluginLogger.LogDebug("MainWindowViewModel.GetReinforcedCrossSections");
			if (Controller is null)
			{
				throw new NullReferenceException("Service is not running");
			}

			var result = await Controller.GetProjectReinforcedCrossSectionsAsync(cancellationTokenSource.Token);

			CalculationResult = Tools.FormatJson(JsonConvert.SerializeObject(result));
		}

		private async Task GetMembers()
		{
			pluginLogger.LogDebug("MainWindowViewModel.GetMembers");
			if (Controller is null)
			{
				throw new NullReferenceException("Service is not running");
			}

			var result = await Controller.GetProjectMembersAsync(cancellationTokenSource.Token);

			CalculationResult = Tools.FormatJson(JsonConvert.SerializeObject(result));
		}

		private async Task GetSections()
		{
			pluginLogger.LogDebug("MainWindowViewModel.GetSections");
			if (Controller is null)
			{
				throw new NullReferenceException("Service is not running");
			}

			var result = await Controller.GetProjectSectionsAsync(cancellationTokenSource.Token);

			CalculationResult = Tools.FormatJson(JsonConvert.SerializeObject(result));
		}

		private async Task GetProjectOverviewAsync()
		{
			pluginLogger.LogDebug("MainWindowViewModel.GetProjectOverviewAsync");
			try
			{
				if (Controller is null)
				{
					throw new NullReferenceException("Service is not running");
				}

				RcsProject = await Controller.GetProjectOverviewAsync(cancellationTokenSource.Token);

				CalculationResult = Tools.FormatJson(JsonConvert.SerializeObject(RcsProject));

				Sections = new ObservableCollection<SectionViewModel>(RcsProject.Sections.Select(s => new SectionViewModel(s)));

				SelectedSection = Sections?.FirstOrDefault();
				
			}
			catch(Exception ex)
			{
				pluginLogger.LogWarning(ex.Message);
			}
		}

		private async Task CalculateResultsAsync()
		{
			if (Controller == null)
			{
				throw new NullReferenceException("Service is not running");
			}

			cancellationTokenSource = new CancellationTokenSource();
			pluginLogger.LogDebug("MainWindowViewModel.CalculateResultsAsync");
			CalculationResult = string.Empty;

			var parameters = new RcsCalculationParameters();
			var sectionList = new List<int>();

			foreach (var selectedSection in Sections)
			{
				sectionList.Add(int.Parse(selectedSection.Id.ToString()));
			}

			parameters.Sections = sectionList;

			CalculationInProgress = true;
			var result = await Controller.CalculateResultsAsync(parameters, cancellationTokenSource.Token);
			CalculationInProgress = false;
			if (result is { })
			{
				CalculationResult = Tools.FormatJson(JsonConvert.SerializeObject(result));
			}
			else
			{
				CalculationResult = "Request failed.";
			}
		}

		private async Task GetResultsAsync()
		{
			if (Controller == null)
			{
				throw new NullReferenceException("Service is not running");
			}

			pluginLogger.LogDebug("MainWindowViewModel.GetResultsAsync");
			CalculationResult = string.Empty;
			UpdateProgress("", 0);

			var parameters = new RcsCalculationParameters();
			var sectionList = new List<int>();

			foreach (var selectedSection in Sections)
			{
				sectionList.Add(int.Parse(selectedSection.Id.ToString()));
			}

			parameters.Sections = sectionList;
			var result = await Controller.GetResultsAsync(parameters, cancellationTokenSource.Token);

			if (result is { })
			{
				CalculationResult = Tools.FormatJson(JsonConvert.SerializeObject(result));
			}
			else
			{
				CalculationResult = "Request failed.";
			}
		}

		private async Task SetReinforcedCssAsyncAsync()
		{
			if (Controller == null)
			{
				throw new Exception("Service is not running");
			}

			CalculationResult = string.Empty;
			UpdateProgress("", 0);

			var parameters = new RcsCalculationParameters();
			var result = await Controller.GetNonConformityIssuesAsync(parameters, CancellationToken.None);
		}


		private async void CreateClientAsync()
		{
			ApiMessage = "Starting RCS Service";
			var apiController = await rcsClientFactory.CreateRcsApiClient();
			this.Controller = apiController;
			ApiMessage = "RCS Service is running";
		}

		private bool projectOpened;
		public bool ProjectOpened
		{
			get => projectOpened;
			set
			{
				projectOpened = value;
				OnPropertyChanged(nameof(ProjectOpened));
				GetProjectOverviewCmdAsync.NotifyCanExecuteChanged();
				CalculateResultsCmdAsync.NotifyCanExecuteChanged();
				GetResultsCmdAsync.NotifyCanExecuteChanged();
				GetSectionsCmdAsync.NotifyCanExecuteChanged();
				GetMembersCmdAsync.NotifyCanExecuteChanged();
				GetReinforcedCrossSectionsCmdAsync.NotifyCanExecuteChanged();
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
			return ProjectOpened;
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
