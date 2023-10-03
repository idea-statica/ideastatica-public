using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.ComponentModel;

namespace ConnectionHiddenCalculation
{
	public class ConnectionVM : INotifyPropertyChanged, IConnectionId
	{
		public event PropertyChangedEventHandler PropertyChanged;
		ConnectionInfo connectionInfo;

		public ConnectionVM(ConnectionInfo item)
		{
			this.connectionInfo = item;
		}

		public string Name
		{
			get => connectionInfo.Name;
		}

		public string ConnectionId
		{
			get => connectionInfo.Identifier;
		}

		private void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
