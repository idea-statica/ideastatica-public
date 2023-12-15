using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class GetCrossSectionsCommand : ConnHiddenCalcCommandBase
	{
		public GetCrossSectionsCommand(IConHiddenCalcModel model) : base(model)
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
			Model.SetResults("Getting cross-sections in the project");
			var connCalculatorTask = Task.Run(() =>
			{
				try
				{
					var Service = Model.GetConnectionService();
					var cssInProject = Service.GetCrossSectionsInProject();

					if (cssInProject != null)
					{
						Model.SetResults(cssInProject);
					}
					else
					{
						Model.SetResults("No cross-sections");
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
