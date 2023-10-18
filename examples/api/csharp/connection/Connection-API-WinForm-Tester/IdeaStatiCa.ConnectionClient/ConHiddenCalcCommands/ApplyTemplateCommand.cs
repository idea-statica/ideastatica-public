using IdeaStatiCa.ConnectionClient.Model;
using Microsoft.Win32;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	/// <summary>
	/// It is an implementaion of of the command pattern. 
	/// </summary>
	public class ApplyTemplateCommand : ConnHiddenCalcCommandBase
	{
		public ApplyTemplateCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && Model.SelectedConnection != null && !IsCommandRunning);
		}

		/// <summary>
		/// Apply template a template on a connection
		/// </summary>
		/// <param name="parameter">If parameter is a string which represents existing connection template file name it will be applied.
		/// Otherwise an open file dialog will be shown</param>
		public override void Execute(object parameter)
		{
			string connTemplateFileName = string.Empty;

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Idea Connection Template| *.contemp";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			else
			{
				connTemplateFileName = openFileDialog.FileName;
			}

			var service = Model.GetConnectionService();
			var connection = (IConnectionId)parameter;
			var res = service.ApplyTemplate(connection.ConnectionId, connTemplateFileName, Model.TemplateSetting);

			Model.SetStatusMessage(res);
		}
	}
}
