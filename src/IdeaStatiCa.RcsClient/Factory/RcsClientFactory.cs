using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.RCS;
using IdeaStatiCa.Plugin.Utilities;
using IdeaStatiCa.RcsClient.Client;
using IdeaStatiCa.RcsClient.HttpWrapper;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.RcsClient.Factory
{
	public class RcsClientFactory : IRcsClientFactory
	{
		private const string LOCALHOST_URL = "http://localhost";
		private readonly IPluginLogger pluginLogger;
		private IHttpClientWrapper httpClientWrapper;
		private int port = -1;
		private Process rcsRestApiProcess = null;
		private string directory = null;

		public Action<string, int> StreamingLog { get; set; } = null;
		public Action<string> HeartbeatLog { get; set; } = null;

		public RcsClientFactory(string path, IPluginLogger pluginLogger = null, IHttpClientWrapper httpClientWrapper = null)
		{
			this.httpClientWrapper = httpClientWrapper;
			this.pluginLogger = pluginLogger ?? new NullLogger();
			this.directory = path;
		}

		/// <summary>
		/// Create instance of IRcsApiController that is connected to locally hosted Rest API
		/// URL = Url for the REST API service
		/// ProcessId = For disposing the API process once the instance is disposed
		/// </summary>
		/// <returns>Instance of RcsApiController</returns>
		public async Task<IRcsApiController> CreateRcsApiClient()
		{
			var (url, processId) = await RunRcsRestApiService();
			var wrapper = httpClientWrapper ?? new HttpClientWrapper(pluginLogger, url);
			if(StreamingLog != null)
			{
				wrapper.ProgressLogAction = StreamingLog;
			}
			if(HeartbeatLog != null)
			{
				wrapper.HeartBeatLogAction = HeartbeatLog;
			}
			return new RcsApiClient(processId, pluginLogger, wrapper);
		}

		private async Task<(string, int)> RunRcsRestApiService()
		{
			return await Task.Run<(string, int)>(() =>
			{
				if (rcsRestApiProcess is null)
				{
					
					port = PortFinder.FindPort(Constants.MinGrpcPort, Constants.MaxGrpcPort, pluginLogger);

					while (port > 0)
					{
						var directoryName = directory ?? Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
						string apiExecutablePath = Path.Combine(directoryName, "IdeaStatiCa.RcsRestApi.exe");

						pluginLogger.LogDebug($"Running {apiExecutablePath} on port {port}");
						// Start the REST API executable with the chosen port
						string arguments = $"-port={port}";
						rcsRestApiProcess = new Process();
						rcsRestApiProcess.StartInfo.FileName = apiExecutablePath;
						rcsRestApiProcess.StartInfo.Arguments = arguments;
						rcsRestApiProcess.StartInfo.UseShellExecute = false;
#if !DEBUG
						rcsRestApiProcess.StartInfo.CreateNoWindow = true;
#endif
						rcsRestApiProcess.Start();

						// Wait for the API to start (you might need a more robust way to determine this)
						Thread.Sleep(5000);

						// Check if the API process is still running
						if (!rcsRestApiProcess.HasExited)
						{
							rcsRestApiProcess.CloseMainWindow();
							pluginLogger.LogInformation($"REST API process started on port {port}.");
							break;
						}
					}

					if (port <= 0)
					{
						pluginLogger.LogError("Failed to start the REST API on an available port.");
					}
				}

				pluginLogger.LogDebug($"Created process with Id {rcsRestApiProcess.Id}");
				return ($"{LOCALHOST_URL}:{port}", rcsRestApiProcess.Id);
			});
		}

		public void Dispose()
		{
			rcsRestApiProcess?.Kill();
			rcsRestApiProcess?.Dispose();
		}
	}
}
