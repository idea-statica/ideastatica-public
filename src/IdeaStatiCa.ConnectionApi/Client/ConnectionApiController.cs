using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Common;
using IdeaStatiCa.Plugin.Api.ConnectionRest;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
		public static readonly string ConnectionController = "ConConnection";
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

				var clientIdResponse = await _httpClient.GetAsync<string>($"api/{ApiVersion}/{ConProjectController}/ConnectClient", cancellationToken, "text/plain");
				ClientId = Guid.Parse(clientIdResponse);
				_httpClient.AddRequestHeader("ClientId", ClientId.ToString());
			}
		}

		/// <inheritdoc cref="OpenProjectAsync(string, CancellationToken)" />
		public async Task<ConProject> OpenProjectAsync(string ideaConProject, CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.OpenProject clientId = {ClientId} path = '{ideaConProject}'");

			byte[] fileData = File.ReadAllBytes(ideaConProject);
			var streamContent = new StreamContent(new MemoryStream(fileData));
			streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

			var response = await _httpClient.PostAsyncStream<ConProject>($"api/{ApiVersion}/{ConProjectController}/Project", streamContent, cancellationToken);
			activeProjectId = response.ProjectId;
			_pluginLogger.LogDebug($"ConnectionApiController.OpenProject projectId = {response.ProjectId}");

			return response;
		}

		public async Task<Stream> DownloadProjectAsync(CancellationToken token = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.DownloadProjectAsync projectId = {activeProjectId}");
			var result = await _httpClient.GetAsync<MemoryStream>($"api/{ApiVersion}/{ConProjectController}/{activeProjectId}/Download", token, "application/octet-stream");
			return result;
		}

		public async Task CloseProjectAsync(CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.CloseProjectAsync clientId = {ClientId} projectId = {activeProjectId}");
			try
			{
				var result = await _httpClient.GetAsync<string>($"api/{ApiVersion}/{ConProjectController}/{activeProjectId}/Close", cancellationToken, "text/plain");
			}
			finally
			{
				activeProjectId = Guid.Empty;
			}
		}

		public async Task<ConProjectData> GetProjectDataAsync(CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.GetProjectDataAsync clientId = {ClientId} projectId = {activeProjectId}");
			var response = await _httpClient.GetAsync<ConProjectData>($"api/{ApiVersion}/{ConProjectController}/{activeProjectId}/ProjectData", cancellationToken);
			return response;
		}

		public async Task<List<ConConnection>> GetConnectionsAsync(CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.GetConnectionsAsync clientId = {ClientId} projectId = {activeProjectId}");
			var response = await _httpClient.GetAsync<List<ConConnection>>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/Connection", cancellationToken);
			return response;
		}

		public async Task<ConConnection> GetConnectionAsync(int connectionId, CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.GetConnectionAsync clientId = {ClientId} projectId = {activeProjectId} connectionId = {connectionId}");
			var response = await _httpClient.GetAsync<List<ConConnection>>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/Connection?connectionId={connectionId}", cancellationToken);
			return response.FirstOrDefault();
		}

		public async Task<ConConnection> UpdateConnectionAsync(int connectionId, ConConnection connectionUpdate, CancellationToken cancellationToken = default)
		{
			_pluginLogger.LogDebug($"ConnectionApiController.UpdateConnectionAsync clientId = {ClientId} projectId = {activeProjectId} connectionId = {connectionId}");
			var result = await _httpClient.PutAsync<ConConnection>($"api/{ApiVersion}/{ConnectionController}/{activeProjectId}/{connectionId}/Connection", connectionUpdate, cancellationToken);
			return result;
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
