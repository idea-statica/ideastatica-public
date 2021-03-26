using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.ConnectionClient.Model
{
	public interface IConHiddenCalcModel
	{
		bool IsService { get; }

		bool IsIdea { get; }

		IConnectionId SelectedConnection { get; }

		ApplyConnTemplateSetting TemplateSetting { get; }

		IConnHiddenCheck GetConnectionService();

		void CloseConnectionService();

		void SetConProjectData(ConProjectInfo projectData);

		void SetResults(object res);

		void SetStatusMessage(string msg);
	}
}
