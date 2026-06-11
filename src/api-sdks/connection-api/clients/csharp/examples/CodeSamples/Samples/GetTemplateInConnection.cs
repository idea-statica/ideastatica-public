using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example retrieves a specific applied template by its instance Id in a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetTemplateInConnection(IConnectionApiClient conClient)
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
			int templateInstanceId = templates[0].TemplateId;

			//Get the template by its instance Id within the connection.
			ConConnectionTemplate template = await conClient.Template.GetTemplateInConnectionAsync(conClient.ActiveProjectId, connectionId, templateInstanceId);

			Console.WriteLine($"Template instance {template.TemplateId} (library template {template.LibraryTemplateId})");
			Console.WriteLine($"Members: {template.Members.Count}, operations: {template.Operations.Count}, parameters: {template.ParameterKeys.Count}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
