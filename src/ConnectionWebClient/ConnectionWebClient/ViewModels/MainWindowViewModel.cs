using CommunityToolkit.Mvvm.Input;
using ConnectionWebClient.Tools;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.ConnectionRest;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
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

			//GetBriefResultsCommand = new AsyncRelayCommand(GetBriefResultsAsync);
			//GetDetailedResultsCommand = new AsyncRelayCommand(GetDetailedResultsAsync);

			//GetBucklingBriefResultsCommand = new AsyncRelayCommand(GetBucklingBriefResultsAsync);
			//GetBucklingDetailedResultsCommand = new AsyncRelayCommand(GetBucklingDetailedResultsAsync);
			Connections = new ObservableCollection<ConnectionViewModel>();
			selectedConnection = null;
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

		public AsyncRelayCommand CloseProjectCommand { get; }
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
				_logger.LogWarning("ConnectAsync", ex);
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
			if(ProjectInfo == null)
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
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}

			await Task.CompletedTask;
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

		private void RefreshCommangs()
		{
			this.ConnectCommand.NotifyCanExecuteChanged();
			this.OpenProjectCommand.NotifyCanExecuteChanged();
			this.CloseProjectCommand.NotifyCanExecuteChanged();
		}
		private void RefreshCommands()
		{
			this.ConnectCommand.NotifyCanExecuteChanged();
			this.OpenProjectCommand.NotifyCanExecuteChanged();
			this.CloseProjectCommand.NotifyCanExecuteChanged();
		}
	}
}
