using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.ConHiddenCalcCommands
{
	/// <summary>
	/// Get json which includes parameters which are defined on the connection
	/// </summary>
	public class GetParametersCommand : ConnHiddenCalcCommandBase
	{
		public GetParametersCommand(IConHiddenCalcModel model) : base(model)
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
			Model.SetResults("Getting geometry parametes of the connection");
			var connectionTask = Task.Run(() =>
			{
				try
				{
					var conVM = (IConnectionId)parameter;
					var Service = Model.GetConnectionService();

					string parametersJson = Service.GetParametersJSON(conVM.ConnectionId);
					var conParameters = new ConnectionDataJson(Guid.Parse(conVM.ConnectionId) ,parametersJson);
					Model.SetResults(conParameters);
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
