using IdeaStatiCa.ConnectionClient.Model;
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	/// <summary>
	/// It is an implementaion of of the command pattern. 
	/// </summary>
	public class OpenProjectCommand : ConnHiddenCalcCommandBase
	{
		public OpenProjectCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && !Model.IsService);
		}

		/// <summary>
		/// Open IDEA Connection project 
		/// </summary>
		/// <param name="parameter">If parameter is a string which represents existing IDEA connection project file name it will be open.
		/// Otherwise an open file dialog will be shown</param>
		public override void Execute(object parameter)
		{
			string connProjectFileName = string.Empty;

			if (parameter != null)
			{
				connProjectFileName = parameter.ToString();
			}
			else
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "IdeaConnection | *.ideacon";
				if (openFileDialog.ShowDialog() == true)
				{
					connProjectFileName = openFileDialog.FileName;
				}
				else
				{
					return;
				}
			}

			try
			{
				Debug.WriteLine("Creating the instance of IdeaRS.ConnectionService.Service.ConnectionSrv");

				var Service = Model.GetConnectionService();

				Debug.WriteLine("Opening the project file '{0}'", connProjectFileName);
				Service.OpenProject(connProjectFileName);

				var projectInfo = Service.GetProjectInfo();
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
