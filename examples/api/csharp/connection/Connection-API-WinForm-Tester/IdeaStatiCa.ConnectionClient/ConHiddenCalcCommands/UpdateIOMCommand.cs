using IdeaStatiCa.ConnectionClient.Model;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class UpdateIOMCommand : ConnHiddenCalcCommandBase
	{
		public UpdateIOMCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && Model.SelectedConnection != null && !IsCommandRunning);
		}

		public override void Execute(object parameter)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "IOM | *.xml";
			openFileDialog.CheckFileExists = true;
			if (openFileDialog.ShowDialog() == true)
			{
				Debug.WriteLine("Creating the instance of IdeaRS.ConnectionService.Service.ConnectionSrv");


				var service = Model.GetConnectionService();

				string iomFileName = openFileDialog.FileName;
				string resultsFileName = Path.ChangeExtension(openFileDialog.FileName, ".xmlR");

				try
				{
					// create temporary idea connection project
					service.UpdateConProjFromIOM(iomFileName, resultsFileName);


					var projectInfo = service.GetProjectInfo();
					Model.SetConProjectData(projectInfo);
				}
				catch (Exception e)
				{
					Debug.Assert(false, e.Message);
					Model.SetStatusMessage(e.Message);
					Model.CloseConnectionService();
				}
			}
		}
	}
}
