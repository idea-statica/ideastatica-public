using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// Factory for creating instances of clients/>
	/// </summary>
	public interface IApiServiceFactory<TClient>
	{
		/// <summary>
		/// Create an instance of API client
		/// </summary>
		/// <returns>Instance of client</returns>
		Task<TClient> CreateApiClient();
	}
}
