using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;
using IdeaStatiCa.RcsClient.Client;
using IdeaStatiCa.RcsClient.HttpWrapper;

namespace IdeaStatiCa.RcsClient.Factory
{
	public class RcsClientFactory : IRcsClientFactory
	{
		private const string LOCALHOST_URL = "https://localhost";
		private readonly IPluginLogger _pluginLogger;
		private IHttpClientWrapper _httpClientWrapper;
		private int _port = -1;
		private Process _rcsRestApiProcess = null;
		private string _directory = null;

		public Action<string, int> StreamingLog { get; set; } = null;
		public Action<string> HeartbeatLog { get; set; } = null;

		public RcsClientFactory(IPluginLogger pluginLogger, IHttpClientWrapper httpClientWrapper = null, string directory = null)
		{
			_httpClientWrapper = httpClientWrapper;
			_pluginLogger = pluginLogger;
			_directory = directory;
		}

		/// <summary>
		/// Create instance of IRcsApiController that is connected to locally hosted Rest API
		/// URL = Url for the REST API service
		/// Controller = Name of the controller
		/// ProcessId = For disposing the API process once the instance is disposed
		/// </summary>
		/// <returns>Instance of RcsApiController</returns>
		public IRcsApiController CreateRcsApiClient()
		{
			var (url, controller, processId) = RunRcsRestApiService();
			var wrapper = _httpClientWrapper ?? new HttpClientWrapper(_pluginLogger, url, controller);
			if(StreamingLog != null)
			{
				wrapper.ProgressLogAction = StreamingLog;
			}
			if(HeartbeatLog != null)
			{
				wrapper.HeartBeatLogAction = HeartbeatLog;
			}
			return new RcsApiClient(processId, _pluginLogger, wrapper);
		}

		private (string, string, int) RunRcsRestApiService()
		{
			if (_rcsRestApiProcess is null)
			{
				_port = FindAvailablePort(5000, 5100);

				while (_port > 0)
				{
					var directoryName = _directory ?? Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
					string apiExecutablePath = Path.Combine(directoryName, "IdeaRcsRestApi.exe");

					_pluginLogger.LogDebug($"Running {apiExecutablePath} on port {_port}");
					// Start the REST API executable with the chosen port
					string arguments = $"-port={_port}";
					_rcsRestApiProcess = new Process();
					_rcsRestApiProcess.StartInfo.FileName = apiExecutablePath;
					_rcsRestApiProcess.StartInfo.Arguments = arguments;
					_rcsRestApiProcess.StartInfo.UseShellExecute = false;
					_rcsRestApiProcess.StartInfo.RedirectStandardOutput = true;
					_rcsRestApiProcess.StartInfo.CreateNoWindow = true;
					_rcsRestApiProcess.Start();

					// Wait for the API to start (you might need a more robust way to determine this)
					Thread.Sleep(5000);

					// Check if the API process is still running
					if (!_rcsRestApiProcess.HasExited)
					{
						_rcsRestApiProcess.CloseMainWindow();
						_pluginLogger.LogInformation($"REST API process started on port {_port}.");
						break;
					}
				}

				if (_port <= 0)
				{
					_pluginLogger.LogError("Failed to start the REST API on an available port.");
				}
			}

			_pluginLogger.LogDebug($"Created process with Id {_rcsRestApiProcess.Id}");
			return ($"{LOCALHOST_URL}:{_port}", "RcsRest", _rcsRestApiProcess.Id);
		}

		// Function to find an available port within a range
		static int FindAvailablePort(int startPort, int endPort)
		{
			for (int port = startPort; port <= endPort; port++)
			{
				if (IsPortAvailable(port))
				{
					return port;
				}
			}
			return -1; // No available port found
		}

		// Function to check if a port is available
		static bool IsPortAvailable(int port)
		{
			using (var client = new TcpClient())
			{
				try
				{
					client.Connect(IPAddress.Loopback, port);
					return false;
				}
				catch (SocketException)
				{
					return true;
				}
			}
		}

		public void Dispose()
		{
			_rcsRestApiProcess?.Dispose();
		}
	}
}
