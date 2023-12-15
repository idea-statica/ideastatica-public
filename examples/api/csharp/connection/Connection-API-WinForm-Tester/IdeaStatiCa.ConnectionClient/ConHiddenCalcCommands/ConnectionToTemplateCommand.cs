using IdeaStatiCa.ConnectionClient.Model;
using Microsoft.Win32;
using System;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class ConnectionToTemplateCommand : ConnHiddenCalcCommandBase
	{
		public ConnectionToTemplateCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && Model.SelectedConnection != null && !IsCommandRunning);
		}

		public override void Execute(object parameter)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Idea Connection Template| *.contemp";
			if (saveFileDialog.ShowDialog() == true)
			{
				var service = Model.GetConnectionService();
				var connection = (IConnectionId)parameter;
				string res = service.ExportToTemplate(connection.ConnectionId, saveFileDialog.FileName);
				Model.SetStatusMessage(res);
			}
		}
	}
}
