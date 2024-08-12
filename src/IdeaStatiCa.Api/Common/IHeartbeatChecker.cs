using System.Threading.Tasks;

namespace IdeaStatiCa.Api.Common
{
	public interface IHeartbeatChecker
	{
		/// <summary>
		/// Start heartbeat task to track if API is active
		/// </summary>
		/// <returns></returns>
		Task StartAsync();
		/// <summary>
		/// Stop heartbeat task
		/// </summary>
		void Stop();
	}
}