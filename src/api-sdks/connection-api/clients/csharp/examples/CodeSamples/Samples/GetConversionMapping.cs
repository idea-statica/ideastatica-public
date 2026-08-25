using IdeaRS.OpenModel;
using IdeaStatiCa.Api.Connection.Model.Conversion;
using IdeaStatiCa.ConnectionApi;

namespace CodeSamples
{
	public partial class ClientExamples
	{
		/// <summary>
		/// Gets the default conversion mappings for converting an ECEN (Eurocode) project to a different design code.
		/// </summary>
		/// <param name="conClient">The connected API Client</param>
		public static async Task GetConversionMapping(IConnectionApiClient conClient)
		{
			//Only projects with the ECEN (Eurocode) design code can be converted.
			string filePath = "Inputs/simple cleat connection.ideaCon";
			await conClient.Project.OpenProjectAsync(filePath);

			//Get the default mapping of materials, cross-sections and fasteners for the conversion to AISC (American design code).
			ConConversionSettings conversionMapping = await conClient.Conversion.GetConversionMappingAsync(conClient.ActiveProjectId, CountryCode.American);

			Console.WriteLine($"Default conversion mapping to design code: {conversionMapping.TargetDesignCode}");
			Console.WriteLine($"Steel: {conversionMapping.Steel?.Count ?? 0}, Bolt grades: {conversionMapping.BoltGrade?.Count ?? 0}, " +
				$"Welds: {conversionMapping.Welds?.Count ?? 0}, Cross-sections: {conversionMapping.CrossSections?.Count ?? 0}");

			foreach (ConversionMapping steelMapping in conversionMapping.Steel)
			{
				Console.WriteLine($"Steel material: {steelMapping.SourceValue} -> {steelMapping.TargetValue}");
			}

			foreach (ConversionMapping crossSectionMapping in conversionMapping.CrossSections)
			{
				Console.WriteLine($"Cross-section: {crossSectionMapping.SourceValue} -> {crossSectionMapping.TargetValue}");
			}

			//Close the opened project.
			await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
		}
	}
}
