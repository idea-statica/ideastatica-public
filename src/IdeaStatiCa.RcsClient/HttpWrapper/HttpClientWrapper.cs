using System;
using System.IO;
using System.Net.Http;
using System.Text;
using IdeaStatiCa.Plugin;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR.Client;
using PluginConstants = IdeaStatiCa.Plugin.Constants;
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
		public Action<string, int> ProgressLogAction { get; set; } = null;
		public Action<string> HeartBeatLogAction { get; set; } = null;

		public HttpClientWrapper(IPluginLogger logger, string baseAddress)
		{
			baseUrl = baseAddress.ToString();
			this.logger = logger;
		}

		/// <summary>
		/// Calls GetAsync on HttpClient
		/// </summary>
		/// <typeparam name="TResult">Expected response object</typeparam>
		/// <param name="requestUri">Request endpoint</param>
		/// <param name="acceptHeader">Optional accept header</param>
		/// <returns>Deserialized object from Http response</returns>
		public async Task<TResult> GetAsync<TResult>(string requestUri, CancellationToken token, string acceptHeader = "application/json")
		{
			var url = baseUrl + "/" + requestUri;
			logger.LogInformation($"Calling {nameof(GetAsync)} method {url} with acceptHeader {acceptHeader}");
			return await ExecuteClientCallAsync<TResult>(async (client) => { return await client.GetAsync(url, token); }
			, acceptHeader);
		}


		public async Task<TResult> PutAsync<TResult>(string requestUri, object requestData, CancellationToken token, string acceptHeader = "application/json")
		{
			var result = await ExecuteClientCallAsync<TResult>(async (client) =>
			{
				using (var content = new StringContent(JsonConvert.SerializeObject(requestData), encoding: Encoding.UTF8, "application/json"))
				{
					content.Headers.ContentType.CharSet = "";
					var url = baseUrl + "/" + requestUri;
					return await client.PutAsync(url, content, token);
				}
			}, acceptHeader);
			return result;
		}

		/// <summary>
		/// Calls PostAsync on HttpClient
		/// </summary>
		/// <typeparam name="TResult">Expected response object</typeparam>
		/// <param name="requestUri">Request endpoint</param>
		/// <param name="requestData">Request body object</param>
		/// <param name="acceptHeader">Optional accept header</param>
		/// <returns>Deserialized object from Http response</returns>
		public async Task<TResult> PostAsync<TResult>(string requestUri, object requestData, CancellationToken token, string acceptHeader = "application/json")
		{
			var url = $"{baseUrl}{PluginConstants.RcsProgressEndpoint}";
			logger.LogInformation($"Calling {nameof(PostAsync)} method {url} with acceptHeader {acceptHeader}");
			HubConnection hubConnection = null;

			if (ProgressLogAction != null)
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
					var url = baseUrl + "/" + requestUri;
					try
					{
						return await client.PostAsync(url, content, token);
					}
					catch (OperationCanceledException ex)
					{
						logger.LogDebug("Operation was cancelled", ex);
						throw ex;
					}

				}
			}, acceptHeader);

			if (hubConnection is { })
			{
				logger.LogInformation("Stopping hub connection");
				await hubConnection.StopAsync();
			}

			return result;
		}

		/// <summary>
		/// Post call that contains binary body content
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="requestUri"></param>
		/// <param name="stream"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<TResult> PostAsyncStream<TResult>(string requestUri, StreamContent stream, CancellationToken token)
		{
			return await ExecuteClientCallAsync<TResult>(async (client) =>
			{
				var url = baseUrl + "/" + requestUri;
				return await client.PostAsync(url, stream, token);
			}, "application/json");
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
					using (HttpResponseMessage response = await clientCall(client))
					{

						// Stop the heartbeat checker
						heartbeatChecker.Stop();
						logger.LogDebug($"Stopping HeartbeatChecker");

						if (response is { IsSuccessStatusCode: true })
						{
							logger.LogDebug($"Response is successfull");

							if (acceptHeader.Equals("application/octet-stream", StringComparison.InvariantCultureIgnoreCase))
							{
								logger.LogDebug("HttpClientWrapper.ExecuteClientCallAsync - response is 'application/octet-stream'");
								var ms = new MemoryStream();
								await response.Content.CopyToAsync(ms);
								return (TResult)Convert.ChangeType(ms, typeof(MemoryStream));
							}
							else
							{
								var stringContent = await response.Content.ReadAsStringAsync();
								logger.LogDebug($"HttpClientWrapper.ExecuteClientCallAsync - response is '{typeof(TResult).Name}'");
								return Deserialize<TResult>(acceptHeader, stringContent);
							}
						}
						else
						{
							logger.LogError("Response code was not successfull: " + response.ReasonPhrase);
							throw new HttpRequestException("Response code was not successfull: " + response.StatusCode + ":" + response.ReasonPhrase);
						}
					}
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
