using IdeaStatiCa.BimApiLink.Results;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Results;

namespace IdeaStatiCa.BimApiLink
{
	public sealed class ResultsImportersConfiguration
	{
		public IBimResultsProvider ResultsProvider => _adapter;

		private readonly BimResultsProviderAdapter _adapter = new BimResultsProviderAdapter();

		public ResultsImportersConfiguration RegisterImporter<T>(IInternalForcesImporter<T> importer)
			where T : IIdeaObjectWithResults
		{
			_adapter.RegisterImporter(importer);
			return this;
		}
	}
}