using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Grpc;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace FEAppTest
{
	public class FEAppTestVM : INotifyPropertyChanged, IHistoryLog
	{
		private IBIMPluginHosting feaAppHosting;
		private string modelFeaXml;

		public FEAppTestVM()
		{
			Actions = new ObservableCollection<string>();
			IdeaStatiCaStatus = AppStatus.Finished;
			RunCmd = new CustomCommand(this.CanRun, this.Run);
			LoadCmd = new CustomCommand(this.CanLoad, this.Load);
			ProjectName = "Bim test";
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<string> Actions { get; set; }
		public AppStatus IdeaStatiCaStatus { get; set; }
		public CustomCommand LoadCmd { get; set; }
		public string ProjectName { get; set; }
		public CustomCommand RunCmd { get; set; }

		public void Add(string action)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(
				System.Windows.Threading.DispatcherPriority.Normal,
				(Action)(() =>
				{
					Actions.Add(action);
				}));
		}

		public bool CanLoad(object param)
		{
			return (IdeaStatiCaStatus == AppStatus.Started);
		}

		public bool CanRun(object param)
		{
			return (IdeaStatiCaStatus == AppStatus.Finished);
		}

		public void Load(object param)
		{
			var filePath = param == null
				? GetFilePath()
				: Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), param.ToString());

			if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
			{
				var xml = File.ReadAllText(filePath);
				var model = Tools.ModelFromXml(xml);
				((FakeFEA)feaAppHosting.Service).FeaModel = model;
				Add($"Model {filePath} loaded.");
			}
		}

		public void Run(object param)
		{
			var factory = new PluginFactory(this);
			// use gRPC instead of wcf

			var bimHostingFactory = new GrpcBimHostingFactory();
			var feaAppHosting = bimHostingFactory.Create(factory, new NullLogger());

			feaAppHosting.AppStatusChanged += new ISEventHandler(IdeaStaticAppStatusChanged);
			var id = Process.GetCurrentProcess().Id.ToString();

			var projectDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ProjectName);
			if (!Directory.Exists(projectDir))
			{
				Directory.CreateDirectory(projectDir);
			}

			Add(string.Format("Starting FEAPluginHosting clientTd = {0}", id));
			feaAppHosting.RunAsync(id, projectDir);
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
				feaAppHosting.AppStatusChanged -= new ISEventHandler(IdeaStaticAppStatusChanged);
				feaAppHosting = null;
			}

			if (e.Status == AppStatus.Started)
			{
				if (string.IsNullOrEmpty(modelFeaXml))
				{
					string fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "default_project.xml");
					modelFeaXml = File.ReadAllText(fileName);
					Add($"Model {fileName} loaded.");
				}

				var model = Tools.ModelFromXml(modelFeaXml);
				((FakeFEA)(feaAppHosting.Service)).FeaModel = model;
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

		// This method is called by the Set accessor of each property.
		// The CallerMemberName attribute that is applied to the optional propertyName
		// parameter causes the property name of the caller to be substituted as an argument.
		private void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}