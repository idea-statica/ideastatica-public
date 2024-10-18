using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Model;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example applies a template to a naked connection project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task ApplyTemplate(ConnectionApiClient conClient)
		{
			string filePath = "inputs/corner-empty.ideaCon";
			ConProject conProject = await conClient.Project.OpenProjectAsync(filePath);

			//Get projectId Guid
			Guid projectId = conProject.ProjectId;
			var connections = await conClient.Connection.GetConnectionsAsync(projectId);
			int connectionId = connections[0].Id;

			string templateFilePath = "inputs/template-I-corner.contemp";

			//FIX: Change class name to 'ConTemplate'?? || 'ConTemplateWrapper'?
			ConTemplateMappingGetParam templateImport = conClient.Template.ImportTemplateFromFile(templateFilePath);

			//FIX: Change class name to 'ConTemplateMappings'
			TemplateConversions conversionMapping = await conClient.Template.GetDefaultTemplateMappingAsync(projectId, connectionId, templateImport);

			//Fix: Param needs to be plural.
			ConTemplateApplyParam applyParam = new ConTemplateApplyParam();
			
			applyParam.ConnectionTemplate = templateImport.Template;

			//TO DO: We can do some custom mapping if we would like to.
			applyParam.Mapping = conversionMapping;

			var result = conClient.Template.ApplyTemplateAsync(projectId, connectionId, applyParam);

			//FIX Parameter --> Params?? 
			ConCalculationParameter calculationParams = new ConCalculationParameter();
			calculationParams.ConnectionIds = new List<int> { connectionId };

			//Calculate the project with the applied template
			List<ConResultSummary> results = await conClient.Calculation.CalculateAsync(projectId, calculationParams);

			string exampleFolder = GetExampleFolderPathOnDesktop("ApplyTemplate");
			string fileName = "corner-template-applied.ideaCon";
			string saveFilePath = Path.Combine(exampleFolder, fileName);

			//Save the applied template
			await conClient.Project.SaveProjectAsync(projectId, saveFilePath);
			Console.WriteLine("Project saved to: " + saveFilePath);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(projectId);
		}
	}
}
