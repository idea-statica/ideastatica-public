using Grpc.Core;
using IdeaStatiCa.Public;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc.Services
{
	/// <summary>
	/// Service in gRPC server for blob storage communication
	/// </summary>
	public class GrpcNotificationService : Grpc.GrpcNotificationService.GrpcNotificationServiceBase
	{
		private readonly IPluginLogger _logger;
		private readonly IParentAppNotificationHandler _parentAppNotificationHandler;

		/// <summary>
		/// Creates gRPC blob storage service
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="parentAppNotificationHandler">target of notification. No action is invoked if null</param>
		public GrpcNotificationService(IPluginLogger logger, IParentAppNotificationHandler parentAppNotificationHandler = null)
		{
			_logger = logger;
			_parentAppNotificationHandler = parentAppNotificationHandler;
		}

		/// <summary>
		/// Calls IParentAppNotificationHandler with information, that currently opened item changed
		/// </summary>
		/// <param name="request"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public override async Task<VoidResponse> CurrentlyOpenedItemModified(ItemNotifyRequest request, ServerCallContext context)
		{
			try
			{
				if (_parentAppNotificationHandler != null)
				{
					_logger.LogDebug($"GrpcNotificationService begins CurrentlyOpenedItemModified");
					await _parentAppNotificationHandler.NotifyItemChangedAsync(request.ItemKey);
				}
				else
				{
					_logger.LogDebug($"GrpcNotificationService CurrentlyOpenedItemModified ignored, as no one is subscribed");
				}
				return new VoidResponse();
			}
			catch (Exception ex)
			{
				_logger.LogDebug("GrpcNotificationService CurrentlyOpenedItemModified failed.", ex);
				throw;
			}
		}
	}
}
