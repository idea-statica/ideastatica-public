using IdeaStatiCa.ConnectionClient.Model;
using Microsoft.Win32;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class SaveAsProjectCommand : ConnHiddenCalcCommandBase
	{
		public SaveAsProjectCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return Model.IsService;
		}

		/// <summary>
		/// Run command for saving connection
		/// </summary>
		/// <param name="parameter">File name. If null SaveFileDialog will be open to get the filename</param>
		public override void Execute(object parameter)
		{
			string projectFileName = string.Empty;
			if(parameter != null)
			{
				projectFileName = parameter.ToString();
			}

			if (string.IsNullOrEmpty(projectFileName))
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "IdeaConnection | *.ideacon";
				if (saveFileDialog.ShowDialog() == true)
				{
					var service = Model.GetConnectionService();
					service.SaveAsProject(saveFileDialog.FileName);
				}
			}
			else
			{
				var service = Model.GetConnectionService();
				service.SaveAsProject(projectFileName);
			}
		}
	}
}
