using IdeaStatiCa.Api.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.RcsApi
{
	/// <summary>
	/// Factory for creating instances of RCS API client that are connected to the automatically started REST API service
	/// </summary>
	public class RcsApiServiceRunner : IApiServiceFactory<IRcsApiClient>, IDisposable
	{
		private const string LOCALHOST_URL = "http://127.0.0.1";
		private const string API_EXECUTABLE_NAME = "IdeaStatiCa.RcsRestApi.exe";
		private Process serviceProcess;
		private string launchPath;
		private int port = -1;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setupDir"> where .exe file is located</param>
		public RcsApiServiceRunner(string setupDir)
		{
			launchPath = setupDir;
		}

		/// <inheritdoc cref="IApiServiceFactory.CreateConnectionApiClient"/>
		public async Task<IRcsApiClient> CreateApiClient()
		{
			var url = await StartService();
			var client = new RcsApiClient(url);
			await client.CreateAsync();
			return client;
		}

		private async Task<string> StartService()
		{
			return await Task.Run<string>(async () =>
			{
				if (serviceProcess is null)
				{
					string setupDir = string.Empty;
					port = GetAvailablePort();

					while (port > 0)
					{
						var directoryName = !string.IsNullOrEmpty(launchPath) ? launchPath : Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
						string apiExecutablePath = Path.Combine(directoryName, API_EXECUTABLE_NAME);

						if (!File.Exists(apiExecutablePath))
						{
							throw new FileNotFoundException($"API executable not found at path: {apiExecutablePath}");
						}

						// Start the REST API executable with the chosen port
						string arguments = $"-port={port}";
						serviceProcess = new Process();
						serviceProcess.StartInfo.FileName = apiExecutablePath;
						serviceProcess.StartInfo.Arguments = arguments;
						serviceProcess.StartInfo.UseShellExecute = true;

						serviceProcess.Start();

						// Wait for the API to start (you might need a more robust way to determine this)
						var apiUrlBase = new Uri($"{LOCALHOST_URL}:{port}");
						var apiUrlHeartbeat = new Uri(apiUrlBase, IdeaStatiCa.Api.Common.RestApiConstants.RestApiHeartbeat);
						var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
						var isApiReady = await WaitForApiToBeReady(apiUrlHeartbeat, cts.Token);

						// Check if the API process is still running

						if (isApiReady && !serviceProcess.HasExited)
						{
							serviceProcess.CloseMainWindow();
							break;
						}
					}

					if (port <= 0)
					{
						throw new InvalidOperationException("No available port found.");
					}
				}

				return $"{LOCALHOST_URL}:{port}";
			});
		}

		private int GetAvailablePort()
		{
			TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
			listener.Start();
			int port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();
			return port;
		}

		public void Dispose()
		{
			if (serviceProcess != null)
			{
				serviceProcess.Kill();
				serviceProcess.Dispose();
				serviceProcess = null;
			}
		}

		private async Task<bool> WaitForApiToBeReady(Uri apiUrl, CancellationToken cts)
		{
			using (var httpClient = new HttpClient())
			{
				while (!cts.IsCancellationRequested)
				{
					try
					{
						var response = await httpClient.GetAsync(apiUrl, HttpCompletionOption.ResponseHeadersRead, cts);
						if (response.IsSuccessStatusCode)
						{
							return true;
						}
					}
					catch (HttpRequestException ex)
					{
						// API is not ready yet. Wait and try it again
						await Task.Delay(3000, cts);
					}
					catch (TaskCanceledException)
					{
						// Timeout occur during http call waiting for the REST API to be ready
						return false;
					}
				}
				// Timeout waiting for the REST API to be ready
				return false;
			}
		}
	}
}
