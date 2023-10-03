using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.ConHiddenCalcCommands
{
	public class UpdateLoadingCommand : ConnHiddenCalcCommandBase, IUpdateCommand
	{
		public event EventHandler UpdateFinished;

		public UpdateLoadingCommand(IConHiddenCalcModel model) : base(model)
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
			Model.SetResults("Updating loading of the connection");
			var connCalculatorTask = Task.Run(() =>
			{
				try
				{
					var updatedLoading = (IConnectionDataJson)parameter;
					var Service = Model.GetConnectionService();

					Service.UpdateLoadingFromJson(updatedLoading.ConnectionId.ToString(), updatedLoading.DataJson);

					NotifyCommandFinished();
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

		private void NotifyCommandFinished()
		{
			if (UpdateFinished != null)
			{
				UpdateFinished.Invoke(this, new EventArgs());
			}
		}
	}
}
