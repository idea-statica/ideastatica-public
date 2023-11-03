using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Api.Rcs;

namespace IdeaStatiCa.RcsClient.Client
{
	internal class HeartbeatChecker : IHeartbeatChecker
	{
		private readonly string _heartbeatEndpoint;
		private readonly HttpClient _client;
		private CancellationTokenSource _cancellationTokenSource;
		private IPluginLogger _logger;
		private TimeSpan _interval;

		public Action<string> HeartBeatLogAction { get; set; } = null;

		public HeartbeatChecker(HttpClient client, string heartbeatEndpoint, TimeSpan? interval = null)
		{
			_client = client;
			_interval = interval ?? TimeSpan.FromSeconds(10);
			_heartbeatEndpoint = heartbeatEndpoint;
			_cancellationTokenSource = new CancellationTokenSource();
		}

		public async Task StartAsync()
		{
			while (!_cancellationTokenSource.Token.IsCancellationRequested)
			{
				try
				{
					var response = await _client.GetStringAsync(_heartbeatEndpoint);
					if (HeartBeatLogAction != null)
					{
						HeartBeatLogAction(response);
					}
				}
				catch (Exception ex)
				{
					_logger.LogError($"API is not responsive: {ex.Message}");
				}

				await Task.Delay(_interval);
			}
		}

		public void Stop()
		{
			if (_cancellationTokenSource is { })
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource.Dispose();
			}
		}
	}
}
