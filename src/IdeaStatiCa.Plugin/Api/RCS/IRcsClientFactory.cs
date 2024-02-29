using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Api.RCS
{
	public interface IRcsClientFactory : IDisposable
	{
		/// <summary>
		/// Action to log calculation process
		/// String = Process message of current operation
		/// Int = Percentage value for progress bar
		/// </summary>
		Action<string, int> StreamingLog { get; set; }

		/// <summary>
		/// Action to log heartbeat logs to track if API is active
		/// </summary>
		Action<string> HeartbeatLog { get; set; }

		/// <summary>
		/// Run a new RcsRestAPI server and create an instance of IRcsApiController that is connected to it
		/// </summary>
		/// <returns>Instance of RcsApiController</returns>eturns>
		Task<IRcsApiController> CreateRcsApiClient();

		/// <summary>
		/// Create instance of IRcsApiController which is connected to existing RcsRestApi service which is defined by <paramref name="url"/>
		/// </summary>
		/// <param name="url">Rcs API endpoint</param>
		/// <returns></returns>
		Task<IRcsApiController> CreateRcsApiClient(string url);
	}
}
