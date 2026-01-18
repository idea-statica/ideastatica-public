using ConApiWpfClientApp.Commands;
using ConApiWpfClientApp.Models;
using ConApiWpfClientApp.Services;
using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConRestApiClientUI;
using IdeaStatiCa.Plugin;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConApiWpfClientApp.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private IApiServiceFactory<IConnectionApiClient>? _connectionApiClientFactory;
		private readonly IConfiguration _configuration;
		private readonly IPluginLogger _logger;
		private readonly ISceneController _sceneController;

		private bool _isBusy;
		private bool _runApiServer;

		private string? outputText; 
		ObservableCollection<ConnectionViewModel>? connectionsVM;
		ConnectionViewModel? selectedConnection;
		private ConProject? _projectInfo;
		private CancellationTokenSource cts;
		private ConAnalysisTypeEnum _conAnalysisType;
		private IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum _weldSizingType;
		private bool _calculateBuckling;
		private bool _getRawResults;
		//private static readonly JsonSerializerOptions jsonPresentationOptions = new JsonSerializerOptions() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Default };

		private bool disposedValue;

		// Make ConnectionApiClientFactory accessible to commands
		internal IApiServiceFactory<IConnectionApiClient>? ConnectionApiClientFactory
		{
			get => _connectionApiClientFactory;
			set => _connectionApiClientFactory = value;
		}

		public MainWindowViewModel(IConfiguration configuration,
			IPluginLogger logger, ISceneController sceneController)
		{
			this._connectionApiClientFactory = null;
			this.cts = new CancellationTokenSource();
			this._configuration = configuration;
			this._logger = logger;
			this._sceneController = sceneController;
			this._conAnalysisType = ConAnalysisTypeEnum.Stress_Strain;
			this._calculateBuckling = false;
			this._getRawResults = false;

			RunApiServer = string.IsNullOrEmpty(_configuration["CONNECTION_API_RUNSERVER"]) ? true : _configuration["CONNECTION_API_RUNSERVER"]! == "true";
			ApiUri = string.IsNullOrEmpty(_configuration["CONNECTION_API_RUNSERVER"]) ? null : new Uri(_configuration["CONNECTION_API_ENDPOINT"]!);
			
			// Initialize commands with independent command classes
			ShowLogsCommand = new ShowLogsCommand(this, logger);
			EditDiagnosticsCommand = new EditDiagnosticsCommand(this, logger);
			ConnectCommand = new ConnectCommand(this, logger, configuration);
			OpenProjectCommand = new OpenProjectCommand(this, logger);
			ImportIomCommand = new ImportIomCommand(this, logger);
			CloseProjectCommand = new CloseProjectCommand(this, logger, cts);
			DownloadProjectCommand = new DownloadProjectCommand(this, logger, cts);
			ApplyTemplateCommand = new ApplyTemplateCommand(this, logger, cts, sceneController);
			CreateTemplateCommand = new CreateTemplateCommand(this, logger, cts);
			GetTopologyCommand = new GetTopologyCommand(this, logger, cts);
			GetSceneDataCommand = new GetSceneDataCommand(this, logger, cts);
			CalculationCommand = new CalculationCommand(this, logger, cts);
			GetMembersCommand = new GetMembersCommand(this, logger, cts);
			GetOperationsCommand = new GetOperationsCommand(this, logger, cts);
			DeleteOperationsCommand = new DeleteOperationsCommand(this, logger, cts);
			GenerateReportCommand = new GenerateReportCommand(this, logger);
			ExportCommand = new ExportCommand(this, logger);
			GetSettingsCommand = new GetSettingsCommand(this, logger, cts);
			UpdateSettingsCommand = new UpdateSettingsCommand(this, logger, cts);
			WeldSizingCommand = new WeldSizingCommand(this, logger, cts);
			UpdateConnectionLoadingCommand = new UpdateConnectionLoadingCommand(this, logger, cts);
			EvaluateExpressionCommand = new EvaluateExpressionCommand(this, logger, cts);
			EditParametersCommand = new EditParametersCommand(this, logger, cts);
			ConIomGeneratorCommand = new GenerateConnectionIomCommand(this, logger, cts);

			Connections = new ObservableCollection<ConnectionViewModel>();
			selectedConnection = null;
		}

		public IConnectionApiClient? ConApiClient { get; set; }

		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

		public ConAnalysisTypeEnum SelectedAnalysisType
		{
			get { return _conAnalysisType; }
			set { SetProperty(ref _conAnalysisType, value); }
		}

		public bool CalculateBuckling
		{
			get { return _calculateBuckling; }
			set { SetProperty(ref _calculateBuckling, value); }
		}

		public bool GetRawXmlResults
		{
			get { return _getRawResults; }
			set { SetProperty(ref _getRawResults, value); }
		}

		public Array AvailableAnalysisTypes => Enum.GetValues(typeof(ConAnalysisTypeEnum));

		public Array AvailableWeldSizingTypes => Enum.GetValues(typeof(IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum));

		public IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum WeldSizingType
		{
			get { return _weldSizingType; }
			set { SetProperty(ref _weldSizingType, value); }
		}

		public bool RunApiServer
		{
			get { return _runApiServer; }
			set { SetProperty(ref _runApiServer, value); }
		}

		public Uri? ApiUri { get; set; }

		public ConProject? ProjectInfo
		{
			get { return _projectInfo; }
			set { SetProperty(ref _projectInfo, value); }
		}

		public ObservableCollection<ConnectionViewModel>? Connections
		{
			get => connectionsVM;
			set
			{
				SetProperty(ref connectionsVM, value);
			}
		}

		public ConnectionViewModel? SelectedConnection
		{
			get => selectedConnection;
			set
			{
				SetProperty(ref selectedConnection, value);
				RefreshConnectionChanged();
				ShowClientUIAsync();
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

		public bool CanStartService => ConApiClient == null;

		public ICommand ConnectCommand { get; }

		public ICommand OpenProjectCommand { get; }

		public ICommand ImportIomCommand { get; }

		public ICommand CalculationCommand { get; }

		public ICommand CloseProjectCommand { get; }

		public ICommand DownloadProjectCommand { get; }

		public ICommand ApplyTemplateCommand { get; }

		public ICommand GetMembersCommand { get; }

		public ICommand GetOperationsCommand { get; }

		public ICommand DeleteOperationsCommand { get; }

		public ICommand CreateTemplateCommand { get; }

		public ICommand GetSceneDataCommand { get; }
		
		public ICommand GetTopologyCommand { get; }

		public ICommand GenerateReportCommand { get; }

		public ICommand ExportCommand { get; }

		public ICommand ShowLogsCommand { get; }

		public ICommand EditDiagnosticsCommand { get; }

		public ICommand GetSettingsCommand { get; }

		public ICommand UpdateSettingsCommand { get; }

		public ICommand WeldSizingCommand { get; }

		public ICommand UpdateConnectionLoadingCommand { get; }

	public ICommand EvaluateExpressionCommand { get; }

	public ICommand EditParametersCommand { get; }

	public ICommand ConIomGeneratorCommand { get; }

	internal async Task ShowClientUIAsync()
		{
			_logger.LogInformation("ShowClientUI");

			if (ProjectInfo == null)
			{
				await _sceneController.PresentAsync(string.Empty);
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			if (SelectedConnection == null || SelectedConnection.Id < 1)
			{
				await _sceneController.PresentAsync(string.Empty);
				return;
			}

			IsBusy = true;
			try
			{
				var sceneJson = await ConApiClient.Presentation.GetDataScene3DTextAsync(ProjectInfo.ProjectId,
					SelectedConnection!.Id, 0, cts.Token);

				if(string.IsNullOrEmpty(sceneJson))
				{
					return;
				}

				await _sceneController.PresentAsync(sceneJson);

				await Task.CompletedTask;
			}
			catch (Exception ex)
			{
				_logger.LogWarning("CreateConTemplateAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}

		}

		public Task OnExitApplication()
		{
			return Task.Run(() =>
			{
				if (!disposedValue)
				{
					if (ConApiClient != null)
					{
						ConApiClient.Dispose();
						ConApiClient = null;
					}

					if (RunApiServer == true && _connectionApiClientFactory != null)
					{
						if (_connectionApiClientFactory is IDisposable disp)
						{
							disp.Dispose();
						}
						_connectionApiClientFactory = null;
					}

					disposedValue = true;
				}
			});
		}

		internal void RefreshCommands()
		{
			if (ConnectCommand is AsyncCommandBase connectCmd)
				connectCmd.RaiseCanExecuteChanged();
			if (OpenProjectCommand is AsyncCommandBase openProjectCmd)
				openProjectCmd.RaiseCanExecuteChanged();
			if (ImportIomCommand is AsyncCommandBase importIomCmd)
				importIomCmd.RaiseCanExecuteChanged();
			if (CloseProjectCommand is AsyncCommandBase closeProjectCmd)
				closeProjectCmd.RaiseCanExecuteChanged();
			if (DownloadProjectCommand is AsyncCommandBase downloadProjectCmd)
				downloadProjectCmd.RaiseCanExecuteChanged();
			if (ApplyTemplateCommand is AsyncCommandBase applyTemplateCmd)
				applyTemplateCmd.RaiseCanExecuteChanged();
			if (CalculationCommand is AsyncCommandBase calculationCmd)
				calculationCmd.RaiseCanExecuteChanged();
			if (CreateTemplateCommand is AsyncCommandBase createTemplateCmd)
				createTemplateCmd.RaiseCanExecuteChanged();
			if (GetTopologyCommand is AsyncCommandBase getTopologyCmd)
				getTopologyCmd.RaiseCanExecuteChanged();
			if (GetSceneDataCommand is AsyncCommandBase getSceneDataCmd)
				getSceneDataCmd.RaiseCanExecuteChanged();
			if (GetMembersCommand is AsyncCommandBase getMembersCmd)
				getMembersCmd.RaiseCanExecuteChanged();
			if (GetOperationsCommand is AsyncCommandBase getOperationsCmd)
				getOperationsCmd.RaiseCanExecuteChanged();
			if (DeleteOperationsCommand is AsyncCommandBase deleteOperationsCmd)
				deleteOperationsCmd.RaiseCanExecuteChanged();
			if (GenerateReportCommand is AsyncCommandBase generateReportCmd)
				generateReportCmd.RaiseCanExecuteChanged();
			if (ExportCommand is AsyncCommandBase exportCmd)
				exportCmd.RaiseCanExecuteChanged();
			if (GetSettingsCommand is AsyncCommandBase getSettingsCmd)
				getSettingsCmd.RaiseCanExecuteChanged();
			if (UpdateSettingsCommand is AsyncCommandBase updateSettingsCmd)
				updateSettingsCmd.RaiseCanExecuteChanged();
			if (WeldSizingCommand is AsyncCommandBase weldSizingCmd)
				weldSizingCmd.RaiseCanExecuteChanged();
			if (UpdateConnectionLoadingCommand is AsyncCommandBase updateConnectionLoadingCmd)
				updateConnectionLoadingCmd.RaiseCanExecuteChanged();
			if (EvaluateExpressionCommand is AsyncCommandBase evaluateExpressionCmd)
				evaluateExpressionCmd.RaiseCanExecuteChanged();
			if (EditParametersCommand is AsyncCommandBase editParametersCmd)
				editParametersCmd.RaiseCanExecuteChanged();
			if (ConIomGeneratorCommand is AsyncCommandBase conIomGeneratorCmd)
				conIomGeneratorCmd.RaiseCanExecuteChanged();

			this.OnPropertyChanged("CanStartService");
		}

		private void RefreshConnectionChanged()
		{
			if (ApplyTemplateCommand is AsyncCommandBase applyTemplateCmd)
				applyTemplateCmd.RaiseCanExecuteChanged();
			if (CalculationCommand is AsyncCommandBase calculationCmd)
				calculationCmd.RaiseCanExecuteChanged();
		}
	}
}
