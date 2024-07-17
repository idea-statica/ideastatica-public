using CommunityToolkit.Mvvm.Input;
using ConnectionWebClient.Tools;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.ConnectionRest;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectionWebClient.ViewModels
{
	public class MainWindowViewModel : ViewModelBase, IDisposable
	{
		private readonly IConnectionApiClientFactory _connectionApiClientFactory;
		private readonly IConfiguration _configuration;
		private readonly IPluginLogger _logger;

		private bool _isBusy;
		private string? outputText; 
		ObservableCollection<ConnectionViewModel>? connectionsVM;
		ConnectionViewModel? selectedConnection;
		private ConProject? _projectInfo;
		private CancellationTokenSource cts;
		
		private bool disposedValue;

		public MainWindowViewModel(IConfiguration configuration,
			IPluginLogger logger,
			IConnectionApiClientFactory apiClientFactory)
		{
			this._connectionApiClientFactory = apiClientFactory;
			this.cts = new CancellationTokenSource();
			this._configuration = configuration;
			this._logger = logger;

			RunApiServer = string.IsNullOrEmpty(_configuration["CONNECTION_API_RUNSERVER"]) ? true : _configuration["CONNECTION_API_RUNSERVER"]! == "true";
			ApiUri = string.IsNullOrEmpty(_configuration["CONNECTION_API_RUNSERVER"]) ? null : new Uri(_configuration["CONNECTION_API_ENDPOINT"]!);

			ConnectCommand = new AsyncRelayCommand(ConnectAsync, () => ConnectionController == null);
			OpenProjectCommand = new AsyncRelayCommand(OpenProjectAsync, () => ConnectionController != null && this.ProjectInfo == null);
			CloseProjectCommand = new AsyncRelayCommand(CloseProjectAsync, () => this.ProjectInfo != null);

			DownloadProjectCommand = new AsyncRelayCommand(DownloadProjectAsync, () => this.ProjectInfo != null);
			ApplyTemplateCommand = new AsyncRelayCommand(ApplyTemplateAsync, () => SelectedConnection != null);

			GetRawResultsCommand = new AsyncRelayCommand(GetRawResultsAsync, () => SelectedConnection != null);

			Connections = new ObservableCollection<ConnectionViewModel>();
			selectedConnection = null;
		}


		private async Task GetRawResultsAsync()
		{
			_logger.LogInformation("ApplyTemplateAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (ConnectionController == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var connectionIdList = new List<int>();
				connectionIdList.Add(SelectedConnection!.Id);

				var res = await ConnectionController.GetRawResultsAsync(connectionIdList, IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection.ConAnalysisTypeEnum.Stress_Strain, cts.Token);
				OutputText = res;
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

		public IConnectionApiController? ConnectionController { get; set; }

		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

		public bool RunApiServer { get; set;}

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

		public AsyncRelayCommand ConnectCommand { get; }

		public AsyncRelayCommand OpenProjectCommand { get; }

		public AsyncRelayCommand GetRawResultsCommand { get; }

		public AsyncRelayCommand CloseProjectCommand { get; }

		public AsyncRelayCommand DownloadProjectCommand { get; }

		public AsyncRelayCommand ApplyTemplateCommand { get; }

		private async Task OpenProjectAsync()
		{
			_logger.LogInformation("OpenProjectAsync");

			if (ConnectionController == null)
			{
				throw new Exception("IConnectionApiController is not connected");
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
				ProjectInfo = await ConnectionController.OpenProjectAsync(openFileDialog.FileName, cts.Token);

				OutputText =JsonTools.ToFormatedJson(ProjectInfo);
				Connections = new ObservableCollection<ConnectionViewModel>(ProjectInfo.Connections.Select(c => new ConnectionViewModel(c)));

				if(Connections.Any())
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
		}

		private async Task ConnectAsync()
		{
			_logger.LogInformation("ConnectAsync");

			if (ConnectionController != null)
			{
				throw new Exception("IConnectionApiController is already connected");
			}

			IsBusy = true;
			try
			{
				if (RunApiServer)
				{
					ConnectionController = await _connectionApiClientFactory.CreateConnectionApiClient();
				}
				else
				{
					if (ApiUri == null)
					{
						throw new Exception("ApiUri is not set");
					}

					ConnectionController = await _connectionApiClientFactory.CreateConnectionApiClient(ApiUri);
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

			if(ConnectionController == null)
			{
				return;
			}	

			IsBusy = true;
			try
			{
				await ConnectionController.CloseProjectAsync(cts.Token);
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

			if (ConnectionController == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var projectStream = await ConnectionController.DownloadProjectAsync(cts.Token);

				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "IdeaConnection | *.ideacon";
				if (saveFileDialog.ShowDialog() == true)
				{
					using (var fileStream = saveFileDialog.OpenFile())
					{
						await projectStream.CopyToAsync(fileStream);
					}
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

		private async Task ApplyTemplateAsync()
		{
			_logger.LogInformation("ApplyTemplateAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (ConnectionController == null)
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

				var templateMapping = await ConnectionController.GetTemplateMappingAsync(SelectedConnection!.Id, templateXml, cts.Token);
				if (templateMapping == null)
				{
					throw new ArgumentException($"Invalid mapping for connection '{SelectedConnection.Name}'");
				}

				var mappingSetter = new Services.TemplateMappingSetter();
				var modifiedTemplateMapping = await mappingSetter.SetAsync(templateMapping);
				if(modifiedTemplateMapping == null)
				{
					// operation was canceled
					return;
				}

				var applyTemplateResult = await ConnectionController.ApplyConnectionTemplateAsync(SelectedConnection!.Id, templateXml, modifiedTemplateMapping, cts.Token);

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

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if(ConnectionController != null)
					{
						ConnectionController.Dispose();
						ConnectionController = null;
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
			this.GetRawResultsCommand.NotifyCanExecuteChanged();
		}

		private void RefreshConnectionChanged()
		{
			this.ApplyTemplateCommand.NotifyCanExecuteChanged();
			this.GetRawResultsCommand.NotifyCanExecuteChanged();
		}
	}
}
