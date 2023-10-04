using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	/// <summary>
	/// Get the instance of IdeaRS.OpenModel.OpenModelContainer for selected connection. It includes structural data and corresponding results of FE analysis.
	/// </summary>
	public class GetAllConnDataCommand : ConnHiddenCalcCommandBase
	{
		public GetAllConnDataCommand(IConHiddenCalcModel model) : base(model)
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
					string openModelTupleInXml = Service.GetAllConnectionData(conVM.ConnectionId);
					IdeaRS.OpenModel.OpenModelContainer openModelTuple = IdeaStatiCa.Plugin.Tools.OpenModelContainerFromXml(openModelTupleInXml);

					if (openModelTuple != null)
					{
						Model.SetResults(openModelTuple);
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
