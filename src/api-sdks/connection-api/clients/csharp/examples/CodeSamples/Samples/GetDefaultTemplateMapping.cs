using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example gets the default mapping for the application of a connection template on a connection.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetDefaultTemplateMapping(IConnectionApiClient conClient)
		{
			string filePath = "inputs/corner-empty.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			string templateFilePath = "inputs/template-I-corner.contemp";
			ConTemplateMappingGetParam templateImport = conClient.Template.ImportTemplateFromFile(templateFilePath);

			//Get the default mapping of the template items (members, cross-sections, materials, bolts) onto the connection.
			TemplateConversions mapping = await conClient.Template.GetDefaultTemplateMappingAsync(conClient.ActiveProjectId, connectionId, templateImport);

			Console.WriteLine("Default mapping conversions: " + mapping.Conversions.Count);
			foreach (BaseTemplateConversion conversion in mapping.Conversions)
			{
				Console.WriteLine($"{conversion.Description}: '{conversion.OriginalValue}' -> '{conversion.NewValue}'");
			}

			//The mapping can be modified and passed to Template.ApplyTemplateAsync in ConTemplateApplyParam.Mapping.

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
