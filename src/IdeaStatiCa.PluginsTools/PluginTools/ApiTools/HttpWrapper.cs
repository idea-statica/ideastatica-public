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
using System.Threading;
using IdeaRS.OpenModel;
using System.Xml;
using IdeaStatiCa.PluginsTools.PluginTools.ApiTools;
using IdeaStatiCa.Plugin.Api.Common;
using System.Collections.Generic;

namespace IdeaStatiCa.PluginsTools.ApiTools.HttpWrapper
{
	/// <summary>
	/// Class to wrap HttpClient for REST calls
	/// </summary>
	public class HttpClientWrapper : IHttpClientWrapper
	{
		private readonly IPluginLogger logger;
		private Uri baseUrl;
		public Action<string, int> ProgressLogAction { get; set; } = null;
		public Action<string> HeartBeatLogAction { get; set; } = null;
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

		public HttpClientWrapper(IPluginLogger logger, string baseAddress)
		{
			baseUrl = new Uri(baseAddress);
			this.logger = logger;
		}

		/// <summary>
		/// Calls GetAsync on HttpClient
		/// </summary>
		/// <typeparam name="TResult">Expected response object</typeparam>
		/// <param name="requestUri">Request endpoint</param>
		/// <param name="acceptHeader">Optional accept header</param>
		/// <param name="useHeartbeatCheck">Optional heartbeat check</param>
		/// <returns>Deserialized object from Http response</returns>
		public async Task<TResult> GetAsync<TResult>(string requestUri, CancellationToken token, string acceptHeader = "application/json", bool useHeartbeatCheck = false)
		{
			var url = new Uri(baseUrl, requestUri);

			logger.LogInformation($"Calling {nameof(GetAsync)} method {url} with acceptHeader {acceptHeader}");
			return await ExecuteClientCallAsync<TResult>(async (client) => 
			{		
				return await client.GetAsync(url, token); 
			}
			, acceptHeader, useHeartbeatCheck);
		}

		public async Task<TResult> PutAsync<TResult>(string requestUri, object requestData, CancellationToken token, string acceptHeader = "application/json", bool useHeartbeatCheck = false)
		{
			var result = await ExecuteClientCallAsync<TResult>(async (client) =>
			{
				var json = JsonConvert.SerializeObject(requestData);
				using (var content = new StringContent(json, encoding: Encoding.UTF8, "application/json"))
				{
					content.Headers.ContentType.CharSet = "";
					var url = new Uri(baseUrl, requestUri);
					return await client.PutAsync(url, content, token);
				}
			}, acceptHeader, useHeartbeatCheck);
			return result;
		}

		/// <summary>
		/// Calls PostAsync on HttpClient
		/// </summary>
		/// <typeparam name="TResult">Expected response object</typeparam>
		/// <param name="requestUri">Request endpoint</param>
		/// <param name="requestData">Request body object</param>
		/// <param name="acceptHeader">Optional accept header</param>
		/// <param name="useHeartbeatCheck">Optional heartbeat check</param>
		/// <returns>Deserialized object from Http response</returns>
		public async Task<TResult> PostAsync<TResult>(string requestUri, object requestData, CancellationToken token, string acceptHeader = "application/json", bool useHeartbeatCheck = false)
		{
			HubConnection hubConnection = null;

			if (ProgressLogAction != null)
			{
				var hubUrl = new Uri(baseUrl, PluginConstants.RcsProgressEndpoint);

				hubConnection = new HubConnectionBuilder()
					.WithUrl(hubUrl)
					.Build();

				hubConnection.On<string, int>(PluginConstants.RcsProgressMethod, (msg, num) => ProgressLogAction(msg, num));

				logger.LogInformation($"Starting hub connection on {hubUrl} address");
				await hubConnection.StartAsync();
			}
			

			var result = await ExecuteClientCallAsync<TResult>(async (client) =>
			{
				using (var content = GetStringContent(requestData))
				{
					var url = new Uri(baseUrl, requestUri);
					try
					{
						logger.LogInformation($"Calling {nameof(PostAsync)} method {url} with acceptHeader {acceptHeader}");
						return await client.PostAsync(url, content, token);
					}
					catch (OperationCanceledException ex)
					{
						logger.LogDebug("Operation was cancelled", ex);
						throw;
					}

				}
			}, acceptHeader, useHeartbeatCheck);

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
		/// <param name="useHeartbeatCheck">Optional heartbeat check</param>
		/// <returns></returns>
		public async Task<TResult> PostAsyncStream<TResult>(string requestUri, StreamContent stream, CancellationToken token, bool useHeartbeatCheck)
		{
			return await ExecuteClientCallAsync<TResult>(async (client) =>
			{
				var url = new Uri(baseUrl, requestUri);
				return await client.PostAsync(url, stream, token);
			}, "application/json", useHeartbeatCheck);
		}

		private async Task<TResult> ExecuteClientCallAsync<TResult>(Func<HttpClient, Task<HttpResponseMessage>> clientCall, string acceptHeader, bool useHeartbeatCheck)
		{
			HeartbeatChecker heartbeatChecker = null;
			try
			{
				using (var client = new HttpClient() { Timeout = Timeout.InfiniteTimeSpan })
				{
					try
					{
						if (useHeartbeatCheck)
						{
							// do not start heartbeat checker for this call
							var hearBeatUrl = new Uri(baseUrl, PluginConstants.RcsApiHeartbeat);
							heartbeatChecker = new HeartbeatChecker(logger, client, hearBeatUrl.AbsoluteUri);
							heartbeatChecker.HeartBeatLogAction = HeartBeatLogAction;
							// Periodically check the heartbeat while the long operation is in progress
							var heartbeatTask = heartbeatChecker.StartAsync();
							logger.LogTrace($"Starting HeartbeatChecker on url {hearBeatUrl.AbsoluteUri}");
						}

						foreach (KeyValuePair<string, string> header in Headers)
						{
							client.DefaultRequestHeaders.Add(header.Key, header.Value);
						}

						using (HttpResponseMessage response = await clientCall(client))
						{
							// Stop the heartbeat checker
							logger.LogTrace($"Stopping HeartbeatChecker");

							if (response is { IsSuccessStatusCode: true })
							{
								logger.LogTrace($"Response is successfull");

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
					finally
					{
						if(heartbeatChecker != null)
						{
							heartbeatChecker.Stop();
							heartbeatChecker = null;
						}
					}
				}
			}
			catch (HttpRequestException ex)
			{
				logger.LogError("API Request has failed.", ex);
				throw new Exception("API Request has failed", ex);
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

		private StringContent GetStringContent(object requestContent)
		{
			if (requestContent is OpenModel openModel)
			{
				XmlSerializer serializer = new XmlSerializer(typeof(OpenModel));
				StringWriter stringWriter = new StringWriter();
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
				{
					serializer.Serialize(xmlWriter, openModel);
				}

				string serializedXml = stringWriter.ToString().Replace("utf-16", "utf-8");
				return new StringContent(serializedXml, encoding: Encoding.UTF8, "application/xml");
			}

			var content = new StringContent(JsonConvert.SerializeObject(requestContent), encoding: Encoding.UTF8, "application/json");
			content.Headers.ContentType.CharSet = "";
			return content;
		}

		public void AddRequestHeader(string header, string value)
		{
			Headers.Add(header, value);
		}
	}
}
