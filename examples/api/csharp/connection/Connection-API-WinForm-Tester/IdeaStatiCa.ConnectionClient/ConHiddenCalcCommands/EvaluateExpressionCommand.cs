using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Collections.Generic;
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
					var values = (object[])parameter;
					var conVM = (IConnectionId)values[0];
					string expression = values[1].ToString();
					var Service = Model.GetConnectionService();

					//string evalResJson = Service.EvaluateExpression(conVM.ConnectionId, "BeamByOperationId([1])", string.Empty);
					string evalResJson = Service.EvaluateExpression(conVM.ConnectionId, expression, string.Empty);

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
