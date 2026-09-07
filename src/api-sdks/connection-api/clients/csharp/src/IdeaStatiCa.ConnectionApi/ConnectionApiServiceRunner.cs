using IdeaStatiCa.Api.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// Factory for creating instances of Connection API client that are connected to the automatically started REST API service
	/// </summary>
	public class ConnectionApiServiceRunner : IApiServiceFactory<IConnectionApiClient>, IDisposable
	{
		private const string LOCALHOST_URL = "http://127.0.0.1";
		private const string API_EXECUTABLE_NAME = "IdeaStatiCa.ConnectionRestApi.exe";
		private Process serviceProcess;
		private string launchPath;
		private int port = -1;
		private readonly Action<string> log;
		private ServiceJobObject job;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setupDir"> where .exe file is located</param>
		public ConnectionApiServiceRunner(string setupDir) : this(setupDir, null)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setupDir">where .exe file is located</param>
		/// <param name="log">
		/// Receives what happens around the service process itself - which executable was started, and
		/// anything that weakens the guarantee that the service is shut down with this process (see
		/// <see cref="IsServiceOwned"/>). Optional; pass null to keep it silent.
		/// </param>
		public ConnectionApiServiceRunner(string setupDir, Action<string> log)
		{
			launchPath = setupDir;
			this.log = log ?? (_ => { });
		}

		/// <summary>
		/// Process id of the service this runner started, or null when it has not started one.
		/// </summary>
		public int? ServiceProcessId
		{
			get
			{
				try
				{
					return serviceProcess != null && !serviceProcess.HasExited ? serviceProcess.Id : (int?)null;
				}
				catch (InvalidOperationException)
				{
					return null;      // the process was never started, or has already been disposed
				}
			}
		}

		/// <summary>
		/// Whether the service is guaranteed to be shut down when THIS process ends, however it ends -
		/// including a hard kill or a crash, which no managed cleanup survives.
		///
		/// It matters because a running service holds an IDEA StatiCa licence seat: a service left behind
		/// costs the user that seat until the process is found and ended. False means only an orderly
		/// <see cref="Dispose"/> shuts it down; the log callback passed to the constructor says why.
		/// </summary>
		public bool IsServiceOwned => job != null && job.IsActive;

		/// <inheritdoc cref="IApiServiceFactory{T}.CreateApiClient"/>
		public async Task<IConnectionApiClient> CreateApiClient()
		{
			var url = await StartService();
			var client = new ConnectionApiClient(url);
			await client.CreateAsync();
			return client;
		}

		private async Task<string> StartService()
		{
			return await Task.Run<string>(async() =>
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
						serviceProcess.StartInfo.UseShellExecute = false;

						if(!serviceProcess.Start())
						{
							throw new InvalidOperationException($"Failed to start the process. {apiExecutablePath}");
						}

						// Own it before waiting for the heartbeat: the service is already running, so from
						// here on a crash of this process could leak it - and a service that outlives its
						// client keeps an IDEA StatiCa licence seat. See ServiceJobObject.
						job = job ?? new ServiceJobObject(log);
						job.Adopt(serviceProcess);
						log($"Connection API service started from '{apiExecutablePath}' on port {port}"
							+ $" (pid {serviceProcess.Id}); it is shut down with this process"
							+ (IsServiceOwned ? ", even if this process is killed." : " only on an orderly exit."));

						// Wait for the API to start (you might need a more robust way to determine this)
						var apiUrlBase = new Uri($"{LOCALHOST_URL}:{port}");
						var apiUrlHeartbeat = new Uri(apiUrlBase, IdeaStatiCa.Api.Common.RestApiConstants.RestApiHeartbeat);
						var cts = new CancellationTokenSource(TimeSpan.FromSeconds(50));
						var isApiReady = await WaitForApiToBeReady(apiUrlHeartbeat, serviceProcess, cts.Token);

						// Check if the API process is still running
						if (isApiReady && !serviceProcess.HasExited)
						{
							serviceProcess.CloseMainWindow();
							break;
						}
						else
						{
							// Service has not started
							throw new InvalidOperationException($"Failed to start the application: '{apiExecutablePath}'.\nThe required .NET framework 'Microsoft.AspNetCore.App' (version 8.0.0) may not be installed or is not configured correctly.\nPlease verify your .NET installation: https://aka.ms/dotnet-core-applaunch?framework=Microsoft.AspNetCore.App&framework_version=8.0.0&arch=x64&rid=win-x64&os=win10");
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
				try
				{
					// A service that has already exited on its own is not an error here, and Kill throws
					// for one - which used to make disposing a runner whose service had crashed throw
					// instead of cleaning up.
					if (!serviceProcess.HasExited)
					{
						serviceProcess.Kill();
					}
				}
				catch (Exception e)
				{
					log($"The Connection API service (pid {ServiceProcessId}) could not be ended"
						+ $" ({e.Message}); the Job Object takes it down with this process.");
				}

				serviceProcess.Dispose();
				serviceProcess = null;
			}

			// Closing the job handle is what terminates anything still in it, so this comes last.
			if (job != null)
			{
				job.Dispose();
				job = null;
			}

			GC.SuppressFinalize(this);
		}

		private async Task<bool> WaitForApiToBeReady(Uri apiUrl, Process process, CancellationToken cts)
		{
			if (process.HasExited == true)
			{
				return false;
			}

			using (var httpClient = new HttpClient())
			{
				while (!cts.IsCancellationRequested)
				{
					if (process.HasExited == true)
					{
						return false;
					}

					try
					{
						var response = await httpClient.GetAsync(apiUrl, HttpCompletionOption.ResponseHeadersRead, cts);
						if (response.IsSuccessStatusCode)
						{
							return true;
						}
					}
					catch (HttpRequestException)
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
