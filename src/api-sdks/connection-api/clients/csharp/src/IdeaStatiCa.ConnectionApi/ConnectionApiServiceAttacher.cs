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

		/// <inheritdoc cref="IApiServiceFactory{T}.CreateApiClient"/>
		public Task<IConnectionApiClient> CreateApiClient() => CreateApiClient(null, null);

		/// <summary>
		/// Creates a client that identifies the calling application to the service, so its usage can be
		/// told apart from every other caller's. See <see cref="ClientApplicationIdentity"/>.
		/// </summary>
		/// <param name="clientApplication">Name of the application making the calls.</param>
		/// <param name="clientApplicationVersion">Version of that application. Optional.</param>
		public async Task<IConnectionApiClient> CreateApiClient(string clientApplication,
			string clientApplicationVersion = null)
		{
			var client = new ConnectionApiClient(BaseUrl, clientApplication, clientApplicationVersion);
			await client.CreateAsync();
			return client;
		}
	}		
}
