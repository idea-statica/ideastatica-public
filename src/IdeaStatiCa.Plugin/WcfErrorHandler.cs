#if NET48

using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// For more details see https://www.c-sharpcorner.com/UploadFile/b182bf/centralize-exception-handling-in-wcf-part-10/
	/// </summary>
	public class ErrorHandlerAttribute : Attribute, IServiceBehavior
	{
		private readonly Type errorHandler;
		public ErrorHandlerAttribute(Type errorHandler)
		{
			this.errorHandler = errorHandler;
		}
		public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{

		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{
			IErrorHandler handler = (IErrorHandler)Activator.CreateInstance(this.errorHandler);
			foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
				if (channelDispatcher != null)
				{
					channelDispatcher.ErrorHandlers.Add(handler);
				}
			}
		}

		public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
		{

		}
	}

	/// <summary>
	/// For more details see https://www.c-sharpcorner.com/UploadFile/b182bf/centralize-exception-handling-in-wcf-part-10/
	/// </summary>
	public class WcfErrorHandler : IErrorHandler
	{
		private static object initLock = new object();
		public static IPluginLogger  Logger { get; private set; }


		/// <summary>
		/// It initialize error pluginLogger for all instances of <see cref="WcfErrorHandler"/>
		/// Only the first initialization is executed
		/// </summary>
		/// <param name="logger">The instance of the pluginLogger where all wcf errors will be written</param>
		public static void InitLogger(IPluginLogger logger)
		{
			lock(initLock)
			{
				if(Logger == null)
				{
					Logger = logger;
				}
			}
		}

		/// <summary>
		/// Provide a fault. The Message fault parameter can be replaced, or set to null to suppress reporting a fault.
		/// </summary>
		/// <param name="error"></param>
		/// <param name="version"></param>
		/// <param name="fault"></param>
		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			if (Logger != null)
			{
				Logger.LogWarning("WcfErrorHandler.ProvideFault", error);
			}
		}

		/// <summary>
		/// HandleError. Log an error, then allow the error to be handled as usual.
		/// </summary>
		/// <param name="error">Exception</param>
		/// <returns>Return true if the error is considered as already handled</returns>
		public bool HandleError(Exception error)
		{
			if (Logger != null)
			{
				Logger.LogWarning("WcfErrorHandler.HandleError", error);
			}
			return true;
		}
	}
}
#endif