using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.RcsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets avaliable settings in the project
		/// </summary>
		/// <param name="rcsClient">The connected RCS API Client</param>
		public static async Task GetSettings(IRcsApiClient rcsClient)
		{
			string filePath = "Inputs/Project1.IdeaRcs";

			RcsProject rcsProject = await rcsClient.Project.OpenProjectAsync(filePath);

			string settings = await rcsClient.Project.GetCodeSettingsAsync(rcsClient.Project.ProjectId);


			/*
			//Provide list of possible settings to update

			//Get a folder on Desktop for the Example
			string exampleFolder = GetExampleFolderPathOnDesktop("Update Section");

			// Save updated file.
			string fileName = "Project1_settings_updated.ideaRcs";
			string saveFilePath = Path.Combine(exampleFolder, fileName);


			await rcsClient.Project.SaveProjectAsync(rcsClient.Project.ProjectId, saveFilePath);

			Console.WriteLine("File saved to: " + saveFilePath);

			//close the project
			await rcsClient.Project.CloseProjectAsync(rcsClient.Project.ProjectId);
			*/

		}
	}
}
