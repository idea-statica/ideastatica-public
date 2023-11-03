using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using IdeaStatiCa.Plugin;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR.Client;
using PluginConstants = IdeaStatiCa.Plugin.Constants;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.RcsClient.Client;
using System.Threading;

namespace IdeaStatiCa.RcsClient.HttpWrapper
{
	/// <summary>
	/// Class to wrap HttpClient for REST calls
	/// </summary>
	public class HttpClientWrapper : IHttpClientWrapper
	{
		private readonly IPluginLogger logger;
		private string baseUrl;
		private string controllerName;
		public Action<string, int> ProgressLogAction { get; set; } = null;
		public Action<string> HeartBeatLogAction { get; set; } = null;

		public HttpClientWrapper(IPluginLogger logger, string baseAddress, string controllerName)
		{
			baseUrl = baseAddress.ToString();
			this.logger = logger;
			this.controllerName = controllerName;
		}

		/// <summary>
		/// Calls GetAsync on HttpClient
		/// </summary>
		/// <typeparam name="TResult">Expected response object</typeparam>
		/// <param name="requestUri">Request endpoint</param>
		/// <param name="acceptHeader">Optional accept header</param>
		/// <returns>Deserialized object from Http response</returns>
		public async Task<TResult> GetAsync<TResult>(string requestUri, string acceptHeader = "application/json")
		{
			var url = baseUrl + "/" + controllerName + "/" + requestUri;
			logger.LogInformation($"Calling {nameof(GetAsync)} method {url} with acceptHeader {acceptHeader}");
			return await ExecuteClientCallAsync<TResult>(async (client) => { return await client.GetAsync(url); }
			, acceptHeader);
		}

		/// <summary>
		/// Calls PostAsync on HttpClient
		/// </summary>
		/// <typeparam name="TResult">Expected response object</typeparam>
		/// <param name="requestUri">Request endpoint</param>
		/// <param name="requestData">Request body object</param>
		/// <param name="acceptHeader">Optional accept header</param>
		/// <returns>Deserialized object from Http response</returns>
		public async Task<TResult> PostAsync<TResult>(string requestUri, object requestData, string acceptHeader = "application/json")
		{
			var url = $"{baseUrl}{PluginConstants.RcsProgressEndpoint}";
			logger.LogInformation($"Calling {nameof(PostAsync)} method {url} with acceptHeader {acceptHeader}");
			HubConnection hubConnection = null;
			
			if(ProgressLogAction != null)
			{
				hubConnection = new HubConnectionBuilder()
					.WithUrl(url)
					.Build();

				hubConnection.On<string, int>(PluginConstants.RcsProgressMethod, (msg, num) => ProgressLogAction(msg, num));

				logger.LogInformation($"Starting hub connection on {url} address");
				await hubConnection.StartAsync();
			}
			
			var result = await ExecuteClientCallAsync<TResult>(async (client) =>
			{
				using (var content = new StringContent(JsonConvert.SerializeObject(requestData), encoding: Encoding.UTF8, "application/json"))
				{
					content.Headers.ContentType.CharSet = "";
					var url = baseUrl + "/" +controllerName + "/" + requestUri;
					return await client.PostAsync(url, content);
				}
			}, acceptHeader);

			if(hubConnection is { })
			{
				logger.LogInformation("Stopping hub connection");
				await hubConnection.StopAsync();
			}

			return result;
		}

		private async Task<TResult> ExecuteClientCallAsync<TResult>(Func<HttpClient, Task<HttpResponseMessage>> clientCall, string acceptHeader)
		{
			HeartbeatChecker heartbeatChecker = null;
			try
			{		
				using (var client = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan })
				{
					heartbeatChecker = new HeartbeatChecker(logger, client, baseUrl + PluginConstants.RcsApiHeartbeat);
					heartbeatChecker.HeartBeatLogAction = HeartBeatLogAction;
					// Periodically check the heartbeat while the long operation is in progress
					var heartbeatTask = heartbeatChecker.StartAsync();
					logger.LogDebug($"Starting HeartbeatChecker on url {baseUrl + PluginConstants.RcsApiHeartbeat}");
					HttpResponseMessage response = await clientCall(client);
					
					if (response is { IsSuccessStatusCode: true })
					{
						logger.LogDebug($"Response is successfull");
						var stringContent = await response.Content.ReadAsStringAsync();
						// Stop the heartbeat checker
						heartbeatChecker.Stop();
						logger.LogDebug($"Stopping HeartbeatChecker");
						return Deserialize<TResult>(acceptHeader, stringContent);
					}

					logger.LogError("Response code was not successfull: " + response.ReasonPhrase);
					throw new HttpRequestException("Response code was not successfull: " + response.StatusCode + ":" + response.ReasonPhrase);
				}
			}
			catch (HttpRequestException ex)
			{
				logger.LogError("API Request for RCS failed.", ex);
				throw new Exception("API Request for RCS failed", ex);
			}
		}
		private TResult Deserialize<TResult>(string acceptHeader, string data)
		{
			return acceptHeader switch
			{
				"application/json" => JsonConvert.DeserializeObject<TResult>(data),
				"application/xml" => DeserializeXml<TResult>(data),
				"text/plain" => (TResult)Convert.ChangeType(data, typeof(string)),
				_ => throw new NotImplementedException($"Serialization for accept header {acceptHeader} is not supported.")
			};
		}

		private TResult DeserializeXml<TResult>(string data)
		{
			var serializer = new XmlSerializer(typeof(TResult));

			using (TextReader reader = new StringReader(data))
			{
				return (TResult)serializer.Deserialize(reader);
			}
		}
	}
}
