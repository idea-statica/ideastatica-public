using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class ConnectionGeometryCommand : ConnHiddenCalcCommandBase
	{
		public ConnectionGeometryCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && Model.SelectedConnection != null && !IsCommandRunning);
		}

		public override void Execute(object parameter)
		{
			var res = string.Empty;
			IsCommandRunning = true;
			Model.SetResults("Getting geometry of the connection");
			var connCalculatorTask = Task.Run(() =>
			{
				try
				{
					var conVM = (IConnectionId)parameter;
					var Service = Model.GetConnectionService();

					// cchange in version 20.1 - connection model must pe passed by XML string otherwise it crashes. Why ? Is it a bug in WCF ???
					string conModelXml = Service.GetConnectionModelXML(conVM.ConnectionId);
					IdeaRS.OpenModel.Connection.ConnectionData conData = IdeaStatiCa.Plugin.Tools.ConnectionDataFromXml(conModelXml);

					if (conData != null)
					{
						Model.SetResults(conData);
					}
					else
					{
						Model.SetResults("No data");
					}
				}
				catch (Exception e)
				{
					Model.SetStatusMessage(e.Message);
				}
				finally
				{
					IsCommandRunning = false;
				}
			});
		}
	}
}
