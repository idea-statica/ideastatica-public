using IdeaStatiCa.ConnectionClient.Model;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class CloseProjectCommand : ConnHiddenCalcCommandBase
	{
		public CloseProjectCommand(IConHiddenCalcModel model, IPluginLogger logger = null) : base(model, logger)
		{
		}

		/// <summary>
		/// Is there a project to close ?
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		public override bool CanExecute(object parameter)
		{
			return Model.IsService;
		}

		/// <summary>
		/// Close the current idea connection project
		/// </summary>
		/// <param name="param"></param>
		public override void Execute(object parameter)
		{
			Logger.LogInformation("CloseProjectCommand.Execute");
			Model.CloseConnectionService();
		}
	}
}
