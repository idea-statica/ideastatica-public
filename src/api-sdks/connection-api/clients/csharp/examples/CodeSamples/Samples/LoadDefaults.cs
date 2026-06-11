using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example loads the parameter defaults of a specific applied template, resetting its parameters to their default values.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task LoadDefaults(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/corner-empty.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Apply a template first so the connection contains an applied template.
			ConTemplateMappingGetParam templateImport = conClient.Template.ImportTemplateFromFile("Inputs/template-I-corner.contemp");
			TemplateConversions mapping = await conClient.Template.GetDefaultTemplateMappingAsync(conClient.ActiveProjectId, connectionId, templateImport);
			await conClient.Template.ApplyTemplateAsync(conClient.ActiveProjectId, connectionId, new ConTemplateApplyParam { ConnectionTemplate = templateImport.Template, Mapping = mapping });

			List<ConConnectionTemplate> templates = await conClient.Template.GetTemplatesInConnectionAsync(conClient.ActiveProjectId, connectionId);
			Guid templateId = templates[0].LibraryTemplateId;

			//Load the default values of the template parameters.
			ParameterUpdateResponse response = await conClient.Template.LoadDefaultsAsync(conClient.ActiveProjectId, connectionId, templateId);

			Console.WriteLine($"Defaults loaded for template {templateId}, set to model: {response.SetToModel}");
			Console.WriteLine($"Parameters: {response.Parameters.Count}, failed validations: {response.FailedValidations.Count}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
