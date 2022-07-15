using IdeaStatiCa.CheckbotPlugin.Models;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public interface IAsyncEventListener
	{
		Task OnEvent(Event e);
	}
}