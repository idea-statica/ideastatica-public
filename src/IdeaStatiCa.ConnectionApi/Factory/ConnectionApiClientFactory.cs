using IdeaStatiCa.ConnectionApi.Client;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Common;
using IdeaStatiCa.Plugin.Api.ConnectionRest;
using IdeaStatiCa.Plugin.Utilities;
using IdeaStatiCa.PluginsTools.ApiTools.HttpWrapper;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi.Factory
{
	public class ConnectionApiClientFactory : IConnectionApiClientFactory
	{
		private const string LOCALHOST_URL = "http://localhost";
		private readonly IPluginLogger _pluginLogger;
		private IHttpClientWrapper? _httpClientWrapper;
		private int port = -1;
		private Process? _restApiProcess = null;
		private readonly string? _setupDir = null;


		public Action<string, int>? StreamingLog { get; set; }
		public Action<string>? HeartbeatLog { get; set; }

		public ConnectionApiClientFactory(string? setupDir, IPluginLogger? pluginLogger = null,
			IHttpClientWrapper? httpClientWrapper = null)
		{
			_pluginLogger = pluginLogger ?? new NullLogger();
			_httpClientWrapper = httpClientWrapper;
			_setupDir = GetConnectionRestApiPath(setupDir);
		}

		private async Task<(string, int)> RunConnectionRestApiService()
		{
			if (_restApiProcess != null)
			{
				throw new Exception("Connection REST API process is already running.");
			}

			return await Task.Run<(string, int)>(async () =>
			{
				port = PortFinder.FindPort(Constants.MinGrpcPort, Constants.MaxGrpcPort, _pluginLogger);

				while (port > 0)
				{
					var directoryName = !string.IsNullOrEmpty(_setupDir) ? _setupDir : Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
					string apiExecutablePath = Path.Combine(directoryName, "IdeaStatiCa.ConnectionRestApi.exe");

					if (!File.Exists(apiExecutablePath))
					{
						_pluginLogger.LogWarning($"ConnectionApiController.RunConnectionRestApiService : '{apiExecutablePath}' doesn't exist");
					}

					_pluginLogger.LogDebug($"Running {apiExecutablePath} on port {port}");
					// Start the REST API executable with the chosen port
					string arguments = $"-port={port}";
					_restApiProcess = new Process();
					_restApiProcess.StartInfo.FileName = apiExecutablePath;
					_restApiProcess.StartInfo.Arguments = arguments;
					_restApiProcess.StartInfo.UseShellExecute = false;
#if !DEBUG
						_restApiProcess.StartInfo.CreateNoWindow = true;
#endif
					_restApiProcess.Start();


					// Wait for the API to start (you might need a more robust way to determine this)
					await Task.Delay(5000);

					// Check if the API process is still running

					if (!_restApiProcess.HasExited)
					{
						_restApiProcess.CloseMainWindow();
						_pluginLogger.LogInformation($"REST API process started on port {port}.");
						break;
					}
				}

				if (port <= 0)
				{
					_pluginLogger.LogError("Failed to start the REST API on an available port.");
				}

				_pluginLogger.LogDebug($"Created process with Id {_restApiProcess?.Id}");
				return ($"{LOCALHOST_URL}:{port}", _restApiProcess == null ? -1 : _restApiProcess.Id);
			});
		}

		public void Dispose()
		{
			_restApiProcess?.Kill();
			_restApiProcess?.Dispose();
			_restApiProcess = null;
		}

		private string? GetConnectionRestApiPath(string? directory)
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

			_pluginLogger.LogWarning($"GetConnectionRestApiPath : returning '{modifiedDir}'");
			return modifiedDir;
		}

		/// <inheritdoc cref="IConnectionApiClientFactory.CreateConnectionApiClient()"/>
		public async Task<IConnectionApiController> CreateConnectionApiClient()
		{
			var (url, processId) = await RunConnectionRestApiService();
			_pluginLogger.LogInformation($"Service is running on {url} with process ID {processId}");

			var wrapper = _httpClientWrapper ?? new HttpClientWrapper(_pluginLogger, url);

			if (StreamingLog != null)
			{
				wrapper.ProgressLogAction = StreamingLog;
			}
			if (HeartbeatLog != null)
			{
				wrapper.HeartBeatLogAction = HeartbeatLog;
			}
			var client = new ConnectionApiController(processId, wrapper, _pluginLogger);
			await client.InitializeClientIdAsync(CancellationToken.None);
			return client;
		}

		/// <inheritdoc cref="IConnectionApiClientFactory.CreateConnectionApiClient(Uri)"/>
		public async Task<IConnectionApiController> CreateConnectionApiClient(Uri uri)
		{
			var wrapper = _httpClientWrapper ?? new HttpClientWrapper(_pluginLogger, uri.AbsoluteUri);

			if (StreamingLog != null)
			{
				wrapper.ProgressLogAction = StreamingLog;
			}
			if (HeartbeatLog != null)
			{
				wrapper.HeartBeatLogAction = HeartbeatLog;
			}
			var client = new ConnectionApiController(-1, wrapper, _pluginLogger);
			await client.InitializeClientIdAsync(CancellationToken.None);
			return client;
		}
	}
}
