using IdeaStatiCa.CheckbotPlugin.Models;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public interface IEventListener
	{
		void OnEvent(Event e);
	}

	public interface IEventService
	{
		void Subscribe(IEventListener eventListener);
	}
}