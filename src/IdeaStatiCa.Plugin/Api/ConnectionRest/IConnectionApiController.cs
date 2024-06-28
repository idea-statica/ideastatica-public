using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project;
using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest
{
	public interface IConnectionApiController : IDisposable
	{
		/// <summary>
		/// Open idea project in the service
		/// </summary>
		/// <param name="ideaConProject">Idea Connection project filename</param>
		/// <param name="cancellationToken"></param>
		Task<ConProject> OpenProjectAsync(string ideaConProject, CancellationToken cancellationToken = default);

		/// <summary>
		/// Cloase the active project
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task CloseProjectAsync(CancellationToken cancellationToken = default);

		Task<ConProjectData> GetProjectDataAsync(CancellationToken cancellationToken = default);

		Task<List<ConConnection>> GetConnectionsAsync(CancellationToken token = default);

		Task<ConConnection> GetConnectionAsync(int connectionId, CancellationToken token = default);
		Task<Stream> DownloadProjectAsync(CancellationToken token = default);

		Task<ConConnection> UpdateConnectionAsync(int connectionId, ConConnection connectionUpdate, CancellationToken cancellationToken = default);

		/// Creates Idea connection project from given <paramref name="iomContainerXmlFileName"/> and projects
		/// </summary>
		/// <param name="iomXmlFileName">Filename of a given IOM xml file</param>
		Task<ConProject> CreateProjectFromIomContainerFileAsync(string iomContainerXmlFileName, ConIomImportOptions options, CancellationToken cancellationToken = default);

		/// Creates Idea connection project from given <paramref name="iomXmlFileName"/>, <paramref name="iomResXmlFileName"/> and projects
		/// </summary>
		/// <param name="iomXmlFileName">Filename of a given IOM xml file</param>
		/// <param name="iomResXmlFileName">Filename of a given IOM Result xml file</param>
		Task<ConProject> CreateProjectFromIomFileAsync(string iomXmlFileName, string iomResXmlFileName, ConIomImportOptions options, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates an IDEA Connection project based on Open Model and Open Model Result)
		/// </summary>
		/// <param name="model"></param>
		/// <param name="result"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Task<ConProject> CreateProjectFromIomModelAsync(OpenModel model, OpenModelResult result, ConIomImportOptions options, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates an IDEA Connection project based on OpenModelContainer (model and results)
		/// </summary>
		/// <param name="model"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		Task<ConProject> CreateProjectFromIomContainerAsync(OpenModelContainer model, ConIomImportOptions options, CancellationToken cancellationToken = default);

	}
}
