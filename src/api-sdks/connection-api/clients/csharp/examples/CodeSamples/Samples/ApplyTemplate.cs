using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example applies a template to a naked connection project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task ApplyTemplate(IConnectionApiClient conClient)
		{
			string filePath = "inputs/corner-empty.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			string templateFilePath = "inputs/template-I-corner.contemp";

			ConTemplateMappingGetParam templateImport = conClient.Template.ImportTemplateFromFile(templateFilePath);

			TemplateConversions conversionMapping = await conClient.Template.GetDefaultTemplateMappingAsync(conClient.ActiveProjectId, connectionId, templateImport);

			ConTemplateApplyParam applyParam = new ConTemplateApplyParam();
			
			applyParam.ConnectionTemplate = templateImport.Template;

			//TO DO: We can do some custom mapping if we would like to.
			applyParam.Mapping = conversionMapping;

			var result = conClient.Template.ApplyTemplateAsync(conClient.ActiveProjectId, connectionId, applyParam);


			List<int> requestedConnections = new List<int> { connectionId };

			//Calculate the project with the applied template
			List<ConResultSummary> results = await conClient.Calculation.CalculateAsync(conClient.ActiveProjectId, requestedConnections);

			string exampleFolder = GetExampleFolderPathOnDesktop("ApplyTemplate");
			string fileName = "corner-template-applied.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);

			//Save the applied template
			await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
