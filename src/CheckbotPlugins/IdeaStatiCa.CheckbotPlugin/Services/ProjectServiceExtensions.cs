using IdeaRS.OpenModel;
using IdeaStatiCa.CheckbotPlugin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public static class ProjectServiceExtensions
	{
		public static Task<OpenModelContainer> GetModel(this IProjectService projectService)
			=> projectService.GetModel(ModelExportOptions.Default);

		public static Task<OpenModelContainer> GetObjects(this IProjectService projectService, IEnumerable<ModelObject> objects)
			=> projectService.GetObjects(objects, ModelExportOptions.Default);
	}
}