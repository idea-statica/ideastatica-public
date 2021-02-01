using System;
using System.ServiceModel;

namespace IdeaStatiCa.Plugin
{
	public class ProgressSender : System.ServiceModel.ClientBase<IProgressCallback>, IProgressCallback
	{
		public ProgressSender(string id) : base(GetBinding(), GetAddress(id))
		{
		}

		public void ExceptionMessage(string function, string message)
		{
			Service?.ExceptionMessage(function, message);
		}

		public void ProgressMessage(double percent, string message)
		{
			Service?.ProgressMessage(percent, message);
		}

		public void IterationMessage(double percent, string message, string iteration)
		{
			Service?.IterationMessage(percent, message, iteration);
		}

		private static System.ServiceModel.Channels.Binding GetBinding()
		{
			NetNamedPipeBinding binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647, OpenTimeout = TimeSpan.MaxValue, CloseTimeout = TimeSpan.MaxValue, ReceiveTimeout = TimeSpan.MaxValue, SendTimeout = TimeSpan.MaxValue };

			return binding;
		}

		private static System.ServiceModel.EndpointAddress GetAddress(string id)
		{
			return new EndpointAddress(string.Format(Constants.ProgressCallbackUrlFormat, id));
		}

		protected IProgressCallback Service => base.Channel;
	}
}