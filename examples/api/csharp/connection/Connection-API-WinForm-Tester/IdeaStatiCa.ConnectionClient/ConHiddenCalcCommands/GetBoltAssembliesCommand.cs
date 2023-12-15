using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class GetBoltAssembliesCommand : ConnHiddenCalcCommandBase
	{
		public GetBoltAssembliesCommand(IConHiddenCalcModel model) : base(model)
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
			Model.SetResults("Getting bolt assemblis in the project");
			var connCalculatorTask = Task.Run(() =>
			{
				try
				{
					var Service = Model.GetConnectionService();
					var boltsInProject = Service.GetBoltAssembliesInProject();

					if (boltsInProject != null)
					{
						Model.SetResults(boltsInProject);
					}
					else
					{
						Model.SetResults("No bolt assemblies");
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
