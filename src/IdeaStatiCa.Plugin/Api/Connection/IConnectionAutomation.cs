using IdeaRS.OpenModel.Connection;
using System.ServiceModel;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// 
	/// </summary>
	[ServiceContract]
	public interface IConnectionAutomation
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		ConProjectInfo GetProjectInfo();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="conId"></param>
		/// <returns></returns>
		string GenerateReport(int conId);
	}
}
