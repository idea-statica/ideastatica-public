using IdeaStatiCa.Api.Connection.Model.Material;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Get all cross-sections available in the project.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetCrossSections(IConnectionApiClient conClient)
		{
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all cross-sections in the project: id, name and how each is defined (library / parametric / custom).
			List<ConCrossSection> crossSections = await conClient.Material.GetCrossSectionsAsync(conClient.ActiveProjectId);

			Console.WriteLine("Cross-sections in the project: " + crossSections.Count);
			foreach (ConCrossSection crossSection in crossSections)
			{
				string definition = crossSection.Definition switch
				{
					ConCrossSectionLibraryDefinition library => $"library {library.MprlName}, material {library.MaterialName}",
					ConCrossSectionParametricDefinition parametric => $"parametric {parametric.ShapeType}, material {parametric.MaterialName}",
					ConCrossSectionCustomDefinition custom => $"custom, {custom.Components.Count} component(s)",
					_ => "unknown",
				};
				Console.WriteLine($"Id: {crossSection.Id} Name: {crossSection.Name} ({definition})");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
