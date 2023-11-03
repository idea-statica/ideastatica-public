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
	public class HttpClientWrapper : IHttpClientWrapper
	{
		private readonly IPluginLogger _logger;
		private string _url;
		private string _controllerName;
		public Action<string, int> ProgressLogAction { get; set; } = null;
		public Action<string> HeartBeatLogAction { get; set; } = null;

		public HttpClientWrapper(IPluginLogger logger, string baseAddress, string controllerName)
		{
			_url = baseAddress.ToString();
			_logger = logger;
			_controllerName = controllerName;
		}

		public async Task<TResult> GetAsync<TResult>(string requestUri, string acceptHeader = "application/json")
		{
			var url = _url + "/" + _controllerName + "/" + requestUri;
			_logger.LogInformation($"Calling {nameof(GetAsync)} method {url} with acceptHeader {acceptHeader}");
			return await ExecuteClientCallAsync<TResult>(async (client) => { return await client.GetAsync(url); }
			, acceptHeader);
		}   

		public async Task<TResult> PostAsync<TResult>(string requestUri, object requestData, string acceptHeader = "application/json")
		{
			var url = $"{_url}{PluginConstants.RcsProgressEndpoint}";
			_logger.LogInformation($"Calling {nameof(PostAsync)} method {url} with acceptHeader {acceptHeader}");
			HubConnection hubConnection = null;
			
			if(ProgressLogAction != null)
			{
				hubConnection = new HubConnectionBuilder()
					.WithUrl(url)
					.Build();

				hubConnection.On<string, int>(PluginConstants.RcsProgressMethod, (msg, num) => ProgressLogAction(msg, num));

				_logger.LogInformation($"Starting hub connection on {url} address");
				await hubConnection.StartAsync();
			}
			
			var result = await ExecuteClientCallAsync<TResult>(async (client) =>
			{
				using (var content = new StringContent(JsonConvert.SerializeObject(requestData), encoding: Encoding.UTF8, "application/json"))
				{
					content.Headers.ContentType.CharSet = "";
					var url = _url + "/" +_controllerName + "/" + requestUri;
					return await client.PostAsync(url, content);
				}
			}, acceptHeader);

			if(hubConnection is { })
			{
				_logger.LogInformation("Stopping hub connection");
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
					heartbeatChecker = new HeartbeatChecker(client, _url + PluginConstants.RcsApiHeartbeat);
					heartbeatChecker.HeartBeatLogAction = HeartBeatLogAction;
					// Periodically check the heartbeat while the long operation is in progress
					var heartbeatTask = heartbeatChecker.StartAsync();
					_logger.LogDebug($"Starting HeartbeatChecker on url {_url + PluginConstants.RcsApiHeartbeat}");
					HttpResponseMessage response = await clientCall(client);
					
					if (response is { IsSuccessStatusCode: true })
					{
						_logger.LogDebug($"Response is successfull");
						var stringContent = await response.Content.ReadAsStringAsync();
						// Stop the heartbeat checker
						heartbeatChecker.Stop();
						_logger.LogDebug($"Stopping HeartbeatChecker");
						return Deserialize<TResult>(acceptHeader, stringContent);
					}

					_logger.LogError("Response code was not successfull: " + response.ReasonPhrase);
					throw new HttpRequestException("Response code was not successfull: " + response.StatusCode + ":" + response.ReasonPhrase);
				}
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError("API Request for RCS failed.", ex);
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
