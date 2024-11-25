using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi
{
	public class ConnectionApiServiceAttacher : IApiServiceController<IConnectionApiClient>
	{
		string BaseUrl { get; set; }

		/// <summary>
		/// Con
		/// </summary>
		/// <param name="baseUrl"></param>
		public ConnectionApiServiceAttacher(string baseUrl)
		{
			this.BaseUrl = baseUrl;
		}

		/// <inheritdoc cref="IApiServiceController.CreateConnectionApiClient"/>
		public async Task<IConnectionApiClient> CreateApiClient()
		{
			var client = new ConnectionApiClient(BaseUrl);
			await client.CreateAsync();
			return await Task.FromResult(client);
		}
	}		
}
