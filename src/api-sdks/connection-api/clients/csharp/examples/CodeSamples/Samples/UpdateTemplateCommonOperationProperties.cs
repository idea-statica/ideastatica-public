using IdeaRS.OpenModel.Material;
using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example updates the common operation properties (plate material, weld material, bolt assembly) of a specific applied template.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task UpdateTemplateCommonOperationProperties(IConnectionApiClient conClient)
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

			//Read the current common properties of the template operations.
			var commonProperties = await conClient.Template.GetTemplateCommonOperationPropertiesAsync(conClient.ActiveProjectId, connectionId, templateId);

			//Set the plate material of all template operations to the first steel material in the project.
			List<MatSteel> steelMaterials = (await conClient.Material.GetSteelMaterialsAsync(conClient.ActiveProjectId)).Cast<MatSteel>().ToList();
			commonProperties.PlateMaterialId = steelMaterials[0].Id;

			await conClient.Template.UpdateTemplateCommonOperationPropertiesAsync(conClient.ActiveProjectId, connectionId, templateId, commonProperties);

			var updatedProperties = await conClient.Template.GetTemplateCommonOperationPropertiesAsync(conClient.ActiveProjectId, connectionId, templateId);
			Console.WriteLine($"Common properties of template {templateId} updated. Plate material Id: {updatedProperties.PlateMaterialId}");

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
