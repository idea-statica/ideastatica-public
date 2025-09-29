using CommunityToolkit.Mvvm.Input;
using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConRestApiClientUI;
using IdeaStatiCa.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
		private readonly ISceneController _sceneController;

		private bool _isBusy;
		private bool _runApiServer;

		private string? outputText; 
		ObservableCollection<ConnectionViewModel>? connectionsVM;
		ConnectionViewModel? selectedConnection;
		private ConProject? _projectInfo;
		private CancellationTokenSource cts;
		private ConAnalysisTypeEnum _conAnalysisType;
		private bool _calculateBuckling;
		private bool _getRawResults;
		//private static readonly JsonSerializerOptions jsonPresentationOptions = new JsonSerializerOptions() { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Default };

		private bool disposedValue;

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
			ShowLogsCommand = new AsyncRelayCommand(ShowIdeaStatiCaLogsAsync);
			EditDiagnosticsCommand = new AsyncRelayCommand(EditDiagnosticsAsync);
			ConnectCommand = new AsyncRelayCommand(ConnectAsync, () => ConApiClient == null);
			OpenProjectCommand = new AsyncRelayCommand(OpenProjectAsync, () => ConApiClient != null && this.ProjectInfo == null);
			ImportIomCommand = new AsyncRelayCommand(ImportIomAsync, () => ConApiClient != null && this.ProjectInfo == null);
			CloseProjectCommand = new AsyncRelayCommand(CloseProjectAsync, () => this.ProjectInfo != null);

			DownloadProjectCommand = new AsyncRelayCommand(DownloadProjectAsync, () => this.ProjectInfo != null);

			ApplyTemplateCommand = new AsyncRelayCommand(ApplyTemplateAsync, () => SelectedConnection != null);

			CreateTemplateCommand = new AsyncRelayCommand(CreateTemplateAsync, () => SelectedConnection != null);

			GetTopologyCommand = new AsyncRelayCommand(GetTopologyAsync, () => SelectedConnection != null);
			GetSceneDataCommand = new AsyncRelayCommand(GetSceneDataAsync, () => SelectedConnection != null);

			CalculationCommand = new AsyncRelayCommand(CalculateAsync, () => SelectedConnection != null);

			GetOperationsCommand = new AsyncRelayCommand(GetOperationsAsync, () => SelectedConnection != null);

			GenerateReportCommand = new AsyncRelayCommand<object>(GenerateReportAsync, (param) => SelectedConnection != null);

			ExportCommand = new AsyncRelayCommand<object>(ExportConnectionAsync, (param) => SelectedConnection != null);

			ShowClientUICommand = new AsyncRelayCommand(ShowClientUIAsync, () => this.ProjectInfo != null);

			Connections = new ObservableCollection<ConnectionViewModel>();
			selectedConnection = null;
		}

		private async Task CalculateAsync()
		{
			_logger.LogInformation("CalculateAsync");

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

				ConCalculationParameter calculationParameter = new ConCalculationParameter()
				{
					AnalysisType = this.SelectedAnalysisType,
					ConnectionIds = connectionIdList
				};

				var selectedConData = await ConApiClient.Connection.GetConnectionAsync(ProjectInfo.ProjectId, SelectedConnection!.Id, 0, cts.Token);
				if(selectedConData.AnalysisType != SelectedAnalysisType ||
					selectedConData.IncludeBuckling != CalculateBuckling)
				{
					selectedConData.AnalysisType = SelectedAnalysisType;
					selectedConData.IncludeBuckling = CalculateBuckling;
					await ConApiClient.Connection.UpdateConnectionAsync(ProjectInfo.ProjectId, SelectedConnection!.Id, selectedConData, 0, cts.Token);
				}

				var calculationResults = await ConApiClient.Calculation.CalculateAsync(ProjectInfo.ProjectId, connectionIdList, 0, cts.Token);

				string rawResultsXml = string.Empty;

				if (GetRawXmlResults)
				{
					var rawResults = await ConApiClient.Calculation.GetRawJsonResultsAsync(ProjectInfo.ProjectId, calculationParameter, 0, cts.Token);
					rawResultsXml = rawResults!.Any() ? rawResults[0] : string.Empty;
				}

				OutputText = $"{ConApiWpfClientApp.Tools.JsonTools.ToFormatedJson(calculationResults)}\n\n{rawResultsXml}";
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

		public AsyncRelayCommand ConnectCommand { get; }

		public AsyncRelayCommand OpenProjectCommand { get; }

		public AsyncRelayCommand ImportIomCommand { get; }

		public AsyncRelayCommand CalculationCommand { get; }

		public AsyncRelayCommand CloseProjectCommand { get; }

		public AsyncRelayCommand DownloadProjectCommand { get; }

		public AsyncRelayCommand ApplyTemplateCommand { get; }

		public AsyncRelayCommand GetOperationsCommand { get; }

		public AsyncRelayCommand CreateTemplateCommand { get; }

		public AsyncRelayCommand GetSceneDataCommand { get; }
		public AsyncRelayCommand GetTopologyCommand { get; }

		public AsyncRelayCommand<object> GenerateReportCommand { get; }

		public AsyncRelayCommand<object> ExportCommand { get; }

		//public AsyncRelayCommand GetSceneDataCommand { get; }

		public AsyncRelayCommand ShowClientUICommand { get; }

		public AsyncRelayCommand ShowLogsCommand { get; }

		public AsyncRelayCommand EditDiagnosticsCommand { get; }


		private async Task ShowIdeaStatiCaLogsAsync()
		{
			_logger.LogInformation("ShowIdeaStatiCaLogsAsync");

			var tempPath = Environment.GetEnvironmentVariable("TEMP");
			var ideaLogDir = Path.Combine(tempPath!, "IdeaStatica", "Logs");

			try
			{
				Process.Start("explorer.exe", ideaLogDir);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("Error", ex);
			}

			await Task.CompletedTask;
		}

		private async Task EditDiagnosticsAsync()
		{
			_logger.LogInformation("EditDiagnosticsAsync");

			string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var ideaDiagnosticsConfig = Path.Combine(localAppDataPath!, "IDEA_RS", "IdeaDiagnostics.config");

			try
			{
				Process.Start("notepad.exe", ideaDiagnosticsConfig);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("Error", ex);
			}

			await Task.CompletedTask;
		}

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
				
				OutputText = string.Format("ClientId = {0}, ProjectId = {1}\n\n{2}", ConApiClient.ClientId, ConApiClient.ActiveProjectId, projectInfoJson);

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


				if (Connections?.Any() == true)
				{
					SelectedConnection = Connections.First();
				}
				else
				{
					SelectedConnection = null;
				}
			}

			await Task.CompletedTask;
		}

		private async Task ImportIomAsync()
		{
			_logger.LogInformation("\t\tprivate async Task ImportIomAsync()\r\n");

			if (ConApiClient == null)
			{
				throw new Exception("IConnectionApiClient is not connected");
			}

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Iom files|*.iom;*.xml";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			IsBusy = true;
			try
			{
				ProjectInfo = await ConApiClient.Project.CreateProjectFromIomFileAsync(openFileDialog.FileName);

				var projectInfoJson = Tools.JsonTools.ToFormatedJson(ProjectInfo);


				OutputText = string.Format("ProjectId = {0}\n\n{1}", ConApiClient.ActiveProjectId, projectInfoJson);

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

				if (Connections?.Any() == true)
				{
					SelectedConnection = Connections.First();
				}
				else
				{
					SelectedConnection = null;
				}
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

		private async Task CreateTemplateAsync()
		{
			_logger.LogInformation("CreateTemplateAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			if(SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			string conTempXmlString = string.Empty;

			IsBusy = true;
			try
			{
				conTempXmlString = await ConApiClient.Template.CreateConTemplateAsync(ProjectInfo.ProjectId, SelectedConnection.Id, 0, cts.Token);
				OutputText = conTempXmlString;

				if(!string.IsNullOrEmpty(conTempXmlString))
				{
					SaveFileDialog saveTemplateFileDialog = new SaveFileDialog();
					saveTemplateFileDialog.Filter = "Connection template | *.contemp";
					saveTemplateFileDialog.OverwritePrompt = true;
					if (saveTemplateFileDialog.ShowDialog() == true)
					{
						await File.WriteAllTextAsync(saveTemplateFileDialog.FileName, conTempXmlString, System.Text.Encoding.Unicode);
					}
				}
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

		private async Task GetTopologyAsync()
		{
			_logger.LogInformation("CreateTemplateAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			if (SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			var topologyJsonString = string.Empty;

			IsBusy = true;
			try
			{
				topologyJsonString = await ConApiClient.Template.GetConnectionTopologyAsync(ProjectInfo.ProjectId, SelectedConnection.Id, 0, cts.Token);

				if (string.IsNullOrEmpty(topologyJsonString))
				{
					OutputText = topologyJsonString;
				}
				else
				{
					dynamic? typology = JsonConvert.DeserializeObject(topologyJsonString!);

					if(typology != null)
					{
						var topologyCode = typology["typologyCode_V2"];

						OutputText = $"typologyCode_V2 = '{topologyCode}'\n\nConnection topology :\n{topologyJsonString}";
					}
					else
					{
						OutputText = "Error";
					}
	
				}

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

				await ShowClientUIAsync();
			}
		}

		private async Task GetOperationsAsync()
		{
			_logger.LogInformation("GetOperationsAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			if (SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var operations = await ConApiClient.Operation.GetOperationsAsync(ProjectInfo.ProjectId,
					SelectedConnection!.Id, 0, cts.Token);

				if(operations == null)
				{
					OutputText = "No operations";
				}
				else
				{
					OutputText = ConApiWpfClientApp.Tools.JsonTools.ToFormatedJson(operations);
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GetOperationsAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		private async Task GenerateReportAsync(object? parameter)
		{
			_logger.LogInformation("GenerateReportAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (parameter == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			if (SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			var format = parameter.ToString();

			IsBusy = true;
			try
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = $"{format} file| *.{format}";
				if (saveFileDialog.ShowDialog() != true)
				{
					return;
				}

				if(format!.Equals("pdf"))
				{
					await ConApiClient.Report.SaveReportPdfAsync(ProjectInfo.ProjectId, SelectedConnection.Id, saveFileDialog.FileName);
					
				}
				else if(format.Equals("docx"))
				{
					await ConApiClient.Report.SaveReportWordAsync(ProjectInfo.ProjectId, SelectedConnection.Id, saveFileDialog.FileName);
				}
				else
				{
					throw new Exception($"Unsupported format {format}");
				}

				OutputText = "Done";
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GenerateReportAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		private async Task GetSceneDataAsync()
		{
			_logger.LogInformation("GetSceneDataAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			if (SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}


			IsBusy = true;
			try
			{
				string sceneDataJson = await ConApiClient.Presentation.GetDataScene3DTextAsync(ProjectInfo.ProjectId, SelectedConnection!.Id);
				OutputText = sceneDataJson;
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GetSceneDataAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}
		
		private async Task ExportConnectionAsync(object? parameter)
		{
			_logger.LogInformation("ExportConnectionAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			if (parameter == null)
			{
				return;
			}

			if (ConApiClient == null)
			{
				return;
			}

			if (SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			var format = parameter.ToString();

			IsBusy = true;
			try
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = $"{format} file| *.{format}";
				if (saveFileDialog.ShowDialog() != true)
				{
					return;
				}

				if (format!.Equals("iom"))
				{
					var iomContainerXml = await ConApiClient.Export.ExportIomAsync(ProjectInfo.ProjectId, SelectedConnection.Id);
					await File.WriteAllTextAsync(saveFileDialog.FileName, iomContainerXml);
					OutputText = iomContainerXml;
				}
				else if (format.Equals("ifc"))
				{
					await ConApiClient.Export.ExportIfcFileAsync(ProjectInfo.ProjectId, SelectedConnection.Id, saveFileDialog.FileName);
					var ifc = await File.ReadAllTextAsync(saveFileDialog.FileName);
					OutputText = ifc;
				}
				else
				{
					throw new Exception($"Unsupported format {format}");
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("ExportConnectionAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		private async Task ShowClientUIAsync()
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
			this.ImportIomCommand.NotifyCanExecuteChanged();
			this.CloseProjectCommand.NotifyCanExecuteChanged();
			this.DownloadProjectCommand.NotifyCanExecuteChanged();
			this.ApplyTemplateCommand.NotifyCanExecuteChanged();
			this.CalculationCommand.NotifyCanExecuteChanged();
			this.CreateTemplateCommand.NotifyCanExecuteChanged();
			this.GetTopologyCommand.NotifyCanExecuteChanged();
			this.GetSceneDataCommand.NotifyCanExecuteChanged();
			this.GetOperationsCommand.NotifyCanExecuteChanged();
			this.GenerateReportCommand.NotifyCanExecuteChanged();
			this.ExportCommand.NotifyCanExecuteChanged();
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
