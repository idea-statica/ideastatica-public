using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
	public interface IGrpcNotificationClient
	{
		void NotifyCurrentlyOpenedItemModified(int itemKey);
		Task NotifyCurrentlyOpenedItemModifiedAsync(int itemKey);
	}

	/// <summary>
	/// Client for gRPC notification service communication
	/// </summary>
	public class GrpcNotificationClient : IDisposable, IGrpcNotificationClient
	{
		private readonly GrpcNotificationService.GrpcNotificationServiceClient client;
		private readonly Channel _channel;
		private readonly IPluginLogger _logger;

		/// <summary>
		/// Creates gRPC notification service client
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="port">Port</param>
		/// <param name="host">Host</param>
		/// <param name="maxDataLength">Maximum message length in bytes for channel</param>
		public GrpcNotificationClient(IPluginLogger logger, int port, string host = "localhost", int maxDataLength = Constants.GRPC_MAX_MSG_SIZE)
		{
			this._logger = logger;
			_channel = new Channel(host, port, ChannelCredentials.Insecure, CommunicationTools.GetChannelOptions(maxDataLength));
			client = new GrpcNotificationService.GrpcNotificationServiceClient(_channel);

			logger.LogDebug($"Created GrpcNotificationClient, host: '{host}', port: '{port}',  maxDataLength: '{maxDataLength}'");
		}

		/// <summary>
		/// Notifies parent application, that currently opened item have been changed
		/// </summary>
		/// <param name="itemKey">Blob storage id</param>
		/// <returns></returns>
		public void NotifyCurrentlyOpenedItemModified(int itemKey)
		{
			_logger.LogDebug($"GrpcNotificationClient starts NotifyCurrentlyOpenedItemModified, itemKey: '{itemKey}'");

			try
			{
				client.CurrentlyOpenedItemModified(new ItemNotifyRequest
				{
					ItemKey = itemKey
				});

				_logger.LogDebug($"GrpcNotificationClient ends NotifyCurrentlyOpenedItemModified, itemKey: '{itemKey}'");
			}
			catch (Exception ex)
			{
				_logger.LogDebug("GrpcNotificationClient NotifyCurrentlyOpenedItemModified failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Notifies parent application, that currently opened item have been changed
		/// </summary>
		/// <param name="itemKey">Blob storage id</param>
		/// <returns></returns>
		public async Task NotifyCurrentlyOpenedItemModifiedAsync(int itemKey)
		{
			_logger.LogDebug($"GrpcNotificationClient starts NotifyCurrentlyOpenedItemModified, itemKey: '{itemKey}'");

			try
			{
				await client.CurrentlyOpenedItemModifiedAsync(new ItemNotifyRequest
				{
					ItemKey = itemKey
				});

				_logger.LogDebug($"GrpcNotificationClient ends NotifyCurrentlyOpenedItemModified, itemKey: '{itemKey}'");
			}
			catch (Exception ex)
			{
				_logger.LogDebug("GrpcNotificationClient NotifyCurrentlyOpenedItemModified failed.", ex);
				throw;
			}
		}

		/// <summary>
		/// Shuts down communication channel with gRPC server
		/// </summary>
		public void Dispose()
		{
			try
			{
				if (!_channel.ShutdownAsync().Wait(TimeSpan.FromMinutes(1)))
				{
					_logger.LogDebug("Time out - gRPC client does not shut down channel within one minute.");
				}

				_logger.LogDebug("Disposed GrpcNotificationClient");
			}
			catch (Exception ex)
			{
				_logger.LogDebug("Dispose of GrpcNotificationClient failed.", ex);
			}
		}
	}
}
