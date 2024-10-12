using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi
{
	public class ConnectionApiClientFactory : IConnectionApiClientFactory
	{
		string BaseUrl { get; set; }
		/// <summary>
		/// Constructor
		/// </summary>
		public ConnectionApiClientFactory(string baseUrl)
		{
			this.BaseUrl = baseUrl;
		}

		/// <inheritdoc cref="IConnectionApiClientFactory.CreateConnectionApiClient"/>
		public async Task<IConnectionApiClient> CreateConnectionApiClient()
		{
			var client = new ConnectionApiClient(BaseUrl);
			await client.CreateAsync();
			return await Task.FromResult(client);
		}
	}		
}
