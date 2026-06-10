using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example retrieves the list of templates applied on a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetTemplatesInConnection(IConnectionApiClient conClient)
		{
			string filePath = "inputs/corner-empty.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Apply a template first so the connection contains an applied template.
			ConTemplateMappingGetParam templateImport = conClient.Template.ImportTemplateFromFile("inputs/template-I-corner.contemp");
			TemplateConversions mapping = await conClient.Template.GetDefaultTemplateMappingAsync(conClient.ActiveProjectId, connectionId, templateImport);
			await conClient.Template.ApplyTemplateAsync(conClient.ActiveProjectId, connectionId, new ConTemplateApplyParam { ConnectionTemplate = templateImport.Template, Mapping = mapping });

			//Get all templates applied on the connection.
			List<ConConnectionTemplate> templates = await conClient.Template.GetTemplatesInConnectionAsync(conClient.ActiveProjectId, connectionId);

			Console.WriteLine($"Templates applied in connection {connectionId}: {templates.Count}");
			foreach (ConConnectionTemplate template in templates)
			{
				Console.WriteLine($"Instance Id: {template.TemplateId}, library template: {template.LibraryTemplateId}, " +
					$"members: {template.Members.Count}, operations: {template.Operations.Count}, parameters: {template.ParameterKeys.Count}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
