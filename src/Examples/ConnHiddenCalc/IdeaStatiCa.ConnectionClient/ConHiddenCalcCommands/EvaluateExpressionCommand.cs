using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.ConHiddenCalcCommands
{
	public class EvaluateExpressionCommand : ConnHiddenCalcCommandBase
	{
		public EvaluateExpressionCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && !IsCommandRunning);
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

					string evalResJson = Service.EvaluateExpression(conVM.ConnectionId, "aaa", string.Empty);
					//var conParameters = new ConnectionDataJson(Guid.Parse(conVM.ConnectionId), parametersJson);


					Model.SetResults(evalResJson);
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
