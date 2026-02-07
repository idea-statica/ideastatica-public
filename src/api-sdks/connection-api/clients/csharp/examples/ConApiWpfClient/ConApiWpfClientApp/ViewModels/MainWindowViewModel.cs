using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConApiWpfClientApp.Models;
using ConApiWpfClientApp.Services;
using ConnectionIomGenerator.UI.Services;
using ConnectionIomGenerator.UI.ViewModels;
using ConnectionIomGenerator.UI.Views;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConRestApiClientUI;
using IdeaStatiCa.Plugin;
using Microsoft.Extensions.Configuration;
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
	/// <summary>
	/// Main view model for the Connection API WPF client application.
	/// Manages the application state, user interactions, and delegates API operations
	/// to <see cref="IConnectionApiService"/>.
	/// </summary>
	public partial class MainWindowViewModel : ViewModelBase
	{
		private readonly IConnectionApiService _connectionApiService;
		private readonly IFileDialogService _fileDialogService;
		private readonly IConfiguration _configuration;
		private readonly IPluginLogger _logger;
		private readonly ISceneController _sceneController;
		private readonly IomEditorWindowViewModel _iomEditorViewModel;
		private readonly CancellationTokenSource _cts;

		/// <summary>
		/// Gets or sets a value indicating whether a long-running operation is in progress.
		/// When <see langword="true"/>, the UI should display a busy indicator.
		/// </summary>
		[ObservableProperty]
		private bool _isBusy;

		/// <summary>
		/// Gets or sets a value indicating whether to start a new API server process
		/// or attach to an existing endpoint.
		/// </summary>
		[ObservableProperty]
		private bool _runApiServer;

		/// <summary>
		/// Gets or sets the text displayed in the output panel, typically containing
		/// JSON results, status messages, or error information.
		/// </summary>
		[ObservableProperty]
		private string? _outputText;

		/// <summary>
		/// Gets the project model holding the currently open project data.
		/// </summary>
		public ProjectModel Model { get; }

		/// <summary>
		/// Gets or sets the collection of connections in the current project.
		/// </summary>
		[ObservableProperty]
		private ObservableCollection<ConnectionViewModel>? _connections;

		/// <summary>
		/// Gets or sets the currently selected connection. When changed, triggers
		/// a 3D scene refresh and command state update.
		/// </summary>
		[ObservableProperty]
		private ConnectionViewModel? _selectedConnection;

		/// <summary>
		/// Gets or sets the analysis type to use for calculations (e.g., Stress_Strain, Stiffness).
		/// </summary>
		[ObservableProperty]
		private ConAnalysisTypeEnum _selectedAnalysisType;

		/// <summary>
		/// Gets or sets a value indicating whether buckling analysis should be included in calculations.
		/// </summary>
		[ObservableProperty]
		private bool _calculateBuckling;

		/// <summary>
		/// Gets or sets a value indicating whether raw XML results should be retrieved after calculation.
		/// </summary>
		[ObservableProperty]
		private bool _getRawXmlResults;

		/// <summary>
		/// Gets or sets the weld sizing method to use for weld pre-design operations.
		/// </summary>
		[ObservableProperty]
		private IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum _weldSizingType;

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
		/// </summary>
		/// <param name="connectionApiService">The service for Connection API operations.</param>
		/// <param name="configuration">The application configuration providing setup paths and endpoints.</param>
		/// <param name="logger">The logger for diagnostic output.</param>
		/// <param name="iomEditorViewModel">The view model for the IOM editor dialog.</param>
		/// <param name="fileDialogService">The service for displaying file open/save dialogs.</param>
		/// <param name="projectModel">The project model holding the currently open project data.</param>
		/// <param name="sceneController">The controller for rendering 3D connection scenes.</param>
		public MainWindowViewModel(
			IConnectionApiService connectionApiService,
			IFileDialogService fileDialogService,
			IConfiguration configuration,
			IPluginLogger logger,
			IomEditorWindowViewModel iomEditorViewModel,
			ProjectModel projectModel,
			ISceneController sceneController)
		{
			_connectionApiService = connectionApiService;
			_fileDialogService = fileDialogService;
			Model = projectModel;
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

		/// <summary>
		/// Gets or sets the API endpoint URI used when attaching to an existing service.
		/// </summary>
		public Uri? ApiUri { get; set; }

		/// <summary>
		/// Gets a value indicating whether the Connect command should be available.
		/// Returns <see langword="true"/> when not yet connected to the API service.
		/// </summary>
		public bool CanStartService => !_connectionApiService.IsConnected;

		/// <summary>
		/// Gets all available analysis types for the analysis type selector.
		/// </summary>
		public Array AvailableAnalysisTypes => Enum.GetValues(typeof(ConAnalysisTypeEnum));

		/// <summary>
		/// Gets all available weld sizing methods for the weld sizing selector.
		/// </summary>
		public Array AvailableWeldSizingTypes => Enum.GetValues(typeof(IdeaStatiCa.Api.Connection.Model.Connection.ConWeldSizingMethodEnum));

		/// <summary>
		/// Called when <see cref="SelectedConnection"/> changes. Refreshes connection-dependent
		/// commands and updates the 3D scene.
		/// </summary>
		/// <param name="value">The newly selected connection, or <see langword="null"/>.</param>
		partial void OnSelectedConnectionChanged(ConnectionViewModel? value)
		{
			RefreshConnectionChanged();
			_ = ShowClientUIAsync();
		}

		#region Commands

		/// <summary>
		/// Connects to the Connection API service using the configured mode (run server or attach to endpoint).
		/// </summary>
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

		/// <summary>
		/// Opens an IDEA Connection project file (.ideacon) selected by the user via a file dialog.
		/// </summary>
		[RelayCommand(CanExecute = nameof(CanOpenProject))]
		private async Task OpenProjectAsync()
		{
			_logger.LogInformation("OpenProjectAsync");

			var filePath = _fileDialogService.ShowOpenFileDialog("IdeaConnection | *.ideacon", "Open Project");
			if (filePath == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				Model.ProjectInfo = await _connectionApiService.OpenProjectAsync(filePath);
				var projectInfoJson = Tools.JsonTools.ToFormatedJson(Model.ProjectInfo);
				OutputText = $"ClientId = {_connectionApiService.ClientId}, ProjectId = {Model.ProjectId}\n\n{projectInfoJson}";
				Connections = new ObservableCollection<ConnectionViewModel>(Model.ProjectInfo.Connections.Select(c => new ConnectionViewModel(c)));
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

		/// <summary>
		/// Imports an IOM (IDEA Open Model) file selected by the user and creates a new project.
		/// </summary>
		[RelayCommand(CanExecute = nameof(CanOpenProject))]
		private async Task ImportIomAsync()
		{
			_logger.LogInformation("ImportIomAsync");

			var filePath = _fileDialogService.ShowOpenFileDialog("Iom files|*.iom;*.xml", "Import IOM");
			if (filePath == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				Model.ProjectInfo = await _connectionApiService.ImportIomFileAsync(filePath);
				var projectInfoJson = Tools.JsonTools.ToFormatedJson(Model.ProjectInfo);
				OutputText = $"ProjectId = {Model.ProjectId}\n\n{projectInfoJson}";
				Connections = new ObservableCollection<ConnectionViewModel>(Model.ProjectInfo.Connections.Select(c => new ConnectionViewModel(c)));
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

		/// <summary>
		/// Opens the IOM generator dialog to create a connection from user-defined input,
		/// then imports the generated IOM into a new project.
		/// </summary>
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
							Model.ProjectInfo = await _connectionApiService.ImportIomStreamAsync(memoryStream, _cts.Token);
							var projectInfoJson = Tools.JsonTools.ToFormatedJson(Model.ProjectInfo);
							OutputText = $"ProjectId = {Model.ProjectId}\n\n{projectInfoJson}";
							Connections = new ObservableCollection<ConnectionViewModel>(Model.ProjectInfo.Connections.Select(c => new ConnectionViewModel(c)));
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

		/// <summary>
		/// Closes the currently open project and clears the UI state.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasProjectInfo))]
		private async Task CloseProjectAsync()
		{
			_logger.LogInformation("CloseProjectAsync");

			if (Model.ProjectInfo == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				await _connectionApiService.CloseProjectAsync(Model.ProjectId, _cts.Token);
				Model.Clear();
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

		/// <summary>
		/// Saves the current project to a file chosen by the user via a save file dialog.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasProjectInfo))]
		private async Task DownloadProjectAsync()
		{
			_logger.LogInformation("DownloadProjectAsync");

			if (Model.ProjectInfo == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var filePath = _fileDialogService.ShowSaveFileDialog("IdeaConnection | *.ideacon", ".ideacon", "", "Download Project");
				if (filePath != null)
				{
					await _connectionApiService.SaveProjectAsync(Model.ProjectId, filePath, _cts.Token);
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

		/// <summary>
		/// Runs a structural analysis calculation on the selected connection using the current analysis settings.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task CalculationAsync()
		{
			_logger.LogInformation("CalculateAsync");

			if (Model.ProjectInfo == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.CalculateAsync(
					Model.ProjectId,
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

		/// <summary>
		/// Retrieves and displays the members (beams/columns) of the selected connection.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task GetMembersAsync()
		{
			_logger.LogInformation("GetMembersAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.GetMembersJsonAsync(
					Model.ProjectId, SelectedConnection.Id, _cts.Token);
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

		/// <summary>
		/// Retrieves and displays the manufacturing operations of the selected connection.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task GetOperationsAsync()
		{
			_logger.LogInformation("GetOperationsAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.GetOperationsJsonAsync(
					Model.ProjectId, SelectedConnection.Id, _cts.Token);
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

		/// <summary>
		/// Deletes all manufacturing operations from the selected connection and refreshes the 3D scene.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task DeleteOperationsAsync()
		{
			_logger.LogInformation("DeleteOperationsAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				await _connectionApiService.DeleteOperationsAsync(
					Model.ProjectId, SelectedConnection.Id, _cts.Token);
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

		/// <summary>
		/// Generates a report for the selected connection in the specified format and saves it to a user-chosen file.
		/// </summary>
		/// <param name="format">The report format: "pdf" or "docx". Passed via XAML CommandParameter.</param>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task GenerateReportAsync(string? format)
		{
			_logger.LogInformation("GenerateReportAsync");

			if (Model.ProjectInfo == null || format == null)
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
				var filePath = _fileDialogService.ShowSaveFileDialog($"{format} file| *.{format}", $".{format}", "", "Generate Report");
				if (filePath == null)
				{
					return;
				}

				if (format.Equals("pdf"))
				{
					await _connectionApiService.SaveReportPdfAsync(
						Model.ProjectId, SelectedConnection.Id, filePath);
				}
				else if (format.Equals("docx"))
				{
					await _connectionApiService.SaveReportWordAsync(
						Model.ProjectId, SelectedConnection.Id, filePath);
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

		/// <summary>
		/// Exports the selected connection in the specified format and saves it to a user-chosen file.
		/// </summary>
		/// <param name="format">The export format: "iom" or "ifc". Passed via XAML CommandParameter.</param>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task ExportAsync(string? format)
		{
			_logger.LogInformation("ExportConnectionAsync");

			if (Model.ProjectInfo == null || format == null)
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
				var filePath = _fileDialogService.ShowSaveFileDialog($"{format} file| *.{format}", $".{format}", "", "Export Connection");
				if (filePath == null)
				{
					return;
				}

				if (format.Equals("iom"))
				{
					var iomContainerXml = await _connectionApiService.ExportIomAsync(
						Model.ProjectId, SelectedConnection.Id);
					await File.WriteAllTextAsync(filePath, iomContainerXml);
					OutputText = iomContainerXml;
				}
				else if (format.Equals("ifc"))
				{
					await _connectionApiService.ExportIfcAsync(
						Model.ProjectId, SelectedConnection.Id, filePath);
					var ifc = await File.ReadAllTextAsync(filePath);
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

		/// <summary>
		/// Retrieves and displays the topology (geometry and connectivity data) of the selected connection.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task GetTopologyAsync()
		{
			_logger.LogInformation("GetTopologyAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.GetTopologyJsonAsync(
					Model.ProjectId, SelectedConnection.Id, _cts.Token);
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

		/// <summary>
		/// Retrieves and displays the raw 3D scene data JSON for the selected connection.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task GetSceneDataAsync()
		{
			_logger.LogInformation("GetSceneDataAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.GetSceneDataJsonAsync(
					Model.ProjectId, SelectedConnection.Id);
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

		/// <summary>
		/// Evaluates a parametric expression in the context of the selected connection.
		/// Opens a text editor for the user to modify the expression before evaluation.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task EvaluateExpressionAsync()
		{
			_logger.LogInformation("EvaluateExpressionAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var expressionProvider = new ExpressionProvider(_connectionApiService.Client!, _logger);
				var expressionModel = await expressionProvider.GetExpressionAsync(
					Model.ProjectId, SelectedConnection.Id, _cts.Token);

				if (expressionModel == null || string.IsNullOrEmpty(expressionModel.Expression))
				{
					_logger.LogInformation("EvaluateExpression - leaving. No Expression was provided");
					return;
				}

				_logger.LogInformation($"Evaluating expression: {expressionModel.Expression}");

				OutputText = await _connectionApiService.EvaluateExpressionAsync(
					Model.ProjectId, SelectedConnection.Id, expressionModel.Expression, _cts.Token);
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

		/// <summary>
		/// Performs weld pre-design (sizing) on the selected connection using the full-strength method
		/// and refreshes the 3D scene.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task WeldSizingAsync()
		{
			_logger.LogInformation("DoWeldSizingAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.WeldSizingAsync(
					Model.ProjectId, SelectedConnection.Id,
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

		/// <summary>
		/// Creates a connection template XML from the selected connection and optionally saves it to a file.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task CreateTemplateAsync()
		{
			_logger.LogInformation("CreateTemplateAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var conTempXmlString = await _connectionApiService.CreateTemplateXmlAsync(
					Model.ProjectId, SelectedConnection.Id, _cts.Token);
				OutputText = conTempXmlString;

				if (!string.IsNullOrEmpty(conTempXmlString))
				{
					var filePath = _fileDialogService.ShowSaveFileDialog("Connection template | *.contemp", ".contemp", "", "Save Template");
					if (filePath != null)
					{
						await File.WriteAllTextAsync(filePath, conTempXmlString, Encoding.Unicode);
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

		/// <summary>
		/// Applies a connection template to the selected connection. The template can be loaded
		/// from a file or from the connection library, depending on the source parameter.
		/// </summary>
		/// <param name="source">The template source: "FromFile" to load from disk, or "ConnectionLibrary"
		/// to browse and select from the connection library. Passed via XAML CommandParameter.</param>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task ApplyTemplateAsync(string? source)
		{
			_logger.LogInformation("ApplyTemplateAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null)
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
					templateRes = await proposeService.GetTemplateAsync(Model.ProjectId, SelectedConnection.Id, _cts.Token);
				}
				else
				{
					ITemplateProvider templateProvider = new TemplateProviderFile();
					templateRes = await templateProvider.GetTemplateAsync(Model.ProjectId, SelectedConnection.Id, _cts.Token);
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
					Model.ProjectId, SelectedConnection.Id, getTemplateParam, _cts.Token);

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
					Model.ProjectId, SelectedConnection.Id, applyTemplateParam, _cts.Token);
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

		/// <summary>
		/// Retrieves and displays the project settings as formatted JSON.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasProjectInfo))]
		private async Task GetSettingsAsync()
		{
			_logger.LogInformation("GetSettingsAsync");

			if (Model.ProjectInfo == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.GetSettingsJsonAsync(Model.ProjectId, _cts.Token);
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

		/// <summary>
		/// Updates the project settings using the JSON currently displayed in the output panel.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasProjectInfo))]
		private async Task UpdateSettingsAsync()
		{
			_logger.LogInformation("UpdateSettingsAsync");

			if (Model.ProjectInfo == null || string.IsNullOrEmpty(OutputText))
			{
				return;
			}

			IsBusy = true;
			try
			{
				OutputText = await _connectionApiService.UpdateSettingsAsync(
					Model.ProjectId, OutputText!, _cts.Token);
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

		/// <summary>
		/// Opens a JSON editor for the user to modify the load effects of the selected connection,
		/// then updates them on the server.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task UpdateConnectionLoadingAsync()
		{
			_logger.LogInformation("UpdateConnectionLoadingAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var connectionLoadingData = await _connectionApiService.GetLoadEffectsAsync(
					Model.ProjectId, SelectedConnection.Id, _cts.Token);

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
					Model.ProjectId, SelectedConnection.Id, editedLoadEffects, _cts.Token);
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

		/// <summary>
		/// Opens a JSON editor for the user to modify the parameters of the selected connection,
		/// then updates them on the server and refreshes the 3D scene.
		/// </summary>
		[RelayCommand(CanExecute = nameof(HasSelectedConnection))]
		private async Task EditParametersAsync()
		{
			_logger.LogInformation("EditParametersAsync");

			if (Model.ProjectInfo == null || SelectedConnection == null || SelectedConnection.Id < 1)
			{
				return;
			}

			IsBusy = true;
			try
			{
				var existingParameters = await _connectionApiService.GetParametersAsync(
					Model.ProjectId, SelectedConnection.Id, _cts.Token);

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
					Model.ProjectId, SelectedConnection.Id, parameterUpdate, _cts.Token);
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

		/// <summary>
		/// Opens Windows Explorer at the IDEA StatiCa logs directory.
		/// </summary>
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

		/// <summary>
		/// Opens the IDEA StatiCa diagnostics configuration file in Notepad for editing.
		/// </summary>
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

		/// <summary>
		/// Updates the 3D scene visualization for the currently selected connection.
		/// Clears the scene if no project is open or no connection is selected.
		/// </summary>
		internal async Task ShowClientUIAsync()
		{
			_logger.LogInformation("ShowClientUI");

			if (Model.ProjectInfo == null)
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
					Model.ProjectId, SelectedConnection!.Id, _cts.Token);

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

		/// <summary>
		/// Performs cleanup when the application is exiting by disconnecting from the API service.
		/// </summary>
		/// <returns>A task representing the asynchronous disconnect operation.</returns>
		public Task OnExitApplication()
		{
			return _connectionApiService.DisconnectAsync();
		}

		/// <summary>
		/// Notifies all commands that their CanExecute state may have changed.
		/// </summary>
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

		/// <summary>
		/// Notifies connection-dependent commands that the selected connection has changed.
		/// </summary>
		private void RefreshConnectionChanged()
		{
			ApplyTemplateCommand.NotifyCanExecuteChanged();
			CalculationCommand.NotifyCanExecuteChanged();
		}

		private bool CanConnect() => !_connectionApiService.IsConnected;
		private bool CanOpenProject() => _connectionApiService.IsConnected && Model.ProjectInfo == null;
		private bool HasProjectInfo() => Model.ProjectInfo != null;
		private bool HasSelectedConnection() => SelectedConnection != null;
	}
}
