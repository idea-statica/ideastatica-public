using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// Factory for creating instances of clients/>
	/// </summary>
	public interface IApiServiceController<TClient>
	{
		/// <summary>
		/// Create an instance of IConnectionApiClient
		/// </summary>
		/// <returns>Instance of client</returns>
		Task<TClient> CreateConnectionApiClient();
	}
}
