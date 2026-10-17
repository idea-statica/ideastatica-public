using System;
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi.Api
{
	/// <summary>
	/// Connection REST API Report API extension methods. 
	/// </summary>
	public interface IExportApiExtAsync : IExportApiAsync
	{
		/// <summary>
		/// Save the connection to the IFC format
		/// </summary>
		/// <param name="projectId">Identifier of the open connection project in the service</param>
		/// <param name="connectionId">ID of the connection in <paramref name="projectId"/></param>
		/// <param name="filePath">The full path to the IFC file which will be created</param>
		/// <returns></returns>
		Task ExportIfcFileAsync(Guid projectId, int connectionId, string filePath);

		/// <summary>
		/// Save the connection as a DWG drawing. The drawing is produced by the IDEA StatiCa cloud Viewer under the licence
		/// signed in on the service machine, and the call waits for it - up to ten minutes.
		/// </summary>
		/// <param name="projectId">Identifier of the open connection project in the service</param>
		/// <param name="connectionId">ID of the connection in <paramref name="projectId"/></param>
		/// <param name="filePath">The full path to the DWG file which will be created</param>
		/// <returns></returns>
		Task ExportDwgFileAsync(Guid projectId, int connectionId, string filePath);
	}

	/// <inheritdoc cref="IExportApiExtAsync" />
	public class ExportApiExt : ExportApi, IExportApiExtAsync
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="client"></param>
		/// <param name="asyncClient"></param>
		/// <param name="configuration"></param>
		internal ExportApiExt(IdeaStatiCa.ConnectionApi.Client.ISynchronousClient client, IdeaStatiCa.ConnectionApi.Client.IAsynchronousClient asyncClient, IdeaStatiCa.ConnectionApi.Client.IReadableConfiguration configuration) : base(client, asyncClient, configuration)
		{
		}

		/// <inheritdoc cref="IExportApiExtAsync.ExportIfcFileAsync(Guid, int, string)"/>/param>
		/// <returns></returns>
		public async Task ExportIfcFileAsync(Guid projectId, int connectionId, string filePath)
		{
			var response = await base.ExportIFCWithHttpInfoAsync(projectId, connectionId, "text/plain");
			string ifc = (string)response.Data;

			// Write the string to the file
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
			await File.AppendAllTextAsync(filePath, ifc);
#else
			File.WriteAllText(filePath, ifc);
#endif
		}

		/// <inheritdoc cref="IExportApiExtAsync.ExportDwgFileAsync(Guid, int, string)"/>
		public async Task ExportDwgFileAsync(Guid projectId, int connectionId, string filePath)
		{
			var response = await base.ExportDWGWithHttpInfoAsync(projectId, connectionId, "application/octet-stream");
			await ProducedFile.SaveAsync((byte[])response.Data, filePath);
		}
	}


}
