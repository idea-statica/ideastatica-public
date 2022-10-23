using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;

namespace IdeaStatica.BimApiLink.Results
{
	public record ResultsData<T>(T Object, IIdeaResult Results)
		where T : IIdeaObjectWithResults;
}