using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// This example exports the connection to Idea Open Model (IOM).
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task ConnectionLibrarySearch(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/HSS_norm_cond.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			var connections = await conClient.Connection.GetConnectionsAsync(conClient.ActiveProjectId);
			int connectionId = connections[0].Id;

			//Define search parameters.
			ConConnectionLibrarySearchParameters librarySearch = new ConConnectionLibrarySearchParameters();

			//Library search can be null.

			//Search the library for applicable templates.
			List<ConConnectionLibraryItem> returnedItems = await conClient.ConnectionLibrary.Propose(conClient.ActiveProjectId, connectionId, librarySearch);

			//Add client helper method which can fetch the Template XML from the location.
			//Pick which library item you want. Get URL to the template (xml) location.
			ConTemplateMappingGetParam templateImport = conClient.Template.FetchTemplateFromPath(returnedItems[0].TemplatePath);

			//Similar Application from Here ->

			TemplateConversions conversionMapping = await conClient.Template.GetDefaultTemplateMappingAsync(conClient.ActiveProjectId, connectionId, templateImport);

			ConTemplateApplyParam applyParam = new ConTemplateApplyParam();

			applyParam.ConnectionTemplate = templateImport.Template;

			//TO DO: We can do some custom mapping if we would like to.
			applyParam.Mapping = conversionMapping;

			var result = conClient.Template.ApplyTemplateAsync(conClient.ActiveProjectId, connectionId, applyParam);

			//FIX Needs to output the Iom Model xml.
			await conClient.Export.ExportIomAsync(conClient.ActiveProjectId, connectionId);

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}

	public class ConConnectionLibrarySearchParameters
	{
		public List<int> Members { get; set; } = null;
		public string Search { get; set; } = "";

		public bool InPredefinedSet = true;
		public bool InCompanySet = true;
		public bool InPersonalSet = true;
		
		public bool? HasBolts = null; //"null / true / false"
		public bool? HasWelds = null;
		public bool? HasAnchors = null;
		public bool? HasClipAngles = null;
		public bool? IsMoment = null;
		public bool? IsShear = null;
		public bool? IsTruss = null;
		public bool? IsParameteric = null;
	}

	public class ConConnectionLibraryItem
	{
		public string Name { get; set; }
		public string AuthorName { get; set; }

		public bool IsParametric { get; set; }

		public Guid ItemId { get; set; }
			
		public string DesignCode { get; set; }

		public string Thumbnail { get; set; }

		public DateTime Created { get; set; }

		/// <summary>
		/// Example: "Predefined", "UserDefined", etc.
		/// </summary>
		public string DesignSet { get; set; }

		/// <summary>
		/// XML text defining the template for applying this connection.
		/// </summary>
		public string TemplatePath { get; set; } //This will result in a large pay load.
	}
}
