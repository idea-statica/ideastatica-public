using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.Model
{
	public interface IConHiddenCalcModel
	{
		bool IsService { get; }

		bool IsIdea { get; }

		ApplyConnTemplateSetting TemplateSetting { get; }

		IConnHiddenCheck GetConnectionService();

		void CloseConnectionService();

		void SetConProjectData(ConProjectInfo projectData);

		void SetResults(object res);

		void SetStatusMessage(string msg);
	}
}
