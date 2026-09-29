using IdeaStatiCa.Api.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
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

		/// <summary>
		/// How long <see cref="CreateApiClient"/> waits for a freshly started service to answer its
		/// heartbeat before giving up. A cold start pulls the Connection Library design sets before it
		/// opens the listener, so it can take considerably longer than a warm one.
		/// </summary>
		public static readonly TimeSpan DefaultStartupTimeout = TimeSpan.FromSeconds(120);

		/// <summary>
		/// Number of trailing output lines of the service process kept for diagnostics. The service logs
		/// every request once it is up, so only the tail is of any use and an unbounded buffer would grow
		/// for as long as the service runs.
		/// </summary>
		private const int CAPTURED_OUTPUT_LINES = 40;

		/// <summary>
		/// How long to wait between heartbeat attempts while the service starts.
		/// </summary>
		private const int POLL_INTERVAL_MS = 500;

		private Process serviceProcess;
		private string launchPath;
		private int port = -1;
		private readonly string clientApplication;
		private readonly string clientApplicationVersion;
		private readonly TimeSpan startupTimeout;
		private readonly Queue<string> capturedOutput = new Queue<string>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setupDir"> where .exe file is located</param>
		/// <param name="clientApplication">
		/// Name of the application making the calls, for example "NorsokChecker" - a constant of the
		/// build, so it belongs to the factory rather than to a single call. Every client this factory
		/// creates is reported under it, which is what lets usage be attributed to an integration at
		/// all; see <see cref="ClientApplicationIdentity"/>. Optional.
		/// </param>
		/// <param name="clientApplicationVersion">Version of that application. Optional.</param>
		/// <param name="startupTimeout">
		/// How long to wait for the started service to answer its heartbeat. Defaults to
		/// <see cref="DefaultStartupTimeout"/>.
		/// </param>
		public ConnectionApiServiceRunner(string setupDir, string clientApplication = null,
			string clientApplicationVersion = null, TimeSpan? startupTimeout = null)
		{
			launchPath = setupDir;
			this.clientApplication = clientApplication;
			this.clientApplicationVersion = clientApplicationVersion;
			this.startupTimeout = startupTimeout ?? DefaultStartupTimeout;
		}

		/// <inheritdoc cref="IApiServiceFactory{T}.CreateApiClient"/>
		public async Task<IConnectionApiClient> CreateApiClient()
		{
			var url = await StartService();
			var client = new ConnectionApiClient(url, clientApplication, clientApplicationVersion);
			await client.CreateAsync();
			return client;
		}

		private async Task<string> StartService()
		{
			return await Task.Run<string>(async() =>
			{
				if (serviceProcess is null)
				{
					port = GetAvailablePort();
					if (port <= 0)
					{
						throw new InvalidOperationException("No available port found.");
					}

					var directoryName = !string.IsNullOrEmpty(launchPath) ? launchPath : Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
					string apiExecutablePath = Path.Combine(directoryName, API_EXECUTABLE_NAME);

					if (!File.Exists(apiExecutablePath))
					{
						throw new FileNotFoundException($"API executable not found at path: {apiExecutablePath}");
					}

					// The process is only published to the field once it is confirmed to be serving. A
					// half-started one left behind there would make every later call return a URL that
					// nothing is listening on, which is indistinguishable from the service having died.
					var startedProcess = StartServiceProcess(apiExecutablePath);
					try
					{
						var apiUrlBase = new Uri($"{LOCALHOST_URL}:{port}");
						var apiUrlHeartbeat = new Uri(apiUrlBase, IdeaStatiCa.Api.Common.RestApiConstants.RestApiHeartbeat);
						var cts = new CancellationTokenSource(startupTimeout);
						var isApiReady = await WaitForApiToBeReady(apiUrlHeartbeat, startedProcess, cts.Token);

						if (!isApiReady || startedProcess.HasExited)
						{
							throw new InvalidOperationException(DescribeFailedStart(apiExecutablePath, startedProcess));
						}

						serviceProcess = startedProcess;
						startedProcess = null;
					}
					finally
					{
						// Anything still assigned here never became usable - kill it rather than leave an
						// orphaned service holding a licence seat for the rest of the session.
						KillQuietly(startedProcess);
					}
				}

				return $"{LOCALHOST_URL}:{port}";
			});
		}

		private Process StartServiceProcess(string apiExecutablePath)
		{
			var process = new Process();
			process.StartInfo.FileName = apiExecutablePath;
			process.StartInfo.Arguments = $"-port={port}";
			process.StartInfo.UseShellExecute = false;

			// Capture the service's own output. Without it a service that dies on startup - a missing
			// ASP.NET Core runtime, a licence that does not permit the API, a port taken in the moment
			// between picking it and binding it - leaves the caller with nothing to go on, because a
			// child process that fails this way writes no Windows Event Log entry of its own.
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.OutputDataReceived += OnServiceOutput;
			process.ErrorDataReceived += OnServiceOutput;

			try
			{
				if (!process.Start())
				{
					throw new InvalidOperationException($"Failed to start the process. {apiExecutablePath}");
				}

				// Both pipes have to be drained continuously; a full pipe buffer would block the service.
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
			}
			catch
			{
				process.Dispose();
				throw;
			}

			return process;
		}

		private void OnServiceOutput(object sender, DataReceivedEventArgs e)
		{
			if (e.Data == null)
			{
				return;
			}

			lock (capturedOutput)
			{
				capturedOutput.Enqueue(e.Data);
				while (capturedOutput.Count > CAPTURED_OUTPUT_LINES)
				{
					capturedOutput.Dequeue();
				}
			}
		}

		private string DescribeFailedStart(string apiExecutablePath, Process process)
		{
			var message = new StringBuilder();
			message.AppendLine($"Failed to start the application: '{apiExecutablePath}'.");

			bool hasExited;
			try
			{
				hasExited = process.HasExited;
			}
			catch (InvalidOperationException)
			{
				hasExited = false;
			}

			if (hasExited)
			{
				// The parameterless overload also waits for the redirected streams to be drained, so the
				// service's last words - usually the only explanation there is - are not lost to a race.
				try
				{
					process.WaitForExit();
				}
				catch (SystemException)
				{
					// Not waitable any more; report what was captured so far.
				}

				message.AppendLine($"The service exited with code {process.ExitCode} before it started serving requests.");
				message.AppendLine("Exit code 150 means the required .NET runtime is missing: the service needs the " +
					"ASP.NET Core Runtime (x64) of its target version, which is a separate download from the " +
					".NET Desktop Runtime. See https://aka.ms/dotnet-core-applaunch");
			}
			else
			{
				message.AppendLine($"It did not answer its heartbeat on port {port} within " +
					$"{startupTimeout.TotalSeconds:N0} s and was stopped. A cold start can be slow; pass a longer " +
					"startupTimeout to the constructor if this machine needs one.");
			}

			var output = GetCapturedOutput();
			if (!string.IsNullOrEmpty(output))
			{
				message.AppendLine("Last output of the service:");
				message.Append(output);
			}
			else
			{
				message.Append("The service produced no output.");
			}

			return message.ToString();
		}

		/// <summary>
		/// The trailing output written by the service process, newest last. Empty when the service was
		/// never started or wrote nothing.
		/// </summary>
		public string GetCapturedOutput()
		{
			lock (capturedOutput)
			{
				return string.Join(Environment.NewLine, capturedOutput.ToArray());
			}
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
			KillQuietly(serviceProcess);
			serviceProcess = null;
			GC.SuppressFinalize(this);
		}

		private static void KillQuietly(Process process)
		{
			if (process == null)
			{
				return;
			}

			try
			{
				if (!process.HasExited)
				{
					process.Kill();
				}
			}
			catch (InvalidOperationException)
			{
				// Already gone - nothing to kill.
			}
			catch (System.ComponentModel.Win32Exception)
			{
				// Exited between the check and the kill, or we are not allowed to. Disposing is not the
				// place to fail over it.
			}
			finally
			{
				process.Dispose();
			}
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

					bool isReady = false;
					try
					{
						var response = await httpClient.GetAsync(apiUrl, HttpCompletionOption.ResponseHeadersRead, cts);
						isReady = response.IsSuccessStatusCode;
					}
					catch (HttpRequestException)
					{
						// Nothing is listening yet. Wait and try it again.
					}
					catch (OperationCanceledException)
					{
						// Timeout occurred during the http call waiting for the REST API to be ready.
						return false;
					}

					if (isReady)
					{
						return true;
					}

					// The back-off must not live inside a catch block: an exception thrown there escapes
					// the whole try statement, so a timeout during it used to leave CreateApiClient
					// throwing a bare "A task was canceled." instead of saying what went wrong. The
					// no-success-status case had no back-off at all and spun on the service.
					try
					{
						await Task.Delay(POLL_INTERVAL_MS, cts);
					}
					catch (OperationCanceledException)
					{
						return false;
					}
				}

				// Timeout waiting for the REST API to be ready
				return false;
			}
		}
	}
}
