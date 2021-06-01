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
			var res = service.DeleteAllOperations(connection.ConnectionId);
			Model.SetStatusMessage(res);
		}
	}
}
