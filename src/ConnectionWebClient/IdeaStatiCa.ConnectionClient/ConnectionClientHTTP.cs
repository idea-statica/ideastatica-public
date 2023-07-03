using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using IdeaRS.OpenModel.Connection;
using Newtonsoft.Json;

namespace IdeaStatiCa.ConnectionClient
{
	public class ConnectionClientHTTP : IConnectionClient
	{
		private readonly HttpClient httpClient;
		public const string ConCalculatorVersionAPI = "1";
		//private readonly JsonSerializerOptions jsonSerializerOptions;

		private OpenProjectResult Project { get; set; }

		public ConnectionClientHTTP(HttpClient httpClient)
		{
			//jsonSerializerOptions = new JsonSerializerOptions();
			//jsonSerializerOptions.
			this.httpClient = httpClient;
		}

		public async Task CloseProjectAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<ConProjectInfo> OpenProjectAsync(Stream ideaConProject, CancellationToken cancellationToken)
		{

			using (MultipartFormDataContent formData = new MultipartFormDataContent())
			{
				using (StreamContent streamContent = new StreamContent(ideaConProject))
				{
					// Add the StreamContent as a form field
					formData.Add(streamContent, "ideaConFile", "connection.ideacon");

					var response = await httpClient.PostAsync($"api/{ConCalculatorVersionAPI}/project/upload", formData, cancellationToken);

					response.EnsureSuccessStatusCode();

					var responseJson = await response.Content.ReadAsStringAsync();

					Project = JsonConvert.DeserializeObject<OpenProjectResult>(responseJson);
				}
			}
			return Project.ProjectInfo;
		}

	}
}
