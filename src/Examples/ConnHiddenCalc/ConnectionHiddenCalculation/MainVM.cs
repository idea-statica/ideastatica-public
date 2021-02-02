using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.ConnectionClient.ConHiddenCalcCommands;
using IdeaStatiCa.ConnectionClient.Model;
using IdeaStatiCa.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
		IdeaConnectionController connectionController;
		readonly string ideaConnExeFileName;
		private string ideaConTempFileName;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public MainVM()
		{
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
					CalcFactory = new ConnHiddenClientFactory(ideaStatiCaDir);
				}
			}

			if (!IsIdea)
			{
				StatusMessage = string.Format("ERROR IdeaStatiCa doesn't exist in '{0}'", ideaStatiCaDir);
			}

			OpenProjectCmd = new OpenProjectCommand(this);
			ImportIOMCmd = new ImportIOMCommand(this);
			CloseProjectCmd = new CloseProjectCommand(this);
			CalculateConnectionCmd = new CalculateConnectionCommand(this);
			ApplySimpleTemplateCmd = new ApplySimpleTemplateCommand(this);
			ConnectionGeometryCmd = new ConnectionGeometryCommand(this);
			SaveAsProjectCmd = new SaveAsProjectCommand(this);
			ConnectionToTemplateCmd = new ConnectionToTemplateCommand(this);
			ApplyTemplateCmd = new ApplyTemplateCommand(this);

			GetMaterialsCmd = new GetMaterialsCommand(this);
			GetCrossSectionsCmd = new GetCrossSectionsCommand(this);
			GetBoltAssembliesCmd = new GetBoltAssembliesCommand(this);
			CreateBoltAssemblyCmd = new CreateBoltAssemblyCommand(this);
			GetParametersCmd = new GetParametersCommand(this);
			GetLoadingCmd = new GetLoadingCommand(this);
			GetConnCheckResultsCmd = new GetConnCheckResults(this);
			GetAllConnectionDataCmd = new GetAllConnDataCommand(this);
			ShowConHiddenCalcLogFileCmd = new ShowConHiddenCalcLogFileCommand();
			OpenTempProjectCmd = new CustomCommand(CanRunIdeaConnection, RunIdeaConnection);

			ShowConHiddenCalcLogFileCmd = new ShowConHiddenCalcLogFileCommand();

			TemplateSetting = new IdeaRS.OpenModel.Connection.ApplyConnTemplateSetting() { DefaultBoltAssemblyID = 1, DefaultCleatCrossSectionID = 1, DefaultConcreteMaterialID = 1, DefaultStiffMemberCrossSectionID = 1};

			jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Culture = CultureInfo.InvariantCulture };

			var jsonFormating = Formatting.Indented;
			this.templateSettingString = JsonConvert.SerializeObject(TemplateSetting, jsonFormating, jsonSerializerSettings);
		}
		#endregion

		#region Commands
		public ICommand OpenProjectCmd { get; set; }
		public ICommand ImportIOMCmd { get; set; }
		public ICommand CloseProjectCmd { get; set; }
		public ICommand CalculateConnectionCmd { get; set; }
		public ICommand ConnectionGeometryCmd { get; set; }
		public ICommand GetAllConnectionDataCmd { get; set; }
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
		public ICommand GetLoadingCmd { get; set; }

		public ICommand GetConnCheckResultsCmd { get; set; }

		public ICommand OpenTempProjectCmd { get; set; }

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
				return Service;
			}

			IdeaConnectionClient = CalcFactory.Create();
			Service = IdeaConnectionClient;
			return Service;
		}

		public void CloseConnectionService()
		{
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

			if(connectionController != null)
			{
				connectionController.ConnectionAppAutomation.CloseProject();
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
					 var jsonFormating = Formatting.Indented;
					 Results = JsonConvert.SerializeObject(conData, jsonFormating, jsonSetting);
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
				 else if(res is ConnectionLoadingJson connLoading)
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
			if(connectionController != null)
			{
				connectionController.ConnectionAppAutomation.CloseProject();
				DeleteTempProjectFile();
			}

			List<ConnectionVM> connectionsVm = new List<ConnectionVM>();
			// get information obaout all aconections in the project
			foreach (var con in projectData.Connections)
			{
				connectionsVm.Add(new ConnectionVM(con));
			}

			this.Connections = new ObservableCollection<ConnectionVM>(connectionsVm);
		}

		#endregion

		#region View model's properties and methods

		public IdeaConnectionController ConnectionController
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
			if (this.ConnectionController == null)
			{
				// it starts the new process of IdeaConnection.exe which is located in the directory ideaStatiCaDir
				this.ConnectionController = IdeaConnectionController.Create(ideaStatiCaDir);
			}
			else
			{
				this.ConnectionController.ConnectionAppAutomation.CloseProject();
				DeleteTempProjectFile();
			}

			IdeaConTempFileName = Path.ChangeExtension(Path.GetTempFileName(), ".ideacon");
			SaveAsProjectCmd.Execute(IdeaConTempFileName);

			ConnectionController.ConnectionAppAutomation.OpenProject(IdeaConTempFileName);
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
