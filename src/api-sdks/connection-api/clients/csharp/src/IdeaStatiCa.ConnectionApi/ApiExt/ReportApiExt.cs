using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionApi.Api
{
	/// <summary>
	/// Connection REST API Report API extension methods. 
	/// </summary>
	public interface IReportApiExtAsync : IReportApiAsync
	{
		/// <summary>
		/// Save the connection report to PDF format
		/// </summary>
		/// <param name="projectId">Identifier of the open connection project in the service</param>
		/// <param name="connectionId">ID of the connection in <paramref name="projectId"/></param>
		/// <param name="filePath">The full path to the Pdf file (.pdf) which will be created</param>
		/// <returns></returns>
		Task SaveReportPdfAsync(Guid projectId, int connectionId, string filePath);

		/// <summary>
		/// Save the connection report of multiple connections to PDF format. The generated PDF will contain one section per each connection report.
		/// </summary>
		/// <param name="projectId"></param>
		/// <param name="connectionIds"></param>
		/// <param name="filePath"></param>
		/// <returns></returns>
		Task SaveMultipleReportsPdfAsync(Guid projectId, List<int> connectionIds, string filePath);

		/// <summary>
		/// Save the connection report in Word format
		/// </summary>
		/// <param name="projectId">Identifier of the open connection project in the service</param>
		/// <param name="connectionId">ID of the connection in <paramref name="projectId"/></param>
		/// <param name="filePath">The full path to the Word file (.docx) which will be created</param>
		/// <returns></returns>
		Task SaveReportWordAsync(Guid projectId, int connectionId, string filePath);

		/// <summary>
		/// Save the connection report of multiple connections to Word format. The generated Word document will contain one section per each connection report.
		/// </summary>
		/// <param name="projectId"></param>
		/// <param name="connectionIds"></param>
		/// <param name="filePath"></param>
		/// <returns></returns>
		Task SaveMultipleReportsWordAsync(Guid projectId, List<int> connectionIds, string filePath);
	}

	/// <inheritdoc cref="IReportApiExtAsync" />
	public class ReportApiExt : ReportApi, IReportApiExtAsync
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="client"></param>
		/// <param name="asyncClient"></param>
		/// <param name="configuration"></param>
		internal ReportApiExt(IdeaStatiCa.ConnectionApi.Client.ISynchronousClient client, IdeaStatiCa.ConnectionApi.Client.IAsynchronousClient asyncClient, IdeaStatiCa.ConnectionApi.Client.IReadableConfiguration configuration) : base(client, asyncClient, configuration)
		{
		}

		/// <inheritdoc cref="IReportApiExtAsync.SaveMultipleReportsPdfAsync(Guid, List{int}, string)"/>/param>
		public async Task SaveMultipleReportsPdfAsync(Guid projectId, List<int> connectionIds, string filePath)
		{
			var response = await base.GeneratePdfForMutlipleWithHttpInfoAsync(projectId, connectionIds, "application/octet-stream");
			byte[] buffer = (byte[])response.Data;
			using (var fileStream = System.IO.File.Create(filePath))
			{
				await fileStream.WriteAsync(buffer, 0, buffer.Length);
			}
		}

		/// <inheritdoc cref="IReportApiExtAsync.SaveMultipleReportsWordAsync(Guid, List{int}, string)"/>/param>
		public async Task SaveMultipleReportsWordAsync(Guid projectId, List<int> connectionIds, string filePath)
		{
			var response = await base.GenerateWordForMultipleWithHttpInfoAsync(projectId, connectionIds, "application/octet-stream");
			byte[] buffer = (byte[])response.Data;
			using (var fileStream = System.IO.File.Create(filePath))
			{
				await fileStream.WriteAsync(buffer, 0, buffer.Length);
			}
		}

		/// <inheritdoc cref="IReportApiExtAsync.SaveReportPdfAsync(Guid, int, string)"/>/param>
		public async Task SaveReportPdfAsync(Guid projectId, int connectionId, string filePath)
		{
            var response = await base.GeneratePdfWithHttpInfoAsync(projectId, connectionId, "application/octet-stream");
            byte[] buffer = (byte[])response.Data;
            using (var fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        /// <inheritdoc cref="IReportApiExtAsync.SaveReportWordAsync(Guid, int, string)"/>/param>
        public async Task SaveReportWordAsync(Guid projectId, int connectionId, string filePath)
        {
            var response = await base.GenerateWordWithHttpInfoAsync(projectId, connectionId, "application/octet-stream");
            byte[] buffer = (byte[])response.Data;
            using (var fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}
