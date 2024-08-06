using IdeaStatiCa.ConnectionClient.Model;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class DeleteAllOperationsCommand : ConnHiddenCalcCommandBase
	{
		public DeleteAllOperationsCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && Model.SelectedConnection != null && !IsCommandRunning);
		}

		public override void Execute(object parameter)
		{
			var service = Model.GetConnectionService();
			var connection = (IConnectionId)parameter;
			var res = service.DeleteAllOperationsAsynchronous(connection.ConnectionId).Result;
			Model.SetStatusMessage(res);
		}
	}
}
