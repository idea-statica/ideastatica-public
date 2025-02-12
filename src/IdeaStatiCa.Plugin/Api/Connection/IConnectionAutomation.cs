using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Public;


namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// 
	/// </summary>
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

		/// <summary>
		/// Generate a report of the connection <paramref name="conId"/>
		/// </summary>
		/// <param name="conId">Id of the required connection</param>
		/// <param name="filePath">The file path</param>
		/// <param name="settings">Report settings</param>
		/// <returns>The byte content of a generated PDF report</returns>
		void GeneratePdfReport(int conId, string filePath, ConnReportSettings settings);

		/// <summary>
		/// Generate a PDF report of the connection <paramref name="conId"/>
		/// </summary>
		/// <param name="conId">Id of the required connection</param>
		/// <param name="settings">Report settings</param>
		/// <returns>The identifier of a generated report</returns>
		string GeneratePdfReportIdentifier(int conId, ConnReportSettings settings);

		/// <summary>
		/// Generate a report of the connection <paramref name="conId"/>
		/// </summary>
		/// <param name="conId">Id of the required connection</param>
		/// <param name="filePath">The file path</param>
		/// <param name="settings">Report settings</param>
		/// <returns>The byte content of a generated Word report</returns>
		void GenerateWordReport(int conId, string filePath, ConnReportSettings settings);

		/// <summary>
		/// Generate a Word report of the connection <paramref name="conId"/>
		/// </summary>
		/// <param name="conId">Id of the required connection</param>
		/// <param name="settings">Report settings</param>
		/// <returns>The identifier of a generated report</returns>
		string GenerateWordReportIdentifier(int conId, ConnReportSettings settings);
	}
}
