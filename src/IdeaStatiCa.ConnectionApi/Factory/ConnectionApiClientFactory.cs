using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Common;
using IdeaStatiCa.Plugin.Api.ConnectionRest;
using IdeaStatiCa.Plugin.Utilities;
using IdeaStatiCa.PluginsTools.ApiTools.HttpWrapper;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi.Factory
{
	public class ConnectionApiClientFactory : IConnectionApiClientFactory
	{
		private const string LOCALHOST_URL = "http://localhost";
		private readonly IPluginLogger _pluginLogger;
		private IHttpClientWrapper _httpClientWrapper;
		private int port = -1;
		private Process _rcsRestApiProcess = null;
		private readonly string _setupDir = null;


		public Action<string, int> StreamingLog { get; set; }
		public Action<string> HeartbeatLog { get; set; }

		public ConnectionApiClientFactory(IPluginLogger pluginLogger, 
			IHttpClientWrapper httpClientWrapper, 
			string setupDir)
		{
			_pluginLogger = pluginLogger;
			_httpClientWrapper = httpClientWrapper;
			_setupDir = GetConnectionRestApiPath(setupDir);
		}

		private async Task<(string, int)> RunConnectionRestApiService()
		{
			return await Task.Run<(string, int)>(async () =>
			{
				if (_rcsRestApiProcess is null)
				{

					port = PortFinder.FindPort(Constants.MinGrpcPort, Constants.MaxGrpcPort, _pluginLogger);

					while (port > 0)
					{
						var directoryName = !string.IsNullOrEmpty(_setupDir) ? _setupDir : Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
						string apiExecutablePath = Path.Combine(directoryName, "IdeaStatiCa.ConnectionRestApi.exe");

						if (!File.Exists(apiExecutablePath))
						{
							_pluginLogger.LogWarning($"RcsClientFactory.RunRcsRestApiService : '{apiExecutablePath}' doesn't exist");
						}

						_pluginLogger.LogDebug($"Running {apiExecutablePath} on port {port}");
						// Start the REST API executable with the chosen port
						string arguments = $"-port={port}";
						_rcsRestApiProcess = new Process();
						_rcsRestApiProcess.StartInfo.FileName = apiExecutablePath;
						//_rcsRestApiProcess.StartInfo.Arguments = arguments;
						_rcsRestApiProcess.StartInfo.UseShellExecute = false;
#if !DEBUG
						//_rcsRestApiProcess.StartInfo.CreateNoWindow = true;
#endif
						_rcsRestApiProcess.Start();

						// Wait for the API to start (you might need a more robust way to determine this)
						await Task.Delay(5000);

						// Check if the API process is still running

						if (!_rcsRestApiProcess.HasExited)
						{
							_rcsRestApiProcess.CloseMainWindow();
							_pluginLogger.LogInformation($"REST API process started on port {port}.");
							break;
						}
					}

					if (port <= 0)
					{
						_pluginLogger.LogError("Failed to start the REST API on an available port.");
					}
				}

				_pluginLogger.LogDebug($"Created process with Id {_rcsRestApiProcess?.Id}");
				return ($"{LOCALHOST_URL}:{port}", _rcsRestApiProcess.Id);
			});
		}

		public void Dispose()
		{
			_rcsRestApiProcess?.Kill();
			_rcsRestApiProcess?.Dispose();
			_rcsRestApiProcess = null;
		}

		private string GetConnectionRestApiPath(string directory)
		{
			if (string.IsNullOrEmpty(directory))
			{
				// setup dir is not passed
				_pluginLogger.LogDebug("GetConnectionRestApiPath : setup dir is not passed");

				return directory;
			}

			// Connection Rest Api is expected in net6.0-windows subdir.
			string modifiedDir = directory;
			if (!directory.ToLower().Contains("net6.0-windows"))
			{
				_pluginLogger.LogDebug("GetConnectionRestApiPath trying to find subdir 'net6.0-windows'");
				var dir = Path.Combine(directory, "net6.0-windows");
				if (Directory.Exists(dir))
				{
					_pluginLogger.LogDebug($"GetConnectionRestApiPath : '{dir}' exists ");
					modifiedDir = dir;
				}
				else
				{
					_pluginLogger.LogWarning($"GetConnectionRestApiPath : '{dir}' doesn't exist");
				}
			}

			_pluginLogger.LogWarning($"GetRcsRestApiPath : returning '{modifiedDir}'");
			return modifiedDir;
		}

		public async Task<IConnectionApiController> CreateConnectionApiClient()
		{
			var (url, processId) = await RunConnectionRestApiService();
			_pluginLogger?.LogInformation($"Service is running on {url} with process ID {processId}");

			var wrapper = _httpClientWrapper ?? new HttpClientWrapper(_pluginLogger, url);
			if (StreamingLog != null)
			{
				wrapper.ProgressLogAction = StreamingLog;
			}
			if (HeartbeatLog != null)
			{
				wrapper.HeartBeatLogAction = HeartbeatLog;
			}
			return new ConnectionApiController(processId, wrapper, _pluginLogger);
		}
	}
}
