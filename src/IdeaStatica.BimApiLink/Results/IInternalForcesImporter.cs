using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.Results
{
	public interface IInternalForcesImporter<T>
		where T : IIdeaObjectWithResults
	{
		IEnumerable<ResultsData<T>> GetResults(IReadOnlyList<T> objects);
	}
}