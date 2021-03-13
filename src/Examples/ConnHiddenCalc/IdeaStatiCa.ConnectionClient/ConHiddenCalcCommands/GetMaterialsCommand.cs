using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class GetMaterialsCommand : ConnHiddenCalcCommandBase
	{
		public GetMaterialsCommand(IConHiddenCalcModel model) : base(model)
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
			Model.SetResults("Getting materials in the project");
			var connCalculatorTask = Task.Run(() =>
			{
				try
				{
					var Service = Model.GetConnectionService();
					var materialsInProject = Service.GetMaterialsInProject();

					if (materialsInProject != null)
					{
						Model.SetResults(materialsInProject);
					}
					else
					{
						Model.SetResults("No materials");
					}
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
