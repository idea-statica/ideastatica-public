using IdeaRS.OpenModel;
using IdeaStatiCa.CheckbotPlugin.Models;
using System.Collections.Generic;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	/// <summary>
	/// Provides project data.
	/// </summary>
	public interface IProjectService
	{
		/// <summary>
		/// Returns information about the current project.
		/// </summary>
		/// <returns></returns>
		ProjectInfo GetInfo();

		/// <summary>
		/// Returns IOM of the whole project. 
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		OpenModelContainer GetModel(ModelExportOptions options);

		/// <summary>
		/// Returns list of all objects in the project.
		/// </summary>
		/// <returns></returns>
		IReadOnlyCollection<ModelObject> ListObjects();

		/// <summary>
		/// Returns data for specified <paramref name="objects"/> in a single IOM.
		/// </summary>
		/// <param name="objects"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		OpenModelContainer GetObjects(List<ModelObject> objects, ModelExportOptions options);
	}
}