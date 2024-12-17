using CommunityToolkit.Mvvm.Input;
using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.ViewModels
{
	public class MainWindowViewModel : ViewModelBase, IDisposable
	{
		private IApiServiceFactory<IConnectionApiClient>? _connectionApiClientFactory;
		private readonly IConfiguration _configuration;
		private readonly IPluginLogger _logger;

		private bool _isBusy;
		private bool _runApiServer;

		private string? outputText; 
		ObservableCollection<ConnectionViewModel>? connectionsVM;
		ConnectionViewModel? selectedConnection;
		private ConProject? _projectInfo;
		private CancellationTokenSource cts;
		//private static readonly JsonSerializerOptions jsonPresentationOptions = new JsonSerializerOptions() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Default };
		
		private bool disposedValue;

		public MainWindowViewModel(IConfiguration configuration,
			IPluginLogger logger)
		{
			this._connectionApiClientFactory = null;
			this.cts = new CancellationTokenSource();
			this._configuration = configuration;
			this._logger = logger;

			RunApiServer = string.IsNullOrEmpty(_configuration["CONNECTION_API_RUNSERVER"]) ? true : _configuration["CONNECTION_API_RUNSERVER"]! == "true";
			ApiUri = string.IsNullOrEmpty(_configuration["CONNECTION_API_RUNSERVER"]) ? null : new Uri(_configuration["CONNECTION_API_ENDPOINT"]!);

			ConnectCommand = new AsyncRelayCommand(ConnectAsync, () => ConApiClient == null);
			OpenProjectCommand = new AsyncRelayCommand(OpenProjectAsync, () => ConApiClient != null && this.ProjectInfo == null);
			CloseProjectCommand = new AsyncRelayCommand(CloseProjectAsync, () => this.ProjectInfo != null);

			DownloadProjectCommand = new AsyncRelayCommand(DownloadProjectAsync, () => this.ProjectInfo != null);
			ApplyTemplateCommand = new AsyncRelayCommand(ApplyTemplateAsync, () => SelectedConnection != null);

			CalculationCommand = new AsyncRelayCommand(CalculateAsync, () => SelectedConnection != null);

			//ShowClientUICommand = new RelayCommand(ShowClientUI, () => this.ProjectInfo != null);

			Connections = new ObservableCollection<ConnectionViewModel>();
			selectedConnection = null;
		}

		private async Task CalculateAsync()
		{
			_logger.LogInformation("ApplyTemplateAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var connectionIdList = new List<int>();
				connectionIdList.Add(SelectedConnection!.Id);

				var calcParam = new ConCalculationParameter()
				{
					ConnectionIds = connectionIdList,
					AnalysisType = ConAnalysisTypeEnum.Stress_Strain
				};

				var calculationResults = await ConApiClient.Calculation.CalculateAsync(ProjectInfo.ProjectId, calcParam, 0, cts.Token);

				OutputText = ConApiWpfClientApp.Tools.JsonTools.ToFormatedJson(calculationResults);
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

		public IConnectionApiClient? ConApiClient { get; set; }

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

			if (ConApiClient == null)
			{
				throw new Exception("IConnectionApiClient is not connected");
			}

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "IdeaConnection | *.ideacon";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			IsBusy = true;
			try
			{
				ProjectInfo = await ConApiClient.Project.OpenProjectAsync(openFileDialog.FileName);

				var projectInfoJson = Tools.JsonTools.ToFormatedJson(ProjectInfo);
				

				OutputText = string.Format("ClientId = {0}\nProjectId = {1}\n\n{2}", ConApiClient.ClientId, ConApiClient.ActiveProjectId, projectInfoJson);

				Connections = new ObservableCollection<ConnectionViewModel>(ProjectInfo.Connections.Select(c => new ConnectionViewModel(c)));

				if (Connections.Any())
				{
					SelectedConnection = Connections.First();
				}
				else
				{
					SelectedConnection = null;
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

			if (ConApiClient != null)
			{
				throw new Exception("IConnectionApiController is already connected");
			}

			IsBusy = true;
			try
			{
				if (RunApiServer)
				{
					_connectionApiClientFactory = new ConnectionApiServiceRunner(_configuration["IdeaStatiCaSetupPath"]);
					ConApiClient = await _connectionApiClientFactory.CreateApiClient();
				}
				else
				{
					if (ApiUri == null)
					{
						throw new Exception("ApiUri is not set");
					}

					_connectionApiClientFactory = new ConnectionApiServiceAttacher(_configuration["CONNECTION_API_ENDPOINT"]!);
					ConApiClient = await _connectionApiClientFactory.CreateApiClient();

					//var connectionInfo = ConnectionController.GetConnectionInfo();
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

			if (ConApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				await ConApiClient.Project.CloseProjectAsync(ProjectInfo.ProjectId, 0, cts.Token);
				ProjectInfo = null;
				SelectedConnection = null;
				Connections = new ObservableCollection<ConnectionViewModel>();
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

			if (ConApiClient == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "IdeaConnection | *.ideacon";
				if (saveFileDialog.ShowDialog() == true)
				{
					await ConApiClient.Project.SaveProjectAsync(ProjectInfo.ProjectId, saveFileDialog.FileName, cts.Token);
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

			if (ConApiClient == null)
			{
				return;
			}

			if ((selectedConnection == null))
			{
				return;
			}

			IsBusy = true;
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "Connection Template | *.conTemp";
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

				var templateMapping = await ConApiClient.Template.GetDefaultTemplateMappingAsync(ProjectInfo.ProjectId,
					selectedConnection.Id,
					getTemplateParam,
					0, cts.Token);

				if (templateMapping == null)
				{
					throw new ArgumentException($"Invalid mapping for connection '{selectedConnection.Name}'");
				}

				var mappingSetter = new Services.TemplateMappingSetter();
				var modifiedTemplateMapping = await mappingSetter.SetAsync(templateMapping);
				if (modifiedTemplateMapping == null)
				{
					// operation was canceled
					return;
				}

				var applyTemplateParam = new ConTemplateApplyParam()
				{
					ConnectionTemplate = templateXml,
					Mapping = modifiedTemplateMapping
				};

				var applyTemplateResult = await ConApiClient.Template.ApplyTemplateAsync(ProjectInfo.ProjectId,
					SelectedConnection!.Id,
					applyTemplateParam,
					0, cts.Token);


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

		//	if (ConnectionController == null)
		//	{
		//		return;
		//	}

		//	try
		//	{
		//		// Open a URL in the default web browser
		//		var connectionInfo = ConnectionController.GetConnectionInfo();
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
					if(ConApiClient != null)
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

		private void RefreshConnectionChanged()
		{
			this.ApplyTemplateCommand.NotifyCanExecuteChanged();
			this.CalculationCommand.NotifyCanExecuteChanged();
			//this.ShowClientUICommand.NotifyCanExecuteChanged();
		}
	}
}
