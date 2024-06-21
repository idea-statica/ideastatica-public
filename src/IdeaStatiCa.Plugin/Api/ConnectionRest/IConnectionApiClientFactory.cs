using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest
{
	public interface IConnectionApiClientFactory : IDisposable
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
		/// Run a new ConnectionRestAPI server and create an instance of IConnectionApiController that is connected to it
		/// </summary>
		/// <returns>Instance of <see cref="IConnectionApiController"/></returns>
		Task<IConnectionApiController> CreateConnectionApiClient();

		/// <summary>
		/// Create an instance of IConnectionApiController that is connected to the service which listens on <paramref name="uri"/>
		/// </summary>
		/// <param name="uri">Uri of the ConnectionRestAPI service</param>
		/// <returns>Instance of <see cref="IConnectionApiController"/></returns>
		Task<IConnectionApiController> CreateConnectionApiClient(Uri uri);
	}
}
