using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.RCS;

namespace IdeaStatiCa.RcsClient.Client
{
	internal class HeartbeatChecker : IHeartbeatChecker
	{
		private readonly string heartbeatEndpoint;
		private readonly HttpClient client;
		private CancellationTokenSource cancellationTokenSource;
		private IPluginLogger logger;
		private TimeSpan interval;

		public Action<string> HeartBeatLogAction { get; set; } = null;

		public HeartbeatChecker(IPluginLogger logger, HttpClient client, string heartbeatEndpoint, TimeSpan? interval = null)
		{
			this.client = client;
			this.logger = logger;
			this.interval = interval ?? TimeSpan.FromSeconds(10);
			this.heartbeatEndpoint = heartbeatEndpoint;
			this.cancellationTokenSource = new CancellationTokenSource();
		}

		/// <summary>
		/// Starts checking process if API is still active based on interval from constructor
		/// Default is 10 seconds
		/// </summary>
		/// <returns></returns>
		public async Task StartAsync()
		{
			while (!cancellationTokenSource.Token.IsCancellationRequested)
			{
				try
				{
					var response = await client.GetStringAsync(heartbeatEndpoint);
					if (HeartBeatLogAction != null)
					{
						HeartBeatLogAction(response);
					}
				}
				catch (Exception ex)
				{
					logger.LogError($"API is not responsive: {ex.Message}");
				}

				await Task.Delay(interval);
			}
		}

		/// <summary>
		/// Stops the automatic check process by cancelling the token
		/// </summary>
		public void Stop()
		{
			if (cancellationTokenSource is { })
			{
				cancellationTokenSource.Cancel();
				cancellationTokenSource.Dispose();
			}
		}
	}
}
