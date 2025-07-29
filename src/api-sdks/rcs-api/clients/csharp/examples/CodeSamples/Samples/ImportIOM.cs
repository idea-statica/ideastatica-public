using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdeaStatiCa.RcsApi;
using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.RcsApi.Client;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Creates a new RCS project from a selected IOM file.
		/// </summary>
		/// <param name="rcsClient">The connected RCS API Client</param>
		public static async Task CreateProjectFromIom(IRcsApiClient rcsClient)
		{
			string filePath = "Inputs/ImportOpenModel.xml";

			RcsProject rcsProject = await rcsClient.Project.CreateProjectFromIomFileAsync(filePath);

			string exampleFolder = GetExampleFolderPathOnDesktop("CreateProjectFromIOM");

			// Save updated file.
			string fileName = "rcs-file-from-IOM.ideaRcs";
			string saveFilePath = Path.Combine(exampleFolder, fileName);
			
			await rcsClient.Project.SaveProjectAsync(rcsProject.ProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);


			await rcsClient.Project.SaveProjectAsync(rcsProject.ProjectId, saveFilePath);

			//Only one connection allowed on Client so it will always be managed by the Client.
			//await rcsClient.Project.CloseProjectAsync(rcsProject.ProjectId);
		}
	}
}
