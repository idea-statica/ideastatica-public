using IdeaRS.OpenModel;
using IdeaStatiCa.Api.Connection.Model.Conversion;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Changes the design code of an ECEN (Eurocode) project to AISC (American design code).
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task ChangeCode(IConnectionApiClient conClient)
		{
			//Only projects with the ECEN (Eurocode) design code can be converted.
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get the default mapping of materials, cross-sections and fasteners for the conversion to AISC (American design code).
			ConConversionSettings conversionSettings = await conClient.Conversion.GetConversionMappingAsync(conClient.ActiveProjectId, CountryCode.American);

			//TO DO: Here we can modify the proposed TargetValue of the individual mappings if we would like to.

			//Change the design code of the project using the conversion settings.
			// v4: ChangeCode returns 204 NoContent — success has no body.
			await conClient.Conversion.ChangeCodeAsync(conClient.ActiveProjectId, conversionSettings);
			Console.WriteLine($"Change of design code to {conversionSettings.TargetDesignCode} finished.");

			string exampleFolder = GetExampleFolderPathOnDesktop("ChangeCode");
			string saveFilePath = Path.Combine(exampleFolder, "cleat-connection-AISC.ideaCon");

			//Save the converted project.
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
