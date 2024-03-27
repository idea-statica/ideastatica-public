namespace IdeaStatiCa.CheckbotPlugin.Models
{
	public abstract class Event
	{ }

	public class EventPluginStop : Event
	{ }

	public class EventProcedureBegin : Event
	{ }

	public class EventOpenCheckApplication : Event
	{
		public ModelObject ModelObject { get; }

		public EventOpenCheckApplication(ModelObject modelObject)
		{
			ModelObject = modelObject;
		}
	}

	public class EventCustomButtonClicked : Event
	{
		public string ButtonName { get; }

		public EventCustomButtonClicked(string buttonName)
		{
			ButtonName = buttonName;
		}
	}
}