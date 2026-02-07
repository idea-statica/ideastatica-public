using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConApiWpfClientApp.Models;
using ConApiWpfClientApp.Services;
using ConnectionIomGenerator.UI.ViewModels;
using ConnectionIomGenerator.UI.Views;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConRestApiClientUI;
using IdeaStatiCa.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.ViewModels
{
	public partial class MainWindowViewModel : ViewModelBase
	{
		private readonly IConnectionApiService _connectionApiService;
		private readonly IConfiguration _configuration;
		private readonly IPluginLogger _logger;
		private readonly ISceneController _sceneController;
		private readonly IomEditorWindowViewModel _iomEditorViewModel;
		private readonly CancellationTokenSource _cts;

		[ObservableProperty]
		private bool _isBusy;

		[ObservableProperty]
		private bool _runApiServer;

		[ObservableProperty]
		private string? _outputText;

		[ObservableProperty]
		private ConProject? _projectInfo;

		[ObservableProperty]
		private ObservableCollection<ConnectionViewModel>? _connections;

		[ObservableProperty]
		private ConnectionViewModel? _selectedConnection;

		[ObservableProperty]
		private ConAnalysisTypeEnum _selectedAnalysisType;

		[ObservableProperty]
		private bool _calculateBuckling;

		[ObservableProperty]
		private bool _getRawXmlResults;

		[ObservableProperty]
		private IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum _weldSizingType;

		public MainWindowViewModel(
			IConnectionApiService connectionApiService,
			IConfiguration configuration,
			IPluginLogger logger,
			IomEditorWindowViewModel iomEditorViewModel,
			ISceneController sceneController)
		{
			_connectionApiService = connectionApiService;
			_configuration = configuration;
			_logger = logger;
			_sceneController = sceneController;
			_iomEditorViewModel = iomEditorViewModel;
			_cts = new CancellationTokenSource();
			_selectedAnalysisType = ConAnalysisTypeEnum.Stress_Strain;

			RunApiServer = string.IsNullOrEmpty(_configuration["CONNECTION_API_RUNSERVER"])
				? true
				: _configuration["CONNECTION_API_RUNSERVER"]! == "true";
			ApiUri = string.IsNullOrEmpty(_configuration["CONNECTION_API_RUNSERVER"])
				? null
				: new Uri(_configuration["CONNECTION_API_ENDPOINT"]!);

			Connections = new ObservableCollection<ConnectionViewModel>();
		}

		public Uri? ApiUri { get; set; }

		public bool CanStartService => !_connectionApiService.IsConnected;

		public Array AvailableAnalysisTypes => Enum.GetValues(typeof(ConAnalysisTypeEnum));

		public Array AvailableWeldSizingTypes => Enum.GetValues(typeof(IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum));

		partial void OnSelectedConnectionChanged(ConnectionViewModel? value)
		{
			RefreshConnectionChanged();
			_ = ShowClientUIAsync();
		}

		#region Commands

		[RelayCommand(CanExecute = nameof(CanConnect))]
		private async Task ConnectAsync()
		{
			_logger.LogInformation("ConnectAsync");
			IsBusy = true;
			try
			{
				OutputText = "Attaching to the ConnectionRestApi";
				var setupPath = _configuration["IdeaStatiCaSetupPath"];
				var endpoint = _configuration["CONNECTION_API_ENDPOINT"];
				await _connectionApiService.ConnectAsync(RunApiServer, setupPath, endpoint);
				OutputText = $"Service Url = {_connectionApiService.ServiceUrl}\nConnected. ClientId = {_connectionApiService.ClientId}";
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

		[RelayCommand(CanExecute = nameof(CanOpenProject))]
		private async Task OpenProjectAsync()
		{
			_logger.LogInformation("OpenProjectAsync");

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "IdeaConnection | *.ideacon";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			IsBusy = true;
			try
			{
				ProjectInfo = await _connectionApiService.OpenProjectAsync(openFileDialog.FileName);
				var projectInfoJson = Tools.JsonTools.ToFormatedJson(ProjectInfo);
				OutputText = $"ClientId = {_connectionApiService.ClientId}, ProjectId = {ProjectInfo.ProjectId}\n\n{projectInfoJson}";
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
		}

		[RelayCommand(CanExecute = nameof(CanOpenProject))]
		private async Task ImportIomAsync()
		{
			_logger.LogInformation("ImportIomAsync");

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Iom files|*.iom;*.xml";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			IsBusy = true;
			try
			{
				ProjectInfo = await _connectionApiService.ImportIomFileAsync(openFileDialog.FileName);
				var projectInfoJson = Tools.JsonTools.ToFormatedJson(ProjectInfo);
				OutputText = $"ProjectId = {ProjectInfo.ProjectId}\n\n{projectInfoJson}";
				Connections = new ObservableCollection<ConnectionViewModel>(ProjectInfo.Connections.Select(c => new ConnectionViewModel(c)));
			}
			catch (Exception ex)
			{
				_logger.LogWarning("ImportIomAsync failed", ex);
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
		}

		[RelayCommand(CanExecute = nameof(CanOpenProject))]
		private async Task ConIomGeneratorAsync()
		{
			_logger.LogInformation("GenerateConnectionIomAsync");

			IsBusy = true;
			try
			{
				var editorWindowVM = _iomEditorViewModel;
				var editorWindow = new IomEditorWindow(editorWindowVM)
				{
					Owner = System.Windows.Application.Current.MainWindow,
				};

				bool? dialogResult = editorWindow.ShowDialog();

				if (dialogResult == true && editorWindowVM?.IomEditorViewModel != null)
				{
					var model = await editorWindowVM.GetResultModelAsync();

					if (model.IomContainer != null)
					{
						_logger.LogInformation("IOM generated successfully, creating project from IOM");
						string xmlString = IdeaRS.OpenModel.Tools.OpenModelContainerToXml(model.IomContainer);
						xmlString = xmlString.Replace("utf-16", "utf-8");

						using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
						{
							memoryStream.Seek(0, SeekOrigin.Begin);
							ProjectInfo = await _connectionApiService.ImportIomStreamAsync(memoryStream, _cts.Token);
							var projectInfoJson = Tools.JsonTools.ToFormatedJson(ProjectInfo);
							OutputText = $"ProjectId = {ProjectInfo.ProjectId}\n\n{projectInfoJson}";
							Connections = new ObservableCollection<ConnectionViewModel>(ProjectInfo.Connections.Select(c => new ConnectionViewModel(c)));
						}
					}
				}
				else
				{
					OutputText = "Operation cancelled by user.";
					_logger.LogInformation("User cancelled the IOM editor dialog");
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GenerateConnectionIomAsync failed", ex);
				OutputText = $"Error: {ex.Message}\n\n{ex.StackTrace}";
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
		}

		[RelayCommand(CanExecute = nameof(HasProjectInfo))]
		private async Task CloseProjectAsync()
		{
			_logger.LogInformation("CloseProjectAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				await _connectionApiService.CloseProjectAsync(ProjectInfo.ProjectId, _cts.Token);
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
		}

		[RelayCommand(CanExecute = nameof(HasProjectInfo))]
		private async Task DownloadProjectAsync()
		{
			_logger.LogInformation("DownloadProjectAsync");

			if (ProjectInfo == null)
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
					await _connectionApiService.SaveProjectAsync(ProjectInfo.ProjectId, saveFileDialog.FileName, _cts.Token);
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
		}

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task CalculationAsync()
		{
			_logger.LogInformation("CalculateAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.CalculateAsync(
					ProjectInfo.ProjectId,
					SelectedConnection!.Id,
					SelectedAnalysisType,
					CalculateBuckling,
					GetRawXmlResults,
					_cts.Token);
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

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task GetMembersAsync()
		{
			_logger.LogInformation("GetMembersAsync");

			if (ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.GetMembersJsonAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, _cts.Token);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GetMembersAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task GetOperationsAsync()
		{
			_logger.LogInformation("GetOperationsAsync");

			if (ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.GetOperationsJsonAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, _cts.Token);
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

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task DeleteOperationsAsync()
		{
			_logger.LogInformation("DeleteOperationsAsync");

			if (ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				await _connectionApiService.DeleteOperationsAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, _cts.Token);
				OutputText = "Operations were removed";
			}
			catch (Exception ex)
			{
				_logger.LogWarning("DeleteOperationsAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
				await ShowClientUIAsync();
			}
		}

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task GenerateReportAsync(string? format)
		{
			_logger.LogInformation("GenerateReportAsync");

			if (ProjectInfo == null || format == null)
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
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = $"{format} file| *.{format}";
				if (saveFileDialog.ShowDialog() != true)
				{
					return;
				}

				if (format.Equals("pdf"))
				{
					await _connectionApiService.SaveReportPdfAsync(
						ProjectInfo.ProjectId, SelectedConnection.Id, saveFileDialog.FileName);
				}
				else if (format.Equals("docx"))
				{
					await _connectionApiService.SaveReportWordAsync(
						ProjectInfo.ProjectId, SelectedConnection.Id, saveFileDialog.FileName);
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

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task ExportAsync(string? format)
		{
			_logger.LogInformation("ExportConnectionAsync");

			if (ProjectInfo == null || format == null)
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
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = $"{format} file| *.{format}";
				if (saveFileDialog.ShowDialog() != true)
				{
					return;
				}

				if (format.Equals("iom"))
				{
					var iomContainerXml = await _connectionApiService.ExportIomAsync(
						ProjectInfo.ProjectId, SelectedConnection.Id);
					await File.WriteAllTextAsync(saveFileDialog.FileName, iomContainerXml);
					OutputText = iomContainerXml;
				}
				else if (format.Equals("ifc"))
				{
					await _connectionApiService.ExportIfcAsync(
						ProjectInfo.ProjectId, SelectedConnection.Id, saveFileDialog.FileName);
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

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task GetTopologyAsync()
		{
			_logger.LogInformation("GetTopologyAsync");

			if (ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.GetTopologyJsonAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, _cts.Token);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("GetTopologyAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task GetSceneDataAsync()
		{
			_logger.LogInformation("GetSceneDataAsync");

			if (ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.GetSceneDataJsonAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id);
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

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task EvaluateExpressionAsync()
		{
			_logger.LogInformation("EvaluateExpressionAsync");

			if (ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var expressionProvider = new ExpressionProvider(_connectionApiService.Client!, _logger);
				var expressionModel = await expressionProvider.GetExpressionAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, _cts.Token);

				if (expressionModel == null || string.IsNullOrEmpty(expressionModel.Expression))
				{
					_logger.LogInformation("EvaluateExpression - leaving. No Expression was provided");
					return;
				}

				_logger.LogInformation($"Evaluating expression: {expressionModel.Expression}");

				OutputText = await _connectionApiService.EvaluateExpressionAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, expressionModel.Expression, _cts.Token);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("EvaluateExpressionAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task WeldSizingAsync()
		{
			_logger.LogInformation("DoWeldSizingAsync");

			if (ProjectInfo == null || SelectedConnection == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.WeldSizingAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id,
					IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum.FullStrength);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("DoWeldSizingAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
				await ShowClientUIAsync();
			}
		}

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task CreateTemplateAsync()
		{
			_logger.LogInformation("CreateTemplateAsync");

			if (ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var conTempXmlString = await _connectionApiService.CreateTemplateXmlAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, _cts.Token);
				OutputText = conTempXmlString;

				if (!string.IsNullOrEmpty(conTempXmlString))
				{
					SaveFileDialog saveTemplateFileDialog = new SaveFileDialog();
					saveTemplateFileDialog.Filter = "Connection template | *.contemp";
					saveTemplateFileDialog.OverwritePrompt = true;
					if (saveTemplateFileDialog.ShowDialog() == true)
					{
						await File.WriteAllTextAsync(saveTemplateFileDialog.FileName, conTempXmlString, Encoding.Unicode);
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

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task ApplyTemplateAsync(string? source)
		{
			_logger.LogInformation("ApplyTemplateAsync");

			if (ProjectInfo == null || SelectedConnection == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				ConnectionLibraryModel? templateRes = null;
				if (source?.Equals("ConnectionLibrary", StringComparison.InvariantCultureIgnoreCase) == true)
				{
					var proposeService = new ConnectionLibraryProposer(_connectionApiService.Client!, _logger);
					templateRes = await proposeService.GetTemplateAsync(ProjectInfo.ProjectId, SelectedConnection.Id, _cts.Token);
				}
				else
				{
					ITemplateProvider templateProvider = new TemplateProviderFile();
					templateRes = await templateProvider.GetTemplateAsync(ProjectInfo.ProjectId, SelectedConnection.Id, _cts.Token);
				}

				if (templateRes == null || string.IsNullOrEmpty(templateRes.SelectedTemplateXml))
				{
					_logger.LogInformation("ApplyTemplateAsync : no template, leaving");
					return;
				}

				var getTemplateParam = new ConTemplateMappingGetParam()
				{
					Template = templateRes.SelectedTemplateXml,
				};

				if (templateRes?.SearchParameters?.Members?.Any() == true)
				{
					getTemplateParam.MemberIds = templateRes.SearchParameters.Members;
				}

				var templateMapping = await _connectionApiService.GetDefaultTemplateMappingAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, getTemplateParam, _cts.Token);

				if (templateMapping == null)
				{
					throw new ArgumentException($"Invalid mapping for connection '{SelectedConnection.Name}'");
				}

				var mappingSetter = new TemplateMappingSetter();
				var modifiedTemplateMapping = await mappingSetter.SetAsync(templateMapping);
				if (modifiedTemplateMapping == null)
				{
					return;
				}

				var applyTemplateParam = new ConTemplateApplyParam()
				{
					ConnectionTemplate = templateRes!.SelectedTemplateXml,
					Mapping = modifiedTemplateMapping,
				};

				OutputText = await _connectionApiService.ApplyTemplateAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, applyTemplateParam, _cts.Token);
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

		[RelayCommand(CanExecute = nameof(HasProjectInfo))]
		private async Task GetSettingsAsync()
		{
			_logger.LogInformation("GetSettingsAsync");

			if (ProjectInfo == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.GetSettingsJsonAsync(ProjectInfo.ProjectId, _cts.Token);
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

		[RelayCommand(CanExecute = nameof(HasProjectInfo))]
		private async Task UpdateSettingsAsync()
		{
			_logger.LogInformation("UpdateSettingsAsync");

			if (ProjectInfo == null || string.IsNullOrEmpty(OutputText))
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.UpdateSettingsAsync(
					ProjectInfo.ProjectId, OutputText!, _cts.Token);
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

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task UpdateConnectionLoadingAsync()
		{
			_logger.LogInformation("UpdateConnectionLoadingAsync");

			if (ProjectInfo == null || SelectedConnection == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var connectionLoadingData = await _connectionApiService.GetLoadEffectsAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, _cts.Token);

				if (connectionLoadingData == null || !connectionLoadingData.Any())
				{
					_logger.LogInformation("UpdateConnectionLoading : no loading for connection");
					return;
				}

				var jsonEditorService = new JsonEditorService<List<ConLoadEffect>>();
				var editedLoadEffects = await jsonEditorService.EditAsync(connectionLoadingData);

				if (editedLoadEffects == null || !editedLoadEffects.Any())
				{
					return;
				}

				await _connectionApiService.UpdateLoadEffectsAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, editedLoadEffects, _cts.Token);
				OutputText = "Loading was updated";
			}
			catch (Exception ex)
			{
				_logger.LogWarning("UpdateConnectionLoading failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
			}
		}

		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task EditParametersAsync()
		{
			_logger.LogInformation("EditParametersAsync");

			if (ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var existingParameters = await _connectionApiService.GetParametersAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, _cts.Token);

				if (existingParameters == null || !existingParameters.Any())
				{
					return;
				}

				var jsonEditorService = new JsonEditorService<List<IdeaParameter>>();
				var updatedParams = await jsonEditorService.EditAsync(existingParameters);
				if (updatedParams == null)
				{
					return;
				}

				List<IdeaParameterUpdate> parameterUpdate = updatedParams.Select(p =>
				{
					var r = new IdeaParameterUpdate();
					r.Key = p.Key;
					r.Expression = p.Expression;
					return r;
				}).ToList();

				await _connectionApiService.UpdateParametersAsync(
					ProjectInfo.ProjectId, SelectedConnection.Id, parameterUpdate, _cts.Token);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("EditParametersAsync failed", ex);
				OutputText = ex.Message;
			}
			finally
			{
				IsBusy = false;
				RefreshCommands();
				await ShowClientUIAsync();
			}
		}

		[RelayCommand]
		private void ShowLogs()
		{
			_logger.LogInformation("ShowIdeaStatiCaLogs");

			var tempPath = Environment.GetEnvironmentVariable("TEMP");
			var ideaLogDir = Path.Combine(tempPath!, "IdeaStatica", "Logs");

			try
			{
				Process.Start("explorer.exe", ideaLogDir);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("ShowLogs failed", ex);
			}
		}

		[RelayCommand]
		private void EditDiagnostics()
		{
			_logger.LogInformation("EditDiagnostics");

			string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var ideaDiagnosticsConfig = Path.Combine(localAppDataPath, "IDEA_RS", "IdeaDiagnostics.config");

			try
			{
				Process.Start("notepad.exe", ideaDiagnosticsConfig);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("EditDiagnostics failed", ex);
			}
		}

		#endregion

		internal async Task ShowClientUIAsync()
		{
			_logger.LogInformation("ShowClientUI");

			if (ProjectInfo == null)
			{
				await _sceneController.PresentAsync(string.Empty);
				return;
			}

			if (!_connectionApiService.IsConnected)
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
				var sceneJson = await _connectionApiService.GetScene3DTextAsync(
					ProjectInfo.ProjectId, SelectedConnection!.Id, _cts.Token);

				if (string.IsNullOrEmpty(sceneJson))
				{
					return;
				}

				await _sceneController.PresentAsync(sceneJson);
			}
			catch (Exception ex)
			{
				_logger.LogWarning("ShowClientUI failed", ex);
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
			return _connectionApiService.DisconnectAsync();
		}

		private void RefreshCommands()
		{
			ConnectCommand.NotifyCanExecuteChanged();
			OpenProjectCommand.NotifyCanExecuteChanged();
			ImportIomCommand.NotifyCanExecuteChanged();
			ConIomGeneratorCommand.NotifyCanExecuteChanged();
			CloseProjectCommand.NotifyCanExecuteChanged();
			DownloadProjectCommand.NotifyCanExecuteChanged();
			CalculationCommand.NotifyCanExecuteChanged();
			GetMembersCommand.NotifyCanExecuteChanged();
			GetOperationsCommand.NotifyCanExecuteChanged();
			DeleteOperationsCommand.NotifyCanExecuteChanged();
			GenerateReportCommand.NotifyCanExecuteChanged();
			ExportCommand.NotifyCanExecuteChanged();
			GetTopologyCommand.NotifyCanExecuteChanged();
			GetSceneDataCommand.NotifyCanExecuteChanged();
			CreateTemplateCommand.NotifyCanExecuteChanged();
			ApplyTemplateCommand.NotifyCanExecuteChanged();
			WeldSizingCommand.NotifyCanExecuteChanged();
			GetSettingsCommand.NotifyCanExecuteChanged();
			UpdateSettingsCommand.NotifyCanExecuteChanged();
			UpdateConnectionLoadingCommand.NotifyCanExecuteChanged();
			EvaluateExpressionCommand.NotifyCanExecuteChanged();
			EditParametersCommand.NotifyCanExecuteChanged();

			OnPropertyChanged(nameof(CanStartService));
		}

		private void RefreshConnectionChanged()
		{
			ApplyTemplateCommand.NotifyCanExecuteChanged();
			CalculationCommand.NotifyCanExecuteChanged();
		}

		private bool CanConnect() => !_connectionApiService.IsConnected;
		private bool CanOpenProject() => _connectionApiService.IsConnected && ProjectInfo == null;
		private bool HasProjectInfo() => ProjectInfo != null;
		private bool HasSelectedConnection() => SelectedConnection != null;
	}
}
