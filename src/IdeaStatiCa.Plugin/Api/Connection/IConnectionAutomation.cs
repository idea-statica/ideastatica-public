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
		/// Generate report of <paramref name="conId"/>
		/// </summary>
		/// <param name="conId">Id of the required connection</param>
		/// <returns>The id of the report</returns>
		string GenerateReport(int conId);
	}
}
