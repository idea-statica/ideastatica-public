using CommunityToolkit.Mvvm.Input;
using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RcsApi;
using IdeaStatiCa.RcsClient.Services;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RcsApiWpfClientApp.ViewModels
{
	public class MainWindowViewModel : ViewModelBase, IDisposable
	{
		private IApiServiceFactory<IRcsApiClient>? _rcsApiClientFactory;
		private readonly IConfiguration _configuration;
		private readonly IReinforcedCrossSectionTemplateProvider _reinfCssTemplateProvider;
		private readonly IPluginLogger _logger;

		private bool _isBusy;
		private bool _runApiServer;

		private string? outputText;
		ObservableCollection<SectionViewModel>? connectionsVM;
		SectionViewModel? selectedSection;
		private RcsProject? _projectInfo;
		private CancellationTokenSource cts;
		//private static readonly JsonSerializerOptions jsonPresentationOptions = new JsonSerializerOptions() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Default };

		private bool disposedValue;

		public MainWindowViewModel(IConfiguration configuration,
			IReinforcedCrossSectionTemplateProvider reinfCssTemplateProvider,
			IPluginLogger logger)
		{
			this._rcsApiClientFactory = null;
			this.cts = new CancellationTokenSource();
			this._configuration = configuration;
			this._logger = logger;
			this._reinfCssTemplateProvider = reinfCssTemplateProvider;
			this.reinforcedCrossSections = new ObservableCollection<ReinforcedCrossSectionViewModel>();

			RunApiServer = string.IsNullOrEmpty(_configuration["RCS_API_RUNSERVER"]) ? true : _configuration["RCS_API_RUNSERVER"]! == "true";
			ApiUri = string.IsNullOrEmpty(_configuration["RCS_API_RUNSERVER"]) ? null : new Uri(_configuration["RCS_API_ENDPOINT"]!);

			ConnectCommand = new AsyncRelayCommand(ConnectAsync, () => RcsApiClient == null);
			OpenProjectCommand = new AsyncRelayCommand(OpenProjectAsync, () => RcsApiClient != null && this.ProjectInfo == null);
			CloseProjectCommand = new AsyncRelayCommand(CloseProjectAsync, () => this.ProjectInfo != null);

			GetProjectSummaryCmdAsync = new AsyncRelayCommand(GetProjectSummaryAsync, CanGetProjectSummary);

			DownloadProjectCommand = new AsyncRelayCommand(DownloadProjectAsync, () => this.ProjectInfo != null);

			UpdateSectionCmdAsync = new AsyncRelayCommand(UpdateSectionAsync, CanUpdateSection);

			CreateReinfCssCmdAsync = new AsyncRelayCommand<object?>((p) => ImportReinforcedCssAsync(p), (p) => CanCreateReinforcedCss(p));
			UpdateReinfCssCmdAsync = new AsyncRelayCommand<object?>((p) => ImportReinforcedCssAsync(p), (p) => CanUpdateReinforcedCss(p));
			UpdateReinforcementCmdAsync = new AsyncRelayCommand<object?>((p) => ImportReinforcedCssAsync(p), (p) => CanUpdateReinforcedCss(p));

			CalculationCommand = new AsyncRelayCommand(CalculateAsync, () => SelectedSection != null);

			GetDetailResultsCommand = new AsyncRelayCommand(GetDetailResultsAsync, () => SelectedSection != null);

			GetSettingCommand = new AsyncRelayCommand(GetSettingsAsync, () => this.ProjectInfo != null);

			UpdateSettingCommand = new AsyncRelayCommand(UpdateSettingsAsync, () => this.ProjectInfo != null);

			GetLoadingCommand = new AsyncRelayCommand(GetLoadingAsync, () => this.ProjectInfo != null);

			UpdateLoadingCommand = new AsyncRelayCommand(UpdateLoadingAsync, () => this.ProjectInfo != null);

			Sections = new ObservableCollection<SectionViewModel>();
			selectedSection = null;
		}

		private async Task GetLoadingAsync()
		{
			_logger.LogInformation("GetLoadingAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (RcsApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var loadingXml = await RcsApiClient.InternalForces.GetSectionLoadingAsync(ProjectInfo.ProjectId, SelectedSection!.Id, 0, cts.Token);

				OutputText = loadingXml;
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GetLoadingAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		private async Task UpdateLoadingAsync()
		{
			_logger.LogInformation("UpdateLoadingAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (RcsApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				RcsSectionLoading sectionLoading = new RcsSectionLoading();
				sectionLoading.LoadingXml = OutputText;
				sectionLoading.SectionId = SelectedSection!.Id;

				var updateResult = await RcsApiClient!.InternalForces.SetSectionLoadingAsync(ProjectInfo.ProjectId, SelectedSection!.Id, sectionLoading, 0, cts.Token);

				OutputText = updateResult.ToString();
			}
			catch (Exception ex)
			{
				_logger.LogWarning("UpdateLoadingAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		private async Task GetSettingsAsync()
		{
			_logger.LogInformation("GetSettingsAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (RcsApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var connectionIdList = new List<int>();
				connectionIdList.Add(SelectedSection!.Id);

				var resultParam = new RcsResultParameters()
				{
					Sections = connectionIdList
				};

				var settingsJson = await RcsApiClient.Project.GetCodeSettingsJsonAsync(ProjectInfo.ProjectId, 0, cts.Token);

				OutputText = settingsJson;
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GetSettingsAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		private async Task UpdateSettingsAsync()
		{
			_logger.LogInformation("UpdateSettingsAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (RcsApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var mappingSetter = new Services.ProjectSettingsSetter();

				var newSettings = new List<RcsSetting>()
				{
					 new RcsSetting()
					 {
						 Id=1,
						 Type="System.Double",
						 Value=10
					 },
					 new RcsSetting()
					 {
						 Id=2,
						 Type="CI.Services.Setup.Setup2Values`2[System.Double,System.Double]",
						 Value="{\"Value1\":1.5,\"Value2\":1.2}"
					 }
				};

				var modifiedTemplateMapping = await mappingSetter.SetAsync(newSettings);
				if (modifiedTemplateMapping == null || !modifiedTemplateMapping.Any())
				{
					// operation was canceled
					return;
				}

				var updateResult = await RcsApiClient!.Project.UpdateCodeSettingsAsync(ProjectInfo.ProjectId, modifiedTemplateMapping);

				OutputText = updateResult.ToString();

			}
			catch (Exception ex)
			{
				_logger.LogWarning("UpdateSettingsAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		private async Task GetDetailResultsAsync()
		{
			_logger.LogInformation("GetDetailResultsAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (RcsApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var connectionIdList = new List<int>();
				connectionIdList.Add(SelectedSection!.Id);

				var resultParam = new RcsResultParameters()
				{
					Sections = connectionIdList
				};

				var calculationResults = await RcsApiClient.Calculation.GetResultsAsync(ProjectInfo.ProjectId, resultParam, 0, cts.Token);

				OutputText = RcsApiWpfClientApp.Tools.JsonTools.ToFormatedJson(calculationResults);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GetDetailResultsAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		private async Task CalculateAsync()
		{
			await Task.CompletedTask;
			_logger.LogInformation("CalculateAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (RcsApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var connectionIdList = new List<int>();
				connectionIdList.Add(SelectedSection!.Id);

				var calcParam = new RcsCalculationParameters()
				{
					Sections = connectionIdList
				};

				var calculationResults = await RcsApiClient.Calculation.CalculateAsync(ProjectInfo.ProjectId, calcParam, 0, cts.Token);

				OutputText = RcsApiWpfClientApp.Tools.JsonTools.ToFormatedJson(calculationResults);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("CalculateAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		public IRcsApiClient? RcsApiClient { get; set; }

		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

		public bool RunApiServer
		{
			get { return _runApiServer; }
			set { SetProperty(ref _runApiServer, value); }
		}

		public Uri? ApiUri { get; set; }

		public RcsProject? ProjectInfo
		{
			get { return _projectInfo; }
			set { SetProperty(ref _projectInfo, value); }
		}


		public ObservableCollection<SectionViewModel>? Sections
		{
			get => connectionsVM;
			set
			{
				SetProperty(ref connectionsVM, value);
			}
		}

		public SectionViewModel? SelectedSection
		{
			get => selectedSection;
			set
			{
				SetProperty(ref selectedSection, value);

				if (selectedSection != null && ReinforcedCrossSections != null)
				{
					SelectedReinforcedCss = ReinforcedCrossSections.FirstOrDefault(s => s.Id == selectedSection?.ReinforcedCssId);
				}
				else
				{
					SelectedReinforcedCss = null;
				}

				RefreshSectionChanged();
			}
		}

		private ObservableCollection<ReinforcedCrossSectionViewModel> reinforcedCrossSections;
		public ObservableCollection<ReinforcedCrossSectionViewModel> ReinforcedCrossSections
		{
			get => reinforcedCrossSections;
			set
			{
				SetProperty(ref reinforcedCrossSections, value);
				RefreshSectionChanged();
			}
		}

		private ReinforcedCrossSectionViewModel? selectedReinforcedCss;
		public ReinforcedCrossSectionViewModel? SelectedReinforcedCss
		{
			get => selectedReinforcedCss;
			set
			{
				SetProperty(ref selectedReinforcedCss, value);
				RefreshSectionChanged();
			}
		}

		public string? OutputText
		{
			get => outputText;
			set
			{
				SetProperty(ref outputText, value);
			}
		}

		public bool CanStartService => RcsApiClient == null;

		public AsyncRelayCommand ConnectCommand { get; }

		public AsyncRelayCommand OpenProjectCommand { get; }

		public AsyncRelayCommand CalculationCommand { get; }

		public AsyncRelayCommand GetDetailResultsCommand { get; }

		public AsyncRelayCommand CloseProjectCommand { get; }

		public AsyncRelayCommand DownloadProjectCommand { get; }

		public IAsyncRelayCommand GetProjectSummaryCmdAsync
		{
			get;
			private set;
		}

		public IAsyncRelayCommand UpdateSectionCmdAsync
		{
			get;
			private set;
		}

		/// <summary>
		/// A command for creating a new reinforced cross-section by importing template (.nav file)
		/// </summary>
		public IAsyncRelayCommand<object?> CreateReinfCssCmdAsync
		{
			get;
			private set;
		}

		/// <summary>
		/// 
		/// </summary>
		public IAsyncRelayCommand<object?> UpdateReinfCssCmdAsync
		{
			get;
			private set;
		}

		public IAsyncRelayCommand<object?> UpdateReinforcementCmdAsync
		{
			get;
			private set;
		}

		public AsyncRelayCommand GetSettingCommand { get; }

		public AsyncRelayCommand UpdateSettingCommand { get; }

		public AsyncRelayCommand GetLoadingCommand { get; }

		public AsyncRelayCommand UpdateLoadingCommand { get; }

		private async Task OpenProjectAsync()
		{
			_logger.LogInformation("OpenProjectAsync");

			if (RcsApiClient == null)
			{
				throw new Exception("IConnectionApiClient is not connected");
			}

			OpenFileDialog openFileDialog = new OpenFileDialog();

			// Set properties for the OpenFileDialog
			openFileDialog.Title = "Select a Project File";
			openFileDialog.Filter = "IDEARCS Files (*.idearcs)|*.idearcs|XML Files (*.xml)|*.xml";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			IsBusy = true;
			try
			{
				// import ideaRcs ideaRcs project from IOM

				if (openFileDialog.FileName.EndsWith("xml", StringComparison.InvariantCultureIgnoreCase))
				{
					ProjectInfo = await RcsApiClient!.Project.CreateProjectFromIomFileAsync(openFileDialog.FileName, CancellationToken.None);
				}
				else
				{
					// open existing ideaRCS project
					ProjectInfo = await RcsApiClient.Project.OpenProjectAsync(openFileDialog.FileName);
				}

				await GetProjectSummaryAsync();
			}
			catch (Exception ex)
			{
				_logger.LogWarning("OpenProjectAsync", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}

			await Task.CompletedTask;
		}


		private async Task GetProjectSummaryAsync()
		{
			_logger.LogDebug("MainWindowViewModel.GetProjectSummaryAsync");
			try
			{
				if (RcsApiClient is null)
				{
					throw new NullReferenceException("Service is not running");
				}

				ProjectInfo = await RcsApiClient.Project.GetActiveProjectAsync(0, cts.Token);

				SelectedReinforcedCss = null;

				ReinforcedCrossSections = new ObservableCollection<ReinforcedCrossSectionViewModel>(ProjectInfo.ReinforcedCrossSections.Select(rf => new ReinforcedCrossSectionViewModel(rf)));

				Sections = new ObservableCollection<SectionViewModel>(ProjectInfo.Sections.Select(s => new SectionViewModel(s)));

				SelectedSection = Sections?.FirstOrDefault();

				var projectInfoJson = Tools.JsonTools.ToFormatedJson(ProjectInfo);

				OutputText = string.Format("ProjectId = {0}\n\n{1}", RcsApiClient.Project.ProjectId, projectInfoJson);

			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex.Message);
			}
		}

		private async Task ConnectAsync()
		{
			_logger.LogInformation("ConnectAsync");

			if (RcsApiClient != null)
			{
				throw new Exception("IRcsApiController is already connected");
			}

			IsBusy = true;
			try
			{
				if (RunApiServer)
				{
					_rcsApiClientFactory = new RcsApiServiceRunner(_configuration["IdeaStatiCaSetupPath"]);
					RcsApiClient = await _rcsApiClientFactory.CreateApiClient();
				}
				else
				{
					if (ApiUri == null)
					{
						throw new Exception("ApiUri is not set");
					}

					_rcsApiClientFactory = new RcsApiServiceAttacher(_configuration["RCS_API_ENDPOINT"]!);
					RcsApiClient = await _rcsApiClientFactory.CreateApiClient();

					//var connectionInfo = RcsController.GetRcsInfo();
					//OutputText = $"ClientId = {connectionInfo.Item1}, ProjectId = {connectionInfo.Item2}";
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("ConnectAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		internal async Task CloseProjectAsync()
		{
			_logger.LogInformation("CloseProjectAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (RcsApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				await RcsApiClient.Project.CloseProjectAsync(ProjectInfo.ProjectId, 0, cts.Token);
				ProjectInfo = null;
				SelectedSection = null;
				Sections = new ObservableCollection<SectionViewModel>();
				OutputText = string.Empty;
			}
			catch (Exception ex)
			{
				_logger.LogWarning("CloseProjectAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}

			await Task.CompletedTask;
		}

		internal async Task DownloadProjectAsync()
		{
			_logger.LogInformation("DownloadProjectAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (RcsApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "IdeaRcs | *.idearcs";
				if (saveFileDialog.ShowDialog() == true)
				{
					await RcsApiClient.Project.SaveProjectAsync(ProjectInfo.ProjectId, saveFileDialog.FileName, cts.Token);
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("DownloadProjectAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}

			await Task.CompletedTask;
		}

		private bool IsRcsProjectOpen()
		{
			return this.ProjectInfo != null;
		}

		private bool CanGetProjectSummary()
		{
			return IsRcsProjectOpen();
		}

		private bool CanCreateReinforcedCss(object? param)
		{
			return IsRcsProjectOpen();
		}

		private bool CanUpdateReinforcedCss(object? param)
		{
			return IsRcsProjectOpen() && (SelectedReinforcedCss != null && SelectedReinforcedCss.Id > 0);
		}

		private bool CanUpdateSection()
		{
			if (!IsRcsProjectOpen() || SelectedSection == null || SelectedReinforcedCss == null)
			{
				return false;
			}

			int rfCssIdInProject = GetCurrentRFCssId();

			return SelectedReinforcedCss.Id != rfCssIdInProject;
		}

		/// <summary>
		/// Returns the ID of the reinforced cross-section from the selected section
		/// If onthing is selected it retuns -1
		/// </summary>
		/// <returns></returns>
		private int GetCurrentRFCssId()
		{
			if (ProjectInfo == null)
			{
				return -1;
			}

			if (SelectedSection == null)
			{
				return -1;
			}

			// get ID of the reinforced cross-section for the selected section from the open RCS projects
			var section = ProjectInfo.Sections?.FirstOrDefault(s => s.Id == SelectedSection?.Id);

			if (section == null || !section.RCSId.HasValue)
			{
				return -1;
			}

			return section.RCSId.Value;

		}

		private async Task ImportReinforcedCssAsync(object? param)
		{
			_logger.LogInformation("ImportReinforcedCssAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (RcsApiClient == null)
			{
				return;
			}

			if ((selectedSection == null))
			{
				return;
			}

			IsBusy = true;
			try
			{

				var template = await _reinfCssTemplateProvider.GetTemplateAsync();

				if (string.IsNullOrEmpty(template))
				{
					// no template is provided
					_logger.LogDebug("MainWindowViewModel.ImportReinforcedCssAsync - leaving, no template to import");
					return;
				}

				var importSetting = new RcsReinforcedCrosssSectionImportSetting();
				if (param != null && "New".Equals(param.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					// create a new reinforced cross-section
					_logger.LogDebug("MainWindowViewModel.ImportReinforcedCssAsync - new reinforced cross-section is required");
				}
				else if (param != null &&
					("Complete".Equals(param.ToString(), StringComparison.InvariantCultureIgnoreCase) ||
					("Reinf".Equals(param.ToString(), StringComparison.InvariantCultureIgnoreCase))))
				{
					if(SelectedReinforcedCss == null)
					{
						return;
					}

					// create a new reinforced cross-section
					_logger.LogDebug($"MainWindowViewModel.ImportReinforcedCssAsync - it is required to update current RF id = {SelectedReinforcedCss.Id}");
					importSetting.ReinforcedCrossSectionId = SelectedReinforcedCss.Id;
					importSetting.PartsToImport = param.ToString();
				}
				else
				{
					throw new NotSupportedException($"Unsupported import type '{param}'");
				}

				var importData = new RcsReinforcedCrossSectionImportData()
				{
					Template = template,
					Setting = importSetting,
				};

				var updatedSection = await RcsApiClient.CrossSection.ImportReinforcedCrossSectionAsync(ProjectInfo.ProjectId, importData, 0, cts.Token);

				await GetProjectSummaryAsync();
			}
			catch (Exception ex)
			{
				_logger.LogWarning("ImportReinforcedCssAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		private async Task UpdateSectionAsync()
		{
			_logger.LogDebug("MainWindowViewModel.UpdateSectionAsync");
			try
			{
				if (RcsApiClient == null || SelectedSection == null || SelectedReinforcedCss == null)
				{
					throw new Exception("Service is not running");
				}

				if (ProjectInfo == null)
				{
					return;
				}

				// selected section
				int sectionId = SelectedSection.Id;

				// ask user to select reinforced cross-section
				int reinforcedSection = SelectedReinforcedCss.Id;

				var newSectionData = new RcsSection();
				newSectionData.Id = sectionId;
				newSectionData.RCSId = reinforcedSection;

				var updatedSection = await RcsApiClient.Section.UpdateSectionAsync(ProjectInfo.ProjectId, newSectionData, 0, cts.Token);

				await GetProjectSummaryAsync();

			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex.Message);
			}
		}


		//private void ShowClientUI()
		//{
		//	_logger.LogInformation("ShowClientUI");

		//	if (ProjectInfo == null)
		//	{
		//		return;
		//	}

		//	if (RcsController == null)
		//	{
		//		return;
		//	}

		//	try
		//	{
		//		// Open a URL in the default web browser
		//		var connectionInfo = RcsController.GetRcsInfo();
		//		string url = string.Format("{0}/client-ui.html?clientId={1}&projectId={2}", ApiUri, connectionInfo.Item1, connectionInfo.Item2);
		//		Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
		//	}
		//	catch (Exception ex)
		//	{
		//		_logger.LogWarning("GetRawResultsAsync failed", ex);
		//		OutputText = ex.Message;
		//	}
		//}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					try
					{
						if (RcsApiClient != null)
						{
							RcsApiClient.Dispose();
							RcsApiClient = null;
						}
					}
					finally
					{
						if (RunApiServer == true && _rcsApiClientFactory != null)
						{
							if (_rcsApiClientFactory is IDisposable disp)
							{
								disp.Dispose();
							}
							_rcsApiClientFactory = null;
						}
					}
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		private void RefreshCommands()
		{
			this.ConnectCommand.NotifyCanExecuteChanged();
			this.OpenProjectCommand.NotifyCanExecuteChanged();
			this.CloseProjectCommand.NotifyCanExecuteChanged();
			this.DownloadProjectCommand.NotifyCanExecuteChanged();

			this.CalculationCommand.NotifyCanExecuteChanged();
			this.GetSettingCommand.NotifyCanExecuteChanged();
			this.UpdateSettingCommand.NotifyCanExecuteChanged();

			this.CreateReinfCssCmdAsync.NotifyCanExecuteChanged();
			this.UpdateReinfCssCmdAsync.NotifyCanExecuteChanged();
			this.UpdateReinforcementCmdAsync.NotifyCanExecuteChanged();

			this.UpdateSettingCommand.NotifyCanExecuteChanged();

			this.GetLoadingCommand.NotifyCanExecuteChanged();
			this.UpdateLoadingCommand.NotifyCanExecuteChanged();

			this.OnPropertyChanged("CanStartService");

		}

		private void RefreshSectionChanged()
		{
			this.CalculationCommand.NotifyCanExecuteChanged();
			this.GetDetailResultsCommand.NotifyCanExecuteChanged();
			UpdateSectionCmdAsync.NotifyCanExecuteChanged();
			UpdateReinfCssCmdAsync.NotifyCanExecuteChanged();
			UpdateReinforcementCmdAsync.NotifyCanExecuteChanged();

		}
	}
}
