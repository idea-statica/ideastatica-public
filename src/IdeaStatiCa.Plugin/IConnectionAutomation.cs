using IdeaRS.OpenModel.Connection;
using System.ServiceModel;

namespace IdeaStatiCa.Plugin
{
	[ServiceContract]
	public interface IConnectionAutomation
	{
		ConProjectInfo GetProjectInfo();

	}
}
