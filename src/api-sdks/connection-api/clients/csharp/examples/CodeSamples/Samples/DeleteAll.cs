using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example deletes all applied templates (including their operations) from a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task DeleteAll(IConnectionApiClient conClient)
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
			Console.WriteLine($"Templates in connection {connectionId} before delete: {templates.Count}");

			//Delete all templates - their parameters and operations are removed from the connection.
			await conClient.Template.DeleteAllAsync(conClient.ActiveProjectId, connectionId);

			var remainingTemplates = await conClient.Template.GetTemplatesInConnectionAsync(conClient.ActiveProjectId, connectionId);
			Console.WriteLine($"Templates in connection {connectionId} after delete: {remainingTemplates.Count}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
