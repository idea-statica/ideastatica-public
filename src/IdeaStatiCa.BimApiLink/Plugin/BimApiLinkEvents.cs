using IdeaStatiCa.Public;

namespace IdeaStatiCa.BimApiLink.Plugin
{

	/// <summary>
	/// Implementation class. Don't use directly, but call from the derived classes
	/// </summary>
	public abstract class IdeaUserEventImplementation : IIdeaUserEvent
	{
		public string EventCategory { get; }
		public string EventName { get; }
		public string EventAction { get; }
		public string EventLabel { get; }
		public int EventValue { get; }

		internal IdeaUserEventImplementation(string eventCategory, string eventName, string eventAction, string eventLabel = null, int eventValue = 0)
		{
			// set the properties
			EventCategory = eventCategory;
			EventName = eventName;
			EventAction = eventAction;
			EventLabel = (eventLabel != null ? eventLabel : null);
			EventValue = eventValue;
		}
	}
	/// <summary>
	/// BIM - plugin command executed event
	/// </summary>
	public class BimPluginCommandEvent : IdeaUserEventImplementation
	{
		public BimPluginCommandEvent(string commandName, string eventLabel = null, int eventValue = 0) : base("BIM", "bim_plugin_command", $"command {commandName} excecuted.", (eventLabel != null ? eventLabel : null), eventValue)
		{
		}
	}

	/// <summary>
	/// BIM - connection imported event
	/// </summary>
	public class BimConnectionsImportEvent : IdeaUserEventImplementation
	{
		public BimConnectionsImportEvent(string bimAppName, string requestType, string contryCode) : base("BIM", "bim_plugin_imported", "connections imported.", $"BIM app {bimAppName}, request type = {requestType}, country code = {contryCode}.")
		{
		}
	}

	/// <summary>
	/// BIM - connection synchronized event
	/// </summary>
	public class BimConnectionsSynchronizeEvent : IdeaUserEventImplementation
	{
		public BimConnectionsSynchronizeEvent(string bimAppName, string contryCode) : base("BIM", "bim_plugin_synchronized", "connections synchronized.", $"BIM app {bimAppName}, country code = {contryCode} .")
		{
		}
	}
}
