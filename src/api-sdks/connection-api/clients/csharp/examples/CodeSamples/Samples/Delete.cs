using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example deletes a specific applied template (including its operations) from a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task Delete(IConnectionApiClient conClient)
		{
			string filePath = "inputs/corner-empty.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Apply a template first so the connection contains an applied template.
			ConTemplateMappingGetParam templateImport = conClient.Template.ImportTemplateFromFile("inputs/template-I-corner.contemp");
			TemplateConversions mapping = await conClient.Template.GetDefaultTemplateMappingAsync(conClient.ActiveProjectId, connectionId, templateImport);
			await conClient.Template.ApplyTemplateAsync(conClient.ActiveProjectId, connectionId, new ConTemplateApplyParam { ConnectionTemplate = templateImport.Template, Mapping = mapping });

			List<ConConnectionTemplate> templates = await conClient.Template.GetTemplatesInConnectionAsync(conClient.ActiveProjectId, connectionId);
			Guid templateId = templates[0].LibraryTemplateId;

			//Delete the template - its parameters and operations are removed from the connection.
			await conClient.Template.DeleteAsync(conClient.ActiveProjectId, connectionId, templateId);

			var remainingTemplates = await conClient.Template.GetTemplatesInConnectionAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine($"Template {templateId} deleted. Templates remaining in connection {connectionId}: {remainingTemplates.Count}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
