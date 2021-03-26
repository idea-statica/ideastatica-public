using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class CalculateConnectionCommand : ConnHiddenCalcCommandBase
	{
		public CalculateConnectionCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && Model.SelectedConnection != null && !IsCommandRunning);
		}

		public override void Execute(object parameter)
		{
			var res = string.Empty;
			Model.SetResults("Running CBFEM");
			IsCommandRunning = true;

			var connCalculatorTask = Task.Run(() =>
			{
				try
				{
					var connection = (IConnectionId)parameter;
					var service = Model.GetConnectionService();

					var resData = service.Calculate(connection.ConnectionId);
					
					Model.SetResults(resData);
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
