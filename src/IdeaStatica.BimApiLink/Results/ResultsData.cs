using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;

namespace IdeaStatica.BimApiLink.Results
{
#if NET6_0
	public record ResultsData<T>(T Object, IIdeaResult Results)
		where T : IIdeaObjectWithResults;
#else

	public class ResultsData<T>
		where T : IIdeaObjectWithResults
	{
		public T Object { get; }
		public IIdeaResult Results { get; }

		public ResultsData(T @object, IIdeaResult results)
		{
			Object = @object;
			Results = results;
		}
	}

#endif
}