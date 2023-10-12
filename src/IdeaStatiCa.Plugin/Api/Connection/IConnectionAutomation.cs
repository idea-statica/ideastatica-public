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
		/// Generate a report of the connection <paramref name="conId"/>
		/// </summary>
		/// <param name="conId">Id of the required connection</param>
		/// <param name="settings">Report settings</param>
		/// <returns>The identifier of a generated report</returns>
		string GenerateReport(int conId, ConnReportSettings settings);
	}
}
