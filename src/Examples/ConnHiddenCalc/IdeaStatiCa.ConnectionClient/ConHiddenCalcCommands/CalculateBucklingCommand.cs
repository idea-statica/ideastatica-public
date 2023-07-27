using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.ConHiddenCalcCommands
{
	public class CalculateBucklingCommand : ConnHiddenCalcCommandBase
	{
		public CalculateBucklingCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && Model.SelectedConnection != null && !IsCommandRunning);
		}

		public override void Execute(object parameter)
		{
			var res = string.Empty;
			Model.SetResults("Running buckling analysis");
			IsCommandRunning = true;

			var connCalculatorTask = Task.Run(() =>
			{
				try
				{
					var connection = (IConnectionId)parameter;
					var service = Model.GetConnectionService();

					var resData = service.CalculateBuckling(connection.ConnectionId);

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
