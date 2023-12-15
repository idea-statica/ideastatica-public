using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.ConnectionClient.ConHiddenCalcCommands;
using IdeaStatiCa.ConnectionClient.Model;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ConnectionHiddenCalculation
{
	/// <summary>
	/// Main view model of the example
	/// </summary>
	public class MainVM : INotifyPropertyChanged, IConHiddenCalcModel
	{
		#region private fields
		private static IdeaStatiCa.Plugin.IPluginLogger Logger { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;
		bool isIdea;
		string statusMessage;
		string ideaStatiCaDir;
		ObservableCollection<ConnectionVM> connections;
		string results;
		ConnHiddenClientFactory CalcFactory { get; set; }
		ConnectionHiddenCheckClient IdeaConnectionClient { get; set; }
		IConnHiddenCheck service;
		string newBoltAssemblyName;
		string templateSettingString;
		ApplyConnTemplateSetting templateSetting;
		readonly JsonSerializerSettings jsonSerializerSettings;
		int supportingMember;
		int attachedMember;
		IConnectionController connectionController;
		readonly string ideaConnExeFileName;
		private string ideaConTempFileName;
		private string expression;
		private IConnectionId selectedConnection;
		#endregion

		#region Constructor
		static MainVM()
		{
			// initialize logger
			SerilogFacade.Initialize();
			Logger = LoggerProvider.GetLogger("feappexample");
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public MainVM()
		{
			Logger.LogInformation($"'{Assembly.GetEntryAssembly().GetName().Name}' is starting");
			NewBoltAssemblyName = "M12 4.6";
			SupportingMember = 1;
			AttachedMember = 2;
			connections = new ObservableCollection<ConnectionVM>();
			ideaStatiCaDir = Properties.Settings.Default.IdeaStatiCaDir;
			if (Directory.Exists(ideaStatiCaDir))
			{
				ideaConnExeFileName = Path.Combine(ideaStatiCaDir, "IdeaConnection.exe");
				if (File.Exists(ideaConnExeFileName))
				{
					IsIdea = true;
					StatusMessage = string.Format("IdeaStatiCa installation was found in '{0}'", ideaStatiCaDir);
					CalcFactory = new ConnHiddenClientFactory(ideaStatiCaDir, Logger);
				}
			}

			if (!IsIdea)
			{
				StatusMessage = string.Format("ERROR IdeaStatiCa doesn't exist in '{0}'", ideaStatiCaDir);
			}

			OpenProjectCmd = new OpenProjectCommand(this, Logger);
			ImportIOMCmd = new ImportIOMCommand(this);
			UpdateIOMCmd = new UpdateIOMCommand(this);
			CloseProjectCmd = new CloseProjectCommand(this, Logger);
			CalculateConnectionCmd = new CalculateConnectionCommand(this);
			CalculateBucklingConnectionCmd = new CalculateBucklingCommand(this);
			ApplySimpleTemplateCmd = new ApplySimpleTemplateCommand(this);
			ConnectionGeometryCmd = new ConnectionGeometryCommand(this);
			SaveAsProjectCmd = new SaveAsProjectCommand(this);
			SaveProjectCmd = new SaveProjectCommand(this);
			ConnectionToTemplateCmd = new ConnectionToTemplateCommand(this);
			ApplyTemplateCmd = new ApplyTemplateCommand(this);
			DeleteOperationsCmd = new DeleteAllOperationsCommand(this);
			GenerateReportCmd = new GenerateReportCommand(this, ConnReportTypeEnum.Zip);
			GeneratePdfReportCmd = new GenerateReportCommand(this, ConnReportTypeEnum.Pdf);
			GenerateWordReportCmd = new GenerateReportCommand(this, ConnReportTypeEnum.Word);

			GetConnectionCostCmd = new GetConnectionCostCommand(this);
			GetMaterialsCmd = new GetMaterialsCommand(this);
			GetCrossSectionsCmd = new GetCrossSectionsCommand(this);
			GetBoltAssembliesCmd = new GetBoltAssembliesCommand(this);
			CreateBoltAssemblyCmd = new CreateBoltAssemblyCommand(this);
			GetParametersCmd = new GetParametersCommand(this);
			EvaluateExpessionCmd = new EvaluateExpressionCommand(this);
			GetLoadingCmd = new GetLoadingCommand(this);
			GetConnCheckResultsCmd = new GetConnCheckResults(this);
			GetAllConnectionDataCmd = new GetAllConnDataCommand(this);
			ShowConHiddenCalcLogFileCmd = new ShowConHiddenCalcLogFileCommand();
			OpenTempProjectCmd =new OpenConnectionInAppCommand(this);

			ShowConHiddenCalcLogFileCmd = new ShowConHiddenCalcLogFileCommand();

			TemplateSetting = new IdeaRS.OpenModel.Connection.ApplyConnTemplateSetting() { DefaultBoltAssemblyID = 1, DefaultCleatCrossSectionID = 1, DefaultConcreteMaterialID = 1, DefaultStiffMemberCrossSectionID = 1, UseMatFromOrigin = false };

			jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Culture = CultureInfo.InvariantCulture };

			var jsonFormating = Formatting.Indented;
			this.templateSettingString = JsonConvert.SerializeObject(TemplateSetting, jsonFormating, jsonSerializerSettings);
		}
		#endregion

		#region Commands
		public ICommand OpenProjectCmd { get; set; }
		public ICommand ImportIOMCmd { get; set; }
		public ICommand UpdateIOMCmd { get; set; }
		public ICommand CloseProjectCmd { get; set; }
		public ICommand CalculateConnectionCmd { get; set; }
		public ICommand CalculateBucklingConnectionCmd { get; set; }
		public ICommand ConnectionGeometryCmd { get; set; }
		public ICommand GetAllConnectionDataCmd { get; set; }
		public ICommand GetConnectionCostCmd { get; set; }
		public ICommand SaveProjectCmd { get; set; }
		public ICommand SaveAsProjectCmd { get; set; }
		public ICommand ConnectionToTemplateCmd { get; set; }
		public ICommand ApplyTemplateCmd { get; set; }
		public ICommand ApplySimpleTemplateCmd { get; set; }
		public ICommand GetMaterialsCmd { get; set; }
		public ICommand GetCrossSectionsCmd { get; set; }
		public ICommand GetBoltAssembliesCmd { get; set; }
		public ICommand CreateBoltAssemblyCmd { get; set; }
		public ICommand ShowConHiddenCalcLogFileCmd { get; set; }
		public ICommand GetParametersCmd { get; set; }
		public ICommand EvaluateExpessionCmd { get; set; }
		public ICommand GetLoadingCmd { get; set; }
		public ICommand GetConnCheckResultsCmd { get; set; }
		public ICommand OpenTempProjectCmd { get; set; }
		public ICommand DeleteOperationsCmd { get;set; }
		public ICommand GenerateReportCmd { get; set; }
		public ICommand GeneratePdfReportCmd { get; set; }
		public ICommand GenerateWordReportCmd { get; set; }
		#endregion

		#region IConHiddenCalcModel

		/// <summary>
		/// Indicate if the installation of IdeaStatiCa exits
		/// </summary>
		public bool IsIdea
		{
			get => isIdea;

			set
			{
				isIdea = value;
				NotifyPropertyChanged("IsIdea");
			}
		}

		public string NewBoltAssemblyName
		{
			get => newBoltAssemblyName;

			set
			{
				newBoltAssemblyName = value;
				NotifyPropertyChanged("NewBoltAssemblyName");
			}
		}

		public int SupportingMember
		{
			get => supportingMember;

			set
			{
				supportingMember = value;
				NotifyPropertyChanged("SupportingMember");
			}
		}
		public int AttachedMember
		{
			get => attachedMember;

			set
			{
				attachedMember = value;
				NotifyPropertyChanged("AttachedMember");
			}
		}

		public bool IsService
		{
			get => Service != null;
		}

		public string Results
		{
			get => results;
			set
			{
				results = value;
				NotifyPropertyChanged("Results");
			}
		}

		public string TemplateSettingString
		{
			get => templateSettingString;
			set
			{
				templateSettingString = value;
				NotifyPropertyChanged("TemplateSettingString");

				try
				{
					TemplateSetting = AppConSettingFromJsonString(templateSettingString);
					SetStatusMessage("OK");
				}
				catch
				{
					SetStatusMessage("Invalid JSON string");
				}
			}
		}

		public IConnHiddenCheck GetConnectionService()
		{
			if (Service != null)
			{
				Logger.LogDebug("ConnectionHiddenCalculation.MainVM.GetConnectionService() : returning the existing instance");
				return Service;
			}

			Logger.LogDebug("ConnectionHiddenCalculation.MainVM.GetConnectionService() : creating the new instance of ConnectionHiddenCheckClient");

			IdeaConnectionClient = CalcFactory.Create();
			Service = IdeaConnectionClient;
			return Service;
		}

		public void CloseConnectionService()
		{
			Logger.LogInformation("MainVM.CloseConnectionService");
			if (Service == null)
			{
				return;
			}

			IdeaConnectionClient.CloseProject();
			IdeaConnectionClient.Close();
			IdeaConnectionClient = null;
			Service = null;

			Results = string.Empty;
			Connections.Clear();

			if (connectionController != null)
			{
				connectionController.CloseProject();
			}

			DeleteTempProjectFile();
		}

		private void DeleteTempProjectFile()
		{
			if (!string.IsNullOrEmpty(IdeaConTempFileName) && File.Exists(IdeaConTempFileName))
			{
				File.Delete(IdeaConTempFileName);
			}

			IdeaConTempFileName = null;
		}

		public void SetStatusMessage(string msg)
		{
			Application.Current.Dispatcher.BeginInvoke(
			 (ThreadStart)delegate
			 {
				 this.StatusMessage = msg;
			 });
		}

		public void SetResults(object res)
		{
			Application.Current.Dispatcher.BeginInvoke(
			 (ThreadStart)delegate
			 {
				 var jsonSetting = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Culture = CultureInfo.InvariantCulture };

				 if (res is ConnectionResultsData cbfemResults)
				 {

					 var jsonFormating = Formatting.Indented;
					 this.Results = JsonConvert.SerializeObject(cbfemResults, jsonFormating, jsonSetting);
				 }
				 else if (res is IdeaRS.OpenModel.Connection.ConnectionData conData)
				 {
					 var jsonSetting2 = new JsonSerializerSettings
					 {
						 ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
						 Culture = CultureInfo.InvariantCulture,
						 TypeNameHandling = TypeNameHandling.Auto
					 };

					 var jsonFormating = Formatting.Indented;
					 Results = JsonConvert.SerializeObject(conData, jsonFormating, jsonSetting2);
				 }
				 else if (res is IdeaRS.OpenModel.OpenModelContainer openModelTuple)
				 {
					 var jsonFormating = Formatting.Indented;
					 Results = JsonConvert.SerializeObject(openModelTuple, jsonFormating, jsonSetting);
				 }
				 else if (res is List<ProjectItem> projectItems)
				 {
					 var jsonFormating = Formatting.Indented;
					 Results = JsonConvert.SerializeObject(projectItems, jsonFormating, jsonSetting);
				 }
				 else if (res is ConnectionLoadingJson connLoading)
				 {
					 var upadateParamCmd = new UpdateLoadingCommand(this);
					 var conParamsVM = new ConnDataJsonVM(upadateParamCmd, connLoading, "Update loading");
					 ConnDataJsonWnd connParamsWnd = new ConnDataJsonWnd(conParamsVM);
					 connParamsWnd.Owner = Application.Current.MainWindow;
					 var updateRes = connParamsWnd.ShowDialog();
					 if (updateRes == true)
					 {
						 this.Results = "Connection loading is updated";
					 }
				 }
				 else if (res is ConnectionDataJson connParams)
				 {
					 var upadateParamCmd = new UpdateConnParamsCommand(this);
					 var conParamsVM = new ConnDataJsonVM(upadateParamCmd, connParams, "Update parameters");
					 ConnDataJsonWnd connParamsWnd = new ConnDataJsonWnd(conParamsVM);
					 connParamsWnd.Owner = Application.Current.MainWindow;
					 var updateRes = connParamsWnd.ShowDialog();
					 if (updateRes == true)
					 {
						 this.Results = "Connection parameters are updated";
					 }
				 }
				 else
				 {
					 this.Results = (res == null ? string.Empty : res.ToString());
				 }
			 });
		}

		public void SetConProjectData(ConProjectInfo projectData)
		{
			if (connectionController != null)
			{
				connectionController.CloseProject();
				DeleteTempProjectFile();
			}

			List<ConnectionVM> connectionsVm = new List<ConnectionVM>();
			// get information obaout all aconections in the project
			foreach (var con in projectData.Connections)
			{
				connectionsVm.Add(new ConnectionVM(con));
			}

			this.Connections = new ObservableCollection<ConnectionVM>(connectionsVm);
			SelectedConnection = Connections.FirstOrDefault();
		}

		#endregion

		#region View model's properties and methods

		public IConnectionController ConnectionController
		{
			get => connectionController;
			set
			{
				connectionController = value;
				NotifyPropertyChanged("ConnectionController");
			}
		}

		private void RunIdeaConnection(object obj)
		{
			//if (this.ConnectionController == null)
			//{
			//	// it starts the new process of IdeaConnection.exe which is located in the directory ideaStatiCaDir
			//	this.ConnectionController = IdeaConnectionController.Create(ideaStatiCaDir, Logger);
			//	this.ConnectionController.ConnectionAppExited += ConnectionController_ConnectionAppExited;
			//}
			//else
			//{
			//	this.ConnectionController.CloseProject();
			//	DeleteTempProjectFile();
			//}

			//IdeaConTempFileName = Path.ChangeExtension(Path.GetTempFileName(), ".ideacon");
			//SaveAsProjectCmd.Execute(IdeaConTempFileName);

			//ConnectionController.OpenProjectAsync(IdeaConTempFileName);
		}

		private void ConnectionController_ConnectionAppExited(object sender, EventArgs e)
		{
			ConnectionController.ConnectionAppExited -= ConnectionController_ConnectionAppExited;
			Logger.LogInformation("MainVM.ConnectionController_ConnectionAppExited");
			IDisposable disp = ConnectionController as IDisposable;
			if (disp != null)
			{
				disp.Dispose();
			}

			ConnectionController = null;
		}

		private bool CanRunIdeaConnection(object arg)
		{
			return IsService;
		}

		private IConnHiddenCheck Service
		{
			get => service;
			set
			{
				service = value;
				NotifyPropertyChanged("Service");
			}
		}

		/// <summary>
		/// The list of view models for all connections in the project
		/// </summary>
		public ObservableCollection<ConnectionVM> Connections
		{
			get => connections;
			set
			{
				connections = value;
				NotifyPropertyChanged("Connections");
			}
		}

		/// <summary>
		/// Notification in the status bar
		/// </summary>
		public string StatusMessage
		{
			get => statusMessage;
			set
			{
				statusMessage = value;
				NotifyPropertyChanged("StatusMessage");
			}
		}

		public ApplyConnTemplateSetting TemplateSetting
		{
			get => templateSetting;
			set
			{
				templateSetting = value;
				NotifyPropertyChanged("TemplateSetting");
			}
		}

		/// <summary>
		/// Get or test the expression which is evaluated by EvaluateExpressionCommand
		/// </summary>
		public string Expression
		{
			get => expression;
			set
			{
				expression = value;
				NotifyPropertyChanged("Expression");
			}
		}

		public IConnectionId SelectedConnection
		{
			get => selectedConnection;
			set
			{
				selectedConnection = value;
				NotifyPropertyChanged("SelectedConnection");
			}
		}

		public string IdeaConTempFileName { get => ideaConTempFileName; set => ideaConTempFileName = value; }

		private void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private ApplyConnTemplateSetting AppConSettingFromJsonString(string json)
		{
			return JsonConvert.DeserializeObject<ApplyConnTemplateSetting>(json, jsonSerializerSettings);
		}
		#endregion
	}

	public class Converter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return values.Clone();
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
