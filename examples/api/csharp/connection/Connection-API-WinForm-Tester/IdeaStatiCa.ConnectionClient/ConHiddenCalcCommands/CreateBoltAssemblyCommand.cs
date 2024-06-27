using IdeaStatiCa.ConnectionClient.Model;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class CreateBoltAssemblyCommand : ConnHiddenCalcCommandBase
	{
		public CreateBoltAssemblyCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && !IsCommandRunning);
		}

		public override void Execute(object parameter)
		{
			string newBoltAssemblyName = parameter.ToString();
			IsCommandRunning = true;
			Model.SetResults("Adding a new bolt assemblis to the project");
			var connCalculatorTask = Task.Run(async () =>
			{
				try
				{
					var Service = Model.GetConnectionService();

					int newBoltAssemblyId = await Service.AddBoltAssemblyAsync(newBoltAssemblyName);

					if (newBoltAssemblyId == -1)
					{
						Model.SetResults($"Can not create the bolt assebly {newBoltAssemblyName}");
					}
					else
					{
						Model.SetResults($"The bolt assembly '{newBoltAssemblyName}' has been created. Its ID is {newBoltAssemblyId}");
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
