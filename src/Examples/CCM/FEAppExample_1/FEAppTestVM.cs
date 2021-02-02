using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Plugin;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace FEAppExample_1
{
	public class FEAppExample_1VM : INotifyPropertyChanged, IHistoryLog
	{
		private IBIMPluginHosting feaAppHosting;
		private string modelFeaXml;
		private string workingDirectory;
		private string projectName;
		private string projectDir;
		private IdeaRS.OpenModel.CountryCode countryCode;
		private bool isCAD;
		private string detailInformation;

		public FEAppExample_1VM()
		{
			this.CountryCode = CountryCode.ECEN;

			Actions = new ObservableCollection<string>();
			IdeaStatiCaStatus = AppStatus.Finished;
			RunCmd = new CustomCommand(this.CanRun, this.Run);
			LoadCmd = new CustomCommand(this.CanLoad, this.Load);
			GetConnectionModelCmd = new CustomCommand(this.CanGetConnectionModel, this.GetConnectionModel);
			GetAllConnectionDataCmd = new CustomCommand(this.CanGetAllConnectionData, this.GetAllConnectionData);
			GetCssInProjectCmd = new CustomCommand(this.CanGetCssInProject, this.GetCssInProject);
			GetCssInMprlCmd = new CustomCommand(this.CanGetCssInMprl, this.GetCssInMprl);
			GetMatInProjectCmd = new CustomCommand(this.CanGetMatInProject, this.GetMatInProject);
			GetMatInMprlCmd = new CustomCommand(this.CanGetMatInMprl, this.GetGetMatInMprl);
			ShowCCMLogCmd = new CustomCommand(this.CanOpenCCMLogFile, this.OpenCCMLogFile);
			ProjectName = string.Empty;

			WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Process.GetCurrentProcess().ProcessName);
			if (!Directory.Exists(WorkingDirectory))
			{
				Directory.CreateDirectory(WorkingDirectory);
			}

			isCAD = false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<string> Actions { get; set; }
		public AppStatus IdeaStatiCaStatus { get; set; }
		public CustomCommand LoadCmd { get; set; }
		public CustomCommand RunCmd { get; set; }
		public CustomCommand GetConnectionModelCmd { get; set; }
		public CustomCommand GetAllConnectionDataCmd { get; set; }
		public CustomCommand GetCssInProjectCmd { get; set; }
		public CustomCommand GetCssInMprlCmd { get; set; }
		public CustomCommand GetMatInProjectCmd { get; set; }
		public CustomCommand GetMatInMprlCmd { get; set; }
		public CustomCommand ShowCCMLogCmd { get; set; }

		public string ModelFeaXml { get => modelFeaXml; set => modelFeaXml = value; }

		private List<BIMItemId> selectedItems;

		public string WorkingDirectory
		{
			get => workingDirectory;
			set
			{
				workingDirectory = value;
				NotifyPropertyChanged("WorkingDirectory");
			}
		}

		public bool IsCAD
		{
			get => isCAD;
			set
			{
				isCAD = value;

				var fakeFea = ((FakeFEA)(FeaAppHosting?.Service));
				if (fakeFea != null)
				{
					fakeFea.IsCadApplication = IsCAD;
				}

				NotifyPropertyChanged("IsCAD");
			}
		}

		public string ProjectName
		{
			get => projectName;
			set
			{
				projectName = value;
				NotifyPropertyChanged("ProjectName");
			}
		}

		public string ProjectDir
		{
			get => projectDir;
			set
			{
				projectDir = value;
				NotifyPropertyChanged("ProjectDir");
			}
		}

		public List<BIMItemId> SelectedItems
		{
			get => selectedItems;
			set
			{
				selectedItems = value;
				NotifyPropertyChanged("SelectedItems");
			}
		}

		public CountryCode CountryCode
		{
			get => countryCode;
			set
			{
				countryCode = value;
				NotifyPropertyChanged("CountryCode");
			}
		}

		public IBIMPluginHosting FeaAppHosting { get => feaAppHosting; set => feaAppHosting = value; }

		public string DetailInformation
		{
			get => detailInformation;
			set
			{
				detailInformation = value;
				NotifyPropertyChanged("DetailInformation");
			}
		}

		public void Add(string action)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(
				System.Windows.Threading.DispatcherPriority.Normal,
				(Action)(() =>
				{
					Actions.Add(action);
				}));
		}

		public void SetDetailInformation(string detailInfo)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(
				System.Windows.Threading.DispatcherPriority.Normal,
				(Action)(() =>
				{
					DetailInformation = detailInfo;
				}));
		}

		public bool CanLoad(object param)
		{
			return (IdeaStatiCaStatus != AppStatus.Started);
		}

		public bool CanRun(object param)
		{
			return ((IdeaStatiCaStatus == AppStatus.Finished) && !string.IsNullOrEmpty(ProjectName));
		}

		public void Load(object param)
		{
			var filePath = param == null
				? GetFilePath()
				: Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), param.ToString());

			if(string.IsNullOrEmpty(filePath))
			{
				// nothing to load;
				return;
			}

			CreateProjectDirectory(filePath);
		}

		private void CreateProjectDirectory(string filePath)
		{
			ProjectName = Path.GetFileNameWithoutExtension(filePath);
			ModelFeaXml = File.ReadAllText(filePath);
			Add($"File '{filePath}' loaded.");

			ProjectDir = Path.Combine(WorkingDirectory, ProjectName);
			if(!Directory.Exists(ProjectDir))
			{
				Directory.CreateDirectory(ProjectDir);
				File.Copy(filePath, Path.Combine(ProjectDir, Path.GetFileName(filePath)));
				Add($"New project directory '{ProjectDir}' was created");
			}
			else
			{
				Add($"Using existing project directory '{ProjectDir}'");
			}
		}

		public void Run(object param)
		{
			var factory = new PluginFactory(this);
			FeaAppHosting = new BIMPluginHosting(factory);
			FeaAppHosting.AppStatusChanged += new ISEventHandler(IdeaStaticAppStatusChanged);
			var id = Process.GetCurrentProcess().Id.ToString();

			ProjectDir = Path.Combine(WorkingDirectory, ProjectName);
			if (!Directory.Exists(ProjectDir))
			{
				Directory.CreateDirectory(ProjectDir);
			}

			var ideaStatiCaProjectDir = Path.Combine(ProjectDir, "IdeaStatiCa-" + ProjectName);
			if (!Directory.Exists(ideaStatiCaProjectDir))
			{
				Directory.CreateDirectory(ideaStatiCaProjectDir);
			}

			Add(string.Format("Starting FEAPluginHosting clientTd = {0}", id));
			FeaAppHosting.RunAsync(id, ideaStatiCaProjectDir);
		}

		private bool CanGetConnectionModel(object arg)
		{
			if(SelectedItems == null)
			{
				return false;
			}

			var firstItem = SelectedItems.FirstOrDefault();
			if(firstItem == null)
			{
				return false;
			}

			if(firstItem.Type != BIMItemType.Node)
			{
				return false;
			}

			return true;
		}

		private bool CanGetAllConnectionData(object arg)
		{
			if (SelectedItems == null)
			{
				return false;
			}

			var firstItem = SelectedItems.FirstOrDefault();
			if (firstItem == null)
			{
				return false;
			}

			if (firstItem.Type != BIMItemType.Node)
			{
				return false;
			}

			return true;
		}

		private void GetConnectionModel(object obj)
		{
			var firstItem = SelectedItems.FirstOrDefault();

			if (FeaAppHosting == null)
			{
				return;
			}

			var bimAppliction = (ApplicationBIM)FeaAppHosting.Service;
			if (bimAppliction == null)
			{
				Debug.Fail("Can not cast to ApplicationBIM");
				return;
			}

			ConnectionData connectionData = null;
			int myProcessId = bimAppliction.Id;
			Add(string.Format("Starting commication with IdeaStatiCa running in  the process {0}", myProcessId));

			using (IdeaStatiCaAppClient ideaStatiCaApp = new IdeaStatiCaAppClient(myProcessId.ToString()))
			{
				ideaStatiCaApp.Open();
				Add(string.Format("Getting connection model for connection #{0}", firstItem.Id));
				connectionData = ideaStatiCaApp.GetConnectionModel(firstItem.Id);


				System.Windows.Application.Current.Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.Normal,
					(Action)(() =>
					{
						if (connectionData == null)
						{
							Add("No data");
						}
						else
						{
							var jsonSetting = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Culture = CultureInfo.InvariantCulture };
							var jsonFormating = Newtonsoft.Json.Formatting.Indented;
							string geometryInJson = JsonConvert.SerializeObject(connectionData, jsonFormating, jsonSetting);

							Add("GetConnectionModel succeeded");
							SetDetailInformation(geometryInJson);
						}
						CommandManager.InvalidateRequerySuggested();
					}));
			}
		}

		private void GetAllConnectionData(object obj)
		{
			var firstItem = SelectedItems.FirstOrDefault();

			if (FeaAppHosting == null)
			{
				return;
			}

			var bimAppliction = (ApplicationBIM)FeaAppHosting.Service;
			if (bimAppliction == null)
			{
				Debug.Fail("Can not cast to ApplicationBIM");
				return;
			}

			int myProcessId = bimAppliction.Id;
			Add(string.Format("Starting commication with IdeaStatiCa running in  the process {0}", myProcessId));

			OpenModelContainer openModelTuple = null;

			using (IdeaStatiCaAppClient ideaStatiCaApp = new IdeaStatiCaAppClient(myProcessId.ToString()))
			{
				ideaStatiCaApp.Open();
				Add(string.Format("Getting connection IOM model for connection #{0}", firstItem.Id));
				string openModelTupleXml = ideaStatiCaApp.GetAllConnectionData(firstItem.Id);

				System.Windows.Application.Current.Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.Normal,
					(Action)(() =>
					{
						if (string.IsNullOrEmpty(openModelTupleXml) || openModelTupleXml.StartsWith("Error", StringComparison.InvariantCultureIgnoreCase))
						{
							Add("Error - see details in the CCM logfile");
						}
						else
						{
							// get an instance of OpenModelContainer from XML
							openModelTuple = Tools.OpenModelContainerFromXml(openModelTupleXml);

							Add("GetAllConnectionData succeeded");
							SetDetailInformation(openModelTupleXml);
						}
						CommandManager.InvalidateRequerySuggested();
					}));
			}
		}

		private bool CanGetMatInMprl(object arg)
		{
			return FeaAppHosting?.Service != null;
		}

		private void GetGetMatInMprl(object obj)
		{
			if (FeaAppHosting == null)
			{
				return;
			}

			var bimAppliction = (ApplicationBIM)FeaAppHosting.Service;
			if (bimAppliction == null)
			{
				Debug.Fail("Can not cast to ApplicationBIM");
				return;
			}

			int myProcessId = bimAppliction.Id;
			Add(string.Format("Starting commication with IdeaStatiCa running in  the process {0}", myProcessId));

			using (IdeaStatiCaAppClient ideaStatiCaApp = new IdeaStatiCaAppClient(myProcessId.ToString()))
			{
				ideaStatiCaApp.Open();

				var mprlMaterials = ideaStatiCaApp.GetMaterialsInMPRL(this.CountryCode);

				System.Windows.Application.Current.Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.Normal,
					(Action)(() =>
					{
						if (mprlMaterials == null)
						{
							Add("No data");
						}
						else
						{
							var jsonSetting = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Culture = CultureInfo.InvariantCulture };
							var jsonFormating = Newtonsoft.Json.Formatting.Indented;
							string mprlMaterialsJson = JsonConvert.SerializeObject(mprlMaterials, jsonFormating, jsonSetting);
							Add("GetGetMatInMprl succeeded");
							SetDetailInformation(mprlMaterialsJson);
						}
						CommandManager.InvalidateRequerySuggested();
					}));
			}
		}

		private bool CanGetMatInProject(object arg)
		{
			return FeaAppHosting?.Service != null;
		}

		private void GetMatInProject(object obj)
		{
			if (FeaAppHosting == null)
			{
				return;
			}

			var bimAppliction = (ApplicationBIM)FeaAppHosting.Service;
			if (bimAppliction == null)
			{
				Debug.Fail("Can not cast to ApplicationBIM");
				return;
			}

			int myProcessId = bimAppliction.Id;
			Add(string.Format("Starting commication with IdeaStatiCa running in  the process {0}", myProcessId));

			using (IdeaStatiCaAppClient ideaStatiCaApp = new IdeaStatiCaAppClient(myProcessId.ToString()))
			{
				ideaStatiCaApp.Open();

				var materialsInProject = ideaStatiCaApp.GetMaterialsInProject();

				System.Windows.Application.Current.Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.Normal,
					(Action)(() =>
					{
						if (materialsInProject == null)
						{
							Add("No data");
						}
						else
						{
							var jsonSetting = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Culture = CultureInfo.InvariantCulture };
							var jsonFormating = Newtonsoft.Json.Formatting.Indented;
							string materialsJson = JsonConvert.SerializeObject(materialsInProject, jsonFormating, jsonSetting);

							Add("GetMatInProject succeeded");
							SetDetailInformation(materialsJson);
						}
						CommandManager.InvalidateRequerySuggested();
					}));
			}
		}

		private bool CanGetCssInMprl(object arg)
		{
			return FeaAppHosting?.Service != null;
		}

		private void GetCssInMprl(object obj)
		{
			if (FeaAppHosting == null)
			{
				return;
			}

			var bimAppliction = (ApplicationBIM)FeaAppHosting.Service;
			if (bimAppliction == null)
			{
				Debug.Fail("Can not cast to ApplicationBIM");
				return;
			}

			int myProcessId = bimAppliction.Id;
			Add(string.Format("Starting commication with IdeaStatiCa running in  the process {0}", myProcessId));

			using (IdeaStatiCaAppClient ideaStatiCaApp = new IdeaStatiCaAppClient(myProcessId.ToString()))
			{
				ideaStatiCaApp.Open();

				var cssInMprl = ideaStatiCaApp.GetCssInMPRL(this.CountryCode);

				System.Windows.Application.Current.Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.Normal,
					(Action)(() =>
					{
						if (cssInMprl == null)
						{
							Add("GetCssInMprl - No data");
						}
						else
						{
							var jsonSetting = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Culture = CultureInfo.InvariantCulture };
							var jsonFormating = Newtonsoft.Json.Formatting.Indented;
							string cssInMprlJson = JsonConvert.SerializeObject(cssInMprl, jsonFormating, jsonSetting);

							Add("GetCssInMprl succeeded");
							SetDetailInformation(cssInMprlJson);
						}
						CommandManager.InvalidateRequerySuggested();
					}));
			}
		}

		private bool CanGetCssInProject(object arg)
		{
			return FeaAppHosting?.Service != null;
		}

		private void GetCssInProject(object obj)
		{
			if (FeaAppHosting == null)
			{
				return;
			}

			var bimAppliction = (ApplicationBIM)FeaAppHosting.Service;
			if (bimAppliction == null)
			{
				Debug.Fail("Can not cast to ApplicationBIM");
				return;
			}

			int myProcessId = bimAppliction.Id;
			Add(string.Format("Starting commication with IdeaStatiCa running in  the process {0}", myProcessId));

			using (IdeaStatiCaAppClient ideaStatiCaApp = new IdeaStatiCaAppClient(myProcessId.ToString()))
			{
				ideaStatiCaApp.Open();

				var cssInProject = ideaStatiCaApp.GetCssInProject();

				System.Windows.Application.Current.Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.Normal,
					(Action)(() =>
					{
						if (cssInProject == null)
						{
							Add("GetCssInProject - No data");
						}
						else
						{
							var jsonSetting = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), Culture = CultureInfo.InvariantCulture };
							var jsonFormating = Newtonsoft.Json.Formatting.Indented;
							string cssInProjectJson = JsonConvert.SerializeObject(cssInProject, jsonFormating, jsonSetting);

							Add("GetCssInProject succeeded");
							SetDetailInformation(cssInProjectJson);

						}
						CommandManager.InvalidateRequerySuggested();
					}));
			}
		}

		/// <summary>
		/// Can CCM log file be opened 
		/// </summary>
		/// <param name="arg"></param>
		/// <returns>Returs true if CCM log file exists</returns>
		private bool CanOpenCCMLogFile(object arg)
		{
			return File.Exists(GetCCMLogFile());
		}

		/// <summary>
		/// Open CCM log file if it exists in the temporary directory
		/// </summary>
		/// <param name="obj"></param>
		private void OpenCCMLogFile(object obj)
		{
			string logFilePath = GetCCMLogFile();

			if (File.Exists(logFilePath))
			{
				using (Process proc = new Process())
				{
					proc.StartInfo = new ProcessStartInfo(logFilePath);
					proc.Start();
				}
			}
		}

		/// <summary>
		/// Returns the full file name of CCM log file
		/// </summary>
		/// <returns></returns>
		private static string GetCCMLogFile()
		{
			var logFileFileName = Path.Combine(Path.GetTempPath(), "IdeaStatiCa\\Logs\\", "IdeaStatiCaCodeCheckManager.log");
			return logFileFileName;
		}

		private static string GetFilePath()
		{
			var openFileDialog = new OpenFileDialog { Filter = "XML Files | *.xml" };
			if (openFileDialog.ShowDialog() == true)
			{
				return openFileDialog.FileName;
			}

			return null;
		}

		private void IdeaStaticAppStatusChanged(object sender, ISEventArgs e)
		{
			if (e.Status == AppStatus.Finished)
			{
				FeaAppHosting.AppStatusChanged -= new ISEventHandler(IdeaStaticAppStatusChanged);
				FeaAppHosting = null;
			}

			if (e.Status == AppStatus.Started)
			{
				if (string.IsNullOrEmpty(modelFeaXml))
				{
					string fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Path.ChangeExtension(ProjectName, ".xml"));
					ModelFeaXml = File.ReadAllText(fileName);
					Add($"Model {fileName} loaded.");
				}

				var model = Tools.ModelFromXml(ModelFeaXml);
				var fakeFea = ((FakeFEA)(FeaAppHosting.Service));
				fakeFea.IsCadApplication = IsCAD;
				fakeFea.SelectionChanged += FakeFea_SelectionChanged;
				((FakeFEA)(FeaAppHosting.Service)).FeaModel = model;
			}

			System.Windows.Application.Current.Dispatcher.BeginInvoke(
				System.Windows.Threading.DispatcherPriority.Normal,
				(Action)(() =>
				{
					Add(string.Format("IdeaStatiCa current status is = {0}", e.Status));
					IdeaStatiCaStatus = e.Status;
					CommandManager.InvalidateRequerySuggested();
				}));
		}

		private void FakeFea_SelectionChanged(object sender, EventArgs e)
		{
			FakeFEA fakeFea = (FakeFEA)sender;
			if(fakeFea != null)
			{
				System.Windows.Application.Current.Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.Normal,
					(Action)(() =>
					{
						this.SelectedItems = fakeFea.SelectedItems;
						CommandManager.InvalidateRequerySuggested();
					}));
			}
		}

		// This method is called by the Set accessor of each property.
		// The CallerMemberName attribute that is applied to the optional propertyName
		// parameter causes the property name of the caller to be substituted as an argument.
		private void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}