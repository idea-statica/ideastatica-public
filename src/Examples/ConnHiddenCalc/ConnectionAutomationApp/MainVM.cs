﻿using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.Plugin;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace ConnectionAutomationApp
{
	public class MainVM : INotifyPropertyChanged, IDisposable
	{
		bool isIdea;
		bool useGrpcCommunication;
		readonly string ideaStatiCaDir;
		string statusMessage;
		IConnectionController connectionController;
		string currentProjectFileName;

		public event PropertyChangedEventHandler PropertyChanged;

		public MainVM()
		{
			UseGrpcCommunication = true;
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
		}

		#region Commands
		public CustomCommand RunIdeaConnectionCmd { get; set; }
		public CustomCommand OpenProjectCmd { get; set; }
		public CustomCommand CloseProjectCmd { get; set; }
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

		public bool UseGrpcCommunication
		{
			get => useGrpcCommunication;

			set
			{
				useGrpcCommunication = value;
				NotifyPropertyChanged("UseGrpcCommunication");
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

		private void RunIdeaConnection(object obj)
		{
			if(UseGrpcCommunication)
			{
				this.ConnectionController = IdeaConnectionControllerGrpc.Create(ideaStatiCaDir);
			}
			else
			{
				this.ConnectionController = IdeaConnectionController.Create(ideaStatiCaDir);
			}
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
}
