using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.Public;
using System;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Automation of IdeaConnection app
	/// </summary>
	public interface IConnectionController
	{
		/// <summary>
		/// Fires when IdeaConnection app exited.
		/// </summary>
		event EventHandler ConnectionAppExited;

		/// <summary>
		/// Indicates whether communication with IdeaConnection.exe is alive.
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// Open idea con project in IdeaConnection
		/// </summary>
		/// <param name="fileName">The file path to the ideaCon.</param>
		/// <returns></returns>
		int OpenProject(string fileName);

		/// <summary>
		/// Close the open project
		/// </summary>
		/// <returns></returns>
		int CloseProject();

		/// <summary>
		/// Send a request to generate a connection report and return a blobstorage with its data 
		/// </summary>
		/// <param name="conId">Id of the requested connection</param>
		/// <param name="settings">Report settings</param>
		/// <returns>The instance of the blobstorage</returns>
		IBlobStorage GenerateReport(int conId, ConnReportSettings settings);

		/// <summary>
		/// Send a request to generate connection report and return word document with its data
		/// </summary>
		/// <param name="conId">Id of the requested connection</param>
		/// <param name="settings">Report settings</param>
		/// <returns>Byte array of report content in docs format</returns>
		byte[] GenerateWordReport(int conId, ConnReportSettings settings);

		/// <summary>
		/// Send a request to generate connection report and return PDF document with its data
		/// </summary>
		/// <param name="conId">Id of the requested connection</param>
		/// <param name="settings">Report settings</param>
		/// <returns>Byte array of report content in PDF format</returns>
		byte[] GeneratePdfReport(int conId, ConnReportSettings settings);
	}
}
