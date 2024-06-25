using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.ConHiddenCalcCommands
{
	/// <summary>
	/// 
	/// </summary>
	public class UpdateConnParamsCommand : ConnHiddenCalcCommandBase, IUpdateCommand
	{
		public event EventHandler UpdateFinished;

		public UpdateConnParamsCommand(IConHiddenCalcModel model) : base(model)
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
			Model.SetResults("Updating parameters of the connection");
			var connCalculatorTask = Task.Run(async () =>
			{
				try
				{
					var updatedParameters = (IConnectionDataJson)parameter;
					var Service = Model.GetConnectionService();

					await Service.ApplyParametersAsync(updatedParameters.ConnectionId.ToString(), updatedParameters.DataJson);

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
