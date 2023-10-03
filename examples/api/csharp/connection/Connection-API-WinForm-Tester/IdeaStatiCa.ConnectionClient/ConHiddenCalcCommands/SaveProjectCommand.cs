using IdeaStatiCa.ConnectionClient.Model;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class SaveProjectCommand : ConnHiddenCalcCommandBase
	{
		public SaveProjectCommand(IConHiddenCalcModel model) : base(model)
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
			Model.GetConnectionService().Save();
		}
	}
}
