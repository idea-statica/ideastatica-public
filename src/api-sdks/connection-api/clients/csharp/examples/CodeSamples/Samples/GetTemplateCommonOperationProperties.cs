using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example gets the common operation properties (plate material, weld material, bolt assembly) of a specific applied template.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetTemplateCommonOperationProperties(IConnectionApiClient conClient)
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

			//Get the common properties shared by the operations of the template.
			var commonProperties = await conClient.Template.GetTemplateCommonOperationPropertiesAsync(conClient.ActiveProjectId, connectionId, templateId);

			Console.WriteLine($"Common operation properties of template {templateId}:");
			Console.WriteLine("Plate material Id: " + commonProperties.PlateMaterialId);
			Console.WriteLine("Weld material Id: " + commonProperties.WeldMaterialId);
			Console.WriteLine("Bolt assembly Id: " + commonProperties.BoltAssemblyId);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
