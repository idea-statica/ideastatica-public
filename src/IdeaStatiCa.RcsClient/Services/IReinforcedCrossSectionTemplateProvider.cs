using System.Threading.Tasks;

namespace IdeaStatiCa.RcsClient.Services
{
	public interface IReinforcedCrossSectionTemplateProvider
	{
		Task<string> GetTemplateAsync();
	}
}
