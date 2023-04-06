using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.Results
{
	public interface IInternalForcesImporter<T>
		where T : IIdeaObjectWithResults
	{
		IEnumerable<ResultsData<T>> GetResults(IReadOnlyList<T> objects);
	}
}