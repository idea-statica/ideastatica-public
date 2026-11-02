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

		private readonly string clientApplication;
		private readonly string clientApplicationVersion;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseUrl"> URL of the REST API service</param>
		/// <param name="clientApplication">
		/// Name of the application making the calls, for example "NorsokChecker" - a constant of the
		/// build, so it belongs to the factory rather than to a single call. Every client this factory
		/// creates is reported under it, which is what lets usage be attributed to an integration at
		/// all; see <see cref="ClientApplicationIdentity"/>. Optional.
		/// </param>
		/// <param name="clientApplicationVersion">Version of that application. Optional.</param>
		public ConnectionApiServiceAttacher(string baseUrl, string clientApplication = null,
			string clientApplicationVersion = null)
		{
			this.BaseUrl = baseUrl;
			this.clientApplication = clientApplication;
			this.clientApplicationVersion = clientApplicationVersion;
		}

		/// <inheritdoc cref="IApiServiceFactory{T}.CreateApiClient"/>
		public async Task<IConnectionApiClient> CreateApiClient()
		{
			var client = new ConnectionApiClient(BaseUrl, clientApplication, clientApplicationVersion);
			await client.CreateAsync();
			return client;
		}
	}		
}
