using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Public;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ConnectionAutomationApp
{
	public class MainVM : INotifyPropertyChanged, IDisposable
	{
		bool isIdea;
		readonly string ideaStatiCaDir;
		string statusMessage;
		IConnectionController connectionController;
		string currentProjectFileName;

		public event PropertyChangedEventHandler PropertyChanged;

		public MainVM()
		{
			ideaStatiCaDir = Properties.Settings.Default.IdeaStatiCaDir;
			if (Directory.Exists(ideaStatiCaDir))
			{
				string ideaConnectionFileName = Path.Combine(ideaStatiCaDir, "IdeaConnection.exe");
				if (File.Exists(ideaConnectionFileName))
				{
					IsIdea = true;
					StatusMessage = string.Format("IdeaStatiCa installation was found in '{0}'", ideaStatiCaDir);
				}
			}

			if (!IsIdea)
			{
				StatusMessage = string.Format("ERROR IdeaStatiCa doesn't exist in '{0}'", ideaStatiCaDir);
			}

			RunIdeaConnectionCmd = new CustomCommand(this.CanRunIdeaConnection, this.RunIdeaConnection);
			OpenProjectCmd = new CustomCommand(this.CanOpenProject, this.OpenProject);
			CloseProjectCmd = new CustomCommand(this.CanCloseProject, this.CloseProject);
			GenerateReportCmd = new CustomCommand(this.CanGenerateReport, this.GenerateReport);
			GeneratePdfReportCmd = new CustomCommand(this.CanGenerateReport, this.GenerateReport);
			GenerateWordReportCmd = new CustomCommand(this.CanGenerateReport, this.GenerateReport);
		}


		#region Commands
		public CustomCommand RunIdeaConnectionCmd { get; set; }
		public CustomCommand OpenProjectCmd { get; set; }
		public CustomCommand CloseProjectCmd { get; set; }
		public CustomCommand GenerateReportCmd { get; set; }
		public CustomCommand GeneratePdfReportCmd { get; set; }
		public CustomCommand GenerateWordReportCmd { get; set; }
		#endregion

		#region Command parameters
		public ConnReportTypeEnum PdfReportType => ConnReportTypeEnum.Pdf;
		public ConnReportTypeEnum WordReportType => ConnReportTypeEnum.Word;
		public ConnReportTypeEnum DXFReportType => ConnReportTypeEnum.DXF;
		public ConnReportTypeEnum ZipReportType => ConnReportTypeEnum.Zip;

		#endregion

		public bool IsIdea
		{
			get => isIdea;

			set
			{
				isIdea = value;
				NotifyPropertyChanged("IsIdea");
			}
		}

		public string StatusMessage
		{
			get => statusMessage;
			set
			{
				statusMessage = value;
				NotifyPropertyChanged("StatusMessage");
			}
		}

		public IConnectionController ConnectionController
		{
			get => connectionController;
			set
			{
				connectionController = value;
				NotifyPropertyChanged("ConnectionController");
			}
		}

		public string CurrentProjectFileName
		{
			get => currentProjectFileName;
			set
			{
				currentProjectFileName = value;
				NotifyPropertyChanged("CurrentProjectFileName");
			}
		}

		private void CloseProject(object obj)
		{
			// close the currently open project
			ConnectionController.CloseProject();
			CurrentProjectFileName = string.Empty;
		}

		private bool CanCloseProject(object arg)
		{
			return !string.IsNullOrEmpty(CurrentProjectFileName);
		}

		private void OpenProject(object obj)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "IdeaConnection | *.ideacon";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}

			try
			{
				// open selected idea connection project in the running application IdeaConnection.exe
				ConnectionController.OpenProject(openFileDialog.FileName);
				CurrentProjectFileName = openFileDialog.FileName;
			}
			catch (Exception e)
			{
				Debug.Assert(false, e.Message);
				CurrentProjectFileName = string.Empty;
			}
		}

		private bool CanOpenProject(object arg)
		{
			return ((this.ConnectionController != null) && string.IsNullOrEmpty(CurrentProjectFileName));
		}

		private void GenerateReport(object obj)
		{
			if(obj is ConnReportTypeEnum reportType)
			{
				try
				{
					var settings = new ConnReportSettings();
					var saveFileDialog = new SaveFileDialog
					{
						Filter = reportType switch
						{
							ConnReportTypeEnum.Pdf => "pdf | *.pdf",
							ConnReportTypeEnum.Word => "doc | *.doc",
							_ => throw new NotImplementedException($"Conversion for report type {reportType} is not implemented.")
						}
					};

					if(reportType == ConnReportTypeEnum.Zip)
					{
						GenerateReportZipFolder();
						return;
					}

					if (saveFileDialog.ShowDialog() != true)
					{
						return;
					}

					switch (reportType)
					{
						case ConnReportTypeEnum.Pdf:
							ConnectionController.GeneratePdfReport(1, saveFileDialog.FileName, new ConnReportSettings());
							break;
						case ConnReportTypeEnum.Word:
							ConnectionController.GenerateWordReport(1, saveFileDialog.FileName, new ConnReportSettings());
							break;
					}
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message, "Error");
				}
			}

		}

		private void GenerateReportZipFolder()
		{
			try
			{
				var blobStorage = ConnectionController.GenerateReport(1, new ConnReportSettings());

				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					Filter = "zip | *.zip"
				};

				if (saveFileDialog.ShowDialog() != true)
				{
					return;
				}

				using (var stream = saveFileDialog.OpenFile())
				{

					using (BlobStorageInArchive archive = new BlobStorageInArchive(stream))
					{
						archive.CopyFrom(blobStorage);
					}
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error");
			}
		}

		private bool CanGenerateReport(object arg)
		{
			return !string.IsNullOrEmpty(CurrentProjectFileName); ;
		}

		private void RunIdeaConnection(object obj)
		{
			ConnectionController = IdeaConnectionController.Create(ideaStatiCaDir);
		}

		private bool CanRunIdeaConnection(object arg)
		{
			return (this.ConnectionController == null) || (this.ConnectionController.IsConnected == false);
		}

		private void NotifyPropertyChanged(string propertyName = "")
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
					// TODO: dispose managed state (managed objects).
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
		#endregion
	}

	public enum ConnReportTypeEnum
	{
		Pdf,
		Word,
		DXF,
		Zip
	}
}
