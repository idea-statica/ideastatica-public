using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Generates the report of a given connection in HTML format and saves it as a zip file.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GenerateHtmlZip(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			string exampleFolder = GetExampleFolderPathOnDesktop("GenerateReport");

			string fileName = "simple cleat connection report.zip";
			string zipFilePath = Path.Combine(exampleFolder, fileName);

			//Generate the report in HTML format and get its content as a byte array.
			var response = await conClient.Report.GenerateHtmlZipWithHttpInfoAsync(conClient.ActiveProjectId, connectionId, "application/octet-stream");
			byte[] buffer = (byte[])response.Data;

			//Save the zipped HTML report.
			await File.WriteAllBytesAsync(zipFilePath, buffer);

			Console.WriteLine($"Report saved to: {zipFilePath}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
