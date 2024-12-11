using IdeaStatiCa.Api.Common;
using System.Threading.Tasks;

namespace IdeaStatiCa.RcsApi
{
	/// <summary>
	/// Factory for creating instances of RCS API client that are connected to the specified REST API service
	/// </summary>
	public class RcsApiServiceAttacher : IApiServiceFactory<IRcsApiClient>
	{
		string BaseUrl { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseUrl"> URL of the REST API service</param>
		public RcsApiServiceAttacher(string baseUrl)
		{
			this.BaseUrl = baseUrl;
		}

		/// <inheritdoc cref="IApiServiceFactory{TClient}.CreateApiClient"/>
		public async Task<IRcsApiClient> CreateApiClient()
		{
			var client = new RcsApiClient(BaseUrl);
			await client.CreateAsync();
			return client;
		}
	}		
}
