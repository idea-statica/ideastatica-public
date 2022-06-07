namespace IdeaStatiCa.CheckbotPlugin.Models
{
	public abstract class Event
	{ }

	public class EventPluginStop : Event
	{ }

	public class EventOperationBegin : Event
	{ }

	public class EventOpenCheckApplication : Event
	{
		public ModelObject ModelObject { get; }

		public EventOpenCheckApplication(ModelObject modelObject)
		{
			ModelObject = modelObject;
		}
	}
}