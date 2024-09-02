namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public interface IPluginControlBehaviour
	{
		/// <summary>
		/// Automatically set the plugin to ready state when the first subscriber subscribes
		/// to the event service.
		/// </summary>
		bool ReadyOnFirstSubscribe { get; }

		/// <summary>
		/// Sets whether the event service subscriber should receive the stop event.
		/// </summary>
		bool ReceiveStopEvent { get; }
	}

	public class PluginControlBehaviour : IPluginControlBehaviour
	{
		public bool ReadyOnFirstSubscribe { get; }
		public bool ReceiveStopEvent { get; }

		public PluginControlBehaviour(bool readyOnFirstSubscribe, bool receiveStopEvent)
		{
			ReadyOnFirstSubscribe = readyOnFirstSubscribe;
			ReceiveStopEvent = receiveStopEvent;
		}
	}

	public class LegacyPluginControlBehaviour : IPluginControlBehaviour
	{
		public bool ReadyOnFirstSubscribe => true;
		public bool ReceiveStopEvent => true;
	}
}