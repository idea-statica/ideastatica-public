using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdeaApp.Services
{
	public interface IStartupService
	{
		Task RunCheckbotAsync();

		Task<string> ExportIOMModelAsync(string sourceFile);
	}
}
