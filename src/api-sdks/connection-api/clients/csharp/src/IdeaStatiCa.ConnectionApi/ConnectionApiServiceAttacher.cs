using IdeaStatiCa.Api.Common;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// Factory for creating instances of Connection API client that are connected to the specified REST API service
	/// </summary>
	public class ConnectionApiServiceAttacher : IApiServiceFactory<IConnectionApiClient>
	{
		string BaseUrl { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseUrl"> URL of the REST API service</param>
		public ConnectionApiServiceAttacher(string baseUrl)
		{
			this.BaseUrl = baseUrl;
		}

		/// <inheritdoc cref="IApiServiceFactory.CreateConnectionApiClient"/>
		public async Task<IConnectionApiClient> CreateApiClient()
		{
			var client = new ConnectionApiClient(BaseUrl);
			await client.CreateAsync();
			return client;
		}
	}		
}
