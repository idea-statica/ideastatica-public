using System.Threading.Tasks;

namespace IdeaStatiCa.RcsClient.Services
{
	public interface IReinfCssTemplateProvider
	{
		Task<string> GetTemplateAsync();
	}
}
