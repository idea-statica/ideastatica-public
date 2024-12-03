using IdeaStatiCa.Api.Common;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// <see cref="ConnectionApiServiceAttacher"/> is a factory for creating <see cref="IConnectionApiClient"/> instances that are connected to the Connection REST API.
	/// </summary>
	public class ConnectionApiServiceAttacher : IApiServiceFactory<IConnectionApiClient>
	{
		string BaseUrl { get; set; }

		/// <summary>
		/// Constructor of the <see cref="ConnectionApiServiceAttacher"/> class that takes the base URL of the Connection REST API.
		/// </summary>
		/// <param name="baseUrl"></param>
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
