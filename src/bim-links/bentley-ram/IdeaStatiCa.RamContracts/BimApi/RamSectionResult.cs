using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	public class RamSectionResult : IIdeaSectionResult
	{
		public RamSectionResult(IIdeaLoading loading, IIdeaResultData data)
		{
			Loading = loading;
			Data = data;
		}

		public IIdeaLoading Loading { get; }

		public IIdeaResultData Data { get; }
	}
}