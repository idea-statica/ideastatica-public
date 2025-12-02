using System.Threading.Tasks;

namespace ConApiWpfClientApp.Services
{
	public interface ITemplateProvider
	{
		Task<string> GetTemplateAsync();
	}
}
