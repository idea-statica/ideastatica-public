using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Results
{
	public interface IInternalForcesImporter<T>
		where T : IIdeaObjectWithResults
	{
		IEnumerable<ResultsData<T>> GetResults(IReadOnlyList<T> objects);
	}
}