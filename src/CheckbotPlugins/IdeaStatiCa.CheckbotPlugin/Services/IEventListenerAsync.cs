using IdeaStatiCa.CheckbotPlugin.Models;
using System.Threading.Tasks;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public interface IAsyncEventListener
	{
		Task OnEvent(Event e);
	}
}