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
			string filePath = "inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get all cross-sections in the project. Items are polymorphic IOM cross-sections (e.g. CrossSectionParameter).
			List<IdeaRS.OpenModel.CrossSection.CrossSection> crossSections = (await conClient.Material.GetCrossSectionsAsync(conClient.ActiveProjectId)).Cast<IdeaRS.OpenModel.CrossSection.CrossSection>().ToList();

			Console.WriteLine("Cross-sections in the project: " + crossSections.Count);
			foreach (IdeaRS.OpenModel.CrossSection.CrossSection crossSection in crossSections)
			{
				Console.WriteLine($"Id: {crossSection.Id} Name: {crossSection.Name}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
