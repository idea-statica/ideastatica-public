using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Common;
using IdeaStatiCa.Plugin.Api.ConnectionRest;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi.Client
{
	public class ConnectionApiController : IConnectionApiController
	{
		public static readonly string ApiVersion = "1";

		private bool disposedValue = false;
		private readonly int restApiProcessId;
		private Guid activeProjectId;
		private Guid ClientId;

		private readonly IHttpClientWrapper _httpClient;
		private readonly IPluginLogger _pluginLogger;

		public static readonly string ConProjectController = "ConProject";
		public static readonly string ConnectionController = "Connection";
		public static readonly string ConParameterController = "ConParameter";

		public ConnectionApiController(int restApiProcessId, IHttpClientWrapper httpClient, IPluginLogger pluginLogger = null)
		{
			this.restApiProcessId = restApiProcessId;
			_httpClient = httpClient;
			_pluginLogger = pluginLogger ?? new NullLogger();
		}

		public async Task InitializeClientIdAsync(CancellationToken cancellationToken)
		{
			if (ClientId == Guid.Empty)
			{
				_pluginLogger.LogDebug("ConnectionApiController.InitializeClientIdAsync");

				var clientIdResponse = await _httpClient.GetAsync<ConApiClientId>($"api/{ApiVersion}/{ConProjectController}/ConnectClient", cancellationToken);
				ClientId = clientIdResponse.ClientId;
				_httpClient.AddRequestHeader("ClientId", ClientId.ToString());
			}
		}

		/// <inheritdoc cref="OpenProjectAsync(string, CancellationToken)" />
		public async Task<ConProject> OpenProjectAsync(string ideaConProject, CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.OpenProject path = '{ideaConProject}'");

			byte[] fileData = File.ReadAllBytes(ideaConProject);
			var streamContent = new StreamContent(new MemoryStream(fileData));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

			var response = await _httpClient.PostAsyncStream<ConProject>($"api/{ApiVersion}/{ConProjectController}/OpenProject", streamContent, cancellationToken);
			activeProjectId = response.ProjectId;
			_pluginLogger.LogDebug($"ConnectionApiController.OpenProject projectId = {response.ProjectId}");

			return response;
		}

		public async Task CloseProjectAsync(CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.CloseProjectAsync");
			try
			{
				var result = await _httpClient.GetAsync<string>($"api/{ApiVersion}/{ConProjectController}/{activeProjectId}/CloseProject", cancellationToken, "text/plain");
			}
			finally
			{
				activeProjectId = Guid.Empty;
			}
		}

		public async Task<ConProjectData> GetProjectDataAsync(CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug("ConnectionApiController.GetProjectDataAsync");
			var response = await _httpClient.GetAsync<ConProjectData>($"api/{ApiVersion}/{ConProjectController}/{activeProjectId}/GetProjectData", cancellationToken);
			return response;
		}

		public async Task<List<ConConnection>> GetConnectionsAsync(CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug("ConnectionApiController.GetConnectionsAsync");
			var response = await _httpClient.GetAsync<List<ConConnection>>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/GetConnections", cancellationToken);
			return response;
		}

		public async Task<ConConnection> GetConnectionAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.GetConnectionAsync connectionId = {connectionId}");
			var response = await _httpClient.GetAsync<ConConnection>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/{connectionId}/GetConnection", cancellationToken);
			return response;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (restApiProcessId != -1)
					{
						var restApiProcess = Process.GetProcessById(restApiProcessId);
						if (restApiProcess is { })
						{
							if (!restApiProcess.HasExited)
							{
								
								_pluginLogger.LogInformation($"Cleaning the API process with ID {restApiProcessId}");

								// TODO - I suppose Kill process does't release resources properly (temp files on a disk)
								restApiProcess.Kill();
							}
						}
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
