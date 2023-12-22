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
		/// Create instance of IRcsApiController that is connected to locally hosted Rest API
		/// </summary>
		/// <returns>Instance of RcsApiController</returns>
		Task<IRcsApiController> CreateRcsApiClient();
	}
}
