using IdeaStatiCa.Plugin;
using System.Threading.Tasks;

namespace IdeaStatiCa.BimApiLink
{
	public interface IApplicationBimRunnable : IApplicationBIM
	{
		Task Run();
	}
}