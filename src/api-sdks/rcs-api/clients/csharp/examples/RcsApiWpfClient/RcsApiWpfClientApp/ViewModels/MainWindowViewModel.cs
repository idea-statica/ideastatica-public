using CommunityToolkit.Mvvm.Input;
using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RcsApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
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
			IPluginLogger logger)
		{
			this._rcsApiClientFactory = null;
			this.cts = new CancellationTokenSource();
			this._configuration = configuration;
			this._logger = logger;

			RunApiServer = string.IsNullOrEmpty(_configuration["RCS_API_RUNSERVER"]) ? true : _configuration["RCS_API_RUNSERVER"]! == "true";
			ApiUri = string.IsNullOrEmpty(_configuration["RCS_API_RUNSERVER"]) ? null : new Uri(_configuration["RCS_API_ENDPOINT"]!);

			ConnectCommand = new AsyncRelayCommand(ConnectAsync, () => RcsApiClient == null);
			OpenProjectCommand = new AsyncRelayCommand(OpenProjectAsync, () => RcsApiClient != null && this.ProjectInfo == null);
			CloseProjectCommand = new AsyncRelayCommand(CloseProjectAsync, () => this.ProjectInfo != null);

			DownloadProjectCommand = new AsyncRelayCommand(DownloadProjectAsync, () => this.ProjectInfo != null);
			ApplyTemplateCommand = new AsyncRelayCommand(ApplyTemplateAsync, () => SelectedSection != null);

			CalculationCommand = new AsyncRelayCommand(CalculateAsync, () => SelectedSection != null);

			//ShowClientUICommand = new RelayCommand(ShowClientUI, () => this.ProjectInfo != null);

			Sections = new ObservableCollection<SectionViewModel>();
			selectedSection = null;
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
				_logger.LogWarning("GetRawResultsAsync failed", ex);
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

		public AsyncRelayCommand CloseProjectCommand { get; }

		public AsyncRelayCommand DownloadProjectCommand { get; }

		public AsyncRelayCommand ApplyTemplateCommand { get; }

		//public AsyncRelayCommand GetSceneDataCommand { get; }

		//public RelayCommand ShowClientUICommand { get; }
		

		private async Task OpenProjectAsync()
		{
			_logger.LogInformation("OpenProjectAsync");

			if (RcsApiClient == null)
			{
				throw new Exception("IConnectionApiClient is not connected");
			}

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "IdeaRcs | *.idearcs";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			IsBusy = true;
			try
			{
				ProjectInfo = await RcsApiClient.Project.OpenProjectAsync(openFileDialog.FileName);

				var projectInfoJson = Tools.JsonTools.ToFormatedJson(ProjectInfo);

				OutputText = string.Format("ProjectId = {0}\n\n{1}", RcsApiClient.Project.ProjectId, projectInfoJson);

				Sections = new ObservableCollection<SectionViewModel>(ProjectInfo.Sections.Select(s => new SectionViewModel(s)));

				if (Sections.Any())
				{
					SelectedSection = Sections.First();
				}
				else
				{
					SelectedSection = null;
				}
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
				saveFileDialog.Filter = "IdeaRcs | *.ideacon";
				if (saveFileDialog.ShowDialog() == true)
				{
					await RcsApiClient.Project.SaveProjectAsync(ProjectInfo.ProjectId, saveFileDialog.FileName, cts.Token);
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("SaveProjectAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}

			await Task.CompletedTask;
		}

		private async Task ApplyTemplateAsync()
		{
			_logger.LogInformation("ApplyTemplateAsync");

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
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "Rcs Template | *.conTemp";
				if (openFileDialog.ShowDialog() != true)
				{
					_logger.LogDebug("ApplyTemplateAsync - no template is selected");
					return;
				}

				var templateXml = await System.IO.File.ReadAllTextAsync(openFileDialog.FileName);
				var getTemplateParam = new ConTemplateMappingGetParam()
				{
					Template = templateXml
				};

				//var templateMapping = await RcsApiClient.Template.GetDefaultTemplateMappingAsync(ProjectInfo.ProjectId,
				//	selectedRcs.Id,
				//	getTemplateParam,
				//	0, cts.Token);

				//if (templateMapping == null)
				//{
				//	throw new ArgumentException($"Invalid mapping for connection '{selectedRcs.Name}'");
				//}

				//var mappingSetter = new Services.TemplateMappingSetter();
				//var modifiedTemplateMapping = await mappingSetter.SetAsync(templateMapping);
				//if (modifiedTemplateMapping == null)
				//{
				//	// operation was canceled
				//	return;
				//}

				//var applyTemplateParam = new ConTemplateApplyParam()
				//{
				//	RcsTemplate = templateXml,
				//	Mapping = modifiedTemplateMapping
				//};

				//var applyTemplateResult = await RcsApiClient.Template.ApplyTemplateAsync(ProjectInfo.ProjectId,
				//	SelectedRcs!.Id,
				//	applyTemplateParam,
				//	0, cts.Token);


				OutputText = "Template was applied";
			}
			catch (Exception ex)
			{
				_logger.LogWarning("ApplyTemplateAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
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
			this.ApplyTemplateCommand.NotifyCanExecuteChanged();
			this.CalculationCommand.NotifyCanExecuteChanged();
			this.OnPropertyChanged("CanStartService");
			//this.ShowClientUICommand.NotifyCanExecuteChanged();
		}

		private void RefreshSectionChanged()
		{
			this.ApplyTemplateCommand.NotifyCanExecuteChanged();
			this.CalculationCommand.NotifyCanExecuteChanged();
			//this.ShowClientUICommand.NotifyCanExecuteChanged();
		}
	}
}
