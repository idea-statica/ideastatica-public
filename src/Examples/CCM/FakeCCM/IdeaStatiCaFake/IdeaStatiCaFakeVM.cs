using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Message;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace IdeaStatiCaFake
{
	public class IdeaStatiCaFakeVM : INotifyPropertyChanged, IDisposable
	{
		private GrpcServiceBasedReflectionClient<IAutomation> grpcClient;

		private AutomationHostingGrpc<IAutomation, IApplicationBIM> AutomationHosting { get; set; }
		private IApplicationBIM FEA
		{
			get { return AutomationHosting?.MyBIM; }
		}

		public AppStatus FEAStatus { get; set; }

		private string modelFeaXml;

		public IdeaStatiCaFakeVM()
		{
			ModelFeaXml = string.Empty;
			FEAStatus = AppStatus.Finished;
			ImportConnectionCmd = new CustomCommand(this.CanImportConnection, this.ImportConnection);
			ImportMemberCmd = new CustomCommand(this.CanImportMember, this.ImportMember);
			ShowModelXmlCmd = new CustomCommand(this.CanShowXml, this.ShowXml);
			SaveModelXmlCmd = new CustomCommand(this.CanSaveXml, this.SaveXml);
			Actions = new ObservableCollection<string>();

			string clientId = string.Empty;
			string project = string.Empty;
			int grpcPort = 0;
			bool grpcEnabled = false;

			var startupArgs = Environment.GetCommandLineArgs();

			if (startupArgs != null)
			{
				var autoArg = startupArgs.FirstOrDefault(a => a.StartsWith(Constants.AutomationParam));
				{
					if (!string.IsNullOrEmpty(autoArg))
					{
						clientId = autoArg.Substring(Constants.AutomationParam.Length + 1);
					}
				}

				var projectArg = startupArgs.FirstOrDefault(a => a.StartsWith(Constants.ProjectParam));
				{
					if (!string.IsNullOrEmpty(projectArg))
					{
						project = projectArg.Substring(Constants.ProjectParam.Length + 1);
					}
				}

				var grpcArg = startupArgs.FirstOrDefault(a => a.StartsWith(Constants.GrpcPortParam));
				{
					if (!string.IsNullOrEmpty(grpcArg))
					{
						grpcEnabled = int.TryParse(grpcArg.Substring(Constants.GrpcPortParam.Length + 1), out grpcPort);
					}
				}

				if (!string.IsNullOrEmpty(clientId) && grpcEnabled)
				{
					Actions.Add(string.Format("Starting Automation clientid = {0}", clientId));

					var grpcClient = new GrpcClient(new NullLogger());
					grpcClient.Connect(clientId, grpcPort);
					var grpcClientTask = grpcClient.StartAsync();

					AutomationHosting = new AutomationHostingGrpc<IAutomation, IApplicationBIM>(new AutomationService<IApplicationBIM>(), grpcClient);
					AutomationHosting.BIMStatusChanged += new ISEventHandler(AutomationHosting_FEAStatusChanged);
					AutomationHosting.RunAsync(grpcPort.ToString());
				}

				if (grpcEnabled)
				{
					InitializeGrpc(clientId, grpcPort);
				}
			}
		}

		private async void InitializeGrpc(string clientId, int grpcPort)
		{
			grpcClient = new GrpcServiceBasedReflectionClient<IAutomation>(new NullLogger());

			grpcClient.Connect(clientId, grpcPort);

			await grpcClient.StartAsync();

			Actions.Add($"GRPC server connected");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<string> Actions { get; set; }

		public CustomCommand ImportConnectionCmd { get; set; }

		public CustomCommand ImportMemberCmd { get; set; }

		public CustomCommand ShowModelXmlCmd { get; set; }

		public CustomCommand SaveModelXmlCmd { get; set; }

		public string ModelFeaXml
		{
			get => modelFeaXml;
			set
			{
				modelFeaXml = value;
				NotifyPropertyChanged();
			}
		}

		public bool CanImportConnection(object param)
		{
			return grpcClient.IsConnected;
		}

		public void ImportConnection(object param)
		{
			Actions.Add("ImportConnection - calling GetActiveSelectionModel");
			var xmlString = FEA.GetActiveSelectionModelXML(IdeaRS.OpenModel.CountryCode.ECEN, RequestedItemsType.Connections);
			var modelFEA = IdeaStatiCa.Plugin.Tools.ModelFromXml(xmlString);
			ModelFeaXml = xmlString;
			Actions.Add(string.Format("ImportConnection - recieved results {0}", modelFEA.Project));
		}

		public bool CanImportMember(object param)
		{
			return grpcClient.IsConnected;
		}

		public void ImportMember(object param)
		{
			Actions.Add("ImportMember - calling GetActiveSelectionModel");
			var xmlString = FEA.GetActiveSelectionModelXML(IdeaRS.OpenModel.CountryCode.ECEN, RequestedItemsType.Substructure);
			var modelFEA = IdeaStatiCa.Plugin.Tools.ModelFromXml(xmlString);
			ModelFeaXml = xmlString;
			Actions.Add(string.Format("ImportMember - recieved results {0}", modelFEA.Project));
		}

		public bool CanShowXml(object param)
		{
			return !string.IsNullOrEmpty(ModelFeaXml);
		}

		public void ShowXml(object param)
		{
			XmlViewerWnd viewer = new XmlViewerWnd();
			viewer.DataContext = this;
			viewer.ShowDialog();
		}

		public bool CanSaveXml(object param)
		{
			return !string.IsNullOrEmpty(ModelFeaXml);
		}

		public void SaveXml(object param)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "XML Files | *.xml";
			if (saveFileDialog.ShowDialog() == true)
			{
				var modelFEA = IdeaStatiCa.Plugin.Tools.ModelFromXml(ModelFeaXml);
				File.WriteAllText(saveFileDialog.FileName, ModelFeaXml);
				//SaveToFiles(modelFEA.Model, modelFEA.Results, modelFEA.Messages, saveFileDialog.FileName);
				//File.WriteAllText(saveFileDialog.FileName + "M", ModelFeaXml);
			}
		}

		protected bool SaveToFiles(OpenModel openStructModel, OpenModelResult openStructModelR, OpenMessages openMessages, string saveFileDialog)
		{
			try
			{
				{
					XmlSerializer xs = new XmlSerializer(typeof(OpenModel));
					Stream fs = new FileStream(saveFileDialog, FileMode.Create);
					XmlTextWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
					// Serialize using the XmlTextWriter.
					writer.Formatting = Formatting.Indented;
					xs.Serialize(writer, openStructModel);
					writer.Close();
					fs.Close();
				}
				if (openStructModelR != null)
				{
					XmlSerializer xs = new XmlSerializer(typeof(OpenModelResult));

					Stream fs = new FileStream(saveFileDialog + "R", FileMode.Create);
					XmlTextWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
					// Serialize using the XmlTextWriter.
					writer.Formatting = Formatting.Indented;
					xs.Serialize(writer, openStructModelR);
					writer.Close();
					fs.Close();
				}
				if (openMessages != null)
				{
					XmlSerializer xs = new XmlSerializer(typeof(OpenMessages));
					Stream fs = new FileStream(saveFileDialog + "E", FileMode.Create);
					XmlTextWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
					// Serialize using the XmlTextWriter.
					writer.Formatting = Formatting.Indented;
					xs.Serialize(writer, openMessages);
					writer.Close();
					fs.Close();
				}

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void AutomationHosting_FEAStatusChanged(object sender, ISEventArgs e)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.Normal,
					(Action)(() =>
					{
						FEAStatus = e.Status;
						CommandManager.InvalidateRequerySuggested();
						Actions.Add(string.Format("FEA Status Changed = {0}", e.Status));
					}));
		}

		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (AutomationHosting != null)
					{
						AutomationHosting.Dispose();
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~MainVM()
		// {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}