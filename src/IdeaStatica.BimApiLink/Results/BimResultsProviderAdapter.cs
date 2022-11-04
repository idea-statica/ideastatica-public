using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Results;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.Results
{
	internal class BimResultsProviderAdapter : IBimResultsProvider
	{
		private readonly List<IBimResultsProvider> _bimResultsProviders = new List<IBimResultsProvider>();

		public void RegisterImporter<T>(IInternalForcesImporter<T> importer)
			where T : IIdeaObjectWithResults
			=> _bimResultsProviders.Add(new InternalForcesImporterAdapter<T>(importer));

		public IEnumerable<ResultsData> GetResults(IEnumerable<IIdeaObjectWithResults> objects)
		{
			foreach (IBimResultsProvider provider in _bimResultsProviders)
			{
				foreach (ResultsData resultData in provider.GetResults(objects))
				{
					yield return resultData;
				}
			}
		}
	}
}