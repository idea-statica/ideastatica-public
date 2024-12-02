using IdeaStatiCa.Api.Common;
using System.Threading.Tasks;

namespace IdeaStatiCa.RcsApi
{
	public class RcsApiServiceAttacher : IApiServiceFactory<IRcsApiClient>
	{
		string BaseUrl { get; set; }

		/// <summary>
		/// Con
		/// </summary>
		/// <param name="baseUrl"></param>
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
