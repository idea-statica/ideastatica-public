using IdeaRS.OpenModel;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.RamToIdeaApp.Models;

namespace IdeaStatiCa.RamToIdeaApp.Services
{
	public interface IProjectService
	{
		ModelBIM GetModel(IProjectInfo projectInfo, IProject project, CountryCode countryCode);

		bool IsAvailable();
	}
}
