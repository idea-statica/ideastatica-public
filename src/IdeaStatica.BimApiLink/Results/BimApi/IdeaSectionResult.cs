using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;

namespace IdeaStatica.BimApiLink.Results.BimApi
{
	public class IdeaSectionResult : IIdeaSectionResult
	{
		public IIdeaLoading Loading { get; }

		public IIdeaResultData Data { get; }

		public IdeaSectionResult(IIdeaLoading loading, InternalForcesData data)
		{
			Loading = loading;
			Data = data;
		}
	}
}