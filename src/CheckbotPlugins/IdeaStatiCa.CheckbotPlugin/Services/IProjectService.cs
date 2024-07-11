using IdeaStatiCa.CheckbotPlugin.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
		Task<ProjectInfo> GetInfo();

		/// <summary>
		/// Returns IOM of the whole project.
		/// </summary>
		/// <param name="options"></param>
		/// <returns>OpenModelContainer XML</returns>
		TextReader GetModel(ModelExportOptions options);

		/// <summary>
		/// Returns list of all objects in the project.
		/// </summary>
		/// <returns></returns>
		Task<IReadOnlyCollection<ModelObject>> ListObjects();

		/// <summary>
		/// Returns data for specified <paramref name="objects"/> in a single IOM.
		/// </summary>
		/// <param name="objects"></param>
		/// <param name="options"></param>
		/// <returns>OpenModelContainer XML</returns>
		TextReader GetObjects(IEnumerable<ModelObject> objects, ModelExportOptions options);
	}
}