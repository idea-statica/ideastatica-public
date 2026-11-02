using IdeaStatiCa.CheckbotPlugin.Models;
using System.Collections.Generic;
using System.IO;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public static class ProjectServiceExtensions
	{
		public static TextReader GetModel(this IProjectService projectService)
			=> projectService.GetModel(ModelExportOptions.Default);

		public static TextReader GetObjects(this IProjectService projectService, IEnumerable<ModelObject> objects)
			=> projectService.GetObjects(objects, ModelExportOptions.Default);
	}
}