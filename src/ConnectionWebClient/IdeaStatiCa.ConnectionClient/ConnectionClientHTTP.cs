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

		private OpenProjectResult Project { get; set; }

		public ConnectionClientHTTP(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public async Task CloseProjectAsync(CancellationToken cancellationToken)
		{
			var response = await httpClient.GetAsync($"api/{ConCalculatorVersionAPI}/project/{Project.OpenProjectId}/close");

			response.EnsureSuccessStatusCode();
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

		public async Task<ConnectionCheckRes> GetPlasticBriefResultsAsync(int connectionId, CancellationToken cancellationToken)
		{
			var response = await httpClient.GetAsync($"api/{ConCalculatorVersionAPI}/connection/{Project.OpenProjectId}/{connectionId}/plastic-brief-results");

			response.EnsureSuccessStatusCode();

			var responseJson = await response.Content.ReadAsStringAsync();

			var res = JsonConvert.DeserializeObject<ConnectionCheckRes>(responseJson);
			return res;
		}

		public async Task<string> GetPlasticDetailedResultsJsonAsync(int connectionId, CancellationToken cancellationToken)
		{
			var response = await httpClient.GetAsync($"api/{ConCalculatorVersionAPI}/connection/{Project.OpenProjectId}/{connectionId}/plastic-detailed-results");

			response.EnsureSuccessStatusCode();

			var responseJson = await response.Content.ReadAsStringAsync();
			
			return responseJson;
		}

		public async Task<ConnectionCheckRes> GetBucklingBriefResultsAsync(int connectionId, CancellationToken cancellationToken)
		{
			var response = await httpClient.GetAsync($"api/{ConCalculatorVersionAPI}/connection/{Project.OpenProjectId}/{connectionId}/buckling-brief-results");

			response.EnsureSuccessStatusCode();

			var responseJson = await response.Content.ReadAsStringAsync();

			var res = JsonConvert.DeserializeObject<ConnectionCheckRes>(responseJson);
			return res;
		}

		public async Task<string> GetBucklingDetailedResultsJsonAsync(int connectionId, CancellationToken cancellationToken)
		{
			var response = await httpClient.GetAsync($"api/{ConCalculatorVersionAPI}/connection/{Project.OpenProjectId}/{connectionId}/buckling-detailed-results");

			response.EnsureSuccessStatusCode();

			var responseJson = await response.Content.ReadAsStringAsync();

			return responseJson;
		}
	}
}
