using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Results
{
	public interface IBimResultsProvider
	{
		IEnumerable<ResultsData> GetResults(IEnumerable<IIdeaObjectWithResults> objects);
	}
}